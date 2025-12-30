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

namespace TEXTA_score_edit.src.ui
{
    /// <summary>
    /// Logique d'interaction pour HomeScreen.xaml
    /// </summary>
    public partial class HomeScreen : UserControl
    {
        public HomeScreen()
        {
            InitializeComponent();
            Loaded += HomeScreen_Loaded;

        }
        
        private void HomeScreen_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationListBox.SelectedIndex = 0;
        }

        private void NavigationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainContent == null)
                return;

            if (NavigationListBox.SelectedItem is ListBoxItem item)
            {
                switch (item.Content?.ToString())
                {
                    case "Partitions":
                        MainContent.Content = new PartitionsView();
                        break;

                    case "Partagés":
                        MainContent.Content = new PartagesView();
                        break;

                    case "Partitions locales":
                        MainContent.Content = new PartitionsLocalesView();
                        break;

                    case "Bibliothèque":
                        MainContent.Content = new BibliothequeView();
                        break;
                }
            }
        }

    }
}
