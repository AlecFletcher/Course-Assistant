using C971.Models;

namespace C971;

public partial class TermOverview : ContentPage
{
	int TermId;
	int count = 0;
	public TermOverview()
	{
		InitializeComponent();
	}

	public TermOverview(int term_id)
	{
		InitializeComponent();
		TermId = term_id;
		Title = "Term Overview";
	}

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
		var selectedTerm = await DBHandler.GetTerm(TermId);
		TermName_Label.Text = selectedTerm.First().TermName;
		CourseAndAssessment.AssignValues(selectedTerm.First().Id);
    }

    private async void CoursesCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CourseAndAssessment course = e.CurrentSelection[0] as CourseAndAssessment;
		CourseOverview courseOverview = new CourseOverview(course.CourseId);
		await Navigation.PushAsync(courseOverview);
    }

    private async void ModifyTerm_Clicked(object sender, EventArgs e)
    {
		var term = await DBHandler.GetTerm(TermId);
		AddEditTerm editTerm = new AddEditTerm(term.First(), true);
		Navigation.PushAsync(editTerm);

    }

    private void AddCourseButton_Clicked(object sender, EventArgs e)
    {
        AddEditCourse addCourse = new AddEditCourse(TermId);
        Navigation.PushAsync(addCourse);
    }
}