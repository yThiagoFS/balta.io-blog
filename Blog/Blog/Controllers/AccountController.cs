using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Text.RegularExpressions;

namespace Blog.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly EmailService _emailService;
        private readonly BlogDataContext _context;

        public AccountController(
            TokenService tokenService
            ,EmailService emailService
            ,BlogDataContext context)
        {
            _tokenService = tokenService; 
            _emailService = emailService;
            _context = context;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel userVm) 
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<User>(ModelState.GetErrors()));


            var user = new User
            {
                Name = userVm.Name,
                Email = userVm.Email,
                Slug = userVm.Email.Replace("@", "-").Replace(".", "-")
            };

            var password = PasswordGenerator.Generate(25);
            user.PasswordHash = PasswordHasher.Hash(password);

            try 
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                _emailService.Send(user.Name, user.Email, "Criação de senha", $"Utilize a seguinte senha para realizar o login:{password}");

                return Ok(new ResultViewModel<dynamic>(new { user = user.Email }));
            }
            catch(DbUpdateException) 
            {
                return StatusCode(400, new ResultViewModel<string>("O5X99 - E-mail já cadastrado."));
            }
            catch 
            {
                return StatusCode(500, new ResultViewModel<string>("O5X04 - Falha interna no servidor."));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel loginVm) 
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = await _context
                    .Users
                    .AsNoTracking()
                    .Include(x => x.Roles)
                    .FirstOrDefaultAsync(x => x.Email == loginVm.Email);

            if (user == null || !PasswordHasher.Verify(user.PasswordHash, loginVm.Password))
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos."));

            try 
            {
                var token = _tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, null!));
            }
            catch 
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor."));
            }

        }

        [Authorize]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromBody]UploadImageViewModel uploadImageVm) 
        {
            var fileName = $"{Guid.NewGuid().ToString().ToUpper()}.jpg";

            var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(uploadImageVm.Base64Image, "");

            var bytes = Convert.FromBase64String(data);

            try
            {
                await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X05 - Falha interna no servidor."));
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == User!.Identity!.Name);

            if (user == null)
                return NotFound(new ResultViewModel<Category>("Usuário não encontrado."));

            user.Image = $"https://localhost:7106/images/{fileName}";

            try 
            {
                _context.Users.Update(user);

                await _context.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X06 - Falha interna no servidor."));
            }

            return Ok(new ResultViewModel<Category>("Imagem alterada com sucesso!"));
        }
    }
}
