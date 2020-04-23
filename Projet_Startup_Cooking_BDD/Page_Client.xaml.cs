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
    /// Logique d'interaction pour Page_Client.xaml
    /// </summary>
    public partial class Page_Client : Page
    {
        private string id_client;
        public Page_Client(string id_client)
        {
            InitializeComponent();
            this.id_client = id_client;
            string query = $"Select Nom_Client from cooking.client where Identifiant = \"{this.id_client}\" ;";
            List<List<string>> liste = Commandes_SQL.Select_Requete(query);
            welcome_message.Content = "Bonjour "+ liste[0][0];

            // Ajout de toutes les recettes
            List<List<string>> recette = new List<List<string>>();
            query = "Select Nom_Recette, Type, Prix_Vente, Descriptif from cooking.recette;";
            recette = Commandes_SQL.Select_Requete(query);

            for (int i = 0; i < recette.Count(); i++)
            {
                Liste_Recette.Items.Add(new Recette_complete { Nom_Recette = recette[i][0], Type = recette[i][1], Descriptif= recette[i][3], Prix = recette[i][2] });
            }

            string query2 = $"select CdR,Credit_Cook from client where Identifiant = \"{this.id_client}\" ;";
            liste = Commandes_SQL.Select_Requete(query2);
            if (liste[0][0] == "False")
            {
                CdR.Content = "Devenir CdR";
            }
            else
            {
                CdR.Content = "Page CdR";
            }

            Solde.Content = liste[0][1];

        }


        private void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            Recette_complete selection = Liste_Recette.SelectedItem as Recette_complete;

            int ajout = Convert.ToInt32(selection.Prix) * Convert.ToInt32(Quantité.Text);
            Total.Content = Convert.ToString(Convert.ToInt32(Total.Content) + ajout);

            Panier.Items.Add(new Recette_Panier { Nom_Recette = selection.Nom_Recette, Quantite_Recette = Quantité.Text , Prix = selection.Prix});
        }
        private void Retirer_Click(object sender, RoutedEventArgs e)
        {
            Recette_Panier selection = Panier.SelectedItem as Recette_Panier;
            int retrait = Convert.ToInt32(selection.Prix) * Convert.ToInt32(selection.Quantite_Recette);
            Total.Content = Convert.ToString(Convert.ToInt32(Total.Content) - retrait);
            Panier.Items.Remove(Panier.SelectedItem);
        }
        private void CdR_Click(object sender, RoutedEventArgs e)
        {
            string query = $"select CdR from client where Identifiant = \"{this.id_client}\" ;";
            List<List<string>> liste = Commandes_SQL.Select_Requete(query);
            if(liste[0][0] == "False")
            {
                string query2 = $"Update cooking.client set CdR = True where Identifiant = \"{this.id_client}\" ;";
                string ex = Commandes_SQL.Insert_Requete(query2);
                CdR.Content = "Page CdR";
            }
            else
            {
                //this.NavigationService.Navigate(Page_CdR(this.id_client));
            }
        }

        private void Deco_Click(object sender, RoutedEventArgs e)
        {
            Interface_Home interhome = new Interface_Home();
            this.NavigationService.Navigate(interhome);
        }

        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            List<List<string>> liste_panier = new List<List<string>>();
            for (int index_panier = 0;  index_panier < Panier.Items.Count;  index_panier++)
            {
                Recette_Panier recette = Panier.Items[index_panier] as Recette_Panier;
                List<string> liste_recette_quantite = new List<string>();
                liste_recette_quantite.Add(recette.Nom_Recette);
                liste_recette_quantite.Add(recette.Quantite_Recette);
                liste_panier.Add(liste_recette_quantite);
            }

            
            Validation_Paiement page_validation = new Validation_Paiement(Total.Content.ToString(),Solde.Content.ToString(),this.id_client, liste_panier);
            this.NavigationService.Navigate(page_validation);
        }
        public class Recette_complete
        {
            public string Nom_Recette { get; set; }

            public string Type { get; set; }

            public string Descriptif { get; set; }

            public string Prix { get; set; }
        }

        public class Recette_Panier
        {
            public string Nom_Recette { get; set; }

            public string Quantite_Recette { get; set; }

            public string Prix { get; set; }
        }

        
    }
}
