using AsteriskMenu.ViewsModels.ItemVM;
using Data.Entities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsteriskMenu.Views.ItemV
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataViewItem: ContentPage
    {
        public DataViewItem(Item item=null)
        {
            InitializeComponent();
            BindingContext = new DataViewItemVM(item);
        }
    }
}