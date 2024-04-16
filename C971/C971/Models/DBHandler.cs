using SQLite;

namespace C971.Models
{

    public static class DBHandler
    {
        private static SQLiteAsyncConnection db;
        public static SQLiteConnection dbConnection;

        public static async Task Init() 
        {

            if(db != null )
            {
                return;
            }

            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AlecFletcherDB.db");
            db = new SQLiteAsyncConnection(databasePath);
            dbConnection = new SQLiteConnection(databasePath);
            await db.CreateTableAsync<Assessment>();
            await db.CreateTableAsync<Course>();
            await db.CreateTableAsync<Note>();
            await db.CreateTableAsync<Term>();
        }



        public static async Task ClearTables()
        {
            await Init();
            await db.DeleteAllAsync<Assessment>();
            await db.DeleteAllAsync<Course>();
            await db.DeleteAllAsync<Note>();
            await db.DeleteAllAsync<Term>();
        }



        #region Term Controls
        public static async Task AddTerm(Term term)
        {
            await Init();
            await db.InsertAsync(term);
        }

        public static async Task RemoveTerm(int id)
        {
            await Init();
            await db.DeleteAsync<Term>(id);
        }

        public static async Task<IEnumerable<Term>> GetTerm(int id)
        {
            await Init();
            var termQuery = await db.Table<Term>()
                .Where(i => i.Id == id).ToListAsync();
            
            return termQuery;
        }

        public static async Task<IEnumerable<Term>> GetTerms()
        {
            await Init();

            var terms = await db.Table<Term>().ToListAsync();
            return terms;
        }

        public static async Task UpdateTerm(int id, string termName, DateTime startDate, DateTime endDate)
        {
            await Init();

            var termQuery = await db.Table<Term>()
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();

            if(termQuery != null)
            {
                termQuery.TermName = termName;
                termQuery.StartDate = startDate;
                termQuery.EndDate = endDate;

                await db.UpdateAsync(termQuery);
            }
        }
        #endregion

        #region Course Controls

        public static async Task AddCourse(Course course)
        {
            await Init();
            dbConnection.Insert(course);
        }

        public static async Task RemoveCourse(string courseId)
        {
            await Init();
            await db.DeleteAsync<Course>(courseId);

            var assessmentList = await GetAssessments(courseId);
            foreach(var assessment in assessmentList)
            {
                await RemoveAssessment(assessment.ID);
            }

            var noteList = await GetNotes(courseId);
            foreach(var note in noteList)
            {
                await RemoveNote(note.Id);
            }
        }

        public static async Task<IEnumerable<Course>> GetCourse(string courseId)
        {
            await Init();
            var course = await db.Table<Course>()
                .Where(i => i.CourseID == courseId)
                .ToListAsync();
            return course;
        }

        public static async Task<IEnumerable<Course>> GetCourses(int termId)
        {
            await Init();
            var courses = await db.Table<Course>()
                .Where(i => i.TermID == termId)
                .ToListAsync();
            return courses;
        }

        public static async Task UpdateCourse(string courseID, string name, DateTime startDate, DateTime endDate, string status, string insName, string insPhone, string insEmail, int termId)
        {
            await Init();

            var course = await db.Table<Course>()
                .Where(i => i.CourseID == courseID)
                .FirstOrDefaultAsync();

            if(course != null)
            {
                course.Name = name;
                course.StartDate = startDate;
                course.EndDate = endDate;
                course.Status = status;
                course.InstructorName = insName;
                course.InstructorEmail = insEmail;
                course.InstructorPhoneNum = insPhone;
                course.TermID = termId;

                await db.UpdateAsync(course);
            }
        }

        public static async Task UpdateCourseNewID(string originalCourseId, string newCourseID, string name, DateTime startDate, DateTime endDate, string status, string insName, string insPhone, string insEmail, int termId)
        {
            await Init();

            var course = await db.Table<Course>()
                .Where(i => i.CourseID == originalCourseId)
                .FirstOrDefaultAsync();

            if (course != null)
            {
                course.Name = name;
                course.StartDate = startDate;
                course.EndDate = endDate;
                course.Status = status;
                course.InstructorName = insName;
                course.InstructorEmail = insEmail;
                course.InstructorPhoneNum = insPhone;
                course.TermID = termId;

                await db.DeleteAsync(course);

                course.CourseID = newCourseID;

                await db.InsertAsync(course);
            }
        }

        public static async Task<IEnumerable<Course>> GetAllCourses()
        {
            await Init();
            var courses = await db.Table<Course>()
                .ToListAsync();
            return courses;
        }
        #endregion

        #region Note Controls
        public static async Task AddNote(Note note)
        {
            await Init();
            await db.InsertAsync(note);

        }

        public static async Task RemoveNote(int noteId)
        {
            await Init();
            await db.DeleteAsync<Note>(noteId);
        }

        public static async Task<IEnumerable<Note>> GetNotes(string courseId)
        {
            await Init();
            var notes = await db.Table<Note>()
                .Where(i => i.CourseId == courseId)
                .ToListAsync();

            return notes;
        }

        public static async Task UpdateNote(int noteId, string noteText)
        {
            await Init();
            var note = await db.Table<Note>()
                .Where(i => i.Id == noteId) 
                .FirstOrDefaultAsync();

            note.NoteText = noteText;
            await db.UpdateAsync(note);
        }
        #endregion

        #region Assessment Controls
        public static async Task AddAssessment(Assessment assessment)
        {
            await Init();
            await db.InsertAsync(assessment);
        }

        public static async Task RemoveAssessment(int assessmentId)
        {
            await Init();
            await db.DeleteAsync<Assessment>(assessmentId);
        }

        public static async Task<IEnumerable<Assessment>> GetAssessments(string courseId)
        {
            await Init();
            var assessments = await db.Table<Assessment>()
                .Where(i => i.CourseID == courseId)
                .ToListAsync();

            return assessments;
        }

        public static async Task UpdateAssessment(int id, string courseId, string name, string assessmentType, DateTime startDate, DateTime endDate)
        {
            await Init();
            var assessment = await db.Table<Assessment>()
                .Where(i => i.ID == id)
                .FirstOrDefaultAsync();

            assessment.CourseID = courseId;
            assessment.Name = name;
            assessment.AssessmentType = assessmentType;
            assessment.StartDate = startDate;
            assessment.EndDate = endDate;
        }
        
        #endregion


        public static async Task GenerateSampleData()
        {
            await Init();
            
            for(int i = 1; i < 11; i++)
            {
                Term term = new Term($"Term {i}", DateTime.Now.AddDays(-14), DateTime.Now.AddDays(14));
                await AddTerm(term);

                Course course = new Course($"C97{i}", "Mobile Application Development Using C#", DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), "In Progress", "Anika Patel", "anika.patel@strimeuniversity.edu", "555-123-4567", term.Id, 0, 1);
                await AddCourse(course);

                Assessment assessment_one = new Assessment(course.CourseID, $"Capstone", "Performance", DateTime.Now.AddDays(-3), DateTime.Now.AddDays(3), 1, 0);
                Assessment assessment_two = new Assessment(course.CourseID, $"Quiz", "Objective", DateTime.Now.AddDays(-3), DateTime.Now.AddDays(3), 1, 0);

                await AddAssessment(assessment_one);
                await AddAssessment(assessment_two);

                course.CourseID = $"C38{i}";
                course.Name = "Software Security and Testing";
                await AddCourse(course);

                assessment_one.CourseID = course.CourseID;
                assessment_two.CourseID = course.CourseID;

                await AddAssessment(assessment_one);
                await AddAssessment(assessment_two);

                course.CourseID = $"D42{i}";
                course.Name = "Software Engineering Capstone";
                await AddCourse(course);

                assessment_one.CourseID = course.CourseID;
                assessment_two.CourseID = course.CourseID;

                await AddAssessment(assessment_one);
                await AddAssessment(assessment_two);

            }

        }
    }
}
