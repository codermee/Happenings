namespace ResourceLibrary
{
	public class Localization
	{
		private static readonly AppResources localizedResources = new AppResources();
		public AppResources LocalizedResources
		{
			get { return localizedResources; }
		}
	}
}