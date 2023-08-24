using LearningSimulator.Models;
using System;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearningSimulator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class OnePVPage : ContentPage
    {
        public OnePVPage()
        {
            InitializeComponent();
            BindingContext = new PhrasalVerb();
        }

        public string ItemId
        {
            set
            {
                // Подгрузка существующей в бд записи
                Load(value);
            }
        }
        private async void Load(string value)
        {
            try
            {
                // Получаем id
                int id = Convert.ToInt32(value);
                // Получаем записку из бд по id
                PhrasalVerb pv = await App.Database.GetPVAsync(id);
                BindingContext = pv;
            }
            catch { }
        }
        private async void OnSaveButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            PhrasalVerb pv = (PhrasalVerb)BindingContext;
            // Если изменения не только из пробелов, то сейвим
            if (!string.IsNullOrWhiteSpace(pv.Meaning) && !string.IsNullOrWhiteSpace(pv.Translation))
            {
                // Возвращаемся к списку (делаем шаг назад двумя точками)
                Task saveTask = App.Database.SavePVAsync(LoginPage.user, pv);
                Task goTask = Shell.Current.GoToAsync($"..?needToLoad={true}", true);
                await Task.WhenAll(saveTask, goTask);
            }
            else
            {
                var options = new ToastOptions
                {
                    BackgroundColor = Color.FromHex("#656491"),
                    MessageOptions = new MessageOptions
                    {
                        Foreground = Color.White,
                        Message = "All fields must be filled in"
                    },
                    Duration = TimeSpan.FromSeconds(4),
                };
                await this.DisplayToastAsync(options);
            }
        }

        private async void OnDeleteButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            PhrasalVerb pv = (PhrasalVerb)BindingContext;
            int num = await App.Database.DeletePVAsync(LoginPage.user, pv);
            if (num == 0)
            {
                var options = new ToastOptions
                {
                    BackgroundColor = Color.FromHex("#656491"),
                    MessageOptions = new MessageOptions
                    {
                        Foreground = Color.White,
                        Message = "There is no such word"
                    },
                    Duration = TimeSpan.FromSeconds(4),
                };
                await this.DisplayToastAsync(options);
            }
            else await Shell.Current.GoToAsync($"..?needToLoad={true}", true);
        }

        private async void OnBackButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            await Shell.Current.GoToAsync($"..?needToLoad={false}", true);
        }

        protected override bool OnBackButtonPressed()
        {
            // By returning TRUE and not calling base we cancel the hardware back button :)
            // base.OnBackButtonPressed();
            return true;
        }
    }
}