using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebProject.DTO_s;
using WebProject.Entities;
using WebProject.Models;
using WebProject.Repositories;

namespace WebProject.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailService _emailService;
        private readonly IPasswordService _passwordService;
        public AccountService(IAccountRepository accountRepository, IEmailService emailService, IPasswordService passwordService)
        {
            _accountRepository = accountRepository;
            _emailService = emailService;
            _passwordService = passwordService;
       
        }

        public async Task<Account> GetAccount(string id)
        {
            var accounts = await _accountRepository.GetAccountByIdAsync(id);
            return accounts;
        }

        public async Task<List<string>> GetAllEmailsAsync()
        {
            var accounts = await _accountRepository.GetAllVerifiedAccountsAsync();
            var emails = accounts.Select(account => account.Email).ToList();
            return emails;
        }
        public async Task<bool> CreateAccountAsync( AccountRegDTO accountDTO)
        {
            var salt = _passwordService.GenerateSalt();
            string hashedPassword = _passwordService.HashPasswordPBKDF2(accountDTO.Password, salt);

            byte[]? avatarImage = null;
            if (accountDTO.AvatarFile != null && accountDTO.AvatarFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await accountDTO.AvatarFile.CopyToAsync(ms);
                avatarImage = ms.ToArray();
            }

            var account = new Account
            {
                Name = accountDTO.Name,
                Email = accountDTO.Email,
                PasswordHash = hashedPassword,
                DateCreation = DateTime.Now,
                VerifiedAt = null,
                Token = Guid.NewGuid().ToString(),
                AvatarImage = avatarImage  
            };

            bool emailSent = await SendVerificationEmailAsync(account.Email, account.Token);
            if (!emailSent)
            {
                throw new Exception("Failed to send verification email.");
            }

            try
            {
                await _accountRepository.CreateAccountAsync(account);
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new Exception("Account with this email or username already exists.");
            }

            return true;
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var account = await _accountRepository.GetAccountByTokenAsync(token);
            if (account == null)
                return false;

            account.VerifiedAt = DateTime.UtcNow;
            return await _accountRepository.UpdateAccountAsync(account);
        }

        private async Task<bool> SendVerificationEmailAsync(string email, string token)
        {
            var subject = "Email Verification";
            var body = $"<b>Please verify your email!<b> \n <a href='http://localhost:5000/Verification/verify-email?token={token}'>[Verify Now!]</a>";

            var emailRequest = new EmailRequest
            {
                To = email,
                Subject = subject,
                Body = body
            };

            return await _emailService.SendEmailAsync(emailRequest);
        }
        
        public async Task<Account> LoginAsync(AccountLoginDTO loginDto)
        {
            var account = await _accountRepository.GetAccountByEmailOrNameAsync(loginDto.Email, loginDto.Email);
            if (account == null)
            {
                throw new Exception("Invalid email or password.");
            }

            if (account.VerifiedAt == null)
            {
                throw new Exception("Email is not verified.");
            }

            if (!_passwordService.VerifyPassword(loginDto.Password, account.PasswordHash))
            {
                throw new Exception("Invalid email or password.");
            }

            return account;
        }

        public async Task<bool> ValidateAccountAsync(string email, string password)
        {
            var account = await _accountRepository.GetAccountByEmailAsync(email);
            if (account == null)
                return false;

            return _passwordService.VerifyPassword(password, account.PasswordHash);

        }

    }
    }
