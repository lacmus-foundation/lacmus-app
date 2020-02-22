using Newtonsoft.Json;
using RescuerLaApp.Models.Docker;

namespace RescuerLaApp.Models.ML
{
    [JsonObject]
    public class MLModelConfig : IMLModelConfig
    {
        public string Url { get; set; }
        public IDockerImage Image { get; set; }
        public IDockerAccaunt Accaunt { get; set; }
    }
}