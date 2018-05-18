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
using System.Windows.Media.Imaging;

namespace SQSinglePlayer
{
    public partial class WinPage : PhoneApplicationPage
    {
        public WinPage()
        {
            InitializeComponent();
        }

        private void GetShowPrize()
        {

        }

        private void ContinueButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PlaySoundEffect("menuselection");
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
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

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

            var level1cup = @"Resources/Images/trophy4.png";
            var level2cup = @"Resources/Images/soccer17.png";
            var level3cup = @"Resources/Images/sportive44.png";
            var level4cup = @"Resources/Images/football89.png";
            var level5cup = @"Resources/Images/trophy6.png";


          if(App.AppSettings.Level == "2")
          {
              leveltext.Text = "Ανέβηκες κατηγορία ξεπερνώντας την  A Ερασιτεχνική !";
              LivesText.Text = "+5";
              LevelCup.Source = new BitmapImage(new Uri(level1cup, UriKind.Relative));
              return;
          }
          if (App.AppSettings.Level == "3")
          {
              leveltext.Text = "Ανέβηκες κατηγορία ξεπερνώντας την  Γ Εθνική !";
              LivesText.Text = "+8";
              LevelCup.Source = new BitmapImage(new Uri(level2cup, UriKind.Relative));
              return;
          }
          if (App.AppSettings.Level == "4")
          {
              leveltext.Text = "Ανέβηκες κατηγορία ξεπερνώντας την  Football League !";
              LivesText.Text = "+12";
              LevelCup.Source = new BitmapImage(new Uri(level3cup, UriKind.Relative));
              return;
          }
          if (App.AppSettings.Level == "5")
          {
              leveltext.Text = "Ανέβηκες κατηγορία ξεπερνώντας την  Super League !";
              LivesText.Text = "+18";
              LevelCup.Source = new BitmapImage(new Uri(level4cup, UriKind.Relative));
              return;
          }
          else
          {
              leveltext.Text = "Συγχαρητήρια !! είσαι στην κορυφή . Τερμάτισες του Sporty Quiz !";
              LevelCup.Source = new BitmapImage(new Uri(level5cup, UriKind.Relative));
          }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Remove navigation back entry except MainPage
            if (NavigationService.BackStack.Any())
            {
                var length = NavigationService.BackStack.Count() - 1;
                var i = 0;
                while (i < length)
                {
                    NavigationService.RemoveBackEntry();
                    i++;
                }
            }

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}