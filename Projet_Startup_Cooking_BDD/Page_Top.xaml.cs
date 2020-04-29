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

            //CdR Semaine
            int jour = (int)DateTime.Today.DayOfWeek; //jour de la semaine
            double decalage = 1 - jour; //on veut le decalage à lundi
            System.DateTime datelimite_system = DateTime.Today.AddDays(decalage);
            string datelimite = $"{datelimite_system.Year}/{datelimite_system.Month}/{datelimite_system.Day}"; // on a notre date limite
            string date = $"{DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now.Day}";
            // on veut sélectionner toutes les commandes faites sur cette période
            query = $"select  recette.Identifiant " +
                $"from (cooking.commande natural join cooking.composition_commande as T)  join cooking.recette " +
                $"on cooking.recette.Nom_Recette=T.Nom_Recette " +
                $"where Date between \"{datelimite}\" and \"{date}\" " +
                $"group by cooking.recette.Identifiant " +
                $"order by sum(Quantite_Recette) desc " +
                $"limit 1;";
            List<List<string>> Liste_CdR_Semaine = Commandes_SQL.Select_Requete(query);
            CdR_Semaine.Content = Liste_CdR_Semaine[0][0];

            //Top commande Semaine
            query = $"select recette.Nom_Recette, Type, recette.Identifiant, sum(Quantite_Recette), Compteur " +
                $"from (cooking.commande natural join cooking.composition_commande as T)  join cooking.recette " +
                $"on cooking.recette.Nom_Recette=T.Nom_Recette " +
                $"where Date between \"{datelimite}\" and \"{date}\" " +
                $"group by cooking.recette.Nom_Recette " +
                $"order by sum(Quantite_Recette) desc " +
                $"limit 5;";
            List<List<string>> Liste_Recette_Semaine = Commandes_SQL.Select_Requete(query);

            for (int i = 0; i < Liste_Recette_Semaine.Count; i++)
            {
                ListeView_Recette_Semaine.Items.Add(new Recette_Semaine
                {
                    Nom = Liste_Recette_Semaine[i][0],
                    Type = Liste_Recette_Semaine[i][1],
                    Createur = Liste_Recette_Semaine[i][2],
                    Volume_Achete_Semaine = Liste_Recette_Semaine[i][3],
                    Compteur = Liste_Recette_Semaine[i][4]
                });
            }
        }

        public class Recette_Or
        {
            public string Nom { get; set; }
            public string Type { get; set; }
            public string Compteur { get; set; }
        }

        public class Recette_Semaine
        {
            public string Nom { get; set; }
            public string Type { get; set; }
            public string Createur { get; set; }
            public string Volume_Achete_Semaine { get; set; }
            public string Compteur { get; set; }
        }
    }
}
