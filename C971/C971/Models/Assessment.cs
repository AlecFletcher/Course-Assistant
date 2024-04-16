using SQLite;

namespace C971.Models
{
    [Table("Assessments")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement] public int ID { get; set; }

        public string CourseID { get; set; }

        public string Name { get; set; }

        public string AssessmentType { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }

        public int NotificationStart { get; set; }

        public int NotificationEnd { get; set; }

        public string NotificationStartString { get { if (NotificationStart == 0) return "False"; else return "True"; } }
        
        public string NotificationEndString { get { if (NotificationEnd == 0) return "False"; else return "True"; } }

        public Assessment()
        {

        }

        public Assessment(string assessmentType)
        {
            AssessmentType = assessmentType;
        }

        public Assessment(string courseID, string name, string assessmentType, DateTime startDate, DateTime endDate, int notificationStart, int notificationEnd)
        {
            CourseID = courseID;
            Name = name;
            AssessmentType = assessmentType;
            StartDate = startDate;
            EndDate = endDate;
            NotificationStart = notificationStart;
            NotificationEnd = notificationEnd;
        }
    }
}
