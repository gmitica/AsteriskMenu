using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;
using AsteriskMenu.Helpers;
using AsteriskMenu.Views.OrderV;
using AsteriskMenu.ViewsModels.Basic;
using Data.Entities;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AsteriskMenu.ViewsModels.OrderVM
{
    public class ListViewOrdersVM : ViewModelBase
    {
        private List<Order> _orders;
        private Order _selectedOrder;

        private bool _isWithoutOrders { get; set; } = false;
        
        public bool IsWithoutOrders
        {
            get => _isWithoutOrders;
            set
            {
                _isWithoutOrders = value;
                OnPropertyChanged();
            }
        }

        private bool _isVisibleCreate { get; set; } = true;
        
        public bool IsVisibleCreate
        {
            get => _isVisibleCreate;
            set
            {
                _isVisibleCreate = value;
                OnPropertyChanged();
            }
        }

        public ICommand ButtonAdd => new Command(() =>
        {
            App.Current.MainPage.Navigation.PushAsync(new DataViewOrder());
        });

        public List<Order> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged();
            }
        }

        public Order SelectedOrder 
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                if (_selectedOrder!=null)
                {
                    OnClickRow(_selectedOrder);
                }

                OnPropertyChanged();
            }
        }
        

        public ListViewOrdersVM(Guid restaurantId = default)
        {
            Orders = new List<Order>();
            InitializeAsync(restaurantId);
        }

        public async void InitializeAsync(Guid restaurantId = default)
        {
            WebApiResquest webApi;
            if (restaurantId != default && restaurantId!=Guid.Empty)
            {
                IsVisibleCreate = true;
                webApi = new WebApiResquest($"/Api/Order/GetAll/{restaurantId}");
            }else
            {
                IsVisibleCreate = false;
                webApi = new WebApiResquest($"/Api/Order/GetAll");
            }
            HttpResponseMessage response = await webApi.Request();
            string result = await response.Content.ReadAsStringAsync();
            Orders = JsonConvert.DeserializeObject<List<Order>>(result);
            if (Orders.Count == 0)
            {
                IsWithoutOrders = true;
            }
            else
            {
                IsWithoutOrders = false;
            }
        }
        private void OnClickRow(Order order)
        {
            ((App) App.Current).ActiveRestaurantId = order.Table.RestaurantId;
            App.Current.MainPage.Navigation.PushAsync(new DataViewOrder(order));
        }
        
     
       
        public ICommand DeleteCommand => new Command<Order>(async (Order item) =>
        {
            bool confirmation = await App.Current.MainPage.DisplayAlert("Eliminar", "¿Confirmar eliminación?", "Si", "No");
            if (confirmation)
            {
                WebApiResquest webApi = new WebApiResquest($"/Api/Order/Delete/{item.Id}", "DELETE");
                HttpResponseMessage response = await webApi.Request();
                if (response.IsSuccessStatusCode)
                {
                    Orders = Orders.Where(x=>x.Id != item.Id).ToList();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar la orden", "Ok");
                }
            }
        });


    }
}