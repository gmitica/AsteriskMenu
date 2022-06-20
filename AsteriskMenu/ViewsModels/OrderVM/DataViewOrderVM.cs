using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using AsteriskMenu.Helpers;
using AsteriskMenu.Views.OrderV;
using AsteriskMenu.ViewsModels.Basic;
using AutoMapper;
using Data.DTO.Order;
using Data.DTO.Restaurant;
using Data.DTO.Table;
using Data.Entities;
using Data.Infrastructure;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace AsteriskMenu.ViewsModels.OrderVM
{
    public class DataViewOrderVM : ViewModelBase
    {

        public ObservableCollection<RestaurantItemCategoryGrouped> RestaurantItemCategoryGrouped { get; set; } = new ObservableCollection<RestaurantItemCategoryGrouped>();



        private bool _isVisibleOptions { get; set; } = true;
        public bool IsVisibleOptions
        {
            get => _isVisibleOptions;
            set
            {
                _isVisibleOptions = value;
                OnPropertyChanged();
            }
        }

        private bool _showCreateOrder { get; set; } = true;
        public bool ShowCreateOrder
        {
            get => _showCreateOrder;
            set
            {
                _showCreateOrder = value;
                OnPropertyChanged();
            }
        }

        private bool _showButtonFinish { get; set; } = false;
        public bool ShowButtonFinish
        {
            get => _showButtonFinish;
            set
            {
                _showButtonFinish = value;
                OnPropertyChanged();
            }
        }

        private bool _showTextFinish { get; set; } = false;
        public bool ShowTextFinish
        {
            get => _showTextFinish;
            set
            {
                _showTextFinish = value;
                OnPropertyChanged();
            }
        }


        public string _message { get; set; } = "";
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        private string _waiterName { get; set; } =
            $"{Helper.GetClaim("UserFirstName")} {Helper.GetClaim("UserLastName")}";
        public string WaiterName
        {
            get => _waiterName;
            set
            {
                _waiterName = value;
                OnPropertyChanged();
            }
        }
        
        private Order _order = new Order();
        public Order Order
        {
            get => _order;
            set
            {
                _order = value;
                OnPropertyChanged();
            }
        }

        private List<Table> _tables { get; set; } = new List<Table>();
        public List<Table> Tables
        {
            get => _tables;
            set
            {
                _tables = value;
                OnPropertyChanged();
            }
        }

        private Table _selectedTable { get; set; }
        public Table SelectedTable
        {
            get => _selectedTable;
            set
            {
                _selectedTable = value;
                OnPropertyChanged();
            }
        }
        
      
        public ICommand CreateOrder => new Command(async () =>
        {
            if (SelectedTable == null)
            {
                Message = "Tienes que seleccionar una mesa";
                return;
            }
            
            
            {
                Message = "";
                _order.DateCreated = DateTime.Now;
                _order.TableId = SelectedTable.Id;
                try
                {
                    _order.WaiterId = Guid.Parse(Helper.GetClaim("UserId"));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Usuario no logueado");
                }
                List<RestaurantItemCategoryOrder> items = RestaurantItemCategoryGrouped.SelectMany(x => x).ToList();
                List<OrderItem> orderItems = new List<OrderItem>();
                bool checkPointUnits = false;
                foreach (RestaurantItemCategoryOrder item in items)
                {
                    if (item.Units>0)
                    {
                        orderItems.Add( new OrderItem
                        {
                            RestaurantItemCategoryId = item.Id,
                            Units = item.Units
                        });
                        checkPointUnits = true;
                    }
                }

                if (!checkPointUnits)
                {
                    Message = "Tienes que seleccionar al menos un producto";
                    return;
                }

                _order.OrderItems = orderItems;
                Order = _order;
                IMapper mapper = (IMapper)App.Current.Properties["Mapper"];
                OrderAddDTO toAdd = mapper.Map<OrderAddDTO>(Order);
                WebApiResquest request = new WebApiResquest("/Api/Order/Add", "POST", toAdd);
                HttpResponseMessage response = await request.Request();
                if (response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Correcto", "Operación finalizada con exito.", "Ok");
                    if (Application.Current.MainPage.Navigation.NavigationStack.Count>2)
                    {
                        Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
                    }
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Vuelva a intentarlo más tarde.", "Ok");
                }
            }
        });
        
    
        public ICommand FinishOrder => new Command(async () =>
        {
            _order.DateFinish = DateTime.Now;
            Order = _order;
            IMapper mapper = (IMapper)App.Current.Properties["Mapper"];
            OrderUpdateDTO toUpdate = mapper.Map<OrderUpdateDTO>(Order);
            WebApiResquest request = new WebApiResquest("/Api/Order/Update", "PUT", toUpdate);
            HttpResponseMessage response = await request.Request();
            if (response.IsSuccessStatusCode)
            {
                App.Current.MainPage.DisplayAlert("Correcto", "Operación finalizada con exito.", "Ok");
                ShowTextFinish = true;
                ShowButtonFinish = false;
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Error", "Vuelva a intentarlo más tarde.", "Ok");
            }
        });
        
        private async void InitializeAsync(Order order = null, Guid tableId = default)
        {
            Guid restaurantId = ((App) App.Current).ActiveRestaurantId;
            WebApiResquest webApi = new WebApiResquest($"/Api/Table/GetAll/{restaurantId}");
            HttpResponseMessage response = await webApi.Request();
            string result = await response.Content.ReadAsStringAsync();
            Tables = JsonConvert.DeserializeObject<List<Table>>(result).Where(x => x.Active).ToList();
            if (order != null)
            {
                Order = order;
                SelectedTable = Tables.FirstOrDefault(x => x.Id == order.TableId);
                WaiterName = "";
                if (_order.Waiter!=null)
                {
                    WaiterName = _order.Waiter.FirstName + " " + _order.Waiter.LastName;
                }

                ShowCreateOrder = false;
                if(_order.DateFinish != null)
                {
                    ShowButtonFinish = false;
                    ShowTextFinish = true;
                }
                else
                {
                    ShowButtonFinish = true;
                    ShowTextFinish = false;
                }
            }
            if (tableId!=null&& tableId != Guid.Empty)
            {
                SelectedTable = Tables.FirstOrDefault(x => x.Id == tableId);
                if (SelectedTable==null)
                {
                    await App.Current.MainPage.DisplayAlert("Info", "La mesa no esta disponible", "Ok");
                    App.Current.MainPage.Navigation.PopAsync();
                }
                if (String.IsNullOrWhiteSpace(Helper.GetClaim("UserId")))
                {
                    IsVisibleOptions = false;
                    ShowButtonFinish = false;
                }
            }
            WebApiResquest webApiRIC = new WebApiResquest($"/Api/RestaurantItemCategory/GetAll/{restaurantId}");
            HttpResponseMessage responseRIC = await webApiRIC.Request();
            string resultRIC = await responseRIC.Content.ReadAsStringAsync();
            List<RestaurantItemCategory> restaurantItemCategories = JsonConvert.DeserializeObject<List<RestaurantItemCategory>>(resultRIC);
            List<string> categoryNames = restaurantItemCategories.Select(x => x.Category.DisplayName).Distinct().ToList();
            foreach (string categoryName in categoryNames)
            {
                IMapper mapper = (IMapper)App.Current.Properties["Mapper"];
                if (order != null)
                {
                    ObservableCollection<RestaurantItemCategoryOrder> restaurantAddDto = mapper.Map<ObservableCollection<RestaurantItemCategoryOrder>>(restaurantItemCategories.Where(x => x.Category.DisplayName == categoryName).ToList());
                    ObservableCollection<RestaurantItemCategoryOrder> restaurantAddDtoExist =
                        new ObservableCollection<RestaurantItemCategoryOrder>();
                    foreach (RestaurantItemCategoryOrder item in restaurantAddDto)
                    {
                        int units = 0;
                        
                            units = order.OrderItems.Where(x => x.RestaurantItemCategoryId == item.Id).Select(x => x.Units).FirstOrDefault();
                        item.Units = units;
                        if (item.Units>0)
                        {
                            restaurantAddDtoExist.Add(item);
                        }

                    }

                    if (restaurantAddDtoExist.Count>0)
                    {
                        RestaurantItemCategoryGrouped.Add(new RestaurantItemCategoryGrouped(categoryName, restaurantAddDtoExist));
                    }
                }
                else
                {
                    ObservableCollection<RestaurantItemCategoryOrder> restaurantAddDto = mapper.Map<ObservableCollection<RestaurantItemCategoryOrder>>(restaurantItemCategories.Where(x => x.Category.DisplayName == categoryName && x.Item.Active).ToList());
                    RestaurantItemCategoryGrouped.Add(
                        new RestaurantItemCategoryGrouped(categoryName, restaurantAddDto));
                }

                
            }
        }



        public DataViewOrderVM(Order order, Guid tableId)
        {
            InitializeAsync(order, tableId);
        }
        
        public DataViewOrderVM()
        {
            InitializeAsync();
        }
      
    }
}