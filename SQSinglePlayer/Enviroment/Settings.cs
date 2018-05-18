using Newtonsoft.Json.Linq;
using SQSinglePlayer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace SQSinglePlayer.Enviroment
{
    class Settings
    {
        public static void LoadSettings()
        {
            IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication();

            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists("Settings.txt"))
                {
                    IsolatedStorageFileStream FS = ISF.OpenFile("Settings.txt", FileMode.Open, FileAccess.Read);
                    using (StreamReader SR = new StreamReader(FS))
                    {
                        var data = SR.ReadToEnd();
                        var settings = JObject.Parse(data);
                        App.AppSettings = settings.ToObject<AppSettings>();
                    }
                }
                // first time.. load settings from files
                else
                {
                    using (StreamWriter SW = new StreamWriter(new IsolatedStorageFileStream("Settings.txt", FileMode.Create, FileAccess.Write, ISF)))
                    {
                        String src = "Resources/Data/Settings.txt";
                        using (StreamReader sr = new StreamReader(src))
                        {
                            var data = sr.ReadToEnd();
                            var settings = JObject.Parse(data);
                            App.AppSettings = settings.ToObject<AppSettings>();

                            SW.WriteLine(data);
                            SW.Close();
                        }
                    }
                }
            }
        }

        public static void SaveSettings()
        {
            IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication();

            using (StreamWriter SW = new StreamWriter(new IsolatedStorageFileStream("Settings.txt", FileMode.Create, FileAccess.Write, ISF)))
            {
                SW.WriteLine((JObject.FromObject(App.AppSettings).ToString()));
                SW.Close();
            }
            ////Set the path of the file
            //String pathoffile = "Resources/Data/Settings.txt";

            //using (System.IO.StreamWriter writer = new System.IO.StreamWriter(pathoffile))
            //{
            //    writer.Write(JObject.FromObject(App.AppSettings).ToString());
            //}
        }
    }
}
