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
    /// Logique d'interaction pour Page_Demo_1.xaml
    /// </summary>
    public partial class Page_Demo_1 : Page
    {
        /// <summary>
        /// Initialisation de la Page_Demo_1, affiche le nombre de client
        /// </summary>
        public Page_Demo_1()
        {
            InitializeComponent();
            string query = "Select count(*) from cooking.client";
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
            Page_Demo_2 page_demo_2 = new Page_Demo_2();
            this.NavigationService.Navigate(page_demo_2);
        }
    }
}
