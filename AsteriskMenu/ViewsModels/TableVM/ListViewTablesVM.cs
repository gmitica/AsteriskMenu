using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using AsteriskMenu.Helpers;
using AsteriskMenu.Views.TableV;
using AsteriskMenu.ViewsModels.Basic;
using Data.Entities;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AsteriskMenu.ViewsModels.TableVM
{
    public class ListViewTablesVM : ViewModelBase
    {
        private List<Table> _tables;
        private Table _selectedTable;

        public ICommand ButtonTableAdd => new Command(() =>
        {
            App.Current.MainPage.Navigation.PushAsync(new DataViewTable());
        });

        public List<Table> Tables
        {
            get => _tables;
            set
            {
                _tables = value;
                OnPropertyChanged();
            }
        }

        public Table SelectedTable
        {
            get => _selectedTable;
            set
            {
                _selectedTable = value;
                if (_selectedTable!=null)
                {
                    OnClickRow(_selectedTable);
                }

                OnPropertyChanged();
            }
        }
        

        public ListViewTablesVM()
        {
            Tables = new List<Table>();
            InitializeAsync();
        }

        public async void InitializeAsync()
        {
            Guid restaurantId = ((App) App.Current).ActiveRestaurantId;
            WebApiResquest webApi = new WebApiResquest($"/Api/Table/GetAll/{restaurantId}");
            HttpResponseMessage response = await webApi.Request();
            string result = await response.Content.ReadAsStringAsync();
            Tables = JsonConvert.DeserializeObject<List<Table>>(result);
        }
        private void OnClickRow(Table table)
        {
            App.Current.MainPage.Navigation.PushAsync(new DataViewTable(table));
        }
        
     
       
        public ICommand DeleteCommand => new Command<Table>(async (Table item) =>
        {
            bool confirmation = await App.Current.MainPage.DisplayAlert("Eliminar", "¿Confirmar eliminación?", "Si", "No");
            if (confirmation)
            {
                WebApiResquest webApi = new WebApiResquest($"/Api/Table/Delete/{item.Id}", "DELETE");
                HttpResponseMessage response = await webApi.Request();
                if (response.IsSuccessStatusCode)
                {
                    Tables = Tables.Where(x=>x.Id != item.Id).ToList();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar la mesa", "Ok");
                }
            }
        });


    }
}