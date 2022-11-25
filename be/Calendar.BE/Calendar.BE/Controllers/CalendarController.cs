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

        private CalendarService GetService(string clientId)
        {
            string[] Scopes = { "https://www.googleapis.com/auth/calendar", "https://www.googleapis.com/auth/calendar.readonly" };
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = clientId,
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
        public async Task<IActionResult> Fetch(string calendarId, string clientId)
        {

            var services = GetService(clientId);
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
            var services = GetService(eventDto.ClientId);
            // define request
            var attendees = new List<EventAttendee>();
            EventAttendee a = new EventAttendee();
           
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
            var eventRequest = services.Events.Insert(eventCalendar, eventDto.CalendarId);
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