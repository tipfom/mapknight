using System;
using System.IO;
using System.Data;
using System.Collections.Generic;

using Mono.Data.Sqlite;

namespace mapKnight_Android
{
	public class SQLManager
	{
		//http://developer.xamarin.com/guides/cross-platform/application_fundamentals/data/part_4_using_adonet/

		static string DataFilePath;

		SqliteConnection DataBase;

		string DataPath;

		public bool DatabaseCreationRequired;

		public SQLManager (string databasefile)
		{
			DataFilePath = System.IO.Path.Combine (System.Environment.GetFolderPath (Environment.SpecialFolder.Personal).ToString (), databasefile + ".db3");

			if (!File.Exists (DataFilePath)) {
				Log.All (this, "Created new Databasefile (" + DataFilePath + ")", MessageType.Debug);
				//Wenn die Datenbank nicht existiert wird sie erstellt
				SqliteConnection.CreateFile (DataFilePath);
				DataBase = new SqliteConnection ("Data Source=" + DataFilePath);

				int rowcount;

				DataBase.Open ();
				using (SqliteCommand Command = DataBase.CreateCommand ()) {
					//Erstellen der Datenbank
					Command.CommandText = "CREATE TABLE data(name CHAR(30), value CHAR(70));";
					rowcount = Command.ExecuteNonQuery ();
				}
				DataBase.Close ();

				DatabaseCreationRequired = true;
			} else {
				DataBase = new SqliteConnection ("Data Source=" + DataFilePath);
				Log.All (this, "Set DataBase to " + DataBase.ToString (), MessageType.Debug);
			}
			DataPath = DataFilePath;
		}

		public string Get (string name)
		{
			DataBase.Open ();
			using (SqliteCommand Command = DataBase.CreateCommand ()) {
				Command.CommandText = "SELECT * FROM [data]";
				SqliteDataReader CommandExecuteReader = Command.ExecuteReader ();

				//liest die Daten der Datenbank in ein Dictionary
				Dictionary<string, string> ReadData = new Dictionary<string, string> ();
				while (CommandExecuteReader.Read ()) {
					ReadData.Add (CommandExecuteReader ["name"].ToString (), CommandExecuteReader ["value"].ToString ());
				}
				CommandExecuteReader.Close ();

				if (ReadData.Count > 0) {
					//wenn Daten ausgelesen wurden
					if (ReadData.ContainsKey (name)) {
						DataBase.Close ();
						return ReadData [name];
					}
					DataBase.Close ();
					return "<$#novalue#$>";
				} else {
					return "<$#novalue#$>";
				}
			}
		}

		public void Create (string name, string value)
		{
			DataBase.Open ();
			using (SqliteCommand Command = DataBase.CreateCommand ()) {
					
				//  neuer Datensatz angelegt
				Command.CommandText = "INSERT INTO [data] ([name], [value]) VALUES ('" + name + "','" + value + "')";
				Command.ExecuteNonQuery ();
				DataBase.Close ();
			}
		}

		public string GetOrCreate (string name, string defaultvalue = "default")
		{
			DataBase.Open ();
			using (SqliteCommand Command = DataBase.CreateCommand ()) {
				Command.CommandText = "SELECT * FROM [data]";
				SqliteDataReader CommandExecuteReader = Command.ExecuteReader ();

				//liest die Daten der Datenbank in ein Dictionary
				Dictionary<string, string> ReadData = new Dictionary<string, string> ();
				while (CommandExecuteReader.Read ()) {
					ReadData.Add (CommandExecuteReader ["name"].ToString (), CommandExecuteReader ["value"].ToString ());
				}
				CommandExecuteReader.Close ();

				if (ReadData.Count > 0) {
					//wenn Daten ausgelesen wurden
					if (ReadData.ContainsKey (name)) {
						DataBase.Close ();
						return ReadData [name];
					} else
						Command.CommandText = "INSERT INTO [data] ([name], [value]) VALUES ('" + name + "','" + defaultvalue + "')";
					Command.ExecuteNonQuery ();
					DataBase.Close ();
					return defaultvalue;
				} else {
					//sonst wird ein neuer Datensatz angelegt
					Command.CommandText = "INSERT INTO [data] ([name], [value]) VALUES ('" + name + "','" + defaultvalue + "')";
					Command.ExecuteNonQuery ();
					DataBase.Close ();
					return defaultvalue;
				}
			}
		}

		public void Set (string name, string value)
		{
			DataBase.Open ();
			using (SqliteCommand Command = DataBase.CreateCommand ()) {
				Command.CommandText = "UPDATE [data] SET [value]='" + value + "' WHERE [name]='" + name + "';";
				Command.ExecuteNonQuery ();
			}
			DataBase.Close ();
		}

		public void Delete (string name)
		{
			DataBase.Open ();
			using (SqliteCommand Command = DataBase.CreateCommand ()) {
				Command.CommandText = "DELETE FROM [data] WHERE [name]='" + name + "';";

				Command.ExecuteNonQuery ();
			}
			DataBase.Close ();
		}
	}
}