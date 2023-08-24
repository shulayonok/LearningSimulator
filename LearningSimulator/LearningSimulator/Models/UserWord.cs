using SQLite;

namespace LearningSimulator.Models
{
    [Table("UsersWords")]
    public class UserWord
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int WordID { get; set; }
    }
}
