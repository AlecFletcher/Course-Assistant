namespace C971;

using Models;
using System.Collections.ObjectModel;

public partial class CourseOverview : ContentPage
{
	Course courseToBeEdited;
	ObservableCollection<Note> NoteList = new ObservableCollection<Note>();
	public CourseOverview()
	{
		InitializeComponent();
	}

	public CourseOverview(string courseId)
	{
		InitializeComponent();
        AssignCourse(courseId);
        Title = "Course Overview";
	}
    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        CourseCollectionView.ItemsSource = await DBHandler.GetCourse(courseToBeEdited.CourseID);
        NoteList.Clear();
        var notes = await DBHandler.GetNotes(courseToBeEdited.CourseID);
        foreach (var note in notes)
        {
            NoteList.Add(note);
        }
        NotesCollectionView.ItemsSource = NoteList;

		var assessments = await DBHandler.GetAssessments(courseToBeEdited.CourseID);
		AssessmentsCollectionView.ItemsSource = assessments;

        CourseName_Label.Text = courseToBeEdited.Description;
        CourseName_Label.HorizontalTextAlignment = TextAlignment.Center;
    }

    private async void AssignCourse(string courseId)
    {
        var Course = await DBHandler.GetCourse(courseId);
        courseToBeEdited = Course.First();
    }

    private async void EditToolBar_Clicked(object sender, EventArgs e)
    {
        AddEditCourse addEditCourse = new AddEditCourse(courseToBeEdited.TermID, courseToBeEdited.CourseID);
        await Navigation.PushAsync(addEditCourse);
    }

    private async void DeleteToolbar_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Delete?", "Are you sure you want to delete this course?", "Yes", "No");
        if (answer)
        {
            //Check if course is in the DB
            var CourseList = await DBHandler.GetCourses(courseToBeEdited.TermID);
            foreach (var tempCourse in CourseList)
            {
                if (tempCourse.CourseID == courseToBeEdited.CourseID)
                {
                    await DBHandler.RemoveCourse(courseToBeEdited.CourseID);
                }
            }

            await Navigation.PopAsync();
        }
    }

    private async void AddNote_Clicked(object sender, EventArgs e)
    {
        AddEditNote addNote = new AddEditNote(courseToBeEdited.CourseID);
        await Navigation.PushAsync(addNote);
    }

    private async void EditNote_Clicked(object sender, EventArgs e)
    {
        ImageButton editButton = (ImageButton)sender;
        Note noteToBeEdited = (Note)editButton.BindingContext;
        AddEditNote editNote = new AddEditNote(noteToBeEdited);

        await Navigation.PushAsync(editNote);
    }

    private async void DeleteNote_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Delete?", "Are you sure you want to delete this note?", "Yes", "No");
        if (answer)
        {
            ImageButton deleteButton = (ImageButton)sender;
            Note note = (Note)deleteButton.BindingContext;
            NoteList.Remove(note);
            await DBHandler.RemoveNote(note.Id);
        }
    }

    private async void ShareButton_Pressed(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Note note = (Note)button.BindingContext;
        await ShareNote(note.CourseId, note.NoteText);
    }

    public async Task ShareNote(string courseId, string noteText)
    {
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = noteText,
            Title = courseId
        });
    }
}