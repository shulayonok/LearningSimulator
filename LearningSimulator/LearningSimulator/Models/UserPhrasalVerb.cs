using SQLite;

namespace LearningSimulator.Models
{
    [Table("UsersPhrasalVerbs")]
    public class UserPhrasalVerb
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int PhrasalVerbID { get; set; }
    }
}
