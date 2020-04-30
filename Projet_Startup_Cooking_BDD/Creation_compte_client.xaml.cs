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
    /// Logique d'interaction pour Creation_compte_client.xaml
    /// </summary>
    public partial class Creation_compte_client : Page
    {
        public Creation_compte_client()
        {
            InitializeComponent();
        }

        private void Button_Click_creer_compte_client(object sender, RoutedEventArgs e)
        {
            
            string id = idTextBox.Text;
            string mdp = mdpTextBox.Text;
            string nom = nomTextBox.Text;
            string tel = telTextBox.Text;
            
            if (id == "" || id.Length>50)
            {
                error.Content = "Identifiant invalide (1-50 caractères)";
            }
            else if (mdp == "" || mdp.Length > 50)
            {
                error.Content = "Mot de passe invalide (1-50 caractères)";
            }
            else if (nom == "" || nom.Length > 50)
            {
                error.Content = "Nom invalide (1-50 caractères)";
            }
            else if (tel == "" || !int.TryParse(tel,out _) || tel.Length > 15)
            {
                error.Content = "Téléphone invalide (1-15 caractères)";
            }
            else
            {
                error.Content = "";
                string requete = $"INSERT INTO cooking.client VALUES (\"{id}\",\"{mdp}\",\"{nom}\",\"{tel}\",0,False);";
                string ex = Commandes_SQL.Insert_Requete(requete);

                if (ex == $"Duplicate entry '{id}' for key 'client.PRIMARY'")
                {
                    error.Content = "Identifiant déjà utilisé";
                }
                else
                {
                    Interface_Home homepage = new Interface_Home();
                    this.NavigationService.Navigate(homepage);
                }
            }
        }


        private void Caractere_interdit(object sender, TextChangedEventArgs e)
        {
            TextBox id_textbox = sender as TextBox;
            // \s - Stands for white space. The rest is for alphabets and numbers
            if (id_textbox.Text.Contains('"')|| id_textbox.Text.Contains('é') || id_textbox.Text.Contains('è') 
                || id_textbox.Text.Contains('î')|| id_textbox.Text.Contains('ê')|| id_textbox.Text.Contains('ô')
                || id_textbox.Text.Contains('ï') || id_textbox.Text.Contains('ë') || id_textbox.Text.Contains('ç') 
                || id_textbox.Text.Contains('à') || id_textbox.Text.Contains('ù'))
            {
                string a = id_textbox.Text.Remove(id_textbox.Text.Length - 1);
                id_textbox.Text = a;
                error.Content = "Accents/Guillemets interdits";
            }
            return;
        }
    }
}
