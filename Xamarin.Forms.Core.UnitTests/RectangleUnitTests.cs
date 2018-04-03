using NUnit.Framework;


namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class RectangleUnitTests : BaseTestFixture
	{
		[Test]
		public void TestRectangleConstruction ()
		{
			var rect = new Rectangle ();
			Assert.AreEqual (0, rect.X);
			Assert.AreEqual (0, rect.Y);
			Assert.AreEqual (0, rect.Width);
			Assert.AreEqual (0, rect.Height);

			rect = new Rectangle (2, 3, 4, 5);
			Assert.AreEqual (2, rect.X);
			Assert.AreEqual (3, rect.Y);
			Assert.AreEqual (4, rect.Width);
			Assert.AreEqual (5, rect.Height);

			rect = new Rectangle (new Point (2, 3), new Size (4, 5));
			Assert.AreEqual (2, rect.X);
			Assert.AreEqual (3, rect.Y);
			Assert.AreEqual (4, rect.Width);
			Assert.AreEqual (5, rect.Height);
		}

		[Test]
		public void TestRectangleFromLTRB ()
		{
			var rect = Rectangle.FromLTRB (10, 10, 30, 40);

			Assert.AreEqual (new Rectangle (10, 10, 20, 30), rect);
		}

		[Test]
		public void TestRectangleCalculatedPoints ()
		{
			var rect = new Rectangle (2, 3, 4, 5);
			Assert.AreEqual (2, rect.Left);
			Assert.AreEqual (3, rect.Top);
			Assert.AreEqual (6, rect.Right);
			Assert.AreEqual (8, rect.Bottom);

			Assert.AreEqual (new Size (4, 5), rect.Size);
			Assert.AreEqual (new Point (2, 3), rect.Location);

			Assert.AreEqual (new Point (4, 5.5), rect.Center);

			rect.Left = 1;
			Assert.AreEqual (1, rect.X);

			rect.Right = 3;
			Assert.AreEqual (2, rect.Width);

			rect.Top = 1;
			Assert.AreEqual (1, rect.Y);
			
			rect.Bottom = 2;
			Assert.AreEqual (1, rect.Height);
		}

		[Test]
		public void TestRectangleContains ()
		{
			var rect = new Rectangle (0, 0, 10, 10);
			Assert.True (rect.Contains (5, 5));
			Assert.True (rect.Contains (new Point (5, 5)));
			Assert.True (rect.Contains (new Rectangle (1, 1, 3, 3)));

			Assert.True (rect.Contains (0, 0));
			Assert.False (rect.Contains (10, 10));
		}

		[Test]
		public void TestRectangleInflate ()
		{
			var rect = new Rectangle (0, 0, 10, 10);
			rect = rect.Inflate (5, 5);

			Assert.AreEqual (new Rectangle (-5, -5, 20, 20), rect);

			rect = rect.Inflate (new Size (-5, -5));

			Assert.AreEqual (new Rectangle (0, 0, 10, 10), rect);
		}

		[Test]
		public void TestRectangleOffset ()
		{
			var rect = new Rectangle (0, 0, 10, 10);
			rect = rect.Offset (10, 10);

			Assert.AreEqual (new Rectangle (10, 10, 10, 10), rect);

			rect = rect.Offset (new Point (-10, -10));

			Assert.AreEqual (new Rectangle (0, 0, 10, 10), rect);
		}

		[Test]
		public void TestRectangleRound ()
		{
			var rect = new Rectangle (0.2, 0.3, 0.6, 0.7);

			Assert.AreEqual (new Rectangle (0, 0, 1, 1), rect.Round ());
		}

		[Test]
		public void TestRectangleIntersect ()
		{
			var rect1 = new Rectangle (0, 0, 10, 10);

			var rect2 = new Rectangle (2, 2, 6, 6);

			var intersection = rect1.Intersect (rect2);

			Assert.AreEqual (rect2, intersection);

			rect2 = new Rectangle(2, 2, 12, 12);
			intersection = rect1.Intersect (rect2);

			Assert.AreEqual (new Rectangle (2, 2, 8, 8), intersection);

			rect2 = new Rectangle (20, 20, 2, 2);
			intersection = rect1.Intersect (rect2);

			Assert.AreEqual (Rectangle.Zero, intersection);
		}

		[Test]
		[TestCase(0, 0, ExpectedResult = true)]
		[TestCase(0, 5, ExpectedResult = true)]
		[TestCase(5, 0, ExpectedResult = true)]
		[TestCase(2, 3, ExpectedResult = false)]
		public bool TestIsEmpty (int w, int h)
		{
			return new Rectangle (0, 0, w, h).IsEmpty;
		}

		[Test]
		[TestCase(0, 0, 8, 8, 0, 0, 5, 5, ExpectedResult = true)]
		[TestCase(0, 0, 5, 5, 5, 5, 5, 5, ExpectedResult = false)]
		[TestCase(0, 0, 2, 2, 3, 0, 5, 5, ExpectedResult = false)]
		public bool TestIntersectsWith (double x1, double y1, double w1, double h1, double x2, double y2, double w2, double h2)
		{
			return new Rectangle (x1, y1, w1, h1).IntersectsWith (new Rectangle (x2, y2, w2, h2));
		}

		[Test]
		public void TestSetSize ()
		{
			var rect = new Rectangle ();
			rect.Size = new Size (10, 20);

			Assert.AreEqual (new Rectangle (0, 0, 10, 20), rect);
		}

		[Test]
		public void TestSetLocation ()
		{
			var rect = new Rectangle ();
			rect.Location = new Point (10, 20);

			Assert.AreEqual (new Rectangle (10, 20, 0, 0), rect);
		}

		[Test]
		public void TestUnion ()
		{
			Assert.AreEqual (new Rectangle (0, 3, 13, 10), new Rectangle (3, 3, 10, 10).Union (new Rectangle(0, 5, 2, 2)));
		}

		[Test]
		[TestCase(0, 0, 2, 2, ExpectedResult = "{X=0 Y=0 Width=2 Height=2}")]
		[TestCase(1, 0, 3, 2, ExpectedResult = "{X=1 Y=0 Width=3 Height=2}")]
		public string TestRectangleToString (double x, double y, double w, double h)
		{
			return new Rectangle (x, y, w, h).ToString ();
		}

		[Test]
		public void TestRectangleEquals ()
		{
			Assert.True (new Rectangle (0, 0, 10, 10).Equals (new Rectangle(0, 0, 10, 10)));
			Assert.False (new Rectangle (0, 0, 10, 10).Equals ("Rectangle"));
			Assert.False (new Rectangle (0, 0, 10, 10).Equals (null));

			Assert.True (new Rectangle (0, 0, 10, 10) == new Rectangle (0, 0, 10, 10));
			Assert.True (new Rectangle (0, 0, 10, 10) != new Rectangle (0, 0, 10, 5));
		}

		[Test]
		public void TestRectangleGetHashCode ([Range(3, 4)] double x1, [Range(3, 4)] double y1, [Range(3, 4)] double w1, [Range(3, 4)] double h1, 
		                                      [Range(3, 4)] double x2, [Range(3, 4)] double y2, [Range(3, 4)] double w2, [Range(3, 4)] double h2)
		{
			bool result = new Rectangle (x1, y1, w1, h1).GetHashCode () == new Rectangle (x2, y2, w2, h2).GetHashCode ();

			if (x1 == x2 && y1 == y2 && w1 == w2 && h1 == h2)
				Assert.True (result);
			else
				Assert.False (result);
		}
	}
}
