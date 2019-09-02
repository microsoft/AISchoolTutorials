// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using SnipInsight.AIServices.AIModels;
using SnipInsight.Util;

namespace SnipInsight.AIServices.AILogic
{
    /// <summary>
    /// API logic for the Image Search service
    /// </summary>
    class ImageAnalysisHandler : CloudService<ImageAnalysisModel>
    {
        /// <summary>
        /// Constructor to initialize API and client
        /// </summary>
        /// <param name="key">string of API key</param>
        /// <param name="client">Instance of HttpClient to be used for the API call</param>
        internal ImageAnalysisHandler(string key): base(key)
        {
            //<add-endpoint-details-here>

            BuildURI();
        }

        protected override string GetDefaultKey()
        {
            return APIKeys.ImageAnalysisAndTextRecognitionAPIKey;
        }
    }
}
