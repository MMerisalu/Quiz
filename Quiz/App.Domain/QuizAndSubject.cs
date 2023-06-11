using Base.Domain;

namespace App.Domain;

public class QuizAndSubject: DomainEntityId
{
    public Guid QuizId { get; set; }
    public Quiz? Quiz { get; set; }
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }
}