using AsteriskMenu.ViewsModels;
using AsteriskMenu.ViewsModels.Basic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsteriskMenu.Views.Basic
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminHomeView : ContentPage
    {
        public AdminHomeView()
        {
            InitializeComponent();
            BindingContext = new AdminHomeViewModel();
        }
    }
}