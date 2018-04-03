﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

using NUnit.Framework;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public partial class Bz28545 : ContentPage
	{
		public Bz28545 ()
		{
			InitializeComponent ();
		}

		public Bz28545 (bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			[TestCase(true)]
			[TestCase(false)]
			public void TypeConverterAreAppliedForSettersToAttachedBP (bool useCompiledXaml)
			{
				var layout = new Bz28545 (useCompiledXaml);
				Assert.AreEqual (Color.Pink, layout.label.TextColor);
				Assert.AreEqual (AbsoluteLayoutFlags.PositionProportional, AbsoluteLayout.GetLayoutFlags (layout.label));
				Assert.AreEqual (new Rectangle (1, 1, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize), AbsoluteLayout.GetLayoutBounds (layout.label));
			}
		}
	}
}