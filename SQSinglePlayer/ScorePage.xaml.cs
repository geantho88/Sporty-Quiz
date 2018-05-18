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
using SQSinglePlayer.Model;

namespace SQSinglePlayer
{
    public partial class ScorePage : PhoneApplicationPage
    {
        public ScorePage()
        {
            InitializeComponent();
            GetScoreUI();
        }

        private void GetScoreUI()
        {
            var uripassed = @"Resources/Images/timerok.png";
            var uricurrent = @"Resources/Images/timercenter-warning.png";
            var uriincomplete = @"Resources/Images/timercenter-hurry.png";

            var SaveGame = App.AppSettings;

            if (SaveGame.Level == "1")
            {
                Level1Image.Source = new BitmapImage(new Uri(uricurrent, UriKind.Relative));
                Level1ScoreText.Text = SaveGame.CorrectAnswers + "/20";

                Level2Image.Source = new BitmapImage(new Uri(uriincomplete, UriKind.Relative));
                Level3Image.Source = new BitmapImage(new Uri(uriincomplete, UriKind.Relative));
                Level4Image.Source = new BitmapImage(new Uri(uriincomplete, UriKind.Relative));
                Level5Image.Source = new BitmapImage(new Uri(uriincomplete, UriKind.Relative));

            }

            if (SaveGame.Level == "2")
            {
                Level1Image.Source = new BitmapImage(new Uri(uripassed, UriKind.Relative));
                Level1ScoreText.Text = "20/20";

                Level2Image.Source = new BitmapImage(new Uri(uricurrent, UriKind.Relative));
                Level2ScoreText.Text = SaveGame.CorrectAnswers + "/40";

                Level3Image.Source = new BitmapImage(new Uri(uriincomplete, UriKind.Relative));
                Level4Image.Source = new BitmapImage(new Uri(uriincomplete, UriKind.Relative));
                Level5Image.Source = new BitmapImage(new Uri(uriincomplete, UriKind.Relative));
            }

            if (SaveGame.Level == "3")
            {
                Level1Image.Source = new BitmapImage(new Uri(uripassed, UriKind.Relative));
                Level1ScoreText.Text = "20/20";

                Level2Image.Source = new BitmapImage(new Uri(uripassed, UriKind.Relative));
                Level2ScoreText.Text = "40/40";

                Level3Image.Source = new BitmapImage(new Uri(uricurrent, UriKind.Relative));
                Level3ScoreText.Text = SaveGame.CorrectAnswers + "/60";

                Level4Image.Source = new BitmapImage(new Uri(uriincomplete, UriKind.Relative));
                Level5Image.Source = new BitmapImage(new Uri(uriincomplete, UriKind.Relative));
            }

            if (SaveGame.Level == "4")
            {
                Level1Image.Source = new BitmapImage(new Uri(uripassed, UriKind.Relative));
                Level1ScoreText.Text = "20/20";

                Level2Image.Source = new BitmapImage(new Uri(uripassed, UriKind.Relative));
                Level2ScoreText.Text = "40/40";

                Level3Image.Source = new BitmapImage(new Uri(uripassed, UriKind.Relative));
                Level3ScoreText.Text = "60/60";

                Level4Image.Source = new BitmapImage(new Uri(uricurrent, UriKind.Relative));
                Level4ScoreText.Text = SaveGame.CorrectAnswers + "/80";

                Level5Image.Source = new BitmapImage(new Uri(uriincomplete, UriKind.Relative));
            }

            if (SaveGame.Level == "5")
            {
                Level1Image.Source = new BitmapImage(new Uri(uripassed, UriKind.Relative));
                Level1ScoreText.Text = "20/20";

                Level2Image.Source = new BitmapImage(new Uri(uripassed, UriKind.Relative));
                Level2ScoreText.Text = "40/40";

                Level3Image.Source = new BitmapImage(new Uri(uripassed, UriKind.Relative));
                Level3ScoreText.Text = "60/60";

                Level4Image.Source = new BitmapImage(new Uri(uripassed, UriKind.Relative));
                Level4ScoreText.Text = "80/80";

                if (SaveGame.CorrectAnswers != 100)
                {
                    Level5Image.Source = new BitmapImage(new Uri(uricurrent, UriKind.Relative));
                    Level5ScoreText.Text = SaveGame.CorrectAnswers + "/100";
                }

                if (SaveGame.CorrectAnswers == 100)
                {
                    Level5Image.Source = new BitmapImage(new Uri(uripassed, UriKind.Relative));
                    Level5ScoreText.Text = SaveGame.CorrectAnswers + "/100";
                }
            }
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
    }
}