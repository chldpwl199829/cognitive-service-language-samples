// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AdaptiveCards;


namespace CoreBotCLU.Bots
{
    public class DialogBot<T> : ActivityHandler where T : Dialog
    {
        protected readonly BotState ConversationState;
        protected readonly Dialog Dialog;

        public DialogBot(ConversationState conversationState, T dialog)
        {
            ConversationState = conversationState;
            Dialog = dialog;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            if (turnContext.Activity.Type == ActivityTypes.Invoke && turnContext.Activity.Value != null)
            {
                var activityValue = turnContext.Activity.Value.ToString();
                var invoke = JsonConvert.DeserializeObject<AdaptiveCardInvoke>(activityValue);

                if (invoke.Action != null)
                {
                    var selectedAnswer = (string)invoke.Action["title"];

                    // Check the selected answer and print a message accordingly
                    switch (selectedAnswer)
                    {
                        case "Maintainer":
                            await turnContext.SendActivityAsync("You selected Maintainer. Please select what you want to get helped with. \n 1. Search AD \n2. View Analytics");
                            break;
                        case "Maintenance Planner":
                            await turnContext.SendActivityAsync("You selected Maintenance Planner. Please select what you want to get helped with. \n 1. Search AD \n2. View Analytics");
                            break;
                        case "Maintenance Manager":
                            await turnContext.SendActivityAsync("You selected Maintenance Manager. Please select what you want to get helped with. \n 1. Search AD \n2. View Analytics");
                            break;
                        case "Administrator":
                            await turnContext.SendActivityAsync("You selected Maintenance Administrator. Please select what you want to get helped with. \n 1. Upload AD \n2. View Analytics");
                            break;
                        default:
                            await turnContext.SendActivityAsync("Invalid selection.");
                            break;
                    }
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
            }
        }
    }
}
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Bot.Builder;
//using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Builder.Dialogs.Choices;

//namespace MyBot.Dialogs
//{
//    public class WelcomeMessageDialog : ComponentDialog
//    {
//        private const string WelcomeMessage = "Welcome! Please select your job title:";
//        private const string MaintainerOption = "Maintainer";
//        private const string PlannerOption = "Planner";
//        private const string ManagerOption = "Manager";
//        private const string AdministratorOption = "Administrator";
//        private const string JobTitlePrompt = "jobTitlePrompt";

//        public WelcomeMessageDialog()
//            : base(nameof(WelcomeMessageDialog))
//        {
//            AddDialog(new ChoicePrompt(JobTitlePrompt));
//            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
//            {
//                ShowWelcomeMessageStepAsync,
//                GetJobTitleStepAsync,
//                ShowSelectedJobTitleStepAsync,
//            }));

//            InitialDialogId = nameof(WaterfallDialog);
//        }

//        private async Task<DialogTurnResult> ShowWelcomeMessageStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            await stepContext.Context.SendActivityAsync(MessageFactory.Text(WelcomeMessage), cancellationToken);
//            return await stepContext.NextAsync(cancellationToken: cancellationToken);
//        }

//        private async Task<DialogTurnResult> GetJobTitleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            var promptOptions = new PromptOptions
//            {
//                Prompt = MessageFactory.Text("Please select your job title:"),
//                RetryPrompt = MessageFactory.Text("I'm sorry, I didn't understand your selection. Please select your job title from the list."),
//                Choices = ChoiceFactory.ToChoices(new List<string> { MaintainerOption, PlannerOption, ManagerOption, AdministratorOption }),
//            };

//            return await stepContext.PromptAsync(JobTitlePrompt, promptOptions, cancellationToken);
//        }

//        private async Task<DialogTurnResult> ShowSelectedJobTitleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            var jobTitle = ((FoundChoice)stepContext.Result).Value;

//            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"You've chosen '{jobTitle}'"), cancellationToken);

//            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
//        }
//    }
//}
