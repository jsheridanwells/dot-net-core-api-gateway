using System.Collections.Generic;
using LoginService.Models;

namespace LoginService.Repositories
{
    public class UserRepository
    {
        public List<User> DemoUsers; 
        public UserRepository()
        {
            DemoUsers = new List<User>();
            DemoUsers.Add(new User() { Name = "User1", Password = "Password1" });
            DemoUsers.Add(new User() { Name = "User2", Password = "Password2" });
            DemoUsers.Add(new User() { Name = "User3", Password = "Password3" });
        }

        public User GetUser(string name)
        {
            try 
            {
                return DemoUsers.Find(u => u.Name.Equals(name));
            }
            catch
            {
                return null;
            }
        }
    }
}
