using System;
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

        private void ShowMenu(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement)
            {
                ContextMenu menuBoutonActif = new ContextMenu();

                // =========================
                // MENU FICHIER
                // =========================
                MenuItem menuFichier = new MenuItem { Header = "Fichier" };
                MenuItem actionOuvrir = new MenuItem { Header = "Ouvrir un projet" };
                //actionOuvrir.Click += OuvrirFichier;
                MenuItem actionNouveauProjet = new MenuItem { Header = "Nouveau projet" };
                //actionNouveauProjet.Click += NouveauProjet;
                MenuItem actionFermerProjet = new MenuItem { Header = "Fermer le projet" };
                MenuItem actionNouvelleVersionProjet = new MenuItem { Header = "Nouvelle version du projet" };
                MenuItem actionEnregistrer = new MenuItem { Header = "Enregistrer" };
                //actionEnregistrer.Click += EnregistrerFichier;
                MenuItem actionEnregistrerSous = new MenuItem { Header = "Enregistrer sous" };
                //actionEnregistrerSous.Click += EnregistrerSous;
                MenuItem actionImprimer = new MenuItem { Header = "Imprimer la partition" };
                //actionImprimer.Click += ImprimerPartition;

                menuFichier.Items.Add(actionOuvrir);
                menuFichier.Items.Add(actionNouveauProjet);
                menuFichier.Items.Add(actionFermerProjet);
                menuFichier.Items.Add(actionNouvelleVersionProjet);
                menuFichier.Items.Add(actionEnregistrer);
                menuFichier.Items.Add(actionEnregistrerSous);
                menuFichier.Items.Add(actionImprimer);
                menuFichier.Items.Add(new Separator());

                MenuItem actionMettreEnLigne = new MenuItem { Header = "Mettre la partition en ligne" };
                MenuItem actionVideoEnLigne = new MenuItem { Header = "Créer une vidéo de partition en ligne" };
                menuFichier.Items.Add(actionMettreEnLigne);
                menuFichier.Items.Add(actionVideoEnLigne);
                menuFichier.Items.Add(new Separator());

                // Importer
                MenuItem menuImporter = new MenuItem { Header = "Importer" };
                MenuItem actionImporterAudio = new MenuItem { Header = "Fichier audio" };
                //actionImporterAudio.Click += ImporterAudio;
                MenuItem actionImporterMidi = new MenuItem { Header = "Fichier MIDI" };
                MenuItem actionImporterXml = new MenuItem { Header = "Fichier XML" };
                menuImporter.Items.Add(actionImporterAudio);
                menuImporter.Items.Add(actionImporterMidi);
                menuImporter.Items.Add(actionImporterXml);
                menuFichier.Items.Add(menuImporter);

                // Exporter
                MenuItem menuExporter = new MenuItem { Header = "Exporter" };
                menuExporter.Items.Add(new MenuItem { Header = "Fichier audio" });
                menuExporter.Items.Add(new MenuItem { Header = "Images" });
                menuExporter.Items.Add(new MenuItem { Header = "Fichier PDF" });
                menuExporter.Items.Add(new MenuItem { Header = "Fichier MIDI" });
                menuExporter.Items.Add(new MenuItem { Header = "Fichier XML" });
                menuFichier.Items.Add(menuExporter);

                menuFichier.Items.Add(new Separator());
                MenuItem actionPreferences = new MenuItem { Header = "Préférences" };
                actionPreferences.Click += Preferences;
                menuFichier.Items.Add(actionPreferences);

                menuFichier.Items.Add(new Separator());
                MenuItem actionQuitter = new MenuItem { Header = "Quitter" };
                //actionQuitter.Click += QuitterApplication;
                menuFichier.Items.Add(actionQuitter);

                menuBoutonActif.Items.Add(menuFichier);

                // =========================
                // MENU ÉDITION
                // =========================
                MenuItem menuEdition = new MenuItem { Header = "Édition" };
                MenuItem actionAnnuler = new MenuItem { Header = "Annuler" };
                //actionAnnuler.Click += Annuler;
                MenuItem actionRetablir = new MenuItem { Header = "Rétablir" };
                MenuItem actionHistorique = new MenuItem { Header = "Historique" };

                menuEdition.Items.Add(actionAnnuler);
                menuEdition.Items.Add(actionRetablir);
                menuEdition.Items.Add(actionHistorique);
                menuEdition.Items.Add(new Separator());

                menuEdition.Items.Add(new MenuItem { Header = "Copier" });
                menuEdition.Items.Add(new MenuItem { Header = "Couper" });
                menuEdition.Items.Add(new MenuItem { Header = "Coller" });
                menuEdition.Items.Add(new Separator());
                menuEdition.Items.Add(new MenuItem { Header = "Sélectionner" });
                menuEdition.Items.Add(new MenuItem { Header = "Tout sélectionner" });
                menuEdition.Items.Add(new MenuItem { Header = "Rechercher / Remplacer" });
                menuEdition.Items.Add(new MenuItem { Header = "Réorganiser" });
                menuEdition.Items.Add(new MenuItem { Header = "Mesures automatiques" });

                MenuItem actionAnnotationPerf = new MenuItem { Header = "Annotation de performance" };
                //actionAnnotationPerf.Click += AnnotationPerformance;
                menuEdition.Items.Add(actionAnnotationPerf);

                menuBoutonActif.Items.Add(menuEdition);

                // =========================
                // MENU MIDI
                // =========================
                MenuItem menuMIDI = new MenuItem { Header = "MIDI" };
                menuMIDI.Items.Add(new MenuItem { Header = "Ouvrir l’éditeur MIDI" });
                menuMIDI.Items.Add(new MenuItem { Header = "Ouvrir l’éditeur de rythme" });
                menuMIDI.Items.Add(new MenuItem { Header = "Éditeur de contrôles" });
                menuMIDI.Items.Add(new MenuItem { Header = "Enregistreur MIDI" });
                menuMIDI.Items.Add(new Separator());
                menuMIDI.Items.Add(new MenuItem { Header = "Transposer" });
                menuMIDI.Items.Add(new MenuItem { Header = "Paramètres MIDI" });
                menuBoutonActif.Items.Add(menuMIDI);

                // =========================
                // MENU PROJET
                // =========================
                MenuItem menuProjet = new MenuItem { Header = "Projet" };
                MenuItem actionNouvellePartition = new MenuItem { Header = "Nouvelle partition" };
                //actionNouvellePartition.Click += NouvellePartition;
                menuProjet.Items.Add(actionNouvellePartition);
                menuProjet.Items.Add(new MenuItem { Header = "Dupliquer la partition" });
                menuProjet.Items.Add(new MenuItem { Header = "Nouvelle version de la partition" });
                menuProjet.Items.Add(new MenuItem { Header = "Supprimer la partition" });
                menuProjet.Items.Add(new MenuItem { Header = "Supprimer toutes les partitions vides" });
                menuProjet.Items.Add(new Separator());
                menuProjet.Items.Add(new MenuItem { Header = "Bibliothèque" });
                menuProjet.Items.Add(new MenuItem { Header = "Marqueurs" });
                menuProjet.Items.Add(new MenuItem { Header = "Explorateur" });
                menuProjet.Items.Add(new MenuItem { Header = "Calculateur de tempo" });
                menuProjet.Items.Add(new MenuItem { Header = "Curseur timecode" });
                menuProjet.Items.Add(new MenuItem { Header = "Bloc-notes" });
                menuBoutonActif.Items.Add(menuProjet);

                // =========================
                // MENU DÉCHIFFRER
                // =========================
                MenuItem menuDechiffrer = new MenuItem { Header = "Déchiffrer" };
                menuDechiffrer.Items.Add(new MenuItem { Header = "Déchiffrer fichier audio" });
                menuDechiffrer.Items.Add(new MenuItem { Header = "Déchiffrer MIDI en direct" });
                menuDechiffrer.Items.Add(new MenuItem { Header = "Déchiffrer fichier MIDI" });
                menuDechiffrer.Items.Add(new MenuItem { Header = "Déchiffrer enregistrement audio" });
                menuDechiffrer.Items.Add(new Separator());
                menuDechiffrer.Items.Add(new MenuItem { Header = "Analyse harmonique" });
                menuDechiffrer.Items.Add(new MenuItem { Header = "Analyse de tonalité" });
                menuDechiffrer.Items.Add(new MenuItem { Header = "Scan déchiffrage de partition" });
                menuDechiffrer.Items.Add(new MenuItem { Header = "Correcteur de hauteur" });
                menuBoutonActif.Items.Add(menuDechiffrer);

                // =========================
                // MENU PARTITION
                // =========================
                MenuItem menuPartition = new MenuItem { Header = "Partition" };
                MenuItem actionOptionsPartition = new MenuItem { Header = "Options de partition" };
                //actionOptionsPartition.Click += OptionsPartition;
                menuPartition.Items.Add(actionOptionsPartition);
                menuPartition.Items.Add(new Separator());
                menuPartition.Items.Add(new MenuItem { Header = "Mode page" });
                menuPartition.Items.Add(new Separator());
                menuPartition.Items.Add(new MenuItem { Header = "Grouper / dégrouper les notes" });
                menuPartition.Items.Add(new MenuItem { Header = "Convertir en note ornementale" });
                menuPartition.Items.Add(new MenuItem { Header = "Définir N-olet" });
                menuPartition.Items.Add(new MenuItem { Header = "Éclater" });
                menuPartition.Items.Add(new Separator());
                menuPartition.Items.Add(new MenuItem { Header = "Fusionner les portées" });
                menuPartition.Items.Add(new MenuItem { Header = "Extraire la voix" });
                menuPartition.Items.Add(new MenuItem { Header = "Insérer un legato" });
                menuPartition.Items.Add(new MenuItem { Header = "Afficher / Masquer" });
                menuPartition.Items.Add(new MenuItem { Header = "Inverser" });
                menuBoutonActif.Items.Add(menuPartition);

                // =========================
                // MENU AIDE
                // =========================
                MenuItem menuAide = new MenuItem { Header = "Aide" };
                menuAide.Items.Add(new MenuItem { Header = "Documentation" });
                menuAide.Items.Add(new MenuItem { Header = "Foire aux questions" });
                menuAide.Items.Add(new Separator());
                menuAide.Items.Add(new MenuItem { Header = "Crédits" });
                MenuItem actionAPropos = new MenuItem { Header = "À propos de TEXTA score edit" };
                actionAPropos.Click += APropos;
                menuAide.Items.Add(actionAPropos);
                menuBoutonActif.Items.Add(menuAide);

                //menuBoutonActif.PlacementTarget = this;
                menuBoutonActif.IsOpen = true;
            }
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
