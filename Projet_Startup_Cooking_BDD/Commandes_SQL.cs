using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;

namespace Projet_Startup_Cooking_BDD
{
    public class Commandes_SQL
    {
    
        /// <summary>
        /// Méthode permettant d'obtenir sous forme de liste de liste de string le résultat d'une query sql sur la database
        /// </summary>
        /// <param name="requete"> Query SQL appliquée à la db </param>
        /// <returns> Une List<List<string>> contenant les réponses  </returns>
        public static List<List<string>> Select_Requete(string requete)
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides !!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=cooking;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = requete;

            MySqlDataReader reader;
            reader = command.ExecuteReader();
            string resReq = "";
            while (reader.Read())                           // parcours ligne par ligne
            {
                string currentRowAsString = "";
                for (int i = 0; i < reader.FieldCount; i++)    // parcours cellule par cellule
                {
                    string valueAsString = reader.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                    if (i == reader.FieldCount - 1)
                    {
                        currentRowAsString += valueAsString;
                    }
                    else
                    {
                        currentRowAsString += valueAsString + ",";
                    }

                }
                resReq += currentRowAsString + ";";   // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
            }

            connection.Close();
            List<List<string>> reponse = new List<List<string>>();
            string[] temp = resReq.Split(';');
            for (int i = 0; i < temp.Length - 1; i++) // lenght -1 parce que dernier élément vide
            {
                reponse.Add(temp[i].Split(',').ToList());
            }

            return reponse;
        }


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
        /// Fonction permettant d'exécuter un script SQL (enregistré sous le format .txt)
        /// </summary>
        /// <param name="fichier"> Chemin du fichier </param>
        public static void Execution_Script_TXT(string fichier)
        {
            //! Récupération des commandes qui sont dans le fichier .txt
            string line;
            string commandes_in_file = "";
            System.IO.StreamReader file = new System.IO.StreamReader(fichier);
            while ((line = file.ReadLine()) != null)
            {
                commandes_in_file += line;
            }
            file.Close();

            //! Exécution des commandes sur MySQL
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
