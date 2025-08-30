using Microsoft.VisualBasic;
using OllamaSharp;
using System.ComponentModel;
using System.Text;

namespace AssistApp
{
    internal class SpellChecker
    {
        private OllamaApiClient? _client = null;
        private Task? _pullTask = null;
        public Logger? _logger = null;

        public SpellChecker(Logger? logger)
        {
            _logger = logger;
        }

        private OllamaApiClient client
        {
            get
            {
                if (_client != null)
                    return _client;

                var uri = new Uri(Properties.Settings.Default.OllamaURL);
                _client = new OllamaApiClient(uri, Properties.Settings.Default.Model);
                return _client;
            }
        }

        public void PullModel()
        {
            try
            {
                var models = client.ListLocalModelsAsync().Result.ToList();
                if (!models.Any(m => m.Name == Properties.Settings.Default.Model))
                {
                    _logger?.Info($"Model '{Properties.Settings.Default.Model}' not found locally. Attempting to pull the model...");
                    var pullResponses = client.PullModelAsync(Properties.Settings.Default.Model);

                    // Warten Sie asynchron auf die Pull-Responses und geben Sie Fortschritt aus
                    _pullTask = Task.Run(async () =>
                    {
                        string last_message = "";
                        await foreach (var response in pullResponses)
                        {
                            if (response != null)
                            {
                                string message = $"'{Properties.Settings.Default.Model}' Status: {response.Status}, {Math.Round(response.Percent, 0)}%";
                                if (message != last_message)
                                {
                                    _logger?.Info(message);
                                    last_message = message;
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error while pulling model '{Properties.Settings.Default.Model}': {ex.Message}");
            }
        }

        public void CheckModel()
        {
            if (_pullTask == null || _pullTask.IsCompletedSuccessfully)
                return; // No pull in progress, assume model is available

            if (_pullTask.Status == TaskStatus.Running)
                throw new Exception("The model is still being pulled. Please wait until the pull is complete.");  

            if (_pullTask.IsFaulted)
                throw new Exception($"Failed to pull model '{Properties.Settings.Default.Model}' from Ollama server. {_pullTask.Exception.Message}");
        }

        public string FixSpelling(string systemPrompt, string text)
        {
            CheckModel();
            var prompt = systemPrompt.Replace("[TEXT]", text);
            var sb = new StringBuilder();
            foreach (var chunk in client.GenerateAsync(prompt).ToBlockingEnumerable())
            {
                if (chunk != null)
                    sb.Append(chunk.Response);
            }
            return sb.ToString();
        }
    }
}
