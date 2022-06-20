using AsteriskMenu.ViewsModels.ItemVM;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsteriskMenu.Views.ItemV
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewItems : ContentPage
    {
        public ListViewItems()
        {
            InitializeComponent();
            BindingContext = new ListViewItemsVM();
        }
    }
}