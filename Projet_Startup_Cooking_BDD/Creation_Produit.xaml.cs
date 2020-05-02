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
    /// Logique d'interaction pour Creation_Produit.xaml
    /// </summary>
    public partial class Creation_Produit : Page
    {
        private string id_admin;

        /// <summary>
        /// Initialisation de la page 
        /// </summary>
        /// <param name="id_admin"> identifiant de l'admin connecté </param>
        public Creation_Produit( string id_admin)
        {
            InitializeComponent();
            this.id_admin = id_admin;
        }


        /// <summary>
        /// Méthode reliée au bouton "Créer le produit" qui crée un nouveau produit dans la database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Creer_Produit_Click(object sender, RoutedEventArgs e)
        {
            // récupération des input
            string nom_produit = textbox_nom.Text;
            string categorie = textbox_categorie.Text;
            string unite = textbox_unite.Text;
            string ref_fournisseur = textbox_ref_fournisseur.Text;
            string stock = textbox_stock.Text;
            string stock_min = textbox_stock_min.Text;
            string stock_max = textbox_stock_max.Text;

            // sécurité pour les input
            if (nom_produit == "" || nom_produit.Length > 50)
            {
                Erreur_message.Content = "Nom invalide (1-50 caractères)";
            }
            else if (categorie == "" || categorie.Length > 50)
            {
                Erreur_message.Content = "Categorie invalide (1-50 caractères)";
            }
            else if (unite == "" || unite.Length > 10)
            {
                Erreur_message.Content = "Unite invalide (1-10 caractères)";
            }
            else if (ref_fournisseur == "" || ref_fournisseur.Length > 50)
            {
                Erreur_message.Content = "Fournisseur invalide (1-50 caractères)";
            }
            else if (stock == "" || !int.TryParse(stock, out _))
            {
                Erreur_message.Content = "Stock invalide";
            }
            else if (stock_min == "" || !int.TryParse(stock_min, out _))
            {
                Erreur_message.Content = "Stock minimal invalide";
            }
            else if (stock_max == "" || !int.TryParse(stock_max, out _) || Convert.ToInt32(stock_max)<Convert.ToInt32(stock_min) ||Convert.ToInt32(stock_max) < Convert.ToInt32(stock))
            {
                Erreur_message.Content = "Stock maximal invalide";
            }
            else
            {
                Erreur_message.Content = "";

                //on vérifie si le fournisseur du produit qu'on veut créer existe dans notre database car Ref_Fournisseur = Foreign key de l'entité produit
                string query = "select Ref_Fournisseur from cooking.fournisseur;";
                List<List<string>> liste_Ref_fournisseur = Commandes_SQL.Select_Requete(query);

                bool ref_fournisseur_dans_BDD = false;
                for (int i = 0; i < liste_Ref_fournisseur.Count; i++)
                {
                    if (liste_Ref_fournisseur[i][0] == ref_fournisseur) ref_fournisseur_dans_BDD = true;
                }

                if (!ref_fournisseur_dans_BDD)
                {
                    Erreur_message.Content = "Fournisseur inconnu";
                }
                else
                {
                    // création du fournisseur dans la database à partir des input
                    query = $"insert into cooking.produit values (\"{nom_produit}\",\"{categorie}\",\"{unite}\",{stock},{stock_min},{stock_max},\"{ref_fournisseur}\")";
                    string ex = Commandes_SQL.Insert_Requete(query);

                    if (ex == $"Duplicate entry '{nom_produit}' for key 'produit.PRIMARY'") // si le nom_produit (clé primaire) existe déjà dans la database.produit
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
        }


        /// <summary>
        /// Méthode reliée au bouton "Reinitialiser" servant à clear toutes les textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Creer_Reinitialiser(object sender, RoutedEventArgs e)
        {
            // on navigue vers une nouvelle page Création_Produit pour réinitialiser les input
            Creation_Produit page_creation_produit = new Creation_Produit(this.id_admin);
            this.NavigationService.Navigate(page_creation_produit);
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
