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
    /// Logique d'interaction pour Interface_Home.xaml
    /// </summary>
    public partial class Interface_Home : Page
    {


        public Interface_Home()
        {
            InitializeComponent();
        }


        private void Button_Click_connect_client(object sender, RoutedEventArgs e)
        {
            string id = ID_client.Text;
            string mdp = MDP_client.Password;
            string query = $"Select count(*) from cooking.client where Identifiant=\"{id}\" and Mot_de_passe=\"{mdp}\";";
            List<List<string>> liste = Commandes_SQL.Select_Requete(query);
            if (liste[0][0] == "1")
            {
                Page_Client page_client = new Page_Client(id);
                this.NavigationService.Navigate(page_client);
            }
            else
            {
                error_label.Content = "Erreur client non reconnu";
            }
            
        }

        private void Button_Click_create_client(object sender, RoutedEventArgs e)
        {
            Creation_compte_client page_creation_client = new Creation_compte_client();
            this.NavigationService.Navigate(page_creation_client);
        }

        private void Button_Click_connect_admin(object sender, RoutedEventArgs e)
        {
            string id = ID_admin.Text;
            string mdp = MDP_admin.Password;
            
            //Récupération infos dans fichier txt --> chaque index de la liste de liste finale contient un id et son mdp associé
            string line;
            string infos_in_file = "";
            System.IO.StreamReader file = new System.IO.StreamReader("Acces_admin.txt");
            while ((line = file.ReadLine()) != null)
            {
                infos_in_file += line + '\n';
            }
            file.Close();

            string[] temp = infos_in_file.Split('\n');
            List <List<string>> liste_infos_in_file = new List<List<string>>();
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
                Page_Admin page_admin = new Page_Admin();
                this.NavigationService.Navigate(page_admin);
            }
            else
            {
                error_label.Content = "Erreur administrateur non reconnu";
            }
        }
    }
}
