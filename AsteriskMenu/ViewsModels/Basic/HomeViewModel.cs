using System;
using System.IO;
using System.Net;
using System.Windows.Input;
using AsteriskMenu.Helpers;
using AsteriskMenu.Views.Basic;
using AsteriskMenu.Views.OrderV;
using Data.Infrastructure;
using Newtonsoft.Json;
using QRCoder;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing;
using ZXing.Net.Mobile.Forms;

namespace AsteriskMenu.ViewsModels.Basic
{
    public class HomeViewModel : ViewModelBase
    {

        private bool _isLogged;
        private bool _isLoggedInverse;

        
        public bool IsLogged{
            get {
                return _isLogged;
            }
            set {
                _isLogged= value;
                IsLoggedInverse = !value;
                OnPropertyChanged();
            }
        }

        public bool IsLoggedInverse
        {
            get
            {
                return _isLoggedInverse;
            }
            set
            {
                _isLoggedInverse = value;
                OnPropertyChanged();
            }
        }



        public Command ClickedButtonSignUp { get; set; }
        
        public Command ClickedButtonLogin { get; set; }

        public Command ClickedButtonAdminPanel { get; set; }


        public ICommand ScanQrCommand => new Command(async () =>
        {
            try
            {
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                scanner.TopText = "Escanea el código QR";
                scanner.BottomText = "Centra el código en la pantalla";
                var result = await scanner.Scan();
                if(result != null)
                {
                    QrTable data = JsonConvert.DeserializeObject<QrTable>(result.Text);
                    Guid tableId = data.TableId;
                    Guid restaurantId = data.RestaurantId;
                    ((App) App.Current).ActiveRestaurantId = restaurantId;
                    App.Current.MainPage.Navigation.PushAsync(new DataViewOrder(null, tableId));
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Vuelva a escanear el código QR","Ok");
            }

        });

        public HomeViewModel()
        {
            if (String.IsNullOrWhiteSpace(Helper.GetBearToken()))
            {
                IsLogged = false;
            }
            else
            {
                IsLogged = true;
            }
            ClickedButtonSignUp = new Command(() =>
            {
                App.Current.MainPage.Navigation.PushAsync(new SingUpView());
            });

            ClickedButtonLogin = new Command(() =>
            {
                App.Current.MainPage.Navigation.PushAsync(new LoginView(""));
            });

            ClickedButtonAdminPanel = new Command(() =>
            {
                App.Current.MainPage.Navigation.PushAsync(new AdminHomeView());
            });

        }
    }
}