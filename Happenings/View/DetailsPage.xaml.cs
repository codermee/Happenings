using System;
using System.Linq;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using Happenings.ViewModel;
using Happenings.Classes;
using ResourceLibrary;
using System.Windows;
using System.IO.IsolatedStorage;

namespace Happenings.View
{
	public partial class DetailsPage
	{

		#region Members

		private readonly HappeningViewModel viewModel;
		private Happening CurrentItem { get; set; }

		#endregion

		#region Constructor

		public DetailsPage()
		{
			InitializeComponent();
			viewModel = new HappeningViewModel();
		}

		#endregion

		#region Private methods

		private void SetupApplicationBar()
		{
			var pinIcon = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
			pinIcon.Text = AppResources.Pin;

			var foundTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("guid=" + CurrentItem.Guid));
			if (foundTile != null)
			{
				pinIcon.IsEnabled = false;
			}

			var editIcon = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
			editIcon.Text = AppResources.Edit;

			var deleteIcon = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
			deleteIcon.Text = AppResources.Delete;
		}

		private string GetFromQuerystring()
		{
			string querystring;
			NavigationContext.QueryString.TryGetValue("guid", out querystring);
			return querystring;
		}

		#endregion

		#region Events

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var common = Common.Instance;
			var guid = GetFromQuerystring();

			if (!String.IsNullOrEmpty(guid))
			{
				viewModel.Load();
				CurrentItem = (from i in viewModel.Happenings where i.Guid == guid select i).FirstOrDefault();

				if (CurrentItem != null)
				{
					PageTitle.Text = CurrentItem.Name;

					dateInfoTextBlock.Text = String.Format("{0} {1} {2} {3}", common.GetDayOfWeek(CurrentItem.Date.DayOfWeek), CurrentItem.Date.Day, common.GetMonth(CurrentItem.Date.Month), CurrentItem.Date.Year);

					numberOfdaysLeftTextBlock.Text = CurrentItem.NumberOfDaysLeft.ToString();
					itemImage.Source = CurrentItem.SavedImage;

					if (CurrentItem.NumberOfDaysLeft.ToString().StartsWith("-"))
					{
						monthsAndDaysLeftTextBlock.Text = AppResources.HappeningOver;
						daysTextBlock.Text = AppResources.Days;
					}
					else
					{
						monthsAndDaysLeftTextBlock.Text = String.IsNullOrEmpty(CurrentItem.MonthsAndDaysLeftText)
																	? String.Format("{0} {1}", AppResources.Occurs, AppResources.Today)
																	: String.Format("{0} {1}", AppResources.OccursIn, CurrentItem.MonthsAndDaysLeftText);
						daysTextBlock.Text = AppResources.DaysLeft;
					}
				}

				SetupApplicationBar();
			}
		}

		private void OnAppBarPinIconClick(object sender, EventArgs e)
		{
			var isLowMemoryDevice = (bool)IsolatedStorageSettings.ApplicationSettings[Globals.IsLowMemoryDevice];
			if (isLowMemoryDevice)
			{
				MessageBox.Show(AppResources.NoSupportForBackgroundAgents);
			}
			else
			{
				var tileUtility = TileUtility.Instance;
				tileUtility.CreateTile(CurrentItem);
			}
		}

		private void OnAppBarEditIconClick(object sender, EventArgs e)
		{
			var common = Common.Instance;
			var editUri = String.Format(Globals.EditItemUri, CurrentItem.Guid);
			common.NavigateToUrl(editUri);
		}

		private void OnAppBarDeleteIconClick(object sender, EventArgs e)
		{
			var tileUtility = TileUtility.Instance;
			var common = Common.Instance;
			var querystring = GetFromQuerystring();

			if (!String.IsNullOrEmpty(querystring))
			{
				var result = MessageBox.Show(AppResources.DeleteMessageBoxText, AppResources.DeleteMessageBoxTitle, MessageBoxButton.OKCancel);
				if (result == MessageBoxResult.OK)
				{
					tileUtility.RemoveTile(CurrentItem);
					viewModel.Delete(CurrentItem);
					common.NavigateToUrl(Globals.MainPageUri);
				}
			}
			else
			{
				common.ShowErrorMessage();
			}
		}

		#endregion

	}
}