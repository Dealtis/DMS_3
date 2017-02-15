using System;
using System.IO;
using System.Json;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Net;
using Android.OS;
using Android.Preferences;
using DMS_3.BDD;
using RaygunClient = Mindscape.Raygun4Net.RaygunClient;
using String = System.String;
using Exception = System.Exception;
using Thread = System.Threading.Thread;
using Math = System.Math;
using Android.Util;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.Gms.Location;

namespace DMS_3
{
	[Service(Exported = true, Name = "com.dealtis.dms.ProcessDMS")]
	public class ProcessDMS : Service, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener
	{
		#region Variables
		string datedujour;
		string _locationProvider;

		string userAndsoft;
		string userTransics;
		string gPS;


		bool isStarted = false;
		private Handler handler = new Handler();
		static readonly string TAG = "X:" + typeof(ProcessDMS).Name;

		GoogleApiClient googleApiClient;

		#endregion

		//string log_file;
		public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
		{
			if (isStarted)
			{
				Log.Debug(TAG, $"This service was already started");
			}
			else
			{
				DBRepository dbr = new DBRepository();
				userAndsoft = dbr.getUserAndsoft();
				userTransics = dbr.getUserTransics();

				StartServiceInForeground();

				handler.PostDelayed(Routine, 2000);

				//initialize location manager
				//InitializeLocationManager();

				//Init Google location manager
				googleApiClient = new GoogleApiClient.Builder(this)
				.AddApi(Android.Gms.Location.LocationServices.API)
				.AddConnectionCallbacks(this)
				.AddOnConnectionFailedListener(this)
				.Build();
				googleApiClient.Connect();
				isStarted = true;
			}

			return StartCommandResult.Sticky;
		}

		public override void OnDestroy()
		{
			isStarted = false;
			StopForeground(true);
			StopSelf();
			base.OnDestroy();
		}

		void InitializeLocationManager()
		{
			//Criteria locationCriteria = new Criteria();

			//locationCriteria.Accuracy = Accuracy.Coarse;
			//locationCriteria.PowerRequirement = Power.Medium;

			//_locationProvider = locMgr.GetBestProvider(locationCriteria, true);

			//locMgr = (LocationManager)GetSystemService(LocationService);

			//if (_locationProvider != null)
			//{
			//	locMgr.RequestLocationUpdates(_locationProvider, 2000, 1, this);
			//}
			//else
			//{
			//	Log.Info(TAG, "No location providers available");
			//}
			//Console.Out.Write("Using " + _locationProvider + ".");
		}

		void StartServiceInForeground()
		{
			// Instantiate the builder and set notification elements:
			Notification.Builder builder = new Notification.Builder(this)
				.SetContentTitle("DMS")
				.SetContentText("DMS en cours d'exécution")
				.SetOngoing(true)
				.SetSmallIcon(Resource.Drawable.iconapp);

			// Build the notification:
			Notification notification = builder.Build();

			// Get the notification manager:
			NotificationManager notificationManager =
				GetSystemService(Context.NotificationService) as NotificationManager;

			// Publish the notification:
			const int notificationId = 0;
			notificationManager.Notify(notificationId, notification);
		}


		void Routine()
		{
			DBRepository dbr = new DBRepository();
			userAndsoft = dbr.getUserAndsoft();
			userTransics = dbr.getUserTransics();
			var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
			var activeConnection = connectivityManager.ActiveNetworkInfo;
			if (userAndsoft != string.Empty)
			{
				if ((activeConnection != null) && activeConnection.IsConnected)
				{
					Log.Debug(TAG, $"Hello from ComPosNotifMsg.");
					ComPosNotifMsg();

					Log.Debug(TAG, $"Hello from ComWebService.");
					ComWebService();
				}
			}

			ISharedPreferences pref = Application.Context.GetSharedPreferences("AppInfo", FileCreationMode.Private);
			ISharedPreferencesEditor edit = pref.Edit();
			edit.PutLong("Service", DateTime.Now.Ticks);
			edit.Apply();
			Console.Out.WriteLine("Service timer :" + pref.GetLong("Service", 0));
			handler.PostDelayed(Routine, 120000);
		}

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}

		#region Webservice
		void InsertData()
		{
			datedujour = DateTime.Now.ToString("yyyyMMdd");

			DBRepository dbr = new DBRepository();
			//récupération de donnée via le webservice
			string content_integdata = String.Empty;
			try
			{
				var webClient = new WebClient();
				webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
				webClient.Encoding = System.Text.Encoding.UTF8;

				webClient.QueryString.Add("codechauffeur", userTransics);
				webClient.QueryString.Add("datecommande", datedujour);

				ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
				string _url = prefs.GetString("API_DOMAIN", String.Empty) + "/api/commandeWsv4";
				content_integdata = webClient.DownloadString(_url);

				//intégration des données dans la BDD
				JsonArray jsonVal = JsonValue.Parse(content_integdata) as JsonArray;
				var jsonArr = jsonVal;
				if (content_integdata != "[]")
				{
					foreach (var row in jsonArr)
					{
						bool checkpos = dbr.pos_AlreadyExist(row["numCommande"], row["groupage"], row["typeMission"], row["typeSegment"]);
						if (!checkpos)
						{
							try
							{
								dbr.insertDataPosition(row["idSegment"], row["codeLivraison"], row["numCommande"], row["nomClient"], row["refClient"], row["nomPayeur"], row["adresseLivraison"], row["CpLivraison"], row["villeLivraison"], row["dateExpe"], row["nbrColis"], row["nbrPallette"], row["poids"], row["adresseExpediteur"], row["CpExpediteur"], row["dateExpe"], row["villeExpediteur"], row["nomExpediteur"], row["instrucLivraison"], row["groupage"], row["PoidsADR"], row["PoidsQL"], row["typeMission"], row["typeSegment"], "0", row["CR"], row["ASSIGNE"], DateTime.Now.Day.ToString(), row["Datemission"], row["Ordremission"], row["planDeTransport"], userAndsoft, row["nomClientLivraison"], row["villeClientLivraison"], row["PositionPole"], "null");
								foreach (JsonValue item in row["detailColis"])
								{
									dbr.InsertDataColis(item["NumColis"], row["numCommande"]);
								}
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex);
								RaygunClient.Current.SendInBackground(ex);
							}
						}
						//NOTIF
						dbr.InsertDataStatutMessage(10, DateTime.Now, 1, row["numCommande"], row["groupage"]);
					}

					//SON
					if (content_integdata != "[]")
					{
						alert();
					}
				}

				//SUPP DES GRP CLO
				string content_grpcloture = String.Empty;
				var tablegroupage = dbr.QueryPositions("SELECT groupage FROM TablePositions group by groupage");
				foreach (var row in tablegroupage)
				{
					string numGroupage = row.groupage;
					Console.WriteLine(numGroupage);
					try
					{
						var webClientGrp = new WebClient();
						webClientGrp.Headers[HttpRequestHeader.ContentType] = "application/json";

						webClientGrp.QueryString.Add("voybdx", numGroupage);
						string _urlb = prefs.GetString("API_DOMAIN", String.Empty) + "/api/groupage";
						content_grpcloture = webClientGrp.DownloadString(_urlb);
						Console.WriteLine(content_grpcloture);
						if (content_grpcloture == "{\"etat\":\"CLO\"}")
						{
							//suppression du groupage en question si clo
							dbr.QueryPositions("DELETE from TablePositions where groupage = '" + numGroupage + "'");
						}
					}
					catch (Exception ex)
					{
						content_grpcloture = "[]";
						Console.WriteLine("\n" + ex);
						RaygunClient.Current.SendInBackground(ex);
					}
				}
			}
			catch (Exception ex)
			{
				content_integdata = "[]";
				Console.WriteLine("\n" + ex);
				RaygunClient.Current.SendInBackground(ex);
			}

			//SET des badges
			dbr.SETBadges(Data.userAndsoft);
			Console.WriteLine("\nTask InsertData done");
		}


		void ComPosNotifMsg()
		{
			DBRepository dbr = new DBRepository();
			//API GPS OK
			var webClient = new WebClient();
			try
			{
				string content_msg = String.Empty;
				//ROUTINE INTEG MESSAGE
				try
				{
					//API LIVRER OK

					var webClientb = new WebClient();
					webClientb.Headers[HttpRequestHeader.ContentType] = "application/json";
					webClientb.Encoding = System.Text.Encoding.UTF8;

					//webClient.QueryString.Add("codechauffeur", userAndsoft);
					ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
					string _urlb = prefs.GetString("API_DOMAIN", String.Empty) + "/api/WSV32?codechauffeur=" + userAndsoft + "";

					content_msg = webClientb.DownloadString(_urlb);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					RaygunClient.Current.SendInBackground(ex);
					content_msg = "[]";
				}
				if (content_msg != "[]")
				{
					JsonArray jsonVal = JsonValue.Parse(content_msg) as JsonArray;
					var jsonarr = jsonVal;
					foreach (var item in jsonarr)
					{
						traitMessages(item["codeChauffeur"], item["texteMessage"], item["utilisateurEmetteur"], item["numMessage"]);
					}
				}

				//SET des badges
				dbr.SETBadges(Data.userAndsoft);

				String datajson = string.Empty;
				String datagps = string.Empty;
				String datamsg = string.Empty;
				String datanotif = string.Empty;

				if (gPS == string.Empty)
				{
					//InitializeLocationManager();
				}

				datagps = "{\"posgps\":\"" + gPS + "\",\"userandsoft\":\"" + userAndsoft + "\"}";

				var tableNotif = dbr.QueryNotif("SELECT * FROM TableNotifications");

				//SEND NOTIF
				foreach (var item in tableNotif)
				{
					datanotif += "{\"statutNotificationMessage\":\"" + item.statutNotificationMessage + "\",\"dateNotificationMessage\":\"" + item.dateNotificationMessage + "\",\"numMessage\":\"" + item.numMessage + "\",\"numCommande\":\"" + item.numCommande + "\",\"groupage\":\"" + item.groupage + "\",\"id\":\"" + item.Id + "\"},";
				}

				//SEND MESSAGE
				var tablemessage = dbr.QueryMessage("SELECT * FROM TableMessages WHERE statutMessage = 2 or statutMessage = 5");
				foreach (var item in tablemessage)
				{
					datamsg += "{\"codeChauffeur\":\"" + item.codeChauffeur + "\",\"texteMessage\":\"" + item.texteMessage + "\",\"utilisateurEmetteur\":\"" + item.utilisateurEmetteur + "\",\"dateImportMessage\":\"" + item.dateImportMessage + "\",\"typeMessage\":\"" + item.typeMessage + "\"},";
				}

				if (datanotif == "")
				{
					datanotif = "{}";
				}
				else {
					datanotif = datanotif.Remove(datanotif.Length - 1);
				}

				if (datamsg == "")
				{
					datamsg = "{}";
				}
				else {
					datamsg = datamsg.Remove(datamsg.Length - 1);
				}
				datajson = "{\"suivgps\":" + datagps + ",\"statutmessage\":[" + datanotif + "],\"Message\":[" + datamsg + "]}";

				//API MSG/NOTIF/GPS
				try
				{
					webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
					webClient.Encoding = System.Text.Encoding.UTF8;

					ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
					System.Uri _url = new System.Uri(prefs.GetString("API_DOMAIN", String.Empty) + "/api/WSV32");
					Console.WriteLine(_url);
					webClient.QueryString.Add("codechauffeur", userAndsoft);
					webClient.UploadStringCompleted += WebClient_UploadStringStatutCompleted;
					webClient.UploadStringAsync(_url, datajson);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					RaygunClient.Current.SendInBackground(e);
				}
			}
			catch (Exception ex)
			{
				Console.Out.Write(ex);
				RaygunClient.Current.SendInBackground(ex);
			}
			Console.WriteLine("\nTask ComPosGps done");
		}

		void ComWebService()
		{

			DBRepository dbr = new DBRepository();
			//récupération des données dans la BDD
			var table = dbr.QueryStatuPos("Select * FROM TableStatutPositions");
			string datajsonArray = string.Empty;
			datajsonArray += "[";
			foreach (var item in table)
			{
				datajsonArray += item.datajson + ",";
			}
			datajsonArray += "]";

			if (datajsonArray != "[]")
			{
				var webClient = new WebClient();
				webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
				webClient.Encoding = System.Text.Encoding.UTF8;
				ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
				System.Uri uri = new System.Uri(prefs.GetString("API_DOMAIN", String.Empty) + "/api/livraisongroupagev3");
				try
				{
					webClient.UploadStringCompleted += WebClient_UploadStringCompleted;
					webClient.UploadStringAsync(uri, datajsonArray);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					RaygunClient.Current.SendInBackground(e);
				}
			}
			Console.WriteLine("\nTask ComWebService done");
		}

		void WebClient_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
		{
			try
			{
				string resultjson = "[" + e.Result + "]";
				//dbr.InsertLogService(e.Result,DateTime.Now,"WebClient_UploadStringCompleted Response");
				if (e.Result != "\"YOLO\"")
				{
					JsonArray jsonVal = JsonArray.Parse(resultjson) as JsonArray;
					var jsonarr = jsonVal;
					foreach (var item in jsonarr)
					{
						traitMessages(item["codeChauffeur"], item["texteMessage"], item["utilisateurEmetteur"], item["numMessage"]);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				RaygunClient.Current.SendInBackground(ex);
			}
		}

		void WebClient_UploadStringStatutCompleted(object sender, UploadStringCompletedEventArgs e)
		{
			try
			{
				DBRepository dbr = new DBRepository();
				string resultjson = "[" + e.Result + "]";
				if (e.Result != "{\"Id\":0,\"codeChauffeur\":null,\"texteMessage\":null,\"utilisateurEmetteur\":null,\"statutMessage\":0,\"dateImportMessage\":\"0001-01-01T00:00:00\",\"typeMessage\":0,\"numMessage\":null}")
				{
					JsonArray jsonVal = JsonArray.Parse(resultjson) as JsonArray;
					var jsonarr = jsonVal;
					foreach (var item in jsonarr)
					{
						traitMessages(item["codeChauffeur"], item["texteMessage"], item["utilisateurEmetteur"], item["numMessage"]);
					}
				}

				var tablemessage = dbr.QueryMessage("SELECT * FROM TableMessages WHERE statutMessage = 2 or statutMessage = 5");
				foreach (var item in tablemessage)
				{
					dbr.QueryMessage("UPDATE TableMessages SET statutMessage = 3 WHERE _Id = '" + item.Id + "'");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				RaygunClient.Current.SendInBackground(ex);
			}
		}

		void traitMessages(string codeChauffeur, string texteMessage, string utilisateurEmetteur, int numMessage)
		{
			try
			{
				DBRepository dbr = new DBRepository();
				if (texteMessage.ToString().Length < 9)
				{
					dbr.insertDataMessage(codeChauffeur, utilisateurEmetteur, texteMessage, 0, DateTime.Now, 1, numMessage);
					dbr.InsertDataStatutMessage(0, DateTime.Now, numMessage, "", "");
					alertsms();
				}
				else {
					switch (texteMessage.ToString().Substring(0, 9))
					{
						case "%%SUPPLIV":
							dbr.updatePositionSuppliv(texteMessage.Remove(texteMessage.Length - 2).Substring(10));
							dbr.InsertDataStatutMessage(1, DateTime.Now, numMessage, "", "");
							TablePositions posMsg = dbr.GetPositionsData(dbr.GetidPosition(texteMessage.Remove(texteMessage.Length - 2).Substring(10)));
							dbr.insertDataMessage(codeChauffeur, utilisateurEmetteur, "La position " + texteMessage.Remove(texteMessage.Length - 2).Substring(10) + "de " + posMsg.typeSegment + " a été supprimée de votre tournée", 0, DateTime.Now, 1, numMessage);
							dbr.SETBadges(Data.userAndsoft);
							break;
						case "%%RETOLIV":
							dbr.QueryPositions("UPDATE TablePositions SET imgpath = null WHERE numCommande = '" + texteMessage.Remove(texteMessage.Length - 2).Substring(10) + "'");
							dbr.InsertDataStatutMessage(1, DateTime.Now, numMessage, "", "");
							break;
						case "%%SUPPGRP":
							dbr.QueryPositions("DELETE from TablePositions where groupage = '" + texteMessage.Remove(texteMessage.Length - 2).Substring(10) + "'");
							dbr.InsertDataStatutMessage(1, DateTime.Now, numMessage, "", "");
							break;
						case "%%RUNTGPS":
							//if (_locationProvider != "")
							//{
							//	locMgr.RequestLocationUpdates(_locationProvider, 0, 0, this);
							//	Console.Out.Write("Listening for location updates using " + _locationProvider + ".");
							//	dbr.insertDataMessage(Data.userAndsoft, "", "Listening for location updates using " + _locationProvider + ".", 5, DateTime.Now, 5, 0);
							//}
							//else
							//{
							//	dbr.insertDataMessage(Data.userAndsoft, "", "Location provider null", 5, DateTime.Now, 5, 0);
							//}
							break;
						case "%%COMMAND":
							InsertData();
							break;
						case "%%GETAIMG":
							try
							{
								string compImg;
								string imgpath = dbr.getAnomalieImgPath(texteMessage.Remove(texteMessage.Length - 2).Substring(10));
								if (imgpath != string.Empty)
								{
									Android.Graphics.Bitmap bmp = Android.Graphics.BitmapFactory.DecodeFile(imgpath);
									Android.Graphics.Bitmap rbmp = Android.Graphics.Bitmap.CreateScaledBitmap(bmp, bmp.Width / 5, bmp.Height / 5, true);
									compImg = imgpath.Replace(".jpg", "-1_1.jpg");
									using (var fs = new FileStream(compImg, FileMode.OpenOrCreate))
									{
										rbmp.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, fs);
									}
									//ftp://77.158.93.75 ftp://10.1.2.75
									ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
									//if jeantet
									if (prefs.GetString("API_LOCATION", null) == "JEANTET")
									{
										Thread threadimgpath = new Thread(() => Data.Instance.UploadFile("ftp://77.158.93.75", compImg, "DMS", "Linuxr00tn", ""));
										threadimgpath.Start();
									}
									else
									{
										if (prefs.GetString("API_LOCATION", null) != null)
										{
											Thread threadimgpath = new Thread(() => Data.Instance.UploadFile("ftp://176.31.10.168:54021", compImg, "DMSPHOTO", "DMS25000", ""));
											threadimgpath.Start();
										}
									}
								}
								dbr.insertDataMessage(Data.userAndsoft, "", "%%GETAIMG Done", 5, DateTime.Now, 5, 0);
							}
							catch (Exception ex)
							{
								Console.Out.Write("%%GETAIMG Upload file error\n" + ex);
								RaygunClient.Current.SendInBackground(ex);
							}
							break;
						case "%%STOPSER":
							StopForeground(true);
							StopSelf();
							break;
						case "%%REQUETE":
							string[] texteMessageInputSplit = Android.Text.TextUtils.Split(texteMessage, "%%");
							switch (texteMessageInputSplit[2])
							{
								case "TableMessages":
									var selMesg = dbr.QueryMessage(texteMessageInputSplit[3]);
									string rowMsg = "";
									rowMsg += "[";
									foreach (var item in selMesg)
									{
										rowMsg += "{" + item.codeChauffeur + "," + item.numMessage + "," + item.texteMessage + "," + item.utilisateurEmetteur + "},";
									}
									rowMsg.Remove(rowMsg.Length - 1);
									rowMsg += "]";
									dbr.insertDataMessage(Data.userAndsoft, "", rowMsg, 5, DateTime.Now, 5, 0);
									break;
								case "TableNotifications":
									var selNotif = dbr.QueryNotif(texteMessageInputSplit[3]);
									string rowNotif = "";
									rowNotif += "[";
									foreach (var item in selNotif)
									{
										rowNotif += "{" + item.numCommande + "," + item.numMessage + "," + item.dateNotificationMessage + "," + item.statutNotificationMessage + "," + item.Id + "},";
									}
									rowNotif.Remove(rowNotif.Length - 1);
									rowNotif += "]";
									dbr.insertDataMessage(Data.userAndsoft, "", rowNotif, 5, DateTime.Now, 5, 0);

									break;
								case "TablePositions":
									var selPos = dbr.QueryPositions(texteMessageInputSplit[3]);
									string rowPos = "";
									rowPos += "[";
									foreach (var item in selPos)
									{
										rowPos += "{" + item.numCommande + "," + item.groupage + "," + item.typeMission + "," + item.typeSegment + "," + item.refClient + "},";
									}
									rowPos.Remove(rowPos.Length - 1);
									rowPos += "]";
									dbr.insertDataMessage(Data.userAndsoft, "", rowPos, 5, DateTime.Now, 5, 0);
									break;
								case "TableColis":
									var selColis = dbr.QueryColis(texteMessageInputSplit[3]);
									string rowColis = "";
									rowColis += "[";
									foreach (var item in selColis)
									{
										rowColis += "{" + item.numCommande + "," + item.numColis + "," + item.dateflashage + "," + item.flashage + "},";
									}
									rowColis.Remove(rowColis.Length - 1);
									rowColis += "]";
									dbr.insertDataMessage(Data.userAndsoft, "", rowColis, 5, DateTime.Now, 5, 0);
									break;
								case "TableStatutPositions":
									var selStat = dbr.QueryStatuPos(texteMessageInputSplit[3]);
									string rowStatut = "";
									rowStatut += "[";
									foreach (var item in selStat)
									{
										rowStatut += "{" + item.codesuiviliv + "," + item.commandesuiviliv + "," + item.datajson + "," + item.datesuiviliv + "," + item.libellesuiviliv + "," + item.memosuiviliv + "," + item.statut + "},";
									}
									rowStatut.Remove(rowStatut.Length - 1);
									rowStatut += "]";
									dbr.insertDataMessage(Data.userAndsoft, "", rowStatut, 5, DateTime.Now, 5, 0);
									break;
								case "REORDER":
									string[] arraySQL = texteMessageInputSplit[3].Split('|');
									foreach (string item in arraySQL)
									{
										var subarrSQL = item.Split('-');
										dbr.updatePositionOrder(subarrSQL[0].ToString(), Convert.ToInt32(subarrSQL[1]));
									}
									break;
								default:
									var execreq = dbr.Execute(texteMessageInputSplit[3]);
									dbr.insertDataMessage(Data.userAndsoft, "", execreq + " lignes traitées : " + texteMessageInputSplit[3], 5, DateTime.Now, 5, 0);
									break;
							}
							break;
						default:
							dbr.insertDataMessage(codeChauffeur, utilisateurEmetteur, texteMessage, 0, DateTime.Now, 1, numMessage);
							dbr.InsertDataStatutMessage(0, DateTime.Now, numMessage, "", "");
							alertsms();
							Console.WriteLine(numMessage.ToString());
							break;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("\n" + ex);
				RaygunClient.Current.SendInBackground(ex);
			}
		}
		#endregion

		#region GPS
		//public void OnLocationChanged(Android.Locations.Location location)
		//{
		//	if (previousLocation == null)
		//	{
		//		gPS = location.Latitude + ";" + location.Longitude;
		//		Data.GPS = location.Latitude + ";" + location.Longitude;
		//		previousLocation = location;
		//	}
		//	else {
		//		if (true)
		//		{
		//			gPS = location.Latitude + ";" + location.Longitude;
		//			Data.GPS = location.Latitude + ";" + location.Longitude;
		//			previousLocation = location;
		//		}
		//	}
		//}
		//public void OnProviderDisabled(string provider)
		//{
		//}
		//public void OnProviderEnabled(string provider)
		//{
		//}
		//public void OnStatusChanged(string provider, Availability status, Bundle extras)
		//{
		//}

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
		#endregion

		public void alert()
		{
			MediaPlayer _player;
			_player = MediaPlayer.Create(this, Resource.Raw.beep4);
			_player.Start();
		}

		public void alertsms()
		{
			MediaPlayer _player;
			_player = MediaPlayer.Create(this, Resource.Raw.msg3);
			_player.Start();
		}

		public bool uploadFile(string ftpUrl, string fileName, string userName, string password, string uploadDirectory)
		{
			try
			{
				string pureFileName = new FileInfo(fileName).Name;
				String uploadUrl = String.Format("{0}{1}/{2}", ftpUrl, uploadDirectory, pureFileName);
				FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(uploadUrl);
				req.Proxy = null;
				req.Method = WebRequestMethods.Ftp.UploadFile;
				req.Credentials = new NetworkCredential(userName, password);
				req.UseBinary = true;
				req.UsePassive = true;
				byte[] data = System.IO.File.ReadAllBytes(fileName);
				req.ContentLength = data.Length;
				System.IO.Stream stream = req.GetRequestStream();
				stream.Write(data, 0, data.Length);
				stream.Close();
				FtpWebResponse res = (FtpWebResponse)req.GetResponse();
				Console.Out.Write("Upload file" + fileName + " good\n" + res);
				return true;
			}
			catch (Exception ex)
			{
				Console.Out.Write("Upload file" + fileName + " error\n" + ex);
				RaygunClient.Current.SendInBackground(ex);
				Thread.Sleep(TimeSpan.FromMinutes(2));
				uploadFile(ftpUrl, fileName, userName, password, uploadDirectory);
				return false;
			}
		}

		public async void OnConnected(Bundle connectionHint)
		{
			// Get Last known location
			var lastLocation = LocationServices.FusedLocationApi.GetLastLocation(googleApiClient);

			gPS = lastLocation == null ? "NULL" : gpsLocation(lastLocation);
			Data.GPS = lastLocation == null ? "NULL" : gpsLocation(lastLocation);

			await RequestLocationUpdates();
		}

		public void OnConnectionSuspended(int cause)
		{
			//throw new NotImplementedException();
		}

		public void OnConnectionFailed(ConnectionResult result)
		{
			googleApiClient.Connect();
			//throw new NotImplementedException();
		}

		async Task RequestLocationUpdates()
		{
			// Describe our location request
			var locationRequest = new LocationRequest()
				.SetInterval(2000)
				.SetFastestInterval(1000)
				.SetPriority(LocationRequest.PriorityHighAccuracy);

			// Check to see if we can request updates first
			if (await CheckLocationAvailability(locationRequest))
			{

				// Request updates
				await LocationServices.FusedLocationApi.RequestLocationUpdates(googleApiClient,
					locationRequest, this);
			}
		}

		async Task<bool> CheckLocationAvailability(LocationRequest locationRequest)
		{
			// Build a new request with the given location request
			var locationSettingsRequest = new LocationSettingsRequest.Builder()
				.AddLocationRequest(locationRequest)
				.Build();

			// Ask the Settings API if we can fulfill this request
			var locationSettingsResult = await LocationServices.SettingsApi.CheckLocationSettingsAsync(googleApiClient, locationSettingsRequest);
			return true;
		}

		public void OnLocationChanged(Android.Locations.Location location)
		{
			// Show latest location
			var l = gpsLocation(location);

			gPS = l;
			Data.GPS = l;
		}

		string DescribeLocation(Android.Locations.Location location)
		{
			return string.Format("{0}: {1}, {2} @ {3}",
				location.Provider,
				location.Latitude,
				location.Longitude,
				new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(location.Time));
		}
		string gpsLocation(Android.Locations.Location location)
		{
			return string.Format("{0};{1}", location.Latitude, location.Longitude);
		}
	}
}