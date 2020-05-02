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

            Remuneration_cooks.Visibility = Visibility.Hidden; //par défaut on cache ce label
            label_Remuneration.Visibility = Visibility.Hidden; //par défaut on cache ce label
            label_cook_remuneration.Visibility = Visibility.Hidden; //par défaut on cache ce label

            //Si le client qui commande est le CdR d'une des recette du panier, on affiche la rémunération qu'il va percevoir de cette commande
            int ajout_credit = 0;
            List<string> liste_CdR = new List<string>();
            for (int i = 0; i < liste_panier.Count; i++)
            {
                string nom_recette = this.liste_panier[i][0];
                string query = $"select Identifiant,Remuneration from cooking.recette where Nom_Recette = \"{nom_recette}\";";
                List<List<string>> Id_CdR_et_remuneration = Commandes_SQL.Select_Requete(query);

                if (this.id_client == Id_CdR_et_remuneration[i][0])
                {
                    int qt = Convert.ToInt32(this.liste_panier[i][1]);
                    int remuneration = Convert.ToInt32(Id_CdR_et_remuneration[i][1]);
                    ajout_credit+=remuneration * qt;
                    Remuneration_cooks.Visibility = Visibility.Visible;
                    label_Remuneration.Visibility = Visibility.Visible;
                    label_cook_remuneration.Visibility = Visibility.Visible;
                }
            }

            Remuneration_cooks.Content = Convert.ToString(ajout_credit);

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

                // Créer une instance de Commande

                string date = $"{DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now.Day}";
                query = $"Insert into cooking.commande (Date, prix, Identifiant) VALUES(\"{date}\",{Total.Content},\"{this.id_client}\");";
                ex = Commandes_SQL.Insert_Requete(query);

                // Actions pour chacune des recettes
                for (int i = 0; i < this.liste_panier.Count; i++)
                {
                    // Rémunérer le/les CdR

                    string nom_recette = this.liste_panier[i][0];
                    int qt = Convert.ToInt32(this.liste_panier[i][1]);
                    query = $"Select Identifiant, Remuneration, compteur, Prix_Vente from cooking.recette where Nom_Recette = \"{nom_recette}\";";
                    List<List<string>> info_recette = Commandes_SQL.Select_Requete(query);
                    string identifiant_CdR = info_recette[0][0];
                    int remuneration = Convert.ToInt32(info_recette[0][1]);
                    int ajout_credit = remuneration * qt;

                    query = $"Select Credit_Cook from cooking.client where Identifiant = \"{identifiant_CdR}\";";
                    List<List<string>> Credit_CdR = Commandes_SQL.Select_Requete(query);

                    int nvCredit_CdR = Convert.ToInt32(Credit_CdR[0][0]) + ajout_credit;
                    query = $"Update cooking.client set Credit_Cook = {nvCredit_CdR} where Identifiant = \"{identifiant_CdR}\"; ";
                    ex = Commandes_SQL.Insert_Requete(query);

                    // Augmenter le compteur des recettes utilisées de la quantité prise

                    int nvcompteur = Convert.ToInt32(info_recette[0][2]) + qt;
                    query = $"Update cooking.recette set compteur = {nvcompteur} where Nom_Recette = \"{nom_recette}\";";
                    ex = Commandes_SQL.Insert_Requete(query);

                    // Augmenter le prix de vente
                    // Augmenter la rémunération de la recette

                    if (nvcompteur>10 && Convert.ToInt32(info_recette[0][2])<=10) //nv compteur >10 et ancien <=10
                    {
                        int nv_prix = 2 + Convert.ToInt32(info_recette[0][3]);
                        int remuneration_CdR = 2;
                        if (nvcompteur > 50 && Convert.ToInt32(info_recette[0][2]) <= 50)//nv compteur >50 et ancien <=50
                        {
                            nv_prix = 5 + Convert.ToInt32(info_recette[0][3]);
                            remuneration_CdR = 4;
                        }
                        query = $"Update cooking.recette set Prix_Vente = {nv_prix}, Remuneration = {remuneration_CdR} where Nom_Recette = \"{nom_recette}\";";
                        ex = Commandes_SQL.Insert_Requete(query);

                    }

                    // Créer ses instances de Recette_Commande
                    query = $"select count(*) from cooking.commande"; //notre commande est la dernière, donc Ref_Commande = count(*)
                                                                      //En effet ref_commande est un autoincrement
                    List<List<string>> List_Ref_Commande = Commandes_SQL.Select_Requete(query);
                

                    query = $"Insert into cooking.composition_commande VALUES (\"{nom_recette}\",{List_Ref_Commande[0][0]}, {qt} );";
                    ex = Commandes_SQL.Insert_Requete(query);


                    //Diminuer les produits

                    query = $"select Nom_Produit, Quantite_Produit from cooking.composition_recette where Nom_Recette = \"{nom_recette}\";";
                    List<List<string>> List_Produit_QT_dans_recette = Commandes_SQL.Select_Requete(query);

                    for (int j = 0; j < List_Produit_QT_dans_recette.Count; j++)
                    {
                        int diminution = qt * Convert.ToInt32(List_Produit_QT_dans_recette[j][1]);
                        query = $"select Stock from cooking.produit where Nom_Produit = \"{List_Produit_QT_dans_recette[j][0]}\";";
                        List<List<string>> List_Stock = Commandes_SQL.Select_Requete(query);
                        int nv_Stock = Convert.ToInt32(List_Stock[0][0]) - diminution;
                        query = $"Update cooking.produit set Stock = \"{nv_Stock}\" where Nom_Produit = \"{List_Produit_QT_dans_recette[j][0]}\" ;";
                        Commandes_SQL.Insert_Requete(query);
                    }
                }

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
