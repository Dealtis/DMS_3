using System;
using SQLite;
using System.Collections.Generic;
using RaygunClient = Mindscape.Raygun4Net.RaygunClient;
using System.Threading.Tasks;
using Android.Util;

namespace DMS_3.BDD
{

	public class DBRepository
	{
		//Instance
		//private static DBRepository instance;

		static readonly string TAG = "X:" + typeof(ProcessDMS).Name;

		SQLiteConnectionString Path = new SQLiteConnectionString(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormDMS.db3"), true);

		//CREATE TABLE
		public bool CreateTable()
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				conn.CreateTable<TableUser>();
				conn.CreateTable<TablePositions>();
				conn.CreateTable<TableStatutPositions>();
				conn.CreateTable<TableMessages>();
				conn.CreateTable<TableNotifications>();
				conn.CreateTable<TableColis>();

				conn.Close();
				Log.Debug(TAG, $"CreateTable done in ${conn.BusyTimeout}");
				return true;
			}
			catch (SQLiteException ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				return false;
			}
		}

		//VERIF SI USER DEJA INTEGRER
		public bool user_AlreadyExist(string user_AndsoftUser, string user_TransicsUser, string user_Password, string user_UseSigna, string user_Societe)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				bool output = false;
				var table = conn.Table<TableUser>().Where(v => v.user_AndsoftUser.Equals(user_AndsoftUser)).Where(v => v.user_TransicsUser.Equals(user_TransicsUser)).Where(v => v.user_Password.Equals(user_Password)).Where(v => v.user_UseSigna.Equals(user_UseSigna)).Where(v => v.user_Societe.Equals(user_Societe));
				foreach (var item in table)
				{
					output = true;
				}

				conn.Close();
				return output;
			}
			catch (Exception ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				return false;
			}
		}

		//Insertion des DATS USER
		public string InsertDataUser(string user_AndsoftUser, string user_TransicsUser, string user_Password, string user_UseSigna, string User_Usepartic, string user_Societe)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				TableUser item = new TableUser();
				item.user_AndsoftUser = user_AndsoftUser;
				item.user_TransicsUser = user_TransicsUser;
				item.user_Password = user_Password;
				item.user_UseSigna = user_UseSigna;
				item.user_UsePartic = User_Usepartic;
				item.user_Societe = user_Societe;
				conn.Insert(item);

				conn.Close();
				return "Insertion" + user_AndsoftUser + " réussite";
			}
			catch (SQLiteException ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				return "Erreur : " + ex.Message;
			}
		}

		//Insertion des donnes des positions
		public string insertDataPosition(string idSegment, string codeLivraison, string numCommande, string nomClient, string refClient, string nomPayeur, string adresseLivraison, string CpLivraison, string villeLivraison, string dateHeure, string nbrColis, string nbrPallette, string poids, string adresseExpediteur, string CpExpediteur, string dateExpe, string villeExpediteur, string nomExpediteur, string instrucLivraison, string GROUPAGE, string poidsADR, string poidsQL, string typeMission, string typeSegment, string statutLivraison, string CR, string ASSIGNE, string dateBDD, string Datemission, string Ordremission, string planDeTransport, string Userandsoft, string nomClientLivraison, string villeClientLivraison, string positionPole, string imgpath)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				TablePositions item = new TablePositions();
				item.idSegment = idSegment;
				item.codeLivraison = codeLivraison;
				item.numCommande = numCommande;
				item.nomClient = nomClient;
				item.refClient = refClient;
				item.nomPayeur = nomPayeur;
				item.adresseLivraison = adresseLivraison;
				item.CpLivraison = CpLivraison;
				item.villeLivraison = villeLivraison;
				item.dateHeure = dateHeure;
				item.nbrColis = nbrColis;
				item.nbrPallette = nbrPallette;
				item.poids = poids;
				item.adresseExpediteur = adresseExpediteur;
				item.CpExpediteur = CpLivraison;
				item.dateExpe = dateExpe;
				item.villeExpediteur = villeExpediteur;
				item.nomExpediteur = nomExpediteur;
				item.CpExpediteur = CpExpediteur;
				item.instrucLivraison = instrucLivraison;
				item.groupage = GROUPAGE;
				item.poidsADR = poidsADR;
				item.poidsQL = poidsQL;
				item.typeMission = typeMission;
				item.typeSegment = typeSegment;
				item.StatutLivraison = statutLivraison;
				item.CR = CR;
				item.ASSIGNE = ASSIGNE;
				item.dateBDD = DateTime.Now.Day;
				item.Datemission = Datemission;
				item.Ordremission = Convert.ToInt32(Ordremission);
				item.planDeTransport = planDeTransport;
				item.Userandsoft = Userandsoft;
				item.nomClientLivraison = nomClientLivraison;
				item.villeClientLivraison = villeClientLivraison;
				item.imgpath = imgpath;
				item.positionPole = positionPole;

				conn.Insert(item);
				conn.Close();
				return "Insertion good";
			}
			catch (SQLiteException ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				return "Erreur : " + ex.Message;
			}
		}

		//Insertion des données Message
		public string insertDataMessage(string codeChauffeur, string utilisateurEmetteur, string texteMessage, int statutMessage, DateTime dateImportMessage, int typeMessage, int numMessage)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				TableMessages item = new TableMessages();
				item.codeChauffeur = codeChauffeur;
				item.utilisateurEmetteur = utilisateurEmetteur;
				item.texteMessage = texteMessage;
				item.statutMessage = statutMessage;
				item.dateImportMessage = dateImportMessage;
				item.typeMessage = typeMessage;
				item.numMessage = numMessage;
				conn.Insert(item);

				conn.Close();
				return "Insertion good";
			}
			catch (SQLiteException ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				return "Erreur : " + ex.Message;
			}
		}

		public string InsertDataColis(string numColis, string numCommande)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				TableColis item = new TableColis();
				item.numColis = numColis;
				item.numCommande = numCommande;
				item.flashage = false;
				conn.Insert(item);

				conn.Close();
				return "Insertion good";
			}
			catch (SQLiteException ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				return "Erreur : " + ex.Message;
			}
		}

		//Insertion des données STATUT MESSAGE
		public string InsertDataStatutMessage(int statutNotificationMessage, DateTime dateNotificationMessage, int numMessage, string numCommande, string groupage)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				TableNotifications item = new TableNotifications();
				item.statutNotificationMessage = statutNotificationMessage;
				item.dateNotificationMessage = dateNotificationMessage;
				item.numMessage = numMessage;
				item.numCommande = numCommande;
				item.groupage = groupage;
				conn.Insert(item);

				conn.Close();
				return "\n" + statutNotificationMessage + " " + numCommande;
			}
			catch (SQLiteException ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				return "Erreur : " + ex.Message;
			}
		}

		public string insertDataStatutpositions(string codesuiviliv, string statut, string libellesuiviliv, string commandesuiviliv, string memosuiviliv, string datesuiviliv, string datajson)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				TableStatutPositions item = new TableStatutPositions();
				item.commandesuiviliv = commandesuiviliv;
				item.codesuiviliv = codesuiviliv;
				item.statut = statut;
				item.libellesuiviliv = libellesuiviliv;
				item.memosuiviliv = memosuiviliv;
				item.datesuiviliv = datesuiviliv;
				item.datajson = datajson;
				conn.Insert(item);

				conn.Close();
				return "Insertion good";
			}
			catch (SQLiteException ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				return "Erreur : " + ex.Message;
			}
		}

		public string InsertLogService(String exeption, DateTime date, String description)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				TableLogService item = new TableLogService();
				item.exeption = exeption;
				item.date = date;
				item.description = description;
				conn.Insert(item);

				conn.Close();
				return "Insertion Log good";
			}
			catch (Exception ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				return "Erreur : " + ex.Message;
			}
		}

		public string InsertLogApp(String exeption, DateTime date, String description)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				TableLogApp item = new TableLogApp();
				item.exeption = exeption;
				item.date = date;
				item.description = description;
				conn.Insert(item);

				conn.Close();
				return "Insertion Log good";
			}
			catch (SQLiteException ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				return "Erreur : " + ex.Message;
			}
		}

		//USER CHECK LOGIN
		public bool user_Check(string user_AndsoftUserTEXT, string user_PasswordTEXT)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				bool output = false;

				var query = conn.Table<TableUser>().Where(v => v.user_AndsoftUser.Equals(user_AndsoftUserTEXT)).Where(v => v.user_Password.Equals(user_PasswordTEXT));

				foreach (var item in query)
				{
					output = true;
					var row = conn.Get<TableUser>(item.Id);
					row.user_IsLogin = true;
					conn.Update(row);

					conn.Close();
					Console.WriteLine("UPDATE GOOD" + row.user_IsLogin);
				}
				return output;
			}
			catch (Exception ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				Console.WriteLine(ex);
				return false;
			}
		}

		public string updatePosition(int idposition, string statut, string txtAnomalie, string txtRemarque, string codeAnomalie, string imgpath)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				string output = "";
				var row = conn.Get<TablePositions>(idposition);
				row.StatutLivraison = statut;
				row.remarque = txtRemarque;
				row.codeAnomalie = codeAnomalie;
				row.libeAnomalie = txtAnomalie;
				conn.Update(row);

				output = "UPDATE POSITIONS " + row.Id;
				conn.Close();
				return output;
			}
			catch (Exception ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				Console.WriteLine(ex);
				return "Erreur : " + ex.Message;
			}
		}

		public string updatePositionSuppliv(string numCommande)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				string output = "";
				var query = conn.Table<TablePositions>().Where(v => v.numCommande.Equals(numCommande));
				foreach (var item in query)
				{
					output = "YALO";
					var row = conn.Get<TablePositions>(item.Id);
					row.imgpath = "SUPPLIV";
					conn.Update(row);
					Console.WriteLine("UPDATE SUPPLIV" + row.numCommande);
				}
				conn.Close();
				return output;
			}
			catch (Exception ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				Console.WriteLine(ex);
				return "Erreur : " + ex.Message;
			}
		}

		public string updateColisFlash(string numColis)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				string output = "";
				var query = conn.Table<TableColis>().Where(v => v.numColis.Equals(numColis));
				output = "notexist";
				foreach (var item in query)
				{
					output = "exist";

					var row = conn.Get<TableColis>(item.Id);
					row.dateflashage = DateTime.Now;
					row.flashage = true;
					conn.Update(row);

					Console.WriteLine("UPDATE COLIS" + row.numColis);
				}
				Console.WriteLine(output);
				conn.Close();
				return output;
			}
			catch (Exception ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				Console.WriteLine(ex);
				return "Erreur : " + ex.Message;
			}
		}

		//public void resetColis() {
		//	var query = db.Table<TableColis>().Where(v => v.flashage.Equals(true));
		//	foreach (var item in query)
		//	{
		//		var row = db.Get<TableColis>(item.Id);
		//		row.flashage = false;
		//		db.Update(row);
		//	}

		//}

		public int is_colis_in_truck(string numColis)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);

			var output = int.MinValue;
			var query = conn.Table<TableColis>().Where(v => v.numColis.Equals(numColis));
			foreach (var item in query)
			{
				var req = conn.Table<TablePositions>().Where(v => v.numCommande.Equals(item.numCommande));
				foreach (var row in req)
				{
					output = row.Id;
				}
			}
			conn.Close();
			return output;
		}

		internal int is_position_in_truck(string num)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);

			var output = int.MinValue;
			var query = conn.Table<TablePositions>().Where(v => v.numCommande.Equals(num));
			foreach (var item in query)
			{
				output = item.Id;
			}

			conn.Close();
			return output;
		}

		internal bool is_colis_in_currentPos(string numColis, string numCommande)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);

			bool output = false;
			var query = conn.Table<TableColis>().Where(v => v.numColis.Equals(numColis)).Where(v => v.numCommande.Equals(numCommande));

			foreach (var item in query)
			{
				output = true;
			}

			conn.Close();
			return output;
		}

		//USER CHECK LOGIN
		public string is_user_Log_In()
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);

			string output = "false";
			var query = conn.Table<TableUser>().Where(v => v.user_IsLogin.Equals(true));
			foreach (var item in query)
			{
				output = item.user_AndsoftUser;
				Console.WriteLine("\nUSER CONNECTE" + item.user_AndsoftUser);
			}
			conn.Close();
			return output;

		}//USER CHECK SIGNA
		public bool is_user_Sign(string userAndsoft)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);
			bool output = false;
			var query = conn.Table<TableUser>().Where(v => v.user_AndsoftUser.Equals(userAndsoft));
			foreach (var item in query)
			{
				if (item.user_UseSigna == "1")
				{
					output = true;
				}
			}
			conn.Close();
			return output;
		}

		internal string GetSociete(string userAndsoft)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);

			string output = "";
			var query = conn.Table<TableUser>().Where(v => v.user_AndsoftUser.Equals(userAndsoft));
			foreach (var item in query)
			{
				output = item.user_Societe;
			}

			conn.Close();
			return output;
		}

		//setUserdata
		public string setUserdata(string userAndsoft)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				string output = string.Empty;
				var query = conn.Table<TableUser>().Where(v => v.user_AndsoftUser.Equals(userAndsoft));
				foreach (var item in query)
				{
					Data.userAndsoft = item.user_AndsoftUser;
					Data.userTransics = item.user_TransicsUser;
					output = "setUserdata good";
					Console.WriteLine("\nUSER CONNECTE" + item.user_AndsoftUser);
				}

				conn.Close();
				return output;
			}
			catch (Exception ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				Console.WriteLine(ex);
				return "Erreur : " + ex.Message;
			}
		}

		public string getUserAndsoft()
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				string output = string.Empty;
				var query = conn.Table<TableUser>().Where(v => v.user_IsLogin.Equals(true));
				foreach (var item in query)
				{
					output = item.user_AndsoftUser;
				}

				conn.Close();
				return output;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				return "Erreur : " + ex.Message;
			}
		}

		public string getUserTransics()
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				string output = string.Empty;
				var query = conn.Table<TableUser>().Where(v => v.user_IsLogin.Equals(true));
				foreach (var item in query)
				{
					output = item.user_TransicsUser;
				}

				return output;
			}
			catch (Exception ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				Console.WriteLine(ex);
				return "Erreur : " + ex.Message;
			}
		}

		public string getAnomalieImgPath(string numCommande)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				string output = string.Empty;
				var query = conn.Table<TablePositions>().Where(v => v.numCommande.Equals(numCommande)).Where(v => v.StatutLivraison.Equals("2"));
				foreach (var item in query)
				{
					output = item.imgpath;
				}

				return output;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				return "Erreur : " + ex.Message;
			}
		}

		internal int CountColis(string num)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);

			return conn.Table<TableColis>().Where(v => v.numCommande.Equals(num)).Count();
		}

		internal int CountColisFlash(string num)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);
			return conn.Table<TableColis>().Where(v => v.numCommande.Equals(num)).Where(v => v.flashage.Equals(true)).Count();
		}

		public void resetColis(string num)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);

			var query = conn.Table<TableColis>().Where(v => v.numCommande.Equals(num));
			foreach (var item in query)
			{
				item.flashage = false;
				conn.Update(item);
			}

		}

		public List<TablePositions> CountMatiereDang(string groupage)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);
			return conn.Query<TablePositions>("SELECT SUM(poidsADR) as poidsADR, SUM(poidsQL) as poidsQL FROM TablePositions WHERE StatutLivraison ='0' AND groupage = ?", groupage);
		}

		public string logout()
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);
				string output = string.Empty;
				var query = conn.Table<TableUser>().Where(v => v.user_IsLogin.Equals(true));
				foreach (var item in query)
				{
					var row = conn.Get<TableUser>(item.Id);
					row.user_IsLogin = false;
					conn.Update(row);
					output = "UPDATE USER LOGOUT " + row.user_AndsoftUser;
				}

				conn.Close();
				return output;
			}
			catch (Exception ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				return "Erreur : " + ex.Message;
			}
		}

		//VERIF SI POS DEJA INTEGRER
		public bool pos_AlreadyExist(string numCommande, string groupage, string typeMission, string typeSegment)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);
				bool output = false;
				var table = conn.Table<TablePositions>().Where(v => v.numCommande.Equals(numCommande)).Where(v => v.groupage.Equals(groupage)).Where(v => v.typeMission.Equals(typeMission)).Where(v => v.typeSegment.Equals(typeSegment));
				foreach (var item in table)
				{
					output = true;
				}
				conn.Close();
				return output;
			}
			catch (Exception ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				Console.WriteLine(ex);
				return false;
			}
		}

		//supp notification
		public string deletenotif(int id)
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				conn.Delete<TableNotifications>(id);
				string result = "delete";

				conn.Close();
				return result;
			}
			catch (SQLiteException ex)
			{
				return "Erreur : " + ex.Message;
			}
		}

		//SELECT PAR ID
		public TablePositions GetPositionsData(int id)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);

			TablePositions data = new TablePositions();
			var item = conn.Get<TablePositions>(id);
			if (item != null)
			{
				data.codeLivraison = item.codeLivraison;
				data.numCommande = item.numCommande;
				data.nomClient = item.nomClient;
				data.refClient = item.refClient;
				data.nomPayeur = item.nomPayeur;
				data.adresseLivraison = item.adresseLivraison;
				data.CpLivraison = item.CpLivraison;
				data.villeLivraison = item.villeLivraison;
				data.dateHeure = item.dateHeure;
				data.dateExpe = item.dateExpe;
				data.nbrColis = item.nbrColis;
				data.nbrPallette = item.nbrPallette;
				data.adresseExpediteur = item.adresseExpediteur;
				data.CpExpediteur = item.CpExpediteur;
				data.villeExpediteur = item.villeExpediteur;
				data.nomExpediteur = item.nomExpediteur;
				data.StatutLivraison = item.StatutLivraison;
				data.instrucLivraison = item.instrucLivraison;
				data.groupage = item.groupage;
				data.poidsADR = item.poidsADR;
				data.poidsQL = item.poidsQL;
				data.planDeTransport = item.planDeTransport;
				data.typeMission = item.typeMission;
				data.typeSegment = item.typeSegment;
				data.CR = item.CR;
				data.ASSIGNE = item.ASSIGNE;
				data.nomClientLivraison = item.nomClientLivraison;
				data.villeClientLivraison = item.villeClientLivraison;
				data.Datemission = item.Datemission;
				data.Ordremission = item.Ordremission;
				data.Userandsoft = item.Userandsoft;
				data.remarque = item.remarque;
				data.codeAnomalie = item.codeAnomalie;
				data.libeAnomalie = item.libeAnomalie;
				data.imgpath = item.imgpath;
				data.Id = item.Id;
				data.positionPole = item.positionPole;

				if (item.poids != "")
				{
					if (Convert.ToDouble(item.poids.Replace('.', ',')) < 1)
					{
						data.poids = Convert.ToDouble(item.poids.Replace('.', ',')) * 1000 + " kg";
					}
					else {
						data.poids = item.poids + " tonnes";
					}

				}
				else
				{
					data.poids = "Poids inconnu";
				}

			}

			conn.Close();
			return data;
		}

		public int GetidPosition(string numCommande)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);

			int id = 0;
			var query = conn.Table<TablePositions>().Where(v => v.numCommande.Equals(numCommande));

			foreach (var row in query)
			{
				id = row.Id;
			}
			conn.Close();
			return id;
		}

		public int GetidPrev(int id)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);

			int idprev;
			//get int ordremission
			var item = conn.Get<TablePositions>(id);
			idprev = (item.Ordremission) - 1;
			//getordremission -1
			var query = conn.Table<TablePositions>().Where(v => v.Ordremission.Equals(idprev));
			//getordremission -1
			foreach (var row in query)
			{
				idprev = row.Id;
			}
			if (idprev < 0)
			{
				idprev = 0;
			}
			conn.Close();
			return idprev;
		}

		public int GetidNext(int id)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);

			int idnext;
			//get int ordremission
			var item = conn.Get<TablePositions>(id);
			idnext = item.Ordremission + 1;
			var query = conn.Table<TablePositions>().Where(v => v.Ordremission.Equals(idnext));
			foreach (var row in query)
			{
				idnext = row.Id;
			}

			if (idnext < 0)
			{
				idnext = 0;
			}
			conn.Close();
			return idnext;
		}

		internal void updatePositionOrder(string idSegment, int Ordremission)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);

			var query = conn.Table<TablePositions>().Where(v => v.idSegment.Equals(idSegment));
			foreach (var item in query)
			{
				item.Ordremission = Ordremission;
				conn.Update(item);
			}
		}

		public string updateposimgpath(int i, string path)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);

			string output = "";
			var row = conn.Get<TablePositions>(i);
			row.imgpath = path;
			conn.Update(row);
			output = "UPDATE POSITIONS " + row.Id;

			conn.Close();
			return output;
		}

		public string DropTableMessage()
		{
			try
			{
				var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
				conn.BusyTimeout = TimeSpan.FromSeconds(2);

				conn.DeleteAll<TableMessages>();
				string result = "delete";

				conn.Close();
				return result;
			}
			catch (SQLiteException ex)
			{
				RaygunClient.Current.SendInBackground(ex); Xamarin.Insights.Report(ex);
				return "Erreur : " + ex.Message;
			}
		}

		//GET NUMBER LIV RAM ET MSG
		public int SETBadges(string userandsoft)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);

			var cLIV = conn.Table<TablePositions>().Where(v => v.Userandsoft.Equals(userandsoft)).Where(v => v.typeMission.Equals("L")).Where(v => v.typeSegment.Equals("LIV")).Where(v => v.StatutLivraison.Equals("0")).Count();
			var cRam = conn.Table<TablePositions>().Where(v => v.Userandsoft.Equals(userandsoft)).Where(v => v.typeMission.Equals("C")).Where(v => v.typeSegment.Equals("RAM")).Where(v => v.StatutLivraison.Equals("0")).Count();
			var cMsg = conn.Table<TableMessages>().Where(v => v.statutMessage.Equals(0)).Count();

			var cSUPPLIV = conn.Table<TablePositions>().Where(v => v.imgpath.Equals("SUPPLIV")).Count();

			Data.Instance.setLivraisonIndicator(cLIV - cSUPPLIV);
			Data.Instance.setEnlevementIndicator(cRam);
			Data.Instance.setMessageIndicator(cMsg);

			conn.Close();
			return 0;
		}

		internal object Execute(string stringinsertpos)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);
			var exec = conn.Execute(stringinsertpos);
			return exec;
		}

		internal List<TablePositions> QueryGRP(string stringSelect, string tyM, string tyS, string userAndsoft)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);
			var grp = conn.Query<TablePositions>(stringSelect, 0, tyM, tyS, userAndsoft);
			return grp;
		}

		internal List<TablePositions> QueryGRPTRAIT(string stringSelect, string tyM, string tyS, string userAndsoft, string tyMB, string tySB, string userAndsoftB)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);
			var grp = conn.Query<TablePositions>(stringSelect, 1, tyM, tyS, userAndsoft, 2, tyMB, tySB, userAndsoftB);
			return grp;
		}

		internal List<TablePositions> QueryPositions(string requete)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);
			var table = conn.Query<TablePositions>(requete);
			conn.Close();
			return table;
		}

		internal List<TableColis> QueryColis(string requete)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);
			var table = conn.Query<TableColis>(requete);
			conn.Close();
			return table;
		}

		internal List<TableNotifications> QueryNotif(string v)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);
			var notifs = conn.Query<TableNotifications>(v);
			conn.Close();
			return notifs;
		}
		internal List<TableMessages> QueryMessage(string v)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);
			var messages = conn.Query<TableMessages>(v);
			conn.Close();
			return messages;
		}
		internal List<TableStatutPositions> QueryStatuPos(string v)
		{
			var conn = new SQLiteConnectionWithLock(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			conn.BusyTimeout = TimeSpan.FromSeconds(2);
			var status = conn.Query<TableStatutPositions>(v);
			conn.Close();
			return status;
		}
	}
}
