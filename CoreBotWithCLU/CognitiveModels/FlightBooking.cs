// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder;
using Microsoft.BotBuilderSamples.Clu;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// An <see cref="IRecognizerConvert"/> implementation that provides helper methods and properties to interact with
    /// the CLU recognizer results.
    /// </summary>
    public class FlightBooking : IRecognizerConvert
    {
        public enum Intent
        {
            EffectiveDate,
            FileName,
            Issue,
            None
        }

        public string Text { get; set; }

        public string AlteredText { get; set; }

        public Dictionary<Intent, IntentScore> Intents { get; set; }

        public CluEntities Entities { get; set; }

        public IDictionary<string, object> Properties { get; set; }

        public void Convert(dynamic result)
        {
            var jsonResult = JsonConvert.SerializeObject(result, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
            var app = JsonConvert.DeserializeObject<FlightBooking>(jsonResult);

            Text = app.Text;
            AlteredText = app.AlteredText;
            Intents = app.Intents;
            Entities = app.Entities;
            Properties = app.Properties;
        }

        public (Intent intent, double score) GetTopIntent()
        {
            var maxIntent = Intent.None;
            var max = 0.0;
            foreach (var entry in Intents)
            {
                if (entry.Value.Score > max)
                {
                    maxIntent = entry.Key;
                    max = entry.Value.Score.Value;
                }
            }

            return (maxIntent, max);
        }

        public class CluEntities
        {
            public CluEntity[] Entities;

            public CluEntity[] GetActionList() => Entities.Where(e => e.Category == "Action").ToArray();

            public CluEntity[] GetADReferenceNumberList() => Entities.Where(e => e.Category == "ADReferenceNumber").ToArray();

            public CluEntity[] GetADTitleList() => Entities.Where(e => e.Category == "ADTitle").ToArray();

            public CluEntity[] GetAircraftSerialNumberList() => Entities.Where(e => e.Category == "AircraftSerialNumber").ToArray();

            public CluEntity[] GetAircraftTypeList() => Entities.Where(e => e.Category == "AircraftType").ToArray();
            
            public CluEntity[] GetEffectiveDateList() => Entities.Where(e => e.Category == "EffectiveDate").ToArray();
            
            public CluEntity[] GetFileNameList() => Entities.Where(e => e.Category == "FileName").ToArray();
            
            public CluEntity[] GetHolderList() => Entities.Where(e => e.Category == "Holder").ToArray();

            public CluEntity[] GetProblemList() => Entities.Where(e => e.Category == "Problem").ToArray();

            public string GetAction() => GetActionList().FirstOrDefault()?.Text;

            public string GetADReferenceNumber() => GetADReferenceNumberList().FirstOrDefault()?.Text;

            public string GetADTitle() => GetADTitleList().FirstOrDefault()?.Text;

            public string GetAircraftSerialNumber() => GetAircraftSerialNumberList().FirstOrDefault()?.Text;


            public string GetAircraftType() => GetAircraftTypeList().FirstOrDefault()?.Text;

            public string GetEffectiveDate() => GetEffectiveDateList().FirstOrDefault()?.Text;

            public string GetFileName() => GetFileNameList().FirstOrDefault()?.Text;

            public string GetHolder() => GetHolderList().FirstOrDefault()?.Text;

            public string GetProblem() => GetProblemList().FirstOrDefault()?.Text;

        }
    }
}
