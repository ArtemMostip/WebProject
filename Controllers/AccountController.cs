using Microsoft.AspNetCore.Mvc;
using WebProject.Entities;
using WebProject.DTO_s;
using WebProject.Services;
using WebProject.Repositories;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
namespace WebProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailService _emailService;
        private readonly IPasswordService _passwordService;
        private readonly IConfiguration _configuration;
     
        public AccountController(IAccountService accountService, IEmailService emailService, IPasswordService passwordService, IAccountRepository accountRepository, IConfiguration configuration)
        {
            _accountService = accountService;
            _emailService = emailService;
            _passwordService = passwordService;
            _accountRepository = accountRepository;
            _configuration = configuration;
         
        }

        [HttpGet("{id}")]
            public async Task<IActionResult> GetAccount(string id)
            {
                try
                {
                    Account account = await _accountService.GetAccount(id);
                    return Ok(account);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("avatar/{id}")]
        public async Task<IActionResult> GetAvatar(string id)
        {
            var account = await _accountRepository.GetAccountByIdAsync(id);

            if (account == null /*|| account.AvatarImage == null*/ || account.AvatarImage.Length == 0)
            {
                return NotFound("Avatar not found.");
            }
            string contentType = "image/png";

            return File(account.AvatarImage, contentType);
        }

        [HttpPost("Create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAccount([FromForm] AccountRegDTO accountDTO)
            {

            var existingAccount = await _accountRepository.GetAccountByEmailOrNameAsync(accountDTO.Email, accountDTO.Name);
            if (existingAccount != null)
            {
                Console.WriteLine("Account_1");
                return BadRequest( new { Message = "Account with this email or username already exists." });
            }
            if (accountDTO == null)
                {
                Console.WriteLine("Account_2");
                return BadRequest(new { Message = "Account data is null" });
                }

                try
                {
              
                await _accountService.CreateAccountAsync(accountDTO);
                Console.WriteLine("Account_3");
                return Ok(new { Message = "Account created successfully. Please check your email to verify." });
                }
                catch (Exception ex)
                {

                Console.WriteLine(ex.ToString());
                Console.WriteLine("Account_4");
                return BadRequest(new { Message = "Account with this email or username already exists" });
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AccountLoginDTO loginDto)
        {
            try
            {
                var account = await _accountService.LoginAsync(loginDto);

                var token = GenerateJwtToken(account);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }
        private string GenerateJwtToken(Account account)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, account.Id),
        new Claim(ClaimTypes.Role, account.Role)
    };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

       
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateDatas([FromBody] UpdateAccountDTO accountDTO)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {

                    return Unauthorized(new { Message = "User is not authenticated." });
                }

                // Отримуємо акаунт за допомогою id з токену
                var account = await _accountService.GetAccount(userId);
                if (account == null)
                    return NotFound(new { Message = "Account not found." });

                // Перевірка: чи інше ім’я або email не зайняті кимось іншим
                var existingAccount = await _accountRepository.GetAccountByNameAsync(accountDTO.Name);
                if (existingAccount != null)
                    return BadRequest(new { Message = "Another account already uses this email or name." });

                // Генеруємо новий salt та хеш пароля
                var salt = _passwordService.GenerateSalt();
                string hashedPassword = _passwordService.HashPasswordPBKDF2(accountDTO.Password, salt);

                if (account.Name.Length > 16)
                {
                    return BadRequest(new { Message = "Nick-name cannot contain more than 16 characters" });
                }
                else
                {
                    // Оновлюємо поля
                    account.Name = accountDTO.Name;
                    account.PasswordHash = hashedPassword;
                }

                bool result = await _accountRepository.UpdateAccountAsync(account);
                if (!result)
                    return BadRequest(new { Message = "Failed to update account." });

                return Ok(new { Message = "Account updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
