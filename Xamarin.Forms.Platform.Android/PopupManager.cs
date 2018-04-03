using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	internal static class PopupManager
	{
		static readonly List<PopupRequestHelper> s_subscriptions = new List<PopupRequestHelper>();

		internal static void Subscribe(Activity context)
		{
			if (s_subscriptions.Any(s => s.Activity == context))
			{
				return;
			}

			s_subscriptions.Add(new PopupRequestHelper(context));
		}

		internal static void Unsubscribe(Context context)
		{
			var toRemove = s_subscriptions.Where(s => s.Activity == context).ToList();
			foreach (PopupRequestHelper popupRequestHelper in toRemove)
			{
				popupRequestHelper.Dispose();
				s_subscriptions.Remove(popupRequestHelper);
			}
		}

		internal static void ResetBusyCount(Activity context)
		{
			s_subscriptions.FirstOrDefault(s => s.Activity == context)?.ResetBusyCount();
		}

		internal sealed class PopupRequestHelper : IDisposable
		{
			int _busyCount;
			bool? _supportsProgress;

			internal PopupRequestHelper(Activity context)
			{
				Activity = context;
				MessagingCenter.Subscribe<Page, bool>(Activity, Page.BusySetSignalName, OnPageBusy);
				MessagingCenter.Subscribe<Page, AlertArguments>(Activity, Page.AlertSignalName, OnAlertRequested);
				MessagingCenter.Subscribe<Page, ActionSheetArguments>(Activity, Page.ActionSheetSignalName, OnActionSheetRequested);
			}

			public Activity Activity { get; }

			public void Dispose()
			{
				MessagingCenter.Unsubscribe<Page, AlertArguments>(Activity, Page.AlertSignalName);
				MessagingCenter.Unsubscribe<Page, bool>(Activity, Page.BusySetSignalName);
				MessagingCenter.Unsubscribe<Page, ActionSheetArguments>(Activity, Page.ActionSheetSignalName);
			}

			public void ResetBusyCount()
			{
				_busyCount = 0;
			}

			void OnPageBusy(Page sender, bool enabled)
			{
				// Verify that the page making the request is part of this activity 
				if (!PageIsInThisContext(sender))
				{
					return;
				}
				
				_busyCount = Math.Max(0, enabled ? _busyCount + 1 : _busyCount - 1);

				UpdateProgressBarVisibility(_busyCount > 0);
			}

			void OnActionSheetRequested(Page sender, ActionSheetArguments arguments)
			{
				// Verify that the page making the request is part of this activity 
				if (!PageIsInThisContext(sender))
				{
					return;
				}

				var builder = new AlertDialog.Builder(Activity);
				builder.SetTitle(arguments.Title);
				string[] items = arguments.Buttons.ToArray();
				builder.SetItems(items, (o, args) => arguments.Result.TrySetResult(items[args.Which]));

				if (arguments.Cancel != null)
					builder.SetPositiveButton(arguments.Cancel, (o, args) => arguments.Result.TrySetResult(arguments.Cancel));

				if (arguments.Destruction != null)
					builder.SetNegativeButton(arguments.Destruction, (o, args) => arguments.Result.TrySetResult(arguments.Destruction));

				AlertDialog dialog = builder.Create();
				builder.Dispose();
				//to match current functionality of renderer we set cancelable on outside
				//and return null
				dialog.SetCanceledOnTouchOutside(true);
				dialog.CancelEvent += (o, e) => arguments.SetResult(null);
				dialog.Show();
			}

			void OnAlertRequested(Page sender, AlertArguments arguments)
			{
				// Verify that the page making the request is part of this activity 
				if (!PageIsInThisContext(sender))
				{
					return;
				}

				AlertDialog alert = new AlertDialog.Builder(Activity).Create();
				alert.SetTitle(arguments.Title);
				alert.SetMessage(arguments.Message);
				if (arguments.Accept != null)
					alert.SetButton((int)DialogButtonType.Positive, arguments.Accept, (o, args) => arguments.SetResult(true));
				alert.SetButton((int)DialogButtonType.Negative, arguments.Cancel, (o, args) => arguments.SetResult(false));
				alert.CancelEvent += (o, args) => { arguments.SetResult(false); };
				alert.Show();
			}

			void UpdateProgressBarVisibility(bool isBusy)
			{
				if (!SupportsProgress)
					return;
#pragma warning disable 612, 618

				Activity.SetProgressBarIndeterminate(true);
				Activity.SetProgressBarIndeterminateVisibility(isBusy);
#pragma warning restore 612, 618
			}

			internal bool SupportsProgress
			{
				get
				{
					if (_supportsProgress.HasValue)
					{
						return _supportsProgress.Value;
					}

					int progressCircularId = Activity.Resources.GetIdentifier("progress_circular", "id", "android");
					if (progressCircularId > 0)
						_supportsProgress = Activity.FindViewById(progressCircularId) != null;
					else
						_supportsProgress = true;
					return _supportsProgress.Value;
				}
			}

			bool PageIsInThisContext(Page page)
			{
				var renderer = Platform.GetRenderer(page);

				if (renderer?.View?.Context == null)
				{
					return false;
				}

				return renderer.View.Context.Equals(Activity);
			}
		}
	}
}