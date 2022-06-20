using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsteriskMenu.ViewsModels.RestaurantVM;
using Data.Entities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsteriskMenu.Views.RestaurantV
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OptionListRestaurant : ContentPage
    {
        public OptionListRestaurant(Restaurant restaurant)
        {
            InitializeComponent();
            BindingContext = new OptionListRestaurantVM(restaurant);
            
        }
    }
}