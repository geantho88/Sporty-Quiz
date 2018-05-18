using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.Collections.ObjectModel;
using SQSinglePlayer.ViewModel;
using SQSinglePlayer.Model;
using System.Windows.Threading;
using System.Windows.Media;
using SQSinglePlayer.Enviroment;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using GoogleAds;
using System.Windows.Media.Imaging;
using SQSinglePlayer.Model.Enums;

namespace SQSinglePlayer
{
    public partial class GamePage : PhoneApplicationPage
    {
        DispatcherTimer _questionstimer;
        DispatcherTimer _delaytimer;
        SoundEffect _sound;
        Question _nextquestion;
        ObservableCollection<Question> _questionsListByDifficultyLevel;

        private const int _countDownTimer = 15;
        private int _questionCounter;
        private int _correctAnswers;
        private int _wrongAnswers;
        private int _lives ;
        private string _currentLevel;
        private static string _tempquestionId;
        private bool _milestoneReached = false;

        public GamePage()
        {
            InitializeComponent();

            var bannerAd = Monetize.SetAdBanner(AdFormats.Banner, "ca-app-pub-5683686090349772/5803920646");
            AdRequest adRequest = new AdRequest();
            adRequest.Gender = UserGender.Male;
            adRequest.ForceTesting = true;     // Enable test ads
            AdPanel.Children.Add(bannerAd);
            bannerAd.LoadAd(adRequest);

            if (DeviceOpStatus.IsInternetConnected())
            {
                OfflineAd.Visibility = Visibility.Collapsed;
            }

            if (App.QuestionsList.Any())
            {
                _questionstimer = new DispatcherTimer();

                GetSavedGame();
                GetQuestionsByDifficultyLevel();
                GetNextQuestion();
                PlaySoundEffect("whistle");
            }
            else
            {
                MessageBox.Show("Πρόβλημα κατα την λήψη & προβολή των ερωτήσεων. Παρακαλούμε πραγματοποιήστε εκ νέου συγχρονισμό νέων ερωτήσεων");
                NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
            }

        }

        private void GetQuestionsByDifficultyLevel()
        {
            _questionsListByDifficultyLevel = new ObservableCollection<Question>();

            if (App.AppSettings.Level == "1")
            {
                _questionsListByDifficultyLevel = new ObservableCollection<Question>(App.QuestionsList.Where(a => a.Difficulty == Difficulty.VeryEasy).ToList());
            }
            if (App.AppSettings.Level == "2")
            {
                _questionsListByDifficultyLevel = new ObservableCollection<Question>(App.QuestionsList.Where(a => a.Difficulty == Difficulty.Easy || a.Difficulty == Difficulty.VeryEasy).ToList());
            }
            if (App.AppSettings.Level == "3")
            {
                _questionsListByDifficultyLevel = new ObservableCollection<Question>(App.QuestionsList.Where(a => a.Difficulty == Difficulty.Normal || a.Difficulty == Difficulty.Easy || a.Difficulty == Difficulty.VeryEasy).ToList());
            }
            if (App.AppSettings.Level == "4")
            {
                _questionsListByDifficultyLevel = new ObservableCollection<Question>(App.QuestionsList.Where(a => a.Difficulty == Difficulty.Challenging || a.Difficulty == Difficulty.Normal || a.Difficulty == Difficulty.Easy || a.Difficulty == Difficulty.VeryEasy).ToList());
            }
            if (App.AppSettings.Level == "5")
            {
                _questionsListByDifficultyLevel = new ObservableCollection<Question>(App.QuestionsList.Where(a => a.Difficulty == Difficulty.VeryDifficult || a.Difficulty == Difficulty.Challenging || a.Difficulty == Difficulty.Normal || a.Difficulty == Difficulty.Easy || a.Difficulty == Difficulty.VeryEasy).ToList());
            }
        }

        private void GetNextQuestion()
        {
            PlayTimerSoundEffect();

            if (_delaytimer != null)
            {
                _delaytimer.Stop();
            }

            _questionstimer.Stop();

            if (_lives > 0)
            {
                CheckMilestone();
                if (_milestoneReached == false)
                {
                    if (!string.IsNullOrEmpty(_tempquestionId))
                    {
                        _nextquestion = _questionsListByDifficultyLevel.Where(a => a.GID == _tempquestionId).SingleOrDefault();
                        _tempquestionId = null;
                    }

                    else
                    {
                        Random random = new Random();
                        _nextquestion = _questionsListByDifficultyLevel.ElementAt(random.Next(_questionsListByDifficultyLevel.Count()));
                    }

                    LiveCounterText.Text = _lives.ToString();
                    Answer1.Text = _nextquestion.Answer1.ToString();
                    Answer2.Text = _nextquestion.Answer2.ToString();
                    Answer3.Text = _nextquestion.Answer3.ToString();
                    Answer4.Text = _nextquestion.Answer4.ToString();
                    QuestionText.Text = _nextquestion.QuestionText.ToString();
                    questioncounter.Text = (_questionCounter++).ToString();
                    devcorrect.Text = _nextquestion.CorrectAnswer.ToString();
                    StartCountDown(_countDownTimer);
                }

                else
                {
                    _milestoneReached = false;
                    return;
                }
            }

            else
            {
                _correctAnswers = 0;
                _wrongAnswers = 0;
                _questionCounter = 0;

                if (_delaytimer != null)
                {
                    _delaytimer.Stop();
                }

                SetSaveGame();
                _questionstimer.Stop();
                StopTimerSoundEffect();

                _delaytimer.Tick -= new EventHandler(delaytimer_Tick);
                _questionstimer.Tick -= new EventHandler(questionstimer_Tick);
                DisposeObjects();
                NavigationService.Navigate(new Uri("/DefeatPage.xaml", UriKind.Relative));
            }
        }

        private void StartCountDown(int secs)
        {
            _questionstimer = new DispatcherTimer();
            _questionstimer.Interval = TimeSpan.FromSeconds(0.1);
            ProgressBar.Maximum = secs;
            ProgressBar.Value = secs;
            _questionstimer.Tick += questionstimer_Tick;
            _questionstimer.Start();
        }

        void questionstimer_Tick(object sender, EventArgs e)
        {

            ProgressBar.Value = ProgressBar.Value - 0.1;
            if (ProgressBar.Value > 10)
            {
                ProgressBarText.Text = ProgressBar.Value.ToString("0", System.Globalization.CultureInfo.InvariantCulture);
            }

            if (ProgressBar.Value < 10)
            {
                ProgressBarText.Text = ProgressBar.Value.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
            }

            if (ProgressBar.Value < 8 && ProgressBar.Value > 4)
            {
                var uriString = @"Resources/Images/timercenter-warning.png";
                TimerBackground.Source = new BitmapImage(new Uri(uriString, UriKind.Relative));
            }

            if (ProgressBar.Value < 4 && ProgressBar.Value > 0)
            {
                var uriString = @"Resources/Images/timercenter-hurry.png";
                TimerBackground.Source = new BitmapImage(new Uri(uriString, UriKind.Relative));
            }

            if (ProgressBar.Value == 0)
            {
                _lives--;
                if (App.AppSettings.SoundEnabled == true)
                {
                    try
                    {
                        _sound.Dispose();
                    }
                    catch { }
                }
                _questionstimer.Stop();
                _delaytimer = new DispatcherTimer();
                _delaytimer.Tick += delaytimer_Tick;
                _delaytimer.Interval = new TimeSpan(0, 0, 0, 0, 450);
                _delaytimer.Start();

                if (_nextquestion.CorrectAnswer == "1")
                {
                    var uriString = @"Resources/Images/buttonreversecorrect.png";
                    scr1.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }

                if (_nextquestion.CorrectAnswer == "2")
                {
                    var uriString = @"Resources/Images/buttoncorrect.png";
                    scr2.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }

                if (_nextquestion.CorrectAnswer == "3")
                {
                    var uriString = @"Resources/Images/buttonreversecorrect.png";
                    scr3.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }

                if (_nextquestion.CorrectAnswer == "4")
                {
                    var uriString = @"Resources/Images/buttoncorrect.png";
                    scr4.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }
            }
        }

        void delaytimer_Tick(object sender, EventArgs e)
        {
            _delaytimer.Stop();
            ResetUI();
            GetNextQuestion();
            _delaytimer = null;
        }

        private void ResetUI()
        {
            var uriString1 = @"Resources/Images/buttonreverse.png";
            scr1.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString1, UriKind.Relative)) };
            scr3.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString1, UriKind.Relative)) };

            var uriString2 = @"Resources/Images/button.png";
            scr2.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString2, UriKind.Relative)) };
            scr4.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString2, UriKind.Relative)) };

            var uriString3 = @"Resources/Images/timercenter.png";
            TimerBackground.Source = new BitmapImage(new Uri(uriString3, UriKind.Relative));

        }

        private void Button1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_delaytimer != null)
            {
                if (_delaytimer.Interval.Milliseconds > 0)
                {
                    return;
                }
            }

            StopTimerSoundEffect();

            if (_nextquestion.CorrectAnswer == "1")
            {
                var uriString = @"Resources/Images/buttonreversecorrect.png";
                scr1.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };

                _correctAnswers++;

                PlaySoundEffect("positive");
            }
            else
            {
                _lives--;
                _wrongAnswers++;

                PlaySoundEffect("negative");

                var uriString2 = @"Resources/Images/buttonreversewrong.png";
                scr1.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString2, UriKind.Relative)) };

                if (_nextquestion.CorrectAnswer == "2")
                {
                    var uriString = @"Resources/Images/buttoncorrect.png";
                    scr2.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }
                if (_nextquestion.CorrectAnswer == "3")
                {
                    var uriString = @"Resources/Images/buttonreversecorrect.png";
                    scr3.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }
                if (_nextquestion.CorrectAnswer == "4")
                {
                    var uriString = @"Resources/Images/buttoncorrect.png";
                    scr4.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }
            }
            _questionstimer.Stop();
            _delaytimer = new DispatcherTimer();
            _delaytimer.Tick += delaytimer_Tick;
            _delaytimer.Interval = new TimeSpan(0, 0, 0, 0, 450);
            _delaytimer.Start();
        }

        private void Button2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_delaytimer != null)
            {
                if (_delaytimer.Interval.Milliseconds > 0)
                {
                    return;
                }
            }

            StopTimerSoundEffect();

            if (_nextquestion.CorrectAnswer == "2")
            {
                var uriString = @"Resources/Images/buttoncorrect.png";
                scr2.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                _correctAnswers++;
                PlaySoundEffect("positive");
            }
            else
            {
                _lives--;
                _wrongAnswers++;

                PlaySoundEffect("negative");

                var uriString2 = @"Resources/Images/buttonwrong.png";
                scr2.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString2, UriKind.Relative)) };

                if (_nextquestion.CorrectAnswer == "1")
                {
                    var uriString = @"Resources/Images/buttonreversecorrect.png";
                    scr1.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }
                if (_nextquestion.CorrectAnswer == "3")
                {
                    var uriString = @"Resources/Images/buttonreversecorrect.png";
                    scr3.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }
                if (_nextquestion.CorrectAnswer == "4")
                {
                    var uriString = @"Resources/Images/buttoncorrect.png";
                    scr4.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }
            }
            _questionstimer.Stop();
            _delaytimer = new DispatcherTimer();
            _delaytimer.Tick += delaytimer_Tick;
            _delaytimer.Interval = new TimeSpan(0, 0, 0, 0, 450);
            _delaytimer.Start();
        }

        private void Button3_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_delaytimer != null)
            {
                if (_delaytimer.Interval.Milliseconds > 0)
                {
                    return;
                }
            }

            StopTimerSoundEffect();

            if (_nextquestion.CorrectAnswer == "3")
            {
                var uriString = @"Resources/Images/buttonreversecorrect.png";
                scr3.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                _correctAnswers++;
                PlaySoundEffect("positive");
            }
            else
            {
                _lives--;
                _wrongAnswers++;

                PlaySoundEffect("negative");

                var uriString2 = @"Resources/Images/buttonreversewrong.png";
                scr3.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString2, UriKind.Relative)) };


                if (_nextquestion.CorrectAnswer == "1")
                {
                    var uriString = @"Resources/Images/buttonreversecorrect.png";
                    scr1.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }
                if (_nextquestion.CorrectAnswer == "2")
                {
                    var uriString = @"Resources/Images/buttoncorrect.png";
                    scr2.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }
                if (_nextquestion.CorrectAnswer == "4")
                {
                    var uriString = @"Resources/Images/buttoncorrect.png";
                    scr4.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }
            }
            _questionstimer.Stop();
            _delaytimer = new DispatcherTimer();
            _delaytimer.Tick += delaytimer_Tick;
            _delaytimer.Interval = new TimeSpan(0, 0, 0, 0, 450);
            _delaytimer.Start();
        }

        private void Button4_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_delaytimer != null)
            {
                if (_delaytimer.Interval.Milliseconds > 0)
                {
                    return;
                }
            }

            StopTimerSoundEffect();

            if (_nextquestion.CorrectAnswer == "4")
            {
                var uriString = @"Resources/Images/buttoncorrect.png";
                scr4.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                _correctAnswers++;

                PlaySoundEffect("positive");
            }
            else
            {
                _lives--;
                _wrongAnswers++;

                PlaySoundEffect("negative");

                var uriString2 = @"Resources/Images/buttonwrong.png";
                scr4.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString2, UriKind.Relative)) };

                if (_nextquestion.CorrectAnswer == "1")
                {
                    var uriString = @"Resources/Images/buttonreversecorrect.png";
                    scr1.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }
                if (_nextquestion.CorrectAnswer == "2")
                {
                    var uriString = @"Resources/Images/buttoncorrect.png";
                    scr2.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }
                if (_nextquestion.CorrectAnswer == "3")
                {
                    var uriString = @"Resources/Images/buttonreversecorrect.png";
                    scr3.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                }
            }

            _questionstimer.Stop();
            _delaytimer = new DispatcherTimer();
            _delaytimer.Tick += delaytimer_Tick;
            _delaytimer.Interval = new TimeSpan(0, 0, 0, 0, 450);
            _delaytimer.Start();
        }

        private void CheckMilestone()
        {
            if (_currentLevel == "1")
            {
                LevelCounterText.Text = _correctAnswers + "/20";

                if (_correctAnswers == 20)
                {
                    _milestoneReached = true;
                    while (this.NavigationService.BackStack.Any())
                    {
                        this.NavigationService.RemoveBackEntry();
                    }

                    _currentLevel = "2";
                    _lives += 5;
                    _correctAnswers = 0;
                    _wrongAnswers = 0;
                    _questionCounter = 1;
                    LevelCounterText.Text = _correctAnswers + "/40";
                    SetSaveGame();
                    if (_delaytimer != null)
                    {
                        _delaytimer.Stop();
                    }

                    _questionstimer.Stop();
                    StopTimerSoundEffect();

                    _delaytimer.Tick -= new EventHandler(delaytimer_Tick);
                    _questionstimer.Tick -= new EventHandler(questionstimer_Tick);
                    DisposeObjects();
                    NavigationService.Navigate(new Uri("/WinPage.xaml", UriKind.Relative));
                }
            }

            if (_currentLevel == "2")
            {
                LevelCounterText.Text = _correctAnswers + "/40";
                if (_correctAnswers == 40)
                {
                    _milestoneReached = true;
                    while (this.NavigationService.BackStack.Any())
                    {
                        this.NavigationService.RemoveBackEntry();
                    }

                    _currentLevel = "3";
                    _lives += 8;
                    _correctAnswers = 0;
                    _wrongAnswers = 0;
                    _questionCounter = 1;
                    LevelCounterText.Text = _correctAnswers + "/60";
                    SetSaveGame();
                    if (_delaytimer != null)
                    {
                        _delaytimer.Stop();
                    }

                    _questionstimer.Stop();
                    StopTimerSoundEffect();

                    _delaytimer.Tick -= new EventHandler(delaytimer_Tick);
                    _questionstimer.Tick -= new EventHandler(questionstimer_Tick);
                    DisposeObjects();
                    NavigationService.Navigate(new Uri("/WinPage.xaml", UriKind.Relative));
                }
            }

            if (_currentLevel == "3")
            {
                LevelCounterText.Text = _correctAnswers + "/60";
                if (_correctAnswers == 60)
                {
                    _milestoneReached = true;
                    while (this.NavigationService.BackStack.Any())
                    {
                        this.NavigationService.RemoveBackEntry();
                    }

                    _currentLevel = "4";
                    _lives += 12;
                    _correctAnswers = 0;
                    _wrongAnswers = 0;
                    _questionCounter = 1;
                    LevelCounterText.Text = _correctAnswers + "/80";
                    SetSaveGame();
                    if (_delaytimer != null)
                    {
                        _delaytimer.Stop();
                    }

                    _questionstimer.Stop();
                    StopTimerSoundEffect();


                    _delaytimer.Tick -= new EventHandler(delaytimer_Tick);
                    _questionstimer.Tick -= new EventHandler(questionstimer_Tick);
                    DisposeObjects();
                    NavigationService.Navigate(new Uri("/WinPage.xaml", UriKind.Relative));
                }
            }

            if (_currentLevel == "4")
            {
                LevelCounterText.Text = _correctAnswers + "/80";
                if (_correctAnswers == 80)
                {
                    _milestoneReached = true;
                    while (this.NavigationService.BackStack.Any())
                    {
                        this.NavigationService.RemoveBackEntry();
                    }

                    _currentLevel = "5";
                    _lives += 18;
                    _correctAnswers = 0;
                    _wrongAnswers = 0;
                    _questionCounter = 1;
                    LevelCounterText.Text = _correctAnswers + "/100";
                    SetSaveGame();
                    if (_delaytimer != null)
                    {
                        _delaytimer.Stop();
                    }

                    _questionstimer.Stop();
                    StopTimerSoundEffect();

                    _delaytimer.Tick -= new EventHandler(delaytimer_Tick);
                    _questionstimer.Tick -= new EventHandler(questionstimer_Tick);
                    DisposeObjects();
                    NavigationService.Navigate(new Uri("/WinPage.xaml", UriKind.Relative));
                }
            }

            if (_currentLevel == "5")
            {
                LevelCounterText.Text = _correctAnswers + "/100";
                if (_correctAnswers == 100)
                {
                    _milestoneReached = true;
                    while (this.NavigationService.BackStack.Any())
                    {
                        this.NavigationService.RemoveBackEntry();
                    }

                    LevelCounterText.Text = _correctAnswers + "/100";
                    _currentLevel = "5";
                    if (_delaytimer != null)
                    {
                        _delaytimer.Stop();
                    }

                    _questionstimer.Stop();
                    StopTimerSoundEffect();
                    SetSaveGame();

                    _delaytimer.Tick -= new EventHandler(delaytimer_Tick);
                    _questionstimer.Tick -= new EventHandler(questionstimer_Tick);
                    DisposeObjects();
                    NavigationService.Navigate(new Uri("/TerminationPage.xaml", UriKind.Relative));
                }
            }
        }

        private void GetSavedGame()
        {

            var level1cup = @"Resources/Images/trophy4.png";
            var level1text = @"Resources/Images/level1label.png";
            var level2cup = @"Resources/Images/soccer17.png";
            var level2text = @"Resources/Images/level2label.png";
            var level3cup = @"Resources/Images/sportive44.png";
            var level3text = @"Resources/Images/level3label.png";
            var level4cup = @"Resources/Images/football89.png";
            var level4text = @"Resources/Images/level4label.png";
            var level5cup = @"Resources/Images/trophy6.png";
            var level5text = @"Resources/Images/level5label.png";


            _correctAnswers = App.AppSettings.CorrectAnswers;
            _wrongAnswers = App.AppSettings.WrongAnswers;
            _lives = App.AppSettings.Lives;
            _questionCounter = App.AppSettings.CurrectQuestion;
            _currentLevel = App.AppSettings.Level;

            if (_currentLevel == "1")
            {
                LevelCounterText.Text = _correctAnswers + "/20";
                LevelCup.Source = new BitmapImage(new Uri(level1cup, UriKind.Relative));
                LevelLabel.Source = new BitmapImage(new Uri(level1text, UriKind.Relative));

            }
            if (_currentLevel == "2")
            {
                LevelCounterText.Text = _correctAnswers + "/40";
                LevelCup.Source = new BitmapImage(new Uri(level2cup, UriKind.Relative));
                LevelLabel.Source = new BitmapImage(new Uri(level2text, UriKind.Relative));

            }
            if (_currentLevel == "3")
            {
                LevelCounterText.Text = _correctAnswers + "/60";
                LevelCup.Source = new BitmapImage(new Uri(level3cup, UriKind.Relative));
                LevelLabel.Source = new BitmapImage(new Uri(level3text, UriKind.Relative));

            }
            if (_currentLevel == "4")
            {
                LevelCounterText.Text = _correctAnswers + "/80";
                LevelCup.Source = new BitmapImage(new Uri(level4cup, UriKind.Relative));
                LevelLabel.Source = new BitmapImage(new Uri(level4text, UriKind.Relative));

            }
            if (_currentLevel == "5")
            {
                LevelCounterText.Text = _correctAnswers + "/100";
                LevelCup.Source = new BitmapImage(new Uri(level5cup, UriKind.Relative));
                LevelLabel.Source = new BitmapImage(new Uri(level5text, UriKind.Relative));

            }

            else
            {
                return;
            }
        }

        private void SetSaveGame()
        {
            App.AppSettings.CorrectAnswers = _correctAnswers;
            App.AppSettings.WrongAnswers = _wrongAnswers;
            App.AppSettings.Lives = _lives;
            App.AppSettings.CurrectQuestion = _questionCounter;
            App.AppSettings.Level = this._currentLevel;
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

        private void PlayTimerSoundEffect()
        {
            if (App.AppSettings.SoundEnabled == true)
            {
                var info = App.GetResourceStream(new Uri(@"/SQSinglePlayer;Component/Resources/Sound/timer.wav", UriKind.Relative));
                _sound = SoundEffect.FromStream(info.Stream);
                FrameworkDispatcher.Update();
                _sound.Play();
            }
        }

        private void StopTimerSoundEffect()
        {
            if (App.AppSettings.SoundEnabled == true)
            {
                if (this._sound != null)
                {
                    this._sound.Dispose();
                }
            }
        }

        // stores the last shown question in order to reshow it after game start again. this function ensures that no question will refresh if user goes back and then reload game page 
        private void StoreTempQuestion()
        {
            _tempquestionId = _nextquestion.GID;
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            while (this.NavigationService.BackStack.Any())
            {
                this.NavigationService.RemoveBackEntry();
            }

            if (_delaytimer != null)
            {
                _delaytimer.Stop();
                _delaytimer.Tick -= new EventHandler(delaytimer_Tick);
            }

            _questionstimer.Tick -= new EventHandler(questionstimer_Tick);

            _questionstimer.Stop();
            StopTimerSoundEffect();
            SetSaveGame();
            StoreTempQuestion();
            PlaySoundEffect("menuselection");
            DisposeObjects();
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void DisposeObjects()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}