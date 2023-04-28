using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;


namespace Microsoft.BotBuilderSamples.Bots
{
    // This IBot implementation can run any type of Dialog. The use of type parameterization is to allows multiple different bots
    // to be run at different endpoints within the same project. This can be achieved by defining distinct Controller types
    // each with dependency on distinct IBot types, this way ASP Dependency Injection can glue everything together without ambiguity.
    // The ConversationState is used by the Dialog system. The UserState isn't, however, it might have been used in a Dialog implementation,
    // and the requirement is that all BotState objects are saved at the end of a turn.
    public class DialogBot<T> : ActivityHandler
        where T : Dialog
    {
        protected readonly Dialog Dialog;
        protected readonly ConversationState ConversationState;
        protected readonly UserState UserState;
        protected readonly ILogger<DialogBot<T>> Logger;

        public DialogBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
        {
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;
            Logger = logger;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);

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
//using System.IO;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Bot.Builder;
//using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Schema;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//namespace Microsoft.BotBuilderSamples.Bots
//{
//    public class DialogBot : ActivityHandler
//    {
//        private readonly DialogSet _dialogs;
//        private readonly ILogger<DialogBot> _logger;

//        public DialogBot(DialogSet dialogs, ILogger<DialogBot> logger)
//        {
//            _dialogs = dialogs;
//            _logger = logger;
//        }


//        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
//        {
//            var messageText = turnContext.Activity.Text.ToLowerInvariant();

//            if (messageText.Contains("maintainer") || messageText.Contains("maintenance planner") || messageText.Contains("maintenance manager") || messageText.Contains("administrator"))
//            {
//                await turnContext.SendActivityAsync($"You selected: {messageText}", cancellationToken: cancellationToken);
//            }

//            else
//            {
//                await turnContext.SendActivityAsync("Please select a valid job title.", cancellationToken: cancellationToken);
//            }
//        }
//    }
//}