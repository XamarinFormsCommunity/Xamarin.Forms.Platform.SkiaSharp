using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class GridTests : BaseTestFixture
	{
		[SetUp]
		public override void Setup()
		{
			base.Setup ();
			Device.PlatformServices = new MockPlatformServices ();
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown ();
			Device.PlatformServices = null;
		}

		[Test]
		public void ThrowsOnNullAdd ()
		{
			var layout = new Grid ();

			Assert.Throws<ArgumentNullException> (() => layout.Children.Add (null));
		}

		[Test]
		public void ThrowsOnNullRemove ()
		{
			var layout = new Grid ();

			Assert.Throws<ArgumentNullException> (() => layout.Children.Remove (null));
		}

		[TestFixture]
		public class AddDimension : GridTests
		{
			[Datapoints]
			public static IEnumerable<string> Operations = new[]
			{
				"HHH",
				"HHV",
				"HVH",
				"HVV",
				"VHH",
				"VHV",
				"VVH",
				"VVV",

				"RCRHVHVHVHVHV",

				"HHHV",
				"VVVH",

				"RV",
				"RH",
				"CV",
				"CH",

				"RVRRV",
				"CHCCH",
			};

			Grid _grid;

			int _id = 0;
			int _rowDef = 0;
			int _colDef = 0;
			int _totalWidth = 0;
			int _totalHeight = 0;

			public void AddHoizontal()
			{
				// new block gets new id
				var id = _id++;

				// adding column only increases height if no rows exist
				if (_totalHeight == 0)
					_totalHeight = 1;

				// adding column always increased width by 1
				_totalWidth++;

				// column spans rows 0 to the last row
				var row = 0;
				var height = _totalHeight;

				// column is always added at the end with a width of 1
				var column = _totalWidth - 1;
				var width = 1;

				_grid.Children.AddHorizontal(
					new Label()
					{
						Text = $"{id}: {column}x{row} {width}x{height}"
					}
				);
			}
			public void AddVertical()
			{
				// new block gets new id
				var id = _id++;

				// adding row only increases width if no columns exist
				if (_totalWidth == 0)
					_totalWidth = 1;

				// adding row always increased height by 1
				_totalHeight++;

				// row spans columns 0 to the last column
				var column = 0;
				var width = _totalWidth;

				// row is always added at the end with a height of 1
				var row = _totalHeight - 1;
				var height = 1;

				_grid.Children.AddVertical(
					new Label()
					{
						Text = $"{id}: {column}x{row} {width}x{height}"
					}
				);
			}
			public void AddRowDef()
			{
				_rowDef++;
				_totalHeight = Math.Max(_rowDef, _totalHeight);

				_grid.RowDefinitions.Add(new RowDefinition());
			}
			public void AddColumnDef()
			{
				_colDef++;
				_totalWidth = Math.Max(_colDef, _totalWidth);

				_grid.ColumnDefinitions.Add(new ColumnDefinition());
			}

			[TearDown]
			public override void TearDown()
			{
				_grid = null;

				_id = 0;
				_rowDef = 0;
				_colDef = 0;
				_totalWidth = 0;
				_totalHeight = 0;
			}

			[Theory]
			public void AddDimensionTheory(string operations)
			{
				_grid = new Grid();
				_grid.Platform = new UnitPlatform();

				foreach (var op in operations)
				{
					if (op == 'H')
						AddHoizontal();

					if (op == 'V')
						AddVertical();

					if (op == 'R')
						AddRowDef();

					if (op == 'C')
						AddColumnDef();

					_grid.Layout(new Rectangle(0, 0, 912, 912));
				}

				Console.WriteLine($"Operations: {string.Join(string.Empty, operations)}");

				var id = 0;
				foreach (var view in _grid.Children.Cast<Label>().OrderBy(o => o.Text))
				{
					var expected = $"{id++}: " +
						$"{Grid.GetColumn(view)}x{Grid.GetRow(view)} " +
						$"{Grid.GetColumnSpan(view)}x{Grid.GetRowSpan(view)}";

					var actual = view.Text;

					Console.WriteLine($"  {expected} == {actual}");
					Assert.That(expected == actual);
				}
			}
		}

		[Test]
		public void TestBasicVerticalLayout ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label3 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.Children.AddVertical (new View[] {
				label1,
				label2,
				label3
			});

			layout.Layout (new Rectangle (0, 0, 912, 912));

			Assert.AreEqual (912, layout.Width);
			Assert.AreEqual (912, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 912, 300), label1.Bounds);
			Assert.AreEqual (new Rectangle (0, 306, 912, 300), label2.Bounds);
			Assert.AreEqual (new Rectangle (0, 612, 912, 300), label3.Bounds);
		}

		[Test]
		public void TestBasicHorizontalLayout ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label3 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.Children.AddHorizontal (new View[] {
				label1,
				label2,
				label3
			});

			layout.Layout (new Rectangle (0, 0, 912, 912));

			Assert.AreEqual (912, layout.Width);
			Assert.AreEqual (912, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 300, 912), label1.Bounds);
			Assert.AreEqual (new Rectangle (306, 0, 300, 912), label2.Bounds);
			Assert.AreEqual (new Rectangle (612, 0, 300, 912), label3.Bounds);
		}

		[Test]
		public void TestVerticalExpandStart ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.RowDefinitions = new RowDefinitionCollection {
				new RowDefinition { Height = new GridLength (1, GridUnitType.Star) },
				new RowDefinition { Height = GridLength.Auto},
			};
			layout.Children.Add (label1, 0, 0);
			layout.Children.Add (label2, 0, 1);

			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (1000, layout.Width);
			Assert.AreEqual (1000, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 1000, 1000 - 20 - layout.RowSpacing), label1.Bounds);
			Assert.AreEqual (new Rectangle (0, 1000 - 20, 1000, 20), label2.Bounds);
		}

		[Test]
		public void TestHorizontalExpandStart ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label { Platform = platform, IsPlatformEnabled = true };
			var label2 = new Label { Platform = platform, IsPlatformEnabled = true };

			layout.ColumnDefinitions = new ColumnDefinitionCollection {
				new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) },
				new ColumnDefinition { Width = GridLength.Auto },
			};
			layout.Children.Add (label1, 0, 0);
			layout.Children.Add (label2, 1, 0);

			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (1000, layout.Width);
			Assert.AreEqual (1000, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 1000 - 106, 1000), label1.Bounds);
			Assert.AreEqual (new Rectangle (1000 - 100, 0, 100, 1000), label2.Bounds);
		}

		[Test]
		public void TestVerticalExpandEnd ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.RowDefinitions = new RowDefinitionCollection {
				new RowDefinition { Height = GridLength.Auto},
				new RowDefinition { Height = new GridLength (1, GridUnitType.Star) },
			};
			layout.Children.Add (label1, 0, 0);
			layout.Children.Add (label2, 0, 1);

			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (1000, layout.Width);
			Assert.AreEqual (1000, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 1000, 20), label1.Bounds);
			Assert.AreEqual (new Rectangle (0, 26, 1000, 1000 - 26), label2.Bounds);
		}

		[Test]
		public void TestHorizontalExpandEnd ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label { Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.ColumnDefinitions = new ColumnDefinitionCollection {
				new ColumnDefinition { Width = GridLength.Auto },
				new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) },
			};

			layout.Children.Add (label1, 0, 0);
			layout.Children.Add (label2, 1, 0);

			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (1000, layout.Width);
			Assert.AreEqual (1000, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 100, 1000), label1.Bounds);
			Assert.AreEqual (new Rectangle (106, 0, 1000 - 106, 1000), label2.Bounds);
		}

		[Test]
		public void TestVerticalExpandMiddle ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label3 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.RowDefinitions = new RowDefinitionCollection {
				new RowDefinition { Height = GridLength.Auto},
				new RowDefinition { Height = new GridLength (1, GridUnitType.Star) }, 
				new RowDefinition { Height = GridLength.Auto}
			};
			layout.Children.Add (label1, 0, 0);
			layout.Children.Add (label2, 0, 1);
			layout.Children.Add (label3, 0, 2);

			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (1000, layout.Width);
			Assert.AreEqual (1000, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 1000, 20), label1.Bounds);
			Assert.AreEqual (new Rectangle (0, 26, 1000, 1000 - 52), label2.Bounds);
			Assert.AreEqual (new Rectangle (0, 980, 1000, 20), label3.Bounds);
		}

		[Test]
		public void TestHorizontalExpandMiddle ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label3 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.ColumnDefinitions = new ColumnDefinitionCollection {
				new ColumnDefinition { Width = GridLength.Auto },
				new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) },
				new ColumnDefinition { Width = GridLength.Auto },
			};

			layout.Children.Add (label1, 0, 0);
			layout.Children.Add (label2, 1, 0);
			layout.Children.Add (label3, 2, 0);

			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (1000, layout.Width);
			Assert.AreEqual (1000, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 100, 1000), label1.Bounds);
			Assert.AreEqual (new Rectangle (106, 0, 1000 - 212, 1000), label2.Bounds);
			Assert.AreEqual (new Rectangle (900, 0, 100, 1000), label3.Bounds);
		}

		[Test]
		public void TestTableNoExpand ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label3 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label4 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.Children.Add (label1, 0, 0);
			layout.Children.Add (label2, 1, 0);
			layout.Children.Add (label3, 0, 1);
			layout.Children.Add (label4, 1, 1);

			layout.ColumnDefinitions = new ColumnDefinitionCollection {
				new ColumnDefinition { Width = GridLength.Auto },
				new ColumnDefinition { Width = GridLength.Auto },
			};
			layout.RowDefinitions = new RowDefinitionCollection {
				new RowDefinition { Height = GridLength.Auto},
				new RowDefinition { Height = GridLength.Auto}
			};

			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (1000, layout.Width);
			Assert.AreEqual (1000, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 100, 20), label1.Bounds);
			Assert.AreEqual (new Rectangle (106, 0, 100, 20), label2.Bounds);
			Assert.AreEqual (new Rectangle (0, 26, 100, 20), label3.Bounds);
			Assert.AreEqual (new Rectangle (106, 26, 100, 20), label4.Bounds);
		}

		[Test]
		public void TestTableExpand ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label3 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label4 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.ColumnDefinitions = new ColumnDefinitionCollection { 
				new ColumnDefinition { Width = GridLength.Auto },
				new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) },
			};

			layout.Children.Add (label1, 0, 0);
			layout.Children.Add (label2, 1, 0);
			layout.Children.Add (label3, 0, 1);
			layout.Children.Add (label4, 1, 1);

			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (1000, layout.Width);
			Assert.AreEqual (1000, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 100, 497), label1.Bounds);
			Assert.AreEqual (new Rectangle (106, 0, 894, 497), label2.Bounds);
			Assert.AreEqual (new Rectangle (0, 503, 100, 497), label3.Bounds);
			Assert.AreEqual (new Rectangle (106, 503, 894, 497), label4.Bounds);
		}

		[Test]
		public void TestTableSpan ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label3 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.Children.Add (label1, 0, 2, 0, 1);
			layout.Children.Add (label2, 0, 1, 1, 2);
			layout.Children.Add (label3, 1, 2, 1, 2);

			layout.ColumnDefinitions = new ColumnDefinitionCollection {
				new ColumnDefinition { Width = GridLength.Auto },
				new ColumnDefinition { Width = GridLength.Auto },
			};
			layout.RowDefinitions = new RowDefinitionCollection {
				new RowDefinition { Height = GridLength.Auto},
				new RowDefinition { Height = GridLength.Auto}
			};

			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (1000, layout.Width);
			Assert.AreEqual (1000, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 206, 20), label1.Bounds);
			Assert.AreEqual (new Rectangle (0, 26, 100, 20), label2.Bounds);
			Assert.AreEqual (new Rectangle (106, 26, 100, 20), label3.Bounds);
		}

		[Test]
		public void TestTableExpandedSpan ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label3 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.ColumnDefinitions = new ColumnDefinitionCollection { 
				new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) },
				new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) },
			};
			layout.RowDefinitions = new RowDefinitionCollection {
				new RowDefinition { Height = GridLength.Auto},
				new RowDefinition { Height = GridLength.Auto}
			};

			layout.Children.Add (label1, 0, 2, 0, 1);
			layout.Children.Add (label2, 0, 1, 1, 2);
			layout.Children.Add (label3, 1, 2, 1, 2);

			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (1000, layout.Width);
			Assert.AreEqual (1000, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 1000, 20), label1.Bounds);
			Assert.AreEqual (new Rectangle (0, 26, 497, 20), label2.Bounds);
			Assert.AreEqual (new Rectangle (503, 26, 497, 20), label3.Bounds);
		}

		[Test]
		public void TestInvalidSet ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};

			bool thrown = false;

			try {
				layout.Children.Add (label1, 2, 1, 0, 1);
			} catch (ArgumentOutOfRangeException) {
				thrown = true;
			}

			Assert.True (thrown);
		}

		[Test]
		public void TestCentering ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
			layout.ColumnDefinitions = new ColumnDefinitionCollection { 
				new ColumnDefinition () {Width = new GridLength (1, GridUnitType.Star)},
			};
			layout.RowDefinitions = new RowDefinitionCollection { 
				new RowDefinition () {Height = new GridLength (1,GridUnitType.Star)},
			};

			layout.Children.Add (label1);

			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (new Rectangle (450, 490, 100, 20), label1.Bounds);
		}

		[Test]
		public void TestStart ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label { Platform = platform, IsPlatformEnabled = true, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.StartAndExpand };

			layout.Children.AddVertical (label1);
			layout.ColumnDefinitions = new ColumnDefinitionCollection { 
				new ColumnDefinition () {Width = new GridLength (1, GridUnitType.Star)},
			};
			layout.RowDefinitions = new RowDefinitionCollection { 
				new RowDefinition () {Height = new GridLength (1,GridUnitType.Star)},
			};

			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (new Rectangle (0, 0, 100, 20), label1.Bounds);
		}

		[Test]
		public void TestEnd ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label { Platform = platform, IsPlatformEnabled = true, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.EndAndExpand };

			layout.Children.AddVertical (label1);
			layout.ColumnDefinitions = new ColumnDefinitionCollection { 
				new ColumnDefinition () {Width = new GridLength (1, GridUnitType.Star)},
			};
			layout.RowDefinitions = new RowDefinitionCollection { 
				new RowDefinition () {Height = new GridLength (1,GridUnitType.Star)},
			};

			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (new Rectangle (900, 980, 100, 20), label1.Bounds);
		}

		[Test]
		public void TestDefaultRowSpacing ()
		{
			var layout = new Grid ();

			bool preferredSizeChanged = false;
			layout.MeasureInvalidated += (sender, args) => {
				preferredSizeChanged = true;
			};

			layout.RowSpacing = layout.RowSpacing;

			Assert.False (preferredSizeChanged);

			layout.RowSpacing = 10;

			Assert.True (preferredSizeChanged);
		}

		[Test]
		public void TestDefaultColumnSpacing ()
		{
			var layout = new Grid ();

			bool preferredSizeChanged = false;
			layout.MeasureInvalidated += (sender, args) => {
				preferredSizeChanged = true;
			};

			layout.ColumnSpacing = layout.ColumnSpacing;

			Assert.False (preferredSizeChanged);

			layout.ColumnSpacing = 10;

			Assert.True (preferredSizeChanged);
		}

		[Test]
		public void TestAddCell ()
		{
			var layout = new Grid ();
			bool preferredSizeChanged = false;
			layout.MeasureInvalidated += (sender, args) => preferredSizeChanged = true;

			Assert.False (preferredSizeChanged);

			layout.Children.Add (new Label (), 0, 0);

			Assert.True (preferredSizeChanged);
		}

		[Test]
		public void TestMoveCell ()
		{
			var layout = new Grid ();
			var label = new Label ();
			layout.Children.Add (label, 0, 0);

			bool preferredSizeChanged = false;
			layout.MeasureInvalidated += (sender, args) => {
				preferredSizeChanged = true;
			};

			Assert.False (preferredSizeChanged);
			Grid.SetRow (label, 2);
			Assert.True (preferredSizeChanged);

			preferredSizeChanged = false;
			Assert.False (preferredSizeChanged);
			Grid.SetColumn (label, 2);
			Assert.True (preferredSizeChanged);

			preferredSizeChanged = false;
			Assert.False (preferredSizeChanged);
			Grid.SetRowSpan (label, 2);
			Assert.True (preferredSizeChanged);

			preferredSizeChanged = false;
			Assert.False (preferredSizeChanged);
			Grid.SetColumnSpan (label, 2);
			Assert.True (preferredSizeChanged);
		}

		[Test]
		public void TestInvalidBottomAdd ()
		{
			var layout = new Grid ();

			Assert.Throws<ArgumentOutOfRangeException> (() => layout.Children.Add (new View (), 0, 1, 1, 0));
		}

		[Test]
		public void TestZeroSizeConstraints ()
		{
			var layout = new Grid {Platform = new UnitPlatform ()};

			Assert.AreEqual (new Size (0, 0), layout.GetSizeRequest (0, 0).Request);
			Assert.AreEqual (new Size (0, 0), layout.GetSizeRequest (0, 10).Request);
			Assert.AreEqual (new Size (0, 0), layout.GetSizeRequest (10, 0).Request);
		}

		[Test]
		public void TestSizeRequest ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid {Platform = platform, IsPlatformEnabled = true};
			layout.Children.AddVertical (new[] {
				new View {Platform = platform, IsPlatformEnabled = true},
				new View {Platform = platform, IsPlatformEnabled = true},
				new View {Platform = platform, IsPlatformEnabled = true}
			});

			var result = layout.GetSizeRequest (double.PositiveInfinity, double.PositiveInfinity).Request;
			Assert.AreEqual (new Size (100, 72), result);
		}

		[Test]
		public void TestLimitedSizeRequest ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid {Platform = platform, IsPlatformEnabled = true};
			layout.Children.AddVertical (new[] {
				new View {Platform = platform, IsPlatformEnabled = true},
				new View {Platform = platform, IsPlatformEnabled = true},
				new View {Platform = platform, IsPlatformEnabled = true}
			});

			var result = layout.GetSizeRequest (10, 10).Request;
			Assert.AreEqual (new Size (100, 72), result);
		}

		[Test]
		public void TestLimitedWidthSizeRequest ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid {Platform = platform, IsPlatformEnabled = true};
			layout.Children.AddVertical (new[] {
				new View {Platform = platform, IsPlatformEnabled = true},
				new View {Platform = platform, IsPlatformEnabled = true},
				new View {Platform = platform, IsPlatformEnabled = true}
			});

			var result = layout.GetSizeRequest (10, double.PositiveInfinity).Request;
			Assert.AreEqual (new Size (100, 72), result);
		}

		[Test]
		public void TestLimitedHeightSizeRequest ()
		{

			var platform = new UnitPlatform ();
			var layout = new Grid {Platform = platform, IsPlatformEnabled = true};
			layout.Children.AddVertical (new[] {
				new View {Platform = platform, IsPlatformEnabled = true},
				new View {Platform = platform, IsPlatformEnabled = true},
				new View {Platform = platform, IsPlatformEnabled = true}
			});

			var result = layout.GetSizeRequest (double.PositiveInfinity, 10).Request;
			Assert.AreEqual (new Size (100, 72), result);
		}

		[Test]
		public void IgnoresInvisibleChildren ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid {Platform = platform};

			var label1 = new Label { Platform = platform, IsVisible = false, IsPlatformEnabled = true, VerticalOptions = LayoutOptions.FillAndExpand };
			var label2 = new Label { Platform = platform, IsPlatformEnabled = true};

			layout.Children.AddVertical (label1);
			layout.Children.AddVertical (label2);

			layout.ColumnDefinitions = new ColumnDefinitionCollection {
				new ColumnDefinition { Width = GridLength.Auto },
			};
			layout.RowDefinitions = new RowDefinitionCollection {
				new RowDefinition { Height = GridLength.Auto},
				new RowDefinition { Height = GridLength.Auto},
			};

			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (1000, layout.Width);
			Assert.AreEqual (1000, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, -1, -1), label1.Bounds);
			Assert.AreEqual (new Rectangle (0, 6, 100, 20), label2.Bounds);
		}

		[Test]
		public void TestSizeRequestWithPadding ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid {Platform = platform, IsPlatformEnabled = true, Padding = new Thickness(20, 10, 15, 5)};
			layout.Children.AddVertical (new[] {
				new View {Platform = platform, IsPlatformEnabled = true},
				new View {Platform = platform, IsPlatformEnabled = true},
				new View {Platform = platform, IsPlatformEnabled = true}
			});

			var result = layout.GetSizeRequest (double.PositiveInfinity, double.PositiveInfinity).Request;
			Assert.AreEqual (new Size (135, 87), result);
		}

		[Test]
		public void InvalidCallsToStaticMethods ()
		{
			Assert.Throws<ArgumentException> (() => Grid.SetRow (new Label (), -1));
			Assert.Throws<ArgumentException> (() => Grid.SetColumn (new Label (), -1));
			Assert.Throws<ArgumentException> (() => Grid.SetRowSpan (new Label (), 0));
			Assert.Throws<ArgumentException> (() => Grid.SetColumnSpan (new Label (), 0));
		}

		[Test]
		public void TestAddedBP ()
		{
			var platform = new UnitPlatform ();

			var labela0 = new Label {Platform = platform, IsPlatformEnabled = true};
			var labela1 = new Label { Platform = platform, IsPlatformEnabled = true };
			Grid.SetColumn (labela1, 1);
			var labelb1 = new Label {Platform = platform, IsPlatformEnabled = true};
			Grid.SetRow (labelb1, 1);
			Grid.SetColumn (labelb1, 1);
			var labelc = new Label {Platform = platform, IsPlatformEnabled = true};
			Grid.SetRow (labelc, 2);
			Grid.SetColumnSpan (labelc, 2);

			var layout = new Grid {
				Platform = platform,
				Children = {
					labela0,
					labela1,
					labelb1,
					labelc
				}
			};

			layout.ColumnDefinitions = new ColumnDefinitionCollection {
				new ColumnDefinition { Width = GridLength.Auto },
				new ColumnDefinition { Width = GridLength.Auto },
			};
			layout.RowDefinitions = new RowDefinitionCollection {
				new RowDefinition { Height = GridLength.Auto},
				new RowDefinition { Height = GridLength.Auto},
				new RowDefinition { Height = GridLength.Auto},
			};

			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (1000, layout.Width);
			Assert.AreEqual (1000, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 100, 20), labela0.Bounds);
			Assert.AreEqual (new Rectangle (106, 0, 100, 20), labela1.Bounds);
			Assert.AreEqual (new Rectangle (106, 26, 100, 20), labelb1.Bounds);
			Assert.AreEqual (new Rectangle (0, 52, 206, 20), labelc.Bounds);
		}

		[Test]
		public void Remove ()
		{
			var platform = new UnitPlatform();
			
			var labela0 = new Label { Platform = platform, IsPlatformEnabled = true };
			var labela1 = new Label { Platform = platform, IsPlatformEnabled = true };
			Grid.SetColumn(labela1, 1);
			var labelb1 = new Label { Platform = platform, IsPlatformEnabled = true };
			Grid.SetRow(labelb1, 1);
			Grid.SetColumn(labelb1, 1);
			var labelc = new Label { Platform = platform, IsPlatformEnabled = true };
			Grid.SetRow(labelc, 2);
			Grid.SetColumnSpan(labelc, 2);

			var layout = new Grid {
				Platform = platform,
				Children = {
					labela0,
					labela1,
					labelb1,
					labelc
				}
			};

			layout.Children.Remove (labela0);
			Assert.False (((IElementController)layout).LogicalChildren.Contains (labela0));
		}

		[Test]
		public void TestAbsoluteLayout ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label3 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.ColumnDefinitions = new ColumnDefinitionCollection { 
				new ColumnDefinition {Width = new GridLength (150)},
				new ColumnDefinition {Width = new GridLength (150)},
				new ColumnDefinition {Width = new GridLength (150)},
			};
			layout.RowDefinitions = new RowDefinitionCollection {
				new RowDefinition {Height = new GridLength (30)},
				new RowDefinition {Height = new GridLength (30)},
				new RowDefinition {Height = new GridLength (30)},
			};
			layout.Children.Add (label1, 0, 0);
			layout.Children.Add (label2, 1, 1);
			layout.Children.Add (label3, 2, 2);


			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (1000, layout.Width);
			Assert.AreEqual (1000, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 150, 30), label1.Bounds);
			Assert.AreEqual (new Rectangle (156, 36, 150, 30), label2.Bounds);
			Assert.AreEqual (new Rectangle (312, 72, 150, 30), label3.Bounds);
		}

		[Test]
		public void TestAbsoluteLayoutWithSpans ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label3 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.ColumnDefinitions = new ColumnDefinitionCollection { 
				new ColumnDefinition {Width = new GridLength (150)},
				new ColumnDefinition {Width = new GridLength (150)},
				new ColumnDefinition {Width = new GridLength (150)},
			};
			layout.RowDefinitions = new RowDefinitionCollection {
				new RowDefinition {Height = new GridLength (30)},
				new RowDefinition {Height = new GridLength (30)},
				new RowDefinition {Height = new GridLength (30)},
			};
			layout.Children.Add (label1, 0, 2, 0, 1);
			layout.Children.Add (label2, 2, 3, 0, 2);
			layout.Children.Add (label3, 1, 2);


			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (1000, layout.Width);
			Assert.AreEqual (1000, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 306, 30), label1.Bounds);
			Assert.AreEqual (new Rectangle (312, 0, 150, 66), label2.Bounds);
			Assert.AreEqual (new Rectangle (156, 72, 150, 30), label3.Bounds);
		}

		[Test]
		public void TestStarLayout ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label3 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.ColumnDefinitions = new ColumnDefinitionCollection { 
				new ColumnDefinition {Width = new GridLength (1, GridUnitType.Star)},
				new ColumnDefinition {Width = new GridLength (1, GridUnitType.Star)},
				new ColumnDefinition {Width = new GridLength (1, GridUnitType.Star)},
			};
			layout.RowDefinitions = new RowDefinitionCollection {
				new RowDefinition {Height = new GridLength (1, GridUnitType.Star)},
				new RowDefinition {Height = new GridLength (1, GridUnitType.Star)},
				new RowDefinition {Height = new GridLength (1, GridUnitType.Star)},
			};
			layout.Children.Add (label1, 0, 0);
			layout.Children.Add (label2, 1, 1);
			layout.Children.Add (label3, 2, 2);

			var request = layout.GetSizeRequest (1002, 462);
			Assert.AreEqual (312, request.Request.Width);
			Assert.AreEqual (72, request.Request.Height);

			layout.Layout (new Rectangle (0, 0, 1002, 462));
			Assert.AreEqual (1002, layout.Width);
			Assert.AreEqual (462, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 330, 150), label1.Bounds);
			Assert.AreEqual (new Rectangle (336, 156, 330, 150), label2.Bounds);
			Assert.AreEqual (new Rectangle (672, 312, 330, 150), label3.Bounds);
		}

		[Test]
		public void TestStarLayoutWithSpans ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label3 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.ColumnDefinitions = new ColumnDefinitionCollection { 
				new ColumnDefinition {Width = new GridLength (1, GridUnitType.Star)},
				new ColumnDefinition {Width = new GridLength (1, GridUnitType.Star)},
				new ColumnDefinition {Width = new GridLength (1, GridUnitType.Star)},
			};
			layout.RowDefinitions = new RowDefinitionCollection {
				new RowDefinition {Height = new GridLength (1, GridUnitType.Star)},
				new RowDefinition {Height = new GridLength (1, GridUnitType.Star)},
				new RowDefinition {Height = new GridLength (1, GridUnitType.Star)},
			};
			layout.Children.Add (label1, 0, 2, 0, 1);
			layout.Children.Add (label2, 2, 3, 0, 2);
			layout.Children.Add (label3, 1, 2);

			layout.Layout (new Rectangle (0, 0, 1002, 462));

			Assert.AreEqual (1002, layout.Width);
			Assert.AreEqual (462, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 666, 150), label1.Bounds);
			Assert.AreEqual (new Rectangle (672, 0, 330, 306), label2.Bounds);
			Assert.AreEqual (new Rectangle (336, 312, 330, 150), label3.Bounds);
		}

		[Test]
		public void TestAutoLayout ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label3 = new Label {Platform = platform, IsPlatformEnabled = true};

			layout.ColumnDefinitions = new ColumnDefinitionCollection { 
				new ColumnDefinition {Width = GridLength.Auto},
				new ColumnDefinition {Width = GridLength.Auto},
				new ColumnDefinition {Width = GridLength.Auto},
			};
			layout.RowDefinitions = new RowDefinitionCollection {
				new RowDefinition {Height = GridLength.Auto},
				new RowDefinition {Height = GridLength.Auto},
				new RowDefinition {Height = GridLength.Auto},
			};
			layout.Children.Add (label1, 0, 0);
			layout.Children.Add (label2, 1, 1);
			layout.Children.Add (label3, 2, 2);


			layout.Layout (new Rectangle (0, 0, 1000, 1000));

			Assert.AreEqual (1000, layout.Width);
			Assert.AreEqual (1000, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 100, 20), label1.Bounds);
			Assert.AreEqual (new Rectangle (106, 26, 100, 20), label2.Bounds);
			Assert.AreEqual (new Rectangle (212, 52, 100, 20), label3.Bounds);
		}

		[Test]
		public void TestAutoLayoutWithSpans ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label { Platform = platform, IsPlatformEnabled = true, WidthRequest = 150, Text = "label1" };
			var label2 = new Label { Platform = platform, IsPlatformEnabled = true, HeightRequest = 50, Text = "label2" };
			var label3 = new Label { Platform = platform, IsPlatformEnabled = true, Text = "label3" };

			layout.ColumnDefinitions = new ColumnDefinitionCollection { 
				new ColumnDefinition {Width = GridLength.Auto},
				new ColumnDefinition {Width = GridLength.Auto},
				new ColumnDefinition {Width = GridLength.Auto},
			};
			layout.RowDefinitions = new RowDefinitionCollection {
				new RowDefinition {Height = GridLength.Auto},
				new RowDefinition {Height = GridLength.Auto},
				new RowDefinition {Height = GridLength.Auto},
			};
			layout.Children.Add (label1, 0, 2, 0, 1);
			layout.Children.Add (label2, 2, 3, 0, 2);
			layout.Children.Add (label3, 1, 2);

			layout.Layout (new Rectangle (0, 0, 1002, 462));

			Assert.AreEqual (1002, layout.Width);
			Assert.AreEqual (462, layout.Height);

			Assert.AreEqual (new Rectangle (0, 0, 150, 20), label1.Bounds);
			Assert.AreEqual (new Rectangle (156, 0, 100, 50), label2.Bounds);
			Assert.AreEqual (new Rectangle (50, 56, 100, 20), label3.Bounds);
		}

		[Test]
		public void AutoLayoutWithComplexSpans ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label3 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label4 = new Label {Platform = platform, IsPlatformEnabled = true, WidthRequest = 206};
			var label5 = new Label {Platform = platform, IsPlatformEnabled = true, WidthRequest = 312};
			var label6 = new Label {Platform = platform, IsPlatformEnabled = true, WidthRequest = 312};

			layout.ColumnDefinitions = new ColumnDefinitionCollection { 
				new ColumnDefinition {Width = GridLength.Auto},
				new ColumnDefinition {Width = GridLength.Auto},
				new ColumnDefinition {Width = GridLength.Auto},
				new ColumnDefinition {Width = GridLength.Auto},
				new ColumnDefinition {Width = GridLength.Auto},
			};

			layout.Children.Add (label1, 0, 0);
			layout.Children.Add (label2, 1, 0);
			layout.Children.Add (label3, 4, 0);
			layout.Children.Add (label4, 2, 4, 0, 1);
			layout.Children.Add (label5, 0, 3, 0, 1);
			layout.Children.Add (label6, 2, 6, 0, 1);

			layout.Layout (new Rectangle (0, 0, 1000, 500));

			Assert.AreEqual (100, layout.ColumnDefinitions [0].ActualWidth);
			Assert.AreEqual (100, layout.ColumnDefinitions [1].ActualWidth);
			Assert.AreEqual (100, layout.ColumnDefinitions [2].ActualWidth);
			Assert.AreEqual (100, layout.ColumnDefinitions [3].ActualWidth);
			Assert.AreEqual (100, layout.ColumnDefinitions [4].ActualWidth);
		}

		[Test]
		public void AutoLayoutExpandColumns ()
		{
			var platform = new UnitPlatform ();
			var layout = new Grid ();
			layout.Platform = platform;

			var label1 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label2 = new Label {Platform = platform, IsPlatformEnabled = true};
			var label3 = new Label {Platform = platform, IsPlatformEnabled = true, WidthRequest = 300};

			layout.ColumnDefinitions = new ColumnDefinitionCollection { 
				new ColumnDefinition { Width = GridLength.Auto },
				new ColumnDefinition { Width = GridLength.Auto },
			};

			layout.Children.Add (label1, 0, 0);
			layout.Children.Add (label2, 1, 0);
			layout.Children.Add (label3, 0, 2, 0, 1);

			layout.Layout (new Rectangle (0, 0, 1000, 500));

			Assert.AreEqual (100, layout.ColumnDefinitions [0].ActualWidth);
			Assert.AreEqual (194, layout.ColumnDefinitions [1].ActualWidth);
		}

		[Test]
		public void GridHasDefaultDefinitions ()
		{
			var grid = new Grid ();
			Assert.NotNull (grid.ColumnDefinitions);
			Assert.NotNull (grid.RowDefinitions);
		}

		[Test]
		public void DefaultDefinitionsArentSharedAccrossInstances ()
		{
			var grid0 = new Grid ();
			var coldefs = grid0.ColumnDefinitions;
			var rowdefs = grid0.RowDefinitions;

			var grid1 = new Grid ();
			Assert.AreNotSame (grid0, grid1);
			Assert.AreNotSame (coldefs, grid1.ColumnDefinitions);
			Assert.AreNotSame (rowdefs, grid1.RowDefinitions);
		}

		[Test]
		public void ChildrenLayoutRespectAlignment ()
		{
			var platform = new UnitPlatform ();
			var grid = new Grid { 
				ColumnDefinitions = { new ColumnDefinition { Width = new GridLength (300) } },
				RowDefinitions = { new RowDefinition { Height = new GridLength (100) } },
				Platform = platform,
			};
			var label = new Label { 
				Platform = platform, 
				IsPlatformEnabled = true,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.End,
			};

			grid.Children.Add (label);
			grid.Layout (new Rectangle (0, 0, 500, 500));

			Assert.AreEqual (new Rectangle (200, 40, 100, 20), label.Bounds);
		}

		[Test]
		public void BothChildrenPropertiesUseTheSameBackendStore ()
		{
			var view = new View ();
			var grid = new Grid ();
			Assert.AreEqual (0, grid.Children.Count);
			(grid as Layout<View>).Children.Add (view);
			Assert.AreEqual (1, grid.Children.Count);
			Assert.AreEqual (1, (grid as Layout<View>).Children.Count);
			Assert.AreSame (view, (grid as Layout<View>).Children.First ());
			Assert.AreSame (view, grid.Children.First ());
		}

		[Test]
		//Issue 1384
		public void ImageInAutoCellIsProperlyConstrained ()
		{
			var platform = new UnitPlatform ();

			var content = new Image { 
				Aspect= Aspect.AspectFit,
				Platform = platform, 
				IsPlatformEnabled = true 
			};
			var grid = new Grid {
				Platform = platform, 
				IsPlatformEnabled = true,
				BackgroundColor = Color.Red, 
				VerticalOptions=LayoutOptions.Start,
				Children = {
					content
				},
				RowDefinitions = { new RowDefinition { Height = GridLength.Auto} },
				ColumnDefinitions = { new ColumnDefinition { Width = GridLength.Auto } }
			};
			var view = new ContentView {
				Platform = platform, 
				IsPlatformEnabled = true,
				Content = grid,
			};
			view.Layout (new Rectangle (0, 0, 100, 100));
			Assert.AreEqual (100, grid.Width);
			Assert.AreEqual (20, grid.Height);

			view.Layout (new Rectangle (0, 0, 50, 50));
			Assert.AreEqual (50, grid.Width);
			Assert.AreEqual (10, grid.Height);
		}

		[Test]
		//Issue 1384
		public void ImageInStarCellIsProperlyConstrained ()
		{
			var platform = new UnitPlatform ();

			var content = new Image { 
				Aspect= Aspect.AspectFit,
				Platform = platform,
				MinimumHeightRequest = 10,
				MinimumWidthRequest = 50,
				IsPlatformEnabled = true 
			};
			var grid = new Grid {
				Platform = platform, 
				IsPlatformEnabled = true,
				BackgroundColor = Color.Red, 
				VerticalOptions=LayoutOptions.Start,
				Children = {
					content
				}
			};
			var view = new ContentView {
				Platform = platform, 
				IsPlatformEnabled = true,
				Content = grid,
			};
			view.Layout (new Rectangle (0, 0, 100, 100));
			Assert.AreEqual (100, grid.Width);
			Assert.AreEqual (20, grid.Height);

			view.Layout (new Rectangle (0, 0, 50, 50));
			Assert.AreEqual (50, grid.Width);
			Assert.AreEqual (10, grid.Height);
		}

		[Test]
		public void SizeRequestForStar ()
		{
			var platform = new UnitPlatform ();

			var grid = new Grid{ 
				RowDefinitions = new RowDefinitionCollection {
					new RowDefinition {Height = new GridLength (1, GridUnitType.Star)},
					new RowDefinition {Height = GridLength.Auto},
				},
				ColumnDefinitions = new ColumnDefinitionCollection {
					new ColumnDefinition {Width = new GridLength (1, GridUnitType.Star)},
					new ColumnDefinition {Width = GridLength.Auto},
				}
			};
			grid.Children.Add (new Label {BackgroundColor = Color.Lime, Text="Foo", Platform = platform, IsPlatformEnabled = true});
			grid.Children.Add (new Label {Text = "Bar", Platform = platform, IsPlatformEnabled = true},0,1);
			grid.Children.Add (new Label {Text="Baz",XAlign = TextAlignment.End, Platform = platform, IsPlatformEnabled = true},1,0);
			grid.Children.Add (new Label {Text="Qux", XAlign = TextAlignment.End, Platform = platform, IsPlatformEnabled = true},1,1);

			var request = grid.GetSizeRequest (double.PositiveInfinity, double.PositiveInfinity);
			Assert.AreEqual (206, request.Request.Width);
			Assert.AreEqual (46, request.Request.Height);

			Assert.AreEqual (106, request.Minimum.Width);
			Assert.AreEqual (26, request.Minimum.Height);
			//
		}

		[Test]
		//Issue 1497
		public void StarRowsShouldOccupyTheSpace ()
		{
			var platform = new UnitPlatform ();
			var label = new Label { 
				Platform = platform,
				IsPlatformEnabled = true,
			};
			var Button = new Button {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.EndAndExpand,
				Platform = platform,
				IsPlatformEnabled = true,
			};
			var grid = new Grid {
				RowDefinitions = new RowDefinitionCollection {
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = new GridLength (1, GridUnitType.Star) },
				},
				ColumnDefinitions = new ColumnDefinitionCollection {
					new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
				},
				Platform = platform,
				IsPlatformEnabled = true,
			};

			grid.Children.Add (label);
			grid.Children.Add (Button, 0, 1);

			grid.Layout (new Rectangle (0, 0, 300, 300));
			Assert.AreEqual (new Rectangle (0, 280, 300, 20), Button.Bounds);
		}

		[Test]
		public void StarColumnsWithSpansDoNotExpandAutos ()
		{
			var grid = new Grid {
				RowDefinitions = {
					new RowDefinition {Height = GridLength.Auto},
					new RowDefinition {Height = GridLength.Auto},
				},
				ColumnDefinitions = {
					new ColumnDefinition {Width = new GridLength (1, GridUnitType.Auto)},
					new ColumnDefinition {Width = new GridLength (1, GridUnitType.Auto)},
					new ColumnDefinition {Width = new GridLength (1, GridUnitType.Star)}
				},
				Platform = new UnitPlatform (),
				IsPlatformEnabled = true
			};

			var spanBox = new BoxView {WidthRequest = 70, HeightRequest = 20, IsPlatformEnabled = true};
			var box1 = new BoxView {WidthRequest = 20, HeightRequest = 20, IsPlatformEnabled = true};
			var box2 = new BoxView {WidthRequest = 20, HeightRequest = 20, IsPlatformEnabled = true};
			var box3 = new BoxView {WidthRequest = 20, HeightRequest = 20, IsPlatformEnabled = true};

			grid.Children.Add (spanBox, 0, 3, 0, 1);
			grid.Children.Add (box1, 0, 1);
			grid.Children.Add (box2, 1, 1);
			grid.Children.Add (box3, 2, 1);

			grid.Layout (new Rectangle(0, 0, 300, 46));

			Assert.AreEqual (new Rectangle (0, 0, 300, 20), spanBox.Bounds);
			Assert.AreEqual (new Rectangle (0, 26, 20, 20), box1.Bounds);
			Assert.AreEqual (new Rectangle (26, 26, 20, 20), box2.Bounds);
			Assert.AreEqual (new Rectangle (52, 26, 248, 20), box3.Bounds);
		}

		static SizeRequest GetResizableSize (VisualElement view, double widthconstraint, double heightconstraint)
		{
			if (!(view is Editor))
				return new SizeRequest(new Size (100, 20));
			if (widthconstraint < 100)
				return new SizeRequest(new Size (widthconstraint, 2000/widthconstraint));
			if (heightconstraint < 20)
				return new SizeRequest(new Size (2000/heightconstraint, heightconstraint));
			return new SizeRequest(new Size (100, 20));
		}
			
		[Test]
		//Issue 1893
		public void EditorSpanningOnMultipleAutoRows ()
		{
			var grid0 = new Grid {
				ColumnDefinitions = {
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) },
				},
				RowDefinitions = {
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
				},
				Platform = new UnitPlatform (GetResizableSize),
				IsPlatformEnabled = true,
			};

			var label0 = new Label { IsPlatformEnabled = true };
			var editor0 = new Editor { IsPlatformEnabled = true };
			grid0.Children.Add (label0, 0, 0);
			grid0.Children.Add (editor0, 1, 2, 0, 2);

			grid0.Layout (new Rectangle (0, 0, 156, 200));
			Assert.AreEqual (new Rectangle (106, 0, 50, 40), editor0.Bounds);

			var grid1 = new Grid {
				ColumnDefinitions = {
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) },
				},
				RowDefinitions = {
					new RowDefinition { Height = GridLength.Auto },
				},
				Platform = new UnitPlatform (GetResizableSize),
				IsPlatformEnabled = true,
			};

			var label1 = new Label { IsPlatformEnabled = true };
			var editor1 = new Editor { IsPlatformEnabled = true };
			grid1.Children.Add (label1, 0, 0);
			grid1.Children.Add (editor1, 1, 0);

			grid1.Layout (new Rectangle (0, 0, 156, 200));
			Assert.AreEqual (new Rectangle (106, 0, 50, 40), editor1.Bounds);
		}

		[Test]
		public void WidthBoundRequestRespected ()
		{
			var grid = new Grid {
				ColumnDefinitions = {
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				},
				RowDefinitions = {
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
				},
				Platform = new UnitPlatform (GetResizableSize),
				IsPlatformEnabled = true,
				RowSpacing = 0,
				ColumnSpacing = 0,
			};

			var topLabel = new Editor {IsPlatformEnabled = true};
			var leftLabel = new Label {IsPlatformEnabled = true, WidthRequest = 10};
			var rightLabel = new Label {IsPlatformEnabled = true, WidthRequest = 10};

			grid.Children.Add (topLabel, 0, 2, 0, 1);
			grid.Children.Add (leftLabel, 0, 1);
			grid.Children.Add (rightLabel, 1, 1);

			var unboundRequest = grid.GetSizeRequest (double.PositiveInfinity, double.PositiveInfinity);
			var widthBoundRequest = grid.GetSizeRequest (50, double.PositiveInfinity);

			Assert.AreEqual (new SizeRequest (new Size (20, 120), new Size (0, 120)), unboundRequest);
			Assert.AreEqual (new SizeRequest (new Size (50, 60), new Size (0, 60)), widthBoundRequest);
		}

		[Test]
		//https://bugzilla.xamarin.com/show_bug.cgi?id=31608
		public void ColAndRowDefinitionsAreActuallyBindable ()
		{
			var rowdef = new RowDefinition ();
			rowdef.SetBinding (RowDefinition.HeightProperty, "Height");
			var grid = new Grid {
				RowDefinitions = new RowDefinitionCollection { rowdef },
			};
			Assert.AreEqual (RowDefinition.HeightProperty.DefaultValue, rowdef.Height);
			grid.BindingContext = new {Height = 32};
			Assert.AreEqual (new GridLength(32), rowdef.Height);
		}

		[Test]
		//https://bugzilla.xamarin.com/show_bug.cgi?id=31967
		public void ChangingRowHeightViaBindingTriggersRedraw ()
		{
			var rowdef = new RowDefinition ();
			rowdef.SetBinding (RowDefinition.HeightProperty, "Height");
			var grid = new Grid {
//				RowDefinitions = new RowDefinitionCollection {
//					new RowDefinition { Height = GridLength.Auto },
//					rowdef
//				},
				RowSpacing = 0,
				Platform = new UnitPlatform (),
				IsPlatformEnabled = true,
			};
			grid.RowDefinitions.Add (new RowDefinition { Height = GridLength.Auto });
			grid.RowDefinitions.Add (rowdef);

			var label0 = new Label { IsPlatformEnabled = true };
			Grid.SetRow (label0, 0);
			var label1 = new Label { IsPlatformEnabled = true };
			Grid.SetRow (label1, 1);

			grid.BindingContext = new {Height = 0};
			grid.Children.Add (label0);
			grid.Children.Add (label1);

			Assert.AreEqual (new SizeRequest (new Size (100, 20), new Size (0, 20)), grid.GetSizeRequest (double.PositiveInfinity, double.PositiveInfinity));
			grid.BindingContext = new {Height = 42};
			Assert.AreEqual (new SizeRequest (new Size (100, 62), new Size (0, 62)), grid.GetSizeRequest (double.PositiveInfinity, double.PositiveInfinity));
		}

		[Test]
		public void InvalidationBlockedForAbsoluteCell ()
		{
			var grid = new Grid () {
				Platform = new UnitPlatform (),
				IsPlatformEnabled = true,
				RowDefinitions = {
					new RowDefinition { Height = new GridLength (100, GridUnitType.Absolute) }
				},
				ColumnDefinitions = {
					new ColumnDefinition { Width = new GridLength (200, GridUnitType.Absolute) }
				}
			};

			var label = new Label { IsPlatformEnabled = true };
			grid.Children.Add (label);

			bool invalidated = false;
			grid.MeasureInvalidated += (sender, args) => {
				invalidated = true;
			};

			label.Text = "Testing";

			Assert.False (invalidated);
		}

		// because the constraint is internal, we need this
		public enum HackLayoutConstraint
		{
			None = LayoutConstraint.None,
			VerticallyFixed = LayoutConstraint.VerticallyFixed,
			HorizontallyFixed = LayoutConstraint.HorizontallyFixed,
			Fixed = LayoutConstraint.Fixed
		}

		[TestCase (HackLayoutConstraint.None, GridUnitType.Absolute, GridUnitType.Absolute, ExpectedResult = true)]
		[TestCase (HackLayoutConstraint.None, GridUnitType.Star, GridUnitType.Absolute, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.None, GridUnitType.Absolute, GridUnitType.Star, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.None, GridUnitType.Auto, GridUnitType.Absolute, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.None, GridUnitType.Absolute, GridUnitType.Auto, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.None, GridUnitType.Star, GridUnitType.Star, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.None, GridUnitType.Auto, GridUnitType.Star, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.None, GridUnitType.Star, GridUnitType.Auto, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.None, GridUnitType.Auto, GridUnitType.Auto, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.VerticallyFixed, GridUnitType.Absolute, GridUnitType.Absolute, ExpectedResult = true)]
		[TestCase (HackLayoutConstraint.VerticallyFixed, GridUnitType.Star, GridUnitType.Absolute, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.VerticallyFixed, GridUnitType.Absolute, GridUnitType.Star, ExpectedResult = true)]
		[TestCase (HackLayoutConstraint.VerticallyFixed, GridUnitType.Auto, GridUnitType.Absolute, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.VerticallyFixed, GridUnitType.Absolute, GridUnitType.Auto, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.VerticallyFixed, GridUnitType.Star, GridUnitType.Star, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.VerticallyFixed, GridUnitType.Auto, GridUnitType.Star, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.VerticallyFixed, GridUnitType.Star, GridUnitType.Auto, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.VerticallyFixed, GridUnitType.Auto, GridUnitType.Auto, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.HorizontallyFixed, GridUnitType.Absolute, GridUnitType.Absolute, ExpectedResult = true)]
		[TestCase (HackLayoutConstraint.HorizontallyFixed, GridUnitType.Star, GridUnitType.Absolute, ExpectedResult = true)]
		[TestCase (HackLayoutConstraint.HorizontallyFixed, GridUnitType.Absolute, GridUnitType.Star, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.HorizontallyFixed, GridUnitType.Auto, GridUnitType.Absolute, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.HorizontallyFixed, GridUnitType.Absolute, GridUnitType.Auto, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.HorizontallyFixed, GridUnitType.Star, GridUnitType.Star, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.HorizontallyFixed, GridUnitType.Auto, GridUnitType.Star, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.HorizontallyFixed, GridUnitType.Star, GridUnitType.Auto, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.HorizontallyFixed, GridUnitType.Auto, GridUnitType.Auto, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.Fixed, GridUnitType.Absolute, GridUnitType.Absolute, ExpectedResult = true)]
		[TestCase (HackLayoutConstraint.Fixed, GridUnitType.Star, GridUnitType.Absolute, ExpectedResult = true)]
		[TestCase (HackLayoutConstraint.Fixed, GridUnitType.Absolute, GridUnitType.Star, ExpectedResult = true)]
		[TestCase (HackLayoutConstraint.Fixed, GridUnitType.Auto, GridUnitType.Absolute, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.Fixed, GridUnitType.Absolute, GridUnitType.Auto, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.Fixed, GridUnitType.Star, GridUnitType.Star, ExpectedResult = true)]
		[TestCase (HackLayoutConstraint.Fixed, GridUnitType.Auto, GridUnitType.Star, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.Fixed, GridUnitType.Star, GridUnitType.Auto, ExpectedResult = false)]
		[TestCase (HackLayoutConstraint.Fixed, GridUnitType.Auto, GridUnitType.Auto, ExpectedResult = false)]
		public bool InvalidationPropogationTests (HackLayoutConstraint gridConstraint, GridUnitType horizontalType, GridUnitType verticalType)
		{
			var grid = new Grid {
				ComputedConstraint = (LayoutConstraint) gridConstraint,
				Platform = new UnitPlatform (),
				IsPlatformEnabled = true,
				RowDefinitions = {
					new RowDefinition { Height = new GridLength (1, verticalType) }
				},
				ColumnDefinitions = {
					new ColumnDefinition { Width = new GridLength (1, horizontalType) }
				}
			};

			var label = new Label { IsPlatformEnabled = true };
			grid.Children.Add (label);

			bool invalidated = false;
			grid.MeasureInvalidated += (sender, args) => {
				invalidated = true;
			};

			label.Text = "Testing";

			return !invalidated;
		}
	}

	[TestFixture]
	public class GridMeasureTests : BaseTestFixture
	{
		static List<Action> delayActions = new List<Action> ();

		[SetUp]
		public override void Setup()
		{
			base.Setup ();
			Device.PlatformServices = new MockPlatformServices (invokeOnMainThread: a => { delayActions.Add (a); });
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown ();
			Device.PlatformServices = null;
		}
		
		[Test]
		public void NestedInvalidateMeasureDoesNotCrash ()
		{
			var grid = new Grid {
				Platform = new UnitPlatform (),
				IsPlatformEnabled = true
			};

			var child = new Label {
				IsPlatformEnabled = true
			};
			grid.Children.Add (child);

			var child2 = new Label {
				IsPlatformEnabled = true
			};
			grid.Children.Add (child2);

			bool fire = true;
			child.SizeChanged += (sender, args) => {
				if (fire)
					((IVisualElementController)child).InvalidateMeasure(InvalidationTrigger.Undefined);
				fire = false;
			};

			grid.Layout (new Rectangle (0, 0, 100, 100));

			foreach (var delayAction in delayActions) {
				delayAction ();
			}
		}
	}
}