using System;
using System.Net.Http;
using System.Windows.Input;
using AsteriskMenu.Helpers;
using AsteriskMenu.ViewsModels.Basic;
using AutoMapper;
using Data.DTO.Category;
using Data.DTO.Restaurant;
using Data.DTO.Table;
using Data.Entities;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AsteriskMenu.ViewsModels.CategoryVM
{
    public class DataViewCategoryVM : ViewModelBase
    {

        private bool _controlIsCreated { get; set; } = false;
        private Category _category = new Category();
        
        public Category Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged();
            }
        }

        public Command ButtonCreate { get; set; }
        
        public string TextAddUpdate { get; set; } = "Crear categoría";


        public DataViewCategoryVM(Category category=null)
        {
            if (category!= null)
            {
                _category = category;
                TextAddUpdate = "Actualizar categoría";
                _controlIsCreated = true;
            }

            ButtonCreate = new Command(async () =>
            {
                WebApiResquest webApi;
                if (_controlIsCreated)
                {
                    
                    IMapper mapper = (IMapper)App.Current.Properties["Mapper"];
                    CategoryUpdateDTO categoryUpdate = mapper.Map<CategoryUpdateDTO>(_category);
                    webApi = new WebApiResquest("/Api/Category/Update", "PUT", categoryUpdate);
                   
                }
                else
                {
                    Guid restaurantId = ((App) App.Current).ActiveRestaurantId;
                    IMapper mapper = (IMapper)App.Current.Properties["Mapper"];
                    CategoryAddDTO categoryAdd = mapper.Map<CategoryAddDTO>(_category);
                    categoryAdd.RestaurantId = restaurantId;
                    webApi = new WebApiResquest("/Api/Category/Add", "POST", categoryAdd);
                }
                HttpResponseMessage response  = await webApi.Request();
                await Application.Current.MainPage.DisplayAlert("Correcto", "Operación finalizada con exito.", "Ok");
                Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
                await Application.Current.MainPage.Navigation.PopAsync();
            });
        }
    }
}