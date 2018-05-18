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
using Microsoft.Phone.Tasks;

namespace SQSinglePlayer
{
    public partial class TerminationPage : PhoneApplicationPage
    {
        public TerminationPage()
        {
            InitializeComponent();         
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PlaySoundEffect("menuselection");
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void NewGameButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PlaySoundEffect("menuselection");
            MessageBoxResult mm = MessageBox.Show("Προσοχή η ενέργεια αυτή θα μηδενίσει το σκορ σας και θα ξεκινήσετε νέο παιχνίδι απο την αρχή. είστε σίγουροι ?","Sporty Quiz", MessageBoxButton.OKCancel);
            if (mm == MessageBoxResult.OK)
            {
                App.AppSettings.CorrectAnswers = 0;
                App.AppSettings.WrongAnswers = 0;
                App.AppSettings.CurrectQuestion = 1;
                App.AppSettings.Level = "1";
                App.AppSettings.Lives = 5;
                NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
            }
            if (mm == MessageBoxResult.Cancel)
            {
                PlaySoundEffect("menuselection");
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }

        private void fbsharebutton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PlaySoundEffect("menuselection");
            ShareStatusTask shareStatusTask = new ShareStatusTask();
            shareStatusTask.Status = "Μόλις τερμάτισα Το Sporty Quiz με 100 σωστές ερωτήσεις ! #SportyQuiz #Win #AMAEntertainment";
            shareStatusTask.Show();
        }

        private void scorebutton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PlaySoundEffect("menuselection");
            NavigationService.Navigate(new Uri("/ScorePage.xaml", UriKind.Relative));
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