using System.Windows;
using System.IO.IsolatedStorage;
using Happenings.Classes;
using System.Windows.Navigation;

namespace Happenings.View
{
	public partial class SettingsPage
	{

		#region Constructor

		public SettingsPage()
		{
			InitializeComponent();
		}

		#endregion

        #region Events

        protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var isLowMemoryDevice = (bool)IsolatedStorageSettings.ApplicationSettings[Globals.IsLowMemoryDevice];
			if (isLowMemoryDevice)
			{
				enableLiveTileUpdatesSwitch.IsEnabled = false;
				enableLiveTileUpdatesSwitch.IsChecked = false;
			}
			else
			{
				enableLiveTileUpdatesSwitch.IsChecked = TileUtility.Instance.GetLiveTileSetting();
			}
		}

        #endregion

        #region Private methods

        private static void ToggleEnableLiveTileUpdates(bool enableLiveTileUpdates)
		{
			var settings = IsolatedStorageSettings.ApplicationSettings;
			if (settings.Contains(Globals.LiveTileSettingKey))
			{
				settings[Globals.LiveTileSettingKey] = enableLiveTileUpdates;
			}
			else
			{
				settings.Add(Globals.LiveTileSettingKey, enableLiveTileUpdates);
			}
		}

		private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
		{
			ToggleEnableLiveTileUpdates(true);
			TileUtility.Instance.CreatePeriodicTask(Globals.LiveTileUpdaterPeriodicTaskName);
		}

		private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
		{
			ToggleEnableLiveTileUpdates(false);
			TileUtility.Instance.RemovePeriodicTask(Globals.LiveTileUpdaterPeriodicTaskName);
		}

		#endregion

	}
}