using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.EntityFrameworkCore;
using MyDummyBot.Utilities;

namespace MyDummyBot.Dialogs
{
    public class AskQuestionsDialog : ComponentDialog
    {
        public AskQuestionsDialog() : base(nameof(AskQuestionsDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
                AskQuestionStepAsync,
                CheckIfMoreQuestionsStepAsync,
                EndAssessmentStepAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new DisplayQuestionDialog());


            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> AskQuestionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            int questionNumber = stepContext.Options is int ? (int)stepContext.Options : 1;

            if (!stepContext.Values.ContainsKey("AlreadyAskedQuestions"))
                stepContext.Values["AlreadyAskedQuestions"] = new List<int>();

            else
            {
                var alreadyAskedQuestions = (List<int>)stepContext.Values["AlreadyAskedQuestions"];
                int internalQuestionNumber = Randomiser.RandomiseQuestion(alreadyAskedQuestions);
                alreadyAskedQuestions.Add(internalQuestionNumber);
                stepContext.Values["AlreadyAskedQuestions"] = alreadyAskedQuestions;
            }

            QuestionDetails q = new QuestionDetails
            {
                QuestionNumber = questionNumber.ToString(),
                QuestionGuid = Guid.NewGuid(),
                QuestionText = "2 + 2",
                Options = new List<string> { "4", "5", "6" }
            };

            stepContext.Values["QuestionNumber"] = questionNumber + 1;

            return await stepContext.BeginDialogAsync(nameof(DisplayQuestionDialog), q, cancellationToken);
        }

        private async Task<DialogTurnResult> CheckIfMoreQuestionsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            int questionNumber = (int)stepContext.Values["QuestionNumber"];

            if (questionNumber <= 3)
            {
                return await stepContext.ReplaceDialogAsync(nameof(AskQuestionsDialog), questionNumber, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> EndAssessmentStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = "You've completed the assessment successfully...";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(promptMessage), cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}