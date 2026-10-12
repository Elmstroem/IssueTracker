using IssueTracker.Api.Dtos;
using IssueTracker.Api.Services;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
namespace IssueTracker.Api.Endpoints;

public static class TicketEndpoint
{
	public static void MapTicketEndpoints(
		this WebApplication app)
	{
		// ============================================
		// GET - Hämta alla supportärenden
		// ============================================

		app.MapGet("/api/tickets", async (
			TicketService ticketService) =>
		{
			var tickets =
				await ticketService.GetTickets();

			return Results.Ok(tickets);
		});


		// ============================================
		// POST - Skapa supportärende
		// ============================================

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


		// ============================================
		// DELETE - Radera supportärende
		// ============================================

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


		// ============================================
		// PUT - Uppdatera supportärende
		// ============================================

		app.MapPut("/api/tickets/{id}", async (
			int id,
			UpdateTicketDto updateTicketDto,
			TicketService ticketService) =>
		{
			var errors =
				ValidateUpdateTicket(updateTicketDto);

			if (errors.Count > 0)
			{
				return Results.ValidationProblem(errors);
			}

			bool updated =
				await ticketService.UpdateTicket(
					id,
					updateTicketDto);

			if (!updated)
			{
				return Results.NotFound();
			}

			return Results.NoContent();
		});

		app.MapPatch("/api/tickets/{id}", async (
	int id,
	JsonPatchDocument<UpdateTicketDto> patchDocument,
	TicketService ticketService) =>
		{
			// Hämta ticketen som redan finns i databasen
			var existingTicket =
				await ticketService.GetTicketById(id);

			if (existingTicket is null)
			{
				return Results.NotFound();
			}


			// Skapa ett objekt med ticketens nuvarande värden
			var ticketToUpdate = new UpdateTicketDto
			{
				Title = existingTicket.Title,
				Description = existingTicket.Description,
				Status = existingTicket.Status,
				Priority = existingTicket.Priority
			};


			// Samla eventuella fel från JSON Patch
			var patchErrors =
				new List<string>();

			patchDocument.ApplyTo(
				ticketToUpdate,
				patchError =>
				{
					patchErrors.Add(
						patchError.ErrorMessage);
				});


			if (patchErrors.Count > 0)
			{
				return Results.BadRequest(
					new
					{
						Errors = patchErrors
					});
			}


			// Återanvänd samma validering som PUT
			var validationErrors =
				ValidateUpdateTicket(
					ticketToUpdate);

			if (validationErrors.Count > 0)
			{
				return Results.ValidationProblem(
					validationErrors);
			}


			// Spara den färdiga uppdateringen
			bool updated =
				await ticketService.UpdateTicket(
					id,
					ticketToUpdate);

			if (!updated)
			{
				return Results.NotFound();
			}


			return Results.NoContent();
		})
	.Accepts<JsonPatchDocument<UpdateTicketDto>>("application/json-patch+json");
	}


	// ============================================
	// VALIDERING - CREATE
	// ============================================

	private static Dictionary<string, string[]>
		ValidateCreateTicket(
			CreateTicketDto ticket)
	{
		var errors =
			new Dictionary<string, string[]>();


		// Title
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


		// Description
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


		// Priority
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


	// ============================================
	// VALIDERING - UPDATE
	// ============================================

	private static Dictionary<string, string[]>
		ValidateUpdateTicket(
			UpdateTicketDto ticket)
	{
		var errors =
			new Dictionary<string, string[]>();


		// Title
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


		// Description
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


		// Status
		if (string.IsNullOrWhiteSpace(ticket.Status))
		{
			errors["Status"] =
				["Du måste ange en status."];
		}
		else
		{
			string status =
				ticket.Status.Trim();

			if (
				status != "Open" &&
				status != "InProgress" &&
				status != "Closed")
			{
				errors["Status"] =
					["Status måste vara Open, InProgress eller Closed."];
			}
		}


		// Priority
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