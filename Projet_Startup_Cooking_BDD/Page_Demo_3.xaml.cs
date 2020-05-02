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
    /// Logique d'interaction pour Page_Demo_3.xaml
    /// </summary>
    public partial class Page_Demo_3 : Page
    {
        /// <summary>
        /// Initialisation de la Page_Demo_3, affichage du nombre de recettes créées
        /// </summary>
        public Page_Demo_3()
        {
            InitializeComponent();
            string query = "Select count(*) from cooking.recette";
            List<List<string>> Liste_Nb = Commandes_SQL.Select_Requete(query);
            Nb.Content = Liste_Nb[0][0];
        }
        /// <summary>
        /// Méthode reliée au bouton "Suivant" permettant de passer à la page de démo suivante
        /// </summary>
        /// <param name="sender">Bouton "Suivant"</param>
        /// <param name="e">Evenement Click</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Page_Demo_4 page_demo_4 = new Page_Demo_4();
            this.NavigationService.Navigate(page_demo_4);
        }
    }
}
