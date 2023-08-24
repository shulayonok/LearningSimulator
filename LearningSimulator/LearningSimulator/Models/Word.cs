using SQLite;

namespace LearningSimulator.Models
{
    [Table("Words")]
    public class Word
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int ID { get; set; }
        public string Meaning { get; set; }
        public string Translation { get; set; }
        public byte PartOfSpeech { get; set; }
    }
}
