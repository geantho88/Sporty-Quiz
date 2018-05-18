using SQSinglePlayer.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQSinglePlayer.Model
{
    public class Question
    {
        public string GID { get; set; }
        public string QuestionText { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public string CorrectAnswer { get; set; }
        public string CategoryName { get; set; }
        public int? CategoryID { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DateModified { get; set; }
        public bool HasImage { get; set; }
        public Difficulty? Difficulty { get; set; }
    }
}
