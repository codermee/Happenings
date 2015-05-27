using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using ResourceLibrary;
using Happenings.Classes;

namespace Happenings.ViewModel
{
	public class HappeningViewModel
	{

		#region Properties

		public ObservableCollection<Happening> Happenings { get; set; }

		#endregion

		#region Public methods

		public void Load()
		{
			if (IsolatedStorageSettings.ApplicationSettings.Count > 0)
			{
				GetAll();
			}
			else
			{
				Reset();
			}
		}

		public void Save(Happening item)
		{
			var settings = IsolatedStorageSettings.ApplicationSettings;

			if (settings.Contains(item.Guid))
			{
				settings[item.Guid] = item;
			}
			else
			{
				settings.Add(item.Guid, item);
			}

			settings.Save();
		}

		public void Delete(Happening item)
		{
			var settings = IsolatedStorageSettings.ApplicationSettings;
			if (settings.Contains(item.Guid))
			{
				settings.Remove(item.Guid);
			}
			settings.Save();
		}

		#endregion

		#region Private methods

		private void GetAll()
		{
			var happeningsCollection = new ObservableCollection<Happening>();

			foreach (var item in IsolatedStorageSettings.ApplicationSettings)
			{
				if (item.Key != Globals.LiveTileSettingKey && item.Key != Globals.IsLowMemoryDevice)
				{
					happeningsCollection.Add((Happening)item.Value);
				}
			}

			Happenings = happeningsCollection;
		}

		private void Reset()
		{
			Happenings = null;
		}

		#endregion

	}
}