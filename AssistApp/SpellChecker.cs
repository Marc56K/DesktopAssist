using OllamaSharp;
using System.Text;

namespace AssistApp
{
    internal class SpellChecker
    {
        private OllamaApiClient? _client = null;

        private OllamaApiClient client 
        { 
            get
            {
                if (_client != null)
                    return _client;

                var uri = new Uri("http://192.168.0.106:11435");
                _client = new OllamaApiClient(uri, "llama3.1:8b");
                return _client;
            }
        }


        public string fixSpelling(string text)
        {
            var prompt = $"Fix spelling and wording of the following text and only return the fixed text without any additional quotes, explanations, or formatting. {text}";
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
