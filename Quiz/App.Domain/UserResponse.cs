using Base.Domain;

namespace App.Domain;

public class UserResponse: DomainEntityId
{
    public Guid QuizId { get; set; }
    public Quiz? Quiz { get; set; }

    public Guid QuestionId { get; set; }
    public Question? Question { get; set; }

    public Guid AnswerId { get; set; }
    public Answer? Answer { get; set; }
    
    public Guid UserId { get; set; }
}