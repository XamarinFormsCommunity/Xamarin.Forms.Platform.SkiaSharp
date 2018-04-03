﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.Generic;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 60123, "Rui's issue", PlatformAffected.Default)]
	public class Bugzilla60123 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			// Initialize ui here instead of ctor
			var items = new List<string>();
			for (int i = 0; i < 100; i++)
			{
				items.Add(i.ToString());
			}

			var listView = new ListView(ListViewCachingStrategy.RecycleElement) 
			{
				BackgroundColor = Color.Yellow,
				AutomationId = "ListView"
			};

			listView.ItemsSource = items;

			Content = listView;
		}

#if UITEST
		[Test]
		public void Issue1Test ()
		{
			RunningApp.WaitForElement (q => q.Marked ("ListView"));
			RunningApp.ScrollDown("ListView");
			RunningApp.WaitForElement(q => q.Marked("ListView"));
		}
#endif
	}
}