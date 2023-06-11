namespace WebApp.ApiControllers.RegularUsers.DTO;

public class QuizResponse
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public Dictionary<string, string> Responses { get; set; }
}