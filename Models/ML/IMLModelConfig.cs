using System;
using RescuerLaApp.Models.Docker;

namespace RescuerLaApp.Models.ML
{
    public interface IMLModelConfig
    {
        string Url { get; set; }
        IDockerImage Image { get; set; }
        IDockerAccaunt Accaunt { get; set; }
    }
}