using System.ComponentModel.DataAnnotations;

namespace ChatServer.Database.Models
{
    internal class User
    {
        [Key]
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;

        public ICollection<Message> Messages { get; set; } = null!;
    }
}
