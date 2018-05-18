using Newtonsoft.Json.Linq;
using SQSinglePlayer.Model;
using SQSinglePlayer.Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage;

namespace SQSinglePlayer.ViewModel
{
    class QuestionsService
    {
        public static ObservableCollection<Question> GetQuestions()
        {
            string data = null;

            IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication();

            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists("Questions.txt"))
                {
                    IsolatedStorageFileStream FS = ISF.OpenFile("Questions.txt", FileMode.Open, FileAccess.Read);
                    using (StreamReader SR = new StreamReader(FS))
                    {
                        data = SR.ReadToEnd();
                    }
                }

                else
                {
                    using (StreamWriter SW = new StreamWriter(new IsolatedStorageFileStream("Questions.txt", FileMode.Create, FileAccess.Write, ISF)))
                    {
                        String src = "Resources/Data/Questions.txt";
                        using (StreamReader sr = new StreamReader(src))
                        {
                            data = sr.ReadToEnd();
                            SW.WriteLine(data);
                            SW.Close();
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(data))
            {
                ObservableCollection<Question> QuestionsList = new ObservableCollection<Question>();
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("el-GR");

                var jArray = JArray.Parse(data);

                foreach (var item in jArray)
                {
                    Question question = new Question
                    {
                        GID = item["GID"].ToString(),
                        Answer1 = item["Answer1"].ToString(),
                        Answer2 = item["Answer2"].ToString(),
                        Answer3 = item["Answer3"].ToString(),
                        Answer4 = item["Answer4"].ToString(),
                        CorrectAnswer = item["CorrectAnswer"].ToString(),
                        QuestionText = item["QuestionText"].ToString(),
                        Difficulty = (Difficulty)int.Parse(item["Difficulty"].ToString())
                    };

                    QuestionsList.Add(question);
                }

                return QuestionsList;
            }

            return null;
        }

        public static async Task<bool> UpdateQuestions()
        {
            IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication();

            DateTime LatestQuestionDate = GetLatestQuestion();
            using (var client = new System.Net.Http.HttpClient())
            {
                var data = await client.GetAsync("http://193.164.131.73//SportyQuizServices/api/game/GetUpdateSinglePlayerQuestions?QuestionDate=" + LatestQuestionDate.ToString(new CultureInfo("en-US")));
                //var data = await client.GetAsync("http://192.168.1.5//SportyQuizServices/api/game/GetUpdateSinglePlayerQuestions?QuestionDate=" + LatestQuestionDate.ToString(new CultureInfo("en-US")));

                if (data.StatusCode == HttpStatusCode.OK)
                {
                    if (data.Content != null)
                    {
                        using (StreamWriter SW = new StreamWriter(new IsolatedStorageFileStream("Questions.txt", FileMode.Create, FileAccess.Write, ISF)))
                        {
                            var writabledata = data.Content.ReadAsStringAsync().Result;
                            SW.WriteLine(writabledata);
                            SW.Close();
                        }

                        App.QuestionsList = GetQuestions();

                        return true;
                    }

                    return false;
                }

                else
                {
                    return false;
                }
            }
        }

        public static int CountQuestions()
        {
            IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication();
            string data = null;

            IsolatedStorageFileStream FS = ISF.OpenFile("Questions.txt", FileMode.Open, FileAccess.Read);
            using (StreamReader SR = new StreamReader(FS))
            {
                data = SR.ReadToEnd();
            }

            if (!string.IsNullOrEmpty(data))
            {
                ObservableCollection<Question> QuestionsList = new ObservableCollection<Question>();
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("el-GR");

                var jArray = JArray.Parse(data);

                return jArray.Count();
            }
            return 0;
        }

        public static DateTime GetLatestQuestion()
        {
            IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication();
            string data = null;

            IsolatedStorageFileStream FS = ISF.OpenFile("Questions.txt", FileMode.Open, FileAccess.Read);
            using (StreamReader SR = new StreamReader(FS))
            {
                data = SR.ReadToEnd();
            }

            if (!string.IsNullOrEmpty(data))
            {
                List<DateTime> DatesList = new List<DateTime>();
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("el-GR");

                var jArray = JArray.Parse(data);

                foreach (var item in jArray)
                {
                    var QDate = DateTime.Parse(item["Date"].ToString());
                    DatesList.Add(QDate);
                }

                DatesList = DatesList.OrderByDescending(a => a.Date).ToList();

                return DatesList[0];
            }

            return DateTime.Now;
        }
    }
}
