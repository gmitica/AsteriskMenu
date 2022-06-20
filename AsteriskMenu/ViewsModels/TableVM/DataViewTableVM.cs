using System;
using System.IO;
using System.Net.Http;
using System.Windows.Input;
using AsteriskMenu.Helpers;
using AsteriskMenu.ViewsModels.Basic;
using AutoMapper;
using Data.DTO.Restaurant;
using Data.DTO.Table;
using Data.Entities;
using Data.Infrastructure;
using Newtonsoft.Json;
using QRCoder;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AsteriskMenu.ViewsModels.TableVM
{
    public class DataViewTableVM : ViewModelBase
    {

        private bool _controlIsCreated { get; set; } = false;
        public bool ControlIsCreated
        {
            get => _controlIsCreated;
            set
            {
                _controlIsCreated = value;
                OnPropertyChanged();
            }
        }

        private Table _table = new Table();
        
        public Table Table
        {
            get => _table;
            set
            {
                _table = value;
                OnPropertyChanged();
            }
        }

        public Command ButtonTableCreate { get; set; }
        
        public string TextAddUpdate { get; set; } = "Crear Mesa";


        public ICommand GenerateQR => new Command(async () =>
        {
            QrTable data = new QrTable
            {
                TableId = _table.Id,
                RestaurantId = _table.RestaurantId
            };
            string jsonString = JsonConvert.SerializeObject(data);
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(jsonString, QRCodeGenerator.ECCLevel.L);
            var qRCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qRCode.GetGraphic(20);
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            Directory.CreateDirectory(path);
            var filename = Path.Combine(path, "qr.png");
            File.WriteAllBytes(filename, qrCodeBytes);         
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filename)
            });
        });


        public DataViewTableVM(Table table=null)
        {
            if (table!= null)
            {
                _table = table;
                TextAddUpdate = "Actualizar Mesa";
                ControlIsCreated = true;
            }

            ButtonTableCreate = new Command(async () =>
            {
                WebApiResquest webApi;
                if (_controlIsCreated)
                {
                    
                    IMapper mapper = (IMapper)App.Current.Properties["Mapper"];
                    TableUpdateDTO tableUpdate = mapper.Map<TableUpdateDTO>(_table);
                    webApi = new WebApiResquest("/Api/Table/Update", "PUT", tableUpdate);
                   
                }
                else
                {
                    Guid restaurantId = ((App) App.Current).ActiveRestaurantId;
                    Table.RestaurantId = restaurantId;
                    IMapper mapper = (IMapper)App.Current.Properties["Mapper"];
                    TableAddDTO tableAdd = mapper.Map<TableAddDTO>(_table);
                    webApi = new WebApiResquest("/Api/Table/Add", "POST", tableAdd);
                }
                await webApi.Request();
                await Application.Current.MainPage.DisplayAlert("Correcto", "Operación finalizada con exito.", "Ok");
                Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
                await Application.Current.MainPage.Navigation.PopAsync();
            });
        }
    }
}