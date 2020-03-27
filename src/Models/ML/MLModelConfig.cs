using Newtonsoft.Json;
using LacmusApp.Extensions;
using LacmusApp.Models.Docker;

namespace LacmusApp.Models.ML
{
    [JsonObject]
    public class MLModelConfig : IMLModelConfig
    {
        private uint _apiVersion = 1;
        private uint _modelVersion = 0;
        private MLModelType _type = MLModelType.Cpu;
        public string Url { get; set; } = "http://localhost:5000";
        public IDockerImage Image { get; set; } = new DockerImage
        {
            Name = "gosha20777/lacmus",
            Tag = MLModelConfigExtension.GetDockerTag(1, 0, MLModelType.Cpu)
        };
        public IDockerAccaunt Accaunt { get; set; } = new DockerAccaunt
        {
            Email = "lizaalertai@yandex.ru",
            Password = "9ny?Mh4b*qfThZ6T",
            Username = "lizaalertai"
        };

        public MLModelType Type
        {
            get => _type;
            set 
            { 
                _type = value;
                Image.Tag = MLModelConfigExtension.GetDockerTag(_apiVersion, _modelVersion, _type);
            }
        }
        public uint ApiVersion 
        {
            get => _apiVersion;
            set 
            { 
                _apiVersion = value;
                Image.Tag = MLModelConfigExtension.GetDockerTag(_apiVersion, _modelVersion, _type);
            }
        }
        public uint ModelVersion
        {
            get => _modelVersion;
            set 
            { 
                _modelVersion = value;
                Image.Tag = MLModelConfigExtension.GetDockerTag(_apiVersion, _modelVersion, _type);
            }
        }
    }
}