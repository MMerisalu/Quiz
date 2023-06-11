using App.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Areas.Admin.ViewModels;

public class AnswerVM
{
    public Answer? Answer { get; set; }
    public SelectList? Questions { get; set; }
}