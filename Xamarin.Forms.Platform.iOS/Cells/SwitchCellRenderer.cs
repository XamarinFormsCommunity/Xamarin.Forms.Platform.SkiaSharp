using System;
using System.ComponentModel;
using System.Drawing;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class SwitchCellRenderer : CellRenderer
	{
		const string CellName = "Xamarin.SwitchCell";

		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var tvc = reusableCell as CellTableViewCell;
			UISwitch uiSwitch = null;
			if (tvc == null)
				tvc = new CellTableViewCell(UITableViewCellStyle.Value1, CellName);
			else
			{
				uiSwitch = tvc.AccessoryView as UISwitch;
				tvc.Cell.PropertyChanged -= OnCellPropertyChanged;
			}

			SetRealCell(item, tvc);

			if (uiSwitch == null)
			{
				uiSwitch = new UISwitch(new RectangleF());
				uiSwitch.ValueChanged += OnSwitchValueChanged;
				tvc.AccessoryView = uiSwitch;
			}

			var boolCell = (SwitchCell)item;

			tvc.Cell = item;
			tvc.Cell.PropertyChanged += OnCellPropertyChanged;
			tvc.AccessoryView = uiSwitch;
			tvc.TextLabel.Text = boolCell.Text;

			uiSwitch.On = boolCell.On;

			WireUpForceUpdateSizeRequested(item, tvc, tv);

			UpdateBackground(tvc, item);
			UpdateIsEnabled(tvc, boolCell);
			UpdateFlowDirection(tvc, boolCell);

			return tvc;
		}

		void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var boolCell = (SwitchCell)sender;
			var realCell = (CellTableViewCell)GetRealCell(boolCell);

			if (e.PropertyName == SwitchCell.OnProperty.PropertyName)
				((UISwitch)realCell.AccessoryView).SetState(boolCell.On, true);
			else if (e.PropertyName == SwitchCell.TextProperty.PropertyName)
				realCell.TextLabel.Text = boolCell.Text;
			else if (e.PropertyName == Cell.IsEnabledProperty.PropertyName)
				UpdateIsEnabled(realCell, boolCell);
			else if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				UpdateFlowDirection(realCell, boolCell);
		}

		void OnSwitchValueChanged(object sender, EventArgs eventArgs)
		{
			var view = (UIView)sender;
			var sw = (UISwitch)view;

			CellTableViewCell realCell = null;
			while (view.Superview != null && realCell == null)
			{
				view = view.Superview;
				realCell = view as CellTableViewCell;
			}

			if (realCell != null)
				((SwitchCell)realCell.Cell).On = sw.On;
		}

		void UpdateFlowDirection(CellTableViewCell cell, SwitchCell switchCell)
		{
			IVisualElementController controller = switchCell.Parent as View;

			var uiSwitch = cell.AccessoryView as UISwitch;

			uiSwitch.UpdateFlowDirection(controller);
		}

		void UpdateIsEnabled(CellTableViewCell cell, SwitchCell switchCell)
		{
			cell.UserInteractionEnabled = switchCell.IsEnabled;
			cell.TextLabel.Enabled = switchCell.IsEnabled;
			cell.DetailTextLabel.Enabled = switchCell.IsEnabled;
			var uiSwitch = cell.AccessoryView as UISwitch;
			if (uiSwitch != null)
				uiSwitch.Enabled = switchCell.IsEnabled;
		}
	}
}