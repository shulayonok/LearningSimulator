using Android.App;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using Application = Xamarin.Forms.Application;

namespace LearningSimulator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(Prog), "progress")]
    public partial class TestPage : ContentPage
    {
        List<TestWord> words;
        byte num;
        byte correctAnswers;
        private Animation animation;
        public TestPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            num = 1;
            correctAnswers = 0;
            progress.Progress = 0;
            number.Text = string.Format("{0}/30", num.ToString());
            StartTest();
        }

        private async void StartTest()
        {
            TestHelper testHelper = new TestHelper();
            words = await testHelper.GetWords();
            await GoToNext();
        }

        private async Task GoToNext()
        {
            if (num <= 30)
            {
                Button[] buttons = new Button[] { one, two, three, four, five };
                Task[] tasks = new Task[buttons.Length + 1];
                byte i = 0;
                buttons.ForEach(button => { tasks[i] = NextAnimationButton(button, i); i++; });
                tasks[i] = NextAnimationLabel(label);
                await Task.WhenAll(tasks);
                number.Text = string.Format("{0}/30", num.ToString());
                if (progress.IsVisible)
                {
                    animation = new Animation(v =>
                    {
                        if (v == 0)
                        {
                            progress.Progress = 0;
                            return;
                        }
                        progress.Progress = (float)(v / 100);
                    }, 0, 100, Easing.SinInOut);
                    animation.Commit(progress, "progress", length:5000, finished:(l, c) =>
                    {
                        animation = null;
                    });
                    await Task.Delay(5000); // додумать (возможно cancelable token)
                    await Proverka(null);
                }
            }
            else await Shell.Current.GoToAsync($"results?answers={correctAnswers}&test={true}", true); 
        }

        private async Task NextAnimationLabel(Label label)
        {
            Task one = label.ScaleTo(0.8, 200, Easing.Linear);
            Task two = label.FadeTo(0.5, 200, Easing.Linear);
            await Task.WhenAll(one, two);
            label.Text = words[num - 1].isEn ? words[num - 1].meaning : words[num - 1].translation;
            Task three = label.ScaleTo(1, 200, Easing.Linear);
            Task four = label.FadeTo(1, 200, Easing.Linear);
            await Task.WhenAll(three, four);
        }

        private async Task NextAnimationButton(Button button, byte i)
        {
            Task one = button.ColorTo(Color.Transparent, 200, Easing.Linear);
            Task two = button.ScaleTo(0.8, 200, Easing.Linear);
            Task three = button.FadeTo(0.5, 200, Easing.Linear);
            await Task.WhenAll(one, two, three);
            button.Text = words[num - 1].isEn ? words[num - 1].variants[i].Translation: button.Text = words[num - 1].variants[i].Meaning;
            Task four = button.ScaleTo(1, 200, Easing.Linear);
            Task five = button.FadeTo(1, 200, Easing.Linear);
            await Task.WhenAll(four, five);
        }

        private async Task Proverka(Button button)
        {
            bool res = words[num - 1].isEn ? button?.Text == words[num - 1].translation : button?.Text == words[num - 1].meaning;
            if (res)
            {
                await button.ColorTo(Color.FromHex("#7DD872"), 300, Easing.Linear);
                correctAnswers++;
            }
            else
            {
                Button[] buttons = new Button[] { one, two, three, four, five };
                Button correct = words[num - 1].isEn ? buttons.Where(a => a.Text == words[num - 1].translation).First() : buttons.Where(a => a.Text == words[num - 1].meaning).First();
                Task red = button?.ColorTo(Color.FromHex("#B00000"));
                if (Equals(red, null)) await correct.ColorTo(Color.FromHex("#7DD872"), 300, Easing.Linear);
                else
                {
                    Task green = correct.ColorTo(Color.FromHex("#7DD872"), 300, Easing.Linear);
                    await Task.WhenAll(green, red);
                }
            }
            num++;
            await Task.Delay(100);
            await GoToNext();
        }

        private async void one_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            if (progress.IsVisible) progress.AbortAnimation("progress");
            await Proverka(one);
        }

        private async void two_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            if (progress.IsVisible) progress.AbortAnimation("progress");
            await Proverka(two);
        }

        private async void three_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            if (progress.IsVisible) progress.AbortAnimation("progress");
            await Proverka(three);
        }

        private async void four_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            if (progress.IsVisible) progress.AbortAnimation("progress");
            await Proverka(four);
        }

        private async void five_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            if (progress.IsVisible) progress.AbortAnimation("progress");
            await Proverka(five);
        }

        private async void cancel_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            var deleteAction = new SnackBarActionOptions
            {
                Action = () => Shell.Current.GoToAsync($"..?alert={false}&sound={false}", true),  
                Text = "Yep",
                ForegroundColor = Color.White
            };
            var options = new SnackBarOptions
            {
                BackgroundColor = Color.FromHex("#02315E"),
                MessageOptions = new MessageOptions
                {
                    Foreground = Color.White,
                    Message = "Abort the test?"
                },
                Duration = TimeSpan.FromSeconds(1.5),
                Actions = new[] { deleteAction }

            };
            await Application.Current.MainPage.DisplaySnackBarAsync(options);
        }

        public bool Prog
        {
            set
            {
                GetProg(value);
            }
        }

        void GetProg(bool prog) => progress.IsVisible = prog;

        protected override bool OnBackButtonPressed()
        {
            // By returning TRUE and not calling base we cancel the hardware back button :)
            // base.OnBackButtonPressed();
            return true;
        }
    }
}