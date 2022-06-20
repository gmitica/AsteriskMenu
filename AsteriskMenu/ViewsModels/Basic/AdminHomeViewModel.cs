using System.Net.Http;
using System.Windows.Input;
using AsteriskMenu.Helpers;
using AsteriskMenu.Views.OrderV;
using AsteriskMenu.Views.RestaurantV;
using Data.DTO.Users;
using Xamarin.Forms;

namespace AsteriskMenu.ViewsModels.Basic
{
    public class AdminHomeViewModel : ViewModelBase
    {

        private string _welcomeMessage;

        public bool IsVisibleRestaurants { get; set; }
        public string WelcomeMessage
        {
            get
            {
                return _welcomeMessage;
            }
            set
            {
                _welcomeMessage = value;
                OnPropertyChanged();
            }
        }
        public Command ClickedButtonLogout { get; set; }

        public Command ClickRestaurant { get; set; }
        
        public ICommand ClickOrders => new Command(async ()=>
        {
            App.Current.MainPage.Navigation.PushAsync(new ListViewOrders());
        });

        public AdminHomeViewModel()
        {

            WelcomeMessage = $"Hola, {Helper.GetClaim("UserFirstName")} {Helper.GetClaim("UserLastName")}";
            if (Helper.GetClaim("IsManaged")=="True")
            {
                IsVisibleRestaurants = false;
            }
            else
            {
                IsVisibleRestaurants = true;
            }
            
            ClickedButtonLogout = new Command(async () =>
            {

                bool dialogResponse = await App.Current.MainPage.DisplayAlert("Advertencia", "¿Cerrar sesión?", "SI", "NO");
                if (dialogResponse)
                {
                    RevokeTokenRequest data = new RevokeTokenRequest();
                    data.Token = "";
                    WebApiResquest request = new WebApiResquest(App.Current.Resources["UsersLogOut"].ToString(), "POST", data);
                    HttpResponseMessage response = await request.Request();
                    Helper.SetBearToken(null);
                    App.Current.MainPage.Navigation.PopAsync();
                }
            });
            
            ClickRestaurant = new Command(() =>
            {
                App.Current.MainPage.Navigation.PushAsync(new ListViewRestaurants());
            });
        }
    }
}