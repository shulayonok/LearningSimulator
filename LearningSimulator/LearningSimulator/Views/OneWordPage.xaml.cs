using LearningSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearningSimulator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class OneWordPage : ContentPage
    {
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
                List<RadioButton> radios = new List<RadioButton>() { noun, verb, adjective, adverb, participle, preposition };
                // Получаем id
                int id = Convert.ToInt32(value);
                // Получаем записку из бд по id
                Word word = await App.Database.GetWordAsync(id);
                RadioButton radio = radios.Where(r => Convert.ToByte(r.Value) == word.PartOfSpeech).First();
                radio.IsChecked = true;
                BindingContext = word;
            }
            catch { }
        }
        public OneWordPage()
        {
            InitializeComponent();
            BindingContext = new Word();
        }

        private async void OnSaveButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            List<RadioButton> radios = new List<RadioButton>() { noun, verb, adjective, adjective, participle, preposition };
            Word word = (Word)BindingContext;
            bool isEmpty = true;
            // Если изменения не только из пробелов, то сейвим
            if (!string.IsNullOrWhiteSpace(word.Meaning) && !string.IsNullOrWhiteSpace(word.Translation))
            {
                foreach (var radio in radios)
                {
                    if (radio.IsChecked)
                    {
                        word.PartOfSpeech = Convert.ToByte(radio.Value);
                        isEmpty = false;
                        break;
                    }
                }
                if (isEmpty)
                {
                    var options = new ToastOptions
                    {
                        BackgroundColor = Color.FromHex("#656491"),
                        MessageOptions = new MessageOptions
                        {
                            Foreground = Color.White,
                            Message = "You need to choose a part of speech"
                        },
                        Duration = TimeSpan.FromSeconds(4),
                    };
                    await this.DisplayToastAsync(options);
                }
                else
                {
                    // Возвращаемся к списку (делаем шаг назад двумя точками)
                    Task saveTask = App.Database.SaveWordAsync(LoginPage.user, word);
                    Task goTask = Shell.Current.GoToAsync($"..?needToLoad={true}", true);
                    await Task.WhenAll(saveTask, goTask);
                }
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
            Word word = (Word)BindingContext;
            int num = await App.Database.DeleteWordAsync(LoginPage.user, word);
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