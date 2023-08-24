using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Xamarin.Forms;

namespace LearningSimulator
{
    public partial class App : Application
    {
        public const string DATABASE_NAME = "DataBase_encrypted.db3";
        public static UsersAsyncRepository database;
        public static UsersAsyncRepository Database
        {
            get
            {
                
                if (database == null)
                {
                    database = new UsersAsyncRepository(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DATABASE_NAME));
                }
                return database;
            }
        }
        public App()
        {
            InitializeComponent();
            //MainPage = new NavigationPage(new LoginPage());
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
