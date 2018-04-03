﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;

using NUnit.Framework;
using System.Diagnostics;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public class SimpleContentPageCode : ContentPage
	{
		public SimpleContentPageCode ()
		{
			Content = new Label {
				Text="Hello, Xamarin.Forms!",
				VerticalOptions=LayoutOptions.CenterAndExpand,
				HorizontalOptions=LayoutOptions.CenterAndExpand
			};
		}

		public SimpleContentPageCode (bool useCompiledXaml) : this ()
		{
		}
	}
	public partial class SimpleContentPage : ContentPage
	{
		public SimpleContentPage ()
		{
			InitializeComponent ();
		}

		public SimpleContentPage (bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		public class Tests
		{
			[SetUp]
			public void Setup ()
			{
				Device.PlatformServices = new MockPlatformServices ();
			}

			[Test]
			[Ignore]
			public void XamlCIs20TimesFasterThanXaml ()
			{
				var swXamlC = new Stopwatch ();
				var swXaml = new Stopwatch ();

				swXamlC.Start ();
				for (var i = 0; i < 1000; i++)
					new SimpleContentPage (true);
				swXamlC.Stop ();

				swXaml.Start ();
				for (var i = 0; i < 1000; i++)
					new SimpleContentPage (false);
				swXaml.Stop ();

				Assert.Less (swXamlC.ElapsedMilliseconds * 20, swXaml.ElapsedMilliseconds);
			}

			[Test]
			[Ignore]
			public void XamlCIsNotMuchSlowerThanCode ()
			{
				var swXamlC = new Stopwatch ();
				var swCode = new Stopwatch ();

				swXamlC.Start ();
				for (var i = 0; i < 1000; i++)
					new SimpleContentPage (true);
				swXamlC.Stop ();

				swCode.Start ();
				for (var i = 0; i < 1000; i++)
					new SimpleContentPageCode (false);
				swCode.Stop ();

				Assert.LessOrEqual (swXamlC.ElapsedMilliseconds*.2, swCode.ElapsedMilliseconds);
			}
		}
	}
}