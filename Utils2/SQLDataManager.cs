using System;
using System.IO;
using System.Collections.Generic;

using System.Data.SQLite;

namespace mapKnight.Utils
{
	public class SQLManager
	{
		//http://developer.xamarin.com/guides/cross-platform/application_fundamentals/data/part_4_using_adonet/

		static string DataFilePath;

		SQLiteConnection DataBase;

		public SQLManager (string databasefile)
		{
			DataFilePath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal).ToString (), Path.ChangeExtension (databasefile, ".db3"));

			if (!File.Exists (DataFilePath)) {
				//Wenn die Datenbank nicht existiert wird sie erstellt
				SQLiteConnection.CreateFile (DataFilePath);
			}

			DataBase = new SQLiteConnection ("Data Source=" + DataFilePath);
			DataBase.Open ();
		}

		public string Get (string database, string name)
		{
			using (SQLiteCommand Command = DataBase.CreateCommand ()) {
				Command.CommandText = "SELECT * FROM [" + database + "] WHERE name='" + name + "';";
				SQLiteDataReader CommandExecuteReader = Command.ExecuteReader ();

				//liest die Daten der Datenbank in ein Dictionary
				Dictionary<string, string> ReadData = new Dictionary<string, string> ();
				while (CommandExecuteReader.Read ()) {
					ReadData.Add (CommandExecuteReader ["name"].ToString (), CommandExecuteReader ["value"].ToString ());
				}
				CommandExecuteReader.Close ();

				if (ReadData.Count > 0) {
					//wenn Daten ausgelesen wurden
					return ReadData [name];
				} else {
					return null;
				}
			}
		}

		public void Create (string database, string name, string value)
		{
			using (SQLiteCommand Command = DataBase.CreateCommand ()) {
					
				//  neuer Datensatz angelegt
				Command.CommandText = "INSERT INTO [" + database + "] (name, value) VALUES ('" + name + "','" + value + "')";
				Command.ExecuteNonQuery ();
			}
		}

		public void Create (string databasename)
		{
			using (SQLiteCommand Command = DataBase.CreateCommand ()) {
				//Erstellen der Datenbank
				Command.CommandText = "CREATE TABLE IF NOT EXISTS [" + databasename + "] (name CHAR(30) PRIMARY KEY, value CHAR(70));";
				Command.ExecuteNonQuery ();
			}
		}

		public string GetOrCreate (string database, string name, string defaultvalue = "default")
		{
			using (SQLiteCommand Command = DataBase.CreateCommand ()) {
				Command.CommandText = "SELECT * FROM [" + database + "]";
				SQLiteDataReader CommandExecuteReader = Command.ExecuteReader ();

				//liest die Daten der Datenbank in ein Dictionary
				Dictionary<string, string> ReadData = new Dictionary<string, string> ();
				while (CommandExecuteReader.Read ()) {
					ReadData.Add (CommandExecuteReader ["name"].ToString (), CommandExecuteReader ["value"].ToString ());
				}
				CommandExecuteReader.Close ();

				if (ReadData.Count > 0) {
					//wenn Daten ausgelesen wurden
					if (ReadData.ContainsKey (name)) {
						return ReadData [name];
					} else
						Command.CommandText = "INSERT INTO [" + database + "] (name, value) VALUES ('" + name + "','" + defaultvalue + "')";
					Command.ExecuteNonQuery ();
					return defaultvalue;
				} else {
					//sonst wird ein neuer Datensatz angelegt
					Command.CommandText = "INSERT INTO [" + database + "] (name, value) VALUES ('" + name + "','" + defaultvalue + "')";
					Command.ExecuteNonQuery ();
					return defaultvalue;
				}
			}
		}

		public void SetOrCreate (string database, string name, string value)
		{
			if (Get (database, name) == null) {
				Create (database, name, value);
			} else {
				Set (database, name, value);
			}
		}

		public void Set (string database, string name, string value)
		{
			using (SQLiteCommand Command = DataBase.CreateCommand ()) {
				Command.CommandText = "UPDATE [" + database + "] SET value='" + value + "' WHERE name='" + name + "';";
				Command.ExecuteNonQuery ();
			}
		}

		public void Delete (string database, string name)
		{
			using (SQLiteCommand Command = DataBase.CreateCommand ()) {
				Command.CommandText = "DELETE FROM [" + database + "] WHERE name='" + name + "';";
				Command.ExecuteNonQuery ();
			}
		}

		public void Close ()
		{
			DataBase.Close ();
		}
	}
}