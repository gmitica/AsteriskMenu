using AsteriskMenu.ViewsModels.CategoryVM;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsteriskMenu.Views.CategoryV
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewCategories : ContentPage
    {
        public ListViewCategories()
        {
            InitializeComponent();
            BindingContext = new ListViewCategoriesVM();
        }
    }
}