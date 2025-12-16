using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TEXTA_score_edit
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AjouterOngletPlus(); // Ajoute l'onglet "+"
            AjouterOnglet();     // Ajoute le premier onglet normal
        }

        private void AjouterOnglet(string cheminFichier = null)
        {
            string nomOnglet = cheminFichier != null ? Path.GetFileName(cheminFichier) : "Nouveau fichier";

            var contenu = new ScoreEditor(); // Remplace par ton contrôle UserControl personnalisé

            var onglet = new TabItem
            {
                Header = CreerEnteteOnglet(nomOnglet),
                Content = contenu
            };

            // Cherche l'onglet "+"
            int indexPlus = -1;
            for (int i = 0; i < MainTabControl.Items.Count; i++)
            {
                if (MainTabControl.Items[i] is TabItem item &&
                    item.Header is TextBlock tb &&
                    tb.Text == "+")
                {
                    indexPlus = i;
                    break;
                }
            }

            if (indexPlus >= 0)
            {
                MainTabControl.Items.Insert(indexPlus, onglet);
            }
            else
            {
                MainTabControl.Items.Add(onglet);
            }

            MainTabControl.SelectedItem = onglet;
        }

        private void AjouterOngletPlus()
        {
            var plusTab = new TabItem
            {
                Header = new TextBlock { Text = "+", FontWeight = FontWeights.Bold, Margin = new Thickness(5) },
                IsEnabled = true
            };
            MainTabControl.Items.Add(plusTab);
        }

        private StackPanel CreerEnteteOnglet(string nom)
        {
            var textBlock = new TextBlock { Text = nom, Margin = new Thickness(0, 0, 5, 0) };
            var closeButton = new Button
            {
                Content = "✕",
                Width = 20,
                Height = 20,
                Padding = new Thickness(0),
                Margin = new Thickness(0),
                Cursor = Cursors.Hand,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0)
            };

            // Pour récupérer le TabItem contenant ce bouton on remonte la chaîne visuelle
            closeButton.Click += (s, e) =>
            {
                var button = s as Button;
                if (button == null) return;

                // Remonter la hiérarchie visuelle pour trouver le TabItem parent
                DependencyObject parent = button;
                while (parent != null && !(parent is TabItem))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }

                if (parent is TabItem tabItem)
                {
                    MainTabControl.Items.Remove(tabItem);
                }
            };

            var panel = new StackPanel { Orientation = Orientation.Horizontal };
            panel.Children.Add(textBlock);
            panel.Children.Add(closeButton);

            return panel;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem is TabItem selectedTab &&
                selectedTab.Header is TextBlock tb &&
                tb.Text == "+")
            {
                AjouterOnglet(); // Créé un nouvel onglet si "+" sélectionné
            }
        }
    }
}
