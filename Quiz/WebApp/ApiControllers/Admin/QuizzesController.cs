using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using WebApp.ApiControllers.RegularUsers.DTO;

namespace WebApp.ApiControllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizzesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuizzesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Quizzes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzes()
        {
          return await _context.Quizzes.ToListAsync();
        }
        
        // GET: api/Quizzes/Statistics
        [HttpGet]
        [Route("Statistics")]
        public async Task<ActionResult<IEnumerable<QuizInfoDTO>>> GetStatistics()
        {
            // what is the current user id?
            Guid userId = Guid.NewGuid();

            var rawData = _context.Quizzes
                .Include(q => q.Subjects)
                .ThenInclude(q => q.Subject)
                .Include(q => q.Responses)
                .ThenInclude(r => r.Answer)
                .ToList(); 
                // deliberate pull into memory
var query = rawData.Select(quiz => new QuizInfoDTO
            {
                Id = quiz.Id,
                UserId = userId,
                Title = quiz.Title,
                Subjects = String.Join(", ", quiz.Subjects.Select(s => s.Subject!.Title).OrderBy(x => x)),
                IsPublic = quiz.IsPublic,
                QuizType = quiz.QuizType,
                NumberOfQuestions = quiz.NumberOfQuestions,
                NumberOfUsers = quiz.Responses.DistinctBy(r => r.UserId).Count(),
                /*AverageScore = quiz.Responses.GroupBy(r => r.UserId)
                 .Average(g => g.Count(x => x.Answer != null && x.Answer.IsCorrect))*/
                HasBeenCompleted = quiz.Responses.Any(r => r.UserId == userId)
            });

            var data = query.ToList();
            foreach (var quiz in data)
            {
                if (quiz.NumberOfUsers > 0)
                {
                    var q = rawData.First(r => r.Id == quiz.Id);
                    quiz.AverageScore = (double)q.Responses.Count(x => x.Answer!.IsCorrect) /
                                        (double)(quiz.NumberOfUsers * quiz.NumberOfQuestions);
                }
            }

            return data;
        }

        // GET: api/Quizzes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QuizDTO?>> GetQuiz(Guid id)
        {

            var quizzes = await _context.Quizzes.Where(x => x.Id == id)
                .Select(q => new
                {
                    q.Id,
                    q.Title,
                    q.QuizType,
                    Subjects = q.Subjects.Select(x => x.Subject!.Title).ToArray(),
                    Questions = q.Questions.Select(qs => new
                    {
                        qs.Question!.Id,
                        qs.Question!.Title,
                        qs.Question.Text,
                        Subjects = qs.Question.Subjects.Select(x => x.Subject!.Title).ToArray(),
                        Answers = qs.Question!.Answers!.Select(a => new AnswerDTO
                        {
                            Id = a.Id,
                            Text = a.AnswerText
                        })
                    })
                })
                .ToListAsync();

            if (quizzes?.Any() == false)
            {
                return NotFound();
            }
            // project into DTO
            return quizzes.Select(q => new QuizDTO
            {
                Id = q.Id,
                Subjects = String.Join(", ", q.Subjects.OrderBy(x => x)),
                Title = q.Title,
                QuizType = q.QuizType,
                Questions = q.Questions.Select(qs => new QuizQuestionDTO
                {
                    Id = qs.Id,
                    Title = qs.Title,
                    Text = qs.Text,
                    Subjects = String.Join(", ", q.Subjects.OrderBy(x => x)),
                    Answers = qs.Answers.ToList()
                })
                
            }).FirstOrDefault();
        }

        // PUT: api/Quizzes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuiz(Guid id, Quiz quiz)
        {
            if (id != quiz.Id)
            {
                return BadRequest();
            }

            _context.Entry(quiz).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuizExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Quizzes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Quiz>> PostQuiz(Quiz quiz)
        {
         
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuiz", new { id = quiz.Id }, quiz);
        }
        
        // POST: api/Quizzes/Response/{id}
        [HttpPost("Response/{id}")]
        public async Task<ActionResult<Guid>> PostResponse(Guid id, QuizResponse quizResponse)
        {
            Guid userId = Guid.TryParse(quizResponse.UserId, out Guid u) ? u : Guid.NewGuid();
            if(userId == Guid.Empty)
                userId = Guid.NewGuid();
            var quiz = _context.Quizzes.Find(id);
            _context.UserResponses.AddRange(quizResponse.Responses.Select(r => new UserResponse
            {
                QuizId = quiz.Id,
                QuestionId = Guid.Parse(r.Key),
                AnswerId = Guid.Parse(r.Value),
                UserId = userId
            }));
            
            await _context.SaveChangesAsync();

            return Ok(new { QuizId = id, UserId = userId});
        }
        // DELETE: api/Quizzes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuiz(Guid id)
        {
           
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool QuizExists(Guid id)
        {
            return (_context.Quizzes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
