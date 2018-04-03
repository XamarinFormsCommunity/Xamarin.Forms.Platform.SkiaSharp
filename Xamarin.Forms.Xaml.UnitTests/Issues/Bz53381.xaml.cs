﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public partial class Bz53381 : ContentView
	{
		public Bz53381()
		{
			InitializeComponent();
		}

		public Bz53381(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			[SetUp]
			public void Setup()
			{
				Device.PlatformServices = new MockPlatformServices();
			}

			[TearDown]
			public void TearDown()
			{
				Application.Current = null;
				Device.PlatformServices = null;
			}

			[TestCase(true)]
			[TestCase(false)]
			public void ControlTemplateAsImplicitAppLevelStyles(bool useCompiledXaml)
			{
				Application.Current = new Bz53381App();
				var view = new Bz53381(useCompiledXaml);
				Application.Current.MainPage = new ContentPage { Content = view };
				var presenter = ((StackLayout)view.InternalChildren[0]).Children[1] as ContentPresenter;
				Assume.That(presenter, Is.Not.Null);
				var grid = presenter.Content as Grid;
				Assert.That(grid, Is.Not.Null);
				Assert.That(grid.BackgroundColor, Is.EqualTo(Color.Green));
			}
		}
	}
}