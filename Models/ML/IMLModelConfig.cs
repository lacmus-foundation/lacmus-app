using RescuerLaApp.Models.Docker;

namespace RescuerLaApp.Models.ML
{
    public interface IMLModelConfig
    {
        IDockerImage Image { get; set; }
        IDockerAccaunt Accaunt { get; set; }
    }
}