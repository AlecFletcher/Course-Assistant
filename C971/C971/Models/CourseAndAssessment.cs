using System.Collections.ObjectModel;

namespace C971.Models
{
    public class CourseAndAssessment
    {
        public static ObservableCollection<CourseAndAssessment> CourseAndAssessmentList { get; set; } = new ObservableCollection<CourseAndAssessment>();

        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string Status { get; set; }
        public string InstructorName { get; set; }
        public string InstructorPhoneNum { get; set; }
        public string InstructorEmail { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AssessmentOneType { get; set; } = string.Empty;
        public string AssessmentTwoType { get; set; } = string.Empty;
        public string Description { get { return $"{CourseId} - {CourseName}"; } }


        public static async void AssignValues(int TermId)
        {
            CourseAndAssessmentList.Clear();
            var courses = await DBHandler.GetCourses(TermId);
            foreach (var course in courses)
            {
                CourseAndAssessment courseAndAssessment = new CourseAndAssessment();
                courseAndAssessment.CourseId = course.CourseID;
                courseAndAssessment.CourseName = course.Name;
                courseAndAssessment.Status = course.Status;
                courseAndAssessment.InstructorName = course.InstructorName;
                courseAndAssessment.InstructorPhoneNum = course.InstructorPhoneNum;
                courseAndAssessment.InstructorEmail = course.InstructorEmail;
                courseAndAssessment.StartDate = course.StartDate;
                courseAndAssessment.EndDate = course.EndDate;

                var assessments = await DBHandler.GetAssessments(course.CourseID);
                foreach (var assessment in assessments)
                {
                    if (courseAndAssessment.AssessmentOneType == String.Empty)
                    {
                        courseAndAssessment.AssessmentOneType = assessment.AssessmentType + " Assessment";
                    }
                    if (courseAndAssessment.AssessmentOneType != String.Empty && assessments.Count() > 1)
                    {
                        courseAndAssessment.AssessmentTwoType = assessment.AssessmentType + " Assessment";
                    }
                }
                CourseAndAssessmentList.Add(courseAndAssessment);

            }

        }
    }



}
