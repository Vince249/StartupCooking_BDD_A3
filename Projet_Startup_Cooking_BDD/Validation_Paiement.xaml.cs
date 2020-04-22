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
    /// Logique d'interaction pour Validation_Paiement.xaml
    /// </summary>
    public partial class Validation_Paiement : Page
    {
        private string id_client;
        public Validation_Paiement(string total, string solde, string id_client)
        {
            InitializeComponent();
            this.id_client = id_client;
            Total.Content = total + " cook(s)";
            Solde.Content = solde + " cook(s)";
            int difference = Convert.ToInt32(solde) - Convert.ToInt32(total);
            if(difference>=0)
            {
                Reste.Content = "0 cook";
                Nv_Solde.Content = Convert.ToString(difference) + " cook(s)";
                Valider.Content = "Confirmer";
            }
            else
            {
                Nv_Solde.Content = "0 cook";
                Reste.Content = Convert.ToString(Math.Abs(difference)) + " cook(s)";
                Valider.Content = "Payer le reste par CB";
            }


        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            Page_Client page_Client = new Page_Client(this.id_client);
            this.NavigationService.Navigate(page_Client);
        }
    }
}
