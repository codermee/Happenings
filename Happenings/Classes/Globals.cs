namespace Happenings.Classes
{
	public class Globals
	{
		public static string EditItemUri = "/View/AddEditPage.xaml?guid={0}";
		public static string AddItemUri = "/View/AddEditPage.xaml";
		public static string DetailsPageUri = "/View/DetailsPage.xaml?guid={0}";
		public static string MainPageUri = "/MainPage.xaml";
		public static string SettingsPageUri = "/View/SettingsPage.xaml";
		public static string AboutPageUri = "/View/AboutPage.xaml";
		public static string ShellContentPath = "/Shared/ShellContent/";
		public static string DefaultImage = "/ApplicationTile.png";
		public static string LiveTileSettingKey = "LiveTileSetting";
		public static string LiveTileUpdaterPeriodicTaskName = "LiveTileUpdaterForHappenings";
		public static string IsLowMemoryDevice = "IsLowMemoryDevice";
	}
}