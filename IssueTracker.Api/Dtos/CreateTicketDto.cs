namespace IssueTracker.Api.Dtos;

public class CreateTicketDto
{
	public string Title { get; set; } = "";

	public string Description { get; set; } = "";

	public string Priority { get; set; } = "";
}