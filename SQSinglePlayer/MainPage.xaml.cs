using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SQSinglePlayer.Resources;
using System.IO.IsolatedStorage;
using SQSinglePlayer.Enviroment;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace SQSinglePlayer
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (App.AppSettings.AppRated == false)
            {
                while (this.NavigationService.BackStack.Any())
                {
                    this.NavigationService.RemoveBackEntry();
                }

                RateHelper rateHelper = new RateHelper();
                rateHelper.RateAppMessage();

                PlaySoundEffect("menuselection");            
            }
        }

        private void NewGameButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PlaySoundEffect("menuselection");
            if ((App.AppSettings.Level == "5") && (App.AppSettings.CorrectAnswers == 100))
            {

                NavigationService.Navigate(new Uri("/TerminationPage.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
            }
        }

        private void ScoreButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PlaySoundEffect("menuselection");
            NavigationService.Navigate(new Uri("/ScorePage.xaml", UriKind.Relative));
        }

        private void SettingsButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PlaySoundEffect("menuselection");
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void HelpButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PlaySoundEffect("menuselection");
            NavigationService.Navigate(new Uri("/HelpPage.xaml", UriKind.Relative));
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
            while (this.NavigationService.BackStack.Any())
            {
                this.NavigationService.RemoveBackEntry();
            }
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}