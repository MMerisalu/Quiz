using App.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Areas.Admin.ViewModels;

public class QuestionVM
{
    public Question? Question { get; set; }
    public SelectList? Subjects { get; set; }
    public ICollection<Guid>? SelectedSubjects { get; set; }

    public string? SubjectName { get; set; }
}