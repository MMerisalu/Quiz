using App.Domain;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<UserResponse>()
            .HasOne(r => r.Question)
            .WithMany(q => q.Responses)
            .OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Question>()
            .HasMany(q => q.Responses)
            .WithOne(r => r.Question)
            .OnDelete(DeleteBehavior.NoAction);
    }

    public DbSet<Subject> Subjects { get; set; } = default!;
    public DbSet<Answer> Answers { get; set; } = default!;
    public DbSet<Question> Questions { get; set; } = default!;
    public DbSet<Admin> Admins { get; set; } = default!;
    public DbSet<QuizAndSubject> QuizzesAndSubjects { get; set; } = default!;
    public DbSet<Quiz> Quizzes { get; set; } = default!;
    public DbSet<QuizAndQuestion>? QuizzesAndQuestions { get; set; } = default!;
    public DbSet<SubjectAndQuestion>? SubjectsAndQuestions { get; set; }
    public DbSet<RegularUser> RegularUsers { get; set; } = default!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;

    public DbSet<UserResponse> UserResponses { get; set; } = default!;
}
