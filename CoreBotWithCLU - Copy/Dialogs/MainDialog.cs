// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Dialogs
{

    public class MainDialog : ComponentDialog
    {
        private readonly FlightBookingRecognizer _cluRecognizer;
        protected readonly ILogger Logger;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(FlightBookingRecognizer cluRecognizer, SearchDialog bookingDialog, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _cluRecognizer = cluRecognizer;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));
            AddDialog(bookingDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

  

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_cluRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: CLU is not configured. To enable all capabilities, add 'CluProjectName', 'CluDeploymentName', 'CluAPIKey' and 'CluAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }

            // Use the text provided in FinalStepAsync or the default if it is the first time.
            var EffectiveDate = DateTime.Now.AddDays(7).ToString("MMMM d, yyyy");
            var weekLaterDate = DateTime.Now.AddDays(7).ToString("MMMM d, yyyy");
            var messageText = stepContext.Options?.ToString() ?? $"Welcome! What can I help you with today? \n Say something like \"Can i view a file called bae 146-140\"";
            var promptMessage = MessageFactory.Text(messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }


        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_cluRecognizer.IsConfigured)
            {
                // CLU is not configured, we just run the BookingDialog path with an empty BookingDetailsInstance.
                return await stepContext.BeginDialogAsync(nameof(SearchDialog), new SearchDetails(), cancellationToken);
            }

            // Call CLU and gather any potential booking details. (Note the TurnContext has the response to the prompt.)
            var cluResult = await _cluRecognizer.RecognizeAsync<FlightBooking>(stepContext.Context, cancellationToken);
            switch (cluResult.GetTopIntent().intent)
            {
                case FlightBooking.Intent.FileName:
                    // Initialize SearchDetails with any entities we may have found in the response.
                    var searchDetails = new SearchDetails()
                    {
                        FileName = cluResult.Entities.GetFileName(),
                        ADTitle = cluResult.Entities.GetADTitle(),
                        ADReferenceNumber = cluResult.Entities.GetADReferenceNumber(),
                        AircraftType = cluResult.Entities.GetAircraftType(),
                        AircraftSerialNumber = cluResult.Entities.GetAircraftSerialNumber(),
                        Holder = cluResult.Entities.GetHolder(),
                        Problem = cluResult.Entities.GetProblem(),
                        EffectiveDate = cluResult.Entities.GetEffectiveDate()
                    };

                    // Run the BookingDialog giving it whatever details we have from the CLU call, it will fill out the remainder.
                    return await stepContext.BeginDialogAsync(nameof(SearchDialog), searchDetails, cancellationToken);

                case FlightBooking.Intent.EffectiveDate:
                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.
                    var getEffectiveDateText = "TODO: get Effective Date flow here";
                    var getEffectiveDateMessage = MessageFactory.Text(getEffectiveDateText, getEffectiveDateText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(getEffectiveDateMessage, cancellationToken);
                    break;

                case FlightBooking.Intent.Issue:
                    // we haven't implemented the getweatherdialog so we just display a todo message.
                    var getIssuetext = "todo: get Issue flow here";
                    var getIssuemessage = MessageFactory.Text(getIssuetext, getIssuetext, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(getIssuemessage, cancellationToken);
                    break;

                default:
                    // Catch all for unhandled intents
                    var didntUnderstandMessageText = $"Sorry, I didn't get that. Please try asking in a different way (intent was {cluResult.GetTopIntent().intent})";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // the Result here will be null.
            if (stepContext.Result is SearchDetails result)
            {
                // Now we have all the booking details call the booking service.

                // If the call to the booking service was successful tell the user.

                //var timeProperty = new TimexProperty(result.TravelDate);
                //var travelDateMsg = timeProperty.ToNaturalLanguage(DateTime.Now);
                var messageText = $"I'll search for a document for a file name: {result.FileName}";
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }

            // Restart the main dialog with a different message the second time around
            var promptMessage = "Do you have more documents you are searching for?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}
