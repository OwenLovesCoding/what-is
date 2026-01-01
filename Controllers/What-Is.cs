using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace what_is.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class What_Is(IConfiguration configuration)
    {



        [HttpGet("/get-c")]
        public async Task<string> FindMeaning([FromBody] Words seachBody)
        {
            //var brevoPort = configuration["Brevo:SmtpServer"];

            var client = new HttpClient();

            try
            {
                var url =
                $"https://api.crossref.org/v1/works" +
                $"?rows=10" +
                $"&select=DOI,prefix,title,URL" +
                $"&order=desc" +
                $"&mailto={seachBody.Email}" +
                $"&query.bibliographic={seachBody.Word}";


                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                //var data = await response.Content.ReadFromJsonAsync();

                return await HandleEmailSending(seachBody.Email, seachBody.Word);

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<string> HandleEmailSending(
            string to,
            string subject,
            string[] items,
            string response,
            [Optional] IConfiguration configuration
        )
        {
            var body = new
            {
                to = to,
                subject = subject,
                items = items,
                response = response
            };

            var json = JsonSerializer.Serialize(body);

            string apiKey = configuration["Brevo:BrevoApiKey"]!;

            return apiKey;

            try
            {
                var request = new HttpRequestMessage(
                 HttpMethod.Post,
                 "https://api.brevo.com/v3/smtp/email"
             );

                //request.Headers.Add("api-key", );
                request.Headers.Add("accept", "application/json");

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

}
