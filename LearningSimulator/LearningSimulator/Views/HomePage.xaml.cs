using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace LearningSimulator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(Alert), "alert")]
    [QueryProperty(nameof(Sound), "sound")]
    public partial class HomePage : ContentPage
    {
        public static bool needChange = false;
        readonly Notificator notificator = new Notificator();
        byte mode = 0;
        public HomePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            StackLayout[] stacks = new StackLayout[] { wordsTest, pvTest, wordsBlitz, pvBlitz };
            Task[] tasks = new Task[stacks.Length];
            byte i = 0;
            stacks.ForEach(stack => { tasks[i] = Animation(stack, 1000); i++; });
            await Task.WhenAll(tasks);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StackLayout[] stacks = new StackLayout[] { wordsTest, pvTest, wordsBlitz, pvBlitz };
            stacks.ForEach(stack => { stack.Opacity = 0; stack.Scale = 0.5; });
            confirm.IsVisible = false;
            confirm.Opacity = 0;
            confirm.Scale = 0.5;
        }

        private async Task Animation(StackLayout stack, uint length)
        {
            Task fadeTask = stack.FadeTo(1, length, Easing.CubicInOut);
            Task scaleTask = stack.ScaleTo(1, length, Easing.SpringOut);
            await Task.WhenAll(fadeTask, scaleTask);
        }

        private async Task Fading(StackLayout stack)
        {
            Task fadeTask = stack.FadeTo(0, 500, Easing.CubicInOut);
            Task scaleTask = stack.ScaleTo(0.5, 500, Easing.SpringOut);
            await Task.WhenAll(fadeTask, scaleTask);
        }

        private async void ConfirmTheAction()
        {
            StackLayout[] stacks = new StackLayout[] { wordsTest, pvTest, wordsBlitz, pvBlitz };
            Task[] tasks = new Task[4];
            byte i = 0;
            stacks.ForEach(stack => { tasks[i] = Fading(stack); i++; });
            await Task.WhenAll(tasks);
            confirm.IsVisible = true;
            await Animation(confirm, 500);
        }

        public bool Alert
        {
            set
            {
                GetAlert(value);
            }
        }

        /// <summary>
        /// Получим уведомление о смене пароля
        /// </summary>
        /// <param name="alert">Параметр, передаваемый при маршрутизации от страницы логина</param>
        async void GetAlert(bool alert)
        {
            if (alert)
            {
                await Task.Delay(500);
                notificator.DisplayToast("You need to change your password in your profile");
            }
        }

        public bool Sound
        {
            set
            {
                GetSound(value);
            }
        }
        async void GetSound(bool sound)
        {
            if (sound)
            {
                await Task.Delay(500);
                DependencyService.Get<IAudio>().PlayAudioFile("salam.flac");
            }
        }
        protected override bool OnBackButtonPressed()
        {
            // By returning TRUE and not calling base we cancel the hardware back button :)
            // base.OnBackButtonPressed();
            return true;
        }

        private void WTImageButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click.mp3");
            mode = 1;
            ConfirmTheAction();
        }

        private void PVBImageButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click.mp3");
            mode = 4;
            ConfirmTheAction();
        }

        private void WBImageButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click.mp3");
            mode = 2;
            ConfirmTheAction();
        }

        private void PVTImageButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click.mp3");
            mode = 3;
            ConfirmTheAction();
        }

        private async void Yes_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            switch (mode)
            {
                case 1:
                    await Shell.Current.GoToAsync($"testPage?progress={false}", true); 
                    break;
                case 2:
                    await Shell.Current.GoToAsync($"testPage?progress={true}", true);
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }

        private async void No_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            await Fading(confirm);
            confirm.IsVisible = false;
            StackLayout[] stacks = new StackLayout[] { wordsTest, pvTest, wordsBlitz, pvBlitz };
            Task[] tasks = new Task[stacks.Length];
            byte i = 0;
            stacks.ForEach(stack => { tasks[i] = Animation(stack, 500); i++; });
            await Task.WhenAll(tasks);
        }
    }
}