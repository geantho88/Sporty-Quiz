using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace SQSinglePlayer
{
    public partial class DefeatPage : PhoneApplicationPage
    {
        public DefeatPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            while (this.NavigationService.BackStack.Any())
            {
                this.NavigationService.RemoveBackEntry();
            }

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void ExitButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            while (this.NavigationService.BackStack.Any())
            {
                this.NavigationService.RemoveBackEntry();
            }

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void NewGameButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            while (this.NavigationService.BackStack.Any())
            {
                this.NavigationService.RemoveBackEntry();
            }

            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if(App.AppSettings.Level == "1")
            {
                App.AppSettings.Lives = 5;
            }
            if (App.AppSettings.Level == "2")
            {
                App.AppSettings.Lives = 8;
            }
            if (App.AppSettings.Level == "3")
            {
                App.AppSettings.Lives = 14;
            }
            if (App.AppSettings.Level == "4")
            {
                App.AppSettings.Lives = 18;
            }
            if (App.AppSettings.Level == "5")
            {
                App.AppSettings.Lives = 22;
            }

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
        }
    }
}