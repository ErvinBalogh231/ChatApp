using ChatServer.Database.Data;
using ChatServer.Database.Models;

namespace ChatServer
{
    static class ServerConsole
    {
        public static void Run(ChatServerContext db)
        {
            Task.Run(() => {

                while (true)
                {
                    string? command = Console.ReadLine();
                    switch (command)
                    {
                        case "help":
                            Console.WriteLine("Log messages - 'messages'\n" +
                                "View users - 'users'\n" +
                                "Add new user - 'add user'\n");
                            break;
                        case "messages":
                            db.Messages.ToList().ForEach(x => Console.WriteLine($"[{x.Username}][{x.SentTime}]: {x.Content}"));
                            break;
                        case "users":
                            db.Users.ToList().ForEach(x => Console.WriteLine($"{x.UserName}, {x.Password}"));
                            break;
                        case "add user":
                            User user = ReadInUser();
                            if (user != null && !db.Users.Any(x => x.UserName == user.UserName))
                            {
                                db.Users.Add(user);
                                db.SaveChanges();
                                Console.WriteLine($"New user added with username: [{user.UserName}] and password: [{user.Password}]");
                            }
                            else
                            {
                                Console.WriteLine($"New user can not be added");
                            }
                            break;
                        default:
                            Console.WriteLine("Invalid command!");
                            break;
                    }
                }
            });
            
        }

        static User ReadInUser()
        {
            Console.WriteLine("Username: ");
            string? username = Console.ReadLine();
            Console.WriteLine("Password: ");
            string? password = Console.ReadLine();

            if (username == "" || password == "")
            {
                return null;
            } 
            else
            {
                User user = new User();
                user.UserName = username;
                user.Password = password;
                return user;
            }
        }
    }
}
