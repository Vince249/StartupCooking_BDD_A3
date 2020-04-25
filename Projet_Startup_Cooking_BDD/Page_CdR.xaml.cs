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
    /// Logique d'interaction pour Page_CdR.xaml
    /// </summary>
    public partial class Page_CdR : Page
    {
        private string id_client;
        public Page_CdR(string id_client)
        {
            InitializeComponent();
            this.id_client = id_client;
            string query = $"Select Nom_Client from cooking.client where Identifiant = \"{this.id_client}\" ;";
            List<List<string>> liste = Commandes_SQL.Select_Requete(query);
            welcome_message.Content = "Bonjour " + liste[0][0];

            query = $"select Credit_Cook from client where Identifiant = \"{this.id_client}\" ;";
            liste = Commandes_SQL.Select_Requete(query);
            Solde.Content = liste[0][0];

            //Ajout produit dans la listView associé
            query = $"select Nom_Produit,Unite from cooking.produit;";
            List<List<string>> liste_produit_nom_unite = Commandes_SQL.Select_Requete(query);
            for (int i = 0; i < liste_produit_nom_unite.Count; i++)
            {
                Produits_ListView.Items.Add(new Produit { Nom_Produit = liste_produit_nom_unite[i][0], Unite = liste_produit_nom_unite[i][1] });
            }

            //Ajout recette CdR dans la listView associé
            query = $"select Nom_Recette,Compteur from cooking.recette where Identifiant = \"{this.id_client}\";";
            List<List<string>> liste_recette_nom_compteur = Commandes_SQL.Select_Requete(query);
            for (int i = 0; i < liste_recette_nom_compteur.Count; i++)
            {
                Recettes_CdR_ListView.Items.Add(new Recette { Nom_Recette = liste_recette_nom_compteur[i][0], Compteur = liste_recette_nom_compteur[i][1] });
            }
        }

        private void Client_Click(object sender, RoutedEventArgs e)
        {
            Page_Client page_client = new Page_Client(this.id_client);
            this.NavigationService.Navigate(page_client);
        }

        private void Deco_Click(object sender, RoutedEventArgs e)
        {
            Interface_Home interhome = new Interface_Home();
            this.NavigationService.Navigate(interhome);
        }

        private void Creer_Recette_Click(object sender, RoutedEventArgs e)
        {
            //récupération infos rentrées par le user
            string nom_recette_input = Nom_Recette_Box.Text;
            string type_recette_input = Type_Recette_Box.Text;
            string prix_recette_input = Prix_Recette_Box.Text;
            string description_recette_input = Description_Recette_Box.Text;
            List<List<string>> liste_produit_nom_quantite = new List<List<string>>();

            for (int i = 0; i < Produits_ListView.Items.Count; i++)
            {
                Produit produit_observee = Produits_ListView.Items[i] as Produit;
                if(produit_observee.Quantite != null)
                {
                    List<string> temp = new List<string>();
                    temp.Add(produit_observee.Nom_Produit);
                    temp.Add(produit_observee.Quantite);
                    liste_produit_nom_quantite.Add(temp);
                }
            }

            if (nom_recette_input == "" && type_recette_input != "" && prix_recette_input != "" && description_recette_input != "" && liste_produit_nom_quantite.Count != 0)
            {
                Erreur_Message.Content = "Aucun nom rentré";
            }
            else if (nom_recette_input != "" && type_recette_input == "" && prix_recette_input != "" && description_recette_input != "" && liste_produit_nom_quantite.Count != 0)
            {
                Erreur_Message.Content = "Aucun type rentré";
            }
            else if (nom_recette_input != "" && type_recette_input != "" && prix_recette_input == "" && description_recette_input != "" && liste_produit_nom_quantite.Count != 0)
            {
                Erreur_Message.Content = "Aucun prix rentré";
            }
            else if (nom_recette_input != "" && type_recette_input != "" && prix_recette_input != "" && description_recette_input == "" && liste_produit_nom_quantite.Count != 0)
            {
                Erreur_Message.Content = "Aucune description rentrée";
            }
            else if (nom_recette_input != "" && type_recette_input != "" && prix_recette_input != "" && description_recette_input != "" && liste_produit_nom_quantite.Count == 0)
            {
                Erreur_Message.Content = "Aucun produit ajouté";
            }
            else if (nom_recette_input != "" && type_recette_input != "" && prix_recette_input != "" && description_recette_input != "" && liste_produit_nom_quantite.Count != 0)
            {
                Erreur_Message.Content = "";

                string query1 = $"INSERT INTO cooking.recette VALUES (\"{nom_recette_input}\",\"{type_recette_input}\",\"{description_recette_input}\",\"{prix_recette_input}\",2,0,\"{this.id_client}\");";
                string query2 = "";
                for (int i = 0; i < liste_produit_nom_quantite.Count; i++)
                {
                    query2 += $"INSERT INTO cooking.composition_recette VALUES (\"{nom_recette_input}\",\"{liste_produit_nom_quantite[i][0]}\",{liste_produit_nom_quantite[i][1]});";
                }
                string query = query1 + query2;
                string ex = Commandes_SQL.Insert_Requete(query);

                Page_CdR page_CdR = new Page_CdR(this.id_client);
                this.NavigationService.Navigate(page_CdR);
            }
            else
            {
                Erreur_Message.Content = "Plusieurs champs sont incomplets";
            }
        }

        private void Supprimer_Recette_Click(object sender, RoutedEventArgs e)
        {
            Recette selection = Recettes_CdR_ListView.SelectedItem as Recette;
            if (selection == null)
            {
                Erreur_Message.Content = "Aucune recette sélectionnée";
            }
            else
            {
                Erreur_Message.Content = "";

                //delete child rows
                string query1 = $"delete from cooking.composition_recette where Nom_Recette = \"{selection.Nom_Recette}\";";
                string query2 = $"delete from cooking.composition_commande where Nom_Recette = \"{selection.Nom_Recette}\";";
                
                //delete parent row
                string query3 = $"delete from cooking.recette where Nom_Recette = \"{selection.Nom_Recette}\";";

                //final query
                string query = query1 + query2 + query3;
                string ex = Commandes_SQL.Insert_Requete(query);

                //update listView
                List<Recette> liste_nv_Item = new List<Recette>();
                for (int i = 0; i < Recettes_CdR_ListView.Items.Count; i++)
                {
                    Recette recette_observee = Recettes_CdR_ListView.Items[i] as Recette;
                    if (recette_observee.Nom_Recette != selection.Nom_Recette)
                    {
                        Recette copie_recette = new Recette{ Nom_Recette = recette_observee.Nom_Recette, Compteur = recette_observee.Compteur};
                        liste_nv_Item.Add(copie_recette);
                    }
                }
                Recettes_CdR_ListView.Items.Clear();
                for (int i = 0; i < liste_nv_Item.Count; i++)
                {
                    Recettes_CdR_ListView.Items.Add(liste_nv_Item[i]);
                }
            }
        }

        private void Ajout_Produit_Click(object sender, RoutedEventArgs e)
        {
            string quantite_input = Quantite_Produit_Box.Text;
            Produit selection = Produits_ListView.SelectedItem as Produit;

            if (selection == null)
            {
                Erreur_Message.Content = "Aucun produit selectionné";
            }
            if (selection != null && quantite_input == "")
            {
                Erreur_Message.Content = "Aucune quantité indiquée";
            }
            if (selection != null && quantite_input != "")
            {
                Erreur_Message.Content = "";

                List<Produit> liste_nv_Item = new List<Produit>();
                for (int i = 0; i < Produits_ListView.Items.Count; i++)
                {
                    Produit produit_observee = Produits_ListView.Items[i] as Produit;
                    if (selection.Nom_Produit == produit_observee.Nom_Produit)
                    {
                        Produit copie_produit = new Produit { Nom_Produit = produit_observee.Nom_Produit, Quantite = quantite_input, Unite = produit_observee.Unite };
                        liste_nv_Item.Add(copie_produit);
                    }
                    else
                    {
                        Produit copie_produit = new Produit { Nom_Produit = produit_observee.Nom_Produit, Quantite = produit_observee.Quantite, Unite = produit_observee.Unite };
                        liste_nv_Item.Add(copie_produit);
                    }
                }
                Produits_ListView.Items.Clear();
                for (int i = 0; i < liste_nv_Item.Count; i++)
                {
                    Produits_ListView.Items.Add(liste_nv_Item[i]);
                }
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Page_CdR page_CdR = new Page_CdR(this.id_client);
            this.NavigationService.Navigate(page_CdR);
        }

        public class Produit
        {
            public string Nom_Produit { get; set; }

            public string Unite { get; set; }

            public string Quantite { get; set; }
        }

        public class Recette
        {
            public string Nom_Recette { get; set; }

            public string Compteur { get; set; }
        }
    }
}
