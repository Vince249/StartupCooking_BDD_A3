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
    /// Logique d'interaction pour Page_Top.xaml
    /// </summary>
    public partial class Page_Top : Page
    {
        public Page_Top()
        {
            InitializeComponent();

            //On peut déjà mettre les informations du CdR d'Or

            //On recherche déjà son nom
            //On peut directement chercher dans Composition_Commande vu qu'on va prendre toutes les commandes
            string query = "SELECT Identifiant, sum(compteur) as SUMQT FROM cooking.recette group by Identifiant order by SUMQT desc limit 1;"; //query pour le CdR d'Or
            List<List<string>> Liste_Nom_CdR_Qte_vendue = Commandes_SQL.Select_Requete(query);
            CdR_Or.Content = Liste_Nom_CdR_Qte_vendue[0][0];

            //On cherche ensuite ses recettes les plus vendues

            query = $"SELECT Nom_Recette, Type, Compteur FROM cooking.recette where Identifiant = \"{Liste_Nom_CdR_Qte_vendue[0][0]}\" order by Compteur desc limit 5;";
            List<List<string>> Liste_Recette_Infos = Commandes_SQL.Select_Requete(query);

            //On ajoute ensuite les informations de chaque recette à la ListView dédiée

            for (int i = 0; i < Liste_Recette_Infos.Count; i++)
            {
                string Nom = Liste_Recette_Infos[i][0];
                string Type = Liste_Recette_Infos[i][1];
                string Qte = Liste_Recette_Infos[i][2];
                Liste_Recette_Or.Items.Add(new Recette_Or { Nom = Nom, Type = Type, Compteur = Qte });
            }

        }

        private void Semaine_Click(object sender, RoutedEventArgs e)
        {

        }

        public class Recette_Or
        {
            public string Nom { get; set; }
            public string Type { get; set; }
            public string Compteur { get; set; }
        }
    }
}
