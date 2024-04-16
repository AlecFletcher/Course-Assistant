using SQLite;

namespace C971.Models
{

    [Table("Courses")]
    public class Course
    {
        [PrimaryKey] public string CourseID { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; }

        public int TermID { get; set; }

        public string InstructorName { get; set; }

        public string InstructorEmail { get; set; }

        public string InstructorPhoneNum { get; set; }

        public int StartNotification { get; set; }

        public int EndNotification { get; set; }

        public string Description { get { return $"{CourseID} - {Name}"; } }

        public Course()
        {

        }

        public Course(string courseID, string name, DateTime startDate, DateTime endDate, string status, string instructorName, string instructorEmail, string instructorPhoneNum, int termID, int notificationStart, int notificationEnd)
        {
            CourseID = courseID;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            Status = status;
            InstructorName = instructorName;
            InstructorEmail = instructorEmail;
            InstructorPhoneNum = instructorPhoneNum;
            TermID = termID;
            StartNotification = notificationStart;
            EndNotification = notificationEnd;
        }

        public bool HasErrors()
        {
            //Remove whitespace to see if entries are blank
            if (CourseID == string.Empty || CourseID.Replace(" ", "") == "")
            {
                return true;
            }

            if (Name == string.Empty || Name.Replace(" ", "") == "")
            {
                return true;
            }

            if (InstructorName == string.Empty || InstructorName.Replace(" ", "") == "")
            {
                return true;
            }

            if (!InstructorEmail.Contains("@") && !InstructorEmail.Contains("."))
            {
                return true;
            }

            if (InstructorPhoneNum == string.Empty || InstructorPhoneNum.Replace(" ", "") == "" || InstructorPhoneNum.Count() < 7)
            {
                return true;
            }


            return false;
        }
    }
}
