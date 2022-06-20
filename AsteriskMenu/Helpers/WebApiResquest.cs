using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Data.DTO.Users;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AsteriskMenu.Helpers
{
    public class WebApiResquest
    {
        
        private string _domain { get; set; } = App.Current.Resources["WebApiDomain"].ToString();

        private string _url
        {
            get => $"{_domain}{PathDomain}";
        }
        
        public string PathDomain { get; set; }
        
        public string Method { get; set; }

        public object Data { get; set; }
        

        public WebApiResquest(string pathDomain, string method="GET", object data = null)
        {
            PathDomain = pathDomain;
            Method = method;
            Data = data;
        }

        public async Task<HttpResponseMessage> Request(bool isRefreshToken=false)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            try
            {
                DateTime now = DateTime.Now;
                DateTime expire =  Helper.GetBearExpire(now);
                if (expire < now && !isRefreshToken)
                {
                    WebApiResquest refreshToken = new WebApiResquest(App.Current.Resources["UsersRefreshToken"].ToString(), "POST");
                    HttpResponseMessage responseRefreshToken = await refreshToken.Request(true);
                    string data = await responseRefreshToken.Content.ReadAsStringAsync();
                    AuthenticateResponse refreshResponse =
                        JsonConvert.DeserializeObject<AuthenticateResponse>(data);
                        
                    Helper.SetBearToken(refreshResponse.JwtToken);
                }
                HttpClient client = new HttpClient();
                string bearToken =  Helper.GetBearToken();
                if (!string.IsNullOrEmpty(bearToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearToken);
                }
                string json = JsonConvert.SerializeObject(Data);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                switch (Method)
                {
                    case "GET":
                        response = await client.GetAsync(_url);
                        break;
                    case "POST":
                        response = await client.PostAsync(_url, content);
                        break;
                    case "PUT":
                        response = await client.PutAsync(_url, content);
                        break;
                    case "DELETE":
                        response = await client.DeleteAsync(_url);
                        break;
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.Content.ReadAsStringAsync().Result.Contains("incorrect"))
                {
                    Helper.SetBearToken(null);
                    await App.Current.MainPage.DisplayAlert("Error", "Autentificación incorrecta", "Ok");
                    return response;
                }

                if (!response.IsSuccessStatusCode)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Error en la conexión, vuelva a intentarlo más tarde.", "Ok");
                    App.Current.MainPage.Navigation.PopAsync();
                }

            }
            catch (Exception e)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Error al obtener los datos, vuelva a intentarlo más tarde.", "Ok");
                App.Current.MainPage.Navigation.PopAsync();
            }

            

            return response;
        }

      
    }
}