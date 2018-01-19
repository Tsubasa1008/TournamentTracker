using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrackerWPFUI.ViewModels;

namespace TrackerWPFUI.Views
{
    /// <summary>
    /// CreateTournamentView.xaml 的互動邏輯
    /// </summary>
    public partial class CreateTournamentView : UserControl
    {
        public CreateTournamentView()
        {
            InitializeComponent();
        }

        private void EmailAddress_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) CreateTournamentViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) CreateTournamentViewModel.Errors -= 1;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var errors = this.GetValue(Validation.ErrorsProperty);
        }
    }
}
