using System;
using System.Collections.Generic;
using LearningSimulator.Models;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace LearningSimulator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        readonly Notificator notificator = new Notificator();
        readonly Сryptographer сryptographer = new Сryptographer();
        public RegisterPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            ClearFields();
            base.OnAppearing();
        }


        public void ClearFields()
        {
            Entry[] entries = new Entry[] { name, surname, username, password, answer, email };
            entries.ForEach(entry => { entry.Text = null; });
        }

        private async void Register_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            // Используем шифровальщика для проверки уникальности пользователя
            Entry[] entries = new Entry[] { name, surname, username, password, answer };

            // проверяем, что все поля окрасились в белый
            entries.ForEach(entry =>
            {
                if (string.IsNullOrEmpty(entry.Text) || !(entry.TextColor.R == 1 && entry.TextColor.G == 1 && entry.TextColor.B == 1))
                {
                    notificator.DisplayToast("One or more fields are incorrect or null");
                    return;
                }
            });

            if (await сryptographer.IsRegistered(username.Text))
            {
                var options = new ToastOptions
                {
                    BackgroundColor = Color.FromHex("#656491"),
                    MessageOptions = new MessageOptions
                    {
                        Foreground = Color.White,
                        Message = "A user with this username already exists"
                    },
                    Duration = TimeSpan.FromSeconds(4),
                };
                await this.DisplayToastAsync(options);
            }
            else
            {
                string hashString = сryptographer.ToHash(password.Text);
                WordsReader wordsReader = new WordsReader();
                LoginPage.user = new User() { Name = name.Text, Password = hashString, Surname = surname.Text, Username = username.Text, Email = email.Text, Answer = answer.StyleId == "date" ? wordsReader.ConvertAnswer(answer.Text): answer.Text, Initials = (name.Text.Substring(0,1) + surname.Text.Substring(0,1)) };
                await App.Database.SaveUserAsync(LoginPage.user);

                await Shell.Current.GoToAsync($"///home?alert={false}&sound={true}", true);
            }
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            answer.StyleId = picker.Items[picker.SelectedIndex] == "Graduation date" ? "date" : "other";
        }

        private async void BackImageButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            await Shell.Current.GoToAsync($"..?isDeleted={false}", true);
        }

        protected override bool OnBackButtonPressed()
        {
            // By returning TRUE and not calling base we cancel the hardware back button :)
            // base.OnBackButtonPressed();
            return true;
        }
    }
}