using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using WebApp.Areas.Admin.ViewModels;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuestionsController : Controller
    {
        private readonly AppDbContext _context;

        public QuestionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Questions
        public async Task<IActionResult> Index()
        {
            var questionsData = await GetQuery()
                .ToListAsync();
                
                var questions = new List<QuestionVM>( );
                foreach (var q in questionsData)
                {
                    questions.Add(MapViewFromData(q));
                }

                return
                View(questions);
        }

        private IQueryable<Question> GetQuery()
        {
            return _context.Questions
                .Include(s => s.Subjects)
                .ThenInclude(s => s.Subject);
        }
        private static QuestionVM MapViewFromData(Question question)
        {
            return new QuestionVM
            {
                Question = question,
                SubjectName = String.Join(", ", question.Subjects.Select(s => s.Subject!.Title))
            };
        }

        // GET: Admin/Questions/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null )
            {
                return NotFound();
            }

            var question = await GetQuery()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            var vm = MapViewFromData(question);
            
            return View(vm);
        }

        // GET: Admin/Questions/Create
        public async Task<IActionResult> Create()
        {
            var vm = new QuestionVM();
            vm.Question = new Question();
            vm.Subjects = new SelectList(await _context.Subjects.ToListAsync(), nameof(Subject.Id),
                nameof(Subject.Title));
            return View(vm);
        }

        // POST: Admin/Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionVM questionVm)
        {
            if (ModelState.IsValid)
            {
                // prepare the data
                questionVm.Question!.Id = Guid.NewGuid();
                foreach (var id in questionVm.SelectedSubjects)
                {
                    questionVm.Question!.Subjects.Add(new SubjectAndQuestion { Id = Guid.NewGuid(), SubjectId = id });
                }

                // save the data
                _context.Add(questionVm.Question);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(questionVm);
        }

        // GET: Admin/Questions/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null )
            {
                return NotFound();
            }

            var question = await GetQuery().FirstOrDefaultAsync(x => x.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            var vm = MapViewFromData(question);
            vm.Subjects = new SelectList(await _context.Subjects.ToListAsync(), nameof(Subject.Id),
                nameof(Subject.Title));
            // Now build the pre-selected Ids!
            vm.SelectedSubjects = question.Subjects.Select(s => s.SubjectId).ToList();
            return View(vm);
        }

        // POST: Admin/Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,  QuestionVM questionVm)
        {
            if (id != questionVm.Question!.Id)
            {
                return NotFound();
            }

            
            if (ModelState.IsValid)
            {
                if (questionVm.SelectedSubjects == null) return BadRequest();
                
                var db = await GetQuery().FirstOrDefaultAsync(x => x.Id == id);
                if (db == null) return NotFound();
                
                // Update the properties
                db.Title = questionVm.Question.Title;
                db.Text = questionVm.Question.Text;
                
                // Check that the selections are correct
                foreach (var subjectId in questionVm.SelectedSubjects)
                {
                    if (!db.Subjects.Any(x => x.SubjectId == subjectId))
                        _context.SubjectsAndQuestions.Add(new SubjectAndQuestion { Id = Guid.NewGuid(), SubjectId = subjectId, QuestionId = id });
                }
                // Check for de-selections as well, use .ToList() so I can remove the items from the collection
                foreach (var subject in db.Subjects)
                {
                    if (!questionVm.SelectedSubjects.Contains(subject.SubjectId))
                    {
                        db.Subjects.Remove(subject);
                    }
                }

                // save the data
                try
                {
                    //_context.Update(db);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(questionVm.Question.Id))
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
            return View(questionVm);
        }

        // GET: Admin/Questions/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            var vm = new QuestionVM();
            vm.Question = question;
            return View(vm);
        }

        // POST: Admin/Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(Guid id)
        {
          return (_context.Questions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
