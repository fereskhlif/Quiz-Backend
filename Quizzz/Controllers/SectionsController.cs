using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizzz.Models;

[ApiController]
[Route("api/[controller]")]
public class SectionsController : ControllerBase
{
    private readonly QuizzContext _context;

    public SectionsController(QuizzContext context)
    {
        _context = context;
    }
    [HttpGet("sections-questions")]
    public async Task<ActionResult<IEnumerable<object>>> GetSectionsWithQuestionsAndReponses()
    {
        var data = await _context.Sections
            .Include(s => s.Questions)
                .ThenInclude(q => q.Reponses)
            .Select(s => new {
                id = s.Id,
                name = s.Nom,
                questions = s.Questions.Select(q => new {
                    id = q.ID,
                    enonce = q.Enonce,
                    section_Id = q.Section_ID,
                    sectionNom = s.Nom,
                    reponses = q.Reponses.Select(r => new {
                        texte = r.Test_reponse,
                        estCorrecte = r.Est_correcte,
                        question_ID = r.Question_ID
                    })
                })
            })
            .ToListAsync();

        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Section section)
    {
        try
        {
            _context.Sections.Add(section);
            await _context.SaveChangesAsync();
            return Ok(section);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur serveur : {ex.Message}");
        }
    }
    [HttpPost("DeleteMultiple")]
    public async Task<IActionResult> DeleteMultiple([FromBody] List<int> sectionIds)
    {
        try
        {
            var sectionsToDelete = await _context.Sections.Where(s => sectionIds.Contains(s.Id)).ToListAsync();
            if (!sectionsToDelete.Any())
                return NotFound("Aucune Section trouvée");

            _context.Sections.RemoveRange(sectionsToDelete);
            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur pendant la suppression : " + ex.Message);
            return StatusCode(500, $"Erreur serveur : {ex.Message}");
        }
    }


}



