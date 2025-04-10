namespace TeaByGreen.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; } // Could be unique if you want authentication
        public string PasswordHash { get; set; } // Store hashed password (not plain text)
    }

}
