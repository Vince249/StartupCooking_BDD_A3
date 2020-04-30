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
            
            if (nom_produit == "")
            {
                Erreur_message.Content = "Nom invalide";
            }
            else if (categorie == "")
            {
                Erreur_message.Content = "Categorie invalide";
            }
            else if (unite == "")
            {
                Erreur_message.Content = "Unite invalide";
            }
            else if (ref_fournisseur == "")
            {
                Erreur_message.Content = "Fournisseur invalide";
            }
            else
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
                    Erreur_message.Content = "Fournisseur inconnu";
                }
                else
                {
                    query = $"insert into cooking.produit values (\"{nom_produit}\",\"{categorie}\",\"{unite}\",0,0,0,\"{ref_fournisseur}\")";
                    string ex = Commandes_SQL.Insert_Requete(query);

                    if (ex == $"Duplicate entry '{nom_produit}' for key 'produit.PRIMARY'")
                    {
                        Erreur_message.Content = "Nom déjà utilisé";
                    }
                    else
                    {
                        Creation_Produit page_creation_produit = new Creation_Produit();
                        this.NavigationService.Navigate(page_creation_produit);
                    }
                }
            }
        }

        private void Creer_Reinitialiser(object sender, RoutedEventArgs e)
        {
            Creation_Produit page_creation_produit = new Creation_Produit();
            this.NavigationService.Navigate(page_creation_produit);
        }


        private void Caractere_interdit(object sender, TextChangedEventArgs e)
        {
            TextBox id_textbox = sender as TextBox;
            // \s - Stands for white space. The rest is for alphabets and numbers
            if (id_textbox.Text.Contains('"'))
            {
                id_textbox.Text = String.Empty;
                Erreur_message.Content = "Guillemets (\") interdits";
            }
            return;
        }
    }
}
