using System;
using AsteriskMenu.ViewsModels.OrderVM;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsteriskMenu.Views.OrderV
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewOrders : ContentPage
    {
        public ListViewOrders(Guid restaurantId = default)
        {
            InitializeComponent();
            BindingContext = new ListViewOrdersVM(restaurantId);
        }
    }
}