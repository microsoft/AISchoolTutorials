using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Newtonsoft.Json;
using Sketch2Code.AI.Entities;

namespace Sketch2Code.AI
{
    public abstract class CustomVisionClient
    {
        protected const int MaxRetries = 10;
        protected const string CustomVisionEndpoint = "https://westus2.api.cognitive.microsoft.com";

        // Shared instance: https://github.com/mspnp/performance-optimization/blob/master/ImproperInstantiation/docs/ImproperInstantiation.md
        private static readonly HttpClient Client = new HttpClient { Timeout = TimeSpan.FromMinutes(1) };

        protected string _apiKey;
        protected string _publishedModelName;
        protected string _projectName;
        protected string _projectId;
        protected ComputerVisionClient _visionClient;

        public CustomVisionClient(string apiKey, string publishedModelName, string projectName)
        {
            _apiKey = apiKey;
            _publishedModelName = publishedModelName;
            _projectName = projectName;

            // Initialize Computer Vision Client
            
        }

        public virtual void Initialize()
        {
            if (String.IsNullOrWhiteSpace(_projectName)) throw new ArgumentNullException("projectName");

            var uri = $"{CustomVisionEndpoint}/customvision/v3.0/training/projects";

            string responseContent;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                requestMessage.Headers.Add("Training-Key", _apiKey);
                var response = Client.SendAsync(requestMessage).Result;
                responseContent = response.Content.ReadAsStringAsync().Result;
            }

            var projects = JsonConvert.DeserializeObject<List<Project>>(responseContent);

            var project = projects.SingleOrDefault(p => p.Name.Equals(_projectName, StringComparison.InvariantCultureIgnoreCase));
            
            if (project == null) throw new InvalidOperationException($"CustomVision client failed to initialize. ({_projectName} Not Found.)");
            this._projectId = project.Id;
        }

        public async Task<PredictionResult> PredictImageAsync(byte[] imageData)
        {
            throw new NotImplementedException();
        }
    }
}
