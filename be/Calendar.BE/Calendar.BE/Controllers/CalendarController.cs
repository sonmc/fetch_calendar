using Calendar.BE.Dal;
using Calendar.BE.Dto;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
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

        [HttpGet("verify")]
        public async Task<IActionResult> Verify(string code)
        {
            RestClient restClient = new RestClient("https://oauth2.googleapis.com/token");
            RestRequest request = new RestRequest();
            request.AddQueryParameter("client_id", "475624587151-p1e4spm2s469j4s9g7dq3au2flar356k.apps.googleusercontent.com");
            request.AddQueryParameter("code", code);
            request.AddQueryParameter("client_secret", "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");
            request.AddQueryParameter("grant_type", "authorization_code");
            try
            {
                var response = restClient.Post(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return Ok("");
        }

        [HttpPost("fetch")]
        public async Task<IActionResult> Fetch(AuthDto auth)
        {  

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "475624587151-p1e4spm2s469j4s9g7dq3au2flar356k.apps.googleusercontent.com",
                    ClientSecret = "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"
                },
                Scopes = new[] { CalendarService.Scope.Calendar }
            });


            var credential = new UserCredential(flow, Environment.UserName, new TokenResponse
            {
                AccessToken = auth.AccessToken
            });

            var services = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Canlendar Api",
            });

            var request = services.Events.List(auth.CalendarId);
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

            // define services
            var services = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // define request
            Event eventCalendar = new Event()
            {
                Summary = eventDto.Summary,
                Location = eventDto.Location,
                Start = new EventDateTime
                {
                    DateTime = eventDto.Start,
                    TimeZone = "Asia/Ho_Chi_Minh"
                },
                End = new EventDateTime
                {
                    DateTime = eventDto.End,
                    TimeZone = "Asia/Ho_Chi_Minh"
                },
                Description = eventDto.Description
            };
            var eventRequest = services.Events.Insert(eventCalendar, "primary");
            var requestCreate = await eventRequest.ExecuteAsync();
            return Ok();
        }

        [HttpGet("getAccounts")]
        public IActionResult GetAccounts()
        {
            var accounts = dbContext.Accounts.ToList();
            return Ok(accounts);
        }
    }
}