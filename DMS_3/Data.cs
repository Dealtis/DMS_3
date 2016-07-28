using System;
using System.IO;
using Java.IO;
using System.Net;
using Android.Graphics;
using System.Threading;
using File = System.IO.File;
using Console = System.Console;
using SQLite;
using DMS_3.BDD;
using Xamarin;
using System.Json;
using Android.Media;
using Android.App;

namespace DMS_3
{
	class Data
	{
		//Instance
		private static Data instance;
		//DATA User
		public static string userAndsoft;
		public static string userTransics;

		//Table user
		public static bool tableuserload = false;

		//Log DATA
		public static string log_file;

		//GPS
		public static string GPS;

		//PHOTO
		public static Java.IO.File _file;
		public static Java.IO.File _dir;
		public static Bitmap bitmap;
		String datedujour;

		//BADGES
		private int livraisonIndicator;
		private int enlevementIndicator;
		private int messageIndicator;

		//FONT

		public static Typeface LatoBlack =  Typeface.CreateFromAsset (Application.Context.Assets, "fonts/Lato-Black.ttf");
		public static Typeface LatoBold = Typeface.CreateFromAsset (Application.Context.Assets, "fonts/Lato-Bold.ttf");
		public static Typeface LatoLight = Typeface.CreateFromAsset (Application.Context.Assets, "fonts/Lato-Light.ttf");
		public static Typeface LatoMedium = Typeface.CreateFromAsset (Application.Context.Assets, "fonts/Lato-Medium.ttf");
		public static Typeface LatoRegular = Typeface.CreateFromAsset (Application.Context.Assets, "fonts/Lato-Regular.ttf");


		public static bool Is_Service_Running = false;
		public static Data Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new Data();
				}
				return instance;
			}
		}
		public int getLivraisonIndicator()
		{ return livraisonIndicator; }

		public void setLivraisonIndicator (int _livraisonIndicator)
		{
			livraisonIndicator = _livraisonIndicator;
		}

		public int getEnlevementIndicator()
		{ return enlevementIndicator; }

		public void setEnlevementIndicator (int _enlevementIndicator)
		{
			enlevementIndicator = _enlevementIndicator;
		}

		public int getMessageIndicator()
		{ return messageIndicator; }

		public void setMessageIndicator (int _messageIndicator)
		{
			messageIndicator = _messageIndicator;
		}

		public bool  UploadFile(string FtpUrl, string fileName, string userName, string password,string UploadDirectory)
		{
			try{
				string PureFileName = new FileInfo(fileName).Name;
				String uploadUrl = String.Format("{0}{1}/{2}", FtpUrl,UploadDirectory,PureFileName);
				FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(uploadUrl);
				req.Proxy = null;
				req.Method = WebRequestMethods.Ftp.UploadFile;
				req.Credentials = new NetworkCredential(userName,password);
				req.UseBinary = true;
				req.UsePassive = true;
				byte[] data = System.IO.File.ReadAllBytes(fileName);
				req.ContentLength = data.Length;
				System.IO.Stream stream = req.GetRequestStream();
				stream.Write(data, 0, data.Length);
				stream.Close();
				FtpWebResponse res = (FtpWebResponse)req.GetResponse();
				//File.AppendAllText(Data.log_file,"Upload file"+fileName+" good\n");
				Console.Out.Write("Upload file"+fileName+" : "+res);
				return true;

			} catch (Exception ex) {
				Insights.Report(ex);
				//File.AppendAllText(Data.log_file,"Upload file"+fileName+" error :"+ex+"\n");
				Console.Out.Write("Upload file"+fileName+" error\n");
				return false;
			}
		}

		public void  InsertData ()
		{	
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			DBRepository dbr = new DBRepository ();
			datedujour = DateTime.Now.ToString("yyyyMMdd");

			//récupération de donnée via le webservice
			string content_integdata = String.Empty;
			try {
				string _url = "http://dmsv3.jeantettransport.com/api/commande?codechauffeur=" + userTransics + "&datecommande=" + datedujour + "";
				var webClient = new WebClient ();
				webClient.Headers [HttpRequestHeader.ContentType] = "application/json";
				content_integdata = webClient.DownloadString (_url);
				Console.Out.WriteLine ("\nWebclient integdata Terminé");
				//intégration des données dans la BDD
				JsonArray jsonVal = JsonArray.Parse (content_integdata) as JsonArray;
				var jsonArr = jsonVal;
				if (content_integdata != "[]") {
					foreach (var row in jsonArr) {
						bool checkpos = dbr.pos_AlreadyExist(row["numCommande"],row["groupage"],row["typeMission"],row["typeSegment"]);
						if (!checkpos) {
							var IntegUser = dbr.InsertDataPosition(row["codeLivraison"],row["numCommande"],row["refClient"],row["nomPayeur"],row["nomExpediteur"],row["adresseExpediteur"],row["villeExpediteur"],row["CpExpediteur"],row["dateExpe"],row["nomClient"],row["adresseLivraison"],row["villeLivraison"],row["CpLivraison"],row["dateHeure"],row["poids"],row["nbrPallette"],row["nbrColis"],row["instrucLivraison"],row["typeMission"],row["typeSegment"],row["groupage"],row["ADRCom"],row["ADRGrp"],"0",row["CR"],DateTime.Now.Day,row["Datemission"],row["Ordremission"],row["planDeTransport"],userAndsoft,row["nomClientLivraison"],row["villeClientLivraison"],null);
							Console.WriteLine ("\n"+IntegUser);
						}
					}
				}
							
			} catch (Exception ex) {
				content_integdata = "[]";
				Console.WriteLine ("\n"+ex);
				Insights.Report(ex);
			}

			//verification des groupages et suppression des cloturer

			//select des grp's
			string content_grpcloture = String.Empty;
			var tablegroupage = db.Query<TablePositions> ("SELECT groupage FROM TablePositions group by groupage");
			foreach (var row in tablegroupage)
			{
				string numGroupage = row.groupage;
				try {
					string _urlb = "http://dmsv3.jeantettransport.com/api/groupage?voybdx="+ numGroupage+"";
					var webClient = new WebClient ();
					webClient.Headers [HttpRequestHeader.ContentType] = "application/json";
					content_grpcloture = webClient.DownloadString (_urlb);
					JsonValue jsonVal = JsonObject.Parse(content_grpcloture);
					//JsonArray jsonVal = JsonArray.Parse (content_grpcloture) as JsonArray;
					//var jsonArr = jsonVal;							
					if (jsonVal["etat"].ToString() == "\"CLO\""){
						//suppression du groupage en question si clo
						dbr.supp_grp(numGroupage);
					}					
				}
				catch (Exception ex) {
					content_grpcloture = "[]";
					Console.WriteLine ("\n"+ex);
					Insights.Report(ex);
				}

			}

			Console.WriteLine ("\nTask InsertData done");
			//File.AppendAllText(Data.log_file, "Task InsertData done"+DateTime.Now.ToString("t")+"\n");

		}

	}
}

