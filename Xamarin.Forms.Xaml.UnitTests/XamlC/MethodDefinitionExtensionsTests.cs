﻿using System;
using System.Linq;

using Mono.Cecil;

using Xamarin.Forms.Build.Tasks;

using NUnit.Framework;
using System.Collections.Generic;

namespace Xamarin.Forms.Xaml.XamlcUnitTests
{
	[TestFixture]
	public class MethodDefinitionExtensionsTests
	{
		public class NonGenericClass
		{
			public object Property { get; set; }
		}

		public class GenericClass<T, U, V>
		{
			public object NonGeneric() => default(object);
			public T GenericT() => default(T);
			public U GenericU() => default(U);
			public V GenericV() => default(V);
			public IEnumerable<T> EnumerableT() => default(IEnumerable<T>);
			public KeyValuePair<V, U> KvpVU() => default(KeyValuePair<V,U>);
		}

		ModuleDefinition module;

		[SetUp]
		public void SetUp()
		{
			module = ModuleDefinition.CreateModule("foo", ModuleKind.Dll);
		}

		[Test]
		public void ResolveGenericReturnType()
		{
			var type = module.ImportReference(typeof(GenericClass<bool, string, int>));

			var getter = type.GetMethods(md => md.Name == "NonGeneric", module).Single();
			var returnType = getter.Item1.ResolveGenericReturnType(getter.Item2, module);
			Assert.AreEqual("System.Object", returnType.FullName);

			getter = type.GetMethods(md => md.Name == "GenericT", module).Single();
			returnType = getter.Item1.ResolveGenericReturnType(getter.Item2, module);
			Assert.AreEqual("System.Boolean", returnType.FullName);

			getter = type.GetMethods(md => md.Name == "GenericU", module).Single();
			returnType = getter.Item1.ResolveGenericReturnType(getter.Item2, module);
			Assert.AreEqual("System.String", returnType.FullName);

			getter = type.GetMethods(md => md.Name == "GenericV", module).Single();
			returnType = getter.Item1.ResolveGenericReturnType(getter.Item2, module);
			Assert.AreEqual("System.Int32", returnType.FullName);
		}
	}
}