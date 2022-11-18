namespace Calendar.BE.Dto
{
    public class AuthDto
    {
        public string AccessToken { get; set; }
        public string CalendarId { get; set; }
        public string RefreshToken { get; set; }
    }
}
