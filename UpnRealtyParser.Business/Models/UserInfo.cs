using System.ComponentModel.DataAnnotations.Schema;

namespace UpnRealtyParser.Business.Models
{
    public class UserInfo : IdInfo
    {
        public string Login { get; set; }

        public byte[] PasswordHash { get; set; }

        [NotMapped]
        public string Password { get; set; }
    }
}
