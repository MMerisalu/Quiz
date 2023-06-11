using App.Domain;
using App.Domain.Enum;

namespace WebApp.ApiControllers.RegularUsers.DTO;

public class QuizInfoDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public QuizType? QuizType { get; set; }
    public string Subjects { get; set; } = default!;
    public string Title { get; set; } = default!;
    public int NumberOfQuestions { get; set; }
    public bool IsPublic { get; set; }

    /// <summary>
    /// Total number of users who have submitted a response for this quiz 
    /// </summary>
    public int NumberOfUsers { get; set; }
    /// <summary>
    /// Average score as a percentage out of 100
    /// </summary>
    public double AverageScore { get; set; }
    /// <summary>
    /// Flag to indicate if the current user has compelted this quiz yet or not
    /// </summary>
    public bool HasBeenCompleted { get; set; }
}