using AsteriskMenu.ViewsModels.RestaurantVM;
using Data.Entities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsteriskMenu.Views.RestaurantV
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataViewRestaurant : ContentPage
    {
        public DataViewRestaurant(Restaurant restaurant=null)
        {
            InitializeComponent();
            BindingContext = new DataViewRestauranVM(restaurant);
        }
    }
}