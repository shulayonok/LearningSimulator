using System.ComponentModel;
using LearningSimulator.Models;
using LearningSimulator.Views;

namespace LearningSimulator.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private User user { get; set; }

        public UserViewModel()
        {
            user = new User(LoginPage.user);
        }

        public string Initials
        {
            get { return (user.Initials); }
            set { }
        }

        public string Name
        {
            get { return user.Name; }
            set
            {
                if (user.Name != value)
                {
                    user.Name = value;
                    if (!string.IsNullOrEmpty(user.Name) && !string.IsNullOrEmpty(user.Surname))
                    {
                        if (user.Initials != (user.Name.Substring(0, 1) + user.Surname.Substring(0, 1)))
                        {
                            user.Initials = (value.Substring(0, 1) + user.Surname.Substring(0, 1));
                            OnPropertyChanged("Initials");
                        }
                    }
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Surname
        {
            get { return user.Surname; }
            set
            {
                if (user.Surname != value)
                {
                    user.Surname = value;
                    if (!string.IsNullOrEmpty(user.Name) && !string.IsNullOrEmpty(user.Surname))
                    {
                        if (user.Initials != (user.Name.Substring(0, 1) + user.Surname.Substring(0, 1)))
                        {
                            user.Initials = (user.Name.Substring(0, 1) + value.Substring(0, 1));
                            OnPropertyChanged("Initials");
                        }
                    }
                    OnPropertyChanged("Surname");
                }
            }
        }

        public string Username
        {
            get { return user.Username; }
            set
            {
                if (user.Username != value)
                {
                    user.Username = value;
                    OnPropertyChanged("Username");
                }
            }
        }

        public string Email
        {
            get { return user.Email; }
            set
            {
                if (user.Email != value)
                {
                    user.Email = value;
                    OnPropertyChanged("Email");
                }
            }
        }

        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
