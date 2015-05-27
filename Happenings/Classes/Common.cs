using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using ResourceLibrary;

namespace Happenings.Classes
{
	public class Common
	{

		#region Singleton

		private static Common instance;
		public static Common Instance
		{
			get { return instance ?? (instance = new Common()); }
		}

		#endregion

		#region Constructor

		private Common()
		{
			// Empty constructor
		}

		#endregion

		#region Public methods

		public void NavigateToUrl(string url)
		{
			var uri = new Uri(url, UriKind.Relative);
			((PhoneApplicationFrame)Application.Current.RootVisual).Navigate(uri);
		}

		public void ShowErrorMessage()
		{
			MessageBox.Show(AppResources.ErrorMessage);
		}

		public void SaveImage(Stream imageStream, string fileName, int orientation, int quality)
		{
			using (var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
			{
				if (isolatedStorage.FileExists(fileName))
				{
					isolatedStorage.DeleteFile(fileName);
				}

				var fileStream = isolatedStorage.CreateFile(fileName);
				var bitmap = new BitmapImage();
				bitmap.SetSource(imageStream);

				var wb = new WriteableBitmap(bitmap);
				wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, orientation, quality);
				wb.Invalidate();
				fileStream.Close();
			}
		}

		public string GetDayOfWeek(DayOfWeek dayOfWeek)
		{
			string day;
			switch (dayOfWeek)
			{
				case DayOfWeek.Monday:
					day = AppResources.Monday;
					break;
				case DayOfWeek.Tuesday:
					day = AppResources.Tuesday;
					break;
				case DayOfWeek.Wednesday:
					day = AppResources.Wednesday;
					break;
				case DayOfWeek.Thursday:
					day = AppResources.Thursday;
					break;
				case DayOfWeek.Friday:
					day = AppResources.Friday;
					break;
				case DayOfWeek.Saturday:
					day = AppResources.Saturday;
					break;
				case DayOfWeek.Sunday:
					day = AppResources.Sunday;
					break;
				default:
					day = String.Empty;
					break;
			}
			return day;
		}

		public string GetMonth(int monthNumber)
		{
			string month;
			switch (monthNumber)
			{
				case 1:
					month = AppResources.January;
					break;
				case 2:
					month = AppResources.February;
					break;
				case 3:
					month = AppResources.March;
					break;
				case 4:
					month = AppResources.April;
					break;
				case 5:
					month = AppResources.May;
					break;
				case 6:
					month = AppResources.June;
					break;
				case 7:
					month = AppResources.July;
					break;
				case 8:
					month = AppResources.August;
					break;
				case 9:
					month = AppResources.September;
					break;
				case 10:
					month = AppResources.October;
					break;
				case 11:
					month = AppResources.November;
					break;
				case 12:
					month = AppResources.December;
					break;
				default:
					month = String.Empty;
					break;
			}
			return month;
		}
		
		#endregion
		
	}
}