using System;
using System.Collections.Generic;

namespace MyDummyBot.Model
{
    public partial class Question
    {
        public int Id { get; set; }
        public string Qdescription { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
    }
}
