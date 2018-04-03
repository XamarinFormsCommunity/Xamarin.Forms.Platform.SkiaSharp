﻿using System;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 39963, "iOS WebView has wrong scrolling size when loading local html content with images")]
	public class Bugzilla39963 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{

			var notWorkingHtml = @"<html><body>
						<p><img src='test.jpg' /></p>
						<p>After starting (not re-entering!) the app in landscape, scroll down to see a black area which is not supposed to be there.</p>
						<p>After starting (not re-entering!) the app in portrait, scroll to the right to see a black area which is not supposed to be there.</p>
						<p>This only happends when a local image is loaded.</p>
						</body></html>";

			var workingHtml = @"<html><body>
						<p></p>
						<p>Without local image, everything works fine.</p>
						</body></html>";

			// Initialize ui here instead of ctor
			WebView webView = new WebView {
				//Source = new UrlWebViewSource {
				//	Url = "https://blog.xamarin.com/",
				//},
				Source = new HtmlWebViewSource() {
					Html = notWorkingHtml
				},
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			Content = webView;
		}

#if UITEST
		[Test]
		public void Bugzilla39963Test()
		{
			RunningApp.Screenshot("I am at Bugzilla39963");
			RunningApp.SwipeRightToLeft();
			RunningApp.Screenshot("Do we see a black bar");
		}
#endif
	}
}
