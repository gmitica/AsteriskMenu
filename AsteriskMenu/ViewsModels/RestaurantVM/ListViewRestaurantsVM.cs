using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using AsteriskMenu.Helpers;
using AsteriskMenu.Views.RestaurantV;
using AsteriskMenu.ViewsModels.Basic;
using Data.Entities;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AsteriskMenu.ViewsModels.RestaurantVM
{
    public class ListViewRestaurantsVM : ViewModelBase
    {
        public ICommand ButtonRestaurantAdd =>  new Command( () =>
        {
            App.Current.MainPage.Navigation.PushAsync(new DataViewRestaurant());
        });
        private Restaurant _selectedRestaurant { get; set; }
        
        public ImageSource ContactImage { get; set; }

        public Restaurant SelectedRestaurant
        {
            get => _selectedRestaurant;
            set
            {
                _selectedRestaurant = value;
                if (_selectedRestaurant!= null)
                {
                    OnClickRow(_selectedRestaurant);
                }
                OnPropertyChanged();
            }
        }

        private void OnClickRow(Restaurant restaurant)
        {
            ((App)App.Current).ActiveRestaurantId = restaurant.Id;
            App.Current.MainPage.Navigation.PushAsync(new OptionListRestaurant(restaurant));
        }

        private List<Restaurant> _restaurants;

        public List<Restaurant> Restaurants
        {
            get { return _restaurants; }
            set
            {
                _restaurants = value;
                OnPropertyChanged();
            }
        }
        
        public ICommand DeleteCommand => new Command<Restaurant>(async (Restaurant item) =>
        {
            bool confirmation = await App.Current.MainPage.DisplayAlert("Eliminar", "¿Confirmar eliminación?", "Si", "No");
            if (confirmation)
            {
                WebApiResquest webApi = new WebApiResquest($"/Api/Restaurant/Delete/{item.Id}", "DELETE");
                HttpResponseMessage response = await webApi.Request();
                if (response.IsSuccessStatusCode)
                {
                    Restaurants = Restaurants.Where(x=>x.Id != item.Id).ToList();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar el restaurante", "Ok");
                }
            }
        });
        
        
        public ICommand ModifyCommand => new Command<Restaurant>(async (Restaurant item) =>
        {
            App.Current.MainPage.Navigation.PushAsync(new DataViewRestaurant(item));
        });


        private async void InitalizedAsync()
        {
            WebApiResquest webApi = new WebApiResquest(App.Current.Resources["RestaurantGetAll"].ToString());
            HttpResponseMessage response = await webApi.Request();
            string result = await response.Content.ReadAsStringAsync();
            Restaurants = JsonConvert.DeserializeObject<List<Restaurant>>(result);
        }

        public ListViewRestaurantsVM()
        {
            InitalizedAsync();    
        }
    }
}