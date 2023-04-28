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

namespace Microsoft.BotBuilderSamples.Bots
{
    public class DialogAndWelcomeBot<T> : DialogBot<T>
        where T : Dialog
    {
        public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
            : base(conversationState, userState, dialog, logger)
        {
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                // Greet anyone that was not the target (recipient) of this message.
                // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var welcomeCard = CreateAdaptiveCardAttachment();
                    var response = MessageFactory.Attachment(welcomeCard, ssml: "Welcome!");
                    await turnContext.SendActivityAsync(response, cancellationToken);
                    await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
                }
            }
        }

        // Load attachment from embedded resource.
        private Attachment CreateAdaptiveCardAttachment()
        {
            var cardResourcePath = "CoreBotCLU.Cards.welcomeCard.json";

            using (var stream = GetType().Assembly.GetManifestResourceStream(cardResourcePath))
            {
                using (var reader = new StreamReader(stream))
                {
                    var adaptiveCard = reader.ReadToEnd();
                    return new Attachment()
                    {
                        ContentType = "application/vnd.microsoft.card.adaptive",
                        Content = JsonConvert.DeserializeObject(adaptiveCard),
                    };
                }
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
