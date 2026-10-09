using Dapper;
using IssueTracker.Api.Dtos;
using IssueTracker.Api.Models;
using Microsoft.Data.SqlClient;

namespace IssueTracker.Api.Services;

public class TicketService
{
	private readonly IConfiguration _configuration;

	public TicketService(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public async Task<IEnumerable<TicketDto>> GetTickets()
	{
		string connectionString =
			_configuration.GetConnectionString("DefaultConnection")!;

		await using var connection =
			new SqlConnection(connectionString);

		string sql = """
            SELECT
                Id,
                Title,
                Description,
                Status,
                Priority
            FROM dbo.Tickets;
            """;

		var tickets =
			await connection.QueryAsync<Ticket>(sql);

		var ticketDtos =
			tickets.Select(ticket => new TicketDto
			{
				Id = ticket.Id,
				Title = ticket.Title,
				Description = ticket.Description,
				Status = ticket.Status,
				Priority = ticket.Priority
			});

		return ticketDtos;
	}
}