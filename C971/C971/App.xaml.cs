using C971.Models;

namespace C971
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Dashboard());
        }

        protected override void OnStart()
        {

            base.OnStart();
        }
    }
}
