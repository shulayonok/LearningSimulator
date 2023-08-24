using LearningSimulator.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearningSimulator
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AppShell : Shell
	{
		public AppShell ()
		{
			InitializeComponent ();
			RegisterRoutes();
		}

        private void RegisterRoutes()
        {
			Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("registration", typeof(RegisterPage));
            Routing.RegisterRoute("oneWord", typeof(OneWordPage));
            Routing.RegisterRoute("onePV", typeof(OnePVPage));
            Routing.RegisterRoute("words", typeof(WordsPage));
            Routing.RegisterRoute("phrasalVerbs", typeof(PhrasalVerbsPage));
            Routing.RegisterRoute("testPage", typeof(TestPage));
            Routing.RegisterRoute("results", typeof(ResultPage));
        }
    }
}