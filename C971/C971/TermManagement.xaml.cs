using C971.Models;
using System.Collections.ObjectModel;

namespace C971;

public partial class TermManagement : ContentPage
{
    public ObservableCollection<Term> Terms { get; set; } = new ObservableCollection<Term>();

	public TermManagement()
	{
		InitializeComponent();

    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        Terms.Clear();
        var termList = await DBHandler.GetTerms();
        foreach(var term in termList)
        {
            Terms.Add(term);
        }
        TermCollectionView.ItemsSource = Terms;
    }

    private async void EditButton_Clicked(object sender, EventArgs e)
    {
        ImageButton button = (ImageButton)sender;
        Term term = (Term)button.BindingContext;
        AddEditTerm termPage = new AddEditTerm(term, true);
        termPage.Title = "Edit Term";
        await Navigation.PushAsync(termPage);
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Delete?", "Are you sure you want to delete this term?", "Yes", "No");
        if (answer)
        {
            ImageButton deleteButton = (ImageButton)sender;
            Term term = (Term)deleteButton.BindingContext;

            var courses = await DBHandler.GetCourses(term.Id);

            foreach( var course in courses )
            {
                await DBHandler.RemoveCourse(course.CourseID);
            }
            await DBHandler.RemoveTerm(term.Id);
            Terms.Remove(term);
        }
    }

    private void TermCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var test = TermCollectionView.SelectedItem as Term;
        TermOverview termOverview = new TermOverview(test.Id);
        Navigation.PushAsync(termOverview);
    }

    private async void AddTermButton_Clicked(object sender, EventArgs e)
    {
        Term term = new Term("",DateTime.Now, DateTime.Now);
        await Navigation.PushAsync(new AddEditTerm(term, false));
    }
}