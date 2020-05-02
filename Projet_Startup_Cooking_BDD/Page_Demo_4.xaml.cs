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
    /// Logique d'interaction pour Page_Demo_4.xaml
    /// </summary>
    public partial class Page_Demo_4 : Page
    {
        /// <summary>
        /// Initialisation de Page_Demo_4, affichage du Nom, Stock actuel et Stock minimum des produits dont le Stock actuel est inférieur à 2*Stock minimal dans une List View
        /// </summary>
        public Page_Demo_4()
        {
            InitializeComponent();
            string query = "SELECT Nom_Produit, Stock, Stock_min FROM cooking.produit where Stock < (2*Stock_min);";
            List<List<string>> Liste_Nom_Stock_Stockmini = Commandes_SQL.Select_Requete(query);

            for (int i = 0; i < Liste_Nom_Stock_Stockmini.Count; i++)
            {
                Liste_Produit.Items.Add(new Produit { Nom = Liste_Nom_Stock_Stockmini[i][0], Stock = Liste_Nom_Stock_Stockmini[i][1], Stock_mini = Liste_Nom_Stock_Stockmini[i][2] });
            }
        }
        /// <summary>
        /// Méthode reliée au bouton "Afficher infos recette" permettant (en cliquant sur un produit au préalable) d'afficher dans une autre ListView les informations relatives aux recettes utilisant ce produit
        /// Notamment leur nom et la quantité de ce produit qu'elles utilisent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Recette(object sender, RoutedEventArgs e)
        {
            Liste_Recette.Items.Clear();
            Produit selection = Liste_Produit.SelectedItem as Produit;
            string nom = selection.Nom;
            string query = $"SELECT Nom_Recette, Quantite_Produit FROM cooking.composition_recette where Nom_Produit = \"{nom}\";";
            List<List<string>> Liste_Nom_Qt = Commandes_SQL.Select_Requete(query);

            for (int i = 0; i < Liste_Nom_Qt.Count; i++)
            {
                Liste_Recette.Items.Add(new Recette { Nom = Liste_Nom_Qt[i][0], Qt = Liste_Nom_Qt[i][1] });
            }
        }
        /// <summary>
        /// Classe permettant de remplir la ListView des produits
        /// </summary>
        public class Produit
        {
            /// <summary>
            /// Nom du Produit
            /// </summary>
            public string Nom { get; set; }
            /// <summary>
            /// Stock du Produit
            /// </summary>
            public string Stock { get; set; }
            /// <summary>
            /// Stock minimal du Produit
            /// </summary>
            public string Stock_mini { get; set; }
        }
        /// <summary>
        /// Classe permettant de remplir la ListView des recettes
        /// </summary>
        public class Recette
        {
            /// <summary>
            /// Nom de la Recette
            /// </summary>
            public string Nom { get; set; }
            /// <summary>
            /// Quantité de la Recette
            /// </summary>
            public string Qt { get; set; }
        }
        /// <summary>
        /// Méthode reliée au bouton "Quitter mode démo" permettant de revenir au menu principal
        /// </summary>
        /// <param name="sender">Bouton "Quitter mode démo"</param>
        /// <param name="e">Evenement Click</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Interface_Home inter = new Interface_Home();
            this.NavigationService.Navigate(inter);
        }
    }
}
