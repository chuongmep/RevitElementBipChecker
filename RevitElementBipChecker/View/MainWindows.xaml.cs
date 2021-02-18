using System.Windows;
using RevitElementBipChecker.Viewmodel;

namespace RevitElementBipChecker.View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainWindows : Window
    {
        public BipCheckerViewmodel Viewmodel;
        public MainWindows(BipCheckerViewmodel vm)
        {
            this.DataContext = vm;
            this.Viewmodel = vm;
            Viewmodel.frmmain = this;
            InitializeComponent();
        }

        private void AboutOnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://chuongmep.com/");
        }
    }
}
