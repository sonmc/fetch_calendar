using Calendar.BE.Dal;
using Calendar.BE.Dto;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

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

        private CalendarService GetService()
        {
            string[] Scopes = { "https://www.googleapis.com/auth/calendar", "https://www.googleapis.com/auth/calendar.readonly" };
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = "475624587151-p1e4spm2s469j4s9g7dq3au2flar356k.apps.googleusercontent.com",
                        ClientSecret = "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"
                    },
                    Scopes,
                    "user",
                    CancellationToken.None).Result;

            if (credential.Token.IsExpired(SystemClock.Default))
                credential.RefreshTokenAsync(CancellationToken.None).Wait();

            var services = new CalendarService(new BaseClientService.Initializer()
            {
                ApplicationName = "Google Canlendar Api",
                HttpClientInitializer = credential
            });
            return services;
        }

        [HttpGet("fetch")]
        public async Task<IActionResult> Fetch(string calendarId)
        {

            var services = GetService();
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
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGoogleCalendar(EventDto eventDto)
        {
            var services = GetService();
            // define request
            var attendees = new List<EventAttendee>();
            EventAttendee a = new EventAttendee();
            a.Email = "sonmc90@gmail.com";
            a.DisplayName = "Sơn Mai";
            attendees.Add(a);
            Event eventCalendar = new Event()
            {
                Summary = eventDto.Summary,
                Attendees = attendees,
                Start = new EventDateTime
                {
                    DateTime = DateTime.Parse(eventDto.StartDate)
                },
                End = new EventDateTime
                {
                    DateTime = DateTime.Parse(eventDto.EndDate)
                },
            };
            var eventRequest = services.Events.Insert(eventCalendar, "maicongson0208@gmail.com");
            var requestCreate = await eventRequest.ExecuteAsync();
            return Ok(requestCreate);
        }

        [HttpGet("getAccounts")]
        public IActionResult GetAccounts()
        {
            var accounts = dbContext.Accounts.ToList();
            return Ok(accounts);
        }
    }
}