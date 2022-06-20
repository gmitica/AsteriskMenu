using System.Net.Http;
using AsteriskMenu.Helpers;
using AsteriskMenu.Views.Basic;
using Data.DTO.Users;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AsteriskMenu.ViewsModels.Basic
{
    public class LogInViewModel : ViewModelBase
    {
        private string _message;
        private string _email;
        private string _password;
        private bool _aceptTerms;

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }


        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public bool AceptTerms
        {
            get => _aceptTerms;
            set
            {
                _aceptTerms = value;
                OnPropertyChanged();
            }
        }

        public Command ClickedLegal { get; set; }

        public Command ClickedLogIn{ get; set; }

        public Command GetData { get; set; }
        
        public LogInViewModel(string email)
        {
            Email = email;
            
            ClickedLegal = new Command(() => { App.Current.MainPage.Navigation.PushAsync(new LegalView()); });

            ClickedLogIn = new Command(async () =>
            {
                if (string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(_password))
                {
                    Message = "Todos los campos son obligatorios";
                }
                else if (AceptTerms == false)
                {
                    Message =
                        "Para poder realizar el inicio de sesión si esta de acuerdo debe aceptar los términos y condiciones";
                }
                else
                {
                    WebApiResquest webApiResquest = new WebApiResquest(App.Current.Resources["UsersLogIn"].ToString(), "POST");
                    UserAuthenticateDTO userAuthenticateRequestser = new UserAuthenticateDTO
                    {
                        Email = _email,
                        Password = _password,
                    };
                    webApiResquest.Data = userAuthenticateRequestser;
                    HttpResponseMessage response = await webApiResquest.Request();
                    string data = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        AuthenticateResponse authenticateResponse =
                            JsonConvert.DeserializeObject<AuthenticateResponse>(data);
                        
                        Helper.SetBearToken(authenticateResponse.JwtToken);
                        Message = "OK, espere...";
                        App.Current.MainPage.Navigation.PopAsync();
                    }
                    else
                    {
                        Message = "Autentificación incorrecta, vuelva a intentarlo o cree una nueva cuenta";
                    }   
                    
                }
            });

            GetData = new Command(async () =>
            {
                WebApiResquest request = new WebApiResquest( App.Current.Resources["UsersGetAllUsers"].ToString());
                HttpResponseMessage response = await request.Request();
                App.Current.MainPage.DisplayAlert("Hola", await response.Content.ReadAsStringAsync(), "OK");
            });
        }
    }
}