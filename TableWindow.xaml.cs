using System.Windows;

namespace HCI
{
    public partial class TableWindow : Window
    {
        public TableWindow(MainViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }
    }
}
