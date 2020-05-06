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

        /// <summary>
        /// Initialisation de la page comprenant le message de bienvenue, le solde du CdR, la listView contenant les produits et la listView contenant les recettes du CdR
        /// </summary>
        /// <param name="id_client"> identifiant de l'utilisateur que l'on a conservé depuis la page Interface_Home </param>
        public Page_CdR(string id_client)
        {
            InitializeComponent();
            this.id_client = id_client;
            string query = $"Select Nom_Client from cooking.client where Identifiant = \"{this.id_client}\" ;";
            List<List<string>> liste = Commandes_SQL.Select_Requete(query);
            welcome_message.Content = "Bonjour " + liste[0][0];
            
            // récupération du solde cook du CdR
            query = $"select Credit_Cook from client where Identifiant = \"{this.id_client}\" ;";
            liste = Commandes_SQL.Select_Requete(query);
            Solde.Content = liste[0][0];

            // ajout produit dans la listView associée
            query = $"select Nom_Produit,Unite from cooking.produit;";
            List<List<string>> liste_produit_nom_unite = Commandes_SQL.Select_Requete(query);
            for (int i = 0; i < liste_produit_nom_unite.Count; i++)
            {
                Produits_ListView.Items.Add(new Produit { Nom_Produit = liste_produit_nom_unite[i][0], Unite = liste_produit_nom_unite[i][1] });
            }

            // ajout recette CdR dans la listView associée
            query = $"select Nom_Recette,Compteur from cooking.recette where Identifiant = \"{this.id_client}\";";
            List<List<string>> liste_recette_nom_compteur = Commandes_SQL.Select_Requete(query);
            for (int i = 0; i < liste_recette_nom_compteur.Count; i++)
            {
                Recettes_CdR_ListView.Items.Add(new Recette { Nom_Recette = liste_recette_nom_compteur[i][0], Compteur = liste_recette_nom_compteur[i][1] });
            }
        }


        /// <summary>
        /// Méthode reliée au bouton "Client" ramenant sur la page_Client
        /// </summary>
        /// <param name="sender">Bouton "Client"</param>
        /// <param name="e">Evenement Click</param>
        private void Client_Click(object sender, RoutedEventArgs e)
        {
            Page_Client page_client = new Page_Client(this.id_client);
            this.NavigationService.Navigate(page_client);
        }


        /// <summary>
        /// Méthode reliée au bouton "Déconnexion" ramenant sur la page Interface_Home
        /// </summary>
        /// <param name="sender">Bouton "Déconnexion"</param>
        /// <param name="e">Evenement Click</param>
        private void Deco_Click(object sender, RoutedEventArgs e)
        {
            Interface_Home interhome = new Interface_Home();
            this.NavigationService.Navigate(interhome);
        }


        /// <summary>
        /// Méthode reliée au bouton "Créer la recette" permettant d'ajouter une nouvelle recette dans la database
        /// </summary>
        /// <param name="sender">Bouton "Créer la recette"</param>
        /// <param name="e">Evenement Click</param>
        private void Creer_Recette_Click(object sender, RoutedEventArgs e)
        {
            //récupération infos rentrées par l'utilisateur
            string nom_recette_input = Nom_Recette_Box.Text;
            string type_recette_input = Type_Recette_Box.Text;
            string prix_recette_input = Prix_Recette_Box.Text;
            string description_recette_input = Description_Recette_Box.Text;
            List<List<string>> liste_produit_nom_quantite = new List<List<string>>();

            for (int i = 0; i < Produits_ListView.Items.Count; i++)
            {
                Produit produit_observe = Produits_ListView.Items[i] as Produit;

                // si la quantité du produit observé n'est pas nulle, il fait partie de la recette
                if(produit_observe.Quantite != null)
                {
                    List<string> temp = new List<string>();
                    temp.Add(produit_observe.Nom_Produit);
                    temp.Add(produit_observe.Quantite);
                    liste_produit_nom_quantite.Add(temp);
                }
            }

            // sécurité pour les input
            if (nom_recette_input == "" || nom_recette_input.Length>100)
            {
                Erreur_Message.Content = "Nom doit contenir 1 à 100 caractères";
            }
            else if (type_recette_input == "" || !type_recette_input.All(Char.IsLetter))
            {
                Erreur_Message.Content = "Type invalide";
            }
            else if (prix_recette_input == ""|| !int.TryParse(prix_recette_input,out _) || Convert.ToInt32(prix_recette_input)>40|| Convert.ToInt32(prix_recette_input) <10)
            {
                Erreur_Message.Content = "Prix doit être entre 10 et 40";
            }
            else if (description_recette_input == "" || description_recette_input.Length > 256)
            {
                Erreur_Message.Content = "Description est limitée à 256 caractères";
            }
            else if (liste_produit_nom_quantite.Count == 0)
            {
                Erreur_Message.Content = "Aucun produit ajouté";
            }
            else
            {
                Erreur_Message.Content = "";

                // query pour insérer la nouvelle recette dans la database avec les input comme valeurs
                string query1 = $"INSERT INTO cooking.recette VALUES (\"{nom_recette_input}\",\"{type_recette_input}\",\"{description_recette_input}\",\"{prix_recette_input}\",2,0,\"{this.id_client}\");";
                string query2 = "";
                string query4 = "";
                for (int i = 0; i < liste_produit_nom_quantite.Count; i++)
                {
                    // query pour insérer un nouveau produit relié à sa recette dans la database avec les input comme valeurs
                    query2 += $"INSERT INTO cooking.composition_recette VALUES (\"{nom_recette_input}\",\"{liste_produit_nom_quantite[i][0]}\",{liste_produit_nom_quantite[i][1]});";
                    
                    // mise à jour des sotck min et max des produits
                    // on a décidé d'augmenter stock min de 1 fois la quantité nécessaire pour cette recette (choix expliqué dans le rapport)
                    // stock max est augmenté de 2 fois la quantité nécessaire pour cette recette
                    string query3 = $"Select Stock_min, Stock_max from cooking.produit where Nom_Produit = \"{liste_produit_nom_quantite[i][0]}\" ;";
                    List<List<string>> Liste_stock_min_max = Commandes_SQL.Select_Requete(query3);

                    int Nv_Stock_min =  Convert.ToInt32(Liste_stock_min_max[0][0]) + Convert.ToInt32(liste_produit_nom_quantite[i][1]);
                    int Nv_Stock_max = Convert.ToInt32(Liste_stock_min_max[0][1]) + 2*Convert.ToInt32(liste_produit_nom_quantite[i][1]);
                    query4 += $"Update cooking.produit set Stock_min = {Nv_Stock_min}, Stock_max = {Nv_Stock_max} where Nom_Produit = \"{liste_produit_nom_quantite[i][0]}\" ;";

                }
                string query = query1 + query2 + query4; // si la recette porte le même nom qu'une recette existante, l'exécution échoue à la query1. Cela empêche l'exécution des query 2 et 4
                string ex = Commandes_SQL.Insert_Requete(query);
                if(ex== $"Duplicate entry '{nom_recette_input}' for key 'recette.PRIMARY'") // si le nom_recette (clé primaire) existe déjà dans la database.recette
                {
                    Erreur_Message.Content = "Nom déjà utilisé";
                }
                else
                {
                    Page_CdR page_CdR = new Page_CdR(this.id_client);
                    this.NavigationService.Navigate(page_CdR);
                }
               
            }

        }


        /// <summary>
        /// Méthode reliée au bouton "Supprimer recette" permettant de supprimer une recette, et tout ce qui est associé à celle-ci, de la database
        /// </summary>
        /// <param name="sender">Bouton "Supprimer recette"</param>
        /// <param name="e">Evenement Click</param>
        private void Supprimer_Recette_Click(object sender, RoutedEventArgs e)
        {
            // selection est un objet que l'on re-définit en tant classe Recette pour pouvoir accéder à ce qu'elle contient
            Recette selection = Recettes_CdR_ListView.SelectedItem as Recette;
            if (selection == null)
            {
                Erreur_Message.Content = "Aucune recette sélectionnée";
            }
            else
            {
                Erreur_Message.Content = "";

                // mettre à jour les stock mini et max de produit
                // on recupère les infos relatives au produit dont on a besoin
                string query = $"Select Nom_Produit,Quantite_Produit,Stock_min,Stock_max from cooking.composition_recette natural join cooking.produit where Nom_Recette = \"{selection.Nom_Recette}\";";
                List<List<string>> Liste_Nom_Produit_QT_Min_Max = Commandes_SQL.Select_Requete(query);

                string query5 = "";
                for (int i = 0; i < Liste_Nom_Produit_QT_Min_Max.Count; i++)
                {
                    // re-définition des termes pour que ce soit plus clair
                    string nom_produit_observe = Liste_Nom_Produit_QT_Min_Max[i][0];
                    int quantite_necessaire_dans_recette = Convert.ToInt32(Liste_Nom_Produit_QT_Min_Max[i][1]);
                    int stock_min = Convert.ToInt32(Liste_Nom_Produit_QT_Min_Max[i][2]);
                    int stock_max = Convert.ToInt32(Liste_Nom_Produit_QT_Min_Max[i][3]);

                    // on adapte les stocks de la même manière que lorsque qu'une recette est créée, sauf qu'ici, le stock est impacté négativement
                    int nv_stock_min = stock_min - quantite_necessaire_dans_recette;
                    int nv_stock_max = stock_max - 2*quantite_necessaire_dans_recette;

                    // query pour modifier les valeurs de stock_min et stock_max où le nom de produit est celui que l'on observe
                    // on fait "+=" pour cette query car veut minimiser les interactions avec MySQL 
                    query5 += $"Update cooking.produit set Stock_min = {nv_stock_min}, Stock_max = {nv_stock_max} where Nom_Produit = \"{nom_produit_observe}\";";
                }
                // on exécute donc la query (contenant plusieurs commandes) dans MySQL à la fin de la boucle
                string ex = Commandes_SQL.Insert_Requete(query5);



                // delete child rows
                string query1 = $"delete from cooking.composition_recette where Nom_Recette = \"{selection.Nom_Recette}\";";
                string query2 = $"delete from cooking.composition_commande where Nom_Recette = \"{selection.Nom_Recette}\";";

                // delete parent row
                string query4 = $"delete from cooking.recette where Nom_Recette = \"{selection.Nom_Recette}\";";

                // final query
                query = query1 + query2  + query4;
                ex = Commandes_SQL.Insert_Requete(query);

                // update listView -> on veut aussi supprimer la recette selectionée de la listView
                List<Recette> liste_nv_Item = new List<Recette>();
                for (int i = 0; i < Recettes_CdR_ListView.Items.Count; i++)
                {
                    Recette recette_observee = Recettes_CdR_ListView.Items[i] as Recette;
                    if (recette_observee.Nom_Recette != selection.Nom_Recette)
                    {
                        Recette copie_recette = new Recette{ Nom_Recette = recette_observee.Nom_Recette, Compteur = recette_observee.Compteur};
                        liste_nv_Item.Add(copie_recette);
                    }
                }
                Recettes_CdR_ListView.Items.Clear();
                for (int i = 0; i < liste_nv_Item.Count; i++)
                {
                    Recettes_CdR_ListView.Items.Add(liste_nv_Item[i]);
                }
            }
        }


        /// <summary>
        /// Méthode reliée au bouton "Ajouter le produit" permettant d'affecter une quantité (input) au produit selectionné dans la listView produit
        /// </summary>
        /// <param name="sender">Bouton "Ajouter le produit"</param>
        /// <param name="e">Evenement Click</param>
        private void Ajout_Produit_Click(object sender, RoutedEventArgs e)
        {
            // récupération de l'input 
            string quantite_input = Quantite_Produit_Box.Text;

            Produit selection = Produits_ListView.SelectedItem as Produit;

            // sécurité
            if (selection == null)
            {
                Erreur_Message.Content = "Aucun produit selectionné";
            }
            else if(quantite_input == "" || !int.TryParse(quantite_input,out _)|| Convert.ToInt32(quantite_input)<=0)
            {
                Erreur_Message.Content = "Quantité invalide";
            }
            else
            {
                Erreur_Message.Content = "";

                // update de la listView produit de manière à affecter une quantité au produit selectioné et que cela soit visible dans cette même listView
                List<Produit> liste_nv_Item = new List<Produit>();
                for (int i = 0; i < Produits_ListView.Items.Count; i++)
                {
                    Produit produit_observe = Produits_ListView.Items[i] as Produit;

                    // on va donc copier la listView avant modification puis, lorsque le produit selectionné est égal au produit regardé, on affecte la quantité rentrée en input
                    if (selection.Nom_Produit == produit_observe.Nom_Produit)
                    {
                        Produit copie_produit = new Produit { Nom_Produit = produit_observe.Nom_Produit, Quantite = quantite_input, Unite = produit_observe.Unite };
                        liste_nv_Item.Add(copie_produit);
                    }
                    else
                    {
                        Produit copie_produit = new Produit { Nom_Produit = produit_observe.Nom_Produit, Quantite = produit_observe.Quantite, Unite = produit_observe.Unite };
                        liste_nv_Item.Add(copie_produit);
                    }
                }

                // update de la listView avec les nouveaux items
                Produits_ListView.Items.Clear();
                for (int i = 0; i < liste_nv_Item.Count; i++)
                {
                    Produits_ListView.Items.Add(liste_nv_Item[i]);
                }
            }
        }


        /// <summary>
        /// Méthode reliée au bouton "Reinitialiser" servant à clear toutes les textbox
        /// </summary>
        /// <param name="sender">Bouton "Reinitialiser"</param>
        /// <param name="e">Evenement Click</param>
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Page_CdR page_CdR = new Page_CdR(this.id_client);
            this.NavigationService.Navigate(page_CdR);
        }


        /// <summary>
        /// Classe utilisée pour remplir la listView produit ou récupérer les valeurs d'une selection dans cette même listViesw
        /// </summary>
        public class Produit
        {
            /// <summary>
            /// Nom du Produit
            /// </summary>
            public string Nom_Produit { get; set; }
            /// <summary>
            /// Unité du Produit
            /// </summary>
            public string Unite { get; set; }
            /// <summary>
            /// Quantité du Produit
            /// </summary>
            public string Quantite { get; set; }
        }


        /// <summary>
        /// Classe utilisée pour remplir la listView recette ou récupérer les valeurs d'une selection dans cette même listViesw
        /// </summary>
        public class Recette
        {
            /// <summary>
            /// Nom de la Recette
            /// </summary>
            public string Nom_Recette { get; set; }
            /// <summary>
            /// Compteur de la Recette
            /// </summary>
            public string Compteur { get; set; }
        }


        /// <summary>
        /// Méthode permettant d'interdire certains caractères pour les input (caratères provoquant des erreurs sur MySQL)
        /// </summary>
        /// <param name="sender">Textbox</param>
        /// <param name="e">Evenement texte modifié</param>
        private void Caractere_interdit(object sender, TextChangedEventArgs e)
        {
            TextBox id_textbox = sender as TextBox;
            if (id_textbox.Text.Contains('"') || id_textbox.Text.Contains('é') || id_textbox.Text.Contains('è')
                || id_textbox.Text.Contains('î') || id_textbox.Text.Contains('ê') || id_textbox.Text.Contains('ô')
                || id_textbox.Text.Contains('ï') || id_textbox.Text.Contains('ë') || id_textbox.Text.Contains('ç')
                || id_textbox.Text.Contains('à') || id_textbox.Text.Contains('ù'))
            {
                // on efface le dernier caractère s'il est interdit, ne laissant ainsi pas l'opportunité à l'utilisateur de l'écrire
                string a = id_textbox.Text.Remove(id_textbox.Text.Length - 1);
                id_textbox.Text = a;
                Erreur_Message.Content = "Accents/Guillemets interdits";
            }
            return;
        }
    }
}
