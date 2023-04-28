using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    //SearchDialog must include 
    public class SearchDialog : CancelAndHelpDialog
    {
        private const string WhichInfoStepMsgText = "Which information do you have?\n\n1. File Name\n2. AD Reference Number\n3. Aircraft Serial Number, AD Title (Problem)";
        private const string ProblemStepMsgText = "If you already know the name of the file, you can simply type it in. Otherwise, please provide the correct Aircraft type, Aircraft serial number, and AD title to help us locate the information you need.";
        private const string FileNameStepMsgText = "What is the name of the file you're looking for? Please provide it below.";
        private const string ADTitleStepMsgText = "Please enter the AD title for which you need information.";
        private const string ADReferenceNumberStepMsgText = "Can you provide me the AD Reference Number associated with the information you're looking for?";
        private const string AircraftSerialNumberStepMsgText = "Could you please provide the Aircraft Serial Number? (including conduct number)";
        private const string AircraftTypeStepMsgText = "Please type in your Aircraft type.";
        private const string HolderStepMsgText = "Please provide the aircraft company's name";

        public SearchDialog()
            : base(nameof(SearchDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                WhichInfoStepAsync,
                ProblemStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> WhichInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var options = new PromptOptions
            {
                Prompt = MessageFactory.Text(WhichInfoStepMsgText),
                Choices = ChoiceFactory.ToChoices(new List<string> { "File Name", "AD Reference Number", "Aircraft Serial Number, AD Title (Problem)", "Aircraft Type, AD Title (Problem)" }),
                Style = ListStyle.HeroCard
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        private async Task<DialogTurnResult> ProblemStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var searchDetails = (SearchDetails)stepContext.Options;
            var choice = (FoundChoice)stepContext.Result;

            switch (choice.Value)
            {
                case "File Name":
                    return await stepContext.NextAsync(searchDetails.FileName, cancellationToken);
                case "AD Reference Number":
                    return await stepContext.NextAsync(searchDetails.ADReferenceNumber, cancellationToken);
                case "Aircraft Serial Number, AD Title (Problem)":
                    if (searchDetails.Problem == null)
                    {
                        var promptMessage = MessageFactory.Text(AircraftSerialNumberStepMsgText, AircraftSerialNumberStepMsgText, InputHints.ExpectingInput);
                        return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
                    }
                    return await stepContext.NextAsync(searchDetails.ADTitle, cancellationToken);
                case "Aircraft Type, AD Title (Problem)":
                    if (searchDetails.Problem == null)
                    {
                        return await stepContext.NextAsync(searchDetails.AircraftType, cancellationToken);
                    }
                    return await stepContext.NextAsync(searchDetails.ADTitle, cancellationToken);
                default:
                    return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }


    }
}




//// Search Dialog for Maintainer, and Maintenance Planner and maintenance manager
//// Maintainer and Maintenance Planner will be mainly asked about the aircraft on search purpose 
//// Maintenance manager will be asked for the date and issue first then the details.


//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Bot.Builder;
//using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Schema;
//using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

//namespace Microsoft.BotBuilderSamples.Dialogs
//{
//    //SearchDialog must include 
//    public class SearchDialog : CancelAndHelpDialog
//    {
//        private const string SearchStepMsgText = "To find the information you need, please answer the following questions. If you're not sure, just type 'I don't know' and we'll move on to the next question.";
//        private const string ProblemStepMsgText = "If you already know the name of the file, you can simply type it in. Otherwise, please provide the correct Aircraft type, Aircraft serial number, and AD title to help us locate the information you need.";
//        private const string FileNameStepMsgText = "What is the name of the file you're looking for? Please provide it below.";
//        private const string ADTitleStepMsgText = "Please enter the AD title for which you need information.";
//        private const string ADReferenceNumberStepMsgText = "Can you provide me the AD Reference Number associated with the information you're looking for?";
//        private const string AircraftSerialNumberStepMsgText = "Could you please provide the Aircraft Serial Number? (including conduct number)";
//        private const string AircraftTypeStepMsgText = "Please type in your Aircraft type.";
//        private const string HolderStepMsgText = "Please provide the aircraft company's name";

//        public SearchDialog()
//            : base(nameof(SearchDialog))
//        {
//            AddDialog(new TextPrompt(nameof(TextPrompt)));
//            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
//            AddDialog(new DateResolverDialog());
//            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
//            {
//                SearchStepAsyrc,
//                ProblemStepAsync,
//                FileNameStepAsync,
//                ADTitleStepAsync,
//                ADReferenceNumberStepAsync,
//                AircraftSerialNumberStepAsync,
//                AircraftTypeStepAsync,
//                HolderStepAsync,
//                EffectiveDateStepAsync,
//                ConfirmStepAsync,
//                FinalStepAsync,
//            }));

//            // The initial child Dialog to run.
//            InitialDialogId = nameof(WaterfallDialog);
//        }

//        private async Task<DialogTurnResult> SearchStepAsyrc(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            var searchDetails = (SearchDetails)stepContext.Options;

//            if (searchDetails.FileName == null)
//            {
//                var promptMessage = MessageFactory.Text(SearchStepMsgText, SearchStepMsgText, InputHints.IgnoringInput);
//                await stepContext.Context.SendActivityAsync(promptMessage, cancellationToken);
//            }

//            return await stepContext.NextAsync(searchDetails.FileName, cancellationToken);
//        }

//        private async Task<DialogTurnResult> ProblemStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            var searchDetails = (SearchDetails)stepContext.Options;

//            if (searchDetails.Problem == null)
//            {
//                var promptMessage = MessageFactory.Text(ProblemStepMsgText, ProblemStepMsgText, InputHints.ExpectingInput);
//                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
//            }

//            return await stepContext.NextAsync(searchDetails.Problem, cancellationToken);
//        }

//        private async Task<DialogTurnResult> FileNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            var searchDetails = (SearchDetails)stepContext.Options;

//            if (searchDetails.FileName == null)
//            {
//                var promptMessage = MessageFactory.Text(FileNameStepMsgText, FileNameStepMsgText, InputHints.ExpectingInput);
//                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
//            }

//            return await stepContext.NextAsync(searchDetails.FileName, cancellationToken);
//        }
//        private async Task<DialogTurnResult> ADReferenceNumberStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            var searchDetails = (SearchDetails)stepContext.Options;

//            if (searchDetails.ADReferenceNumber == null)
//            {
//                var promptMessage = MessageFactory.Text(ADReferenceNumberStepMsgText, ADReferenceNumberStepMsgText, InputHints.ExpectingInput);
//                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
//            }

//            return await stepContext.NextAsync(searchDetails.ADReferenceNumber, cancellationToken);
//        }

//        private async Task<DialogTurnResult> AircraftSerialNumberStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            var searchDetails = (SearchDetails)stepContext.Options;

//            if (searchDetails.AircraftSerialNumber == null)
//            {
//                var promptMessage = MessageFactory.Text(AircraftSerialNumberStepMsgText, AircraftSerialNumberStepMsgText, InputHints.ExpectingInput);
//                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
//            }

//            return await stepContext.NextAsync(searchDetails.AircraftSerialNumber, cancellationToken);
//        }

//        private async Task<DialogTurnResult> AircraftTypeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            var searchDetails = (SearchDetails)stepContext.Options;

//            if (searchDetails.AircraftType == null)
//            {
//                var promptMessage = MessageFactory.Text(AircraftTypeStepMsgText, AircraftTypeStepMsgText, InputHints.ExpectingInput);
//                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
//            }

//            return await stepContext.NextAsync(searchDetails.FileName, cancellationToken);
//        }
//        private async Task<DialogTurnResult> HolderStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            var searchDetails = (SearchDetails)stepContext.Options;

//            if (searchDetails.Holder == null)
//            {
//                var promptMessage = MessageFactory.Text(HolderStepMsgText, HolderStepMsgText, InputHints.ExpectingInput);
//                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
//            }

//            return await stepContext.NextAsync(searchDetails.Holder, cancellationToken);
//        }

//        private async Task<DialogTurnResult> ADTitleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            var searchDetails = (SearchDetails)stepContext.Options;

//            searchDetails.FileName = (string)stepContext.Result;

//            if (searchDetails.ADTitle == null)
//            {
//                var promptMessage = MessageFactory.Text(ADTitleStepMsgText, ADTitleStepMsgText, InputHints.ExpectingInput);
//                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
//            }

//            return await stepContext.NextAsync(searchDetails.ADTitle, cancellationToken);
//        }

//        private async Task<DialogTurnResult> EffectiveDateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            var searchDetails = (SearchDetails)stepContext.Options;

//            searchDetails.ADTitle = (string)stepContext.Result;

//            if (searchDetails.EffectiveDate == null || IsAmbiguous(searchDetails.EffectiveDate))
//            {
//                return await stepContext.BeginDialogAsync(nameof(DateResolverDialog), searchDetails.EffectiveDate, cancellationToken);
//            }

//            return await stepContext.NextAsync(searchDetails.EffectiveDate, cancellationToken);
//        }

//        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            var searchDetails = (SearchDetails)stepContext.Options;

//            searchDetails.EffectiveDate = (string)stepContext.Result;

//            var messageText = $"Please confirm, I have you, {searchDetails.FileName}, {searchDetails.ADTitle}, {searchDetails.EffectiveDate}. Is this correct?";
//            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

//            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
//        }

//        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            if ((bool)stepContext.Result)
//            {
//                var searchDetails = (SearchDetails)stepContext.Options;

//                return await stepContext.EndDialogAsync(searchDetails, cancellationToken);
//            }

//            return await stepContext.EndDialogAsync(null, cancellationToken);
//        }

//        private static bool IsAmbiguous(string timex)
//        {
//            var timexProperty = new TimexProperty(timex);
//            return !timexProperty.Types.Contains(Constants.TimexTypes.Definite);
//        }
//    }
//}
