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
using System.Xml.Serialization; //pour (Dé)Sérialisation en XML
using System.IO; //lecture et écriture de fichiers

namespace Projet_Startup_Cooking_BDD
{
    /// <summary>
    /// Logique d'interaction pour Page_Admin.xaml
    /// </summary>
    public partial class Page_Admin : Page
    {
        private string id_admin;
        public Page_Admin(string id_admin)
        {
            InitializeComponent();
            this.id_admin = id_admin;

            welcome_message.Content = "Bonjour " + id_admin;

            //Ajout recette CdR et id_CdR dans la listView associé
            string query = $"select Nom_Recette,Identifiant from cooking.recette;";
            List<List<string>> liste_recette_nom_compteur = Commandes_SQL.Select_Requete(query);
            for (int i = 0; i < liste_recette_nom_compteur.Count; i++)
            {
                Recettes_id_ListView.Items.Add(new Recette_id_CdR { Nom_Recette = liste_recette_nom_compteur[i][0], Identifiant_CdR = liste_recette_nom_compteur[i][1] });
            }
        }

        private void Deco_Click(object sender, RoutedEventArgs e)
        {
            Interface_Home interhome = new Interface_Home();
            this.NavigationService.Navigate(interhome);
        }

        private void Trier_par_id_Click(object sender, RoutedEventArgs e)
        {
            string query = $"select Nom_Recette,Identifiant from cooking.recette order by Identifiant;";
            List<List<string>> liste_recette_nom_compteur = Commandes_SQL.Select_Requete(query);

            Recettes_id_ListView.Items.Clear();
            for (int i = 0; i < liste_recette_nom_compteur.Count; i++)
            {
                Recettes_id_ListView.Items.Add(new Recette_id_CdR { Nom_Recette = liste_recette_nom_compteur[i][0], Identifiant_CdR = liste_recette_nom_compteur[i][1] });
            }
        }

        private void Trier_par_recette_Click(object sender, RoutedEventArgs e)
        {
            string query = $"select Nom_Recette,Identifiant from cooking.recette order by Nom_Recette;";
            List<List<string>> liste_recette_nom_compteur = Commandes_SQL.Select_Requete(query);

            Recettes_id_ListView.Items.Clear();
            for (int i = 0; i < liste_recette_nom_compteur.Count; i++)
            {
                Recettes_id_ListView.Items.Add(new Recette_id_CdR { Nom_Recette = liste_recette_nom_compteur[i][0], Identifiant_CdR = liste_recette_nom_compteur[i][1] });
            }
        }

        private void Supprimer_recette_Click(object sender, RoutedEventArgs e)
        {
            Recette_id_CdR selection = Recettes_id_ListView.SelectedItem as Recette_id_CdR;
            if (selection == null)
            {
                Erreur_Message.Content = "Aucune recette sélectionnée";
            }
            else
            {
                Erreur_Message.Content = "";

                //delete child rows
                string query1 = $"delete from cooking.composition_recette where Nom_Recette = \"{selection.Nom_Recette}\";";
                string query2 = $"delete from cooking.composition_commande where Nom_Recette = \"{selection.Nom_Recette}\";";
                string query3 = $"delete from cooking.palmares_recette where Nom_Recette = \"{selection.Nom_Recette}\";";

                //delete parent row
                string query4 = $"delete from cooking.recette where Nom_Recette = \"{selection.Nom_Recette}\";";

                //final query
                string query = query1 + query2 + query3 + query4;
                string ex = Commandes_SQL.Insert_Requete(query);

                //update listView
                List<Recette_id_CdR> liste_nv_Item = new List<Recette_id_CdR>();
                for (int i = 0; i < Recettes_id_ListView.Items.Count; i++)
                {
                    Recette_id_CdR recette_observee = Recettes_id_ListView.Items[i] as Recette_id_CdR;
                    if (recette_observee.Nom_Recette != selection.Nom_Recette)
                    {
                        Recette_id_CdR copie_recette = new Recette_id_CdR { Nom_Recette = recette_observee.Nom_Recette, Identifiant_CdR = recette_observee.Identifiant_CdR };
                        liste_nv_Item.Add(copie_recette);
                    }
                }
                Recettes_id_ListView.Items.Clear();
                for (int i = 0; i < liste_nv_Item.Count; i++)
                {
                    Recettes_id_ListView.Items.Add(liste_nv_Item[i]);
                }
            }
        }

        private void Ban_CdR_Click(object sender, RoutedEventArgs e)
        {
            if (id_CdR_Box.Text == "")
            {
                Erreur_Message.Content = "Aucun CdR rentré";
            }
            else
            {
                Erreur_Message.Content = "";

                string query = $"select Nom_Recette from cooking.recette where Identifiant = \"{id_CdR_Box.Text}\";";
                List<List<string>> liste_nom_recette_CdR = Commandes_SQL.Select_Requete(query);

                for (int i = 0; i < liste_nom_recette_CdR.Count; i++)
                {
                    //delete child rows
                    string query1 = $"delete from cooking.composition_recette where Nom_Recette = \"{liste_nom_recette_CdR[i][0]}\";";
                    string query2 = $"delete from cooking.composition_commande where Nom_Recette = \"{liste_nom_recette_CdR[i][0]}\";";
                    string query3 = $"delete from cooking.palmares_recette where Nom_Recette = \"{liste_nom_recette_CdR[i][0]}\";";

                    //delete parent row
                    string query4 = $"delete from cooking.recette where Nom_Recette = \"{liste_nom_recette_CdR[i][0]}\";";

                    //final query
                    string query_final = query1 + query2 + query3 + query4;
                    string ex = Commandes_SQL.Insert_Requete(query_final);
                }

                query = $"delete from cooking.client where Identifiant = \"{id_CdR_Box.Text}\";";
                string ex2 = Commandes_SQL.Insert_Requete(query);

                //on actualise la listView contenant les recettes
                Recettes_id_ListView.Items.Clear();
                query = $"select Nom_Recette,Identifiant from cooking.recette;";
                List<List<string>> liste_recette_nom_compteur = Commandes_SQL.Select_Requete(query);
                for (int i = 0; i < liste_recette_nom_compteur.Count; i++)
                {
                    Recettes_id_ListView.Items.Add(new Recette_id_CdR { Nom_Recette = liste_recette_nom_compteur[i][0], Identifiant_CdR = liste_recette_nom_compteur[i][1] });
                }
            }
        }

        private void CdR_To_Client_Click(object sender, RoutedEventArgs e)
        {
            if (id_CdR_Box.Text == "")
            {
                Erreur_Message.Content = "Aucun CdR rentré";
            }
            else
            {
                Erreur_Message.Content = "";

                //passer le CdR en client et l'empêcher de redevenir CdR (cela est représenté par la valeur 2)
                string query = $"Update cooking.client set CdR = 2 where Identifiant = \"{id_CdR_Box.Text}\";";
                string ex = Commandes_SQL.Insert_Requete(query);

                //suppression de toutes les recettes du CdR

                query = $"select Nom_Recette from cooking.recette where Identifiant = \"{id_CdR_Box.Text}\";";
                List<List<string>> liste_nom_recette_CdR = Commandes_SQL.Select_Requete(query);

                for (int i = 0; i < liste_nom_recette_CdR.Count; i++)
                {
                    //delete child rows
                    string query1 = $"delete from cooking.composition_recette where Nom_Recette = \"{liste_nom_recette_CdR[i][0]}\";";
                    string query2 = $"delete from cooking.composition_commande where Nom_Recette = \"{liste_nom_recette_CdR[i][0]}\";";
                    string query3 = $"delete from cooking.palmares_recette where Nom_Recette = \"{liste_nom_recette_CdR[i][0]}\";";

                    //delete parent row
                    string query4 = $"delete from cooking.recette where Nom_Recette = \"{liste_nom_recette_CdR[i][0]}\";";

                    //final query
                    string query_final = query1 + query2 + query3 + query4;
                    ex = Commandes_SQL.Insert_Requete(query_final);
                }

                //on actualise la listView contenant les recettes
                Recettes_id_ListView.Items.Clear();
                query = $"select Nom_Recette,Identifiant from cooking.recette;";
                List<List<string>> liste_recette_nom_compteur = Commandes_SQL.Select_Requete(query);
                for (int i = 0; i < liste_recette_nom_compteur.Count; i++)
                {
                    Recettes_id_ListView.Items.Add(new Recette_id_CdR { Nom_Recette = liste_recette_nom_compteur[i][0], Identifiant_CdR = liste_recette_nom_compteur[i][1] });
                }
            }
        }

        private void Top_Click(object sender, RoutedEventArgs e)
        {
            Page_Top pageTop = new Page_Top();
            this.NavigationService.Navigate(pageTop);
        }

        private void Carnet_commandes_produit_XML_Click(object sender, RoutedEventArgs e)
        {
            //récupération des produits triés par fournisseur et nom des produits
            string query = "select Nom_Produit,Categorie,Unite,Stock,Stock_min,Stock_max,Ref_Fournisseur from cooking.produit where Stock < Stock_min order by Ref_Fournisseur,Nom_Produit;";
            List<List<string>> liste_produit_a_commander = Commandes_SQL.Select_Requete(query);

            List<Fournisseur> liste_Fournisser_XML = new List<Fournisseur>(); //liste finale qu'on va rentrer dans XML
            List<Produit> liste_produits_pour_un_fournisseur = new List<Produit>();
            int a = 0; //compteur utilisé pour rassembler les produits d'un même fournisseur dans une liste
            bool changement_fournisseur_iteration_precedente = true;
            for (int i = 0; i < liste_produit_a_commander.Count; i+=a)
            {
                a = 0;
                //si on a changé de fournisseur à l'itération précédente, on doit rentrer dans la boucle
                //de même, si le fournisseur du produit regardé est le même que celui du produit de l'itération précédente, on doit rentrer dans la boucle
                //la condition du dessus est valable car les produits sont triés par fournisseur dans la liste des produits commandés
                while (changement_fournisseur_iteration_precedente || liste_produit_a_commander[i+a][6] == liste_produit_a_commander[i+a - 1][6])
                {
                    //info du produit qu'on regarde
                    Produit produit_concerne = new Produit
                    {
                        Nom_Produit = liste_produit_a_commander[i+a][0],
                        Categorie = liste_produit_a_commander[i+a][1],
                        Unite = liste_produit_a_commander[i+a][2],
                        Stock = liste_produit_a_commander[i+a][3],
                        Stock_min = liste_produit_a_commander[i+a][4],
                        Stock_max = liste_produit_a_commander[i+a][5],
                        Ref_Fournisseur = liste_produit_a_commander[i+a][6],
                        Quantite_a_commander = Convert.ToString(Convert.ToInt32(liste_produit_a_commander[i+a][5]) - Convert.ToInt32(liste_produit_a_commander[i+a][3]))
                    };

                    liste_produits_pour_un_fournisseur.Add(produit_concerne); //tous les produits à commander pour un fournisseur
                    a++;
                    if (i + a == liste_produit_a_commander.Count) break; //si on atteint la taille de la liste des produits à commander on sort du while
                    changement_fournisseur_iteration_precedente = false;
                }

                //récupération des infos du fournisseur concerné
                query = $"select Ref_Fournisseur,Nom_Fournisseur,Numero_tel_Fournisseur from cooking.fournisseur where Ref_Fournisseur = \"{liste_produit_a_commander[i+a - 1][6]}\";";
                List<List<string>> liste_info_fournisseur = Commandes_SQL.Select_Requete(query);

                Fournisseur nouveau_fournisseur = new Fournisseur
                {
                    Ref_Fournisseur = liste_info_fournisseur[0][0],
                    Nom_Fournisseur = liste_info_fournisseur[0][1],
                    Numero_tel_Fournisseur = liste_info_fournisseur[0][2],
                    liste_produit_a_commander = liste_produits_pour_un_fournisseur
                };

                liste_Fournisser_XML.Add(nouveau_fournisseur);

                liste_produits_pour_un_fournisseur = new List<Produit>(); //reset la liste des produits pour un fournisseur

                changement_fournisseur_iteration_precedente = true;
            }

            //Création du fichier XML (site pour aide : https://tlevesque.developpez.com/dotnet/xml-serialization/#LI-A-1)
            XmlSerializer xs = new XmlSerializer(typeof(List<Fournisseur>));
            using (StreamWriter wr = new StreamWriter("Liste_des_produits_à_commander.xml"))
            {
                xs.Serialize(wr, liste_Fournisser_XML);
            }
        }

        private void Creation_Produit_Click(object sender, RoutedEventArgs e)
        {

        }

        public class Recette_id_CdR
        {
            public string Nom_Recette { get; set; }

            public string Identifiant_CdR { get; set; }
        }

        public class Produit
        {
            public string Nom_Produit { get; set; }

            public string Categorie { get; set; }

            public string Unite { get; set; }

            public string Stock { get; set; }

            public string Stock_min { get; set; }

            public string Stock_max { get; set; }

            public string Ref_Fournisseur { get; set; }

            public string Quantite_a_commander { get; set; } 
        }

        public class Fournisseur
        {
            public string Ref_Fournisseur { get; set; }

            public string Nom_Fournisseur { get; set; }

            public string Numero_tel_Fournisseur { get; set; }

            public List<Produit> liste_produit_a_commander { get; set; }
        }

    }
}
