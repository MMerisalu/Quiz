using App.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Areas.Admin.ViewModels;

public class QuizVM
{
    public Quiz? Quiz { get; set; }
    
    public SelectList? Questions { get; set; }
    public ICollection<Guid>? SelectedQuestions { get; set; }
    public SelectList? Subjects { get; set; }
    public ICollection<Guid>? SelectedSubjects { get; set; }
}