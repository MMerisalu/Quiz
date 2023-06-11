using Base.Domain;

namespace App.Domain;

public class QuizAndQuestion: DomainEntityId
{
    public Guid QuizId { get; set; }
    public Quiz? Quiz { get; set; }

    public Guid QuestionId { get; set; }
    public Question? Question { get; set; }
    
    
}