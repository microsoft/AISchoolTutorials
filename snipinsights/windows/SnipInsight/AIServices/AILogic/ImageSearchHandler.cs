// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using SnipInsight.AIServices.AIModels;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SnipInsight.Util;

namespace SnipInsight.AIServices.AILogic
{
    /// <summary>
    /// API logic for the Image Search service
    /// </summary>
    class ImageSearchHandler : CloudService<ImageSearchModelContainer>
    {
        /// <summary>
        /// Expected number of results for highest confidence on response
        /// </summary>
        private const double ConfidenceThreshold = 10.0;

        /// <summary>
        /// Normalize confidence of result to avoid dominance in suggested search
        /// </summary>
        private const double ConfidenceOffset = 0.2;

        /// <summary>
        /// Constructor to initialize API and client
        /// </summary>
        /// <param name="key">string of API key</param>
        /// <param name="client">Instance of HttpClient to be used for the API call</param>
        public ImageSearchHandler(string keyFile): base(keyFile)

        {
            //<add-endpoint-details-here>

            BuildURI();
        }

        protected override string GetDefaultKey()
        {
            return APIKeys.ImageSearchAPIKey;
        }

        /// <summary>
        /// Make the API call with supplied request contents and records telemetry event with time to complete api call and status code
        /// </summary>
        /// <param name="stream">MemoryStream to read the image byte array</param>
        /// <returns>String response from the API call</returns>
        protected override async Task<HttpResponseMessage> Run(MemoryStream stream)
        {
            //<add-form-creation-logic-here>

            HttpResponseMessage result = null;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                //<add-api-call-here>
            }
            finally
            {
                stopwatch.Stop();
                string responseStatusCode = Telemetry.PropertyValue.NoResponse;
                if (result != null)
                {
                    responseStatusCode = result.StatusCode.ToString();
                }
                Telemetry.ApplicationLogger.Instance.SubmitApiCallEvent(Telemetry.EventName.CompleteApiCall, Telemetry.EventName.CelebrityRecognitionApi, stopwatch.ElapsedMilliseconds, responseStatusCode);
            }

            return result;
        }

    }
}
