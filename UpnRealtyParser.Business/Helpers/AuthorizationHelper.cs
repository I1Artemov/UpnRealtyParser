using System;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Business.Helpers
{
    public class AuthorizationHelper
    {
        public UserInfo Authenticate(string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                return null;

            if (login != "admin")
                return null;

            byte[] requiredHash = new System.Security.Cryptography.HMACSHA512()
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes("nothing"));

            if (!verifyPasswordHash(password, requiredHash))
                return null;

            UserInfo user = new UserInfo { Login = login };
            return user;
        }

        private bool verifyPasswordHash(string password, byte[] storedHash)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
