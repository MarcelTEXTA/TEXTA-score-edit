// TEXTA score edit - Éditeur de partitions musicales
// Copyright (C) 2025 Ethan Macaluso
// Ce programme est seulement destiné à un usage éducatif et personnel.s

using System;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TEXTA_score_edit
{
    public partial class ScoreEditor : UserControl
    {
        public enum OutilEdition
        {
            Selection,
            Insertion,
            Gomme,
            Ciseaux,
            Colle
        }

        private OutilEdition outilActif = OutilEdition.Selection;

        private string cleActuelle = "Clé de sol";
        private string figureRythmiqueActuelle = "Noire";
        private string signatureActuelle = "4/4";

        public OutilEdition OutilActif
        {
            get => outilActif;
            set
            {
                outilActif = value;
                Console.WriteLine($"Outil actif : {outilActif}");
            }
        }

        private bool isInitialise = false;

        public ScoreEditor()
        {
            InitializeComponent();
            Loaded += ScoreEditor_Loaded;
        }

        private void ScoreEditor_Loaded(object sender, RoutedEventArgs e)
        {
            isInitialise = true;
            DessinerPartition();
        }

        private void Outil_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == RadioSelection)
                OutilActif = OutilEdition.Selection;
            else if (sender == RadioInsertion)
                OutilActif = OutilEdition.Insertion;
            else if (sender == RadioGomme)
                OutilActif = OutilEdition.Gomme;
            else if (sender == RadioCiseaux)
                OutilActif = OutilEdition.Ciseaux;
            else if (sender == RadioColle)
                OutilActif = OutilEdition.Colle;
        }

        private void PartitionCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPosition = e.GetPosition(PartitionCanvas);

            switch (OutilActif)
            {
                case OutilEdition.Selection:
                    selectelement(clickPosition);
                    break;
                case OutilEdition.Insertion:
                    InsererNoteVisuelle(clickPosition);
                    break;
                case OutilEdition.Gomme:
                    SupprimerElementSousCurseur(clickPosition);
                    break;
                case OutilEdition.Ciseaux:
                    MessageBox.Show("Mode ciseaux : action future");
                    break;
                case OutilEdition.Colle:
                    MessageBox.Show("Mode colle : action future");
                    break;
            }

            switch (outilActif)
            {
                case OutilEdition.Selection:
                    PartitionCanvas.Cursor = Cursors.Arrow;
                    break;
                case OutilEdition.Insertion:
                    PartitionCanvas.Cursor = Cursors.Pen;
                    break;
                case OutilEdition.Gomme:
                    PartitionCanvas.Cursor = Cursors.Hand;
                    break;
                case OutilEdition.Ciseaux:
                    PartitionCanvas.Cursor = Cursors.Arrow;
                    break;
                case OutilEdition.Colle:
                    PartitionCanvas.Cursor = Cursors.Arrow;
                    break;
            }

        }

        private void InsererNoteVisuelle(Point position)
        {
            string caractereNote;

            switch (figureRythmiqueActuelle)
            {
                case "Noire":
                    caractereNote = "\uECA5";
                    break;
                case "Blanche":
                    caractereNote = "\uECA3";
                    break;
                case "Ronde":
                    caractereNote = "\uECA2";
                    break;
                case "Croche":
                    caractereNote = "\uECA7";
                    break;
                case "Double croche":
                    caractereNote = "\uECA9";
                    break;
                case "Triple croche":
                    caractereNote = "\uECAB";
                    break;
                case "Quadruple croche":
                    caractereNote = "\uECAD";
                    break;
                default:
                    caractereNote = "\uECA5";
                    break;
            }

            TextBlock note = new TextBlock
            {
                Text = caractereNote,
                FontFamily = new FontFamily("Bravura"),
                FontSize = 24,
                Foreground = Brushes.Black
            };

            Canvas.SetLeft(note, position.X);
            //Canvas.SetTop(note, AjusterYNote(position.Y));
            Canvas.SetTop(note, position.Y);
            PartitionCanvas.Children.Add(note);
        }

        private double AjusterYNote(double y)
        {
            double ligne0 = 50 + 4 * 10;
            double ecart = 5;
            return ligne0 - Math.Round((ligne0 - y) / ecart) * ecart;
        }

        private void SupprimerElementSousCurseur(Point position)
        {
            UIElement elementASupprimer = null;

            foreach (UIElement element in PartitionCanvas.Children)
            {
                if (element is FrameworkElement fe)
                {
                    double left = Canvas.GetLeft(fe);
                    double top = Canvas.GetTop(fe);
                    Rect zone = new Rect(left, top, fe.ActualWidth + 10, fe.ActualHeight + 10);
                    if (zone.Contains(position))
                    {
                        elementASupprimer = fe;
                        break;
                    }
                }
            }

            if (elementASupprimer != null)
            {
                PartitionCanvas.Children.Remove(elementASupprimer);
            }
        }

        // fonction pour sélectionner un élément
        private void selectelement(Point position)
        {
            UIElement elementSelectionne = null;
            foreach (UIElement element in PartitionCanvas.Children)
            {
                if (element is FrameworkElement fe)
                {
                    double left = Canvas.GetLeft(fe);
                    double top = Canvas.GetTop(fe);
                    Rect zone = new Rect(left, top, fe.ActualWidth + 10, fe.ActualHeight + 10);
                    if (zone.Contains(position))
                    {
                        elementSelectionne = fe;
                        break;
                    }
                }
            }
            if (elementSelectionne != null)
            {
                Console.WriteLine("Élément sélectionné : " + elementSelectionne);

                // élément en bleu
                elementSelectionne.SetValue(Panel.ZIndexProperty, 1);
                elementSelectionne.Opacity = 0.5;
            }
        }

        private void ComboCle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialise) return;

            if (ComboCle.SelectedItem is ComboBoxItem selectedItem)
            {
                cleActuelle = selectedItem.Content.ToString();
                Console.WriteLine($"Clé sélectionnée : {cleActuelle}");
                DessinerPartition(); // Redessine avec la clé choisie
            }
        }

        private void ComboNote_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialise) return;

            if (ComboNote.SelectedItem is ComboBoxItem selectedItem)
            {
                figureRythmiqueActuelle = selectedItem.Content.ToString();
                Console.WriteLine($"Figure rythmique sélectionnée : {figureRythmiqueActuelle}");
                // Inutile de redessiner toute la partition ici
            }
        }

        private void ComboSignature_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialise) return;

            if (ComboSignature.SelectedItem is ComboBoxItem selectedItem)
            {
                signatureActuelle = selectedItem.Content.ToString();
                Console.WriteLine($"Signature sélectionnée : {signatureActuelle}");
                DessinerPartition(); // Redessine avec la signature choisie
            }
        }

        private void ShowHome(object sender, RoutedEventArgs e)
        {
            FloatingFrame.Visibility = Visibility.Collapsed;
        }

        private void CloseFloatingFrame(object sender, RoutedEventArgs e)
        {
            FloatingFrame.Visibility = Visibility.Hidden;
        }

        private void ShowPartMenu(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement)
            {
                ContextMenu menuBoutonActif = new ContextMenu();

                MenuItem actionOptionsPartition = new MenuItem { Header = "Options de partition" };
                //actionOptionsPartition.Click += OptionsPartition;
                menuBoutonActif.Items.Add(actionOptionsPartition);
                menuBoutonActif.Items.Add(new Separator());
                menuBoutonActif.Items.Add(new MenuItem { Header = "Mode page" });
                menuBoutonActif.Items.Add(new Separator());
                menuBoutonActif.Items.Add(new MenuItem { Header = "Grouper / dégrouper les notes" });
                menuBoutonActif.Items.Add(new MenuItem { Header = "Convertir en note ornementale" });
                menuBoutonActif.Items.Add(new MenuItem { Header = "Définir N-olet" });
                menuBoutonActif.Items.Add(new MenuItem { Header = "Éclater" });
                menuBoutonActif.Items.Add(new Separator());
                menuBoutonActif.Items.Add(new MenuItem { Header = "Fusionner les portées" });
                menuBoutonActif.Items.Add(new MenuItem { Header = "Extraire la voix" });
                menuBoutonActif.Items.Add(new MenuItem { Header = "Insérer un legato" });
                menuBoutonActif.Items.Add(new MenuItem { Header = "Afficher / Masquer" });
                menuBoutonActif.Items.Add(new MenuItem { Header = "Inverser" });

                menuBoutonActif.IsOpen = true;
            }
        }

        private void DessinerPartition()
        {
            if (PartitionCanvas == null)
                return;

            PartitionCanvas.Children.Clear();
            double margeGauche = 20;
            double espacementLignes = 10;
            double longueurPortee = 500;
            double positionYDebut = 50;

            for (int i = 0; i < 5; i++)
            {
                Line ligne = new Line
                {
                    X1 = margeGauche,
                    Y1 = positionYDebut + i * espacementLignes,
                    X2 = margeGauche + longueurPortee,
                    Y2 = positionYDebut + i * espacementLignes,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                PartitionCanvas.Children.Add(ligne);
            }

            TextBlock symboleCle = new TextBlock
            {
                FontSize = 40,
                FontFamily = new FontFamily("Bravura, Arial Unicode MS, Segoe UI Symbol"),
                Foreground = Brushes.Black
            };

            switch (cleActuelle)
            {
                case "Clé de sol":
                    symboleCle.Text = "\uD834\uDD1E";
                    break;
                case "Clé de Fa":
                    symboleCle.Text = "\uD834\uDD22";
                    break;
                case "Clé de Ut":
                    symboleCle.Text = "\uD834\uDD20";
                    break;
                default:
                    symboleCle.Text = "\uD834\uDD1E"; // Par défaut : clé de sol
                    break;
            }

            Canvas.SetLeft(symboleCle, margeGauche);
            Canvas.SetTop(symboleCle, positionYDebut - 42);
            PartitionCanvas.Children.Add(symboleCle);

            string[] parties = signatureActuelle.Split('/');
            string chiffrageHautTexte = (parties.Length > 0) ? parties[0] : "4";
            string chiffrageBasTexte = (parties.Length > 1) ? parties[1] : "4";

            double xCle = margeGauche + 35;

            // Chiffre du haut
            TextBlock chiffreHaut = new TextBlock
            {
                Text = chiffrageHautTexte,
                FontSize = 16,
                FontFamily = new FontFamily("Arial"),
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(chiffreHaut, xCle);
            Canvas.SetTop(chiffreHaut, positionYDebut - 10);
            PartitionCanvas.Children.Add(chiffreHaut);

            // Chiffre du bas
            TextBlock chiffreBas = new TextBlock
            {
                Text = chiffrageBasTexte,
                FontSize = 16,
                FontFamily = new FontFamily("Arial"),
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(chiffreBas, xCle);
            Canvas.SetTop(chiffreBas, positionYDebut + 15);
            PartitionCanvas.Children.Add(chiffreBas);



            double xBarreMesure = xCle + 25;

            Line barreDeMesure = new Line
            {
                X1 = xBarreMesure,
                Y1 = positionYDebut,
                X2 = xBarreMesure,
                Y2 = positionYDebut + 4 * espacementLignes,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            PartitionCanvas.Children.Add(barreDeMesure);
        }

        // fonction pour ouvrir le navigateur sur la boite mail de l'utilisateur
        private void Contact_me(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:spprttexta@gmail.com");
        }

        private void Contact_me_whatsapp(object sender, RoutedEventArgs e)
        {
            string phoneNumber = "32492088904"; // Numéro de téléphone au format international
            string message = "Bonjour, j'aimerais vous contacter via WhatsApp à propos de TEXTA score edit PRO.";
            string url = $"https://wa.me/{phoneNumber}?text={Uri.EscapeDataString(message)}";
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture de WhatsApp : {ex.Message}");
            }
        }

        private void Preferences(object sender, RoutedEventArgs e)
        {
            Preferences prefWindow = new Preferences();
            prefWindow.Owner = Window.GetWindow(this); // Définit la fenêtre parente
            prefWindow.ShowDialog(); // Ouvre la fenêtre en mode modal
        }

        private void APropos(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TEXTA score edit PRO\nVersion 1.2\nDéveloppé par Ethan Macaluso\n© 2024 All rights reserved.", "À propos de TEXTA score edit", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void showHelpItem(object sender, RoutedEventArgs e)
        {
            // affichier le dockpanel à droite
            if (RightPanelFrame.Visibility != Visibility.Collapsed)
            {
                RightPanelFrame.Visibility = Visibility.Collapsed;
            }
            else
            {
                RightPanelFrame.Visibility = Visibility.Visible;
            }
        }
    }
}
