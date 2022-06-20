using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AsteriskMenu.Helpers;
using AsteriskMenu.ViewsModels.Basic;
using AutoMapper;
using Data.DTO.Restaurant;
using Data.Entities;
using Data.Mappings;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;


namespace AsteriskMenu.ViewsModels.RestaurantVM
{
    public class DataViewRestauranVM : ViewModelBase
    {
        
        private bool _controlIsCreated = false;
        private int _stateId = -1;
        private int _cityId = -1;
        private List<Country> _countries = new List<Country>();
        private List<State> _states = new List<State>();
        private List<City> _cities = new List<City>();
        private Restaurant _restaurant = new Restaurant();
        private Country _selectedCountry = new Country();
        private State _selectedState = new State();
        private City _selectedCity = new City();
        private ImageSource _image;
        public string TextAddUpdate { get; set; } = "Crear Restaurante";



 
        
        public Restaurant Restaurant
        {
            get { return _restaurant; }
            set
            {
                _restaurant = value;
                OnPropertyChanged();
            }
        }
        
        public Country SelectedCountry
        {
            get
            {
                return _selectedCountry;
            }
            set
            {
                _selectedCountry = value;
                if (_selectedCountry!=null)
                {
                    Task.Run(async () =>
                    {
                        WebApiResquest webApi = new WebApiResquest(pathDomain:App.Current.Resources["StatesGetAll"]+"/"+_selectedCountry.Id);
                        HttpResponseMessage response = await webApi.Request();
                        if (response.IsSuccessStatusCode)
                        {
                            string result = await response.Content.ReadAsStringAsync();
                            States = JsonConvert.DeserializeObject<List<State>>(result);
                            SelectedState = States.FirstOrDefault(x=>x.Id==_stateId);
                        }
                        else
                        {
                            Console.WriteLine("Error");
                        }
                    }).Wait();
                }
                OnPropertyChanged();
            }
        }
        
        public State SelectedState
        {
            get
            {
                return _selectedState;
            }
            set
            {
                _selectedState = value;
                if (_selectedState!=null)
                {
                    Task.Run(async () =>
                    {
                        WebApiResquest webApi = new WebApiResquest(pathDomain:App.Current.Resources["CitiesGetAll"]+"/"+_selectedState.Id);
                        HttpResponseMessage response = await webApi.Request();
                        if (response.IsSuccessStatusCode)
                        {
                            string result = await response.Content.ReadAsStringAsync();
                            Cities = JsonConvert.DeserializeObject<List<City>>(result);
                            SelectedCity = Cities.FirstOrDefault(x=>x.Id==_cityId); 
                        }
                        else
                        {
                            Console.WriteLine("Error");
                        }
                    }).Wait();
                }
                OnPropertyChanged();
            }
        }
        
        public City SelectedCity
        {
            get
            {
                return _selectedCity;
            }
            set
            {
                _selectedCity = value;
                if (_selectedCity != null && _restaurant!=null)
                {
                    _restaurant.CityId = _selectedCity.Id;
                }
                OnPropertyChanged();
            }
        }
        public List<Country> Countries
        {
            get
            {
                return _countries;
            }
            set
            {
                _countries = value;
                OnPropertyChanged();
            }
        }
        
        public List<State> States
        {
            get
            {
                return _states;
            }
            set
            {
                _states = value;
                OnPropertyChanged();
            }
        }
        
        public List<City> Cities
        {
            get
            {
                return _cities;
            }
            set
            {
                _cities = value;
                OnPropertyChanged();
            }
        }

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


        public Command ButtonAddRestaurant { get; set; }
        public Command PickImageCommand { get; set; }
        
       

        public DataViewRestauranVM(Restaurant restaurant = null)
        {
            Task.Run(async () =>
            {
                WebApiResquest webApi = new WebApiResquest(App.Current.Resources["CountriesGetAll"].ToString());
                HttpResponseMessage response = await webApi.Request();
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Countries = JsonConvert.DeserializeObject<List<Country>>(result);
                    SelectedCountry = Countries.FirstOrDefault(x => x.Name=="Spain");
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }).Wait();
            if (restaurant!=null)
            {
                _restaurant = restaurant;
                _controlIsCreated = true;
                TextAddUpdate = "Modificar restaurante";
                _stateId = restaurant.City.StateId;
                _cityId = restaurant.CityId;
                SelectedCountry = Countries.FirstOrDefault(x => x.Id == restaurant.City.State.CountryId);
                
                Image = ImageSource.FromStream(() =>
                {
                    return new MemoryStream(restaurant.Image);
                });
            }




            ButtonAddRestaurant = new Command(async () =>
            {
                WebApiResquest webApi;

                if (_controlIsCreated)
                {
                    webApi = new WebApiResquest("/Api/Restaurant/Update", "PUT", _restaurant);
                }else
                {
                    IMapper mapper = (IMapper)App.Current.Properties["Mapper"];
                    RestaurantAddDTO restaurantAddDto = mapper.Map<RestaurantAddDTO>(_restaurant);
                    webApi = new WebApiResquest("/Api/Restaurant/Add", "POST", restaurantAddDto);
                }

                HttpResponseMessage response = await webApi.Request();
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Restaurant resultObjt = JsonConvert.DeserializeObject<Restaurant>(result);
                    Console.WriteLine(resultObjt.Id);
                    await Application.Current.MainPage.DisplayAlert("Correcto", "Operación finalizada con exito.", "Ok");
                    Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Vuelva a intentarlo más tarde.", "Ok");
                }
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
                _restaurant.Image = ms.ToArray();;
            });

        }
    }
}