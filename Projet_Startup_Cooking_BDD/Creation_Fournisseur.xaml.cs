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
    /// Logique d'interaction pour Creation_Fournisseur.xaml
    /// </summary>
    public partial class Creation_Fournisseur : Page
    {
        private string id_admin;
        public Creation_Fournisseur(string id_admin)
        {
            InitializeComponent();
            this.id_admin = id_admin;
        }

        private void Creer_Fournisseur_Click(object sender, RoutedEventArgs e)
        {
            string nom = textbox_nom.Text;
            string refe = textbox_ref.Text;
            string tel = textbox_tel.Text;

            if (nom == "" || nom.Length > 50)
            {
                Erreur_message.Content = "Nom invalide (1-50 caractères)";
            }
            else if (refe == "" || refe.Length > 50)
            {
                Erreur_message.Content = "Référence invalide (1-50 caractères)";
            }
            else if (tel == "" || !int.TryParse(tel, out _) || tel.Length > 15)
            {
                Erreur_message.Content = "Téléphone invalide (1-15 caractères)";
            }
            else
            {
                Erreur_message.Content = "";

                string query = $"insert into cooking.fournisseur VALUES(\"{refe}\",\"{nom}\",\"{tel}\");";
                string ex = Commandes_SQL.Insert_Requete(query);
                if (ex == $"Duplicate entry '{refe}' for key 'fournisseur.PRIMARY'")
                {
                    Erreur_message.Content = "Nom déjà utilisé";
                }
                else
                {
                    Page_Admin page_admin = new Page_Admin(this.id_admin);
                    this.NavigationService.Navigate(page_admin);
                }
                
            }

        }

        private void Creer_Reinitialiser(object sender, RoutedEventArgs e)
        {
            Creation_Produit page_creation_produit = new Creation_Produit(this.id_admin);
            this.NavigationService.Navigate(page_creation_produit);
        }

        private void Caractere_interdit(object sender, TextChangedEventArgs e)
        {
            TextBox id_textbox = sender as TextBox;
            // \s - Stands for white space. The rest is for alphabets and numbers
            if (id_textbox.Text.Contains('"') || id_textbox.Text.Contains('é') || id_textbox.Text.Contains('è')
                || id_textbox.Text.Contains('î') || id_textbox.Text.Contains('ê') || id_textbox.Text.Contains('ô')
                || id_textbox.Text.Contains('ï') || id_textbox.Text.Contains('ë') || id_textbox.Text.Contains('ç')
                || id_textbox.Text.Contains('à') || id_textbox.Text.Contains('ù'))
            {
                string a = id_textbox.Text.Remove(id_textbox.Text.Length - 1);
                id_textbox.Text = a;
                Erreur_message.Content = "Accents/Guillemets interdits";
                
            }
            return;
        }


    }
}
