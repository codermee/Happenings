using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Happenings.Classes;
using Happenings.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ResourceLibrary;
using System.IO.IsolatedStorage;

namespace Happenings.View
{
	public partial class ListControl
	{

		#region Members

		private readonly HappeningViewModel _viewModel;
		public Happening SelectedItem { get; set; }

		#endregion

		#region Constructor

		public ListControl()
		{
			InitializeComponent();
			_viewModel = new HappeningViewModel();
		}

		#endregion

		#region Events

		private void OnContextMenuPinToStartClick(object sender, RoutedEventArgs e)
		{
			var isLowMemoryDevice = (bool)IsolatedStorageSettings.ApplicationSettings[Globals.IsLowMemoryDevice];
			if (isLowMemoryDevice)
			{
				MessageBox.Show(AppResources.NoSupportForBackgroundAgents);
			}
			else
			{
				var tileUtility = TileUtility.Instance;
				tileUtility.CreateTile(SelectedItem);
			}
		}

		private void OnContextMenuEditClick(object sender, RoutedEventArgs e)
		{
			var common = Common.Instance;
			var editUri = String.Format(Globals.EditItemUri,  SelectedItem.Guid);
			common.NavigateToUrl(editUri);
		}

		private void OnContextMenuDeleteClick(object sender, RoutedEventArgs e)
		{
			var tileUtility = TileUtility.Instance;
			var common = Common.Instance;
			if (SelectedItem != null)
			{
				_viewModel.Load();
				var item = (from i in _viewModel.Happenings where i.Guid == SelectedItem.Guid select i).FirstOrDefault();

				var result = MessageBox.Show(AppResources.DeleteMessageBoxText, AppResources.DeleteMessageBoxTitle, MessageBoxButton.OKCancel);
				if (result == MessageBoxResult.OK)
				{
					tileUtility.RemoveTile(item);
					_viewModel.Delete(item);
					_viewModel.Load();
					HappeningsListBox.DataContext = _viewModel.Happenings;
				}
			}
			else
			{
				common.ShowErrorMessage();
			}
		}

		private void OnUpcomingListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var common = Common.Instance;

			// If selected index is not -1 (no selection)
			if (HappeningsListBox.SelectedIndex != -1)
			{
				// Navigate to the new page
				var item = HappeningsListBox.SelectedItem as Happening;
				if (item != null)
				{
					var detailsUri = String.Format(Globals.DetailsPageUri, item.Guid);
					common.NavigateToUrl(detailsUri);
				}
			}

			// Reset selected index to -1 (no selection)
			HappeningsListBox.SelectedIndex = -1;
		}

		protected void GestureListener_Hold(object sender, GestureEventArgs e)
		{
			// sender is the StackPanel in this example 
			var item = ((Grid)sender).DataContext;

			// item has the type of the model
			SelectedItem = item as Happening;
		}

		private void OnPinToStartMenuItemLoaded(object sender, RoutedEventArgs e)
		{
			var foundTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("guid=" + SelectedItem.Guid));
			if (foundTile != null)
			{
				((MenuItem)sender).IsEnabled = false;
			}
		}

		#endregion

	}
}