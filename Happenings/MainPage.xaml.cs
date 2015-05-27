using System;
using System.Linq;
using System.Windows.Navigation;
using Happenings.Classes;
using Happenings.ViewModel;
using Microsoft.Phone.Shell;
using ResourceLibrary;
using System.IO.IsolatedStorage;

namespace Happenings
{
	public partial class StartPage
	{

		#region Members

		private readonly HappeningViewModel _viewModel;

		#endregion

		#region Constructor

		public StartPage()
		{
			InitializeComponent();
			_viewModel = new HappeningViewModel();
		}

		#endregion

		#region Private methods

		private static void SetupBackgroundAgent()
		{
			var isLowMemoryDevice = (bool)IsolatedStorageSettings.ApplicationSettings[Globals.IsLowMemoryDevice];
			if (!isLowMemoryDevice)
			{
				TileUtility.Instance.CreatePeriodicTask(Globals.LiveTileUpdaterPeriodicTaskName);
			}
		}

		private void SetupApplicationBar()
		{
			var addIcon = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
			addIcon.Text = AppResources.Add;

			var settingsMenuItem = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
			settingsMenuItem.Text = AppResources.Settings;

			var aboutMenuItem = (ApplicationBarMenuItem)ApplicationBar.MenuItems[1];
			aboutMenuItem.Text = AppResources.About;
		}

		#endregion

		#region Events

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			while (NavigationService.CanGoBack)
			{
				NavigationService.RemoveBackEntry();
			}

			_viewModel.Load();

			if (_viewModel.Happenings != null && _viewModel.Happenings.Count > 0)
			{
				AllListControl.DataContext = _viewModel.Happenings;
				UpcomingListControl.DataContext = _viewModel.Happenings.Where(i => i.NumberOfDaysLeft >= 0).ToList();
				PassedListControl.DataContext = _viewModel.Happenings.Where(i => i.NumberOfDaysLeft < 0).ToList();
			}

			SetupBackgroundAgent();
			SetupApplicationBar();
		}

		private void OnAppBarAddIconClick(object sender, EventArgs e)
		{
			var common = Common.Instance;
			common.NavigateToUrl(Globals.AddItemUri);
		}

		private void OnAppBarSettingsMenuItemClick(object sender, EventArgs e)
		{
			var common = Common.Instance;
			common.NavigateToUrl(Globals.SettingsPageUri);
		}

		private void OnAppBarAboutMenuItemClick(object sender, EventArgs e)
		{
			var common = Common.Instance;
			common.NavigateToUrl(Globals.AboutPageUri);
		}

		#endregion

	}
}