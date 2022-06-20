using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using AsteriskMenu.Helpers;
using AsteriskMenu.Views.CategoryV;
using AsteriskMenu.Views.TableV;
using AsteriskMenu.ViewsModels.Basic;
using Data.Entities;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AsteriskMenu.ViewsModels.CategoryVM
{
    public class ListViewCategoriesVM : ViewModelBase
    {
        private List<Category> _categories;
        private Category _selectedCategory;

        public ICommand ButtonAdd => new Command(() =>
        {
            App.Current.MainPage.Navigation.PushAsync(new DataViewCategory());
        });

        public List<Category> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                if (_selectedCategory!=null)
                {
                    OnClickRow(_selectedCategory);
                }

                OnPropertyChanged();
            }
        }
        

        public ListViewCategoriesVM()
        {
            Categories = new List<Category>();
            InitializeAsync();
        }

        public async void InitializeAsync()
        {
            Guid restaurantId = ((App) App.Current).ActiveRestaurantId;
            WebApiResquest webApi = new WebApiResquest($"/Api/Category/GetAll/{restaurantId}");
            HttpResponseMessage response = await webApi.Request();
            string result = await response.Content.ReadAsStringAsync();
            Categories = JsonConvert.DeserializeObject<List<Category>>(result);
        }
        private void OnClickRow(Category category)
        {
            App.Current.MainPage.Navigation.PushAsync(new DataViewCategory(category));
        }
        
     
       
        public ICommand DeleteCommand => new Command<Category>(async (Category item) =>
        {
            bool confirmation = await App.Current.MainPage.DisplayAlert("Eliminar", "¿Confirmar eliminación?", "Si", "No");
            if (confirmation)
            {
                WebApiResquest webApi = new WebApiResquest($"/Api/Category/Delete/{item.Id}", "DELETE");
                HttpResponseMessage response = await webApi.Request();
                if (response.IsSuccessStatusCode)
                {
                    Categories = Categories.Where(x=>x.Id != item.Id).ToList();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar la categoría", "Ok");
                }
            }
        });


    }
}