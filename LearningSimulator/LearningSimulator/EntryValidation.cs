using Xamarin.Forms;
using System.Text.RegularExpressions;
using System;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.CommunityToolkit.Extensions;

namespace LearningSimulator
{
    /// <summary>
    /// Класс для отслеживания правильно введённых данных пользователем
    /// </summary>
    public class EntryValidation : TriggerAction<Entry>
    {
        public bool activate { get; set; }
        public static string password = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])[a-zA-Z0-9]{8,30}$";
        public static string user = @"^[a-zA-Z][a-zA-Z0-9-_\.]{2,20}$";
        public static string name = @"^[A-Z]{1}[a-z]{1,20}$";
        public static string date = @"(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d";
        public static string email = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!# $%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9 ])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
        readonly Notificator notificator = new Notificator();


        protected override void Invoke(Entry sender)
        {
            if(!string.IsNullOrEmpty(sender.Text))
            {
                string pattern;
                string warning;
                switch (sender.StyleId)
                {
                    case "name":
                        pattern = name;
                        warning = "First and last names must start with a capital letter and be no shorter than 2 characters (max. 20 characters)";
                        break;
                    case "user":
                        pattern = user;
                        warning = "Your name must contain at least 3 characters, which can be letters and numbers, and start with a letter (max. 20 characters)";
                        break;
                    case "password":
                        pattern = password;
                        warning = "Your password must contain numbers, uppercase and lowercase Latin characters ( min. 9 characters | max. 30 characters)";
                        break;
                    case "date":
                        pattern = date;
                        warning = "Date in the format DD/MM/YYYY (DD-MM-YYYY or DD.MM.YYYY)";
                        break;
                    case "email":
                        pattern = email;
                        warning = "Incorrect email format";
                        break;
                    default:
                        pattern = "";
                        warning = "";
                        break;
                }
                if (activate)
                {
                    if (!Regex.IsMatch(sender.Text, pattern))
                    {
                        sender.Animate("EntryAnimation", new Animation((value) => { sender.TextColor = Color.FromRgb(0.83 - (0.17 * value), 0.07 + (0.59 * value), 0.13 + (0.53 * value)); }),
                        length: 800,
                        easing: Easing.Linear);
                    }
                    else
                    {
                        sender.Animate("EntryAnimation", new Animation((value) => { sender.TextColor = Color.FromRgb(1 - (0.34 * value), 1 - (0.34 * value), 1 - (0.34 * value)); }),
                        length: 800,
                        easing: Easing.Linear);
                    }
                }
                else
                {
                    if (Regex.IsMatch(sender.Text, pattern))
                    {
                        sender.Animate("EntryAnimation", new Animation((value) => { sender.TextColor = Color.FromRgb(0.66 + (0.34 * value), 0.66 + (0.34 * value), 0.66 + (0.34 * value)); }),
                        length: 800,
                        easing: Easing.Linear);
                    }
                    else 
                    {                  
                        sender.Animate("EntryAnimation", new Animation((value) => { sender.TextColor = Color.FromRgb(0.66 + (0.17 * value), 0.66 - (0.59 * value), 0.66 - (0.53 * value)); }),
                        length: 800,
                        easing: Easing.Linear);
                        notificator.DisplayToast(warning);
                    }
                }
            }
            else
            {
                if (activate) { sender.TextColor = Color.DarkGray; }
                else notificator.DisplayToast("This field is required");
            }
        }
    }
}
