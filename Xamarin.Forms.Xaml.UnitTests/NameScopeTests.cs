using NUnit.Framework;

using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{

	[TestFixture]
	public class NameScopeTests : BaseTestFixture
	{
		[Test]
		public void TopLevelObjectsHaveANameScope ()
		{
			var xaml = @"
				<View 
				xmlns=""http://xamarin.com/schemas/2014/forms""
				xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" />";

			var view = new CustomView ().LoadFromXaml (xaml);

			Assert.IsNotNull (Forms.Internals.NameScope.GetNameScope (view));
			Assert.That (Forms.Internals.NameScope.GetNameScope (view), Is.TypeOf<Forms.Internals.NameScope> ());
		}

		[Test]
		public void NameScopeAreSharedWithChildren ()
		{
			var xaml = @"
				<StackLayout 
				xmlns=""http://xamarin.com/schemas/2014/forms""
				xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" >
					<Label />
					<Label />
				</StackLayout>";

			var layout = new StackLayout ().LoadFromXaml (xaml);

			Assert.IsNotNull (Forms.Internals.NameScope.GetNameScope (layout));
			Assert.That (Forms.Internals.NameScope.GetNameScope (layout), Is.TypeOf<Forms.Internals.NameScope> ());

			foreach (var child in layout.Children) {
				Assert.IsNotNull (Forms.Internals.NameScope.GetNameScope (child));
				Assert.That (Forms.Internals.NameScope.GetNameScope (child), Is.TypeOf<Forms.Internals.NameScope> ());
				Assert.AreSame (Forms.Internals.NameScope.GetNameScope (layout), Forms.Internals.NameScope.GetNameScope (child));
			}
		}

		[Test]
		public void DataTemplateChildrenDoesNotParticipateToParentNameScope ()
		{
			var xaml = @"
				<ListView
				xmlns=""http://xamarin.com/schemas/2014/forms""
				xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
				x:Name=""listview"">
					<ListView.ItemTemplate>
						<DataTemplate>
						    <TextCell Text=""{Binding name}"" x:Name=""textcell""/>
						</DataTemplate>
					</ListView.ItemTemplate>
				</ListView>";

			var listview = new ListView ();
			listview.LoadFromXaml (xaml);	

			Assert.AreSame (listview, ((Forms.Internals.INameScope)listview).FindByName ("listview"));
			Assert.IsNull (((Forms.Internals.INameScope)listview).FindByName ("textcell"));
		}

		[Test]
		public void ElementsCreatedFromDataTemplateHaveTheirOwnNameScope ()
		{
			var xaml = @"
				<ListView
				xmlns=""http://xamarin.com/schemas/2014/forms""
				xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
				x:Name=""listview"">
					<ListView.ItemTemplate>
						<DataTemplate>
						    <TextCell Text=""{Binding name}"" x:Name=""textcell""/>
						</DataTemplate>
					</ListView.ItemTemplate>
				</ListView>";

			var listview = new ListView ();
			listview.LoadFromXaml (xaml);	
			Assert.IsNotNull (Forms.Internals.NameScope.GetNameScope (listview));
			Assert.That (Forms.Internals.NameScope.GetNameScope (listview), Is.TypeOf<Forms.Internals.NameScope> ());

			var cell0 = listview.ItemTemplate.CreateContent () as Element;
			var cell1 = listview.ItemTemplate.CreateContent () as Element;

			Assert.IsNotNull (Forms.Internals.NameScope.GetNameScope (cell0));
			Assert.That (Forms.Internals.NameScope.GetNameScope (cell0), Is.TypeOf<Forms.Internals.NameScope> ());
			Assert.IsNotNull (Forms.Internals.NameScope.GetNameScope (cell1));
			Assert.That (Forms.Internals.NameScope.GetNameScope (cell1), Is.TypeOf<Forms.Internals.NameScope> ());

			Assert.AreNotSame (Forms.Internals.NameScope.GetNameScope (listview), Forms.Internals.NameScope.GetNameScope (cell0));
			Assert.AreNotSame (Forms.Internals.NameScope.GetNameScope (listview), Forms.Internals.NameScope.GetNameScope (cell1));
			Assert.AreNotSame (Forms.Internals.NameScope.GetNameScope (cell0), Forms.Internals.NameScope.GetNameScope (cell1));

			Assert.IsNull (((Forms.Internals.INameScope)listview).FindByName ("textcell"));
			Assert.NotNull (((Forms.Internals.INameScope)cell0).FindByName ("textcell"));
			Assert.NotNull (((Forms.Internals.INameScope)cell1).FindByName ("textcell"));

			Assert.AreNotSame (((Forms.Internals.INameScope)cell0).FindByName ("textcell"), ((Forms.Internals.INameScope)cell1).FindByName ("textcell"));

		}
	}
}
