﻿using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CSharp;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Build.Tasks
{
	class XamlGenerator
	{
		internal XamlGenerator()
		{
		}

		public XamlGenerator(
			ITaskItem taskItem,
			string language,
			string assemblyName,
			string outputFile,
			TaskLoggingHelper logger)
			: this(
				taskItem.ItemSpec,
				language,
				taskItem.GetMetadata("ManifestResourceName"),
				taskItem.GetMetadata("TargetPath"),
				assemblyName,
				outputFile,
				logger)
		{
		}

		static int generatedTypesCount;
		internal static CodeDomProvider Provider = new CSharpCodeProvider();

		public string XamlFile { get; }
		public string Language { get; }
		public string ResourceId { get; }
		public string TargetPath { get; }
		public string AssemblyName { get; }
		public string OutputFile { get; }
		public TaskLoggingHelper Logger { get; }
		public string RootClrNamespace { get; private set; }
		public string RootType { get; private set; }
		bool GenerateDefaultCtor { get; set; }
		bool AddXamlCompilationAttribute { get; set; }
		bool HideFromIntellisense { get; set; }
		bool XamlResourceIdOnly { get; set; }
		internal IEnumerable<CodeMemberField> NamedFields { get; set; }
		internal CodeTypeReference BaseType { get; set; }

		public XamlGenerator(
			string xamlFile,
			string language,
			string resourceId,
			string targetPath,
			string assemblyName,
			string outputFile,
			TaskLoggingHelper logger = null)
		{
			XamlFile = xamlFile;
			Language = language;
			ResourceId = resourceId;
			TargetPath = targetPath;
			AssemblyName = assemblyName;
			OutputFile = outputFile;
			Logger = logger;
		}

		//returns true if a file is generated
		public bool Execute()
		{
			Logger?.LogMessage(MessageImportance.Low, "Source: {0}", XamlFile);
			Logger?.LogMessage(MessageImportance.Low, " Language: {0}", Language);
			Logger?.LogMessage(MessageImportance.Low, " ResourceID: {0}", ResourceId);
			Logger?.LogMessage(MessageImportance.Low, " TargetPath: {0}", TargetPath);
			Logger?.LogMessage(MessageImportance.Low, " AssemblyName: {0}", AssemblyName);
			Logger?.LogMessage(MessageImportance.Low, " OutputFile {0}", OutputFile);

			using (StreamReader reader = File.OpenText(XamlFile))
				if (!ParseXaml(reader))
					return false;

			GenerateCode();

			return true;
		}

		internal bool ParseXaml(TextReader xaml)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(xaml);

			// if the following xml processing instruction is present
			//
			// <?xaml-comp compile="true" ?>
			//
			// we will generate a xaml.g.cs file with the default ctor calling InitializeComponent, and a XamlCompilation attribute
			var hasXamlCompilationProcessingInstruction = GetXamlCompilationProcessingInstruction(xmlDoc);

			var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
			nsmgr.AddNamespace("__f__", XamlParser.XFUri);

			var root = xmlDoc.SelectSingleNode("/*", nsmgr);
			if (root == null) {
				Logger?.LogMessage(MessageImportance.Low, " No root node found");
				return false;
			}

			foreach (XmlAttribute attr in root.Attributes) {
				if (attr.Name == "xmlns")
					nsmgr.AddNamespace("", attr.Value); //Add default xmlns
				if (attr.Prefix != "xmlns")
					continue;
				nsmgr.AddNamespace(attr.LocalName, attr.Value);
			}

			var rootClass = root.Attributes["Class", XamlParser.X2006Uri]
						 ?? root.Attributes["Class", XamlParser.X2009Uri];

			if (rootClass != null) {
				string rootType, rootNs, rootAsm, targetPlatform;
				XmlnsHelper.ParseXmlns(rootClass.Value, out rootType, out rootNs, out rootAsm, out targetPlatform);
				RootType = rootType;
				RootClrNamespace = rootNs;
			}
			else if (hasXamlCompilationProcessingInstruction) {
				RootClrNamespace = "__XamlGeneratedCode__";
				RootType = $"__Type{generatedTypesCount++}";
				GenerateDefaultCtor = true;
				AddXamlCompilationAttribute = true;
				HideFromIntellisense = true;
			}
			else { // rootClass == null && !hasXamlCompilationProcessingInstruction) {
				XamlResourceIdOnly = true; //only generate the XamlResourceId assembly attribute
				return true;
			}

			NamedFields = GetCodeMemberFields(root, nsmgr);
			var typeArguments = GetAttributeValue(root, "TypeArguments", XamlParser.X2006Uri, XamlParser.X2009Uri);
			var xmlType = new XmlType(root.NamespaceURI, root.LocalName, typeArguments != null ? TypeArgumentsParser.ParseExpression(typeArguments, nsmgr, null) : null);
			BaseType = GetType(xmlType, root.GetNamespaceOfPrefix);

			return true;
		}

		static Version version = typeof(XamlGenerator).Assembly.GetName().Version;
		static CodeAttributeDeclaration GeneratedCodeAttrDecl =>
			new CodeAttributeDeclaration(new CodeTypeReference($"global::{typeof(GeneratedCodeAttribute).FullName}"),
						new CodeAttributeArgument(new CodePrimitiveExpression("Xamarin.Forms.Build.Tasks.XamlG")),
						new CodeAttributeArgument(new CodePrimitiveExpression($"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}")));

		void GenerateCode()
		{
			//Create the target directory if required
			Directory.CreateDirectory(Path.GetDirectoryName(OutputFile));

			var ccu = new CodeCompileUnit();
			ccu.AssemblyCustomAttributes.Add(
				new CodeAttributeDeclaration(new CodeTypeReference($"global::{typeof(XamlResourceIdAttribute).FullName}"),
											 new CodeAttributeArgument(new CodePrimitiveExpression(ResourceId)),
											 new CodeAttributeArgument(new CodePrimitiveExpression(TargetPath.Replace('\\', '/'))), //use forward slashes, paths are uris-like
											 new CodeAttributeArgument(RootType == null ? (CodeExpression)new CodePrimitiveExpression(null) : new CodeTypeOfExpression($"global::{RootClrNamespace}.{RootType}"))
											));
			if (XamlResourceIdOnly)
				goto writeAndExit;

			if (RootType == null)
				throw new Exception("Something went wrong while executing XamlG");

			var declNs = new CodeNamespace(RootClrNamespace);
			ccu.Namespaces.Add(declNs);

			var declType = new CodeTypeDeclaration(RootType) {
				IsPartial = true,
				CustomAttributes = {
					new CodeAttributeDeclaration(new CodeTypeReference($"global::{typeof(XamlFilePathAttribute).FullName}"),
						 new CodeAttributeArgument(new CodePrimitiveExpression(XamlFile))),
				}
			};
			if (AddXamlCompilationAttribute)
				declType.CustomAttributes.Add(
					new CodeAttributeDeclaration(new CodeTypeReference($"global::{typeof(XamlCompilationAttribute).FullName}"),
												 new CodeAttributeArgument(new CodeSnippetExpression($"global::{typeof(XamlCompilationOptions).FullName}.Compile"))));
			if (HideFromIntellisense)
				declType.CustomAttributes.Add(
					new CodeAttributeDeclaration(new CodeTypeReference($"global::{typeof(System.ComponentModel.EditorBrowsableAttribute).FullName}"),
												 new CodeAttributeArgument(new CodeSnippetExpression($"global::{typeof(System.ComponentModel.EditorBrowsableState).FullName}.{nameof(System.ComponentModel.EditorBrowsableState.Never)}"))));

			declType.BaseTypes.Add(BaseType);

			declNs.Types.Add(declType);

			//Create a default ctor calling InitializeComponent
			if (GenerateDefaultCtor) {
				var ctor = new CodeConstructor {
					Attributes = MemberAttributes.Public,
					CustomAttributes = { GeneratedCodeAttrDecl },
					Statements = {
						new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "InitializeComponent")
					}
				};

				declType.Members.Add(ctor);
			}

			//Create InitializeComponent()
			var initcomp = new CodeMemberMethod {
				Name = "InitializeComponent",
				CustomAttributes = { GeneratedCodeAttrDecl }
			};

			declType.Members.Add(initcomp);

			//Create and initialize fields
			initcomp.Statements.Add(new CodeMethodInvokeExpression(
				new CodeTypeReferenceExpression(new CodeTypeReference($"global::{typeof(Extensions).FullName}")),
				"LoadFromXaml", new CodeThisReferenceExpression(), new CodeTypeOfExpression(declType.Name)));

			foreach (var namedField in NamedFields) {
				declType.Members.Add(namedField);

				var find_invoke = new CodeMethodInvokeExpression(
					new CodeMethodReferenceExpression(
						new CodeTypeReferenceExpression(new CodeTypeReference($"global::{typeof(NameScopeExtensions).FullName}")),
						"FindByName", namedField.Type),
					new CodeThisReferenceExpression(), new CodePrimitiveExpression(namedField.Name));

				CodeAssignStatement assign = new CodeAssignStatement(
					new CodeVariableReferenceExpression(namedField.Name), find_invoke);

				initcomp.Statements.Add(assign);
			}

		writeAndExit:
			//write the result
			using (var writer = new StreamWriter(OutputFile))
				Provider.GenerateCodeFromCompileUnit(ccu, writer, new CodeGeneratorOptions());
		}

		static IEnumerable<CodeMemberField> GetCodeMemberFields(XmlNode root, XmlNamespaceManager nsmgr)
		{
			var xPrefix = nsmgr.LookupPrefix(XamlParser.X2006Uri) ?? nsmgr.LookupPrefix(XamlParser.X2009Uri);
			if (xPrefix == null)
				yield break;

			XmlNodeList names =
				root.SelectNodes(
				"//*[@" + xPrefix + ":Name" +
					"][not(ancestor:: __f__:DataTemplate) and not(ancestor:: __f__:ControlTemplate) and not(ancestor:: __f__:Style) and not(ancestor:: __f__:VisualStateManager.VisualStateGroups)]", nsmgr);
			foreach (XmlNode node in names) {
				// Don't take the root canvas
				if (node == root)
					continue;
				var name = GetAttributeValue(node, "Name", XamlParser.X2006Uri, XamlParser.X2009Uri);
				var typeArguments = GetAttributeValue(node, "TypeArguments", XamlParser.X2006Uri, XamlParser.X2009Uri);
				var fieldModifier = GetAttributeValue(node, "FieldModifier", XamlParser.X2006Uri, XamlParser.X2009Uri);

				var xmlType = new XmlType(node.NamespaceURI, node.LocalName,
										  typeArguments != null
										  ? TypeArgumentsParser.ParseExpression(typeArguments, nsmgr, null)
										  : null);

				var access = MemberAttributes.Private;
				if (fieldModifier != null) {
					switch (fieldModifier.ToLowerInvariant()) {
						default:
						case "private":
							access = MemberAttributes.Private;
							break;
						case "public":
							access = MemberAttributes.Public;
							break;
						case "protected":
							access = MemberAttributes.Family;
							break;
						case "internal":
						case "notpublic": //WPF syntax
							access = MemberAttributes.Assembly;
							break;
					}
				}

				yield return new CodeMemberField {
					Name = name,
					Type = GetType(xmlType, node.GetNamespaceOfPrefix),
					Attributes = access,
					CustomAttributes = { GeneratedCodeAttrDecl }
				};
			}
		}

		static bool GetXamlCompilationProcessingInstruction(XmlDocument xmlDoc)
		{
			var instruction = xmlDoc.SelectSingleNode("processing-instruction('xaml-comp')") as XmlProcessingInstruction;
			if (instruction == null)
				return false;

			var parts = instruction.Data.Split(' ', '=');
			string compileValue = null;
			var indexOfCompile = Array.IndexOf(parts, "compile");
			if (indexOfCompile != -1)
				compileValue = parts[indexOfCompile + 1].Trim('"', '\'');
			return compileValue.Equals("true", StringComparison.InvariantCultureIgnoreCase);
		}

		static CodeTypeReference GetType(XmlType xmlType,
			Func<string, string> getNamespaceOfPrefix = null)
		{
			var type = xmlType.Name;
			var ns = GetClrNamespace(xmlType.NamespaceUri);
			if (ns != null)
				type = $"{ns}.{type}";

			if (xmlType.TypeArguments != null)
				type = $"{type}`{xmlType.TypeArguments.Count}";

			var returnType = new CodeTypeReference(type);
			if (ns != null)
				returnType.Options |= CodeTypeReferenceOptions.GlobalReference;

			if (xmlType.TypeArguments != null)
				foreach (var typeArg in xmlType.TypeArguments)
					returnType.TypeArguments.Add(GetType(typeArg, getNamespaceOfPrefix));

			return returnType;
		}

		static string GetClrNamespace(string namespaceuri)
		{
			if (namespaceuri == XamlParser.XFUri)
				return "Xamarin.Forms";
			if (namespaceuri == XamlParser.X2009Uri)
				return "System";
			if (namespaceuri != XamlParser.X2006Uri && !namespaceuri.Contains("clr-namespace"))
				throw new Exception($"Can't load types from xmlns {namespaceuri}");
			return XmlnsHelper.ParseNamespaceFromXmlns(namespaceuri);
		}

		static string GetAttributeValue(XmlNode node, string localName, params string[] namespaceURIs)
		{
			if (node == null)
				throw new ArgumentNullException(nameof(node));
			if (localName == null)
				throw new ArgumentNullException(nameof(localName));
			if (namespaceURIs == null)
				throw new ArgumentNullException(nameof(namespaceURIs));
			foreach (var namespaceURI in namespaceURIs) {
				var attr = node.Attributes[localName, namespaceURI];
				if (attr == null)
					continue;
				return attr.Value;
			}
			return null;
		}
	}
}
