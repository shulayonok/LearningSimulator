using Android.App;
using LearningSimulator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using static Android.Provider.UserDictionary;

namespace LearningSimulator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(needToLoad), "needToLoad")]
    public partial class PhrasalVerbsPage : ContentPage
    {
        public ObservableCollection<PhrasalVerb> phrasalVerbs = new ObservableCollection<PhrasalVerb>();
        public List<PhrasalVerb> sourceItems;
        readonly Notificator notificator = new Notificator();
        public PhrasalVerbsPage()
        {
            InitializeComponent();
        }

        public bool needToLoad
        {
            set
            {
                GetLoad(value);
            }
        }

        async void GetLoad(bool need)
        {
            if (need)
            {
                sourceItems = await App.Database.GetUserPVAsync(LoginPage.user);
                phrasalVerbs = new ObservableCollection<PhrasalVerb>(sourceItems);
                collectionView.ItemsSource = phrasalVerbs;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            search.Text = string.Empty;
        }

        private async void AddDefaultButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            refresh.IsVisible = false;
            indicator.IsVisible = true;
            indicator.IsRunning = true;
            WordsReader reader = new WordsReader();
            bool result = await reader.ReadVerbs(LoginPage.user);
            string message = result ? "The words have been successfully added" : "Something went wrong...";
            sourceItems = await App.Database.GetUserPVAsync(LoginPage.user);
            phrasalVerbs = new ObservableCollection<PhrasalVerb>(sourceItems);
            collectionView.ItemsSource = phrasalVerbs;
            indicator.IsVisible = false;
            indicator.IsRunning = false;
            refresh.IsVisible = true;
            notificator.DisplayToast(message, !result);
        }

        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            sourceItems = await App.Database.GetUserPVAsync(LoginPage.user);
            phrasalVerbs = new ObservableCollection<PhrasalVerb>(sourceItems);
            collectionView.ItemsSource = phrasalVerbs;
            DependencyService.Get<IAudio>().PlayAudioFile("refresh.wav");
            refresh.IsRefreshing = false;
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                DependencyService.Get<IAudio>().PlayAudioFile("press.wav");
                // Приводим к word текущее выделение
                PhrasalVerb pv = (PhrasalVerb)e.CurrentSelection.FirstOrDefault();
                await Shell.Current.GoToAsync($"onePV?{nameof(OnePVPage.ItemId)}={pv.ID}");
            }
        }

        private async void AddNewButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            await Shell.Current.GoToAsync("onePV", true);
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchItem = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(searchItem)) searchItem = string.Empty;

            searchItem = searchItem.Trim().ToLowerInvariant();

            var filteredItems = sourceItems.Where(i =>
                i.Meaning.ToLowerInvariant().Contains(searchItem) ||
                i.Translation.ToLowerInvariant().Contains(searchItem)).ToList();

            sourceItems.ForEach(item =>
            {
                if (!filteredItems.Contains(item)) phrasalVerbs.Remove(item);
                else if (!phrasalVerbs.Contains(item)) phrasalVerbs.Add(item);
            });
        }

        public async Task DeleteWords()
        {
            refresh.IsVisible = false;
            indicator.IsVisible = true;
            indicator.IsRunning = true;
            phrasalVerbs.ForEach(async item =>
            {
                await App.Database.DeletePVAsync(LoginPage.user, item);
            });
            await Task.Delay(200);
        }


        private async void DeleteAllButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            var deleteAction = new SnackBarActionOptions
            {
                Action = DeleteWords,
                Text = "Yep",
                ForegroundColor = Color.White,
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
                Actions = new[] { deleteAction }
            };
            bool res = await this.DisplaySnackBarAsync(options);
            if (res)
            {
                sourceItems = await App.Database.GetUserPVAsync(LoginPage.user);
                phrasalVerbs = new ObservableCollection<PhrasalVerb>(sourceItems);
                collectionView.ItemsSource = phrasalVerbs;
                indicator.IsVisible = false;
                indicator.IsRunning = false;
                refresh.IsVisible = true;
                notificator.DisplayToast("All phrasal verbs have been deleted...", false);
            }
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("click2.wav");
            await Shell.Current.GoToAsync("..", true);
        }

        protected override bool OnBackButtonPressed()
        {
            // By returning TRUE and not calling base we cancel the hardware back button :)
            // base.OnBackButtonPressed();
            return true;
        }
    }
}