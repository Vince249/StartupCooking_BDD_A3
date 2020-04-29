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
        public Creation_Produit()
        {
            InitializeComponent();
        }

        private void Creer_Produit_Click(object sender, RoutedEventArgs e)
        {
            string nom_produit = textbox_nom.Text;
            string categorie = textbox_categorie.Text;
            string unite = textbox_unite.Text;
            string ref_fournisseur = textbox_ref_fournisseur.Text;
            string stock = textbox_stock.Text;
            string stock_min = textbox_stock_min.Text;
            string stock_max = textbox_stock_max.Text;
            
            if (nom_produit == "" && categorie != "" && unite != "" && ref_fournisseur != "" && stock != "" && stock_min != "" && stock_max != "")
            {
                Erreur_message.Content = "Erreur nom";
            }
            else if (nom_produit != "" && categorie == "" && unite != "" && ref_fournisseur != "" && stock != "" && stock_min != "" && stock_max != "")
            {
                Erreur_message.Content = "Erreur categorie";
            }
            else if (nom_produit != "" && categorie != "" && unite == "" && ref_fournisseur != "" && stock != "" && stock_min != "" && stock_max != "")
            {
                Erreur_message.Content = "Erreur unite";
            }
            else if (nom_produit != "" && categorie != "" && unite != "" && ref_fournisseur == "" && stock != "" && stock_min != "" && stock_max != "")
            {
                Erreur_message.Content = "Erreur fournisseur";
            }
            else if (nom_produit != "" && categorie != "" && unite != "" && ref_fournisseur != "" && stock == "" && stock_min != "" && stock_max != "")
            {
                Erreur_message.Content = "Erreur stock";
            }
            else if (nom_produit != "" && categorie != "" && unite != "" && ref_fournisseur != "" && stock != "" && stock_min == "" && stock_max != "")
            {
                Erreur_message.Content = "Erreur stock min";
            }
            else if (nom_produit != "" && categorie != "" && unite != "" && ref_fournisseur != "" && stock != "" && stock_min != "" && stock_max == "")
            {
                Erreur_message.Content = "Erreur stock max";
            }
            else if (nom_produit != "" && categorie != "" && unite != "" && ref_fournisseur != "" && stock != "" && stock_min != "" && stock_max != "")
            {
                Erreur_message.Content = "";

                //on vérifie si le fournisseur du produit qu'on veut créer existe dans notre BDD
                string query = "select Ref_Fournisseur from cooking.fournisseur;";
                List<List<string>> liste_Ref_fournisseur = Commandes_SQL.Select_Requete(query);

                bool ref_fournisseur_dans_BDD = false;
                for (int i = 0; i < liste_Ref_fournisseur.Count; i++)
                {
                    if (liste_Ref_fournisseur[i][0] == ref_fournisseur) ref_fournisseur_dans_BDD = true;
                }

                if (!ref_fournisseur_dans_BDD)
                {
                    Erreur_message.Content = "Erreur fournisseur inconnu";
                }
                else if (Convert.ToInt32(stock_min) > Convert.ToInt32(stock_max))
                {
                    Erreur_message.Content = "Combinaison impossible de stock min et max";
                }
                else
                {
                    query = $"insert into cooking.produit values (\"{nom_produit}\",\"{categorie}\",\"{unite}\",{stock},{stock_min},{stock_max},\"{ref_fournisseur}\")";
                    string ex = Commandes_SQL.Insert_Requete(query);

                    Creation_Produit page_creation_produit = new Creation_Produit();
                    this.NavigationService.Navigate(page_creation_produit);
                }
            }
            else
            {
                Erreur_message.Content = "Plusieurs champs sont incomplets";
            }

        }

        private void Creer_Reinitialiser(object sender, RoutedEventArgs e)
        {
            Creation_Produit page_creation_produit = new Creation_Produit();
            this.NavigationService.Navigate(page_creation_produit);
        }
    }
}
