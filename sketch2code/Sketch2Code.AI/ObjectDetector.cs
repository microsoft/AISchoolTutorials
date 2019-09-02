using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Sketch2Code.AI.Entities;

namespace Sketch2Code.AI
{
    public class ObjectDetector : CustomVisionClient
    {
        private const int numberOfCharsInOperationId = 36;

        public ObjectDetector()
            : base(ConfigurationManager.AppSettings["ObjectDetectionApiKey"],
                   ConfigurationManager.AppSettings["ObjectDetectionPublishedModelName"],
                   ConfigurationManager.AppSettings["ObjectDetectionProjectName"])
        {
        }

        public ObjectDetector(string trainingKey, string predictionKey, string projectName) 
            : base(trainingKey, predictionKey, projectName)
        {

        }

        public async Task<PredictionResult> GetDetectedObjects(byte[] image)
        {
            return await PredictImageAsync(image);
        }

        public async Task<List<String>> GetText(byte[] image)
        {
            var list = new List<String>();
            var lines = await GetTextLines(image);

            if (lines != null)
            {
                list = lines.SelectMany(l => l.Words?.Select(w => w.Text)).ToList();
            }
            
            return list;
        }

        public async Task<List<Line>> GetTextLines(byte[] image)
        {
            try
            {
                using (var ms = new MemoryStream(image))
                {
                    var operation = await _visionClient.BatchReadFileInStreamWithHttpMessagesAsync(ms);
                    var operationLocation = operation.Headers.OperationLocation;

                    // Retrieve the URI where the recognized text will be
                    // stored from the Operation-Location header
                    string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

                    var result = await _visionClient.GetReadOperationResultWithHttpMessagesAsync(operationId);

                    // Wait for the operation to complete
                    int i = 0;
                    while ((result.Body.Status == TextOperationStatusCodes.Running ||
                            result.Body.Status == TextOperationStatusCodes.NotStarted) && i++ < MaxRetries)
                    {
                        Console.WriteLine("Server status: {0}, waiting {1} seconds...", result.Body.Status, i);
                        await Task.Delay(Convert.ToInt32(ConfigurationManager.AppSettings["ComputerVisionDelay"]));

                        result = await _visionClient.GetReadOperationResultWithHttpMessagesAsync(operationId);
                    }

                    return result.Body.RecognitionResults.SelectMany(rs => rs.Lines).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}

