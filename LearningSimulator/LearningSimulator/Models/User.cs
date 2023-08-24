using SQLite;

namespace LearningSimulator.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int ID { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Initials { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Answer { get; set; }

        public User(User user)
        {
            ID = user.ID;
            Name = user.Name;
            Surname = user.Surname;
            Initials = user.Initials;
            Username = user.Username;
            Password = user.Password;
            Email = user.Email;
            Answer = user.Answer;
        }
        public User() { }
    }
}
