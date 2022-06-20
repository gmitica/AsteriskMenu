using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsteriskMenu.ViewsModels.TableVM;
using Data.Entities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsteriskMenu.Views.TableV
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataViewTable : ContentPage
    {
        public DataViewTable(Table table=null)
        {
            InitializeComponent();
            BindingContext = new DataViewTableVM(table);
        }
    }
}