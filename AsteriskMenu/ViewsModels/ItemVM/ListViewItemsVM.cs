using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;
using AsteriskMenu.Helpers;
using AsteriskMenu.Views.ItemV;
using AsteriskMenu.ViewsModels.Basic;
using Data.Entities;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AsteriskMenu.ViewsModels.ItemVM
{
    public class ListViewItemsVM : ViewModelBase
    {
        private List<Item> _items;
        private Item _selectedItem;

        public ICommand ButtonAdd => new Command(() =>
        {
            App.Current.MainPage.Navigation.PushAsync(new DataViewItem());
        });

        public List<Item> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }


        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                if (_selectedItem!=null)
                {
                    OnClickRow(_selectedItem);
                }

                OnPropertyChanged();
            }
        }

        

        public ListViewItemsVM()
        {
            Items = new List<Item>();
            InitializeAsync();
        }

        public async void InitializeAsync()
        {
            Guid restaurantId = ((App) App.Current).ActiveRestaurantId;
            WebApiResquest webApi = new WebApiResquest($"/Api/Item/GetAll/{restaurantId}");
            HttpResponseMessage response = await webApi.Request();
            string result = await response.Content.ReadAsStringAsync();
            Items = JsonConvert.DeserializeObject<List<Item>>(result);
        }
        private void OnClickRow(Item item)
        {
            App.Current.MainPage.Navigation.PushAsync(new DataViewItem(item));
        }
        
     
       
        public ICommand DeleteCommand => new Command<Item>(async (Item item) =>
        {
            bool confirmation = await App.Current.MainPage.DisplayAlert("Eliminar", "¿Confirmar eliminación?", "Si", "No");
            if (confirmation)
            {
                WebApiResquest webApi = new WebApiResquest($"/Api/Item/Delete/{item.Id}", "DELETE");
                HttpResponseMessage response = await webApi.Request();
                if (response.IsSuccessStatusCode)
                {
                    Items = Items.Where(x=>x.Id != item.Id).ToList();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar el artículo", "Ok");
                }
            }
        });


    }
}