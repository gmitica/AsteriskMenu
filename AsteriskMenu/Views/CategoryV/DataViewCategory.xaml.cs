using AsteriskMenu.ViewsModels.CategoryVM;
using Data.Entities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsteriskMenu.Views.CategoryV
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataViewCategory : ContentPage
    {
        public DataViewCategory(Category category=null)
        {
            InitializeComponent();
            BindingContext = new DataViewCategoryVM(category);
        }
    }
}