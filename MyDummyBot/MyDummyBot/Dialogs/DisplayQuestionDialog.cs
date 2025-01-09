using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using MyDummyBot.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyDummyBot.Dialogs
{
    public class DisplayQuestionDialog : ComponentDialog
    {
        public DisplayQuestionDialog() : base(nameof(DisplayQuestionDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
                DisplayQuestionAsync,
                SaveAnswerAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> DisplayQuestionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                var questionDetails = (QuestionDetails)stepContext.Options;
                var questionNumber = questionDetails.QuestionNumber;
                var questionText = questionDetails.QuestionText;
                var options = questionDetails.Options;

                var cardJson = LoadAdaptiveCardJson("questionCard.json");
                var modifiedCardJson = ModifyAdaptiveCardJson(cardJson, questionNumber.ToString(), questionText, options);

                var cardAttachment = new Attachment()
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(modifiedCardJson),
                };

                var response = MessageFactory.Attachment(cardAttachment);
                await stepContext.Context.SendActivityAsync(response, cancellationToken);

                return Dialog.EndOfTurn;
            }
            catch (Exception ex)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Error: {ex.Message}"), cancellationToken);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> SaveAnswerAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                var questionDetails = (QuestionDetails)stepContext.Options;
                var response = stepContext.Context.Activity.Value as JObject;
                var selectedOption = response?["selectedOption"]?.ToString();

                if (selectedOption != null)
                {
                    // Save the response
                    await SaveResponseAsync(new QuestionResponse
                    {
                        Number = int.Parse(questionDetails.QuestionNumber),
                        Guid = questionDetails.QuestionGuid,
                        Option = int.Parse(selectedOption)
                    });

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("You Selected " + selectedOption), cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("No option selected."), cancellationToken);
                }

                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
            catch (Exception ex)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Error: {ex.Message}"), cancellationToken);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }

        private async Task SaveResponseAsync(QuestionResponse response)
        {
            // Implement your logic to save the response here
            // For example, save to a database or a file
            var record = new { QuestionNumber = response.Number, QuestionGuid = response.Guid, SelectedOption = response.Option };
            // Save response to database or file
            await Task.CompletedTask;
        }

        private string ModifyAdaptiveCardJson(string cardJson, string questionNumber, string questionText, List<string> options)
        {
            var card = JsonConvert.DeserializeObject<JObject>(cardJson);
            card["body"][0]["text"] = $"Question - {questionNumber}";
            card["body"][1]["columns"][0]["items"][0]["text"] = questionText;

            var choiceSet = card["body"].FirstOrDefault(x => x["type"]?.ToString() == "Input.ChoiceSet");
            if (choiceSet != null)
            {
                var choices = new JArray();
                for (int i = 0; i < options.Count; i++)
                {
                    choices.Add(new JObject
                    {
                        { "title", options[i] },
                        { "value", i.ToString() },
                        { "wrap", true }
                    });
                }
                choiceSet["choices"] = choices;
            }

            return card.ToString();
        }

        private string ModifySubmittedCardJson(string cardJson, string message)
        {
            var card = JsonConvert.DeserializeObject<JObject>(cardJson);
            card["body"] = new JArray
            {
                new JObject
                {
                    ["type"] = "TextBlock",
                    ["size"] = "Medium",
                    ["weight"] = "Bolder",
                    ["text"] = message
                }
            };

            return card.ToString();
        }

        private string LoadAdaptiveCardJson(string cardFilePath)
        {
            var cardResourcePath = GetType().Assembly.GetManifestResourceNames().First(name => name.EndsWith(cardFilePath));

            using (var stream = GetType().Assembly.GetManifestResourceStream(cardResourcePath))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}