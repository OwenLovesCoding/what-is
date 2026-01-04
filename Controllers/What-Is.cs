using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace what_is.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class What_Is(IConfiguration configuration)
    {

        private readonly IFluentEmail? email;

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

                return await HandleEmailSending(seachBody.Email, seachBody.Word, []);

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
            [Optional] IConfiguration configuration
        )
        {
            var body = new
            {
                to = to,
                subject = subject,
                items = items,
            };



            try
            {
                var responses = await email!
               .To(to)
               .Subject(subject)
               .Body(items[0])
               .SendAsync();


                return "Message sent";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

}
