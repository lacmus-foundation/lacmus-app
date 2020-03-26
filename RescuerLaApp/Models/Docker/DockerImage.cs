namespace RescuerLaApp.Models.Docker
{
    public class DockerImage : IDockerImage
    {
        public string Tag { get; set; }
        public string Name { get; set; }
    }
}