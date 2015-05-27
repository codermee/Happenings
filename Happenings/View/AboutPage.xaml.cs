using System;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using ResourceLibrary;

namespace Happenings.View
{
	public partial class AboutPage
	{
		public AboutPage()
		{
			InitializeComponent();
		    SetupApplicationBar();
		}

        private void SetupApplicationBar()
        {
            var reviewIcon = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            reviewIcon.Text = AppResources.Review;

            var mailIcon = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            mailIcon.Text = AppResources.Mail;
        }

        private void OnApplicationBarEmailIconButtonClick(object sender, EventArgs e)
        {
            var emailComposeTask = new EmailComposeTask
                    {
                        Subject = AppResources.MailSubject,
                        To = AppResources.MailTo
                    };

            emailComposeTask.Show();
        }

        private void OnApplicationBarReviewIconButtonClick(object sender, EventArgs e)
        {
            var marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }
	}
}