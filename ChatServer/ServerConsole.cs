using ChatServer.Database.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer
{
    static class ServerConsole
    {
        public static void Run(ChatServerContext db)
        {
            Task.Run(() => {

                while (true)
                {
                    string command = Console.ReadLine();
                    switch (command)
                    {
                        case "help":
                            Console.WriteLine("Get user history - 'users'\n" +
                            "Log messages - 'messages'\n");
                            break;
                        case "users":
                            db.Users.ToList().ForEach(x => Console.WriteLine($"[{x.ConnectionTime}] {x.UserName}"));
                            break;
                        case "messages":
                            db.Messages.ToList().ForEach(x => Console.WriteLine($"[{x.UId}][{x.SentTime}] {x.Content}"));
                            break;
                        default:
                            Console.WriteLine("Invalid command!");
                            break;
                    }
                }
            });
            
        }
    }
}
