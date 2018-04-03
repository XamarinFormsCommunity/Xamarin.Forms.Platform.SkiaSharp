﻿using System;

using Xamarin.Forms;

namespace Xamarin.Forms.Controls
{
	public class ToolbarItems : ContentPage
	{
		bool _isEnable = false;
		public ToolbarItems()
		{

			var label = new Label { Text = "Hello ContentPage", AutomationId = "label_id" };

			var command = new Command((obj) =>
							{
								label.Text = "tb4";
							}, (obj) => _isEnable);
			var tb1 = new ToolbarItem("tb1", "menuIcon.png", () =>
			{
				label.Text = "tb1";
			}, ToolbarItemOrder.Primary);
			tb1.IsEnabled = _isEnable;
			tb1.AutomationId = "toolbaritem_primary";

			var tb2 = new ToolbarItem("tb2", null, () =>
			{
				label.Text = "tb2";
			}, ToolbarItemOrder.Primary);
			tb2.AutomationId = "toolbaritem_primary2";

			var tb3 = new ToolbarItem("tb3", "bank.png", () =>
			{
				label.Text = "tb3";
				_isEnable = !_isEnable;
				command.ChangeCanExecute();
			}, ToolbarItemOrder.Secondary);
			tb3.AutomationId = "toolbaritem_secondary";

			var tb4 = new ToolbarItem();
			tb4.Text = "tb4";
			tb4.Order = ToolbarItemOrder.Secondary;
			tb4.Command = command;
			tb4.AutomationId = "toolbaritem_secondary2";

			ToolbarItems.Add(tb1);
			ToolbarItems.Add(tb2);
			ToolbarItems.Add(tb3);
			ToolbarItems.Add(tb4);

			Content = new StackLayout
			{
				Children = {
					label
				}
			};
		}
	}
}


