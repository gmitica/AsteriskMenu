using System;
using AsteriskMenu.Views.Basic;
using AutoMapper;
using Data.Mappings;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace AsteriskMenu
{
    public partial class App : Application
    {
        public Guid ActiveRestaurantId { get; set; } = Guid.Empty;
        public App()
        {
            InitializeComponent();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            Current.Properties["Mapper"] = mapper;
            MainPage = new NavigationPage(new HomeView());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}