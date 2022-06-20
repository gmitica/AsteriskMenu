using AsteriskMenu.ViewsModels.RestaurantVM;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsteriskMenu.Views.RestaurantV
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewRestaurants : ContentPage
    {
        public ListViewRestaurants()
        {
            InitializeComponent();
            BindingContext = new ListViewRestaurantsVM();
        }
    }
}