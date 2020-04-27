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
    /// Logique d'interaction pour Page_Top.xaml
    /// </summary>
    public partial class Page_Top : Page
    {
        public Page_Top()
        {
            InitializeComponent();

            //On peut déjà mettre les informations du CdR d'Or

            //On recherche déjà son nom
            //On peut directement chercher dans Composition_Commande vu qu'on va prendre toutes les commandes
        }

        private void Semaine_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
