using SQLite;

namespace LearningSimulator.Models
{
    [Table("PhrasalVerbs")]
    public class PhrasalVerb
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int ID { get; set; }
        public string Meaning { get; set; }
        public string Translation { get; set; }
    }
}
