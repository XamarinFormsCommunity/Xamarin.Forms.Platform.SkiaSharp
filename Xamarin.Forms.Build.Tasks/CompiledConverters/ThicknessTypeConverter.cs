using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Forms.Xaml;
using Xamarin.Forms.Build.Tasks;

namespace Xamarin.Forms.Core.XamlC
{
	class ThicknessTypeConverter : ICompiledTypeConverter
	{
		public IEnumerable<Instruction> ConvertFromString(string value, ILContext context, BaseNode node)
		{
			var module = context.Body.Method.Module;

			if (!string.IsNullOrEmpty(value)) {
				double l, t, r, b;
				var thickness = value.Split(',');
				switch (thickness.Length) {
				case 1:
					if (double.TryParse(thickness[0], NumberStyles.Number, CultureInfo.InvariantCulture, out l))
						return GenerateIL(module, l);
					break;
				case 2:
					if (double.TryParse(thickness[0], NumberStyles.Number, CultureInfo.InvariantCulture, out l) &&
					    double.TryParse(thickness[1], NumberStyles.Number, CultureInfo.InvariantCulture, out t))
						return GenerateIL(module, l, t);
					break;
				case 4:
					if (double.TryParse(thickness[0], NumberStyles.Number, CultureInfo.InvariantCulture, out l) &&
					    double.TryParse(thickness[1], NumberStyles.Number, CultureInfo.InvariantCulture, out t) &&
						double.TryParse(thickness[2], NumberStyles.Number, CultureInfo.InvariantCulture, out r) &&
					    double.TryParse(thickness[3], NumberStyles.Number, CultureInfo.InvariantCulture, out b))
						return GenerateIL(module, l, t, r, b);
					break;
				}
			}
			throw new XamlParseException($"Cannot convert \"{value}\" into {typeof(Thickness)}", node);
		}

		IEnumerable<Instruction> GenerateIL(ModuleDefinition module, params double[] args)
		{
			foreach (var d in args)
				yield return Instruction.Create(OpCodes.Ldc_R8, d);
			yield return Instruction.Create(OpCodes.Newobj, module.ImportCtorReference(("Xamarin.Forms.Core", "Xamarin.Forms", "Thickness"),
																					   paramCount: args.Length));
		}
	}
	
}