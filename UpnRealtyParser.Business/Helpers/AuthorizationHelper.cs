using System;
using System.Linq;
using System.Security.Cryptography;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class AuthorizationHelper
    {
        private readonly EFGenericRepo<UserInfo, RealtyParserContext> _userRepo;

        public AuthorizationHelper(EFGenericRepo<UserInfo, RealtyParserContext> userRepo)
        {
            _userRepo = userRepo;
        }

        /// <summary>
        /// Проверка реквизитов пользователя (пароль сверяется по хэшу)
        /// </summary>
        public UserInfo Authenticate(string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                return null;

            UserInfo foundUser = _userRepo.GetAllWithoutTracking().FirstOrDefault(x => x.Login == login);

            if (foundUser == null)
                return null;

            byte[] requiredHash = foundUser.PasswordHash;

            if (!verifyPasswordHash(password, requiredHash))
                return null;

            UserInfo user = new UserInfo { Login = login };
            return user;
        }

        private bool verifyPasswordHash(string password, byte[] storedHash)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var computedHash = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
