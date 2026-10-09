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


	// ============================================
	// Hämta alla supportärenden
	// ============================================

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


	// ============================================
	// Skapa nytt supportärende
	// ============================================

	public async Task<TicketDto> CreateTicket(
		CreateTicketDto createTicketDto)
	{
		string connectionString =
			_configuration.GetConnectionString("DefaultConnection")!;

		await using var connection =
			new SqlConnection(connectionString);

		string sql = """
            INSERT INTO dbo.Tickets
            (
                Title,
                Description,
                Status,
                Priority
            )
            OUTPUT
                INSERTED.Id,
                INSERTED.Title,
                INSERTED.Description,
                INSERTED.Status,
                INSERTED.Priority
            VALUES
            (
                @Title,
                @Description,
                @Status,
                @Priority
            );
            """;

		var ticket =
			await connection.QuerySingleAsync<Ticket>(
				sql,
				new
				{
					Title = createTicketDto.Title.Trim(),
					Description = createTicketDto.Description.Trim(),
					Status = "Open",
					Priority = createTicketDto.Priority.Trim()
				});

		return new TicketDto
		{
			Id = ticket.Id,
			Title = ticket.Title,
			Description = ticket.Description,
			Status = ticket.Status,
			Priority = ticket.Priority
		};
	}
}