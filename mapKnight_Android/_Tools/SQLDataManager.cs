using System;
using System.IO;
using System.Data;
using System.Collections.Generic;

using Mono.Data.Sqlite;

namespace mapKnight_Android
{
	namespace Utils
	{
		public class SQLDataManager
		{
			//http://developer.xamarin.com/guides/cross-platform/application_fundamentals/data/part_4_using_adonet/

			static string DataFilePath = System.IO.Path.Combine (System.Environment.GetFolderPath (Environment.SpecialFolder.Personal).ToString (), "_game_database_.db3");

			SqliteConnection DataBase;

			string DataPath;

			public bool DatabaseCreationRequired;

			public SQLDataManager ()
			{
				if (!File.Exists (DataFilePath)) {
					Log.All (this, "Created new Databasefile (" + DataFilePath + ")", MessageType.Debug);
					//Wenn die Datenbank nicht existiert wird sie erstellt
					SqliteConnection.CreateFile (DataFilePath);
					DataBase = new SqliteConnection ("Data Source=" + DataFilePath);

					int rowcount;

					DataBase.Open ();
					using (SqliteCommand Command = DataBase.CreateCommand ()) {
						//Erstellen der Datenbank für int-Werte
						Command.CommandText = "CREATE TABLE intdata(name CHAR(30), value INT);";
						rowcount = Command.ExecuteNonQuery ();
						//Erstellen der Datenbank für string-Werte
						Command.CommandText = "CREATE TABLE stringdata(name CHAR(30), value CHAR(50));";
						rowcount = Command.ExecuteNonQuery ();
					}
					DataBase.Close ();

					DatabaseCreationRequired = true;
				} else {
					DataBase = new SqliteConnection ("Data Source=" + DataFilePath);
					Log.All (this, "Set DataBase to " + DataBase.ToString(), MessageType.Debug);
				}
				DataPath = DataFilePath;
			}

			public int GetOrCreate (string name, int defaultvalue = 0)
			{
				if (name.StartsWith ("int:")) {
					DataBase.Open ();
					using (SqliteCommand Command = DataBase.CreateCommand ()) {
						Command.CommandText = "SELECT * FROM [intdata]";
						SqliteDataReader CommandExecuteReader = Command.ExecuteReader ();

						//liest die Daten der Datenbank in ein Dictionary
						Dictionary<string, int> ReadData = new Dictionary<string, int> ();
						while (CommandExecuteReader.Read ()) {
							ReadData.Add (CommandExecuteReader ["name"].ToString (), Convert.ToInt32 (CommandExecuteReader ["value"].ToString ()));
						}
						CommandExecuteReader.Close ();

						if (ReadData.Count > 0) {
							//wenn Daten ausgelesen wurden
							if (ReadData.ContainsKey (name)) 
							{
								DataBase.Close ();
								return ReadData [name];
							}
							else
								Command.CommandText = "INSERT INTO [intdata] ([name], [value]) VALUES ('" + name + "', '" + defaultvalue + "');";
							Command.ExecuteNonQuery ();
							DataBase.Close ();
							return defaultvalue;
						} else {
							//sonst wird ein neuer Datensatz angelegt
							Command.CommandText = "INSERT INTO [intdata] ([name], [value]) VALUES ('" + name + "', '" + defaultvalue + "');";
							Command.ExecuteNonQuery ();
							DataBase.Close ();
							return defaultvalue;
						}
					}
				} else {
					throw new ArgumentException ("wrong format of dataset name (originalname=" + name + ") put a 'int:' before the name");
				}
			}

			public string GetOrCreate (string name, string defaultvalue = "default")
			{
				if (name.StartsWith ("string:")) {
					DataBase.Open ();
					using (SqliteCommand Command = DataBase.CreateCommand ()) {
						Command.CommandText = "SELECT * FROM [stringdata]";
						SqliteDataReader CommandExecuteReader = Command.ExecuteReader ();

						//liest die Daten der Datenbank in ein Dictionary
						Dictionary<string, string> ReadData = new Dictionary<string, string> ();
						while (CommandExecuteReader.Read ()) {
							ReadData.Add (CommandExecuteReader ["name"].ToString (), CommandExecuteReader ["value"].ToString ());
						}
						CommandExecuteReader.Close ();

						if (ReadData.Count > 0) {
							//wenn Daten ausgelesen wurden
							if (ReadData.ContainsKey (name)) 
							{
								DataBase.Close ();
								return ReadData [name];
							}
							else
								Command.CommandText = "INSERT INTO [stringdata] ([name], [value]) VALUES ('" + name + "','" + defaultvalue + "')";
							Command.ExecuteNonQuery ();
							DataBase.Close ();
							return defaultvalue;
						} else {
							//sonst wird ein neuer Datensatz angelegt
							Command.CommandText = "INSERT INTO [stringdata] ([name], [value]) VALUES ('" + name + "','" + defaultvalue + "')";
							Command.ExecuteNonQuery ();
							DataBase.Close ();
							return defaultvalue;
						}
					}
				} else if (name.StartsWith ("int:")) {
					return GetOrCreate (name, 0).ToString ();
				} else {
					throw new ArgumentException ("wrong format of dataset name (originalname=" + name + ") put a 'string:' before the name");
				}
			}

			public void Set (string name, string value)
			{
				DataBase.Open ();
				using (SqliteCommand Command = DataBase.CreateCommand ()) {
					if (name.StartsWith ("int")) {
						//wenn die Zahl eine Nummer ist
						Command.CommandText = "UPDATE [intdata] SET [value]='" + value + "' WHERE [name]='" + name + "';";
					} else {
						Command.CommandText = "UPDATE [stringdata] SET [value]='" + value + "' WHERE [name]='" + name + "';";
					}
					Command.ExecuteNonQuery ();
				}
				DataBase.Close ();
			}

			public void Delete (string name)
			{
				DataBase.Open ();
				using (SqliteCommand Command = DataBase.CreateCommand ()) {
					if (name.StartsWith ("int")) {
						Command.CommandText = "DELETE FROM [intdata] WHERE [name]='" + name + "';";
					} else {
						Command.CommandText = "DELETE FROM [stringdata] WHERE [name]='" + name + "';";
					}

					Command.ExecuteNonQuery ();
				}
				DataBase.Close ();
			}

			public bool BeginRead ()
			{
				Log.All (this, "Begin Reading from " + DataBase.DataSource, MessageType.Debug);
				return true;
			}

			public bool EndRead ()
			{
				Log.All (this, "Ended Reading from " + DataBase.DataSource, MessageType.Debug);
				return true;
			}
		}
	}
}