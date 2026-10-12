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

	public async Task<bool> DeleteTicket(int id)
	{
		string connectionString =
			_configuration.GetConnectionString("DefaultConnection")!;

		await using var connection =
			new SqlConnection(connectionString);

		string sql = """
        DELETE FROM dbo.Tickets
        WHERE Id = @Id;
        """;

		int rowsAffected =
			await connection.ExecuteAsync(
				sql,
				new
				{
					Id = id
				});

		return rowsAffected > 0;
	}

	public async Task<bool> UpdateTicket(
	int id,
	UpdateTicketDto updateTicketDto)
	{
		string connectionString =
			_configuration.GetConnectionString("DefaultConnection")!;

		await using var connection =
			new SqlConnection(connectionString);

		string sql = """
        UPDATE dbo.Tickets
        SET
            Title = @Title,
            Description = @Description,
            Status = @Status,
            Priority = @Priority
        WHERE Id = @Id;
        """;

		int rowsAffected =
			await connection.ExecuteAsync(
				sql,
				new
				{
					Id = id,
					Title = updateTicketDto.Title.Trim(),
					Description = updateTicketDto.Description.Trim(),
					Status = updateTicketDto.Status.Trim(),
					Priority = updateTicketDto.Priority.Trim()
				});

		return rowsAffected > 0;
	}

	public async Task<TicketDto?> GetTicketById(int id)
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
        FROM dbo.Tickets
        WHERE Id = @Id;
        """;

		var ticket =
			await connection.QuerySingleOrDefaultAsync<Ticket>(
				sql,
				new
				{
					Id = id
				});

		if (ticket is null)
		{
			return null;
		}

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