using System;
using LacmusApp.Models.Docker;

namespace LacmusApp.Models.ML
{
    public interface IMLModelConfig
    {
        string Url { get; set; }
        IDockerImage Image { get; set; }
        IDockerAccaunt Accaunt { get; set; }
    }
}