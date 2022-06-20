using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsteriskMenu.ViewsModels.TableVM;
using AsteriskMenu.ViewsModels.UserVM;
using Data.Entities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsteriskMenu.Views.UserV
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataViewUser : ContentPage
    {
        public DataViewUser(UserRestaurant userRestaurant=null)
        {
            InitializeComponent();
            BindingContext = new DataViewUserVM(userRestaurant);
        }
    }
}