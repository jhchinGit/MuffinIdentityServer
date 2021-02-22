using System.ComponentModel.DataAnnotations.Schema;

namespace MuffinIdentityServer.Models
{
    [Table("user_profile")]
    public class UserProfile
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("password")]
        public string Password { get; set; }
    }
}