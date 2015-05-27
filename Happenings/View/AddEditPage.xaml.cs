using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Happenings.ViewModel;
using System.Windows.Navigation;
using Happenings.Classes;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using ResourceLibrary;

namespace Happenings.View
{
	public partial class AddEditPage
	{

		#region Members

		private readonly HappeningViewModel _viewModel;
		private Happening CurrentItem { get; set; }
		private string ImageFile { get; set; }

		#endregion

		#region Constructor

		public AddEditPage()
		{
			InitializeComponent();
			_viewModel = new HappeningViewModel();
		}

		#endregion

		#region Private methods

		private void SetupApplicationBar()
		{
			var saveIcon = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
			saveIcon.Text = AppResources.Save;

			var cancelIcon = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
			cancelIcon.Text = AppResources.Cancel;
		}

		private string GetFromQuerystring()
		{
			string querystring;
			NavigationContext.QueryString.TryGetValue("guid", out querystring);
			NavigationContext.QueryString.Clear();
			return querystring;
		}

		#endregion

		#region Events

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			_viewModel.Load();

			var guid = GetFromQuerystring();
			if (!String.IsNullOrEmpty(guid))
			{
				PageTitle.Text = AppResources.Edit;
				CurrentItem = (from i in _viewModel.Happenings where i.Guid == guid select i).FirstOrDefault();

				if (CurrentItem != null)
				{
					NameTextBox.Text = CurrentItem.Name;
					DatePicker.Value = CurrentItem.Date;
					ItemImage.Source = CurrentItem.SavedImage;
					ShowMonthsAndDaysOnLiveTileCheckBox.IsChecked = CurrentItem.ShowMonthsAndDaysOnLiveTile;
				}
			}

			SetupApplicationBar();
		}

		private void OnAppBarSaveIconClick(object sender, EventArgs e)
		{
			var common = Common.Instance;
			var tileUtility = TileUtility.Instance;

			if (CurrentItem != null)
			{
				CurrentItem.Name = NameTextBox.Text;
				if (ShowMonthsAndDaysOnLiveTileCheckBox.IsChecked != null)
				{
					CurrentItem.ShowMonthsAndDaysOnLiveTile = ShowMonthsAndDaysOnLiveTileCheckBox.IsChecked.Value;
				}

				if (ImageFile != null)
				{
					CurrentItem.ImagePath = ImageFile;
				}

				if (DatePicker.Value != null)
				{
					CurrentItem.Date = DatePicker.Value.Value;
				}

				_viewModel.Save(CurrentItem);
				tileUtility.UpdateTile(CurrentItem);
				common.NavigateToUrl(Globals.MainPageUri);
			}
			else
			{
				if (DatePicker.Value != null)
				{
					if (NameTextBox.Text != String.Empty)
					{
						var item = new Happening
										{
											Guid = Guid.NewGuid().ToString(),
											Name = NameTextBox.Text,
											Date = DatePicker.Value.Value,
											ShowMonthsAndDaysOnLiveTile = ShowMonthsAndDaysOnLiveTileCheckBox.IsChecked != null && ShowMonthsAndDaysOnLiveTileCheckBox.IsChecked.Value,
											ImagePath = ImageFile
										};
						_viewModel.Save(item);
						common.NavigateToUrl(Globals.MainPageUri);
					}
					else
					{
						MessageBox.Show(AppResources.MissingNameMessageBox, AppResources.MissingTextMessageTitle, MessageBoxButton.OK);
					}
				}
			}
		}

		private void OnAppBarCancelIconClick(object sender, EventArgs e)
		{
			var common = Common.Instance;
			common.NavigateToUrl(Globals.MainPageUri);
		}

		private void OnDefaultImageButtonClick(object sender, RoutedEventArgs e)
		{
			ItemImage.Source = new BitmapImage(new Uri(Globals.DefaultImage, UriKind.Relative));
		}

		private void OnPhotoPickerButtonClick(object sender, RoutedEventArgs e)
		{
			var photoChooserTask = new PhotoChooserTask
						{
							PixelHeight = 173,
							PixelWidth = 173,
						};
			photoChooserTask.Completed += OnPhotoChooserTaskCompleted;
			photoChooserTask.Show();
		}

		private void OnPhotoChooserTaskCompleted(object sender, PhotoResult e)
		{
			if (e.TaskResult == TaskResult.OK)
			{
				var bmp = new BitmapImage();
				bmp.SetSource(e.ChosenPhoto);
				ItemImage.Source = bmp;

				var separator = new[] { "\\" };
				var filePathArray = e.OriginalFileName.Split(separator, StringSplitOptions.RemoveEmptyEntries);

				ImageFile = Globals.ShellContentPath + filePathArray[filePathArray.Length - 1];

				var common = Common.Instance;
				common.SaveImage(e.ChosenPhoto, ImageFile, 0, 100);
			}
		}

		#endregion

	}
}