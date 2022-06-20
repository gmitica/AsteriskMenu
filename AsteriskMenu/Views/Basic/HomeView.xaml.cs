using AsteriskMenu.ViewsModels;
using AsteriskMenu.ViewsModels.Basic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsteriskMenu.Views.Basic
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeView : ContentPage
    {
        public HomeView()
        {
            InitializeComponent();
            BindingContext = new HomeViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = new HomeViewModel();
        }
    }
}