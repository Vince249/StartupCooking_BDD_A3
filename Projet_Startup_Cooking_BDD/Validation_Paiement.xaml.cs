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
        private List<List<string>> liste_panier;
        public Validation_Paiement(string total, string solde, string id_client, List<List<string>> liste_panier)
        {
            InitializeComponent();
            this.liste_panier = liste_panier;
            this.id_client = id_client;
            Total.Content = total;
            Solde.Content = solde;
            int difference = Convert.ToInt32(solde) - Convert.ToInt32(total);
            if(difference>=0)
            {
                Reste_a_payer.Content = "0";
                Nv_Solde.Content = Convert.ToString(difference);
                Valider.Content = "Confirmer";
            }
            else
            {
                Nv_Solde.Content = "0";
                Reste_a_payer.Content = Convert.ToString(Math.Abs(difference));
                Valider.Content = "Payer le reste par CB";
            }


        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            Page_Client page_Client = new Page_Client(this.id_client);
            this.NavigationService.Navigate(page_Client);
        }

        private void Tricher_Click(object sender, RoutedEventArgs e)
        {
            string updatesolde = Convert.ToString(Convert.ToInt32(Solde.Content) + 50);
            string query = $"Update cooking.client set Credit_Cook = {updatesolde} where Identifiant = \"{this.id_client}\" ;";
            string ex = Commandes_SQL.Insert_Requete(query);

            Solde.Content = updatesolde;

            int difference = Convert.ToInt32(Solde.Content) - Convert.ToInt32(Total.Content);
            if (difference >= 0)
            {
                Reste_a_payer.Content = "0";
                Nv_Solde.Content = Convert.ToString(difference);
                Valider.Content = "Confirmer";
            }
            else
            {
                Nv_Solde.Content = "0";
                Reste_a_payer.Content = Convert.ToString(Math.Abs(difference));
                Valider.Content = "Payer le reste par CB";
            }

        }

        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            if(Valider.Content.ToString() == "Confirmer")
            {
                // Décrémenter le nb de crédit du client
                int nouveau_solde = Convert.ToInt32(Nv_Solde.Content);
                string query = $"Update cooking.client set Credit_Cook = {nouveau_solde}  where Identifiant = \"{this.id_client}\" ;";
                string ex = Commandes_SQL.Insert_Requete(query);

                // Augmenter le compteur des recettes utilisées de la quantité prise



                // Augmenter le prix de vente
                // Augmenter la rémunération de la recette
                // Créer une instance de Commande
                // Créer ses instances de Recette_Commande



                Page_Client page_Client = new Page_Client(this.id_client);
                this.NavigationService.Navigate(page_Client);
            }
            else
            {
                Page_Payer_CB page_payer_cb = new Page_Payer_CB();
                this.NavigationService.Navigate(page_payer_cb);
            }
        }
    }
}
