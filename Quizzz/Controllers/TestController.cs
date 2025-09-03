using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizzz.Models;
using Quizzz.Repository;

namespace Quizzz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IRepository<Test> _repository;
        private readonly IMapper _mapper;
        private readonly QuizzContext _context;
        private readonly IConfiguration _config;

        public TestController(IRepository<Test> repository, IMapper mapper, QuizzContext context, IConfiguration config)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTest([FromBody] TestCreateDTO dto)
        {
            var test = new Test
            {
                Date_Passage = dto.Date_Passage,
                NoteObtenu = dto.NoteObtenu,
                Est_reussi = dto.Est_reussi,
                Candidat_ID = dto.Candidat_ID,
                SectionID = dto.SectionID
            };

            _context.Tests.Add(test);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Test enregistré", testId = test.ID });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestDTO>>> GetTests()
        {
            var questions = await _context.Questions
                .Include(q => q.Section)
                .ToListAsync();

            var tests = await _context.Tests
                .Include(t => t.Section)
                .ToListAsync();

            var result = tests.Select(t => new TestDTO
            {
                Id = t.ID,
                DateDePassage = DateOnly.FromDateTime(t.Date_Passage),
                SectionId = t.SectionID,
                CandidatId = t.Candidat_ID,
                NoteObtenu = t.NoteObtenu,
                Est_reussi = ConvertStringToBoolNullable(t.Est_reussi),

                Questions = questions
                    .Where(q => q.Section_ID == t.SectionID)
                    .Select(q => new QuestionDTO
                    {
                        Id = q.ID,
                        Enonce = q.Enonce,
                        Section_Id = q.Section_ID,
                        SectionNom = q.Section.Nom
                    })
                    .ToList()

            }).ToList();

            return Ok(result);
        }
        private bool? ConvertStringToBoolNullable(string? str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            if (bool.TryParse(str, out bool parsed)) return parsed;
            if (str == "1") return true;
            if (str == "0") return false;
            return null;
        }
    }
}
