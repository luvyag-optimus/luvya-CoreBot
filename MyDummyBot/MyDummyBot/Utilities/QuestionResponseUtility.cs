using Newtonsoft.Json;

namespace MyDummyBot.Utilities
{
    public class QuestionResponseUtility
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string QuestionText { get; set; }
    }
}
