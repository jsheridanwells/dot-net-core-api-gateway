namespace LoginService.Models
{
    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public enum Role
        {
            ADMIN,
            PUBLIC
        }
    }
}
