using System;
using AsteriskMenu.ViewsModels.OrderVM;
using Data.Entities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsteriskMenu.Views.OrderV
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataViewOrder : ContentPage
    {
        public DataViewOrder(Order order=null, Guid tableId = default)
        {
            InitializeComponent();
            BindingContext = new DataViewOrderVM(order, tableId);
        }
    }
}