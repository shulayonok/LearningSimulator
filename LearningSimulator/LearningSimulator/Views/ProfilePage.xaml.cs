using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LearningSimulator.ViewModels;
using System;
using System.Text.RegularExpressions;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace LearningSimulator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        UserViewModel viewModel;
        readonly Notificator notificator = new Notificator();
        readonly Сryptographer сryptographer = new Сryptographer();
        public ProfilePage()
        {
            InitializeComponent();
            viewModel = new UserViewModel();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            StackLayout[] stacks = new StackLayout[] { first, second, third, fourth, fifth, sixth, seventh };
            Task[] tasks = new Task[stacks.Length];
            byte i = 0;
            stacks.ForEach(stack => { tasks[i] = AppearAnimation(stack); i++; });
            await Task.WhenAll(tasks);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StackLayout[] stacks = new StackLayout[] { first, second, third, fourth, fifth, sixth, seventh };
            stacks.ForEach(stack => { stack.Opacity = 0; stack.Scale = 0.5; });
        }

        private async Task AppearAnimation(StackLayout stack)
        {
            Task fadeTask = stack.FadeTo(1, 1000, Easing.CubicInOut);
            Task scaleTask = stack.ScaleTo(1, 1000, Easing.SpringOut);
            await Task.WhenAll(fadeTask, scaleTask);
        }

        private async Task Animation(Entry entry, bool isReverse)
        {
            entry.IsReadOnly = !isReverse;
            Task fadeTask = entry.FadeTo(isReverse ? 0.7: 1, 200, isReverse ? Easing.CubicIn: Easing.CubicInOut);
            Task scaleTask = entry.ScaleTo(isReverse ? 0.9 : 1, 200, Easing.Linear);
            await Task.WhenAll(fadeTask, scaleTask);
        }

        private async void DoAllAnimations(Entry[] entries, bool isReverse = false)
        {
            edit.IsEnabled = !isReverse;
            save.IsEnabled = isReverse;
            cancel.IsEnabled = isReverse;
            Task[] tasks = new Task[entries.Length];
            byte i = 0;
            entries.ForEach(entry => 
            {
                tasks[i] = Animation(entry, isReverse);
                i++;
            });
            await Task.WhenAll(tasks);
            Task fadeTask = cancel.FadeTo(isReverse ? 1: 0, 300, isReverse ? Easing.CubicIn: Easing.CubicOut);
            Task scaleTask = cancel.ScaleTo(isReverse ? 1: 0.8, 300, isReverse ? Easing.BounceIn: Easing.BounceOut);
            await Task.WhenAll(fadeTask, scaleTask);
        }

        /// <summary>
        /// Даём возможность пользователю изменить личные данные
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            Entry[] entries = new Entry[] { name, surname, username, email };
            Shell.SetTabBarIsVisible(this, false);
            DoAllAnimations(entries, true);
        }

        /// <summary>
        /// Сохраняем изменения в личных данных юзера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            Entry[] entries = new Entry[] { name, surname, username, email };
            bool isCorrect = true;
            // проверяем, что все поля окрасились в белый
            entries.ForEach(entry =>
            {
                if (string.IsNullOrEmpty(entry.Text) || !(entry.TextColor.R == 1 && entry.TextColor.G == 1 && entry.TextColor.B == 1)) isCorrect = false;
            });
            if (isCorrect)
            {
                DoAllAnimations(entries);
                LoginPage.user.Name = viewModel.Name;
                LoginPage.user.Surname = viewModel.Surname;
                LoginPage.user.Username = viewModel.Username;
                LoginPage.user.Email = viewModel.Email;
                LoginPage.user.Initials = viewModel.Initials;
                await App.Database.SaveUserAsync(LoginPage.user);
                Shell.SetTabBarIsVisible(this, true);
            }
            else notificator.DisplayToast("One or more fields are incorrect or null");
        }

        private async void ChangingPassword()
        {
            string password = await DisplayPromptAsync("Changing the password", "Enter your new password");
            if (!(string.IsNullOrEmpty(password) || password == "Cancel"))
            {
                if (Regex.IsMatch(password, EntryValidation.password))
                {
                    string hashString = сryptographer.ToHash(password);
                    LoginPage.user.Password = hashString;
                    await App.Database.SaveUserAsync(LoginPage.user);
                    HomePage.needChange = false;
                    notificator.DisplayToast("The password has been successfully changed!", false);
                }
                else notificator.DisplayToast("Your password must contain numbers, uppercase and lowercase Latin characters ( min. 9 characters | max. 30 characters)");
            }
        }

        /// <summary>
        /// Меняем пароль юзеру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ChangePasswordButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            if (HomePage.needChange) ChangingPassword();
            else
            {
                string password = await DisplayPromptAsync("Changing the password", "Enter your old password");
                if (!(string.IsNullOrEmpty(password) || password == "Cancel"))
                {
                    // Используем шифровальщика для процедуры проверки правильности ввода
                    if (await сryptographer.IsAuthorised(username.Text, password)) ChangingPassword();
                    else notificator.DisplayToast("Incorrect password!");
                }
            }
        }

        /// <summary>
        /// Удаляем профиль юзера (перекидываем его на стартовую страницу и там удаляем)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            var deleteAction = new SnackBarActionOptions
            {
                Action = () => Shell.Current.GoToAsync($"login?isDeleted={true}", true),
                Text = "Yep",
                ForegroundColor = Color.White
            };
            var options = new SnackBarOptions
            {
                BackgroundColor = Color.FromHex("#02315E"),
                MessageOptions = new MessageOptions
                {
                    Foreground = Color.White,
                    Message = "Are you sure?"
                },
                Duration = TimeSpan.FromSeconds(1.5),
                Actions = new[] {deleteAction} 

            };
            await this.DisplaySnackBarAsync(options);
        }

        /// <summary>
        /// Отменяем изменения, сделанные пользователем в личных данных 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            Entry[] entries = new Entry[] { name, surname, username, email };

            viewModel.Name = LoginPage.user.Name;
            viewModel.Surname = LoginPage.user.Surname;
            viewModel.Username = LoginPage.user.Username;
            viewModel.Email = LoginPage.user.Email;
            viewModel.Initials = LoginPage.user.Initials;

            entries.ForEach(entry => { entry.TextColor = Color.White; });

            DoAllAnimations(entries);
            Shell.SetTabBarIsVisible(this, true);
        }

        protected override bool OnBackButtonPressed()
        {
            // By returning TRUE and not calling base we cancel the hardware back button :)
            // base.OnBackButtonPressed();
            return true;
        }
    }
}