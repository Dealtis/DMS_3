using System;
using SQLite;
using Xamarin;
using Mono.Data.Sqlite;
using System.Collections.Generic;

namespace DMS_3.BDD
{
	public class DBRepository
	{
		private static SQLiteConnection db;

		//CREATE BDD
		public string CreateDB()
		{
			try
			{
				var output = "";
				output += "Création de la BDD";
				db = new SQLiteConnection(System.IO.Path.Combine(Environment.GetFolderPath
					(Environment.SpecialFolder.Personal), "ormDMS.db3"));
				output += "\nBDD crée...";
				return output;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return "error";
			}
		}

		//CREATE TABLE
		public string CreateTable()
		{
			try
			{
				db.CreateTable<TableUser>();
				db.CreateTable<TablePositions>();
				db.CreateTable<TableStatutPositions>();
				db.CreateTable<TableMessages>();
				db.CreateTable<TableNotifications>();
				db.CreateTable<TableLogService>();
				db.CreateTable<TableColis>();
				string result = "Tables crées !";
				return result;
			}
			catch (SQLiteException ex)
			{
				return "Erreur : " + ex.Message;
			}
		}

		//VERIF SI USER DEJA INTEGRER
		public bool user_AlreadyExist(string user_AndsoftUser, string user_TransicsUser, string user_Password, string user_UsePartic)
		{
			bool output = false;
			var table = db.Table<TableUser>().Where(v => v.user_AndsoftUser.Equals(user_AndsoftUser)).Where(v => v.user_TransicsUser.Equals(user_TransicsUser)).Where(v => v.user_Password.Equals(user_Password)).Where(v => v.user_UsePartic.Equals(user_UsePartic));
			foreach (var item in table)
			{
				output = true;
			}
			return output;
		}

		//Insertion des DATS USER
		public string InsertDataUser(string user_AndsoftUser, string user_TransicsUser, string user_Password, string user_UsePartic)
		{
			try
			{
				TableUser item = new TableUser();
				item.user_AndsoftUser = user_AndsoftUser;
				item.user_TransicsUser = user_TransicsUser;
				item.user_Password = user_Password;
				item.user_UsePartic = user_UsePartic;
				db.Insert(item);
				return "Insertion" + user_AndsoftUser + " réussite";
			}
			catch (SQLiteException ex)
			{
				return "Erreur : " + ex.Message;
			}
		}

		//Insertion des donnes des positions
		public string insertDataPosition(string codeLivraison, string numCommande, string refClient, string nomPayeur, string nomExpediteur, string adresseExpediteur, string villeExpediteur, string cpExpediteur, string dateExpe, string nomClient, string adresseLivraison, string villeLivraison, string cpLivraison, string dateHeure, string poids, string nbrPallette, string nbrColis, string instrucLivraison, string typeMission, string typeSegment, string groupage, string poidsADR, string poidsQL, string statutLivraison, string cr, int dateBDD, string dateMission, int ordreMission, string planDeTransport, string userAndsoft, string nomClientLivraison, string villeClientLivraison, string imgpath)
		{
			try
			{
				TablePositions item = new TablePositions();

				item.codeLivraison = codeLivraison;
				item.numCommande = numCommande;
				item.nomClient = nomClient;
				item.refClient = refClient;
				item.nomPayeur = nomPayeur;
				item.adresseLivraison = adresseLivraison;
				item.CpLivraison = cpLivraison;
				item.villeLivraison = villeLivraison;
				item.dateHeure = dateHeure;
				item.nbrColis = nbrColis;
				item.nbrPallette = nbrPallette;
				item.poids = poids;
				item.adresseExpediteur = adresseExpediteur;
				item.CpExpediteur = cpExpediteur;
				item.dateExpe = dateExpe;
				item.villeExpediteur = villeExpediteur;
				item.nomExpediteur = nomExpediteur;
				item.instrucLivraison = instrucLivraison;
				item.groupage = groupage;
				item.poidsADR = poidsADR;
				item.poidsQL = poidsQL;
				item.typeMission = typeMission;
				item.typeSegment = typeSegment;
				item.StatutLivraison = statutLivraison;
				item.CR = cr;
				item.dateBDD = dateBDD;
				item.Datemission = dateMission;
				item.Ordremission = ordreMission;
				item.planDeTransport = planDeTransport;
				item.Userandsoft = userAndsoft;
				item.nomClientLivraison = nomClientLivraison;
				item.villeClientLivraison = villeClientLivraison;
				item.imgpath = imgpath;

				db.Insert(item);
				return "Insertion good";
			}
			catch (SQLiteException ex)
			{
				return "Erreur : " + ex.Message;
			}
		}
		//Insertion des données Message
		public string insertDataMessage(string codeChauffeur, string utilisateurEmetteur, string texteMessage, int statutMessage, DateTime dateImportMessage, int typeMessage, int numMessage)
		{
			try
			{
				TableMessages item = new TableMessages();
				item.codeChauffeur = codeChauffeur;
				item.utilisateurEmetteur = utilisateurEmetteur;
				item.texteMessage = texteMessage;
				item.statutMessage = statutMessage;
				item.dateImportMessage = dateImportMessage;
				item.typeMessage = typeMessage;
				item.numMessage = numMessage;
				db.Insert(item);
				return "Insertion good";
			}
			catch (SQLiteException ex)
			{
				return "Erreur : " + ex.Message;
			}
		}

		public string InsertDataColis(string numColis, string numCommande)
		{
			try
			{
				TableColis item = new TableColis();
				item.numColis = numColis;
				item.numCommande = numCommande;
				item.flashage = false;
				db.Insert(item);
				return "Insertion good";
			}
			catch (SQLiteException ex)
			{
				return "Erreur : " + ex.Message;
			}
		}

		//Insertion des données STATUT MESSAGE
		public string InsertDataStatutMessage(int statutNotificationMessage, DateTime dateNotificationMessage, int numMessage, string numCommande, string groupage)
		{
			try
			{
				TableNotifications item = new TableNotifications();
				item.statutNotificationMessage = statutNotificationMessage;
				item.dateNotificationMessage = dateNotificationMessage;
				item.numMessage = numMessage;
				item.numCommande = numCommande;
				item.groupage = groupage;
				db.Insert(item);
				return "\n" + statutNotificationMessage + " " + numCommande;
			}
			catch (SQLiteException ex)
			{
				return "Erreur : " + ex.Message;
			}
		}

		public string insertDataStatutpositions(string codesuiviliv, string statut, string libellesuiviliv, string commandesuiviliv, string memosuiviliv, string datesuiviliv, string datajson)
		{
			try
			{
				TableStatutPositions item = new TableStatutPositions();
				item.commandesuiviliv = commandesuiviliv;
				item.codesuiviliv = codesuiviliv;
				item.statut = statut;
				item.libellesuiviliv = libellesuiviliv;
				item.memosuiviliv = memosuiviliv;
				item.datesuiviliv = datesuiviliv;
				item.datajson = datajson;
				db.Insert(item);
				return "Insertion good";
			}
			catch (SQLiteException ex)
			{
				return "Erreur : " + ex.Message;
			}
		}

		public string InsertLogService(String exeption, DateTime date, String description)
		{
			try
			{
				TableLogService item = new TableLogService();
				item.exeption = exeption;
				item.date = date;
				item.description = description;
				db.Insert(item);
				return "Insertion Log good";
			}
			catch (Exception ex)
			{
				return "Erreur : " + ex.Message;
			}
		}

		public string InsertLogApp(String exeption, DateTime date, String description)
		{
			try
			{
				TableLogApp item = new TableLogApp();
				item.exeption = exeption;
				item.date = date;
				item.description = description;
				db.Insert(item);
				return "Insertion Log good";
			}
			catch (SQLiteException ex)
			{
				return "Erreur : " + ex.Message;
			}
		}

		//USER CHECK LOGIN
		public bool user_Check(string user_AndsoftUserTEXT, string user_PasswordTEXT)
		{
			try
			{
				bool output = false;

				var query = db.Table<TableUser>().Where(v => v.user_AndsoftUser.Equals(user_AndsoftUserTEXT)).Where(v => v.user_Password.Equals(user_PasswordTEXT));

				foreach (var item in query)
				{
					output = true;
					var row = db.Get<TableUser>(item.Id);
					row.user_IsLogin = true;
					db.Update(row);
					Console.WriteLine("UPDATE GOOD" + row.user_IsLogin);
				}
				return output;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public string updatePosition(int idposition, string statut, string txtAnomalie, string txtRemarque, string codeAnomalie, string imgpath)
		{
			try
			{
				string output = "";
				var row = db.Get<TablePositions>(idposition);
				row.StatutLivraison = statut;
				row.remarque = txtRemarque;
				row.codeAnomalie = codeAnomalie;
				row.libeAnomalie = txtAnomalie;
				db.Update(row);
				output = "UPDATE POSITIONS " + row.Id;
				return output;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return "Erreur : " + ex.Message;
			}
		}

		public string updatePositionSuppliv(string numCommande)
		{
			try
			{
				string output = "";
				var query = db.Table<TablePositions>().Where(v => v.numCommande.Equals(numCommande));
				foreach (var item in query)
				{
					output = "YALO";
					var row = db.Get<TablePositions>(item.Id);
					row.imgpath = "SUPPLIV";
					db.Update(row);
					Console.WriteLine("UPDATE SUPPLIV" + row.numCommande);
				}
				return output;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return "Erreur : " + ex.Message;
			}
		}

		public string updateColisFlash(string numColis)
		{
			try
			{
				string output = "";
				var query = db.Table<TableColis>().Where(v => v.numColis.Equals(numColis));
				output = "notexist";
				foreach (var item in query)
				{
					output = "exist";

					var row = db.Get<TableColis>(item.Id);
					row.dateflashage = DateTime.Now;
					row.flashage = true;
					db.Update(row);
					Console.WriteLine("UPDATE COLIS" + row.numColis);
				}
				Console.WriteLine(output);
				return output;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return "Erreur : " + ex.Message;
			}
		}

		public int is_colis_in_truck(string numColis)
		{
			var output = int.MinValue;
			var query = db.Table<TableColis>().Where(v => v.numColis.Equals(numColis));
			foreach (var item in query)
			{
				var req = db.Table<TablePositions>().Where(v => v.numCommande.Equals(item.numCommande));
				foreach (var row in req)
				{
					output = row.Id;
				}
			}
			return output;
		}

		internal int is_position_in_truck(string num)
		{
			var output = int.MinValue;
			var query = db.Table<TableColis>().Where(v => v.numCommande.Equals(num));
			foreach (var item in query)
			{
				var req = db.Table<TablePositions>().Where(v => v.numCommande.Equals(item.numCommande));
				foreach (var row in req)
				{
					output = row.Id;
				}
			}
			return output;
		}

		internal bool is_colis_in_currentPos(string numColis, string numCommande)
		{
			bool output = false;
			var query = db.Table<TableColis>().Where(v => v.numColis.Equals(numColis)).Where(v => v.numCommande.Equals(numCommande));

			foreach (var item in query)
			{
				output = true;
			}
			return output;
		}

		//USER CHECK LOGIN
		public string is_user_Log_In()
		{
			string output = "false";
			var query = db.Table<TableUser>().Where(v => v.user_IsLogin.Equals(true));
			foreach (var item in query)
			{
				output = item.user_AndsoftUser;
				Console.WriteLine("\nUSER CONNECTE" + item.user_AndsoftUser);
			}
			return output;
		}

		//setUserdata
		public string setUserdata(string userAndsoft)
		{
			try
			{
				string output = string.Empty;
				var query = db.Table<TableUser>().Where(v => v.user_AndsoftUser.Equals(userAndsoft));
				foreach (var item in query)
				{
					Data.userAndsoft = item.user_AndsoftUser;
					Data.userTransics = item.user_TransicsUser;
					output = "setUserdata good";
					Console.WriteLine("\nUSER CONNECTE" + item.user_AndsoftUser);
				}
				return output;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return "Erreur : " + ex.Message;
			}
		}

		public string getUserAndsoft()
		{
			try
			{
				string output = string.Empty;
				var query = db.Table<TableUser>().Where(v => v.user_IsLogin.Equals(true));
				foreach (var item in query)
				{
					output = item.user_AndsoftUser;
				}
				return output;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return "Erreur : " + ex.Message;
			}
		}

		public string getUserTransics()
		{
			try
			{
				string output = string.Empty;
				var query = db.Table<TableUser>().Where(v => v.user_IsLogin.Equals(true));
				foreach (var item in query)
				{
					output = item.user_TransicsUser;
				}
				return output;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return "Erreur : " + ex.Message;
			}
		}

		public string getAnomalieImgPath(string numCommande)
		{
			try
			{
				string output = string.Empty;
				var query = db.Table<TablePositions>().Where(v => v.numCommande.Equals(numCommande)).Where(v => v.StatutLivraison.Equals("2"));
				foreach (var item in query)
				{
					output = item.imgpath;
				}
				return output;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return "Erreur : " + ex.Message;
			}
		}

		internal int CountColis(string num)
		{
			return db.Table<TableColis>().Where(v => v.numCommande.Equals(num)).Count();
		}

		internal int CountColisFlash(string num)
		{
			return db.Table<TableColis>().Where(v => v.numCommande.Equals(num)).Where(v => v.flashage.Equals(true)).Count();
		}

		public List<TablePositions> CountMatiereDang(string groupage)
		{
			return db.Query<TablePositions>("SELECT SUM(poidsADR) as poidsADR, SUM(poidsQL) as poidsQL FROM TablePositions WHERE StatutLivraison ='0' AND groupage = ?", groupage);
		}

		public string logout()
		{
			try
			{
				string output = string.Empty;
				var query = db.Table<TableUser>().Where(v => v.user_IsLogin.Equals(true));
				foreach (var item in query)
				{
					var row = db.Get<TableUser>(item.Id);
					row.user_IsLogin = false;
					db.Update(row);
					output = "UPDATE USER LOGOUT " + row.user_AndsoftUser;
				}
				return output;
			}
			catch (Exception ex)
			{
				return "Erreur : " + ex.Message;
			}
		}

		//VERIF SI POS DEJA INTEGRER
		public bool pos_AlreadyExist(string numCommande, string groupage, string typeMission, string typeSegment)
		{
			try
			{
				bool output = false;
				var table = db.Table<TablePositions>().Where(v => v.numCommande.Equals(numCommande)).Where(v => v.groupage.Equals(groupage)).Where(v => v.typeMission.Equals(typeMission)).Where(v => v.typeSegment.Equals(typeSegment));
				foreach (var item in table)
				{
					output = true;
				}
				return output;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public string purgeLog()
		{
			//var query = db.Table<TableLog>().Where (v => v.date.CompareTo(DateTime.Now));
			var query = db.Table<TableLogService>();
			foreach (var item in query)
			{
				if (item.date.Day.CompareTo(DateTime.Now.Day) > 1)
				{
					var row = db.Get<TableLogService>(item.Id);
					db.Delete(row);
				}
			}

			var queryApp = db.Table<TableLogApp>();
			foreach (var item in queryApp)
			{
				if (item.date.Day.CompareTo(DateTime.Now.Day) > 1)
				{
					var row = db.Get<TableLogService>(item.Id);
					db.Delete(row);
				}
			}
			return "Log purgé";
		}

		//supp notification
		public string deletenotif(int id)
		{
			try
			{
				db.Delete<TableNotifications>(id);
				string result = "delete";
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
			TablePositions data = new TablePositions();
			var item = db.Get<TablePositions>(id);
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

			if (Convert.ToDouble(item.poids.Replace('.', ',')) < 1)
			{
				data.poids = Convert.ToDouble(item.poids.Replace('.', ',')) * 1000 + " kg";
			}
			else {
				data.poids = item.poids + " tonnes";
			}
			return data;
		}

		public int GetidPrev(int id)
		{
			int idprev;
			//get int ordremission
			var item = db.Get<TablePositions>(id);
			idprev = (item.Ordremission) - 1;
			//getordremission -1
			var query = db.Table<TablePositions>().Where(v => v.Ordremission.Equals(idprev));
			//getordremission -1
			foreach (var row in query)
			{
				idprev = row.Id;
			}
			if (idprev < 0)
			{
				idprev = 0;
			}
			return idprev;
		}

		public int GetidNext(int id)
		{
			int idnext;
			//get int ordremission
			var item = db.Get<TablePositions>(id);
			idnext = item.Ordremission + 1;
			var query = db.Table<TablePositions>().Where(v => v.Ordremission.Equals(idnext));
			foreach (var row in query)
			{
				idnext = row.Id;
			}

			if (idnext < 0)
			{
				idnext = 0;
			}
			return idnext;
		}

		public string updateposimgpath(int i, string path)
		{
			string output = "";
			var row = db.Get<TablePositions>(i);
			row.imgpath = path;
			db.Update(row);
			output = "UPDATE POSITIONS " + row.Id;
			return output;
		}

		public string DropTableMessage()
		{
			try
			{
				db.DeleteAll<TableMessages>();
				string result = "delete";
				return result;
			}
			catch (SQLiteException ex)
			{
				return "Erreur : " + ex.Message;
			}
		}

		//GET NUMBER LIV RAM ET MSG
		public int SETBadges(string userandsoft)
		{
			var cLIV = db.Table<TablePositions>().Where(v => v.Userandsoft.Equals(userandsoft)).Where(v => v.typeMission.Equals("L")).Where(v => v.typeSegment.Equals("LIV")).Where(v => v.StatutLivraison.Equals("0")).Count();
			var cRam = db.Table<TablePositions>().Where(v => v.Userandsoft.Equals(userandsoft)).Where(v => v.typeMission.Equals("C")).Where(v => v.typeSegment.Equals("RAM")).Where(v => v.StatutLivraison.Equals("0")).Count();
			var cMsg = db.Table<TableMessages>().Where(v => v.statutMessage.Equals(0)).Count();

			var cSUPPLIV = db.Table<TablePositions>().Where(v => v.imgpath.Equals("SUPPLIV")).Count();

			Data.Instance.setLivraisonIndicator(cLIV - cSUPPLIV);
			Data.Instance.setEnlevementIndicator(cRam);
			Data.Instance.setMessageIndicator(cMsg);
			return 0;
		}
	}
}