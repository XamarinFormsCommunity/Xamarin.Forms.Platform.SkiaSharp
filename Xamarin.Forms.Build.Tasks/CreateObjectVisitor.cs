using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Xamarin.Forms.Xaml;
using System.Xml;

using static Mono.Cecil.Cil.Instruction;
using static Mono.Cecil.Cil.OpCodes;

namespace Xamarin.Forms.Build.Tasks
{
	class CreateObjectVisitor : IXamlNodeVisitor
	{
		public CreateObjectVisitor(ILContext context)
		{
			Context = context;
			Module = context.Body.Method.Module;
		}

		public ILContext Context { get; }

		ModuleDefinition Module { get; }

		public TreeVisitingMode VisitingMode => TreeVisitingMode.BottomUp;
		public bool StopOnDataTemplate => true;
		public bool StopOnResourceDictionary => false;
		public bool VisitNodeOnDataTemplate => false;
		public bool SkipChildren(INode node, INode parentNode) => false;

		public void Visit(ValueNode node, INode parentNode)
		{
			Context.Values[node] = node.Value;
		}

		public void Visit(MarkupNode node, INode parentNode)
		{
			//At this point, all MarkupNodes are expanded to ElementNodes
		}

		public void Visit(ElementNode node, INode parentNode)
		{
			var typeref = Module.ImportReference(node.XmlType.GetTypeReference(Module, node));
			TypeDefinition typedef = typeref.ResolveCached();

			if (IsXaml2009LanguagePrimitive(node)) {
				var vardef = new VariableDefinition(typeref);
				Context.Variables [node] = vardef;
				Context.Body.Variables.Add(vardef);

				Context.IL.Append(PushValueFromLanguagePrimitive(typedef, node));
				Context.IL.Emit(OpCodes.Stloc, vardef);
				return;
			}

			//if this is a MarkupExtension that can be compiled directly, compile and returns the value
			var compiledMarkupExtensionName = typeref
				.GetCustomAttribute(Module, ("Xamarin.Forms.Core", "Xamarin.Forms.Xaml", "ProvideCompiledAttribute"))
				?.ConstructorArguments?[0].Value as string;
			Type compiledMarkupExtensionType;
			ICompiledMarkupExtension markupProvider;
			if (compiledMarkupExtensionName != null &&
				(compiledMarkupExtensionType = Type.GetType(compiledMarkupExtensionName)) != null &&
				(markupProvider = Activator.CreateInstance(compiledMarkupExtensionType) as ICompiledMarkupExtension) != null) {

				var il = markupProvider.ProvideValue(node, Module, Context, out typeref);
				typeref = Module.ImportReference(typeref);

				var vardef = new VariableDefinition(typeref);
				Context.Variables[node] = vardef;
				Context.Body.Variables.Add(vardef);

				Context.IL.Append(il);
				Context.IL.Emit(OpCodes.Stloc, vardef);

				//clean the node as it has been fully exhausted
				foreach (var prop in node.Properties)
					if (!node.SkipProperties.Contains(prop.Key))
						node.SkipProperties.Add(prop.Key);
				node.CollectionItems.Clear();
				return;
			}

			MethodDefinition factoryCtorInfo = null;
			MethodDefinition factoryMethodInfo = null;
			MethodDefinition parameterizedCtorInfo = null;
			MethodDefinition ctorInfo = null;

			if (node.Properties.ContainsKey(XmlName.xArguments) && !node.Properties.ContainsKey(XmlName.xFactoryMethod)) {
				factoryCtorInfo = typedef.AllMethods().FirstOrDefault(md => md.IsConstructor &&
																			!md.IsStatic &&
																			md.HasParameters &&
																			md.MatchXArguments(node, typeref, Module, Context));
				if (factoryCtorInfo == null) {
					throw new XamlParseException(
						string.Format("No constructors found for {0} with matching x:Arguments", typedef.FullName), node);
				}
				ctorInfo = factoryCtorInfo;
				if (!typedef.IsValueType) //for ctor'ing typedefs, we first have to ldloca before the params
					Context.IL.Append(PushCtorXArguments(factoryCtorInfo, node));
			} else if (node.Properties.ContainsKey(XmlName.xFactoryMethod)) {
				var factoryMethod = (string)(node.Properties [XmlName.xFactoryMethod] as ValueNode).Value;
				factoryMethodInfo = typedef.AllMethods().FirstOrDefault(md => !md.IsConstructor &&
																			  md.Name == factoryMethod &&
																			  md.IsStatic &&
																			  md.MatchXArguments(node, typeref, Module, Context));
				if (factoryMethodInfo == null) {
					throw new XamlParseException(
						String.Format("No static method found for {0}::{1} ({2})", typedef.FullName, factoryMethod, null), node);
				}
				Context.IL.Append(PushCtorXArguments(factoryMethodInfo, node));
			}
			if (ctorInfo == null && factoryMethodInfo == null) {
				parameterizedCtorInfo = typedef.Methods.FirstOrDefault(md => md.IsConstructor &&
																			 !md.IsStatic &&
																			 md.HasParameters &&
																			 md.Parameters.All(
																				 pd =>
																					 pd.CustomAttributes.Any(
																						 ca =>
																							 ca.AttributeType.FullName ==
																							 "Xamarin.Forms.ParameterAttribute")));
			}
			string missingCtorParameter = null;
			if (parameterizedCtorInfo != null && ValidateCtorArguments(parameterizedCtorInfo, node, out missingCtorParameter)) {
				ctorInfo = parameterizedCtorInfo;
//				IL_0000:  ldstr "foo"
				Context.IL.Append(PushCtorArguments(parameterizedCtorInfo, node));
			}
			ctorInfo = ctorInfo ?? typedef.Methods.FirstOrDefault(md => md.IsConstructor && !md.HasParameters && !md.IsStatic);
			if (parameterizedCtorInfo != null && ctorInfo == null)
				//there was a parameterized ctor, we didn't use it
				throw new XamlParseException($"The Property '{missingCtorParameter}' is required to create a '{typedef.FullName}' object.", node);
			var ctorinforef = ctorInfo?.ResolveGenericParameters(typeref, Module);
			var factorymethodinforef = factoryMethodInfo?.ResolveGenericParameters(typeref, Module);
			var implicitOperatorref = typedef.Methods.FirstOrDefault(md =>
				md.IsPublic &&
				md.IsStatic &&
				md.IsSpecialName &&
				md.Name == "op_Implicit" && md.Parameters [0].ParameterType.FullName == "System.String");

			if (ctorinforef != null || factorymethodinforef != null || typedef.IsValueType) {
				VariableDefinition vardef = new VariableDefinition(typeref);
				Context.Variables [node] = vardef;
				Context.Body.Variables.Add(vardef);

				ValueNode vnode = null;
				if (node.CollectionItems.Count == 1 && (vnode = node.CollectionItems.First() as ValueNode) != null &&
					vardef.VariableType.IsValueType) {
					//<Color>Purple</Color>
					Context.IL.Append(vnode.PushConvertedValue(Context, typeref, new ICustomAttributeProvider [] { typedef },
						node.PushServiceProvider(Context), false, true));
					Context.IL.Emit(OpCodes.Stloc, vardef);
				} else if (node.CollectionItems.Count == 1 && (vnode = node.CollectionItems.First() as ValueNode) != null &&
						   implicitOperatorref != null) {
					//<FileImageSource>path.png</FileImageSource>
					var implicitOperator = Module.ImportReference(implicitOperatorref);
					Context.IL.Emit(OpCodes.Ldstr, ((ValueNode)(node.CollectionItems.First())).Value as string);
					Context.IL.Emit(OpCodes.Call, implicitOperator);
					Context.IL.Emit(OpCodes.Stloc, vardef);
				} else if (factorymethodinforef != null) {
					Context.IL.Emit(OpCodes.Call, Module.ImportReference(factorymethodinforef));
					Context.IL.Emit(OpCodes.Stloc, vardef);
				} else if (!typedef.IsValueType) {
					var ctor = Module.ImportReference(ctorinforef);
//					IL_0001:  newobj instance void class [Xamarin.Forms.Core]Xamarin.Forms.Button::'.ctor'()
//					IL_0006:  stloc.0 
					Context.IL.Emit(OpCodes.Newobj, ctor);
					Context.IL.Emit(OpCodes.Stloc, vardef);
				} else if (ctorInfo != null && node.Properties.ContainsKey(XmlName.xArguments) &&
						   !node.Properties.ContainsKey(XmlName.xFactoryMethod) && ctorInfo.MatchXArguments(node, typeref, Module, Context)) {
//					IL_0008:  ldloca.s 1
//					IL_000a:  ldc.i4.1 
//					IL_000b:  call instance void valuetype Test/Foo::'.ctor'(bool)

					var ctor = Module.ImportReference(ctorinforef);
					Context.IL.Emit(OpCodes.Ldloca, vardef);
					Context.IL.Append(PushCtorXArguments(factoryCtorInfo, node));
					Context.IL.Emit(OpCodes.Call, ctor);
				} else {
//					IL_0000:  ldloca.s 0
//					IL_0002:  initobj Test/Foo
					Context.IL.Emit(OpCodes.Ldloca, vardef);
					Context.IL.Emit(OpCodes.Initobj, Module.ImportReference(typedef));
				}

				if (typeref.FullName == "Xamarin.Forms.Xaml.ArrayExtension") {
					var visitor = new SetPropertiesVisitor(Context);
					foreach (var cnode in node.Properties.Values.ToList())
						cnode.Accept(visitor, node);
					foreach (var cnode in node.CollectionItems)
						cnode.Accept(visitor, node);

					markupProvider = new ArrayExtension();

					var il = markupProvider.ProvideValue(node, Module, Context, out typeref);

					vardef = new VariableDefinition(typeref);
					Context.Variables[node] = vardef;
					Context.Body.Variables.Add(vardef);

					Context.IL.Append(il);
					Context.IL.Emit(OpCodes.Stloc, vardef);

					//clean the node as it has been fully exhausted
					foreach (var prop in node.Properties)
						if (!node.SkipProperties.Contains(prop.Key))
							node.SkipProperties.Add(prop.Key);
					node.CollectionItems.Clear();

					return;
				}
			}
		}

		public void Visit(RootNode node, INode parentNode)
		{
//			IL_0013:  ldarg.0 
//			IL_0014:  stloc.3 

			var ilnode = (ILRootNode)node;
			var typeref = ilnode.TypeReference;
			var vardef = new VariableDefinition(typeref);
			Context.Variables[node] = vardef;
			Context.Root = vardef;
			Context.Body.Variables.Add(vardef);
			Context.IL.Emit(OpCodes.Ldarg_0);
			Context.IL.Emit(OpCodes.Stloc, vardef);
		}

		public void Visit(ListNode node, INode parentNode)
		{
			XmlName name;
			if (SetPropertiesVisitor.TryGetPropertyName(node, parentNode, out name))
				node.XmlName = name;
		}

		bool ValidateCtorArguments(MethodDefinition ctorinfo, ElementNode enode, out string firstMissingProperty)
		{
			firstMissingProperty = null;
			foreach (var parameter in ctorinfo.Parameters)
			{
				var propname =
					parameter.CustomAttributes.First(ca => ca.AttributeType.FullName == "Xamarin.Forms.ParameterAttribute")
						.ConstructorArguments.First()
						.Value as string;
				if (!enode.Properties.ContainsKey(new XmlName("", propname))) {
					firstMissingProperty = propname;
					return false;
				}
			}
			return true;
		}

		IEnumerable<Instruction> PushCtorArguments(MethodDefinition ctorinfo, ElementNode enode)
		{
			foreach (var parameter in ctorinfo.Parameters)
			{
				var propname =
					parameter.CustomAttributes.First(ca => ca.AttributeType.FullName == "Xamarin.Forms.ParameterAttribute")
						.ConstructorArguments.First()
						.Value as string;
				var node = enode.Properties[new XmlName("", propname)];
				if (!enode.SkipProperties.Contains(new XmlName("", propname)))
					enode.SkipProperties.Add(new XmlName("", propname));
				VariableDefinition vardef;
				ValueNode vnode = null;

				if (node is IElementNode && (vardef = Context.Variables[node as IElementNode]) != null)
					yield return Instruction.Create(OpCodes.Ldloc, vardef);
				else if ((vnode = node as ValueNode) != null)
				{
					foreach (var instruction in vnode.PushConvertedValue(Context,
						parameter.ParameterType,
						new ICustomAttributeProvider[] { parameter, parameter.ParameterType.ResolveCached() },
						enode.PushServiceProvider(Context), false, true))
						yield return instruction;
				}
			}
		}

		IEnumerable<Instruction> PushCtorXArguments(MethodDefinition factoryCtorInfo, ElementNode enode)
		{
			if (!enode.Properties.ContainsKey(XmlName.xArguments))
				yield break;

			var arguments = new List<INode>();
			var node = enode.Properties[XmlName.xArguments] as ElementNode;
			if (node != null)
				arguments.Add(node);
			var list = enode.Properties[XmlName.xArguments] as ListNode;
			if (list != null)
			{
				foreach (var n in list.CollectionItems)
					arguments.Add(n);
			}

			for (var i = 0; i < factoryCtorInfo.Parameters.Count; i++)
			{
				var parameter = factoryCtorInfo.Parameters[i];
				var arg = arguments[i];
				VariableDefinition vardef;
				ValueNode vnode = null;

				if (arg is IElementNode && (vardef = Context.Variables[arg as IElementNode]) != null)
					yield return Instruction.Create(OpCodes.Ldloc, vardef);
				else if ((vnode = arg as ValueNode) != null)
				{
					foreach (var instruction in vnode.PushConvertedValue(Context,
						parameter.ParameterType,
						new ICustomAttributeProvider[] { parameter, parameter.ParameterType.ResolveCached() },
						enode.PushServiceProvider(Context), false, true))
						yield return instruction;
				}
			}
		}

		static bool IsXaml2009LanguagePrimitive(IElementNode node)
		{
			if (node.NamespaceURI == XamlParser.X2009Uri) {
				var n = node.XmlType.Name.Split(':') [1];
				return n != "Array";
			}
			if (node.NamespaceURI != "clr-namespace:System;assembly=mscorlib")
				return false;
			var name = node.XmlType.Name.Split(':')[1];
			if (name == "SByte" ||
				name == "Int16" ||
				name == "Int32" ||
				name == "Int64" ||
				name == "Byte" ||
				name == "UInt16" ||
				name == "UInt32" ||
				name == "UInt64" ||
				name == "Single" ||
				name == "Double" ||
				name == "Boolean" ||
				name == "String" ||
				name == "Char" ||
				name == "Decimal" ||
				name == "TimeSpan" ||
				name == "Uri")
				return true;
			return false;
		}

		IEnumerable<Instruction> PushValueFromLanguagePrimitive(TypeDefinition typedef, ElementNode node)
		{
			var module = Context.Body.Method.Module;
			var hasValue = node.CollectionItems.Count == 1 && node.CollectionItems[0] is ValueNode &&
			               ((ValueNode)node.CollectionItems[0]).Value is string;
			var valueString = hasValue ? ((ValueNode)node.CollectionItems[0]).Value as string : string.Empty;
			switch (typedef.FullName) {
			case "System.SByte":
				if (hasValue && sbyte.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out sbyte outsbyte))
					yield return Create(Ldc_I4, (int)outsbyte);
				else
					yield return Create(Ldc_I4, 0x00);
				break;
			case "System.Int16":
				if (hasValue && short.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out short outshort))
					yield return Create(Ldc_I4, outshort);
				else
					yield return Create(Ldc_I4, 0x00);
				break;
			case "System.Int32":
				if (hasValue && int.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out int outint))
					yield return Create(Ldc_I4, outint);
				else
					yield return Create(Ldc_I4, 0x00);
				break;
			case "System.Int64":
				if (hasValue && long.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out long outlong))
					yield return Create(Ldc_I8, outlong);
				else
					yield return Create(Ldc_I8, 0L);
				break;
			case "System.Byte":
				if (hasValue && byte.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out byte outbyte))
					yield return Create(Ldc_I4, (int)outbyte);
				else
					yield return Create(Ldc_I4, 0x00);
				break;
			case "System.UInt16":
				if (hasValue && short.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out short outushort))
					yield return Create(Ldc_I4, outushort);
				else
					yield return Create(Ldc_I4, 0x00);
				break;
			case "System.UInt32":
				if (hasValue && int.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out int outuint))
					yield return Create(Ldc_I4, outuint);
				else
					yield return Create(Ldc_I4, 0x00);
				break;
			case "System.UInt64":
				if (hasValue && long.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out long outulong))
					yield return Create(Ldc_I8, outulong);
				else
					yield return Create(Ldc_I8, 0L);
				break;
			case "System.Boolean":
				if (hasValue && bool.TryParse(valueString, out bool outbool))
					yield return Create(outbool ? Ldc_I4_1 : Ldc_I4_0);
				else
					yield return Create(Ldc_I4_0);
				break;
			case "System.String":
				yield return Create(Ldstr, valueString);
				break;
			case "System.Object":
				var ctorinfo =
					module.TypeSystem.Object.ResolveCached()
						.Methods.FirstOrDefault(md => md.IsConstructor && !md.HasParameters);
				var ctor = module.ImportReference(ctorinfo);
				yield return Create(Newobj, ctor);
				break;
			case "System.Char":
				if (hasValue && char.TryParse(valueString, out char outchar))
					yield return Create(Ldc_I4, outchar);
				else
					yield return Create(Ldc_I4, 0x00);
				break;
			case "System.Decimal":
				decimal outdecimal;
				if (hasValue && decimal.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out outdecimal)) {
					var vardef = new VariableDefinition(module.ImportReference(("mscorlib", "System", "Decimal")));
					Context.Body.Variables.Add(vardef);
					//Use an extra temp var so we can push the value to the stack, just like other cases
//					IL_0003:  ldstr "adecimal"
//					IL_0008:  ldc.i4.s 0x6f
//					IL_000a:  call class [mscorlib]System.Globalization.CultureInfo class [mscorlib]System.Globalization.CultureInfo::get_InvariantCulture()
//					IL_000f:  ldloca.s 0
//					IL_0011:  call bool valuetype [mscorlib]System.Decimal::TryParse(string, valuetype [mscorlib]System.Globalization.NumberStyles, class [mscorlib]System.IFormatProvider, [out] valuetype [mscorlib]System.Decimal&)
//					IL_0016:  pop
					yield return Create(Ldstr, valueString);
					yield return Create(Ldc_I4, 0x6f); //NumberStyles.Number
					var getInvariant = module.ImportPropertyGetterReference(("mscorlib", "System.Globalization", "CultureInfo"),
																			propertyName: "InvariantCulture");
					yield return Create(Call, getInvariant);
					yield return Create(Ldloca, vardef);
					var tryParse = module.ImportMethodReference(("mscorlib", "System", "Decimal"),
																methodName: "TryParse",
																paramCount: 4);
					yield return Create(Call, tryParse);
					yield return Create(Pop);
					yield return Create(Ldloc, vardef);
				} else {
					yield return Create(Ldc_I4_0);
					var decimalctor = module.ImportCtorReference(("mscorlib", "System", "Decimal"),
																 paramCount: 1,
																 predicate: md => md.Parameters[0].ParameterType.FullName == "System.Int32");
					yield return Create(Newobj, decimalctor);
				}
				break;
			case "System.Single":
				if (hasValue && float.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out float outfloat))
					yield return Create(Ldc_R4, outfloat);
				else
					yield return Create(Ldc_R4, 0f);
				break;
			case "System.Double":
				if (hasValue && double.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out double outdouble))
					yield return Create(Ldc_R8, outdouble);
				else
					yield return Create(Ldc_R8, 0d);
				break;
			case "System.TimeSpan":
				if (hasValue && TimeSpan.TryParse(valueString, CultureInfo.InvariantCulture, out TimeSpan outspan)) {
					var vardef = new VariableDefinition(module.ImportReference(("mscorlib", "System", "TimeSpan")));
					Context.Body.Variables.Add(vardef);
					//Use an extra temp var so we can push the value to the stack, just like other cases
					yield return Create(Ldstr, valueString);
					var getInvariant = module.ImportPropertyGetterReference(("mscorlib", "System.Globalization", "CultureInfo"),
																			propertyName: "InvariantCulture");
					yield return Create(Call, getInvariant);
					yield return Create(Ldloca, vardef);
					var tryParse = module.ImportMethodReference(("mscorlib", "System", "TimeSpan"),
																methodName: "TryParse",
																paramCount: 3);

					yield return Create(Call, tryParse);
					yield return Create(Pop);
					yield return Create(Ldloc, vardef);
				} else {
					yield return Create(Ldc_I8, 0L);
					var timespanctor = module.ImportCtorReference(("mscorlib", "System", "TimeSpan"),
																  paramCount: 1,
																  predicate: md => md.Parameters[0].ParameterType.FullName == "System.Int64");
					yield return Create(Newobj, timespanctor);
				}
				break;
			case "System.Uri":
				if (hasValue && Uri.TryCreate(valueString, UriKind.RelativeOrAbsolute, out Uri outuri)) {
					var vardef = new VariableDefinition(module.ImportReference(("System", "System", "Uri")));
					Context.Body.Variables.Add(vardef);
					//Use an extra temp var so we can push the value to the stack, just like other cases
					yield return Create(Ldstr, valueString);
					yield return Create(Ldc_I4, (int)UriKind.RelativeOrAbsolute);
					yield return Create(Ldloca, vardef);
					var tryCreate = module.ImportMethodReference(("System", "System", "Uri"),
																 methodName: "TryCreate",
																 paramCount: 3);
					yield return Create(Call, tryCreate);
					yield return Create(Pop);
					yield return Create(Ldloc, vardef);
				} else
					yield return Create(Ldnull);
				break;
			default:
				var defaultCtor = module.ImportCtorReference(typedef, paramCount: 0);
				if (defaultCtor != null)
					yield return Create(Newobj, defaultCtor);
				else {
					//should never happen. but if it does, this prevents corrupting the IL stack
					yield return Create(Ldnull);
				}
				break;
			}
		}
	}
}