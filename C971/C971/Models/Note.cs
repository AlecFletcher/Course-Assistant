using SQLite;

namespace C971.Models
{
    [Table("Notes")]
    public class Note
    {

        [PrimaryKey, AutoIncrement] public int Id { get; set; }

        public string CourseId { get; set; }

        public string NoteText { get; set; }

        public Note(string courseId, string noteText)
        {
            CourseId = courseId;
            NoteText = noteText;
        }

        public Note() { }
    }
}
