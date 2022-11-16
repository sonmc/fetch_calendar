using Calendar.BE.Dal;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.BE.Controllers
{
    [ApiController]
    [Route("api/calendars")]
    public class CalendarController : Controller
    {
        private readonly DataContext dbContext;
        public CalendarController(DataContext _dbContext)
        {
            dbContext = _dbContext;
        }

        [HttpGet("fetch")]
        public async Task<IActionResult> Fetch(string calendarId)
        {
            string[] Scopes = { "https://www.googleapis.com/auth/calendar" };
            string ApplicationName = "Google Canlendar Api";
            UserCredential credential;
            using (var stream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "token.json"), FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None).Result;
            } 
            var services = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            }); 
            var request = services.Events.List(calendarId);
            try
            {
                var response = await request.ExecuteAsync();
                return Ok(response.Items);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return BadRequest();
        }

        [HttpGet("getAccounts")]
        public IActionResult GetAccounts()
        {
            var accounts = dbContext.Accounts.ToList();
            return Ok(accounts);
        }
    }
}