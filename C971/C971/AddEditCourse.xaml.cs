using C971.Models;
using Plugin.LocalNotification;

namespace C971;

public partial class AddEditCourse : ContentPage
{
	int TermId;
	string CourseID;
    bool IsBeingEdited = false;
	Assessment ObjectiveAssessment = new Assessment("Objective");
	Assessment PerformanceAssessment = new Assessment("Performance");
	public AddEditCourse()
	{
		InitializeComponent();
	}

	//Overload for editing course
	public AddEditCourse(int termId, string courseId)
	{
		InitializeComponent();
		TermId = termId;
		CourseID = courseId;
        IsBeingEdited = true;
		Save_Button.Text = "Save Course Changes";
        Title = "Edit Course";
	}


	//Overload for creating new course
	public AddEditCourse(int termId) 
	{
        InitializeComponent();
        TermId = termId;
		Save_Button.Text = "Add Course";
        Title = "Add Course";
        OA_StartDate_DatePicker.BindingContext = null;
        OA_EndDate_DatePicker.BindingContext = null;
        PA_StartDate_DatePicker.BindingContext = null;
        PA_EndDate_DatePicker.BindingContext = null;
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        //Pre-fill course information for entries
        if (CourseID != null)
		{
            var placeHolder = await DBHandler.GetCourse(CourseID);
            Course course = placeHolder.First();
            CourseCollectionView.BindingContext = course;
            CourseCollectionView1.BindingContext = course;
            InstructorCollectionView.BindingContext = course;

            var assessments = await DBHandler.GetAssessments(CourseID);

			foreach(var assessment in assessments)
			{
				if (assessment.AssessmentType == "Objective")
				{
					ObjectiveAssessment_Grid.BindingContext = assessment;
					ObjectiveAssessmentEnabled_Checkbox.IsChecked = true;

				}
				else
				{
					PerformanceAssessment_Grid.BindingContext = assessment;
					PerformanceAssessmentEnabled_Checkbox.IsChecked = true;
				}
			}
        }
		else
		{
			PerformanceAssessment_Grid.BindingContext = PerformanceAssessment;
			ObjectiveAssessment_Grid.BindingContext = ObjectiveAssessment;
        }
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        //Create course instance and check for errors

        if(ID_Entry.Text == null || ID_Entry.Text.Replace(" ", "") == string.Empty)
        {
            await DisplayAlert("Error", "Course ID is empty.", "Continue");
            return;
        }

        if (Name_Entry.Text == null || Name_Entry.Text.Replace(" ", "") == string.Empty)
        {
            await DisplayAlert("Error", "Course name is empty.", "Continue");
            return;
        }

        if(Status_Picker.SelectedIndex == -1)
        {
            await DisplayAlert("Error", "Status is empty.", "Continue");
            return;
        }

        if (InsName_Entry.Text == null || InsName_Entry.Text.Replace(" ", "") == string.Empty)
        {
            await DisplayAlert("Error", "Instructor name is empty.", "Continue");
            return;
        }

        if (InsPhoneNum_Entry.Text == null || InsPhoneNum_Entry.Text.Replace(" ", "") == string.Empty)
        {
            await DisplayAlert("Error", "Instructor phone number is blank.", "Continue");
            return;
        }

        if (InsPhoneNum_Entry.Text.Count() < 10)
        {
            await DisplayAlert("Error", "Instructor phone number is too short.", "Continue");
            return;
        }

        if (InsEmail_Entry.Text == null || InsEmail_Entry.Text.Replace(" ", "") == string.Empty)
        {
            await DisplayAlert("Error", "Instructor email is empty.", "Continue");
            return;
        }

        if(!InsEmail_Entry.Text.Contains("@") && !InsEmail_Entry.Text.Contains("."))
        {
            await DisplayAlert("Error", "Instructor email is not valid.", "Continue");
            return;
        }

        if (CourseStartDate_DatePicker.Date > CourseEndDate_DatePicker.Date)
        {
            await DisplayAlert("Error", "Course start date is later than the end date.", "Continue");
            return;
        }

        var termList = await DBHandler.GetTerm(TermId);
        Term t = termList.First();

        if(CourseStartDate_DatePicker.Date < t.StartDate)
        {
            await DisplayAlert("Error", "Course start date is before the terms start date.", "Continue");
            return;
        }

        if (CourseStartDate_DatePicker.Date > t.EndDate)
        {
            await DisplayAlert("Error", "Course start date is after the terms end date.", "Continue");
            return;
        }

        if (CourseEndDate_DatePicker.Date < t.StartDate)
        {
            await DisplayAlert("Error", "Course end date is before the terms start date.", "Continue");
            return;
        }

        if (CourseEndDate_DatePicker.Date > t.EndDate)
        {
            await DisplayAlert("Error", "Course end date is after the terms end date.", "Continue");
            return;
        }

        Course courseToBeAdded = new Course(ID_Entry.Text, Name_Entry.Text, CourseStartDate_DatePicker.Date, CourseEndDate_DatePicker.Date, Status_Picker.SelectedItem.ToString(), InsName_Entry.Text, InsEmail_Entry.Text, InsPhoneNum_Entry.Text, TermId, CourseStartNotification_Picker.SelectedIndex, CourseEndNotification_Picker.SelectedIndex);

        //Check for duplicate IDs
        var courses = await DBHandler.GetAllCourses();
        foreach (var course in courses)
        {
            //If course is being added, perform simple check to see if course ID exists. 
            //If course is being edited, check if the ID has been changed and check for duplicates
            if ((course.CourseID == courseToBeAdded.CourseID && !IsBeingEdited) || (course.CourseID == courseToBeAdded.CourseID && course.CourseID != courseToBeAdded.CourseID && IsBeingEdited))
            {
                var term = await DBHandler.GetTerm(course.TermID);
                await DisplayAlert("Error", $"Course exists in {term.First().TermName}", "Continue");
                return;
            }
        }


        //Create assessments and check for errors
        Assessment OA = new Assessment(courseToBeAdded.CourseID, OA_Name.Text, "Objective", OA_StartDate_DatePicker.Date, OA_EndDate_DatePicker.Date, OA_StartNotification_Picker.SelectedIndex, OA_EndNotification_Picker.SelectedIndex);
		Assessment PA = new Assessment(courseToBeAdded.CourseID, PA_Name.Text, "Performance", PA_StartDate_DatePicker.Date, PA_EndDate_DatePicker.Date, PA_StartNotification_Picker.SelectedIndex, PA_EndNotification_Picker.SelectedIndex);


        bool addObjectiveAssessment = false;
        bool addPerformanceAssessment = false;

		if(ObjectiveAssessmentEnabled_Checkbox.IsChecked)
		{

            //Check for blank / invalid fields
            if (OA_Name.Text == null || OA_Name.Text.Replace(" ", "") == string.Empty)
            {
                await DisplayAlert("Error", "Objective Assessment name is blank.", "Continue");
                return;
            }

            if (OA.StartDate > OA.EndDate)
            {
                await DisplayAlert("Error", "Objective Assessment start date is later than the end date.", "Continue");
                return;
            }
            addObjectiveAssessment = true;

        }
        if (PerformanceAssessmentEnabled_Checkbox.IsChecked)
        {
            //Check for blank / invalid fields
            if(PA_Name.Text == null || PA_Name.Text.Replace(" ","") == string.Empty)
            {
                await DisplayAlert("Error", "Performance Assessment name is blank.", "Continue");
                return;
            }

            if (PA.StartDate > PA.EndDate)
            {
                await DisplayAlert("Error", "Performance Assessment start date is later than the end date.", "Continue");
                return;
            }

            addPerformanceAssessment = true;
        }

        //If course is being edited
        if (IsBeingEdited)
        {
            //Remove assessments and re-add them instead of tracking previous status' and calling the update.
            var assessments = await DBHandler.GetAssessments(CourseID);
            foreach ( var assessment in assessments)
            {
                await DBHandler.RemoveAssessment(assessment.ID);
            }

            //In case of the course ID being changed, find all notes associated with the course and update their courseID property
            var notes = await DBHandler.GetNotes(CourseID);
            foreach ( var note in notes)
            {
                note.CourseId = courseToBeAdded.CourseID;
            }

            //If course ID was changed
            if(CourseID != courseToBeAdded.CourseID)
            {
                //Check to see if new course id exists
                var courseList = await DBHandler.GetAllCourses();
                foreach(var course in courseList)
                {
                    if(courseToBeAdded.CourseID == course.CourseID)
                    {
                        var term = await DBHandler.GetTerm(course.TermID);
                        await DisplayAlert("Error", $"Course exists in {term.First().TermName}", "Continue");
                        return;

                    }
                }
                //Call separate "update" function
                await DBHandler.UpdateCourseNewID(CourseID, courseToBeAdded.CourseID, courseToBeAdded.Name, courseToBeAdded.StartDate, courseToBeAdded.EndDate,
    courseToBeAdded.Status, courseToBeAdded.InstructorName, courseToBeAdded.InstructorPhoneNum, courseToBeAdded.InstructorEmail, TermId);
                
                
                //Pull to main dashboard as course overview would still hold old course information
                await Navigation.PopToRootAsync();
            }

            //If the update is not to the course Id, call standard update
            else
            {
                await DBHandler.UpdateCourse(CourseID, courseToBeAdded.Name, courseToBeAdded.StartDate, courseToBeAdded.EndDate,
    courseToBeAdded.Status, courseToBeAdded.InstructorName, courseToBeAdded.InstructorPhoneNum, courseToBeAdded.InstructorEmail, TermId);

            }
        }


        //If course is not being updated, add course
        else
        {
            await DBHandler.AddCourse(courseToBeAdded);
            if(courseToBeAdded.StartNotification == 1)
            {
                //Schedule notifications if they're enabled
                SetNotification(Convert.ToInt32(TermId.ToString() + PA.ID.ToString() + "1") * 1000, courseToBeAdded.Name, "Course Starting", $"Your course {courseToBeAdded.CourseID} is about to begin", courseToBeAdded.StartDate.Date);

            }

            if (courseToBeAdded.EndNotification == 1)
            {
                //Schedule notifications if they're enabled
                SetNotification(Convert.ToInt32(TermId.ToString() + PA.ID.ToString() + "2") * 1000, courseToBeAdded.Name, "Course Ending", $"Your course {courseToBeAdded.CourseID} is about to end", courseToBeAdded.StartDate.Date);

            }

        }

        //Add assessments if the checkbox is enabled
        if (addObjectiveAssessment)
        {
            //Set notifications for objective assessment
            if (OA.NotificationStart == 1)
            {
                SetNotification(Convert.ToInt32(PA.ID.ToString() + TermId.ToString() + "1"), courseToBeAdded.Name, "Objective Assessment", "Your objective assessment is about to end", OA_StartDate_DatePicker.Date);
            }

            if (OA.NotificationEnd == 1)
            {
                SetNotification(Convert.ToInt32(PA.ID.ToString() + TermId.ToString() + "2"), courseToBeAdded.Name, "Objective Assessment", "Your objective assessment is about to end", OA_EndDate_DatePicker.Date);
            }

            await DBHandler.AddAssessment(OA);
        }


        if (addPerformanceAssessment)
        {
            //Set notifications for performance assessments
            if (PA.NotificationStart == 1)
            {
                SetNotification(Convert.ToInt32(PA.ID.ToString() + TermId.ToString() + "3"), courseToBeAdded.Name, "Performance Assessment", "Your performance assessment is about to begin", PA_StartDate_DatePicker.Date);
            }

            if (PA.NotificationEnd == 1)
            {
                SetNotification(Convert.ToInt32(PA.ID.ToString() + TermId.ToString() + "4"), courseToBeAdded.Name, "Performance Assessment", "Your performance assessment is about to end", PA_EndDate_DatePicker.Date);
            }


            await DBHandler.AddAssessment(PA);
        }

        await Navigation.PopAsync();
    }

    private async void SetNotification(int id, string name, string type, string message, DateTime date)
    {
        var request = new NotificationRequest
        {
            NotificationId = id,
            Title = name,
            Subtitle = type,
            Description = message,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = date
            }
        };

        await LocalNotificationCenter.Current.Show(request);
    }

}