using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQSinglePlayer.Model
{
    public class AppSettings
    {
        public bool SoundEnabled { get; set; }
        public bool AutoSyncEnabled { get; set; }
        public bool AppRated { get; set; }
        public string Level { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int Lives { get; set; }
        public int CurrectQuestion { get; set; }
    }
}
