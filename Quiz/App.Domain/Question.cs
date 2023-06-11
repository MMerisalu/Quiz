using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Question: DomainEntityId
{
    public ICollection<SubjectAndQuestion> Subjects { get; set; } = new HashSet<SubjectAndQuestion>();
    public ICollection<QuizAndQuestion> Quizzes { get; set; } = new HashSet<QuizAndQuestion>();

    [Required]
    [MaxLength(50)]
    [StringLength(50)]
    public string Title { get; set; } = default!;
    
    [Required]
    [MaxLength(200)]
    [StringLength(200)]
    public string Text { get; set; } = default!;

    public ICollection<Answer>? Answers { get; set; }

    public ICollection<UserResponse>? Responses { get; set; }


}