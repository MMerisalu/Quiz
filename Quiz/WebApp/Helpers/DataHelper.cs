using App.DAL.EF;
using App.Domain;
using App.Domain.Enum;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Helpers;

/// <summary>
/// Data helper 
/// </summary>
public static class DataHelper
{
    /// <summary>
    /// Set up App data
    /// </summary>
    /// <param name="app">App</param>
    /// <param name="env">Environment variable</param>
    /// <param name="configuration">Configuration variable</param>
    /// <exception cref="ApplicationException">Application exception</exception>
    public static async Task SetupAppData(IApplicationBuilder app, IWebHostEnvironment env,
        IConfiguration configuration)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();

        await using var context = serviceScope
            .ServiceProvider.GetService<AppDbContext>();

        using var userManager = serviceScope.ServiceProvider.GetService<UserManager<AppUser>>();
        using var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<AppRole>>();

        if (context == null) throw new ApplicationException("Problem in services. No db context.");

        if (userManager == null || roleManager == null) Console.Write("Cannot instantiate userManager or rolemanager!");

        if (configuration.GetValue<bool>("DataInitialization:DropDatabase"))
            await context.Database.EnsureDeletedAsync();

        if (configuration.GetValue<bool>("DataInitialization:MigrateDatabase")) await context.Database.MigrateAsync();

        await SeedDatabase(context, userManager!, roleManager!,
            configuration.GetValue<bool>("DataInitialization:SeedIdentity"),
            configuration.GetValue<bool>("DataInitialization:SeedData"));
    }

    /// <summary>
    /// Seeding data
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="userManager">Manager for the system users</param>
    /// <param name="roleManager">Manager for the system roles</param>
    /// <param name="seedIdentity">Identity seeding</param>
    /// <param name="seedData">Data seeding</param>
    public static async Task SeedDatabase(AppDbContext context, UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager, bool seedIdentity, bool seedData)
    {
        if (seedIdentity)
        {
            var roles = new (string name, string displayName)[]
            {
                ("Admin", "Administrator"),
                ("RegularUser", "User"),

            };

            foreach (var roleInfo in roles)
            {
                var role = roleManager!.FindByNameAsync(roleInfo.name).Result;
                if (role == null)
                {
                    var identityResult = roleManager.CreateAsync(new AppRole
                    {
                        Name = roleInfo.name,
                        DisplayName = roleInfo.displayName
                    });
                    if (!identityResult.Result.Succeeded)
                        foreach (var identityError in identityResult.Result.Errors)
                            Console.WriteLine("Cant create role! Error: " + identityError.Description);
                }
            }

            if (seedData)
            {

                var appUser = new AppUser
                {
                    Id = new Guid(),
                    FirstName = "Katrin",
                    LastName = "Salu",

                    Email = "kati@gmail.com",


                };

                appUser.UserName = appUser.Email;

                var result = userManager!.CreateAsync(appUser, "Katrinkass123$").Result;


                if (!result.Succeeded)
                    foreach (var identityError in result.Errors)
                        Console.WriteLine("Cant create user! Error: " + identityError.Description);
                result = userManager.AddToRoleAsync(appUser, "Admin").Result;
                if (!result.Succeeded)
                    foreach (var identityError in result.Errors)
                        Console.WriteLine("Cant add user to role! Error: " + identityError.Description);



                var admin = new Admin
                {
                    Id = new Guid(),
                    AppUserId = context.Users.OrderBy(u => u.LastName).First(a =>
                        a.FirstName.Equals("Katrin") && a.LastName.Equals("Salu")).Id,
                };

                await context.Admins.AddAsync(admin);
                await context.SaveChangesAsync();



                appUser = new AppUser
                {
                    Id = new Guid(),
                    FirstName = "Toomas",
                    LastName = "Paju",
                    Email = "toomas.paju@gmail.com"

                };

                appUser.UserName = appUser.Email;

                result = userManager!.CreateAsync(appUser, "Toomaskoer123$").Result;

                if (!result.Succeeded)
                    foreach (var identityError in result.Errors)
                        Console.WriteLine("Cant create user! Error: " + identityError.Description);
                result = userManager.AddToRoleAsync(appUser, "RegularUser").Result;

                var regularUser = new RegularUser()
                {
                    Id = new Guid(),
                    AppUserId = context.Users.OrderBy(u => u.LastName).First(a =>
                        a.FirstName.Equals("Toomas") && a.LastName.Equals("Paju")).Id,

                };

                await context.RegularUsers.AddAsync(regularUser);
                await context.SaveChangesAsync();




                var history = new Subject { Id = Guid.NewGuid(), Title = "History" };
                var economics = new Subject { Id = Guid.NewGuid(), Title = "Economics" };
                var technology = new Subject { Id = Guid.NewGuid(), Title = "Technology" };
                var popCulture = new Subject { Id = Guid.NewGuid(), Title = "Popular Culture" };
                var science = new Subject { Id = Guid.NewGuid(), Title = "Science" };
                var maths = new Subject { Id = Guid.NewGuid(), Title = "Mathematics" };
                var geography = new Subject { Id = Guid.NewGuid(), Title = "Geography" };
                await context.Subjects.AddRangeAsync(new[]
                    { history, economics, technology, popCulture, science, maths, geography });
                await context.SaveChangesAsync();
                var questions = new List<Question>();

                var q1 = new Question
                {
                    Title = "Business", Text = "What is the difference between a recession and a depression ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = economics.Id } }),
                    Answers = new HashSet<Answer>
                    {
                        new Answer { IsCorrect = true, AnswerText = "Duration" },
                        new Answer { AnswerText = "Severity" },
                        new Answer { AnswerText = "Cause" },
                        new Answer { AnswerText = "Location" },
                        new Answer { AnswerText = "Frequency" },
                        new Answer { AnswerText = "Magnitude" },
                        new Answer { AnswerText = "Type" },
                        new Answer { AnswerText = "Outcome" },
                        new Answer { AnswerText = "Policy" }
                    }
                };
                questions.Add(q1);
                questions.Add(new Question
                {
                    Title = "Business",
                    Text = "What is the most common form of business organization in the United States ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = economics.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Investments", Text = "What is the difference between a stock and a bond ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = economics.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Investments", Text = "What is the difference between a debit card and a credit card ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = economics.Id } }),
                });

                var q2 = new Question
                {
                    Title = "First People", Text = "Who was the first president of the United States ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = history.Id } }),
                    Answers = new HashSet<Answer>
                    {
                        new Answer { IsCorrect = true, AnswerText = "George Washington" },
                        new Answer { AnswerText = "John Adams" },
                        new Answer { AnswerText = "Thomas Jefferson" },
                        new Answer { AnswerText = "James Madison" },
                        new Answer { AnswerText = "James Monroe" },
                        new Answer { AnswerText = "John Quincy Adams" },
                        new Answer { AnswerText = "Andrew Jackson" },
                        new Answer { AnswerText = "Martin Van Buren" },
                        new Answer { AnswerText = "William Henry Harrison" }
                    }
                };
                questions.Add(q2);
                questions.Add(new Question
                {
                    Title = "First People", Text = "Who was the first person to circumnavigate the globe ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = history.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "First People", Text = "Who was the first person to reach the South Pole ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = history.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "First People", Text = "Who was the first person to reach the North Pole ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = history.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "First People", Text = "Who was the first person to sail around the world ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = history.Id } }),
                });

                var q3 = new Question
                {
                    Title = "Information Technology", Text = "What is an IP address ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = technology.Id } }),
                    Answers = new HashSet<Answer>
                    {
                        new Answer { IsCorrect = true, AnswerText = "Internet Protocol Address" },
                        new Answer { AnswerText = "Internet Provider Address" },
                        new Answer { AnswerText = "Internal Protocol Address" },
                        new Answer { AnswerText = "Internal Provider Address" },
                        new Answer { AnswerText = "Internet Protocol Association" }
                    }
                };
                questions.Add(q3);
                questions.Add(new Question
                {
                    Title = "Information Technology", Text = "What is a URL ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = technology.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Information Technology", Text = "What is an operating system ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = technology.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Information Technology", Text = "What is a computer virus ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = technology.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Information Technology", Text = "What is cloud computing ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = technology.Id } }),
                });

                var q4 = new Question
                {
                    Title = "Lead Roles", Text = "Who played James Bond in “Dr.No”?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = popCulture.Id } }),
                    Answers = new HashSet<Answer>
                    {
                        new Answer { IsCorrect = true, AnswerText = "Sean Connery" },
                        new Answer { AnswerText = "Roger Moore" },
                        new Answer { AnswerText = "George Lazenby" },
                        new Answer { AnswerText = "Timothy Dalton" },
                        new Answer { AnswerText = "Pierce Brosnan" },
                        new Answer { AnswerText = "Daniel Craig" }
                    }
                };
                questions.Add(q4);
                questions.Add(new Question
                {
                    Title = "Lead Roles", Text = "Who played Batman in “Batman Begins”?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = popCulture.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Lead Roles",
                    Text = "Who played Harry Potter in “Harry Potter and the Philosopher’s Stone”?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = popCulture.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Lead Roles", Text = "Who played Katniss Everdeen in “The Hunger Games”?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = popCulture.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Lead Roles", Text = "Who played Iron Man in “Iron Man”?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = popCulture.Id } }),
                });

                var q5 = new Question
                {
                    Title = "Trigonometry", Text = "What is Pythagoras’ theorem ?",
                    Subjects = new HashSet<SubjectAndQuestion>(
                        new[] { new SubjectAndQuestion { SubjectId = maths.Id } }),
                    Answers = new HashSet<Answer>()
                    {
                        new Answer { IsCorrect = true, AnswerText = "a^2 + b^2 = c^2" },
                        new Answer { AnswerText = "a^2 - b^2 = c^2" },
                        new Answer { AnswerText = "(a + b)^2 = c^2" },
                        new Answer { AnswerText = "(a - b)^2 = c^2" }
                    }
                };
                questions.Add(q5);
                questions.Add(new Question
                {
                    Title = "Calculus", Text = "What is calculus used for?",
                    Subjects = new HashSet<SubjectAndQuestion>(
                        new[] { new SubjectAndQuestion { SubjectId = maths.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Calculus", Text = "What is the difference between an equation and an expression ?",
                    Subjects = new HashSet<SubjectAndQuestion>(
                        new[] { new SubjectAndQuestion { SubjectId = maths.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Calculus", Text = "What is a quadratic equation ?",
                    Subjects = new HashSet<SubjectAndQuestion>(
                        new[] { new SubjectAndQuestion { SubjectId = maths.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Calculus", Text = "What is a linear equation ?",
                    Subjects = new HashSet<SubjectAndQuestion>(
                        new[] { new SubjectAndQuestion { SubjectId = maths.Id } }),
                });

                var q6 = new Question
                {
                    Title = "Biology", Text = "What is photosynthesis ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = science.Id } }),
                    Answers = new HashSet<Answer>
                    {
                        new Answer
                        {
                            IsCorrect = true,
                            AnswerText =
                                "The process by which green plants and some other organisms use sunlight to synthesize foods with the help of chlorophyll."
                        },
                        new Answer
                        {
                            AnswerText =
                                "The process by which green plants and some other organisms use sunlight to synthesize oxygen with the help of chlorophyll."
                        },
                        new Answer
                        {
                            AnswerText =
                                "The process by which green plants and some other organisms use sunlight to synthesize water with the help of chlorophyll."
                        },
                        new Answer
                        {
                            AnswerText =
                                "The process by which green plants and some other organisms use sunlight to synthesize carbon dioxide with the help of chlorophyll."
                        }
                    }
                };
                questions.Add(q6);
                questions.Add(new Question
                {
                    Title = "Physics", Text = "What is Newton’s second law of motion ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = science.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Chemistry", Text = "What is Boyle’s law ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = science.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Physics", Text = "What is Archimedes’ principle ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = science.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Physics", Text = "What is Einstein’s theory of relativity ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = science.Id } }),
                });

                var q7 = new Question
                {
                    Title = "Population", Text = "Which country has the largest population in Africa ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = geography.Id } }),
                    Answers = new HashSet<Answer>
                    {
                        new Answer { Id = Guid.NewGuid(), IsCorrect = true, AnswerText = "Nigeria" },
                        new Answer { Id = Guid.NewGuid(), AnswerText = "Ethiopia" },
                        new Answer { Id = Guid.NewGuid(), AnswerText = "Egypt" },
                        new Answer { Id = Guid.NewGuid(), AnswerText = "Democratic Republic of the Congo" },
                        new Answer { Id = Guid.NewGuid(), AnswerText = "Cote d'Ivoire" },
                        new Answer { Id = Guid.NewGuid(), AnswerText = "South Africa" },
                        new Answer { Id = Guid.NewGuid(), AnswerText = "Tanzania" },
                        new Answer { Id = Guid.NewGuid(), AnswerText = "Kenya" },
                        new Answer { Id = Guid.NewGuid(), AnswerText = "Uganda" }
                    }
                };
                questions.Add(q7);
                questions.Add(new Question
                {
                    Title = "Population", Text = "Which country has the largest population in Asia ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = geography.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Population", Text = "Which country has the largest population in Europe ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = geography.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Population", Text = "Which country has the largest population in South America ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = geography.Id } }),
                });
                questions.Add(new Question
                {
                    Title = "Population", Text = "Which country has the largest population in North America ?",
                    Subjects = new HashSet<SubjectAndQuestion>(new[]
                        { new SubjectAndQuestion { SubjectId = geography.Id } }),
                });

                await context.Questions.AddRangeAsync(questions);
                await context.SaveChangesAsync();

                // Create a Quiz!
                var quiz1 = new Quiz
                {
                    Id = Guid.Parse("48a96a4e-ddb2-4e99-b1e9-7779e802e1d6"),
                    Title = "First Quiz",
                    IsPublic = false,
                    QuizType = QuizType.Quiz,
                    NumberOfQuestions = 5,
                    Questions = new List<QuizAndQuestion>
                    {
                        new QuizAndQuestion { QuestionId = q3.Id },
                        new QuizAndQuestion { QuestionId = q7.Id },
                        new QuizAndQuestion { QuestionId = q6.Id },
                        new QuizAndQuestion { QuestionId = q4.Id },
                        new QuizAndQuestion { QuestionId = q1.Id },
                    }
                };
                quiz1.Subjects = new List<QuizAndSubject>() { new QuizAndSubject { SubjectId = maths.Id }, new QuizAndSubject { SubjectId = geography.Id } };
                context.Quizzes.Add(quiz1);
                var quiz2 = new Quiz
                {
                    Id = Guid.NewGuid(),
                    Title = "First Poll",
                    IsPublic = true,
                    NumberOfQuestions = 3,
                    QuizType = QuizType.Poll,
                    Questions = new List<QuizAndQuestion>
                    {
                        new QuizAndQuestion { QuestionId = q1.Id },
                        new QuizAndQuestion { QuestionId = q2.Id },
                        new QuizAndQuestion { QuestionId = q3.Id },
                    }
                };
                quiz2.Subjects = new List<QuizAndSubject>() { new QuizAndSubject { SubjectId = history.Id } };
                context.Quizzes.Add(quiz2);
                await context.SaveChangesAsync();

                await context.SaveChangesAsync();
            }
        }
    }
} 
