namespace LacmusApp.Models.Docker
{
    public interface IDockerImage
    {
        string Tag { get; set; }
        string Name { get; set; }
    }
}