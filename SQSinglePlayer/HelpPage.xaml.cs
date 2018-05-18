using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;

namespace SQSinglePlayer
{
    public partial class HelpPalge : PhoneApplicationPage
    {

        public HelpPalge()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PlaySoundEffect("menuselection");
        }

        private void PlaySoundEffect(string effect)
        {
            if (App.AppSettings.SoundEnabled == true)
            {
                var info = App.GetResourceStream(new Uri(@"/SQSinglePlayer;Component/Resources/Sound/" + effect + ".wav", UriKind.Relative));
                SoundEffect Soundeffect = SoundEffect.FromStream(info.Stream);
                FrameworkDispatcher.Update();
                Soundeffect.Play();
            }
        }

        private void RateButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void WebButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var webBrowserTask = new WebBrowserTask();

            webBrowserTask.Uri = new Uri("http://www.sportyquiz.gr", UriKind.Absolute);

            webBrowserTask.Show();
        }

        private void facebookbutton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var webBrowserTask = new WebBrowserTask();

            webBrowserTask.Uri = new Uri("https://www.facebook.com/sportyquiz", UriKind.Absolute);

            webBrowserTask.Show();
        }
    }
}