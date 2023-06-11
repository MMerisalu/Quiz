using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using WebApp.Areas.Admin.ViewModels;
#nullable enable
namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuizzesController : Controller
    {
        private readonly AppDbContext _context;

        public QuizzesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Quizzes
        public async Task<IActionResult> Index()
        {
              return View(await _context.Quizzes.ToListAsync());
        }

        // GET: Admin/Quizzes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null )
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        // GET: Admin/Quizzes/Create
        public async Task<IActionResult> Create()
        {
            var vm = new QuizVM();
            vm.Quiz = new Quiz();
            vm.Questions =
                new SelectList(_context.Questions.Select(x => new { x.Id, Title = x.Title + " - " + x.Text }),
                    nameof(Question.Id), nameof(Question.Title));
            vm.Subjects = new SelectList(await _context.Subjects.ToListAsync(), nameof(Subject.Id),
                nameof(Subject.Title));
            return View(vm);
        }

        // POST: Admin/Quizzes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuizVM quizVm)
        {
            if (ModelState.IsValid)
            {
                if (quizVm.Quiz == null) return BadRequest();
                if (quizVm.SelectedQuestions == null) return BadRequest();
                if (quizVm.SelectedSubjects == null) return BadRequest();
                
                // prepare the data
                quizVm.Quiz.Id = Guid.NewGuid();
                quizVm.Quiz.NumberOfQuestions = quizVm.SelectedQuestions.Count();
                foreach (var id in quizVm.SelectedQuestions)
                {
                    quizVm.Quiz.Questions.Add(new QuizAndQuestion() { Id = Guid.NewGuid(), QuestionId = id });
                }
                foreach (var id in quizVm.SelectedSubjects)
                {
                    quizVm.Quiz.Subjects.Add(new QuizAndSubject() { Id = Guid.NewGuid(), SubjectId = id });
                }

                // save the data
                _context.Add(quizVm.Quiz);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(quizVm);
        }

        // GET: Admin/Quizzes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await GetQuery().FirstOrDefaultAsync(x => x.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

            var vm = new QuizVM();
            vm.Quiz = quiz;
            vm.Questions =
                new SelectList(_context.Questions.Select(x => new { x.Id, Title = x.Title + " - " + x.Text }),
                    nameof(Question.Id), nameof(Question.Title));
            vm.SelectedQuestions = quiz.Questions!.Select(q => q.QuestionId).ToList();
            vm.Subjects = new SelectList(await _context.Subjects.ToListAsync(), nameof(Subject.Id),
                nameof(Subject.Title));
            vm.SelectedSubjects = quiz.Subjects!.Select(s => s.SubjectId).ToList();
            return View(vm);
        }

        private IQueryable<Quiz> GetQuery()
        {
            return _context.Quizzes.Include(x => x.Questions)
                .Include(x => x.Subjects)
                .OrderBy(q => q.Title)
                ;
        }
        
        // POST: Admin/Quizzes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, QuizVM vm)
        {
            if (id != vm.Quiz?.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                if (vm.SelectedSubjects == null || vm.SelectedQuestions == null) return BadRequest();
                
                var db = await GetQuery().FirstOrDefaultAsync(x => x.Id == id);
                if (db == null) return NotFound();
                
                // Update the properties
                db.Title = vm.Quiz.Title;
                db.IsPublic = vm.Quiz.IsPublic;
                db.QuizType = vm.Quiz.QuizType;
                db.NumberOfQuestions = vm.SelectedQuestions.Count();
                
                // Check that the selections are correct
                foreach (var questionId in vm.SelectedQuestions)
                {
                    if (!db.Questions!.Any(x => x.QuestionId == questionId))
                        _context.QuizzesAndQuestions.Add(new QuizAndQuestion() { Id = Guid.NewGuid(), QuestionId = questionId, QuizId = id });
                }
                foreach (var subjectId in vm.SelectedSubjects)
                {
                    if (!db.Subjects!.Any(x => x.SubjectId == subjectId))
                        _context.QuizzesAndSubjects.Add(new QuizAndSubject { Id = Guid.NewGuid(), SubjectId = subjectId, QuizId = id });
                }
                // Check for de-selections as well, use .ToList() so I can remove the items from the collection
                foreach (var question in db.Questions!)
                {
                    if (!vm.SelectedQuestions.Contains(question.QuestionId))
                    {
                        db.Questions.Remove(question);
                    }
                }
                foreach (var subject in db.Subjects!)
                {
                    if (!vm.SelectedSubjects.Contains(subject.SubjectId))
                    {
                        db.Subjects.Remove(subject);
                    }
                }
                
                try
                {
                    //_context.Update(vm.Quiz);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizExists(vm.Quiz.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        // GET: Admin/Quizzes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null )
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

            var vm = new QuizVM();

            vm.Quiz = quiz;
            return View(vm);
        }

        // POST: Admin/Quizzes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz != null)
            {
                _context.Quizzes.Remove(quiz);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuizExists(Guid id)
        {
          return (_context.Quizzes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
