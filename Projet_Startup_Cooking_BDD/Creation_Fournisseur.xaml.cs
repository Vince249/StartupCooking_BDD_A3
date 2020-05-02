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

namespace Projet_Startup_Cooking_BDD
{
    /// <summary>
    /// Logique d'interaction pour Creation_Fournisseur.xaml
    /// </summary>
    public partial class Creation_Fournisseur : Page
    {
        private string id_admin;

        /// <summary>
        /// Initialisation de la page 
        /// </summary>
        /// <param name="id_admin"> identifiant de l'admin connecté </param>
        public Creation_Fournisseur(string id_admin)
        {
            InitializeComponent();
            this.id_admin = id_admin;
        }


        /// <summary>
        /// Méthode reliée au bouton "Créer le fournisseur" qui crée un nouveau fournisseur dans la database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Creer_Fournisseur_Click(object sender, RoutedEventArgs e)
        {
            // récupération des input
            string nom = textbox_nom.Text;
            string refe = textbox_ref.Text;
            string tel = textbox_tel.Text;

            // sécurité pour les input
            if (nom == "" || nom.Length > 50)
            {
                Erreur_message.Content = "Nom invalide (1-50 caractères)";
            }
            else if (refe == "" || refe.Length > 50)
            {
                Erreur_message.Content = "Référence invalide (1-50 caractères)";
            }
            else if (tel == "" || !int.TryParse(tel, out _) || tel.Length > 15)
            {
                Erreur_message.Content = "Téléphone invalide (1-15 caractères)";
            }
            else
            {
                Erreur_message.Content = "";

                // création du fournisseur dans la database à partir des input
                string query = $"insert into cooking.fournisseur VALUES(\"{refe}\",\"{nom}\",\"{tel}\");";
                string ex = Commandes_SQL.Insert_Requete(query);
                if (ex == $"Duplicate entry '{refe}' for key 'fournisseur.PRIMARY'") // si la ref_fournisseur (clé primaire) existe déjà dans la database.fournisseur
                {
                    Erreur_message.Content = "Nom déjà utilisé";
                }
                else
                {
                    // on navigue vers la page Admin
                    Page_Admin page_admin = new Page_Admin(this.id_admin);
                    this.NavigationService.Navigate(page_admin);
                }
                
            }

        }


        /// <summary>
        ///  Méthode reliée au bouton "Reinitialiser" servant à clear toutes les textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Creer_Reinitialiser(object sender, RoutedEventArgs e)
        {
            // on navigue vers une nouvelle page Création_Fournisseur pour réinitialiser les input
            Creation_Fournisseur page_creation_fournisseur = new Creation_Fournisseur(this.id_admin);
            this.NavigationService.Navigate(page_creation_fournisseur);
        }


        /// <summary>
        /// Méthode permettant d'interdire certains caractères pour les input (caratères provoquant des erreurs sur MySQL)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Caractere_interdit(object sender, TextChangedEventArgs e)
        {
            TextBox id_textbox = sender as TextBox;
            if (id_textbox.Text.Contains('"') || id_textbox.Text.Contains('é') || id_textbox.Text.Contains('è')
                || id_textbox.Text.Contains('î') || id_textbox.Text.Contains('ê') || id_textbox.Text.Contains('ô')
                || id_textbox.Text.Contains('ï') || id_textbox.Text.Contains('ë') || id_textbox.Text.Contains('ç')
                || id_textbox.Text.Contains('à') || id_textbox.Text.Contains('ù'))
            {
                // on efface le dernier caractère s'il est interdit, ne laissant ainsi pas l'opportunité à l'utilisateur de l'écrire
                string a = id_textbox.Text.Remove(id_textbox.Text.Length - 1);
                id_textbox.Text = a;
                Erreur_message.Content = "Accents/Guillemets interdits";
                
            }
            return;
        }


    }
}
