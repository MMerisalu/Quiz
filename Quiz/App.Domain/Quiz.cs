using System.ComponentModel.DataAnnotations;
using App.Domain.Enum;
using Base.Domain;

namespace App.Domain;

public class Quiz: DomainEntityId
{
    [Required]
    public QuizType? QuizType { get; set; }
    
    public ICollection<QuizAndSubject> Subjects { get; set; } = new HashSet<QuizAndSubject>();
    [Required]
    [MaxLength(50)]
    [StringLength(50)]
    public string Title { get; set; } = default!;

    public int NumberOfQuestions { get; set; }

    public ICollection<QuizAndQuestion> Questions { get; set; } = new HashSet<QuizAndQuestion>();
    public bool IsPublic { get; set; }
    
    public ICollection<UserResponse> Responses { get; set; } = new HashSet<UserResponse>();
}