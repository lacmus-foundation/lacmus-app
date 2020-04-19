using LacmusApp.Models.ML;
using ReactiveUI.Fody.Helpers;

namespace LacmusApp.Models
{
    public class MlModelData
    {
        [Reactive] public string Name { get; set; }
        [Reactive] public MLModelType Type { get; set; }
        [Reactive] public uint Version { get; set; }
        [Reactive] public uint ApiVersion { get; set; }

        public MlModelData(string name, MLModelType type, uint version, uint apiVersion)
        {
            Name = name;
            Type = type;
            Version = version;
            ApiVersion = apiVersion;
        }
    }
}