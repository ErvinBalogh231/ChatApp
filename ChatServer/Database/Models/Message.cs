using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatServer.Database.Models
{
    internal class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }
        public DateTime SentTime { get; set; }
        public string? Content { get; set; }

        public string Username { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
