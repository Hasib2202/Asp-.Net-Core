using AutoMapper;
using ITS.BLL.DTOs;
using ITS.BLL.Mappings;
using ITS.DAL;
using ITS.DAL.Models;
using ITS.DAL.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ITS.BLL.Services
{
    public class UserService
    {
        private static Mapper mapper = AutoMapperConfig.GetMapper();

        public static bool Register(UserDTO userDto)
        {
            using (var db = new ITSContext())
            {
                // Check if username or email already exists
                if (db.Users.Any(u => u.Username == userDto.Username || u.Email == userDto.Email))
                    return false;

                var user = mapper.Map<User>(userDto);

                // Hash password before storing
                user.Password = HashPassword(user.Password);

                db.Users.Add(user);
                return db.SaveChanges() > 0;
            }
        }

        public static UserDTO Login(string username, string password, out string token)
        {
            using (var db = new ITSContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username);
                token = null;

                if (user != null && VerifyPassword(password, user.Password))
                {
                    // Generate Token
                    token = Guid.NewGuid().ToString();

                    db.Tokens.Add(new Token
                    {
                        Key = token,
                        CreatedAt = DateTime.Now,
                        UserId = user.Id
                    });
                    db.SaveChanges();

                    var dto = mapper.Map<UserDTO>(user);
                    dto.Token = token;
                    return dto;
                }
                return null;
            }
        }

        // 🔐 Hash password
        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var hashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashed);
            }
        }

        // ✅ Compare password
        private static bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            var hashOfInput = HashPassword(enteredPassword);
            return hashOfInput == storedHashedPassword;
        }

        public static bool Logout(string token)
        {
            using (var db = new ITSContext())
            {
                var existing = db.Tokens.FirstOrDefault(t => t.Key == token);
                if (existing != null)
                {
                    db.Tokens.Remove(existing);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
        }
    }
}
