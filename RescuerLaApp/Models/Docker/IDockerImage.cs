namespace RescuerLaApp.Models.Docker
{
    public interface IDockerImage
    {
        string Tag { get; set; }
        string Name { get; set; }
    }
}