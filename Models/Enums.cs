namespace RescuerLaApp.Models
{
    public static class Enums
    {
        public enum Status        
        {
            Ready = 1,
            Working,
            Success,
            Unauthenticated,
            Error
        }
        
        public enum ImageLoadMode
        {
            Miniature,
            Full
        }
    }
}
