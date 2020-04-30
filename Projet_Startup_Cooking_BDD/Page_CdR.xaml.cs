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

            if (nom_recette_input == "" || nom_recette_input.Length>50)
            {
                Erreur_Message.Content = "Nom doit contenir 1 à 50 caractères";
            }
            else if (type_recette_input == "" || !type_recette_input.All(Char.IsLetter))
            {
                Erreur_Message.Content = "Type invalide";
            }
            else if (prix_recette_input == ""|| !int.TryParse(prix_recette_input,out _) || Convert.ToInt32(prix_recette_input)>40|| Convert.ToInt32(prix_recette_input) <10)
            {
                Erreur_Message.Content = "Prix doit être entre 10 et 40";
            }
            else if (description_recette_input == "")
            {
                Erreur_Message.Content = "Aucune description rentrée";
            }
            else if (liste_produit_nom_quantite.Count == 0)
            {
                Erreur_Message.Content = "Aucun produit ajouté";
            }
            else
            {
                Erreur_Message.Content = "";

                string query1 = $"INSERT INTO cooking.recette VALUES (\"{nom_recette_input}\",\"{type_recette_input}\",\"{description_recette_input}\",\"{prix_recette_input}\",2,0,\"{this.id_client}\");";
                string query2 = "";
                string query4 = "";
                for (int i = 0; i < liste_produit_nom_quantite.Count; i++)
                {
                    query2 += $"INSERT INTO cooking.composition_recette VALUES (\"{nom_recette_input}\",\"{liste_produit_nom_quantite[i][0]}\",{liste_produit_nom_quantite[i][1]});";
                    
                    // mise à jour des sotck min et max des produits
                    // on a décidé d'augmenter stock min de 1 fois la quantité nécessaire pour cette recette (choix expliqué dans le rapport)
                    // stock max est augmenté de 2 fois la quantité nécessaire pour cette recette
                    string query3 = $"Select Stock_min, Stock_max from cooking.produit where Nom_Produit = \"{liste_produit_nom_quantite[i][0]}\" ;";
                    List<List<string>> Liste_stock_min_max = Commandes_SQL.Select_Requete(query3);
                    int Nv_Stock_min =  Convert.ToInt32(Liste_stock_min_max[0][0]) + Convert.ToInt32(liste_produit_nom_quantite[i][1]);
                    int Nv_Stock_max = Convert.ToInt32(Liste_stock_min_max[0][1]) + 2*Convert.ToInt32(liste_produit_nom_quantite[i][1]);
                    query4 += $"Update cooking.produit set Stock_min = {Nv_Stock_min}, Stock_max = {Nv_Stock_max} where Nom_Produit = \"{liste_produit_nom_quantite[i][0]}\" ;";

                }
                string query = query1 + query2 + query4; //Si la recette porte le même nom qu'une recette existante, l'exécution plante à la query1. Cela empêche l'exécution des query 2 et 4
                string ex = Commandes_SQL.Insert_Requete(query);
                if(ex== $"Duplicate entry '{nom_recette_input}' for key 'recette.PRIMARY'")
                {
                    Erreur_Message.Content = "Nom déjà utilisé";
                }
                else
                {
                    Page_CdR page_CdR = new Page_CdR(this.id_client);
                    this.NavigationService.Navigate(page_CdR);
                }
               
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

                //mettre à jour les stock mini et max de produit
                //on recupère les infos relatives au produit dont on a besoin
                string query = $"Select Nom_Produit,Quantite_Produit,Stock_min,Stock_max from cooking.composition_recette natural join cooking.produit where Nom_Recette = \"{selection.Nom_Recette}\";";
                List<List<string>> Liste_Nom_Produit_QT_Min_Max = Commandes_SQL.Select_Requete(query);

                string query5 = "";
                for (int i = 0; i < Liste_Nom_Produit_QT_Min_Max.Count; i++)
                {
                    string nom_produit_observe = Liste_Nom_Produit_QT_Min_Max[i][0];
                    int quantite_necessaire_dans_recette = Convert.ToInt32(Liste_Nom_Produit_QT_Min_Max[i][1]);
                    int stock_min = Convert.ToInt32(Liste_Nom_Produit_QT_Min_Max[i][2]);
                    int stock_max = Convert.ToInt32(Liste_Nom_Produit_QT_Min_Max[i][3]);

                    //on adapte les stocks de la même manière que lorsque qu'une recette est créée, sauf qu'ici, le stock est impacté négativement
                    int nv_stock_min = stock_min - quantite_necessaire_dans_recette;
                    int nv_stock_max = stock_max - 2*quantite_necessaire_dans_recette;

                    query5 += $"Update cooking.produit set Stock_min = {nv_stock_min}, Stock_max = {nv_stock_max} where Nom_Produit = \"{nom_produit_observe}\";";
                }
                string ex = Commandes_SQL.Insert_Requete(query5);



                //delete child rows
                string query1 = $"delete from cooking.composition_recette where Nom_Recette = \"{selection.Nom_Recette}\";";
                string query2 = $"delete from cooking.composition_commande where Nom_Recette = \"{selection.Nom_Recette}\";";
                string query3 = $"delete from cooking.palmares_recette where Nom_Recette = \"{selection.Nom_Recette}\";";

                //delete parent row
                string query4 = $"delete from cooking.recette where Nom_Recette = \"{selection.Nom_Recette}\";";

                //final query
                query = query1 + query2 + query3 + query4;
                ex = Commandes_SQL.Insert_Requete(query);

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
            else if(quantite_input == "" || !int.TryParse(quantite_input,out _)|| Convert.ToInt32(quantite_input)<=0)
            {
                Erreur_Message.Content = "Quantité invalide";
            }
            else
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
