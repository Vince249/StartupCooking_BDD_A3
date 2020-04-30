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

        public class Produit
        {
            public string Nom { get; set; }
            public string Stock { get; set; }
            public string Stock_mini { get; set; }
        }

        public class Recette
        {
            public string Nom { get; set; }
            public string Qt { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Interface_Home inter = new Interface_Home();
            this.NavigationService.Navigate(inter);
        }
    }
}
