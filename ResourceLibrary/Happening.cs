using System.ComponentModel;
using System;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;

namespace ResourceLibrary
{
	public class Happening : INotifyPropertyChanged
	{

		#region Properties

		public string Guid { get; set; }

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				NotifyPropertyChanged("Name");
			}
		}

		private DateTime _date;
		public DateTime Date
		{
			get { return _date; }
			set
			{
				_date = value;
				NotifyPropertyChanged("Date");
			}
		}

		private int _numberOfDaysLeft;
		public int NumberOfDaysLeft
		{
			get
			{
				_numberOfDaysLeft = CalculateNumberOfDaysLeft(Date);
				//HasPassed = _numberOfDaysLeft < 0;
				return _numberOfDaysLeft;
			}
			set
			{
				_numberOfDaysLeft = value;
				NotifyPropertyChanged("NumberOfDaysLeft");
			}
		}

		private string _monthsAndDaysLeftText;
		public string MonthsAndDaysLeftText
		{
			get
			{
				_monthsAndDaysLeftText = CalculateMonthsAndDaysLeft(Date);
				return _monthsAndDaysLeftText;
			}
			set
			{
				_monthsAndDaysLeftText = value;
				NotifyPropertyChanged("MonthsAndDaysLeftText");
			}
		}

		private bool _showMonthsAndDaysOnLiveTile;
		public bool ShowMonthsAndDaysOnLiveTile
		{
			get { return _showMonthsAndDaysOnLiveTile; }
			set
			{
				_showMonthsAndDaysOnLiveTile = value;
				NotifyPropertyChanged("ShowMonthsAndDaysOnLiveTile");
			}
		}

		private string _imagePath;
		public string ImagePath
		{
			get
			{
				return _imagePath;
			}
			set
			{
				_imagePath = value;
				NotifyPropertyChanged("ImagePath");
			}
		}

		private string _daysLeftText;
		public string DaysLeftText
		{
			get
			{
				var daysLeft = CalculateNumberOfDaysLeft(Date);
				if (daysLeft < 0)
				{
					_daysLeftText = AppResources.HappeningOver;
				}
				else if (daysLeft == 0)
				{
					_daysLeftText = AppResources.Today;
				}
				else
				{
					_daysLeftText = daysLeft > 1
										? String.Format("{0} {1}", daysLeft, AppResources.DaysLeft)
										: String.Format("{0} {1}", daysLeft, AppResources.DayLeft);
				}
				return _daysLeftText;
			}
			set
			{
				_daysLeftText = value;
				NotifyPropertyChanged("DaysLeftText");
			}
		}

		public BitmapImage SavedImage
		{
			get
			{
				return GetSavedImage(ImagePath);
			}
		}

		#endregion

		#region Private methods

		private static int CalculateNumberOfDaysLeft(DateTime happeningDate)
		{
			int daysLeft;
			var diff = happeningDate.Subtract(DateTime.Now);
			if (diff.Hours >= 0)
			{
				daysLeft = diff.Days + 1;
			}
			else
			{
				daysLeft = diff.Days;
			}
			return daysLeft;
		}

		private static string CalculateMonthsAndDaysLeft(DateTime happeningDate)
		{
			string result;
			var now = DateTime.Now;

			var yearDiff = happeningDate.Year - now.Year;

			var currentMonth = DateTime.Now.Month;
			var currentDay = DateTime.Now.Day;
			var nrOfDaysInCurrentMonth = DateTime.DaysInMonth(DateTime.Now.Year, currentMonth);

			var happeningMonth = happeningDate.Month;
			var happeningDay = happeningDate.Day;

			if (happeningDay < currentDay)
			{
				currentDay -= nrOfDaysInCurrentMonth;
				currentMonth++;
			}

			if (happeningMonth < currentMonth)
			{
				currentMonth -= 12;
			}

			var monthsLeft = happeningMonth - currentMonth;
			var daysLeft = happeningDay - currentDay;

			var yearTextSuffix = yearDiff > 1 ? AppResources.Years : AppResources.Year;
			var monthTextSuffix = monthsLeft > 1 ? AppResources.Months : AppResources.Month;
			var dayTextSuffix = daysLeft > 1 ? AppResources.Days : AppResources.Day;

			if (yearDiff == 0) // Occurs this year
			{
				if (monthsLeft == 0 && daysLeft > 0) // Just days left
				{
					result = String.Format("{0} {1}", daysLeft, dayTextSuffix);
				}
				else if (monthsLeft > 0 && daysLeft == 0) // Exactly x months left
				{
					result = String.Format("{0} {1}", monthsLeft, monthTextSuffix);
				}
				else if (monthsLeft == 0 && daysLeft == 0) // Occurs today
				{
					result = String.Empty;
				}
				else // Months and days left
				{
					result = String.Format("{0} {1} {2} {3} {4}", monthsLeft, monthTextSuffix, AppResources.And, daysLeft, dayTextSuffix);
				}
			}
			else if (yearDiff == 1 && monthsLeft <= 12) // Occurs within 12 months but next year
			{
				if (monthsLeft < 12) // Less than 12 months left
				{
					if (monthsLeft == 0 && daysLeft > 0) // Just days left
					{
						result = String.Format("{0} {1}", daysLeft, dayTextSuffix);
					}
					else // Months and days left
					{
						result = daysLeft > 0
									? String.Format("{0} {1} {2} {3} {4}", monthsLeft, monthTextSuffix, AppResources.And, daysLeft, dayTextSuffix)
									: String.Format("{0} {1}", monthsLeft, monthTextSuffix);
					}
				}
				else // Exactly 12 months left, shows '1 year' (and some days)
				{
					result = daysLeft > 0
								? String.Format("{0} {1} {2} {3} {4}", yearDiff, yearTextSuffix, AppResources.And, daysLeft, dayTextSuffix)
								: String.Format("{0} {1}", yearDiff, yearTextSuffix);
				}
			}
			else // More than one year left
			{
				if (monthsLeft == 0 && daysLeft == 0) // Exactly x years left
				{
					result = String.Format("{0} {1}", yearDiff, yearTextSuffix);
				}
				else if (monthsLeft == 0 && daysLeft > 0) // Years and days left
				{
					result = String.Format("{0} {1} {2} {3} {4}", yearDiff, yearTextSuffix, AppResources.And, daysLeft, dayTextSuffix);
				}
				else if (monthsLeft > 0 && daysLeft == 0) // Years and months left
				{
					result = String.Format("{0} {1} {2} {3} {4}", yearDiff, yearTextSuffix, AppResources.And, monthsLeft, monthTextSuffix);
				}
				else // Years, months and days left
				{
					result = String.Format("{0} {1}{2} {3} {4} {5} {6} {7}", yearDiff, yearTextSuffix, ",", monthsLeft, monthTextSuffix, AppResources.And, daysLeft, dayTextSuffix);
				}
			}

			return result;
		}

		private static BitmapImage GetSavedImage(string filename)
		{
			try
			{
				BitmapImage savedImage;
				if (filename != null && filename != "/ApplicationTile.png")
				{
					using (var store = IsolatedStorageFile.GetUserStoreForApplication())
					{
						using (var isoStream = store.OpenFile(filename, FileMode.Open))
						{
							savedImage = new BitmapImage();
							savedImage.SetSource(isoStream);
						}
					}
				}
				else
				{
					savedImage = new BitmapImage(new Uri("/ApplicationTile.png", UriKind.Relative));
				}
				return savedImage;
			}
			catch (Exception)
			{
				return null;
			}
		}

		#endregion

		#region INotifyPropertyChanged Members

		// Used to notify the page that a data context property changed
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

	}
}