using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;
using Quizzz.Helper;
using Quizzz.Models;
using Quizzz.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Quizzz.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UtilisateurController : ControllerBase
    {
        private readonly IRepository<Utilisateur> _repository;
        private readonly IMapper _mapper;
        private readonly QuizzContext _context;
        private readonly IConfiguration _config;
        private readonly NETCore.MailKit.Core.IEmailService _emailService;

        public UtilisateurController(IRepository<Utilisateur> repository, IMapper mapper, QuizzContext context, IConfiguration config, NETCore.MailKit.Core.IEmailService emailService)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
            _config = config;
            _emailService = emailService;
        }


        [HttpGet("{id}", Name = "GetUtilisateurById")]
        public async Task<ActionResult<UtilisateurDTO>> GetUtilisateurByIdAsync(int id)
        {
            var utilisateur = await _repository.GetByIdAsync(u => u.Id == id);
            if (utilisateur == null)
                return NotFound($"User with Id {id} not found");

            var utilisateurDTO = _mapper.Map<UtilisateurDTO>(utilisateur);
            return Ok(utilisateurDTO);
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UtilisateurDTO>>> GetUsers()
        {
            var users = await _repository.GetAllAsync();
            var usersDTO = _mapper.Map<List<UtilisateurDTO>>(users);
            return Ok(usersDTO);
        }

        [HttpPost]
        public async Task<ActionResult<UtilisateurDTO>> CreateUser([FromBody] UtilisateurDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var utilisateur = _mapper.Map<Utilisateur>(dto);
            var created = await _repository.CreateAsync(utilisateur);
            dto.Id = created.Id;

            return CreatedAtRoute("GetUtilisateurById", new { id = dto.Id }, dto);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser([FromBody] UtilisateurDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var utilisateur = _mapper.Map<Utilisateur>(dto);
            try
            {
                await _repository.UpdateAsync(utilisateur);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            return NoContent();
        }

        [HttpDelete("delete/{id}", Name = "DeleteUserById")]
        public async Task<ActionResult<bool>> DeleteUserById(int id)
        {
            if (id <= 0)
                return BadRequest();

            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return Ok(true);
        }
      /*  [HttpGet("ByUser/{utilisateurId}")]
        public async Task<ActionResult<CondidatDTO>> GetByUserId(int utilisateurId)
        {
            var candidat = await _context.Candidats
                .Include(c => c.Tests)
                .Include(c => c.Utilisateur)
                .FirstOrDefaultAsync(c => c.UtilisateurId == utilisateurId);

            if (candidat == null)
                return NotFound();

            var dto = _mapper.Map<CondidatDTO>(candidat);
            return Ok(dto);
        }*/
        [HttpGet("ByUsername/{username}")]
        public async Task<ActionResult<UtilisateurDTO>> GetByUserName(string username)
        {
            var user = await _repository.GetByUsernameAsync(u => u.NomUtilisateur == username);

            if (user == null)
                return NotFound($"User with name {username} not found");

            var utilisateurDTO = _mapper.Map<UtilisateurDTO>(user);
            return Ok(utilisateurDTO);
        }
        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate([FromBody] Utilisateur userObj)
        {
            try
            {
                if (userObj == null)
                    return BadRequest(new { Message = "Requête invalide" });

                var user = await _repository.GetByIdAsync(u => u.NomUtilisateur == userObj.NomUtilisateur);
                if (user == null)
                    return NotFound(new { Message = "User Not Found" });

                bool passwordValid = PasswordHasher.VerifyPassword(userObj.MotPasseHache, user.MotPasseHache);
                if (!passwordValid)
                    return Unauthorized(new { Message = "Invalid credentials" });

                return Ok(new
                {
                    Message = "Login Success",
                    utilisateur = new
                    {
                        Id = user.Id,
                        NomUtilisateur = user.NomUtilisateur,
                        Role = user.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erreur serveur", Details = ex.Message });
            }
        }


        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] Utilisateur userObj)
        {
            if (userObj == null)
                return BadRequest();

            if (await checkUserNameAsync(userObj.NomUtilisateur))
                return BadRequest(new { Message = "Username already exists" });

            if (await checkEmailAsync(userObj.Email))
                return BadRequest(new { Message = "Email already exists" });

            var pass = checkPasswordStrength(userObj.MotPasseHache);
            if (!string.IsNullOrEmpty(pass))
                return BadRequest(new { Message = pass });

            userObj.MotPasseHache = PasswordHasher.HashPassword(userObj.MotPasseHache);
            var createdUser = await _repository.CreateAsync(userObj);

            return Ok(new { Message = "Registration Successful", UserId = createdUser.Id });
        }

        private async Task<bool> checkUserNameAsync(string username)
            => await _context.Utilisateurs.AnyAsync(x => x.NomUtilisateur == username);

        private async Task<bool> checkEmailAsync(string email)
            => await _context.Utilisateurs.AnyAsync(x => x.Email == email);

        private string checkPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();

            if (password.Length < 4)
                sb.Append("Minimum Password Length Should Be 4" + Environment.NewLine);

            if (!Regex.IsMatch(password, "[a-z]") || !Regex.IsMatch(password, "[A-Z]") || !Regex.IsMatch(password, "[0-9]"))
                sb.Append("Password Should Be Alphanumeric (lowercase, uppercase, number)" + Environment.NewLine);

            return sb.ToString();
        }

        [HttpPost("send-reset-email/{email}")]
        public async Task<IActionResult> SendResetEmail(string email)
        {
            var user = await _context.Utilisateurs.FirstOrDefaultAsync(a => a.Email == email);
            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Email doesn't exist"
                });
            }

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);
            user.ResetPasswordToken = emailToken;
            user.ResetTokenExpires = DateTime.Now.AddMinutes(15);

            string emailBody = EmailBody.EmailStringBody(email, emailToken);

            try
            {
                await _emailService.SendAsync(email, "Reset Password", emailBody, true);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erreur lors de l'envoi du mail: " + ex.Message });
            }

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                StatusCode = 200,
                Message = "Email Sent"
            });
        }
        [HttpPost("DeleteMultiple")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            try
            {
                var utilisateurs = await _context.Utilisateurs
                    .Where(u => ids.Contains(u.Id))
                    .ToListAsync();

                if (!utilisateurs.Any())
                    return NotFound("Aucun utilisateur trouvé.");

            

                _context.Utilisateurs.RemoveRange(utilisateurs);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new
                {
                    message = "Impossible de supprimer : cet utilisateur est lié à d'autres données.",
                    detail = dbEx.InnerException?.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erreur serveur",
                    detail = ex.Message
                });
            }
        }

        [HttpPost("reset Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTo resetPasswordDTo)
        {
            var newToken = resetPasswordDTo.EmailToken.Replace(" ", "+");

            var user = _context.Utilisateurs.AsNoTracking().FirstOrDefault(a => a.Email == resetPasswordDTo.Email);
            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "User doesn't exist"
                });
            }
            var tokenCode = user.ResetPasswordToken;
            DateTime emailTokenExpiry = user.ResetTokenExpires.Value;
            if(tokenCode !=resetPasswordDTo.EmailToken || emailTokenExpiry < DateTime.Now)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid Reset Link"
                });
            }
            user.MotPasseHache = PasswordHasher.HashPassword(resetPasswordDTo.NewPassword);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(
                new
                {
                    StatusCode = 200,
                    Message = "Password Reset Successfully"
                }
                );
        }
    }
}
