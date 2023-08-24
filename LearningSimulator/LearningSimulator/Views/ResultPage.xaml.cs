using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace LearningSimulator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(Answers), "answers")]
    [QueryProperty(nameof(Test), "test")]
    public partial class ResultPage : ContentPage
    {
        byte correct;
        byte total;
        public ResultPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (correct < 15)
                label.TextColor = Color.Red;
            else if (correct < 20 && correct >= 15)
                label.TextColor = Color.Orange;
            else if (correct < 25 && correct >= 20)
                label.TextColor = Color.Yellow;
            else
                label.TextColor = Color.Green;
            label.Text = string.Format("{0}/{1} correct answers", correct, total);
        }

        public bool Test
        {
            set
            {
                GetTest(value);
            }
        }
        void GetTest(bool test)
        {
            total = (byte)(test ? 30 : 10);
        }

        public byte Answers
        {
            set
            {
                GetAnswers(value);
            }
        }
        void GetAnswers(byte answers)
        {
            correct = answers;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            await Shell.Current.GoToAsync($"///home?alert={false}&sound={false}", true);
        }

        protected override bool OnBackButtonPressed()
        {
            // By returning TRUE and not calling base we cancel the hardware back button :)
            // base.OnBackButtonPressed();
            return true;
        }
    }
}