// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using SnipInsight.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace SnipInsight.AIServices.AILogic
{
    /// <summary>
    /// Handle the translation service
    /// </summary>
    public class TranslationHandler: CloudService<string>
    {
        /// <summary>
        /// Language codes for the translation
        /// </summary>
        private static string[] languageCodes;
        private string detectedLanguage = string.Empty;

        public TranslationHandler(string keyFile): base(keyFile)
        {
            //<add-endpoint-host-and-get-languages-logic-here>
        }

        /// <summary>
        /// Enable translation if languages loaded
        /// </summary>
        public bool TranslatorEnable { get; set; }

        /// <summary>
        /// Maps the language codes to their full name
        /// </summary>
        public SortedDictionary<string, string> LanguageCodesAndTitles { get; set; }

        /// <summary>
        /// Translate the text and return the translated text and records telemetry
        /// </summary>
        /// <param name="textToTranslate">Text to translate</param>
        /// <returns>Translated text</returns>
        public async Task<string> GetResult(string textToTranslate, string fromLanguage, string toLanguage)
        {
            //TODO: After Refactoring Log results status and api run time
            Telemetry.ApplicationLogger.Instance.SubmitApiCallEvent(Telemetry.EventName.CompleteApiCall, Telemetry.EventName.TranslationApi, -1, "N/A");

            var result = textToTranslate;

            //<add-endpoint-details-here>

            try
            {
                //<add-api-call-here>
            }
            catch (Exception e) when (e is WebException || e is XmlException )
            {
                Diagnostics.LogException(e);
            }
            return result;
        }

        /// <summary>
        /// Get the languages for the translation
        /// </summary>
        private async Task GetLanguagesForTranslate()
        {
            //<add-endpoint-details-and-call-here>
        }

        /// <summary>
        /// Populate the array of string with a list of language codes
        /// </summary>
        private async Task GetLanguageNames()
        {
            //<add-endpoint-details-and-call-here>
        }

        protected override string GetDefaultKey()
        {
            return APIKeys.TranslatorAPIKey;
        }
    }
}
