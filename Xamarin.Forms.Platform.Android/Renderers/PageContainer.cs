using Android.Content;
using Android.Views;

namespace Xamarin.Forms.Platform.Android
{
	internal class PageContainer : ViewGroup
	{
		public PageContainer(Context context, IVisualElementRenderer child, bool inFragment = false) : base(context)
		{
			AddView(child.View);
			Child = child;
			IsInFragment = inFragment;
			Id = Platform.GenerateViewId();
		}

		public IVisualElementRenderer Child { get; set; }

		public bool IsInFragment { get; set; }

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			Child.UpdateLayout();
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			Child.View.Measure(widthMeasureSpec, heightMeasureSpec);
			SetMeasuredDimension(Child.View.MeasuredWidth, Child.View.MeasuredHeight);
		}
	}
}