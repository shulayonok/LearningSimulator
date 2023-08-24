using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LearningSimulator.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace LearningSimulator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(DeleteUser), "isDeleted")]
    public partial class LoginPage : ContentPage
    {
        readonly Notificator notificator = new Notificator();
        readonly Сryptographer сryptographer = new Сryptographer();
        public static User user;
        public LoginPage()
        {
            InitializeComponent();
        }

        public bool DeleteUser
        {
            set
            {
                Delete(value);
            }
        }
        /// <summary>
        /// Удаляет юзера и возвращает на стартовую страницу
        /// </summary>
        /// <param name="isDeleted">Параметр, передаваемый при маршрутизации от страницы профиля</param>
        private async void Delete(bool isDeleted)
        {
            if (isDeleted)
            {
                await App.Database.DeleteUserAsync(user);
                notificator.DisplayToast("The user has been successfully deleted", false);
            }
        }

        protected override async void OnAppearing()
        {
            ClearFields();
            // создание таблиц, если их нет
            Task userTable = App.Database.CreateUserTable();
            Task wordTable =  App.Database.CreateWordTable();
            Task interTable = App.Database.CreateIntermediateTable();
            Task pvTable = App.Database.CreatePVTable();
            Task interpvTable = App.Database.CreateIntermediatePVTable();
            await Task.WhenAll(userTable, wordTable, pvTable, interTable, interpvTable);

            base.OnAppearing();
        }

        /// <summary>
        /// Зачищает все поля при переходе на домашнюю страницу
        /// </summary>
        public void ClearFields()
        {
            Entry[] entries = new Entry[] { username, password };
            entries.ForEach(entry => { entry.Text = null; });
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            // Используем шифровальщика для процедуры идентификации и аутентификации
            if (await сryptographer.IsAuthorised(username.Text, password.Text))
            {
                // Если успешно, то переходим на основную страницу
                user = await App.Database.FindUserAsync(username.Text);
                await Shell.Current.GoToAsync($"///home?alert={false}&sound={true}", true);               
            }
            else
            {
                notificator.DisplayToast("Username or password is incorrect or empty");
                if (regain.Opacity == 0)
                {
                    regain.Animate("RegainAnimation", new Animation((value) => { regain.Opacity = 0 + value; }),
                    length: 600,
                    easing: Easing.Linear);
                }
            }
        }

        private async void Register_Tapped(object sender, EventArgs e) => await Shell.Current.GoToAsync("registration", true);

        private async void Regain_Tapped(object sender, EventArgs e)
        {
            var connect = Connectivity.NetworkAccess;
            if (connect == NetworkAccess.Internet)
            {
                string action = await DisplayActionSheet("Choose your control question", "Cancel", null, "First pet's name", "Graduation date", "Brand of the first car");
                if (!(string.IsNullOrEmpty(action) || action == "Cancel"))
                {
                    string answer = await DisplayPromptAsync(action, "Enter your answer");
                    if (!(string.IsNullOrEmpty(answer) || answer == "Cancel"))
                    {
                        // Используем шифровальщика для проверки корректности контрольного вопроса
                        WordsReader wordsReader = new WordsReader();
                        if (await сryptographer.IsAuthorised(username.Text, Regex.IsMatch(answer, EntryValidation.date) ? wordsReader.ConvertAnswer(answer) : answer, false))
                        {
                            user = await App.Database.FindUserAsync(username.Text);
                            // Если ответ на контрольный вопрос совпадает, то отправляем код доступа на почту
                            EmailService emailService = new EmailService();

                            Random random = new Random();
                            string code = random.Next(100000, 999999).ToString();

                            await emailService.SendEmailAsync(user.Email, "Regain access", code, "bewsgncammjunsru");
                            answer = await DisplayPromptAsync("Regain access", "Enter the code from the email", keyboard: Keyboard.Numeric);
                            if (answer == code)
                            {
                                HomePage.needChange = true;
                                await Shell.Current.GoToAsync($"///home?alert={HomePage.needChange}&sound={true}", true);
                            }
                            else notificator.DisplayToast("Wrong code");
                        }
                        else notificator.DisplayToast("Wrong answer or username");
                    }
                }
            }
            else notificator.DisplayToast("Bad connection... Try it later");
        }

        protected override bool OnBackButtonPressed()
        {
            // By returning TRUE and not calling base we cancel the hardware back button :)
            // base.OnBackButtonPressed();
            return true;
        }
    }
}