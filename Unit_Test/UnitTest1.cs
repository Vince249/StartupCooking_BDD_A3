using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unit_Test
{
    /// <summary>
    /// Tests Unitaires, ils portent sur nos fonctions en lien avec SQL
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// On utilise la table cooking.fournisseur car à moins de supprimer manuellement depuis MySQL, 
        /// on ne peut pas supprimer un fournisseur depuis l'application
        /// Nous savons donc que le fournisseur ayant la référence "#999" à le nom "Leguman" (on l'ajoute via Dummy_Data lorsque l'on initialise la database)
        /// </summary>
        [TestMethod]
        public void Test_Select_Requete()
        {
            string query = "select Nom_Fournisseur from cooking.fournisseur where Ref_Fournisseur=\"#999\";";
            List<List<string>> liste_fonction = Projet_Startup_Cooking_BDD.Commandes_SQL.Select_Requete(query);

            string reponse_attendue = "Leguman";

            Assert.AreEqual(reponse_attendue, liste_fonction[0][0]);
        }



        /// <summary>
        /// Nous allons compter le nombre de client avant insertion d'un nouveau puis,
        /// après nous allons re-compter le nombre de client après insertion de ce dernier.
        /// Si nombre de client avant insertion du nouveau client + 1 = nombre de client après insertion du nouveau client
        /// Alors la fonction Insert_Requete fonctionne
        /// Après avoir testé cela, nous allons supprimer le client de la database
        /// Remarque : la primary key de cooking.client étant l'identifiant, nous en avons choisi un qui a une faible chance d'être un doublon
        /// </summary>
        [TestMethod]
        public void Test_Insert_Requete()
        {
            string query = $"Insert into cooking.client VALUES( \"A98DB973KWL8XP1\",\"mdp\",\"nom\",\"1234567890\",0,0);";

            string query2 = "Select count(*) from cooking.client;";
            List<List<string>> liste_count_avant_insert = Projet_Startup_Cooking_BDD.Commandes_SQL.Select_Requete(query2);
            int nb_client_avant_insert = Convert.ToInt32(liste_count_avant_insert[0][0]);

            //insertion du nouveau client
            string ex = Projet_Startup_Cooking_BDD.Commandes_SQL.Insert_Requete(query);
            List<List<string>> liste_count_apres_insert = Projet_Startup_Cooking_BDD.Commandes_SQL.Select_Requete(query2);
            int nb_client_apres_insert = Convert.ToInt32(liste_count_apres_insert[0][0]);

            bool reponse = false;
            if (nb_client_avant_insert + 1 == nb_client_apres_insert)
            {
                reponse = true;
            }
            Assert.IsTrue(reponse);

            //on supprime le client que l'on a créé pour le test
            query = $"delete from cooking.client where Identifiant = \"A98DB973KWL8XP1\";";
            ex = Projet_Startup_Cooking_BDD.Commandes_SQL.Insert_Requete(query);
        }


        /// <summary>
        /// Nous allons effectuer le même processus que pour le test de "Insert_Requete" mais ici la query pour insérer le nouveau client
        /// se trouvera dans un fichier .txt que nous allons créer
        /// Après avoir testé cela, nous allons supprimer le client de la database ainsi que le fichier que nous avons créé
        /// </summary>
        [TestMethod]
        public void Execution_Script_TXT()
        {
            string query2 = "Select count(*) from cooking.client;";
            List<List<string>> liste_count_avant_insert = Projet_Startup_Cooking_BDD.Commandes_SQL.Select_Requete(query2);
            int nb_client_avant_insert = Convert.ToInt32(liste_count_avant_insert[0][0]);

            //insertion du nouveau client
            string path = "test.txt";
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("Insert into cooking.client VALUES( \"A98DB973KWL8XP1\",\"mdp\",\"nom\",\"1234567890\",0,0);");
            }

            Projet_Startup_Cooking_BDD.Commandes_SQL.Execution_Script_TXT(path);

            List<List<string>> liste_count_apres_insert = Projet_Startup_Cooking_BDD.Commandes_SQL.Select_Requete(query2);
            int nb_client_apres_insert = Convert.ToInt32(liste_count_apres_insert[0][0]);

            bool reponse = false;
            if (nb_client_avant_insert + 1 == nb_client_apres_insert)
            {
                reponse = true;
            }
            Assert.IsTrue(reponse);

            //on supprime le client que l'on a créé pour le test et le fichier créé
            string query = $"delete from cooking.client where Identifiant = \"A98DB973KWL8XP1\";";
            string ex = Projet_Startup_Cooking_BDD.Commandes_SQL.Insert_Requete(query);
            File.Delete(path);
        }
    }
}
