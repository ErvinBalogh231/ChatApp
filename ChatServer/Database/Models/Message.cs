using System.ComponentModel.DataAnnotations;

namespace ChatServer.Database.Models
{
    internal class Message
    {
        [Key]
        public int MessageId { get; set; }
        public DateTime SentTime { get; set; }
        public string? Content { get; set; }

        public Guid UId { get; set; }
        public User User { get; set; } = null!;
    }
}
