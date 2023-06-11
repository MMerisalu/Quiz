using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Subject: DomainEntityId
{
    [Required]
    [MaxLength(50)]
    [StringLength(50)]
    public string Title { get; set; } = default!;

    public List<QuizAndSubject>? Quizzes { get; set; }
    
    
    

}