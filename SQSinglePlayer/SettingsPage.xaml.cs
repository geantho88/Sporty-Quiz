using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using SQSinglePlayer.ViewModel;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using System.Windows.Media.Imaging;
using System.Net.Http;
using SQSinglePlayer.Enviroment;
using SQSinglePlayer.Model.Enums;

namespace SQSinglePlayer
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;

        public SettingsPage()
        {
            InitializeComponent();
            GetSoundSettings();
            GetUpdateSettings();
            QuestionsCountText.Text = "Σύνολο ερωτήσεων " + QuestionsService.CountQuestions().ToString();
        }

        private void SoundButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SetSoundSettings();
        }

        private void SoundLabel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SetSoundSettings();
        }

        private void UpdateLabel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SetUpdateSettings();
        }

        private async void DownloadQuestionsButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (DeviceOpStatus.IsInternetConnected() == true)
            {
                progressIndicator = new ProgressIndicator();
                progressIndicator.IsVisible = true;
                progressIndicator.Text = "Λήψη Ερωτήσεων. Παρακαλώ περιμένετε...";
                progressIndicator.IsIndeterminate = true;
                SystemTray.SetProgressIndicator(this, progressIndicator);

                try
                {
                    if (await QuestionsService.UpdateQuestions() == true)
                    {
                        MessageBox.Show("Επιτυχής λήψη νέων ερωτήσεων");
                        QuestionsCountText.Text = "Σύνολο ερωτήσεων " + QuestionsService.CountQuestions().ToString();
                    }
                    else
                    {
                        MessageBox.Show("Δεν βρέθηκαν νέες ερωτήσεις");
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Σφάλμα κατα τον συγχρονισμό. Παρακαλώ δοκιμάστε αργότερα");
                }

                progressIndicator.IsVisible = false;
                progressIndicator.IsIndeterminate = false;
            }
            else
            {
                MessageBox.Show("Δεν βρέθηκε σύνδεση στο ίντερνετ. Παρακαλώ δοκιμάστε αργότερα");
            }
        }

        private void SetSoundSettings()
        {
            if (App.AppSettings.SoundEnabled == true)
            {
                App.AppSettings.SoundEnabled = false;
                var uriString = @"Resources/Images/timercenter-hurry.png";
                SoundButton.Source = new BitmapImage(new Uri(uriString, UriKind.Relative));
                SoundLabel.Text = "Off";
            }

            else
            {
                App.AppSettings.SoundEnabled = true;
                var uriString = @"Resources/Images/timerok.png";
                SoundButton.Source = new BitmapImage(new Uri(uriString, UriKind.Relative));
                SoundLabel.Text = "On";
            }

            PlaySoundEffect("switch");
        }

        private void GetSoundSettings()
        {
            if (App.AppSettings.SoundEnabled == true)
            {
                var uriString = @"Resources/Images/timerok.png";
                SoundButton.Source = new BitmapImage(new Uri(uriString, UriKind.Relative));
                SoundLabel.Text = "On";
            }

            else
            {
                var uriString = @"Resources/Images/timercenter-hurry.png";
                SoundButton.Source = new BitmapImage(new Uri(uriString, UriKind.Relative));
                SoundLabel.Text = "Off";
            }
        }

        private void SetUpdateSettings()
        {
            if (App.AppSettings.AutoSyncEnabled == true)
            {
                App.AppSettings.AutoSyncEnabled = false;
                var uriString = @"Resources/Images/timercenter-hurry.png";
                UpdateButton.Source = new BitmapImage(new Uri(uriString, UriKind.Relative));
                UpdateLabel.Text = "Off";
            }

            else
            {
                App.AppSettings.AutoSyncEnabled = true;
                var uriString = @"Resources/Images/timerok.png";
                UpdateButton.Source = new BitmapImage(new Uri(uriString, UriKind.Relative));
                UpdateLabel.Text = "On";
            }

            PlaySoundEffect("switch");
        }

        private void GetUpdateSettings()
        {
            if (App.AppSettings.AutoSyncEnabled == true)
            {
                var uriString = @"Resources/Images/timerok.png";
                UpdateButton.Source = new BitmapImage(new Uri(uriString, UriKind.Relative));
                UpdateLabel.Text = "On";
            }

            else
            {
                var uriString = @"Resources/Images/timercenter-hurry.png";
                UpdateButton.Source = new BitmapImage(new Uri(uriString, UriKind.Relative));
                UpdateLabel.Text = "Off";
            }
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

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PlaySoundEffect("menuselection");
        }

        private void QuestionsCountText_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var veryEasyQuestions = App.QuestionsList.Where(a => a.Difficulty == Difficulty.VeryEasy).Count().ToString();
            var easyQuestions = App.QuestionsList.Where(a => a.Difficulty == Difficulty.Easy).Count().ToString();
            var normalQuestions = App.QuestionsList.Where(a => a.Difficulty == Difficulty.Normal).Count().ToString();
            var challengingQuestions = App.QuestionsList.Where(a => a.Difficulty == Difficulty.Challenging).Count().ToString();
            var veryDifficultQuestions = App.QuestionsList.Where(a => a.Difficulty == Difficulty.VeryDifficult).Count().ToString();

            MessageBox.Show(" Α' ερασιτεχνική : " + veryEasyQuestions + " Ερωτήσεις " + Environment.NewLine + " Γ' Εθνική : " + easyQuestions + " Ερωτήσεις " + Environment.NewLine + " Football League : " + normalQuestions + " Ερωτήσεις " + Environment.NewLine + " Super League : " + challengingQuestions + " Ερωτήσεις " + Environment.NewLine + " Champions League : " + veryDifficultQuestions + " Ερωτήσεις " + Environment.NewLine);
        }
    }
}