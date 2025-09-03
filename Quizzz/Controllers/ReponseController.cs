using Microsoft.AspNetCore.Mvc;
using Quizzz.Models;
using Quizzz.Repository;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

[Route("api/[controller]")]
[ApiController]
public class ReponseController : ControllerBase
{
    private readonly IRepository<Reponse> _repository;
    private readonly QuizzContext _context;
    private readonly IMapper _mapper;

    public ReponseController(IRepository<Reponse> repository, QuizzContext context, IMapper mapper)
    {
        _repository = repository;
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<ReponseDTO>> Create(ReponseDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var reponse = _mapper.Map<Reponse>(dto);


        _context.Reponses.Add(reponse);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = reponse.ID }, dto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReponseDTO>> GetById(int id)
    {
        var reponse = await _context.Reponses.FindAsync(id);

        if (reponse == null)
            return NotFound();

        var dto = _mapper.Map<ReponseDTO>(reponse);
        return Ok(dto);
    }
}
