using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using zBind.Mvvm.HomePage;
using zBind.Mvvm.TestPage;

namespace zBind
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new HomePage();
            MainPage = new TestPage();
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
