using App.Domain.Enum;

namespace WebApp.ApiControllers.RegularUsers.DTO;

public class QuizDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public QuizType? QuizType { get; set; }
    public string Subjects { get; set; } = default!;
    public string Title { get; set; } = default!;
    
    public IEnumerable<QuizQuestionDTO> Questions { get; set; }
}

public class QuizQuestionDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Text { get; set; } = default!;
    public string Subjects { get; set; } = default!;
    public IEnumerable<AnswerDTO> Answers { get; set; } = default!;
}

public class AnswerDTO
{
    public Guid Id { get; set; }
    public string Text { get; set; } = default!;
}