using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;
using AsteriskMenu.Helpers;
using AsteriskMenu.Views.OrderV;
using AsteriskMenu.ViewsModels.Basic;
using AutoMapper;
using Data.DTO.Category;
using Data.DTO.Item;
using Data.DTO.Restaurant;
using Data.DTO.RestaurantItemCategory;
using Data.DTO.Table;
using Data.Entities;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace AsteriskMenu.ViewsModels.ItemVM
{
    public class DataViewItemVM : ViewModelBase
    {

        private bool _controlIsCreated { get; set; } = false;
        private Item _item = new Item();
        private ImageSource _image;
        private List<RestaurantItemCategory> _categoryList { get; set; } = new List<RestaurantItemCategory>();
        public List<RestaurantItemCategory> CategoryList
        {
            get => _categoryList;
            set
            {
                _categoryList = value;
                OnPropertyChanged();
            }
        }
        


        public Item Item
        {
            get => _item;
            set
            {
                _item = value;
                OnPropertyChanged();
            }
        }   

        public Command ButtonCreate { get; set; }
        
        public string TextAddUpdate { get; set; } = "Crear artículo";
        
        public ImageSource Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }
        

        public Command PickImageCommand { get; set; }




        public async void InitalizedAsync()
        {
            
            Guid restaurantId = ((App) App.Current).ActiveRestaurantId;
            WebApiResquest webApi = new WebApiResquest($"/Api/Category/GetAll/{restaurantId}");
            HttpResponseMessage response = await webApi.Request();
            string result = await response.Content.ReadAsStringAsync();
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(result);

            if (categories.Count<=0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No hay categorías disponibles, tiene que exisitr una categoría...", "Aceptar");
                await Application.Current.MainPage.Navigation.PopAsync();
            }


            WebApiResquest webApiRIC = new WebApiResquest($"/Api/RestaurantItemCategory/GetAll/{restaurantId}");
            HttpResponseMessage responseRIC = await webApiRIC.Request();
            string resultRIC = await responseRIC.Content.ReadAsStringAsync();
            List<RestaurantItemCategory> listRIC = JsonConvert.DeserializeObject<List<RestaurantItemCategory>>(resultRIC);
            
             
            foreach (Category category in categories)
            {
                _categoryList.Add(new RestaurantItemCategory()
                {
                    Category = category,
                    CategoryId = category.Id,
                    RestaurantId = restaurantId,
                    ItemId = Item.Id,
                    Id = listRIC.FirstOrDefault(x => x.RestaurantId==restaurantId && x.CategoryId==category.Id && x.ItemId==Item.Id)?.Id ?? Guid.NewGuid(),
                    Active = listRIC.FirstOrDefault(x => x.RestaurantId==restaurantId && x.CategoryId==category.Id && x.ItemId==Item.Id)?.Active ?? false
                });
            }
            CategoryList = new List<RestaurantItemCategory>(_categoryList);

           
        }
        
        
        public DataViewItemVM(Item item=null)
        {
            InitalizedAsync();
            IMapper mapper = (IMapper)App.Current.Properties["Mapper"];
            if (item!= null)
            {
                _item = item;
                TextAddUpdate = "Actualizar artículo";
                _controlIsCreated = true;
                Image = ImageSource.FromStream(() =>
                {
                    return new MemoryStream(_item.Image);
                });
            }

            ButtonCreate = new Command(async () =>
            {
                WebApiResquest webApi;
                if (_controlIsCreated)
                {
                    
                    ItemUpdateDTO itemUpdate = mapper.Map<ItemUpdateDTO>(_item);
                    webApi = new WebApiResquest("/Api/Item/Update", "PUT", itemUpdate);
                   
                }
                else
                {
                    Guid restaurantId = ((App) App.Current).ActiveRestaurantId;
                    IMapper mapper = (IMapper)App.Current.Properties["Mapper"];
                    ItemAddDTO itemAdd = mapper.Map<ItemAddDTO>(_item);
                    itemAdd.RestaurantId = restaurantId;
                    webApi = new WebApiResquest("/Api/Item/Add", "POST", itemAdd);
                }
                HttpResponseMessage response  = await webApi.Request();
                if (response.IsSuccessStatusCode)
                {
                    if (!_controlIsCreated)
                    {
                        CategoryList.Add(new RestaurantItemCategory()
                        {
                            RestaurantId = ((App)App.Current).ActiveRestaurantId,
                            ItemId = _item.Id,
                            Id = Guid.NewGuid(),
                            Active = true
                        });
                    }

                    foreach (RestaurantItemCategory ric in CategoryList)
                    {
                        RestaurantItemCategoryAddUpdateDTO ricAddUpdate = mapper.Map<RestaurantItemCategoryAddUpdateDTO>(ric);
                        WebApiResquest webApiCategory = new WebApiResquest("/Api/RestaurantItemCategory/AddUpdate", "POST", ricAddUpdate);
                        await webApiCategory.Request();
                    }
                    
                    await Application.Current.MainPage.DisplayAlert("Correcto", "Operación finalizada con exito.", "Ok");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Vuelva a intentarlo más tarde", "Ok");
                }
                Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
                await Application.Current.MainPage.Navigation.PopAsync();
            });
            
            PickImageCommand = new Command(async () =>
            {
                await CrossMedia.Current.Initialize();
    
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Sin cámara disponible.", "Ok");
                    return;
                }

                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium
                });

                if (file == null)
                    return;

                Image = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                }); 
                MemoryStream ms = new MemoryStream();
                file.GetStream().CopyTo(ms);
                _item.Image = ms.ToArray();;
            });
        }
    }
}