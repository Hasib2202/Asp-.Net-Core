using ITS.DAL;
using ITS.DAL.Models;
using System;
using System.Linq;

namespace ITS.BLL.Services
{
    public class TokenService
    {
        public static bool IsTokenValid(string tokenKey)
        {
            using (var db = new ITSContext())
            {
                var token = db.Tokens.FirstOrDefault(t => t.Key == tokenKey);
                return token != null && (token.ExpiredAt == null || token.ExpiredAt > DateTime.Now);
            }
        }

        public static bool InvalidateToken(string tokenKey)
        {
            using (var db = new ITSContext())
            {
                var token = db.Tokens.FirstOrDefault(t => t.Key == tokenKey);
                if (token == null) return false;

                db.Tokens.Remove(token);
                return db.SaveChanges() > 0;
            }
        }
    }
}
