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

            string query2 = $"select CdR from client where Identifiant = \"{this.id_client}\" ;";
            liste = Commandes_SQL.Select_Requete(query2);
            if (liste[0][0] == "False")
            {
                CdR.Content = "Devenir CdR";
            }
            else
            {
                CdR.Content = "Page CdR";
            }

        }


        private void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            Recette_complete selection = Liste_Recette.SelectedItem as Recette_complete;

            Panier.Items.Add(new Recette_Panier { Nom_Recette = selection.Nom_Recette, Quantite_Recette = Quantité.Text });
        }
        private void Retirer_Click(object sender, RoutedEventArgs e)
        {
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
        public class Recette_complete
        {
            public string Nom_Recette { get; set; }

            public string Type { get; set; }

            public string Descriptif { get; set; }

            public string Prix { get; set; }

            public string test ()
            {
                return this.Nom_Recette;
            }
        }

        public class Recette_Panier
        {
            public string Nom_Recette { get; set; }

            public string Quantite_Recette { get; set; }
        }

        
    }
}
