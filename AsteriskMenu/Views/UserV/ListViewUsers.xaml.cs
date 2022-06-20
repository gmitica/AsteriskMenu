using AsteriskMenu.ViewsModels.UserVM;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsteriskMenu.Views.UserV
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewUsers : ContentPage
    {
        public ListViewUsers()
        {
            InitializeComponent();
            BindingContext = new ListViewUsersVM();
        }
    }
}