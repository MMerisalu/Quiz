using Base.Domain;

namespace App.Domain;

public class Answer: DomainEntityId
{
    public Guid QuestionId { get; set; }
    public Question? Question { get; set; }

    public string AnswerText { get; set; } = default!;
    public bool IsCorrect { get; set; } 


}