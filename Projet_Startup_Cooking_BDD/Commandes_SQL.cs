using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;

namespace Projet_Startup_Cooking_BDD
{
    /// <summary>
    /// Fonctions nous permettant d'intéragir avec MySQL
    /// </summary>
    public class Commandes_SQL
    {



        /// <summary>
        /// Méthode permettant d'obtenir sous forme de liste de liste de string le résultat d'une query MySQL sur la database
        /// </summary>
        /// <param name="requete"> Query MySQL appliquée à la database </param>
        /// <returns> Une ListListstring contenant les valeurs souhaitées</returns>
        public static List<List<string>> Select_Requete(string requete)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=cooking;UID=root;PASSWORD=root;"; //connexion à la database
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = requete;

            MySqlDataReader reader;
            reader = command.ExecuteReader();
            string resReq = "";
            while (reader.Read()) // parcours ligne par ligne
            {
                string currentRowAsString = "";
                for (int i = 0; i < reader.FieldCount; i++) // parcours cellule par cellule
                {
                    // on va récupérer chaque valeur contenue les celulles sous forme de string
                    // on va séparer les éléments d'un row par une virgule ',' et chaque row par un point-virgule ';'
                    string valueAsString = reader.GetValue(i).ToString();
                    if (i == reader.FieldCount - 1)
                    {
                        currentRowAsString += valueAsString;
                    }
                    else
                    {
                        currentRowAsString += valueAsString + ",";
                    }

                }
                resReq += currentRowAsString + ";"; 
            }

            connection.Close();

            // on crée une liste de liste de string que l'on va remplir avec resReq puis on va la return
            List<List<string>> reponse = new List<List<string>>();
            string[] temp = resReq.Split(';');
            for (int i = 0; i < temp.Length - 1; i++) // lenght -1 parce que dernier élément vide
            {
                reponse.Add(temp[i].Split(',').ToList());
            }

            return reponse;
        }


        /// <summary>
        /// Méthode permettant d'exécuter n'importe quelle query MySQL sur la database, autres que les "select"
        /// </summary>
        /// <param name="requete"> Query MySQL appliquée à la database </param>
        /// <returns> un message d'erreur si la query MySQL est incorrecte </returns>
        public static string Insert_Requete(string requete)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=cooking;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(requete, connection);
                cmd.ExecuteNonQuery();
                connection.Close();
                return "";
            }
            catch (Exception ex)
            {
                connection.Close();
                return ex.Message;
            }

            
        }


        /// <summary>
        /// Méthode nous permettant d'exécuter un script MySQL (enregistré sous le format .txt)
        /// </summary>
        /// <param name="fichier"> nom + extension du fichier se situant dans Bin/Debug </param>
        public static void Execution_Script_TXT(string fichier)
        {
            // Récupération des commandes qui sont dans le fichier .txt -> on les place toutes dans un seul string
            string line;
            string commandes_in_file = "";
            System.IO.StreamReader file = new System.IO.StreamReader(fichier);
            while ((line = file.ReadLine()) != null)
            {
                commandes_in_file += line;
            }
            file.Close();

            // exécution des commandes sur MySQL
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=cooking;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = commandes_in_file;

            MySqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception)
            {
            }
        }
    }
}
