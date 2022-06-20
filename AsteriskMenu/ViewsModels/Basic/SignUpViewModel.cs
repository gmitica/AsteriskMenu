using System;
using System.Diagnostics;
using System.Net.Http;
using AsteriskMenu.Helpers;
using AsteriskMenu.Views.Basic;
using Data.Entities;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AsteriskMenu.ViewsModels.Basic
{
    public class SignUpViewModel : ViewModelBase
    {
        private string _message;
        private string _name;
        private string _surName;
        private string _email;
        private string _password;
        private string _confirmPassword;
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

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string SurName
        {
            get => _surName;
            set
            {
                _surName = value;
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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
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

        public Command ClickedSignUp { get; set; }

        public SignUpViewModel()
        {
            ClickedLegal = new Command(() => { App.Current.MainPage.Navigation.PushAsync(new LegalView()); });

            ClickedSignUp = new Command(async () =>
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(SurName) || string.IsNullOrEmpty(Email) ||
                    string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
                {
                    Message = "Todos los campos son obligatorios";
                }
                else if (AceptTerms == false)
                {
                    Message =
                        "Para poder realizar el registro si esta de acuerdo debe aceptar los términos y condiciones";
                }
                else if (Password != ConfirmPassword)
                {
                    Message = "Las contraseñas no coinciden";
                }
                else
                {
                    User user = new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = _name,
                        LastName = _surName,
                        Email = _email,
                        Password = _password,
                    };
                    WebApiResquest webApiResquest = new WebApiResquest(App.Current.Resources["UsersSignUp"].ToString(), "POST", user);
                    HttpResponseMessage response = await webApiResquest.Request();
                    Debug.WriteLine(JsonConvert.SerializeObject(response));
                    if (response.IsSuccessStatusCode)
                    {
                        Message = "Registro exitoso";
                        App.Current.MainPage.Navigation.PopAsync();
                    }
                    else
                    {
                        Message = "El correo ya esta registrado";
                    }   
                    
                    
                    
                }
            });
        }
    }
}