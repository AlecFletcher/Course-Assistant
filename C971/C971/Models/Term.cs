using SQLite;

namespace C971.Models
{
    [Table("Terms")]
    public class Term
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }

        [MaxLength(50)] public string TermName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }


        public Term(string termName, DateTime startDate, DateTime endDate)
        {
            TermName = termName;
            StartDate = startDate;
            EndDate = endDate;
        }

        public Term() { }
    }


}
