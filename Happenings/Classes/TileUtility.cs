using System;
using System.IO.IsolatedStorage;
using System.Linq;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using ResourceLibrary;
using System.Windows;

namespace Happenings.Classes
{
	public class TileUtility
	{

		#region Singleton

		private static TileUtility instance;
		public static TileUtility Instance
		{
			get { return instance ?? (instance = new TileUtility()); }
		}

		#endregion

		#region Constructor

		private TileUtility()
		{
			// Empty constructor
		}

		#endregion

		#region Public methods

		public bool GetLiveTileSetting()
		{
			bool isEnabled;
			var settings = IsolatedStorageSettings.ApplicationSettings;

			if (settings.Contains(Globals.LiveTileSettingKey))
			{
				isEnabled = (bool)settings[Globals.LiveTileSettingKey];
			}
			else
			{
				isEnabled = true;
			}

			return isEnabled;
		}

		public void RemoveTile(Happening item)
		{
			var foundTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("guid=" + item.Guid));

			if (foundTile != null)
			{
				foundTile.Delete();
			}
		}

		public void CreateTile(Happening item)
		{
			var foundTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("guid=" + item.Guid));

			if (foundTile == null)
			{
				var liveTileUpdatesEnabled = GetLiveTileSetting();

				if (liveTileUpdatesEnabled)
				{
					CreatePeriodicTask(Globals.LiveTileUpdaterPeriodicTaskName);
				}

				var tile = SetupTileData(item);

				ShellTile.Create(new Uri(String.Format(Globals.DetailsPageUri, item.Guid), UriKind.Relative), tile);
			}
		}

		public void UpdateTile(Happening item)
		{
			 var foundTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("guid=" + item.Guid));

			if (foundTile != null)
			{
				var tile = SetupTileData(item);

				foundTile.Update(tile);
			}
		}

		public void CreatePeriodicTask(string name)
		{
			var task = new PeriodicTask(name)
								{
									Description = "Live tile updater for Happenings",
									ExpirationTime = DateTime.Now.AddDays(14)
								};

			RemovePeriodicTask(task.Name);

			try
			{
				// Can only be called when application is running in foreground
				ScheduledActionService.Add(task);
			}
			catch (InvalidOperationException exception)
			{
				if (exception.Message.Contains("BNS Error: The action is disabled"))
				{
					MessageBox.Show(AppResources.BackgroundAgentsDisabled);
				}
				if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
				{
					// No user action required.
					// The system prompts the user when the hard limit of periodic tasks has been reached.
				}
			}			

			//#if DEBUG
			//ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
			//#endif
		}

		public void RemovePeriodicTask(string name)
		{
			if (ScheduledActionService.Find(name) != null)
			{
				ScheduledActionService.Remove(name);
			}
		}

		#endregion

		#region Private methods

		private static StandardTileData SetupTileData(Happening item)
		{
			var backContent = String.Empty;
			if (item.ShowMonthsAndDaysOnLiveTile)
			{
				if (item.NumberOfDaysLeft.ToString().StartsWith("-"))
				{
					backContent = AppResources.HappeningOver;
				}
				else if (!String.IsNullOrEmpty(item.MonthsAndDaysLeftText))
				{
					backContent = String.Format("{0} {1}", item.MonthsAndDaysLeftText, AppResources.Left);
				}
				else
				{
					backContent = String.Format("{0} {1}", AppResources.Occurs, AppResources.Today);
				}
			}

			var tile = new StandardTileData
			                    {
				                    BackgroundImage = !String.IsNullOrEmpty(item.ImagePath)
											                    ? new Uri("isostore:" + item.ImagePath, UriKind.Absolute)
											                    : new Uri(Globals.DefaultImage, UriKind.Relative),
				                    Title = item.Name,
				                    Count = item.NumberOfDaysLeft <= 99 ? item.NumberOfDaysLeft : 0,
				                    BackTitle = item.ShowMonthsAndDaysOnLiveTile
										                    ? item.Name
										                    : String.Empty,
				                    BackContent = backContent
			                    };
			return tile;
		}

		#endregion

	}
}