using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace what_is.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class What_Is(IConfiguration configuration, IFluentEmail email)
    {

        private readonly IFluentEmail _email = email;


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

                //Console.WriteLine("loaded the documents from send docu function");

                //parse the document
                var parseRes = JsonDocument.Parse(data);

                var root = parseRes.RootElement;
                //return $"{parseRes}";

                //Console.WriteLine(await response.Content.ReadAsStringAsync());
                var items = root.GetProperty("message").GetProperty("items").EnumerateArray();
                string html = string.Empty;

                foreach (var item in items)
                {
                    var doi = item.GetProperty("DOI").GetString();
                    var prefix = item.GetProperty("prefix").GetString();
                    var title = item.GetProperty("title")[0].GetString();
                    var urls = item.GetProperty("URL").GetString();

                    html += $"<div><p>{doi}</p><p>DOI: {doi}</p><p>Prefix: {prefix}</p><p>Title: {title}</p><p>Link: {urls}</p></div>\n";
                }


                return await HandleEmailSending(seachBody.Email, seachBody.Word, html);

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public async Task<string> HandleEmailSending(
            string to,
            string subject,
             string items
        )
        {
            try
            {
                Console.WriteLine("Touched the sending email func");

                var responses = await _email
               .To(to.Trim())
               .Subject(subject)
               .Body(items)
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
