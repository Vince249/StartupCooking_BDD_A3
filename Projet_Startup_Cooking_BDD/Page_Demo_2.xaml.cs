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
    /// Logique d'interaction pour Page_Demo_2.xaml
    /// </summary>
    public partial class Page_Demo_2 : Page
    {
        /// <summary>
        /// Initialisation de la Page_Demo_2, affiche le nombre de CdR et une ListView contenant la liste pour chaque CdR de leur Identifant, Nom et le volume que représente les commandes de leurs recettes
        /// </summary>
        public Page_Demo_2()
        {
            InitializeComponent();

            string query = "Select Nom_Client, Identifiant from cooking.client where CdR = \"1\";";
            List<List<string>> Liste_Nom_Id = Commandes_SQL.Select_Requete(query);
            Nb_CdR.Content = Liste_Nom_Id.Count;

            for (int i = 0; i < Liste_Nom_Id.Count; i++)
            {
                string nom = Liste_Nom_Id[i][0];
                string id = Liste_Nom_Id[i][1];
                query = $"SELECT sum(Compteur) FROM cooking.recette where Identifiant = \"{id}\" ;";
                List<List<string>> Liste_Qt = Commandes_SQL.Select_Requete(query);
                string qt = Liste_Qt[0][0];
                Liste_CdR.Items.Add(new Nom_QT { Nom = nom, Qt = qt , Identifiant=id});
            }   
            
        }
        /// <summary>
        /// Classe utilisée pour remplir la ListView avec Nom, Identifiant et volume de commande de ses recettes pour chaque CdR
        /// </summary>
        public class Nom_QT
        {
            /// <summary>
            /// Nom du CdR
            /// </summary>
            public string Nom { get; set; }
            /// <summary>
            /// Identifiant du CdR
            /// </summary>
            public string Identifiant { get; set; }
            /// <summary>
            /// volume de commande de ses recettes
            /// </summary>
            public string Qt { get; set; }
        }
        /// <summary>
        /// Méthode reliée au bouton "Suivant" permettant de passer à la page de démo suivante
        /// </summary>
        /// <param name="sender">Bouton "Suivant"</param>
        /// <param name="e">Evenement Click</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Page_Demo_3 page_demo_3 = new Page_Demo_3();
            this.NavigationService.Navigate(page_demo_3);
        }
    }
}
