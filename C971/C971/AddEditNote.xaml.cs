namespace C971;
using Models;

public partial class AddEditNote : ContentPage
{
	Note note { get; set; }
	bool beingEdited = false;
	public AddEditNote()
	{
		InitializeComponent();
	}

	//Editing note
	public AddEditNote(Note n)
	{
		InitializeComponent();
		note = n;
		beingEdited = true;
		Title = "Edit Note";
	}


    //Adding note
    public AddEditNote(string courseId)
    {
        InitializeComponent();
		note = new Note();
		note.CourseId = courseId;
		Title = $"Add Note for {courseId}";
    }

    private async void SaveNote_Button(object sender, EventArgs e)
    {
		if(NoteText_Editor.Text == null || NoteText_Editor.Text == "")
		{
			await DisplayAlert("Error", "Note text cannot be blank.", "Continue");
			return;
		}
		if (beingEdited)
		{
            string text = NoteText_Editor.Text;
			await DBHandler.UpdateNote(note.Id, text);
			await Navigation.PopAsync();
			return;
        }
		note.NoteText = NoteText_Editor.Text;
		await DBHandler.AddNote(note);
		await Navigation.PopAsync();
		return;
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
		Note_Grid.BindingContext = note;
    }

    private async void DeleteNote_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Delete?", "Are you sure you want to delete this note?", "Yes", "No");
		if(answer)
		{
			await DBHandler.RemoveNote(note.Id); 
			await Navigation.PopAsync();
		}
    }
}