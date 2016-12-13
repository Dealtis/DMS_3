using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.Media;
using Android.Net;
using Android.OS;
using DMS_3.BDD;

namespace DMS_3
{
	[Service]
	[IntentFilter(new string[] { "com.dealtis.dms_3.ProcessDMS" })]
	public class ProcessDMS : Service, ILocationListener
	{
		#region Variables
		ProcessDMSBinder binder;
		string datedujour;
		LocationManager locMgr;

		string userAndsoft;
		string userTransics;
		string gPS;
		Location previousLocation;
		string _locationProvider;
		string stringValues;
		string stringNotif;

		DBRepository dbr = new DBRepository();
		#endregion

		//string log_file;
		public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
		{
			userAndsoft = dbr.getUserAndsoft();
			userTransics = dbr.getUserTransics();

			StartServiceInForeground();
			Routine();

			//initialize location manager
			InitializeLocationManager();

			if (_locationProvider != "")
			{
				locMgr.RequestLocationUpdates(_locationProvider, 0, 0, this);
				Console.Out.Write("Listening for location updates using " + _locationProvider + ".");
			}

			return StartCommandResult.Sticky;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			StopForeground(true);
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
			Task.Factory.StartNew(() =>
				{
					while (true)
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
								Task.Factory.StartNew(
									() =>
									{
										Console.WriteLine("\nHello from ComPosNotifMsg.");
										//dbr.InsertLogService("",DateTime.Now,"ComPosNotifMsg Start");
										ComPosNotifMsg();
										Thread.Sleep(500);
									}
								).ContinueWith(
									t =>
									{
										Console.WriteLine("\nHello from ComWebService.");
										//dbr.InsertLogService("",DateTime.Now,"ComWebService Start");
										ComWebService();
										Thread.Sleep(500);
									}
								);
							}
						}

						ISharedPreferences pref = Application.Context.GetSharedPreferences("AppInfo", FileCreationMode.Private);
						ISharedPreferencesEditor edit = pref.Edit();
						edit.PutLong("Service", DateTime.Now.Ticks);
						edit.Apply();
						Console.Out.WriteLine("Service timer :" + pref.GetLong("Service", 0));
						Thread.Sleep(120000);
					}
				});
		}

		public override Android.OS.IBinder OnBind(Android.Content.Intent intent)
		{
			binder = new ProcessDMSBinder(this);
			return binder;
		}

		#region Webservice
		void InsertData()
		{
			datedujour = DateTime.Now.ToString("yyyyMMdd");

			//récupération de donnée via le webservice
			string content_integdata = String.Empty;
			try
			{
				string _url = "http://dmsv3.jeantettransport.com/api/commandeWsv4?codechauffeur=" + userTransics + "&datecommande=" + datedujour + "";
				var webClient = new WebClient();
				webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
				webClient.Encoding = System.Text.Encoding.UTF8;
				content_integdata = webClient.DownloadString(_url);
				//intégration des données dans la BDD
				JsonArray jsonVal = JsonValue.Parse(content_integdata) as JsonArray;
				var jsonArr = jsonVal;
				if (content_integdata != "[]")
				{
					stringValues = string.Empty;
					stringNotif = string.Empty;
					foreach (var row in jsonArr)
					{
						bool checkpos = dbr.pos_AlreadyExist(row["numCommande"], row["groupage"], row["typeMission"], row["typeSegment"]);
						if (!checkpos)
						{
							stringValues += " SELECT " + row["idSegment"].ToString() + "," + row["codeLivraison"].ToString() + "," + row["numCommande"].ToString() + "," + row["nomClient"].ToString() + "," + row["refClient"].ToString() + "," + row["nomPayeur"].ToString() + "," + row["adresseLivraison"].ToString() + "," + row["CpLivraison"].ToString() + "," + row["villeLivraison"].ToString() + "," + row["dateExpe"].ToString() + "," + row["nbrColis"].ToString() + "," + row["nbrPallette"].ToString() + "," + row["poids"].ToString() + "," + row["adresseExpediteur"].ToString() + "," + row["CpExpediteur"].ToString() + "," + row["dateExpe"].ToString() + "," + row["villeExpediteur"].ToString() + "," + row["nomExpediteur"].ToString() + "," + row["instrucLivraison"].ToString() + "," + row["groupage"].ToString() + "," + row["PoidsADR"].ToString() + "," + row["PoidsQL"].ToString() + "," + row["typeMission"].ToString() + "," + row["typeSegment"].ToString() + ",0," + row["CR"].ToString() + "," + row["ASSIGNE"].ToString() + "," + DateTime.Now.Day + "," + row["Datemission"].ToString() + "," + row["Ordremission"].ToString() + "," + row["planDeTransport"].ToString() + ",\"" + userAndsoft + "\"," + row["nomClientLivraison"].ToString() + "," + row["villeClientLivraison"].ToString() + "," + row["PositionPole"].ToString() + ",\"null\" UNION ALL";

							foreach (JsonValue item in row["detailColis"])
							{
								dbr.InsertDataColis(item["NumColis"], row["numCommande"]);
							}
						}
						//NOTIF
						stringNotif += "" + row["numCommande"] + "|";
					}
					if (stringValues != string.Empty)
					{
						string stringinsertpos = "INSERT INTO ";
						stringinsertpos += "TablePositions ( idSegment, codeLivraison, numCommande, nomClient, refClient, nomPayeur, adresseLivraison, CpLivraison, villeLivraison, dateHeure, nbrColis, nbrPallette, poids, adresseExpediteur, CpExpediteur, dateExpe, villeExpediteur, nomExpediteur, instrucLivraison, GROUPAGE, poidsADR, poidsQL, typeMission, typeSegment, statutLivraison, CR, ASSIGNE, dateBDD, Datemission, Ordremission, planDeTransport, Userandsoft, nomClientLivraison, villeClientLivraison, positionPole,imgpath)";
						stringinsertpos += " ";
						stringinsertpos += stringValues.Remove(stringValues.Length - 9);
						var execreq = dbr.Execute(stringinsertpos);

						Console.Out.WriteLine(execreq);
						//SON
						if (content_integdata == "[]")
						{
						}
						else {
							alert();
						}
					}
					if (stringNotif != string.Empty)
					{
						string stringinsertnotif = "INSERT INTO TableNotifications ( statutNotificationMessage, dateNotificationMessage, numMessage, numCommande ) VALUES ('10','" + DateTime.Now + "','1','" + stringNotif.Remove(stringNotif.Length - 1) + "')";
						var execreqnotif = dbr.Execute(stringinsertnotif);
						Console.Out.WriteLine("Execnotif" + execreqnotif);
					}
				}

				//select des grp's
				string content_grpcloture = String.Empty;
				var tablegroupage = dbr.QueryPositions("SELECT groupage FROM TablePositions group by groupage");
				foreach (var row in tablegroupage)
				{
					string numGroupage = row.groupage;
					Console.WriteLine(numGroupage);
					try
					{
						string _urlb = "http://dmsv3.jeantettransport.com/api/groupage?voybdx=" + numGroupage + "";
						var webClientGrp = new WebClient();
						webClientGrp.Headers[HttpRequestHeader.ContentType] = "application/json";
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
					}
				}
			}
			catch (Exception ex)
			{
				content_integdata = "[]";
				Console.WriteLine("\n" + ex);
			}

			//SET des badges
			dbr.SETBadges(Data.userAndsoft);
			Console.WriteLine("\nTask InsertData done");
		}


		void ComPosNotifMsg()
		{
			//API GPS OK
			var webClient = new WebClient();
			try
			{
				string content_msg = String.Empty;
				//ROUTINE INTEG MESSAGE
				try
				{
					//API LIVRER OK
					string _urlb = "http://dmsv3.jeantettransport.com/api/WSV32?codechauffeur=" + userAndsoft + "";
					var webClientb = new WebClient();
					webClientb.Headers[HttpRequestHeader.ContentType] = "application/json";
					webClientb.Encoding = System.Text.Encoding.UTF8;

					content_msg = webClientb.DownloadString(_urlb);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
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
					System.Uri uri = new System.Uri("http://dmsv3.jeantettransport.com/api/WSV32?codechauffeur=" + userAndsoft + "");
					webClient.UploadStringCompleted += WebClient_UploadStringStatutCompleted;
					webClient.UploadStringAsync(uri, datajson);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
			catch (Exception ex)
			{
				Console.Out.Write(ex);
			}
			Console.WriteLine("\nTask ComPosGps done");
		}

		void ComWebService()
		{
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
				System.Uri uri = new System.Uri("http://dmsv3.jeantettransport.com/api/livraisongroupagev3");
				try
				{
					webClient.UploadStringCompleted += WebClient_UploadStringCompleted;
					webClient.UploadStringAsync(uri, datajsonArray);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
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
			}
		}

		void WebClient_UploadStringStatutCompleted(object sender, UploadStringCompletedEventArgs e)
		{
			try
			{
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
			}
		}

		void traitMessages(string codeChauffeur, string texteMessage, string utilisateurEmetteur, int numMessage)
		{
			try
			{
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
							if (_locationProvider != "")
							{
								locMgr.RequestLocationUpdates(_locationProvider, 0, 0, this);
								Console.Out.Write("Listening for location updates using " + _locationProvider + ".");
								dbr.insertDataMessage(Data.userAndsoft, "", "Listening for location updates using " + _locationProvider + ".", 5, DateTime.Now, 5, 0);
							}
							else
							{
								dbr.insertDataMessage(Data.userAndsoft, "", "Location provider null", 5, DateTime.Now, 5, 0);
							}
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
									Thread threadimgpath = new Thread(() => uploadFile("ftp://77.158.93.75", compImg, "DMS", "Linuxr00tn", ""));
									threadimgpath.Start();
								}
								dbr.insertDataMessage(Data.userAndsoft, "", "%%GETAIMG Done", 5, DateTime.Now, 5, 0);
							}
							catch (Exception ex)
							{
								Console.Out.Write("%%GETAIMG Upload file error\n" + ex);
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
										rowColis += "{" + item.numCommande + "," + item.numColis + "," + item.dateflashage + "," + item.flashage+ "},";
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
									//string vTest = "7521157-23|7504793-22|7504911-21|7520938-20|7508789-19|7508319-18|7520258-17|7517092-16|7504745-15|7504742-14|7518147-13|7493546-12|7520226-11|7521200-10|7518630-9|7519496-8|7518382-7|7482816-6|7504679-5|7521031-4|7496922-3|7496918-2|7484337-1|";
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
			}
		}
		#endregion

		#region GPS
		public void OnLocationChanged(Android.Locations.Location location)
		{
			if (previousLocation == null)
			{
				gPS = location.Latitude + ";" + location.Longitude;
				Data.GPS = location.Latitude + ";" + location.Longitude;
				previousLocation = location;
			}
			else {
				if (true)
				{
					gPS = location.Latitude + ";" + location.Longitude;
					Data.GPS = location.Latitude + ";" + location.Longitude;
					previousLocation = location;
				}
			}
		}
		public void OnProviderDisabled(string provider)
		{
		}
		public void OnProviderEnabled(string provider)
		{
		}
		public void OnStatusChanged(string provider, Availability status, Bundle extras)
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
				Thread.Sleep(TimeSpan.FromMinutes(2));
				uploadFile(ftpUrl, fileName, userName, password, uploadDirectory);
				return false;
			}
		}
	}

	public class ProcessDMSBinder : Binder
	{
		ProcessDMS service;

		public ProcessDMSBinder(ProcessDMS service)
		{
			this.service = service;
		}

		public ProcessDMS GetDemoService()
		{
			return service;
		}

		public ProcessDMS StopService()
		{
			return service;
		}
	}
}