using System.Collections.Generic;
using System;

namespace MyDummyBot
{
    public class QuestionDetails
    {
        public string QuestionNumber { get; set; }
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public Guid QuestionGuid { get; set; }
    }
}
