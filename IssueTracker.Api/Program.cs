using IssueTracker.Api.Endpoints;
using IssueTracker.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Registrerar TicketService för Dependency Injection
builder.Services.AddScoped<TicketService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// Kopplar in endpoints från TicketEndpoint.cs
app.MapTicketEndpoints();

app.Run();