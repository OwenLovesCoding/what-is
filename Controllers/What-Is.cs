using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace what_is.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class What_Is(IConfiguration configuration)
    {

        private readonly IFluentEmail _email;

        [HttpPost("/get-c")]
        public async Task<object> FindMeaning([FromBody] Words seachBody)
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

                var data = await response.Content.ReadAsStringAsync();

                Console.WriteLine("loaded the documents from send docu function");

                //parse the document
                var parseRes = JsonDocument.Parse(data);

                var root = parseRes.RootElement;
                //return $"{parseRes}";

                //Console.WriteLine(await response.Content.ReadAsStringAsync());
                var items = root.GetProperty("message").GetProperty("items").EnumerateArray();

                //foreach (var item in items.EnumerateArray()) { }



                return await HandleEmailSending(seachBody.Email, seachBody.Word, ["home home home"]);

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
            try
            {
                Console.WriteLine("Touched the sending email func");

                //return $"This is the recipient - {to} and this is the items - {items[0]}";
                string req = items[0];

                if (_email != null)
                {

                    var responses = await _email
                   .To(to)
                   .Subject(subject)
                   .Body(req)
                   .SendAsync();
                }
                else
                {
                    return "email is invalid";
                }


                return "Message sent";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

}
