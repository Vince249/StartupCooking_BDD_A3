using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Logique d'interaction pour Interface_Home.xaml
    /// </summary>
    public partial class Interface_Home : Page
    {

        /// <summary>
        /// Initialisation de la page
        /// </summary>
        public Interface_Home()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Méthode reliée au bouton "Se connecter" côté client qui vérifie si l'identifiant et le mdp rentrés par l'utilisateur correspondent à un couple identifiant/mdp dans la database 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_connect_client(object sender, RoutedEventArgs e)
        {
            // Récupération des input
            string id = ID_client.Text;
            string mdp = MDP_client.Password;

            // sécurité sur le mdp pour éviter le caractère retournant une erreur MySQL (fonction Caractere_interdit ne fonctionne pas sur les passwordBox)
            if (mdp.Contains('"'))
            {
                error_label.Content = "Guillemets (\") interdits";
            }
            else
            {
                // query pour rechercher si le couple identifiant/mdp rentré par l'utilisateur correspondant à un couple dans la database
                string query = $"Select count(*) from cooking.client where Identifiant=\"{id}\" and Mot_de_passe=\"{mdp}\";";
                List<List<string>> liste = Commandes_SQL.Select_Requete(query);
                if (liste[0][0] == "1") // si on trouve une occurence de ce couple alors le client existe
                {
                    // on navigue vers la page client en conservant l'id
                    Page_Client page_client = new Page_Client(id);
                    this.NavigationService.Navigate(page_client);
                }
                else
                {
                    error_label.Content = "Erreur client non reconnu";
                }
            }
        }


        /// <summary>
        /// Méthode reliée au bouton "Créer un compte" amenant sur la page correspondante
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_create_client(object sender, RoutedEventArgs e)
        {
            Creation_compte_client page_creation_client = new Creation_compte_client();
            this.NavigationService.Navigate(page_creation_client);
        }


        /// <summary>
        /// Méthode reliée au bouton "Se connecter" côté admin qui vérifie si l'identifiant et le mdp rentrés par l'utilisateur correspondent à un couple identifiant/mdp dans la database 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_connect_admin(object sender, RoutedEventArgs e)
        {
            // récupération des input
            string id = ID_admin.Text;
            string mdp = MDP_admin.Password;

            // sécurité sur le mdp pour éviter le caractère retournant une erreur MySQL (fonction Caractere_interdit ne fonctionne pas sur les passwordBox)
            if (mdp.Contains('"'))
            {
                error_label.Content = "Guillemets (\") interdits";
            }
            else
            {
                // sécurité des input
                if (id.Length > 0 && mdp.Length > 0) // rien de rentré
                {
                    //Récupération infos dans fichier txt --> chaque index de la liste de liste finale contient un couple identifiant/mdp
                    string line;
                    string infos_in_file = "";
                    System.IO.StreamReader file = new System.IO.StreamReader("Acces_admin.txt");
                    while ((line = file.ReadLine()) != null)
                    {
                        infos_in_file += line + '\n';
                    }
                    file.Close();

                    string[] temp = infos_in_file.Split('\n');
                    List<List<string>> liste_infos_in_file = new List<List<string>>();
                    bool connection_ok = false;
                    for (int i = 0; i < temp.Length; i++)
                    {
                        liste_infos_in_file.Add(temp[i].Split(';').ToList());
                        if (liste_infos_in_file[i][0] == id && liste_infos_in_file[i][1] == mdp)
                        {
                            connection_ok = true;
                        }
                    }

                    if (connection_ok)
                    {
                        // on navigue vers la page client en conservant l'id
                        Page_Admin page_admin = new Page_Admin(id);
                        this.NavigationService.Navigate(page_admin);
                    }

                }
                else
                {
                    error_label.Content = "Erreur admin non reconnu";
                }
            }
        }


        /// <summary>
        /// Méthode reliée au bouton "Reset BDD" nous permettant de réinitialiser notre database à partir de fichier txt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Reset_BDD(object sender, RoutedEventArgs e)
        {
            Commandes_SQL.Execution_Script_TXT("Initialisation_DB.txt");
            Commandes_SQL.Execution_Script_TXT("Dummy_Data.txt");
        }


        /// <summary>
        /// Méthode reliée au bouton "Mode démo" amenant sur la page correspondante
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_mode_demo(object sender, RoutedEventArgs e)
        {
            Page_Demo_1 page_demo = new Page_Demo_1();
            this.NavigationService.Navigate(page_demo);
        }


        /// <summary>
        /// Méthode permettant d'interdire certains caractères pour les input (caratères provoquant des erreurs sur MySQL)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                error_label.Content = "Accents/Guillemets interdits";
            }
            return;
        }
    }
}
