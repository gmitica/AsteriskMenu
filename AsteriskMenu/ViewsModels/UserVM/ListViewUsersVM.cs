using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;
using AsteriskMenu.Helpers;
using AsteriskMenu.Views.TableV;
using AsteriskMenu.Views.UserV;
using AsteriskMenu.ViewsModels.Basic;
using Data.Entities;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AsteriskMenu.ViewsModels.UserVM
{
    public class ListViewUsersVM : ViewModelBase
    {
        private List<UserRestaurant> _userRestaurants;
        private UserRestaurant _selectedUserRestaurant;

        public ICommand ButtonAdd => new Command(() =>
        {
            App.Current.MainPage.Navigation.PushAsync(new DataViewUser());
        });

        public List<UserRestaurant> UserRestaurants
        {
            get => _userRestaurants;
            set
            {
                _userRestaurants = value;
                OnPropertyChanged();
            }
        }

        public UserRestaurant SelectedUserRestaurant
        {
            get => _selectedUserRestaurant;
            set
            {
                _selectedUserRestaurant = value;
                if (_selectedUserRestaurant!=null)
                {
                    OnClickRow(_selectedUserRestaurant);
                }

                OnPropertyChanged();
            }
        }
        

        public ListViewUsersVM()
        {
            UserRestaurants = new List<UserRestaurant>();
            InitializeAsync();
        }

        public async void InitializeAsync()
        {
            Guid restaurantId = ((App) App.Current).ActiveRestaurantId;
            WebApiResquest webApi = new WebApiResquest($"/Api/UserRestaurants/GetAll/{restaurantId}");
            HttpResponseMessage response = await webApi.Request();
            string result = await response.Content.ReadAsStringAsync();
            UserRestaurants = JsonConvert.DeserializeObject<List<UserRestaurant>>(result);
        }
        private void OnClickRow(UserRestaurant userRestaurant)
        {
            App.Current.MainPage.Navigation.PushAsync(new DataViewUser(userRestaurant));
        }
        
     
       
        public ICommand DisableCommand => new Command<UserRestaurant>(async (UserRestaurant item) =>
        {
            bool confirmation = await App.Current.MainPage.DisplayAlert("Eliminar", "¿Confirmar eliminación?", "Si", "No");
            if (confirmation)
            {
                WebApiResquest webApi = new WebApiResquest($"/Api/UserRestaurants/Update", "PUT");
                HttpResponseMessage response = await webApi.Request();
                if (response.IsSuccessStatusCode)
                {
                    UserRestaurants = UserRestaurants.Where(x=>x.UserId != item.UserId && x.RestaurantId!=item.RestaurantId).ToList();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar el usuario", "Ok");
                }
            }
        });


    }
}