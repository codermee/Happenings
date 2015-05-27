using System.Windows;
using System.Linq;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using ResourceLibrary;

namespace LiveTileUpdater
{
	public class ScheduledAgent : ScheduledTaskAgent
	{
		private static volatile bool _classInitialized;

		/// <remarks>
		/// ScheduledAgent constructor, initializes the UnhandledException handler
		/// </remarks>
		public ScheduledAgent()
		{
			if (!_classInitialized)
			{
				_classInitialized = true;
				// Subscribe to the managed exception handler
				Deployment.Current.Dispatcher.BeginInvoke(delegate
				{
					Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
				});
			}
		}

		/// Code to execute on Unhandled Exceptions
		private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// An unhandled exception has occurred; break into the debugger
				System.Diagnostics.Debugger.Break();
			}
		}

		/// <summary>
		/// Agent that runs a scheduled task
		/// </summary>
		/// <param name="task">
		/// The invoked task
		/// </param>
		/// <remarks>
		/// This method is called when a periodic or resource intensive task is invoked
		/// </remarks>
		protected override void OnInvoke(ScheduledTask task)
		{
			var tiles = (from t in ShellTile.ActiveTiles
						 where t.NavigationUri.ToString().Contains("/View/DetailsPage.xaml")
						 select t).ToList();

			foreach (var tile in tiles)
			{
				var uri = tile.NavigationUri.OriginalString;
				var uriArray = uri.Split('=');
				var guid = uriArray[uriArray.Length - 1];

				var settings = IsolatedStorageSettings.ApplicationSettings;
				if (settings.Contains(guid))
				{
					var item = settings[guid] as Happening;
					if (item != null)
					{
						var backContent = string.Empty;
						if (item.ShowMonthsAndDaysOnLiveTile)
						{
							if (item.NumberOfDaysLeft.ToString().StartsWith("-"))
							{
								backContent = AppResources.HappeningOver;
							}
							else if (!string.IsNullOrEmpty(item.MonthsAndDaysLeftText))
							{
								backContent = string.Format("{0} {1}", item.MonthsAndDaysLeftText, AppResources.Left);
							}
							else
							{
								backContent = string.Format("{0} {1}", AppResources.Occurs, AppResources.Today);
							}
						}

						var tileData = new StandardTileData
											{
												BackContent = backContent,
												Count = item.NumberOfDaysLeft <= 99 ? item.NumberOfDaysLeft : 0
											};
						tile.Update(tileData);
					}
				}
			}

			// Call NotifyComplete to let the system know the agent is done working.
			NotifyComplete();
		}
	}
}