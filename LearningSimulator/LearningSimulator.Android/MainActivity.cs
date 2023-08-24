using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;

namespace LearningSimulator.Droid
{
    [Activity(Label = "Learning Simulator", Icon = "@drawable/app_icon", Theme = "@style/MainTheme.Splash", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : FormsAppCompatActivity
    {
        static public readonly string testPath = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);

            int uiOptions = (int)Window.DecorView.SystemUiVisibility;
            uiOptions |= (int)SystemUiFlags.LowProfile;
            uiOptions |= (int)SystemUiFlags.Fullscreen;
            uiOptions |= (int)SystemUiFlags.HideNavigation;
            uiOptions |= (int)SystemUiFlags.Immersive;
            uiOptions |= (int)SystemUiFlags.ImmersiveSticky;
            uiOptions |= (int)SystemUiFlags.LayoutFullscreen;
            uiOptions |= (int)SystemUiFlags.LayoutHideNavigation;
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;

            //Window.AddFlags(WindowManagerFlags.Fullscreen);

            LoadApplication(new App());
        }
    }
}