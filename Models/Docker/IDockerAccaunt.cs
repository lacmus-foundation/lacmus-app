namespace RescuerLaApp.Models.Docker
{
    public interface IDockerAccaunt
    {
        string Email { get; set; }
        string Username { get; set; }
        string Password { get; set; }
    }
}