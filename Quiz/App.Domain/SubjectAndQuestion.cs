using Base.Domain;

namespace App.Domain;

public class SubjectAndQuestion: DomainEntityId
{
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public Guid QuestionId { get; set; }
    public Question? Question { get; set; }
    
    
}