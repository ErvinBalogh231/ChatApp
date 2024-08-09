using System.ComponentModel.DataAnnotations;

namespace ChatServer.Database.Models
{
    internal class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string? UserName { get; set; }
        public DateTime ConnectionTime { get; set; }

        public ICollection<Message> Messages { get; set; } = null!;
    }
}
