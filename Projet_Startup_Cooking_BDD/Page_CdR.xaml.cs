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

        }

        private void Deco_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Creer_Recette_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Supprimer_Recette_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Ajout_Produit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {

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
