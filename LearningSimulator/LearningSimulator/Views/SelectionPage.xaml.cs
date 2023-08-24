using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace LearningSimulator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectionPage : ContentPage
    {
        public SelectionPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {;
            base.OnAppearing();
            StackLayout[] stacks = new StackLayout[] { words, pv };
            Task[] tasks = new Task[stacks.Length];
            byte i = 0;
            stacks.ForEach(stack => { tasks[i] = Animation(stack); i++; });
            await Task.WhenAll(tasks);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StackLayout[] stacks = new StackLayout[] { words, pv};
            stacks.ForEach(stack => { stack.Opacity = 0; stack.Scale = 0.5; });
        }

        private async Task Animation(StackLayout stack)
        {
            Task fadeTask = stack.FadeTo(1, 1000,  Easing.CubicInOut);
            Task scaleTask = stack.ScaleTo(1, 1000, Easing.SpringOut);
            await Task.WhenAll(fadeTask, scaleTask);
        }

        private async void WordsImageButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click.mp3");
            await Shell.Current.GoToAsync($"words?needToLoad={true}", true);
        }

        private async void PVImageButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("press.wav");
            await Shell.Current.GoToAsync($"phrasalVerbs?needToLoad={true}", true);
        }

        protected override bool OnBackButtonPressed()
        {
            // By returning TRUE and not calling base we cancel the hardware back button :)
            // base.OnBackButtonPressed();
            return true;
        }
    }
}