using IssueTracker.Api.Services;

namespace IssueTracker.Api.Endpoints;

public static class TicketEndpoint
{
	public static void MapTicketEndpoints(
		this WebApplication app)
	{
		app.MapGet("/api/tickets", async (
			TicketService ticketService) =>
		{
			var tickets =
				await ticketService.GetTickets();

			return Results.Ok(tickets);
		});
	}
}