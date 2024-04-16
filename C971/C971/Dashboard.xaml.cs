using C971.Models;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace C971;

public partial class Dashboard : ContentPage
{

    public ObservableCollection<Course> Courses { get; set; }
    public ObservableCollection<Course> Terms { get; set; }
    public Dashboard()
    {   
        InitializeComponent();

    }

    private async void Modify_Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TermManagement());
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        //Ask to send notifications if they're not enabled
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

        //Get term info and assessment info for dashboard
            var terms = await DBHandler.GetTerms();
            if (terms.Count() == 0)
            {
            await DBHandler.GenerateSampleData();
            //CurrentTermLabel.Text = "No Terms";
            //return;
            terms = await DBHandler.GetTerms();
        }
            var firstTerm = terms.FirstOrDefault();

            CourseAndAssessment.AssignValues(firstTerm.Id);

            CourseCollectionView.ItemsSource = CourseAndAssessment.CourseAndAssessmentList;
            CurrentTermLabel.Text = firstTerm.TermName;
            TermCollectionView.ItemsSource = await DBHandler.GetTerms();
    }


    //Generate list of courses for the corresponding term that was clicked. Also change the header to match the clicked term.
    private void TermCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CourseAndAssessment.AssignValues((e.CurrentSelection[0] as Term).Id);
        Term currentTerm = e.CurrentSelection[0] as Term;
        CurrentTermLabel.Text = currentTerm.TermName;
    }


    //Get bound information from selected course and create / navigate to a new CourseOverview instance
    private async void CourseCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CourseAndAssessment selectedCourse = (e.CurrentSelection[0]) as CourseAndAssessment;
        if(selectedCourse != null)
        {
            var course = await DBHandler.GetCourse(selectedCourse.CourseId);
            CourseOverview courseOverview = new CourseOverview(course.First().CourseID);
            await Navigation.PushAsync(courseOverview);
        }

    }
}