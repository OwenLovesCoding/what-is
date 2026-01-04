using System.Net;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


//Configure fluentemail
var emailSettings = builder.Configuration.GetSection("Brevo");
var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
{
    Port = int.Parse(emailSettings["Port"]!),
    Credentials = new NetworkCredential(
        emailSettings["Login"],
        emailSettings["BrevoPassword"]),
    EnableSsl = true
};

builder.Services
    .AddFluentEmail(emailSettings["BrevoSender"], emailSettings["BrevoSender"])
    .AddRazorRenderer()
    .AddSmtpSender(smtpClient);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
