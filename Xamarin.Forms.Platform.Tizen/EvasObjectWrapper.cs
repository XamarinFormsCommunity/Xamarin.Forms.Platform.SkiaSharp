using ElmSharp;
using ESize = ElmSharp.Size;

namespace Xamarin.Forms.Platform.Tizen
{
	public delegate ESize? MeasureDelegate(EvasObjectWrapperRenderer renderer, int availableWidth, int availableHeight);

	public class EvasObjectWrapper : View
	{
		public EvasObjectWrapper(EvasObject obj, MeasureDelegate measureDelegate = null)
		{
			EvasObject = obj;
			MeasureDelegate = measureDelegate;

			obj.TransferBindablePropertiesToWrapper(this);
		}

		public EvasObject EvasObject
		{
			get;
			private set;
		}

		public MeasureDelegate MeasureDelegate { get; }

		protected override void OnBindingContextChanged()
		{
			// TODO: we should provide a delegate to obtain children of a Container object,
			//       however currently there is no way to get the list of children
			EvasObject.SetBindingContext(BindingContext);
			base.OnBindingContextChanged();
		}
	}
}
