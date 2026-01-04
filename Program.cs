using System.Net;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


//Configure fluentemail
//var emailSettings = builder.Configuration.GetSection("Brevo");
var smtpClient = new SmtpClient(builder.Configuration["EmailSettings:SmtpServer"])
{
    Port = int.Parse(builder.Configuration["EmailSettings:SmtpPort"]!),
    Credentials = new NetworkCredential(
        builder.Configuration["EmailSettings:SmtpLogin"],
        builder.Configuration["EmailSettings:SmtpPassword"]),
    EnableSsl = true
};

builder.Services
    .AddFluentEmail(builder.Configuration["EmailSettings:SmtpSender"], builder.Configuration["EmailSettings:SmtpSender"])
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
