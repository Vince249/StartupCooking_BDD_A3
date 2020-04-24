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
        }

        private void Client_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Deco_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
