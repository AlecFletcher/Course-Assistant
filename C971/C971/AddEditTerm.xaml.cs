using C971.Models;
using System.Collections.ObjectModel;

namespace C971;

public partial class AddEditTerm : ContentPage
{
    Term term = new Term();
    bool isBeingEdited = false;
    public static ObservableCollection<Course> Courses { get; set; } = new ObservableCollection<Course>();
    public AddEditTerm()
	{
		InitializeComponent();
	}

    public AddEditTerm(Term t, bool editing)
    {
        InitializeComponent();
        term = t;
        isBeingEdited = editing;
        if (editing)
        {
            Title = "Edit Term";
        }
        else
        {
            Title = "Add Term";
        }
    }

    protected override bool OnBackButtonPressed()
    {
        ExitFunction();
        return base.OnBackButtonPressed();
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        CoursesCollectionView.ItemsSource = Courses;

        var courses = await DBHandler.GetCourses(term.Id);
        if(courses.Count() > 0)
        {
            Courses.Clear();
            foreach (Course course in courses)
            {
                Courses.Add(course);
            }
        }

        TermName_Entry.Text = term.TermName;
        StartDate_DatePicker.Date = term.StartDate;
        EndDate_DatePicker.Date = term.EndDate;

    }



    private async void EditCourse_Button(object sender, EventArgs e)
    {
        ImageButton imageButton = (ImageButton)sender;
        Course course = (Course)imageButton.BindingContext;
        if (course.CourseID == string.Empty)
        {
            AddEditCourse addEditCourse = new AddEditCourse(term.Id);
            await Navigation.PushAsync(addEditCourse);
        }
        else
        {
            AddEditCourse addEditCourse = new AddEditCourse(term.Id, course.CourseID);
            await Navigation.PushAsync(addEditCourse);
        }
    }

    private async void DeleteCourse_Button(object sender, EventArgs e)
    {
        ImageButton button = (ImageButton)sender;
        Course course = (Course)button.BindingContext;
        
        bool answer = await DisplayAlert("Delete?", "Are you sure you want to delete this course?", "Yes", "No");
        if (answer)
        {
            //Check if course is in the DB
            var CourseList = await DBHandler.GetCourses(term.Id);
            foreach(var tempCourse in CourseList)
            {
                if(tempCourse.CourseID == course.CourseID)
                {
                    await DBHandler.RemoveCourse(course.CourseID);
                }
            }

            Courses.Remove(course);
        }
    }

    private async void SaveTerm_Button(object sender, EventArgs e)
    {
        //Perform basic data validation
        if (TermName_Entry.Text == null || TermName_Entry.Text.Replace(" ", "") == "")
        {
            await DisplayAlert("Error", "Term name cannot be blank", "Continue");
            return;
        }
        if(StartDate_DatePicker.Date > EndDate_DatePicker.Date)
        {
            await DisplayAlert("Error", "Term start date is later than the end date.", "Continue");
            return;
        }

        //If updating
        if (isBeingEdited)
        {
            await DBHandler.UpdateTerm(term.Id, TermName_Entry.Text, StartDate_DatePicker.Date, EndDate_DatePicker.Date);
            Courses.Clear();
            await Navigation.PopAsync();
        }


        //If new term
        else
        {
            term.TermName = TermName_Entry.Text;
            term.StartDate = StartDate_DatePicker.Date;
            term.EndDate = EndDate_DatePicker.Date;

            if(TermName_Entry.Text.Replace(" ","") == "")
            {
                await DisplayAlert("Error", "Term name is blank.", "Continue");
                return;
            }

            int courseCount = 1;
            foreach (var course in Courses)
            {
                if (course.HasErrors())
                {
                    await DisplayAlert("Error", $"Course {courseCount} has issues. Please verify start date is before end date and no fields are left blank. Delete any blank courses before adding this term.", "Continue");
                    return;
                }
                courseCount++;
            }

            //Now all checks have passed, add term to DB
            await DBHandler.AddTerm(term);

            //Add each course for the term to DB
            foreach (var course in Courses)
            {
                Course newCourse = course;
                await DBHandler.RemoveCourse(course.CourseID);
                newCourse.TermID = term.Id;
                await DBHandler.AddCourse(newCourse);

            }
            Courses.Clear();



            await Navigation.PopAsync();
        }
    }

    private void AddCourse_Button_Clicked(object sender, EventArgs e)
    {
        Course course = new Course(string.Empty, "", DateTime.Now, DateTime.Now, "", "", "", "", term.Id, 0, 0);
        Courses.Add(course);
    }

    private async void ExitFunction()
    {
        //Check if term exists in DB
        var checkedTerm = await DBHandler.GetTerm(term.Id);
        
        //If term is not in DB delete all courses and assessments associated with term
        if (checkedTerm.Count() == 0)
        {
            var invalidCourses = await DBHandler.GetCourses(term.Id);
            foreach (var course in invalidCourses)
            {
                var assessments = await DBHandler.GetAssessments(course.CourseID);
                foreach(var assessment in assessments)
                {
                    await DBHandler.RemoveAssessment(assessment.ID);
                }
                await DBHandler.RemoveCourse(course.CourseID);
            }
        }
        Courses.Clear();
    }

    private async void CoursesCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Course course = (Course)e.CurrentSelection[0];
        CourseOverview courseOverview = new CourseOverview(course.CourseID);
        await Navigation.PushAsync(courseOverview);
    }

}