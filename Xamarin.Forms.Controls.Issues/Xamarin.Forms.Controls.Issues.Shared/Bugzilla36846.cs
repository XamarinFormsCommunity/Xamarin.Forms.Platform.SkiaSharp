﻿using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

// Apply the default category of "Issues" to all of the tests in this assembly
// We use this as a catch-all for tests which haven't been individually categorized
#if UITEST
[assembly: NUnit.Framework.Category("Issues")]
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 36846, "ActionBar does not dismiss when content which called it is removed", PlatformAffected.Android)]
	public class Bugzilla36846 : TestNavigationPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			PushAsync(new ListWithLongPress());
		}
	}

	public class ListWithLongPress : ContentPage
	{
		public ObservableCollection<string> MyCollection { get; set; }

		public ListWithLongPress()
		{
			MyCollection = new ObservableCollection<string>();
			PopulateCollection();

			var stackLayout = new StackLayout
			{
				Orientation = StackOrientation.Vertical
			};

			var listView = new ListView
			{
				HasUnevenRows = true,
				ItemsSource = MyCollection,
				ItemTemplate = new DataTemplate(() =>
				{
					var viewCell = new ViewCell();

					var grid = new Grid
					{
						Padding = new Thickness(0, 5, 0, 5),
						RowSpacing = 3
					};

					var label = new Label();
					label.SetBinding(Label.TextProperty, new Binding("."));
					grid.Children.Add(label);

					viewCell.ContextActions.Add(new MenuItem { Text = "Edit" });
					viewCell.ContextActions.Add(new MenuItem { Text = "Delete", IsDestructive = true });

					viewCell.View = grid;

					return viewCell;
				})
			};
			stackLayout.Children.Add(listView);

			var button1 = new Button
			{
				Text = "Clear list",
				Command = new Command(() => { MyCollection.Clear(); })
			};
			stackLayout.Children.Add(button1);

			var button2 = new Button
			{
				Text = "Remove last item",
				Command = new Command(() =>
				{
					if (MyCollection.Count > 0)
						MyCollection.RemoveAt(MyCollection.Count - 1);
				})
			};
			stackLayout.Children.Add(button2);

			var button3 = new Button
			{
				Text = "Load items",
				Command = new Command(PopulateCollection)
			};
			stackLayout.Children.Add(button3);

			Content = stackLayout;
		}

		void PopulateCollection()
		{
			for (var i = 0; i < 10; i++)
			{
				MyCollection.Add("This is a Dummy Item #" + i);
			}
		}
	}
}