using IssueTracker.Api.Dtos;
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


		app.MapPost("/api/tickets", async (
			CreateTicketDto createTicketDto,
			TicketService ticketService) =>
		{
			var errors =
				ValidateCreateTicket(createTicketDto);

			if (errors.Count > 0)
			{
				return Results.ValidationProblem(errors);
			}

			var createdTicket =
				await ticketService.CreateTicket(
					createTicketDto);

			return Results.Created(
				$"/api/tickets/{createdTicket.Id}",
				createdTicket);
		});


		app.MapDelete("/api/tickets/{id}", async (
			int id,
			TicketService ticketService) =>
		{
			bool deleted =
				await ticketService.DeleteTicket(id);

			if (!deleted)
			{
				return Results.NotFound();
			}

			return Results.NoContent();
		});
	}


	private static Dictionary<string, string[]>
		ValidateCreateTicket(
			CreateTicketDto ticket)
	{
		var errors =
			new Dictionary<string, string[]>();

		if (string.IsNullOrWhiteSpace(ticket.Title))
		{
			errors["Title"] =
				["Titeln får inte vara tom eller endast innehålla blanksteg."];
		}
		else if (
			ticket.Title.Trim().Length < 5 ||
			ticket.Title.Trim().Length > 100)
		{
			errors["Title"] =
				["Titeln måste vara mellan 5 och 100 tecken."];
		}

		if (string.IsNullOrWhiteSpace(ticket.Description))
		{
			errors["Description"] =
				["Beskrivningen får inte vara tom eller endast innehålla blanksteg."];
		}
		else if (
			ticket.Description.Trim().Length < 10 ||
			ticket.Description.Trim().Length > 1000)
		{
			errors["Description"] =
				["Beskrivningen måste vara mellan 10 och 1000 tecken."];
		}

		if (string.IsNullOrWhiteSpace(ticket.Priority))
		{
			errors["Priority"] =
				["Du måste ange en prioritet."];
		}
		else
		{
			string priority =
				ticket.Priority.Trim();

			if (
				priority != "Low" &&
				priority != "Normal" &&
				priority != "High")
			{
				errors["Priority"] =
					["Prioriteten måste vara Low, Normal eller High."];
			}
		}

		return errors;
	}
}