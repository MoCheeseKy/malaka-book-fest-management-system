namespace WinformsGUI.Utils
{
    public static class Config
    {
        // Adjust the port if necessary based on how the API is launched
        public static string ApiBaseUrl { get; set; } = "http://localhost:5278/api/";
        public static string JwtToken { get; set; } = string.Empty;

        public static bool IsLoggedIn => !string.IsNullOrEmpty(JwtToken);
    }
}
