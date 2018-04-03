﻿using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Build.Tasks
{
	interface ICompiledMarkupExtension
	{
		IEnumerable<Instruction> ProvideValue(IElementNode node, ModuleDefinition module, ILContext context, out TypeReference typeRef);
	}
}