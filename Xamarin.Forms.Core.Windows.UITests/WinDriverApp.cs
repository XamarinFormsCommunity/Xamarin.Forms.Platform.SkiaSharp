﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using Xamarin.UITest.Queries.Tokens;

namespace Xamarin.Forms.Core.UITests
{
	public class WinDriverApp : IApp
	{
		public const string AppName = "Xamarin.Forms.ControlGallery.WindowsUniversal";

		readonly Dictionary<string, string> _controlNameToTag = new Dictionary<string, string>
		{
			{ "button", "ControlType.Button" }
		};

		readonly WindowsDriver<WindowsElement> _session;

		readonly Dictionary<string, string> _translatePropertyAccessor = new Dictionary<string, string>
		{
			{ "getAlpha", "Opacity" }
		};

		int _scrollBarOffset = 5;

		WindowsElement _viewPort;

		WindowsElement _window;

		public WinDriverApp(WindowsDriver<WindowsElement> session)
		{
			_session = session;
			TestServer = new WindowsTestServer(_session);
		}

		public void Back()
		{
			QueryWindows("Back").First().Click();
		}

		public void ClearText(Func<AppQuery, AppQuery> query)
		{
			QueryWindows(query).First().Clear();
		}

		public void ClearText(Func<AppQuery, AppWebQuery> query)
		{
			throw new NotImplementedException();
		}

		public void ClearText(string marked)
		{
			QueryWindows(marked).First().Clear();
		}

		public void ClearText()
		{
			throw new NotImplementedException();
		}

		public IDevice Device { get; }

		public void DismissKeyboard()
		{
			// No-op for Desktop, which is all we're doing right now
		}

		public void DoubleTap(Func<AppQuery, AppQuery> query)
		{
			DoubleTap(WinQuery.FromQuery(query));
		}

		public void DoubleTap(string marked)
		{
			DoubleTap(WinQuery.FromMarked(marked));
		}

		public void DoubleTapCoordinates(float x, float y)
		{
			throw new NotImplementedException();
		}

		public void DragAndDrop(Func<AppQuery, AppQuery> from, Func<AppQuery, AppQuery> to)
		{
			throw new NotImplementedException();
		}

		public void DragAndDrop(string from, string to)
		{
			throw new NotImplementedException();
		}

		public void DragCoordinates(float fromX, float fromY, float toX, float toY)
		{
			throw new NotImplementedException();
		}

		public void EnterText(string text)
		{
			_session.Keyboard.SendKeys(text);
		}

		public void EnterText(Func<AppQuery, AppQuery> query, string text)
		{
			QueryWindows(query).First().SendKeys(text);
		}

		public void EnterText(string marked, string text)
		{
			QueryWindows(marked).First().SendKeys(text);
		}

		public void EnterText(Func<AppQuery, AppWebQuery> query, string text)
		{
			throw new NotImplementedException();
		}

		public AppResult[] Flash(Func<AppQuery, AppQuery> query = null)
		{
			throw new NotImplementedException();
		}

		public AppResult[] Flash(string marked)
		{
			throw new NotImplementedException();
		}

		public object Invoke(string methodName, object argument = null)
		{
			return Invoke(methodName, new[] { argument });
		}

		public object Invoke(string methodName, object[] arguments)
		{
			if (methodName == "ContextClick")
			{
				// The IApp interface doesn't have a context click concept, and mapping TouchAndHold to 
				// context clicking would box us in if we have the option of running these tests on touch
				// devices later. So we're going to use the back door.
				ContextClick(arguments[0].ToString());
				return null;
			}

			return null;
		}

		public void PinchToZoomIn(Func<AppQuery, AppQuery> query, TimeSpan? duration = null)
		{
			throw new NotImplementedException();
		}

		public void PinchToZoomIn(string marked, TimeSpan? duration = null)
		{
			throw new NotImplementedException();
		}

		public void PinchToZoomInCoordinates(float x, float y, TimeSpan? duration)
		{
			throw new NotImplementedException();
		}

		public void PinchToZoomOut(Func<AppQuery, AppQuery> query, TimeSpan? duration = null)
		{
			throw new NotImplementedException();
		}

		public void PinchToZoomOut(string marked, TimeSpan? duration = null)
		{
			throw new NotImplementedException();
		}

		public void PinchToZoomOutCoordinates(float x, float y, TimeSpan? duration)
		{
			throw new NotImplementedException();
		}

		public void PressEnter()
		{
			_session.Keyboard.PressKey(Keys.Enter);
		}

		public void PressVolumeDown()
		{
			throw new NotImplementedException();
		}

		public void PressVolumeUp()
		{
			throw new NotImplementedException();
		}

		public AppPrintHelper Print { get; }

		public AppResult[] Query(Func<AppQuery, AppQuery> query = null)
		{
			ReadOnlyCollection<WindowsElement> elements = QueryWindows(WinQuery.FromQuery(query));
			return elements.Select(ToAppResult).ToArray();
		}

		public AppResult[] Query(string marked)
		{
			ReadOnlyCollection<WindowsElement> elements = QueryWindows(marked);
			return elements.Select(ToAppResult).ToArray();
		}

		public AppWebResult[] Query(Func<AppQuery, AppWebQuery> query)
		{
			throw new NotImplementedException();
		}

		public T[] Query<T>(Func<AppQuery, AppTypedSelector<T>> query)
		{
			AppTypedSelector<T> appTypedSelector = query(new AppQuery(QueryPlatform.iOS));

			// Swiss-Army Chainsaw time
			// We'll use reflection to dig into the query and get the element selector 
			// and the property value invocation in text form
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
			Type selectorType = appTypedSelector.GetType();
			PropertyInfo tokensProperty = selectorType.GetProperties(bindingFlags)
				.First(t => t.PropertyType == typeof(IQueryToken[]));

			var tokens = (IQueryToken[])tokensProperty.GetValue(appTypedSelector);

			string selector = tokens[0].ToQueryString(QueryPlatform.iOS);
			string invoke = tokens[1].ToCodeString();

			// Now that we have them in text form, we can reinterpret them for Windows
			WinQuery winQuery = WinQuery.FromRaw(selector);
			// TODO hartez 2017/07/19 17:08:44 Make this a bit more resilient if the translation isn't there	
			string attribute = _translatePropertyAccessor[invoke.Substring(8).Replace("\")", "")];

			ReadOnlyCollection<WindowsElement> elements = QueryWindows(winQuery);

			foreach (WindowsElement e in elements)
			{
				string x = e.GetAttribute(attribute);
				Debug.WriteLine($">>>>> WinDriverApp Query 261: {x}");
			}

			// TODO hartez 2017/07/19 17:09:14 Alas, for now this simply doesn't work. Waiting for WinAppDriver to implement it	
			return elements.Select(e => (T)Convert.ChangeType(e.GetAttribute(attribute), typeof(T))).ToArray();
		}

		public string[] Query(Func<AppQuery, InvokeJSAppQuery> query)
		{
			throw new NotImplementedException();
		}

		public void Repl()
		{
			throw new NotImplementedException();
		}

		public FileInfo Screenshot(string title)
		{
			// TODO hartez 2017/07/18 10:16:56 Verify that this is working; seems a bit too simple	
			string filename = $"{title}.png";

			Screenshot screenshot = _session.GetScreenshot();
			screenshot.SaveAsFile(filename, ImageFormat.Png);
			return new FileInfo(filename);
		}

		public void ScrollDown(Func<AppQuery, AppQuery> withinQuery = null, ScrollStrategy strategy = ScrollStrategy.Auto,
			double swipePercentage = 0.67,
			int swipeSpeed = 500, bool withInertia = true)
		{
			if (withinQuery == null)
			{
				Scroll(null, true);
				return;
			}

			WinQuery winQuery = WinQuery.FromQuery(withinQuery);
			Scroll(winQuery, true);
		}

		public void ScrollDown(string withinMarked, ScrollStrategy strategy = ScrollStrategy.Auto,
			double swipePercentage = 0.67,
			int swipeSpeed = 500, bool withInertia = true)
		{
			WinQuery winQuery = WinQuery.FromMarked(withinMarked);
			Scroll(winQuery, true);
		}

		public void ScrollDownTo(string toMarked, string withinMarked = null, ScrollStrategy strategy = ScrollStrategy.Auto,
			double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
		{
			ScrollTo(WinQuery.FromMarked(toMarked), withinMarked == null ? null : WinQuery.FromMarked(withinMarked), timeout);
		}

		public void ScrollDownTo(Func<AppQuery, AppWebQuery> toQuery, string withinMarked,
			ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67,
			int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
		{
			throw new NotImplementedException();
		}

		public void ScrollDownTo(Func<AppQuery, AppQuery> toQuery, Func<AppQuery, AppQuery> withinQuery = null,
			ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67,
			int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
		{
			ScrollTo(WinQuery.FromQuery(toQuery), withinQuery == null ? null : WinQuery.FromQuery(withinQuery), timeout);
		}

		public void ScrollDownTo(Func<AppQuery, AppWebQuery> toQuery, Func<AppQuery, AppQuery> withinQuery = null,
			ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67,
			int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
		{
			throw new NotImplementedException();
		}

		public void ScrollTo(string toMarked, string withinMarked = null, ScrollStrategy strategy = ScrollStrategy.Auto,
			double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
		{
			throw new NotImplementedException();
		}

		public void ScrollUp(Func<AppQuery, AppQuery> query = null, ScrollStrategy strategy = ScrollStrategy.Auto,
			double swipePercentage = 0.67, int swipeSpeed = 500,
			bool withInertia = true)
		{
			if (query == null)
			{
				Scroll(null, false);
				return;
			}

			WinQuery winQuery = WinQuery.FromQuery(query);
			Scroll(winQuery, false);
		}

		public void ScrollUp(string withinMarked, ScrollStrategy strategy = ScrollStrategy.Auto,
			double swipePercentage = 0.67, int swipeSpeed = 500,
			bool withInertia = true)
		{
			WinQuery winQuery = WinQuery.FromMarked(withinMarked);
			Scroll(winQuery, false);
		}

		public void ScrollUpTo(string toMarked, string withinMarked = null, ScrollStrategy strategy = ScrollStrategy.Auto,
			double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
		{
			ScrollTo(WinQuery.FromMarked(toMarked), withinMarked == null ? null : WinQuery.FromMarked(withinMarked), timeout,
				down: false);
		}

		public void ScrollUpTo(Func<AppQuery, AppWebQuery> toQuery, string withinMarked,
			ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67,
			int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
		{
			throw new NotImplementedException();
		}

		public void ScrollUpTo(Func<AppQuery, AppQuery> toQuery, Func<AppQuery, AppQuery> withinQuery = null,
			ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67,
			int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
		{
			ScrollTo(WinQuery.FromQuery(toQuery), withinQuery == null ? null : WinQuery.FromQuery(withinQuery), timeout,
				down: false);
		}

		public void ScrollUpTo(Func<AppQuery, AppWebQuery> toQuery, Func<AppQuery, AppQuery> withinQuery = null,
			ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67,
			int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
		{
			throw new NotImplementedException();
		}

		public void SetOrientationLandscape()
		{
			// Deliberately leaving this as a no-op for now
			// Trying to set the orientation on the Desktop (the only version of UWP we're testing for the moment)
			// gives us a 405 Method Not Allowed, which makes sense. Haven't figured out how to determine
			// whether we're in a mode which allows orientation, but if we were, the next line is probably how to set it.
			//_session.Orientation = ScreenOrientation.Landscape;
		}

		public void SetOrientationPortrait()
		{
			// Deliberately leaving this as a no-op for now
			// Trying to set the orientation on the Desktop (the only version of UWP we're testing for the moment)
			// gives us a 405 Method Not Allowed, which makes sense. Haven't figured out how to determine
			// whether we're in a mode which allows orientation, but if we were, the next line is probably how to set it.
			//_session.Orientation = ScreenOrientation.Portrait;
		}

		public void SetSliderValue(string marked, double value)
		{
			throw new NotImplementedException();
		}

		public void SetSliderValue(Func<AppQuery, AppQuery> query, double value)
		{
			throw new NotImplementedException();
		}

		public void SwipeLeft()
		{
			throw new NotImplementedException();
		}

		public void SwipeLeftToRight(double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
		{
			throw new NotImplementedException();
		}

		public void SwipeLeftToRight(string marked, double swipePercentage = 0.67, int swipeSpeed = 500,
			bool withInertia = true)
		{
			throw new NotImplementedException();
		}

		public void SwipeLeftToRight(Func<AppQuery, AppQuery> query, double swipePercentage = 0.67, int swipeSpeed = 500,
			bool withInertia = true)
		{
			throw new NotImplementedException();
		}

		public void SwipeLeftToRight(Func<AppQuery, AppWebQuery> query, double swipePercentage = 0.67, int swipeSpeed = 500,
			bool withInertia = true)
		{
			throw new NotImplementedException();
		}

		public void SwipeRight()
		{
			throw new NotImplementedException();
		}

		public void SwipeRightToLeft(double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
		{
			throw new NotImplementedException();
		}

		public void SwipeRightToLeft(string marked, double swipePercentage = 0.67, int swipeSpeed = 500,
			bool withInertia = true)
		{
			throw new NotImplementedException();
		}

		public void SwipeRightToLeft(Func<AppQuery, AppQuery> query, double swipePercentage = 0.67, int swipeSpeed = 500,
			bool withInertia = true)
		{
			throw new NotImplementedException();
		}

		public void SwipeRightToLeft(Func<AppQuery, AppWebQuery> query, double swipePercentage = 0.67, int swipeSpeed = 500,
			bool withInertia = true)
		{
			throw new NotImplementedException();
		}

		public void Tap(Func<AppQuery, AppQuery> query)
		{
			WinQuery winQuery = WinQuery.FromQuery(query);
			Tap(winQuery);
		}

		public void Tap(string marked)
		{
			WinQuery winQuery = WinQuery.FromMarked(marked);
			Tap(winQuery);
		}

		public void Tap(Func<AppQuery, AppWebQuery> query)
		{
			throw new NotImplementedException();
		}

		public void TapCoordinates(float x, float y)
		{
			// Okay, this one's a bit complicated. For some reason, _session.Tap() with coordinates does not work
			// (Filed https://github.com/Microsoft/WinAppDriver/issues/229 for that)
			// But we can do the equivalent by manipulating the mouse. The mouse methods all take an ICoordinates
			// object, and you'd think that the "coordinates" part of ICoordinates would have something do with 
			// where the mouse clicks. You'd be wrong. The coordinates parts of that object are ignored and it just
			// clicks the center of whatever WindowsElement the ICoordinates refers to in 'AuxiliaryLocator'

			// If we could just use the element, we wouldn't be tapping at specific coordinates, so that's not 
			// very helpful.

			// Instead, we'll use MouseClickAt

			MouseClickAt(x, y);
		}

		public ITestServer TestServer { get; }

		public void TouchAndHold(Func<AppQuery, AppQuery> query)
		{
			throw new NotImplementedException();
		}

		public void TouchAndHold(string marked)
		{
			throw new NotImplementedException();
		}

		public void TouchAndHoldCoordinates(float x, float y)
		{
			throw new NotImplementedException();
		}

		public void WaitFor(Func<bool> predicate, string timeoutMessage = "Timed out waiting...", TimeSpan? timeout = null,
			TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
		{
			throw new NotImplementedException();
		}

		public AppResult[] WaitForElement(Func<AppQuery, AppQuery> query,
			string timeoutMessage = "Timed out waiting for element...",
			TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
		{
			Func<ReadOnlyCollection<WindowsElement>> result = () => QueryWindows(query);
			return WaitForAtLeastOne(result, timeoutMessage, timeout, retryFrequency).Select(ToAppResult).ToArray();
		}

		public AppResult[] WaitForElement(string marked, string timeoutMessage = "Timed out waiting for element...",
			TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
		{
			Func<ReadOnlyCollection<WindowsElement>> result = () => QueryWindows(marked);
			return WaitForAtLeastOne(result, timeoutMessage, timeout, retryFrequency).Select(ToAppResult).ToArray();
		}

		public AppWebResult[] WaitForElement(Func<AppQuery, AppWebQuery> query,
			string timeoutMessage = "Timed out waiting for element...",
			TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
		{
			throw new NotImplementedException();
		}

		public void WaitForNoElement(Func<AppQuery, AppQuery> query,
			string timeoutMessage = "Timed out waiting for no element...",
			TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
		{
			Func<ReadOnlyCollection<WindowsElement>> result = () => QueryWindows(query);
			WaitForNone(result, timeoutMessage, timeout, retryFrequency);
		}

		public void WaitForNoElement(string marked, string timeoutMessage = "Timed out waiting for no element...",
			TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
		{
			Func<ReadOnlyCollection<WindowsElement>> result = () => QueryWindows(marked);
			WaitForNone(result, timeoutMessage, timeout, retryFrequency);
		}

		public void WaitForNoElement(Func<AppQuery, AppWebQuery> query,
			string timeoutMessage = "Timed out waiting for no element...",
			TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
		{
			throw new NotImplementedException();
		}

		public void ContextClick(string marked)
		{
			WindowsElement element = QueryWindows(marked).First();
			PointF point = ElementToClickablePoint(element);

			MouseClickAt(point.X, point.Y, ClickType.ContextClick);
		}

		internal void MouseClickAt(float x, float y, ClickType clickType = ClickType.SingleClick)
		{
			// Mouse clicking with ICoordinates doesn't work the way we'd like (see TapCoordinates comments),
			// so we have to do some math on our own to get the mouse in the right spot

			// So here's how we're working around it for the moment:
			// 1. Get the Window viewport (which is a known-to-exist element)
			// 2. Using the Window's ICoordinates and the MouseMove() overload with x/y offsets, move the pointer
			//		to the location we care about
			// 3. Use the (undocumented, except in https://github.com/Microsoft/WinAppDriver/issues/118#issuecomment-269404335)
			//		null parameter for Mouse.Click() to click at the current pointer location

			WindowsElement viewPort = GetViewPort();
			int xOffset = viewPort.Coordinates.LocationInViewport.X;
			int yOffset = viewPort.Coordinates.LocationInViewport.Y;
			_session.Mouse.MouseMove(viewPort.Coordinates, (int)x - xOffset, (int)y - yOffset);

			switch (clickType)
			{
				case ClickType.DoubleClick:
					_session.Mouse.DoubleClick(null);
					break;
				case ClickType.ContextClick:
					_session.Mouse.ContextClick(null);
					break;
				case ClickType.SingleClick:
				default:
					_session.Mouse.Click(null);
					break;
			}
		}

		void ClickOrTapElement(WindowsElement element)
		{
			try
			{
				// For most stuff, a simple click will work
				element.Click();
			}
			catch (InvalidOperationException)
			{
				// Some elements aren't "clickable" from an automation perspective (e.g., Frame renders as a Border
				// with content in it; if the content is just a TextBlock, we'll end up here)

				// All is not lost; we can figure out the location of the element in in the application window
				// and Tap in that spot
				PointF p = ElementToClickablePoint(element);
				TapCoordinates(p.X, p.Y);
			}
		}

		void DoubleClickElement(WindowsElement element)
		{
			PointF point = ElementToClickablePoint(element);

			MouseClickAt(point.X, point.Y, clickType: ClickType.DoubleClick);
		}

		void DoubleTap(WinQuery query)
		{
			WindowsElement element = FindFirstElement(query);

			if (element == null)
			{
				return;
			}

			DoubleClickElement(element);
		}

		PointF ElementToClickablePoint(WindowsElement element)
		{
			PointF clickablePoint = GetClickablePoint(element);

			WindowsElement window = GetWindow();
			PointF origin = GetOriginOfBoundingRectangle(window);

			// Use the coordinates in the app window's viewport relative to the window's origin
			return new PointF(clickablePoint.X - origin.X, clickablePoint.Y - origin.Y);
		}

		ReadOnlyCollection<WindowsElement> FilterControlType(IEnumerable<WindowsElement> elements, string controlType)
		{
			string tag = controlType;

			if (tag == "*")
			{
				return new ReadOnlyCollection<WindowsElement>(elements.ToList());
			}

			if (_controlNameToTag.ContainsKey(controlType))
			{
				tag = _controlNameToTag[controlType];
			}

			return new ReadOnlyCollection<WindowsElement>(elements.Where(element => element.TagName == tag).ToList());
		}

		WindowsElement FindFirstElement(WinQuery query)
		{
			Func<ReadOnlyCollection<WindowsElement>> fquery = () => QueryWindows(query);

			string timeoutMessage = $"Timed out waiting for element: {query.Raw}";

			ReadOnlyCollection<WindowsElement> results = WaitForAtLeastOne(fquery, timeoutMessage);

			WindowsElement element = results.FirstOrDefault();

			return element;
		}

		static PointF GetBottomRightOfBoundingRectangle(WindowsElement element)
		{
			string vpcpString = element.GetAttribute("BoundingRectangle");

			// returned string format looks like:
			// Left:-1868 Top:382 Width:1013 Height:680

			string[] vpparts = vpcpString.Split(new[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			float vpx = float.Parse(vpparts[1]);
			float vpy = float.Parse(vpparts[3]);

			float vpw = float.Parse(vpparts[5]);
			float vph = float.Parse(vpparts[7]);

			return new PointF(vpx + vpw, vpy + vph);
		}

		static PointF GetClickablePoint(WindowsElement element)
		{
			string cpString = element.GetAttribute("ClickablePoint");
			string[] parts = cpString.Split(',');
			float x = float.Parse(parts[0]);
			float y = float.Parse(parts[1]);

			return new PointF(x, y);
		}

		static PointF GetOriginOfBoundingRectangle(WindowsElement element)
		{
			string vpcpString = element.GetAttribute("BoundingRectangle");

			// returned string format looks like:
			// Left:-1868 Top:382 Width:1013 Height:680

			string[] vpparts = vpcpString.Split(new[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			float vpx = float.Parse(vpparts[1]);
			float vpy = float.Parse(vpparts[3]);

			return new PointF(vpx, vpy);
		}

		static PointF GetTopRightOfBoundingRectangle(WindowsElement element)
		{
			string vpcpString = element.GetAttribute("BoundingRectangle");

			// returned string format looks like:
			// Left:-1868 Top:382 Width:1013 Height:680

			string[] vpparts = vpcpString.Split(new[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			float vpx = float.Parse(vpparts[1]);
			float vpy = float.Parse(vpparts[3]);

			float vpw = float.Parse(vpparts[5]);

			return new PointF(vpx + vpw, vpy);
		}

		WindowsElement GetViewPort()
		{
			if (_viewPort != null)
			{
				return _viewPort;
			}

			ReadOnlyCollection<WindowsElement> candidates = QueryWindows(AppName);
			_viewPort = candidates[3]; // We really just want the viewport; skip the full window, title bar, min/max buttons...

			int xOffset = _viewPort.Coordinates.LocationInViewport.X;

			if (xOffset > 1) // Everything having to do with scrolling right now is a horrid kludge
			{
				// This makes the scrolling stuff work correctly on a higher density screen (e.g. MBP running Windows) 
				_scrollBarOffset = -70;
			}

			return _viewPort;
		}

		WindowsElement GetWindow()
		{
			if (_window != null)
			{
				return _window;
			}

			_window = QueryWindows(AppName)[0];
			return _window;
		}

		void OriginMouse()
		{
			WindowsElement viewPort = GetViewPort();
			int xOffset = viewPort.Coordinates.LocationInViewport.X;
			int yOffset = viewPort.Coordinates.LocationInViewport.Y;
			_session.Mouse.MouseMove(viewPort.Coordinates, xOffset, yOffset);
		}

		ReadOnlyCollection<WindowsElement> QueryWindows(WinQuery query)
		{
			ReadOnlyCollection<WindowsElement> resultByAccessibilityId = _session.FindElementsByAccessibilityId(query.Marked);
			ReadOnlyCollection<WindowsElement> resultByName = _session.FindElementsByName(query.Marked);

			IEnumerable<WindowsElement> result = resultByAccessibilityId.Concat(resultByName);

			// TODO hartez 2017/10/30 09:47:44 Should this be == "*" || == "TextBox"?	
			// what about other controls where we might be looking by content? TextBlock?
			if (query.ControlType == "*")
			{
				IEnumerable<WindowsElement> textBoxesByContent =
					_session.FindElementsByClassName("TextBox").Where(e => e.Text == query.Marked);
				result = result.Concat(textBoxesByContent);
			}

			return FilterControlType(result, query.ControlType);
		}

		ReadOnlyCollection<WindowsElement> QueryWindows(string marked)
		{
			WinQuery winQuery = WinQuery.FromMarked(marked);
			return QueryWindows(winQuery);
		}

		ReadOnlyCollection<WindowsElement> QueryWindows(Func<AppQuery, AppQuery> query)
		{
			WinQuery winQuery = WinQuery.FromQuery(query);
			return QueryWindows(winQuery);
		}

		void Scroll(WinQuery query, bool down)
		{
			if (query == null)
			{
				ScrollClick(GetWindow(), down);
				return;
			}

			WindowsElement element = FindFirstElement(query);

			ScrollClick(element, down);
		}

		void ScrollClick(WindowsElement element, bool down = true)
		{
			PointF point = down ? GetBottomRightOfBoundingRectangle(element) : GetTopRightOfBoundingRectangle(element);

			PointF origin = GetOriginOfBoundingRectangle(GetWindow());

			var realPoint = new PointF(point.X - origin.X, point.Y - origin.Y);

			int xOffset = _scrollBarOffset;
			if (origin.X < 0)
			{
				// The scrollbar's in a slightly different place relative to the window bounds
				// if we're running on the left monitor (which I like to do)
				xOffset = xOffset * 3;
			}

			float finalX = realPoint.X - xOffset;
			float finalY = realPoint.Y - (down ? 15 : -15);

			OriginMouse();
			MouseClickAt(finalX, finalY, ClickType.SingleClick);
		}

		void ScrollTo(WinQuery toQuery, WinQuery withinQuery, TimeSpan? timeout = null, bool down = true)
		{
			timeout = timeout ?? TimeSpan.FromSeconds(5);
			DateTime start = DateTime.Now;

			while (true)
			{
				Func<ReadOnlyCollection<WindowsElement>> result = () => QueryWindows(toQuery);
				TimeSpan iterationTimeout = TimeSpan.FromMilliseconds(0);
				TimeSpan retryFrequency = TimeSpan.FromMilliseconds(0);

				try
				{
					ReadOnlyCollection<WindowsElement> found = WaitForAtLeastOne(result, timeoutMessage: null,
						timeout: iterationTimeout, retryFrequency: retryFrequency);

					if (found.Count > 0)
					{
						// Success
						return;
					}
				}
				catch (TimeoutException ex)
				{
					// Haven't found it yet, keep scrolling
				}

				long elapsed = DateTime.Now.Subtract(start).Ticks;
				if (elapsed >= timeout.Value.Ticks)
				{
					Debug.WriteLine($">>>>> {elapsed} ticks elapsed, timeout value is {timeout.Value.Ticks}");
					throw new TimeoutException($"Timed out scrolling to {toQuery}");
				}

				Scroll(withinQuery, down);
			}
		}

		void Tap(WinQuery query)
		{
			WindowsElement element = FindFirstElement(query);

			if (element == null)
			{
				return;
			}

			ClickOrTapElement(element);
		}

		static AppRect ToAppRect(WindowsElement windowsElement)
		{
			try
			{
				var result = new AppRect
				{
					X = windowsElement.Location.X,
					Y = windowsElement.Location.Y,
					Height = windowsElement.Size.Height,
					Width = windowsElement.Size.Width
				};

				result.CenterX = result.X + result.Width / 2;
				result.CenterY = result.Y + result.Height / 2;

				return result;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(
					$"Warning: error determining AppRect for {windowsElement}; "
					+ $"if this is a Label with a modified Text value, it might be confusing Windows automation. " +
					$"{ex}");
			}

			return null;
		}

		static AppResult ToAppResult(WindowsElement windowsElement)
		{
			return new AppResult
			{
				Rect = ToAppRect(windowsElement),
				Label = windowsElement.Id, // Not entirely sure about this one
				Description = windowsElement.Text, // or this one
				Enabled = windowsElement.Enabled,
				Id = windowsElement.Id
			};
		}

		static ReadOnlyCollection<WindowsElement> Wait(Func<ReadOnlyCollection<WindowsElement>> query,
			Func<int, bool> satisfactory,
			string timeoutMessage = null,
			TimeSpan? timeout = null, TimeSpan? retryFrequency = null)
		{
			timeout = timeout ?? TimeSpan.FromSeconds(5);
			retryFrequency = retryFrequency ?? TimeSpan.FromMilliseconds(500);
			timeoutMessage = timeoutMessage ?? "Timed out on query.";

			DateTime start = DateTime.Now;

			ReadOnlyCollection<WindowsElement> result = query();

			while (!satisfactory(result.Count))
			{
				long elapsed = DateTime.Now.Subtract(start).Ticks;
				if (elapsed >= timeout.Value.Ticks)
				{
					Debug.WriteLine($">>>>> {elapsed} ticks elapsed, timeout value is {timeout.Value.Ticks}");

					throw new TimeoutException(timeoutMessage);
				}

				Task.Delay(retryFrequency.Value.Milliseconds).Wait();
				result = query();
			}

			return result;
		}

		static ReadOnlyCollection<WindowsElement> WaitForAtLeastOne(Func<ReadOnlyCollection<WindowsElement>> query,
			string timeoutMessage = null,
			TimeSpan? timeout = null, TimeSpan? retryFrequency = null)
		{
			return Wait(query, i => i > 0, timeoutMessage, timeout, retryFrequency);
		}

		void WaitForNone(Func<ReadOnlyCollection<WindowsElement>> query,
			string timeoutMessage = null,
			TimeSpan? timeout = null, TimeSpan? retryFrequency = null)
		{
			Wait(query, i => i == 0, timeoutMessage, timeout, retryFrequency);
		}

		internal enum ClickType
		{
			SingleClick,
			DoubleClick,
			ContextClick
		}
	}
}