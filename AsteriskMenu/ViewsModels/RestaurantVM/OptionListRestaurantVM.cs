using System;
using AsteriskMenu.Views.CategoryV;
using AsteriskMenu.Views.ItemV;
using AsteriskMenu.Views.OrderV;
using AsteriskMenu.Views.TableV;
using AsteriskMenu.Views.UserV;
using AsteriskMenu.ViewsModels.Basic;
using Data.Entities;
using Xamarin.Forms;

namespace AsteriskMenu.ViewsModels.RestaurantVM
{
    public class OptionListRestaurantVM : ViewModelBase
    {

        public Command ViewTables { get; set; }
        public Command ViewCategories { get; set; }
        public Command ViewItems { get; set; }
        public Command ViewUsers { get; set; }
        public Command ViewOrders { get; set; }
        private Restaurant _activeRestaurant;
        public OptionListRestaurantVM(Restaurant restaurant)
        {
            _activeRestaurant = restaurant;
            ViewTables = new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new ListViewTables());
            });
            ViewCategories =new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new ListViewCategories());
            });
            ViewItems =new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new ListViewItems());
            });
            ViewUsers =new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new ListViewUsers());
            });
            ViewOrders =new Command(async () =>
            {
                Guid restaurantId = ((App) App.Current).ActiveRestaurantId;
                await App.Current.MainPage.Navigation.PushAsync(new ListViewOrders(restaurantId));
            });
        }
    }
}