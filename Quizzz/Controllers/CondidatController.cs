using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizzz.Models;
using Quizzz.Repository;

/*[Route("api/[controller]")]
[ApiController]
public class CandidatController : ControllerBase
{
    private readonly IRepository<Candidat> _repository;
    private readonly IMapper _mapper;
    private readonly QuizzContext _context;
    public CandidatController(IRepository<Candidat> repository, IMapper mapper, QuizzContext context)
    {
        _repository = repository;
        _mapper = mapper;
        _context = context;
    }

    [HttpGet("{id}", Name = "GetCandidatById")]
    public async Task<ActionResult<CondidatDTO>> GetCandidatByIdAsync(int id)
    {
        var candidat = await _repository.GetByIdAsync(u => u.Id == id);
        if (candidat == null)
            return NotFound($"Candidat with Id {id} not found");

        var candidatDTO = _mapper.Map<CondidatDTO>(candidat);

        return Ok(candidatDTO);
    }

    [HttpGet("ByUser/{utilisateurId}")]
    public async Task<ActionResult<CondidatDTO>> GetByUserId(int utilisateurId)
    {
        var candidat = await _context.Candidats
    .Include(c => c.Tests)
        .ThenInclude(t => t.QuestionTests)
            .ThenInclude(qt => qt.Question)
                .ThenInclude(q => q.Reponses)
    .Include(c => c.Tests)
        .ThenInclude(t => t.QuestionTests)
            .ThenInclude(qt => qt.ReponseChoisie)
    .Include(c => c.Utilisateur)
    .FirstOrDefaultAsync(c => c.UtilisateurId == utilisateurId);


        if (candidat == null)
            return NotFound();

        var tests = candidat.Tests.Select(t => new TestDTO
        {
            Id = t.ID,
            DateDePassage = DateOnly.FromDateTime(t.Date_Passage),
            SectionId = t.SectionID,
            CandidatId = t.Candidat_ID,
            NoteObtenu = t.NoteObtenu,
            Est_reussi = ConvertStringToBoolNullable(t.Est_reussi),

            Questions = t.QuestionTests.Select(qt => new QuestionDTO
            {
                Id = qt.Question.ID,
                Enonce = qt.Question.Enonce,
                Section_Id = qt.Question.Section_ID,
                SectionNom = qt.Question.Section.Nom,
                BonneReponse = qt.Question.Reponses.FirstOrDefault(r => r.Est_correcte)?.Test_reponse,
                ReponseUtilisateur = qt.ReponseChoisie?.Test_reponse,
                Reponses = qt.Question.Reponses.Select(r => new ReponseDTO
                {
                    Id = r.ID,
                    Texte = r.Test_reponse,
                    EstCorrecte = r.Est_correcte
                }).ToList()
            }).ToList()
        }).ToList(); 

        var candidatDTO = _mapper.Map<CondidatDTO>(candidat);
        candidatDTO.Tests = tests;

        return Ok(candidatDTO);
    }

    private bool? ConvertStringToBoolNullable(string? str)
    {
        if (string.IsNullOrEmpty(str)) return null;
        if (bool.TryParse(str, out bool parsed)) return parsed;
        if (str == "1") return true;
        if (str == "0") return false;
        return null;
    }

    [HttpPost]
    public async Task<ActionResult<CondidatDTO>> CreateCandidat([FromBody] CondidatCreateDTO candidatCreateDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!Request.Headers.TryGetValue("UtilisateurId", out var utilisateurIdHeader) ||
            !int.TryParse(utilisateurIdHeader, out int utilisateurId))
        {
            return Unauthorized(new { Message = "Utilisateur non identifié" });
        }

        var utilisateur = await _context.Utilisateurs.FindAsync(utilisateurId);
        if (utilisateur == null)
            return NotFound(new { Message = "Utilisateur introuvable" });

        var candidat = new Candidat
        {
            Nom = candidatCreateDTO.Nom,
            Utilisateur = utilisateur
        };

        await _repository.CreateAsync(candidat);

        var createdDTO = _mapper.Map<CondidatDTO>(candidat);

        return CreatedAtRoute("GetCandidatById", new { id = candidat.Id }, createdDTO);
    }


    [HttpPut]
    public async Task<ActionResult> UpdateCandidat([FromBody] CondidatDTO candidatDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var candidat = _mapper.Map<Candidat>(candidatDTO);

        try
        {
            await _repository.UpdateAsync(candidat);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CondidatDTO>>> GetCandidats()
    {
        var candidats = await _repository.GetAllAsync();
        var candidatsDTO = _mapper.Map<List<CondidatDTO>>(candidats);

        return Ok(candidatsDTO);
    }

    [HttpDelete("delete/{id}", Name = "DeleteCandidatById")]
    public async Task<ActionResult<bool>> DeleteCandidatById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var deleted = await _repository.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok(true);
    }
}
*/