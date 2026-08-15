namespace ToDoList.Application.Settings
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public int Lifetime { get; set; }
        public int RefreshTokenLifetimeDays { get; set; }
    }
}
