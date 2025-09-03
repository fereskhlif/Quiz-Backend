using Microsoft.AspNetCore.Mvc;
using Quizzz.Models;
using Quizzz.Repository;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quizzz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IRepository<Question> _repository;
        private readonly QuizzContext _context;
        private readonly IMapper _mapper;

        public QuestionsController(IRepository<Question> repository, QuizzContext context, IMapper mapper)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetAll()
        {
            var questions = await _context.Questions
                .Include(q => q.Reponses)
                .Include(q => q.Section)
                .ToListAsync();

            var result = questions.Select(q => new QuestionDTO
            {
                Id = q.ID,
                Enonce = q.Enonce,
                Section_Id = q.Section_ID,
                SectionNom = q.Section?.Nom,
                Reponses = q.Reponses.Select(r => new ReponseDTO
                {
                    Texte = r.Test_reponse,
                    EstCorrecte = r.Est_correcte,
                    Question_ID = r.Question_ID
                }).ToList()
            });

            return Ok(result);
        }




        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDTO>> GetById(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
                return NotFound();

            var dto = _mapper.Map<QuestionDTO>(question);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<QuestionDTO>> Create(QuestionDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newQuestion = new Question
            {
                Enonce = dto.Enonce,
                Section_ID = dto.Section_Id,
                Reponses = dto.Reponses?.Select(r => new Reponse
                {
                    Test_reponse = r.Texte,
                    Est_correcte = r.EstCorrecte
                }).ToList() ?? new List<Reponse>()
            };

            _context.Questions.Add(newQuestion);
            Console.WriteLine("Nombre de réponses reçues : " + newQuestion.Reponses.Count);
            foreach (var rep in newQuestion.Reponses)
            {
                Console.WriteLine($"→ {rep.Test_reponse}, correcte ? {rep.Est_correcte}");
            }

            await _context.SaveChangesAsync();
            if (dto.Reponses != null && dto.Reponses.Any())
            {
                foreach (var rep in dto.Reponses)
                {
                    var reponse = new Reponse
                    {
                        Test_reponse = rep.Texte,
                        Est_correcte = rep.EstCorrecte,
                        Question_ID = newQuestion.ID
                    };
                    _context.Reponses.Add(reponse);
                }

                await _context.SaveChangesAsync(); 
            }

            dto.Id = newQuestion.ID;
            return CreatedAtAction(nameof(GetById), new { id = newQuestion.ID }, dto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, QuestionDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var question = await _context.Questions.FindAsync(id);
            if (question == null)
                return NotFound();

            _mapper.Map(dto, question);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Reponses)
                .FirstOrDefaultAsync(q => q.ID == id);

            if (question == null)
                return NotFound();

            _context.Reponses.RemoveRange(question.Reponses);

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("DeleteMultiple")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            try
            {
                var questionsToDelete = await _context.Questions
                    .Include(q => q.Reponses)
                    .Where(q => ids.Contains(q.ID))
                    .ToListAsync();

                if (!questionsToDelete.Any())
                    return NotFound("Aucune question trouvée");

                var reponsesToDelete = questionsToDelete.SelectMany(q => q.Reponses).ToList();
                _context.Reponses.RemoveRange(reponsesToDelete);

                _context.Questions.RemoveRange(questionsToDelete);

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur pendant la suppression : " + ex.Message);
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }


        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(q => q.ID == id);
        }
    }
}
