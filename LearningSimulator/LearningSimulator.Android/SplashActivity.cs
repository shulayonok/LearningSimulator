using Android.App;
using Android.Content;
using Android.OS;
using System.Threading.Tasks;

namespace LearningSimulator.Droid
{
    [Activity(Label = "Learning Simulator", MainLauncher = true, Theme = "@style/MainTheme.Splash", NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            SetContentView(Resource.Layout.splash);
            Task startWork = new Task(() => { SimulateStart(); });
            startWork.Start();
        }

        private async void SimulateStart()
        {
            await Task.Delay(2300);
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}