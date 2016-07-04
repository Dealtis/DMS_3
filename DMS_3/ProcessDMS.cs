using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.Net;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using DMS_3.BDD;
using Java.Text;
using SQLite;
using Environment = System.Environment;
using Xamarin;
using Android.Media;
using Android.Telephony;

namespace DMS_3
{
	[Service]
	[IntentFilter(new string[]{"com.dealtis.dms_3.ProcessDMS"})]
	public class ProcessDMS : Service, ILocationListener
	{
		ProcessDMSBinder binder;
		string datedujour;
		LocationManager locMgr;

		string userAndsoft;
		string userTransics;
		string GPS;
		string GPSTemp = string.Empty;
		Location previousLocation;
		string _locationProvider;
		string stringValues;
		string stringNotif;

		Task task;
		DBRepository dbr = new DBRepository();


		//string log_file;
		public override StartCommandResult OnStartCommand (Android.Content.Intent intent, StartCommandFlags flags, int startId)
		{			
			var t = DateTime.Now.ToString("dd_MM_yy");
			string dir_log = (Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads)).ToString();
			ISharedPreferences pref = Application.Context.GetSharedPreferences("AppInfo", FileCreationMode.Private);
			string log = pref.GetString("Log", String.Empty);
			//GetTelId
			TelephonyManager tel = (TelephonyManager)this.GetSystemService(Context.TelephonyService);
			var telId = tel.DeviceId;
			//provisoire
			DBRepository dbr = new DBRepository ();
			userAndsoft = dbr.getUserAndsoft ();
			userTransics = dbr.getUserTransics ();

			StartServiceInForeground ();
			Routine ();

			// initialize location manager
			InitializeLocationManager ();

			if (_locationProvider == "") {
				//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"][GPS] Le GPS est désactiver");
			} else {
				locMgr.RequestLocationUpdates (_locationProvider, 0, 0, this);
				Console.Out.Write ("Listening for location updates using " + _locationProvider + ".");
				//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"][GPS] Start");
			}

			return StartCommandResult.Sticky;
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			StopForeground (true);
			StopSelf();
		}

		void InitializeLocationManager()
		{
			locMgr = (LocationManager)GetSystemService(LocationService);

			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Fine
			};
			IList<string> acceptableLocationProviders = locMgr.GetProviders(criteriaForLocationService, true);

			if (acceptableLocationProviders.Any())
			{
				_locationProvider = acceptableLocationProviders.First();
			}
			else
			{
				_locationProvider = String.Empty;
			}
			Console.Out.Write("Using " + _locationProvider + ".");
		}

		void StartServiceInForeground ()
		{
			var ongoing = new Notification (Resource.Drawable.iconapp , "DMS en cours d'exécution");
			var pendingIntent = PendingIntent.GetActivity (this, 0, new Intent (this, typeof(HomeActivity)), 0);
			ongoing.SetLatestEventInfo (this, "DMS", "DMS en cours d'exécution", pendingIntent);
			StartForeground ((int)NotificationFlags.ForegroundService,ongoing);
		}


		void Routine ()
		{			
//			_timer = new System.Threading.Timer ((o) => {
//				
//
//			}, null, 0, 120000);

			task = Task.Factory.StartNew(() =>
				{
					while (true)
					{
						DBRepository dbr = new DBRepository ();
						userAndsoft = dbr.getUserAndsoft ();
						userTransics = dbr.getUserTransics ();
						var connectivityManager = (ConnectivityManager)GetSystemService (ConnectivityService);
						var activeConnection = connectivityManager.ActiveNetworkInfo;
						if (userAndsoft != string.Empty) {
							if ((activeConnection != null) && activeConnection.IsConnected) {			
								Task.Factory.StartNew (
									() => {
										Console.WriteLine ("\nHello from ComPosNotifMsg.");
										//dbr.InsertLogService("",DateTime.Now,"ComPosNotifMsg Start");
										ComPosNotifMsg ();
										Thread.Sleep(500);
									}					
								).ContinueWith (
									t => {
										Console.WriteLine ("\nHello from ComWebService.");
										//dbr.InsertLogService("",DateTime.Now,"ComWebService Start");
										ComWebService ();
										Thread.Sleep(500);
									}						
								);
							}
						}

						string dir_log = (Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads)).ToString();
						ISharedPreferences pref = Application.Context.GetSharedPreferences("AppInfo", FileCreationMode.Private);
						ISharedPreferencesEditor edit = pref.Edit();
						edit.PutLong("Service",DateTime.Now.Ticks);
						edit.Apply();
						Console.Out.WriteLine ("Service timer :"+pref.GetLong("Service", 0));
						Thread.Sleep(120000);
					}
				});
		}

		public override Android.OS.IBinder OnBind (Android.Content.Intent intent)
		{
			binder = new ProcessDMSBinder (this);
			return binder;
		}
		void  InsertData ()
		{	
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			datedujour = DateTime.Now.ToString("yyyyMMdd");

			//récupération de donnée via le webservice
			string content_integdata = String.Empty;
			try {
				string _url = "http://dmsv3.jeantettransport.com/api/commandeWSV3?codechauffeur=" + userTransics + "&datecommande=" + datedujour + "";
				var webClient = new WebClient ();
				webClient.Headers [HttpRequestHeader.ContentType] = "application/json";
				webClient.Encoding = System.Text.Encoding.UTF8;
				content_integdata = webClient.DownloadString (_url);
				//dbr.InsertLogService("",DateTime.Now,"webClient.DownloadString Done");
				//intégration des données dans la BDD
				JsonArray jsonVal = JsonArray.Parse (content_integdata) as JsonArray;
				var jsonArr = jsonVal;
				if (content_integdata != "[]") {
					stringValues = string.Empty;
					stringNotif  =string.Empty;
					foreach (var row in jsonArr) {
						bool checkpos = dbr.pos_AlreadyExist(row["numCommande"],row["groupage"],row["typeMission"],row["typeSegment"]);
						if (!checkpos) {
							stringValues +=" SELECT "+row["codeLivraison"].ToString()+","+row["numCommande"].ToString()+","+row["nomClient"].ToString()+","+row["refClient"].ToString()+","+row["nomPayeur"].ToString()+","+row["adresseLivraison"].ToString()+","+row["CpLivraison"].ToString()+","+row["villeLivraison"].ToString()+","+row["dateExpe"].ToString()+","+row["nbrColis"].ToString()+","+row["nbrPallette"].ToString()+","+row["poids"].ToString()+","+row["adresseExpediteur"].ToString()+","+row["CpExpediteur"].ToString()+","+row["dateExpe"].ToString()+","+row["villeExpediteur"].ToString()+","+row["nomExpediteur"].ToString()+","+row["instrucLivraison"].ToString()+","+row["groupage"].ToString()+","+row["ADRCom"].ToString()+","+row["ADRGrp"].ToString()+","+row["typeMission"].ToString()+","+row["typeSegment"].ToString()+",0,"+row["CR"].ToString()+","+DateTime.Now.Day+","+row["Datemission"].ToString()+","+row["Ordremission"].ToString()+","+row["planDeTransport"].ToString()+",\""+userAndsoft+"\","+row["nomClientLivraison"].ToString()+","+row["villeClientLivraison"].ToString()+",\"null\" UNION ALL";
							//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"][TASK]Intégration d'une position "+row["numCommande"]+" "+row["groupage"]+"\n");
						}
						//NOTIF
						stringNotif += ""+row["numCommande"]+"|";
					}
					if (stringValues != string.Empty) {
						string stringinsertpos="INSERT INTO ";
						stringinsertpos +="TablePositions ( codeLivraison, numCommande, nomClient, refClient, nomPayeur, adresseLivraison, CpLivraison, villeLivraison, dateHeure, nbrColis, nbrPallette, poids, adresseExpediteur, CpExpediteur, dateExpe, villeExpediteur, nomExpediteur, instrucLivraison, GROUPAGE, AdrLiv, AdrGrp, typeMission, typeSegment, statutLivraison, CR, dateBDD, Datemission, Ordremission,planDeTransport, Userandsoft, nomClientLivraison, villeClientLivraison, imgpath )";
						stringinsertpos +=	" " ;
						stringinsertpos +=	stringValues.Remove(stringValues.Length-9);
						var execreq = db.Execute(stringinsertpos);
						Console.Out.WriteLine(execreq);
						//SON
						if (content_integdata == "[]") {
						} else {
							alert ();
						}
					}
					if (stringNotif != string.Empty) {
						string stringinsertnotif="INSERT INTO TableNotifications ( statutNotificationMessage, dateNotificationMessage, numMessage, numCommande ) VALUES ('10','"+DateTime.Now+"','1','"+(stringNotif.Remove(stringNotif.Length-1))+"')";
						var execreqnotif = db.Execute(stringinsertnotif);
						Console.Out.WriteLine("Execnotif"+execreqnotif);
					}	
				}

			} catch (Exception ex) {
				content_integdata = "[]";
				Console.WriteLine ("\n"+ex);
				//dbr.InsertLogService(ex.ToString(),DateTime.Now,"Insert Data Error");
				//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"][ERROR] InserData : "+ex+" à "+DateTime.Now.ToString("t")+"\n");
				Insights.Report(ex);
			}


			//SET des badges
			dbr.SETBadges(Data.userAndsoft);

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
							var suppgrp = dbr.supp_grp(numGroupage);
					}					
				}
				catch (Exception ex) {
					content_grpcloture = "[]";
					Console.WriteLine ("\n"+ex);
					Insights.Report(ex);
					//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"]"+"[ERROR] Cloture : "+ex+" à "+DateTime.Now.ToString("t")+"\n");
				}

			}

			Console.WriteLine ("\nTask InsertData done");
			//File.AppendAllText(Data.log_file, "Task InsertData done"+DateTime.Now.ToString("t")+"\n");
			//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"][TASK] InserData done:\n");
		}

		void  ComPosNotifMsg ()
		{
			//API GPS OK
			string dbPath = System.IO.Path.Combine (System.Environment.GetFolderPath
				(System.Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection (dbPath);
			var webClient = new WebClient ();

			try {

			string content_msg = String.Empty;
				//ROUTINE INTEG MESSAGE
				try {					
					//API LIVRER OK
				string _urlb = "http://dmsv3.jeantettransport.com/api/WSV32?codechauffeur=" + userAndsoft +"";
				var webClientb = new WebClient ();
				webClientb.Headers [HttpRequestHeader.ContentType] = "application/json";
				webClientb.Encoding = System.Text.Encoding.UTF8;

				content_msg = webClientb.DownloadString (_urlb);

				} catch (Exception ex) {
					content_msg = "[]";
					Insights.Report (ex,Xamarin.Insights.Severity.Error);
				}
			if (content_msg != "[]") {
				JsonArray jsonVal = JsonArray.Parse (content_msg) as JsonArray;
				var jsonarr = jsonVal;
				foreach (var item in jsonarr) {
					traitMessages(item["codeChauffeur"],item ["texteMessage"],item ["utilisateurEmetteur"],item["numMessage"]);
				}
			}

			//SET des badges
			dbr.SETBadges(Data.userAndsoft);

			String datajson = string.Empty;
			String datagps=string.Empty;
			String datamsg=string.Empty;
			String datanotif=string.Empty;


			datagps = "{\"posgps\":\"" + GPS + "\",\"userandsoft\":\"" + userAndsoft + "\"}";

			var tablestatutmessage = db.Query<TableNotifications> ("SELECT * FROM TableNotifications");

			//SEND NOTIF
			foreach (var item in tablestatutmessage) {
					datanotif += "{\"statutNotificationMessage\":\"" + item.statutNotificationMessage + "\",\"dateNotificationMessage\":\"" + item.dateNotificationMessage + "\",\"numMessage\":\""+item.numMessage+"\",\"numCommande\":\""+item.numCommande+"\",\"groupage\":\""+item.groupage+"\",\"id\":\""+item.Id+"\"},";
				}

			//SEND MESSAGE
			var tablemessage = db.Query<TableMessages> ("SELECT * FROM TableMessages WHERE statutMessage = 2 or statutMessage = 5");
			foreach (var item in tablemessage) {
				datamsg += "{\"codeChauffeur\":\"" + item.codeChauffeur + "\",\"texteMessage\":\"" + item.texteMessage + "\",\"utilisateurEmetteur\":\""+item.utilisateurEmetteur+"\",\"dateImportMessage\":\""+item.dateImportMessage+"\",\"typeMessage\":\""+item.typeMessage+"\"},";
			}

			if(datanotif == ""){
				datanotif ="{}";
			}else{
				datanotif = datanotif.Remove(datanotif.Length - 1);
			}

			if(datamsg == ""){
				datamsg ="{}";
			}else{
				datamsg = datamsg.Remove(datamsg.Length - 1);
			}
			datajson = "{\"suivgps\":"+datagps+",\"statutmessage\":["+datanotif+"],\"Message\":["+datamsg+"]}";


			//API MSG/NOTIF/GPS
			try{
					webClient.Headers [HttpRequestHeader.ContentType] = "application/json";
					webClient.Encoding = System.Text.Encoding.UTF8;
					System.Uri uri = new System.Uri("http://dmsv3.jeantettransport.com/api/WSV32?codechauffeur=" + userAndsoft +"");
					webClient.UploadStringCompleted += WebClient_UploadStringStatutCompleted;
					webClient.UploadStringAsync (uri, datajson);
					//GPSTemp = string.Empty;
				}
			catch (Exception e)
			{
				Insights.Report (e,Xamarin.Insights.Severity.Error);
					//dbr.InsertLogService(e.ToString(),DateTime.Now,"ComPosNotifMsg UploadStringAsync Error");
			}
			} catch (Exception ex) {
				Insights.Report (ex,Xamarin.Insights.Severity.Error);
				Console.Out.Write(ex);
				//dbr.InsertLogService(ex.ToString(),DateTime.Now,"ComPosNotifMsg Error");
				//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"]"+"[ERROR] ComPosNotifMsg : "+ex+" à "+DateTime.Now.ToString("t")+"\n");
			}
		Console.WriteLine ("\nTask ComPosGps done");
			//dbr.InsertLogService("",DateTime.Now,"Task ComPosGps done");
		}
		

		void  ComWebService ()
		{
			//récupération des données dans la BDD
			string dbPath = System.IO.Path.Combine (Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection (dbPath);
			var table = db.Query<TableStatutPositions> ("Select * FROM TableStatutPositions");
			string datajsonArray = string.Empty;
			datajsonArray += "[";
			foreach (var item in table) {
				datajsonArray += item.datajson+",";
			}
			datajsonArray += "]";

			if (datajsonArray == "[]") {
				
			} else {
				var webClient = new WebClient ();
				webClient.Headers [HttpRequestHeader.ContentType] = "application/json";
				webClient.Encoding = System.Text.Encoding.UTF8;
				System.Uri uri = new System.Uri("http://dmsv3.jeantettransport.com/api/livraisongroupagev3");
				try {
					webClient.UploadStringCompleted += WebClient_UploadStringCompleted;
					webClient.UploadStringAsync (uri, datajsonArray);
					//dbr.InsertLogService("",DateTime.Now,"ComWebService UploadStringAsync Done");
				} catch (Exception e) {
					Console.WriteLine (e);
					Insights.Report(e);
					//dbr.InsertLogService(e.ToString(),DateTime.Now,"ComWebService Error");
					//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"]"+"[ERROR] ComWebService : "+e+" à "+DateTime.Now.ToString("t")+"\n");
				}
			}
			Console.WriteLine ("\nTask ComWebService done");
			//dbr.InsertLogService("",DateTime.Now,"Task ComWebService done");
			//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"]"+"[TASK] ComWebService Done \n");
		}

		void WebClient_UploadStringCompleted (object sender, UploadStringCompletedEventArgs e)
		{
			try {
				string resultjson = "[" + e.Result + "]";
				//dbr.InsertLogService(e.Result,DateTime.Now,"WebClient_UploadStringCompleted Response");
				if (e.Result == "\"YOLO\"") {

				} else {
					JsonArray jsonVal = JsonArray.Parse (resultjson) as JsonArray;
					var jsonarr = jsonVal;
					foreach (var item in jsonarr) {
						traitMessages (item ["codeChauffeur"], item ["texteMessage"], item ["utilisateurEmetteur"], item ["numMessage"]);
					}
				}
			} catch (Exception ex) {
				Console.WriteLine (ex);
				Insights.Report(ex);
				//dbr.InsertLogService(e.Result,DateTime.Now,"WebClient_UploadStringCompleted Response");
				//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"]"+"[ERROR] WebClient_UploadStringCompleted : "+ex+" à "+DateTime.Now.ToString("t")+"\n");
			}
		}

		void WebClient_UploadStringStatutCompleted (object sender, UploadStringCompletedEventArgs e)
		{
			try {
				string dbPath = System.IO.Path.Combine (Environment.GetFolderPath
					(Environment.SpecialFolder.Personal), "ormDMS.db3");
				var db = new SQLiteConnection (dbPath);
				string resultjson = "[" + e.Result + "]";
				if (e.Result == "{\"Id\":0,\"codeChauffeur\":null,\"texteMessage\":null,\"utilisateurEmetteur\":null,\"statutMessage\":0,\"dateImportMessage\":\"0001-01-01T00:00:00\",\"typeMessage\":0,\"numMessage\":null}") {
				} else {
					JsonArray jsonVal = JsonArray.Parse (resultjson) as JsonArray;
					var jsonarr = jsonVal;
					foreach (var item in jsonarr) {
						traitMessages (item ["codeChauffeur"], item ["texteMessage"], item ["utilisateurEmetteur"], item ["numMessage"]);
					}
				}

				var tablemessage = db.Query<TableMessages> ("SELECT * FROM TableMessages WHERE statutMessage = 2 or statutMessage = 5");
				foreach (var item in tablemessage) {
					var updatestatutmessage = db.Query<TableMessages> ("UPDATE TableMessages SET statutMessage = 3 WHERE _Id = ?",item.Id);
				}
				//dbr.InsertLogService("",DateTime.Now,"WebClient_UploadStringStatutCompleted Done");
			} catch (Exception ex) {
				Console.WriteLine (ex);
				Insights.Report(ex);
				//dbr.InsertLogService(ex.ToString(),DateTime.Now,"WebClient_UploadStringStatutCompleted Error");
				//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"]"+"[ERROR] WebClient_UploadStringStatutCompleted : "+ex+" à "+DateTime.Now.ToString("t")+"\n");
			}

		}

		public void OnLocationChanged (Android.Locations.Location location)
		{
			if (previousLocation == null) {
				GPS = location.Latitude + ";" + location.Longitude;
				Data.GPS = location.Latitude+ ";" + location.Longitude;
				previousLocation = location;
			} else {
				//distance (location.Latitude, location.Longitude, previousLocation.Latitude, previousLocation.Longitude < 150
				if (true){
					//GPS = location.Latitude.ToString() +";"+ location.Longitude.ToString();
					GPS = location.Latitude+ ";" + location.Longitude;
					Data.GPS = location.Latitude+ ";" + location.Longitude;
					previousLocation = location;
				}
			}
		}
		public void OnProviderDisabled (string provider)
		{

		}
		public void OnProviderEnabled (string provider)
		{

		}
		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{

		}

		public double distance(double lat1, double lng1, double lat2, double lng2)
		{
			double earthRadius = 6371 * 1000;
			double dLat = toRadians(lat2 - lat1);
			double dLng = toRadians(lng2 - lng1);
			double sindLat = Math.Sin(dLat / 2);
			double sindLng = Math.Sin(dLng / 2);
			double a = Math.Pow(sindLat, 2) + Math.Pow(sindLng, 2) * Math.Cos(toRadians(lat1)) * Math.Cos(toRadians(lat2));
			double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			double dist = earthRadius * c;
			// Return the computed distance
			return dist;
		}

		private double toRadians(double ang)
		{
			return ang * Math.PI / 180;
		}


		void traitMessages (string codeChauffeur, string texteMessage, string utilisateurEmetteur, int numMessage){
			string dbPath = System.IO.Path.Combine (System.Environment.GetFolderPath
				(System.Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection (dbPath);

			DBRepository dbr = new DBRepository ();
			try {
				if (texteMessage.ToString().Length < 9) {
					var resinteg = dbr.InsertDataMessage (codeChauffeur, utilisateurEmetteur, texteMessage,0,DateTime.Now,1,numMessage);
					//TODO
					//var resintegstatut = dbr.InsertDataStatutMessage(0,DateTime.Now,numMessage,"","");
					alertsms ();	
				}else{
					switch(texteMessage.ToString().Substring(0,9))
					{
					case "%%SUPPLIV":
						var updatestat = dbr.updatePositionSuppliv((texteMessage.ToString()).Remove((texteMessage.ToString()).Length - 2).Substring(10));
						dbr.InsertDataStatutMessage (1,DateTime.Now,numMessage,"","");
						dbr.InsertDataMessage (codeChauffeur, utilisateurEmetteur,"La position "+(texteMessage.ToString()).Remove((texteMessage.ToString()).Length - 2).Substring(10)+" a été supprimée de votre tournée",0,DateTime.Now,1, numMessage);
						//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"]"+"[SYSTEM]Réception d'un SUPPLIV à "+DateTime.Now.ToString("t")+"\n");
						break;
					case "%%RETOLIV":
						var updatestattretour = db.Query<TablePositions>("UPDATE TablePositions SET imgpath = null WHERE numCommande = ?",(texteMessage.ToString()).Remove((texteMessage.ToString()).Length - 2).Substring(10));
						var resstatutbis = dbr.InsertDataStatutMessage (1,DateTime.Now,numMessage,"","");
						break;
					case "%%SUPPGRP":
						var supgrp = db.Query<TablePositions>("DELETE from TablePositions where groupage = ?",(texteMessage.ToString()).Remove((texteMessage.ToString()).Length - 2).Substring(10));
						var ressupgrp = dbr.InsertDataStatutMessage (1,DateTime.Now,numMessage,"","");
						break;
					case "%%GETFLOG":
						//ftp://77.158.93.75 or ftp://10.1.2.75
						//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"]"+"[SYSTEM]Réception d'un GETFLOG à "+DateTime.Now.ToString("t")+"\n");
						Thread thread = new Thread(() => UploadFile("ftp://77.158.93.75",Data.log_file,"DMS","Linuxr00tn",""));
						thread.Start ();
						dbr.InsertDataStatutMessage(0,DateTime.Now,numMessage,"","");
						dbr.InsertDataMessage (Data.userAndsoft, "", "%%GETFLOG Done", 5, DateTime.Now, 5, 0);
						break;
					case "%%COMMAND":
						//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"]"+"[SYSTEM]Réception d'un COMMAND à "+DateTime.Now.ToString("t")+"\n");
						InsertData ();									
						break;
					case "%%GETAIMG":
						try {
							string compImg;
							string imgpath = dbr.getAnomalieImgPath((texteMessage.ToString()).Remove((texteMessage.ToString()).Length - 2).Substring(10));
							if (imgpath!=string.Empty) {
								Android.Graphics.Bitmap bmp = Android.Graphics.BitmapFactory.DecodeFile (imgpath);
								Android.Graphics.Bitmap rbmp = Android.Graphics.Bitmap.CreateScaledBitmap (bmp, bmp.Width / 5, bmp.Height / 5, true);
								compImg = imgpath.Replace (".jpg", "-1_1.jpg");
								using (var fs = new FileStream (compImg, FileMode.OpenOrCreate)) {
									rbmp.Compress (Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, fs);
								}
								Thread threadimgpath = new Thread(() => UploadFile("ftp://77.158.93.75",compImg,"DMS","Linuxr00tn",""));
								threadimgpath.Start ();	
							}
							dbr.InsertDataMessage (Data.userAndsoft, "", "%%GETAIMG Done", 5, DateTime.Now, 5, 0);
						} catch (Exception ex) {
							Insights.Report(ex);
							//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"]"+"%%GETAIMG Upload file error :"+ex+"\n");
							Console.Out.Write("%%GETAIMG Upload file error\n");
						}
						break;
					case "%%STOPSER":

						StopForeground (true);
						StopSelf();
						break;
					case "%%REQUETE":
						string[] texteMessageInputSplit = Android.Text.TextUtils.Split (texteMessage,"%%");
						switch (texteMessageInputSplit [2]) {
						case"TableMessages":
							//File.AppendAllText (log_file, "[" + DateTime.Now.ToString ("G") + "]" + "[SYSTEM]Réception d'un REQUETE sur TableMessages\n");
							var selMesg = db.Query<TableMessages> (texteMessageInputSplit [3]);
							string rowMsg = "";
							rowMsg += "[";
							foreach (var item in selMesg) {
								rowMsg += "{"+item.codeChauffeur+","+item.numMessage+","+item.texteMessage+","+item.utilisateurEmetteur+"},";
							}
							rowMsg.Remove(rowMsg.Length -1);
							rowMsg += "]";
							var rMsg = dbr.InsertDataMessage (Data.userAndsoft, "", rowMsg, 5, DateTime.Now, 5, 0);
							//File.AppendAllText (log_file, "[" + DateTime.Now.ToString ("G") + "]" + "[SYSTEM]REQUETE Execute " + rowMsg + "\n");
							break;
						case"TableNotifications":
							//File.AppendAllText (log_file, "[" + DateTime.Now.ToString ("G") + "]" + "[SYSTEM]Réception d'un REQUETE sur TableNotifications\n");
							var selNotif = db.Query<TablePositions> (texteMessageInputSplit [3]);
							string rowNotif = "";
							rowNotif += "[";
							foreach (var item in selNotif) {
								rowNotif += "{"+item.numCommande+","+item.groupage+","+item.typeMission+","+item.typeSegment+","+item.refClient+"},";
							}
							rowNotif.Remove(rowNotif.Length -1);
							rowNotif += "]";
							var rNotif = dbr.InsertDataMessage (Data.userAndsoft, "", rowNotif, 5, DateTime.Now, 5, 0);
							//File.AppendAllText (log_file, "[" + DateTime.Now.ToString ("G") + "]" + "[SYSTEM]REQUETE Execute " + rowNotif + "\n");
							break;
						case"TablePositions":
							//File.AppendAllText (log_file, "[" + DateTime.Now.ToString ("G") + "]" + "[SYSTEM]Réception d'un REQUETE sur TablePositions\n");
							var selPos = db.Query<TablePositions> (texteMessageInputSplit [3]);
							string rowPos = "";
							rowPos += "[";
							foreach (var item in selPos) {
								rowPos += "{"+item.numCommande+","+item.groupage+","+item.typeMission+","+item.typeSegment+","+item.refClient+"},";
							}
							rowPos.Remove(rowPos.Length -1);
							rowPos += "]";
							var rPOS = dbr.InsertDataMessage (Data.userAndsoft, "", rowPos, 5, DateTime.Now, 5, 0);
							//File.AppendAllText (log_file, "[" + DateTime.Now.ToString ("G") + "]" + "[SYSTEM]REQUETE Execute " + rowPos + "\n");
							break;
						case"TableStatutPositions":
							//File.AppendAllText (log_file, "[" + DateTime.Now.ToString ("G") + "]" + "[SYSTEM]Réception d'un REQUETE sur TableUser\n");
							var selStat = db.Query<TableStatutPositions>(texteMessageInputSplit [3]);
							string rowStatut = "";
							rowStatut += "[";
							foreach (var item in selStat) {
								rowStatut += "{"+item.codesuiviliv+","+item.commandesuiviliv+","+item.datajson+","+item.datesuiviliv+","+item.libellesuiviliv+","+item.memosuiviliv+","+item.statut+"},";
							}
							rowStatut.Remove(rowStatut.Length -1);
							rowStatut += "]";
							var rSTATLIV = dbr.InsertDataMessage (Data.userAndsoft, "", rowStatut, 5, DateTime.Now, 5, 0);
							//File.AppendAllText (log_file, "[" + DateTime.Now.ToString ("G") + "]" + "[SYSTEM]REQUETE Execute " + rowStatut + "\n");
							break;
						case"TableUser":
							//File.AppendAllText (log_file, "[" + DateTime.Now.ToString ("G") + "]" + "[SYSTEM]Réception d'un REQUETE sur TableUser\n");
							var selUser = db.Query<TableUser> (texteMessageInputSplit [3]);
							string rowUser = "";
							rowUser += "[";
							foreach (var item in selUser) {
								rowUser += "{"+item.user_AndsoftUser+","+item.user_TransicsUser+","+item.user_Password+","+item.user_UsePartic+"},";
							}
							rowUser.Remove(rowUser.Length -1);
							rowUser += "]";
							var rMUSER = dbr.InsertDataMessage (Data.userAndsoft, "", rowUser, 5, DateTime.Now, 5, 0);
							break;
						case"TableLogService":
							var selLog = db.Query<TableLogService> (texteMessageInputSplit [3]);
							string rowLog = "";
							rowLog += "[";
							foreach (var item in selLog) {
								rowLog += "{"+item.exeption+","+item.date.ToString("g")+","+item.description+"},";
							}
							rowLog.Remove(rowLog.Length -1);
							rowLog += "]";
							dbr.InsertDataMessage (Data.userAndsoft, "", rowLog, 5, DateTime.Now, 5, 0);
							break;
						case"TableLogApp":
							var appLog = db.Query<TableLogApp> (texteMessageInputSplit [3]);
							string rowapLog = "";
							rowapLog += "[";
							foreach (var item in appLog) {
								rowapLog += "{"+item.exeption+","+item.date.ToString("g")+","+item.description+"},";
							}
							rowapLog.Remove(rowapLog.Length -1);
							rowapLog += "]";
							dbr.InsertDataMessage (Data.userAndsoft, "", rowapLog, 5, DateTime.Now, 5, 0);
							break;
						case"NOTHING":
							break;
						default:
							//File.AppendAllText (log_file, "[" + DateTime.Now.ToString ("G") + "]" + "[SYSTEM]Réception d'un REQUETE\n");
							var execreq = db.Execute (texteMessageInputSplit [3]);
							var rEXEC = dbr.InsertDataMessage (Data.userAndsoft, "", execreq+" lignes traitées : "+texteMessageInputSplit [3], 5, DateTime.Now, 5, 0);
							//File.AppendAllText (log_file, "[" + DateTime.Now.ToString ("G") + "]" + "[SYSTEM]REQUETE Execute " + execreq +" pour "+texteMessageInputSplit [3]+"\n");
							break;
						}
						break;
					default:
						var resinteg = dbr.InsertDataMessage (codeChauffeur, utilisateurEmetteur, texteMessage,0,DateTime.Now,1,numMessage);
						//TODO
						//dbr.InsertDataStatutMessage(0,DateTime.Now,numMessage,"","");
						alertsms ();
						Console.WriteLine (numMessage.ToString());
						Console.WriteLine (resinteg);
						break;
					}
				}
				
			} catch (Exception ex) {
				Console.WriteLine ("\n"+ex);
				//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"]"+"[ERROR METHOD MESSAGE]"+ex+"\n");
			}
						
		}

		public void alert()
		{

			MediaPlayer _player;
			_player = MediaPlayer.Create(this,Resource.Raw.beep4);
			_player.Start();
		}

		public void alertsms()
		{

			MediaPlayer _player;
			_player = MediaPlayer.Create(this,Resource.Raw.msg3);
			_player.Start();
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
				//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"]"+"Upload file"+fileName+" good\n");
				Console.Out.Write("Upload file"+fileName+" good\n");
				return true;
			} catch (Exception ex) {
				Insights.Report(ex);
				//File.AppendAllText(log_file,"["+DateTime.Now.ToString("t")+"]"+"Upload file"+fileName+" error :"+ex+"\n");
				Console.Out.Write("Upload file"+fileName+" error\n");
				Thread.Sleep(TimeSpan.FromMinutes(2));
				UploadFile (FtpUrl, fileName, userName, password, UploadDirectory);
				return false;
			}
		}
	}

	public class ProcessDMSBinder : Binder
	{
		ProcessDMS service;

		public ProcessDMSBinder (ProcessDMS service)
		{
			this.service = service;
		}

		public ProcessDMS GetDemoService ()
		{
			return service;
		}

		public ProcessDMS StopService ()
		{
			return service;
		}
	}
}