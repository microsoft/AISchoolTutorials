// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using SnipInsight.AIServices.AIModels;
using SnipInsight.Util;

namespace SnipInsight.AIServices.AILogic
{
    /// <summary>
    /// Printed Text OCR call
    /// </summary>
    class PrintedTextHandler : CloudService<PrintedModel>
    {
        /// <summary>
        /// Initalizes handler with correct endpoint
        /// </summary>
        public PrintedTextHandler(string keyFile) : base(keyFile)
        {
            //<add-endpoint-details-here>
        }

        protected override string GetDefaultKey()
        {
            return APIKeys.ImageAnalysisAndTextRecognitionAPIKey;
        }
    }
}
