using System;
using System.Net.Http;
using System.Windows.Input;
using AsteriskMenu.Helpers;
using AsteriskMenu.ViewsModels.Basic;
using AutoMapper;
using Data.DTO.Restaurant;
using Data.DTO.Table;
using Data.DTO.UserRestaurants;
using Data.Entities;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AsteriskMenu.ViewsModels.UserVM
{
    public class DataViewUserVM : ViewModelBase
    {

        private bool _controlIsCreated { get; set; } = false;
        private User _user = new User();
        public string TextAddUpdate { get; set; } = "Crear usuario";
        
        public User User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        public Command ButtonCreate { get; set; }
        
        public DataViewUserVM(UserRestaurant userRestaurant=null)
        {
            if (userRestaurant!= null)
            {
                _user = userRestaurant.User;
                _controlIsCreated = true;
                TextAddUpdate = "Actualizar usuario";
            }

            ButtonCreate = new Command(async () =>
            {
                WebApiResquest webApi;
                UserRestaurantAddUpdateDTO ur = new UserRestaurantAddUpdateDTO();
                Guid restaurantId = ((App) App.Current).ActiveRestaurantId;
                ur.RestaurantId = restaurantId;
                ur.FirstName = _user.FirstName;
                ur.SurName = _user.LastName;
                ur.IsActive = _user.isActive;
                ur.Password = _user.Password;
                ur.Email = _user.Email;
                if (_user.Id != null)
                {
                    ur.UserId = _user.Id;
                }
                if (_controlIsCreated)
                {
                    webApi = new WebApiResquest("/Api/UserRestaurants/Update", "PUT", ur);
                }
                else
                {
                    webApi = new WebApiResquest("/Api/UserRestaurants/Add", "POST", ur);
                }
                HttpResponseMessage response =  await webApi.Request();
                await Application.Current.MainPage.DisplayAlert("Correcto", "Operación finalizada con exito.", "Ok");
                Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
                await Application.Current.MainPage.Navigation.PopAsync();
            });
        }
    }
}