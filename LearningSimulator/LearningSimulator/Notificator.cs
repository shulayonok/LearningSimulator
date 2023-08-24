using System;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;
using Xamarin.CommunityToolkit.Extensions;

namespace LearningSimulator
{
    public class Notificator
    {
        public async void DisplayToast(string message, bool warning = true)
        {
            var options = new ToastOptions
            {
                BackgroundColor = Color.FromHex(warning ? "#656491": "#02315E"),
                MessageOptions = new MessageOptions
                {
                    Foreground = Color.White,
                    Message = message
                },
                Duration = TimeSpan.FromSeconds(4),
            };
            await Application.Current.MainPage.DisplayToastAsync(options);
        }
    }
}
