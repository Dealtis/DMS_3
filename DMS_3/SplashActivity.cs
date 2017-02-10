using System;
using System.ComponentModel;
using System.Json;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Preferences;
using Android.Telephony;
using Android.Widget;
using DMS_3.BDD;
using Mindscape.Raygun4Net.Messages;
using RaygunClient = Mindscape.Raygun4Net.RaygunClient;

namespace DMS_3
{
	[Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class SplashActivity : Activity
	{
		BackgroundWorker bgService;
		DBRepository dbr = new DBRepository();

		public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
		{
			base.OnCreate(savedInstanceState, persistentState);
		}

		protected override void OnResume()
		{
			base.OnResume();
			Task startupWork = new Task(() =>
			{
				//CREATION DE LA BDD
				DBRepository dbr = new DBRepository();
				//CREATION DES TABLES
				dbr.CreateTable();

				try
				{
					var telephonyManager = (TelephonyManager)GetSystemService(TelephonyService);
					var IMEI = telephonyManager.DeviceId;
					var webClient = new TimeoutWebclient();
					webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
					webClient.QueryString.Add("IMEI", IMEI);
					string userData = "";
					string _url = "";
					//si pref societe == null
					ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
					ISharedPreferencesEditor editor = prefs.Edit();

					if (prefs.GetString("API_LOCATION", String.Empty) != String.Empty)
					{
						switch (prefs.GetString("API_LOCATION", String.Empty))
						{
							case "JEANTET":
								_url = "http://dmsv3.jeantettransport.com/api/authenWsv4";
								break;
							case "OVH":
								_url = "https://dmsws.dealtis.fr/api/authenWsv4";
								break;
							default:
								break;
						}
						userData = webClient.DownloadString(_url);
					}
					else
					{
						//try jeantet
						try
						{
							_url = "http://dmsv3.jeantettransport.com/api/authenWsv4";
							userData = webClient.DownloadString(_url);

							if (userData == "[]")
							{
								_url = "https://dmsws.dealtis.fr/api/authenWsv4";
								userData = webClient.DownloadString(_url);
								if (userData != "[]")
								{
									//set pref API_LOCATION OVH
									editor.PutString("API_LOCATION", "OVH");
									editor.PutString("API_DOMAIN", "https://dmsws.dealtis.fr");
									editor.Apply();
								}
							}
							else
							{
								//set pref API_LOCATION JEANTET
								editor.PutString("API_LOCATION", "JEANTET");
								editor.PutString("API_DOMAIN", "http://dmsv3.jeantettransport.com");
								editor.Apply();
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex);
						}
					}

					//GESTION DU JSON
					JsonArray jsonVal = JsonValue.Parse(userData) as JsonArray;
					var jsonArr = jsonVal;
					foreach (var row in jsonArr)
					{
						var checkUser = dbr.user_AlreadyExist(row["userandsoft"], row["usertransics"], row["mdpandsoft"], row["User_Usesigna"], row["User_Societe"]);
						Console.WriteLine("\n" + checkUser + " " + row["userandsoft"]);
						if (!checkUser)
						{
							var IntegUser = dbr.InsertDataUser(row["userandsoft"], row["usertransics"], row["mdpandsoft"], row["User_Usesigna"], row["User_Usepartic"], row["User_Societe"]);
							Console.WriteLine("\n" + IntegUser);
						}
					}

					if (userData != "[]")
					{
						Data.tableuserload = "true";
					}
				}
				catch (System.Exception ex)
				{
					System.Console.WriteLine(ex);
					Thread.Sleep(5000);
				}


			});
			startupWork.ContinueWith(t =>
			{
				//Is a user login ?
				var user_Login = dbr.is_user_Log_In();
				if (!(user_Login == "false"))
				{
					//Data.userAndsoft = user_Login;
					dbr.setUserdata(user_Login);

					//lancement du BgWorker Service
					StartService(new Intent(this, typeof(ProcessDMS)));
					bgService = new BackgroundWorker();
					bgService.DoWork += new DoWorkEventHandler(bgService_DoWork);
					bgService.RunWorkerAsync();

					var telephonyManager = (TelephonyManager)GetSystemService(TelephonyService);

					// Raygun4Net
					RaygunClient.Initialize("VXMXLFnw+2LJyuTXX8taYg==").AttachCrashReporting().AttachPulse(this);
					RaygunClient.Current.UserInfo = new RaygunIdentifierMessage(Data.userAndsoft)
					{
						IsAnonymous = false,
						FullName = telephonyManager.DeviceId
					};

					StartActivity(new Intent(Application.Context, typeof(HomeActivity)));
				}
				else {
					StartActivity(new Intent(Application.Context, typeof(MainActivity)));
				}

			}, TaskScheduler.FromCurrentSynchronizationContext());
			startupWork.Start();
		}

		private void bgService_DoWork(object sender, DoWorkEventArgs e)
		{
			while (true)
			{
				Thread.Sleep(600000);
				try
				{

					Console.WriteLine("Check Service Start" + DateTime.Now.ToString("T"));
					ISharedPreferences pref = Application.Context.GetSharedPreferences("AppInfo", FileCreationMode.Private);
					long servicedate = pref.GetLong("Service", 0L);
					try
					{
						if ((TimeSpan.FromTicks(DateTime.Now.Ticks - servicedate).TotalMinutes) > 10)
						{
							//LANCEMENT DU SERVICE
							if (Data.userAndsoft == null || Data.userAndsoft == "")
							{
							}
							else {
								StartService(new Intent(this, typeof(ProcessDMS)));
								//dbr.InsertLogApp("",DateTime.Now,"Relance du service après 10 min d'inactivité");
							}
						}
						else {
							//dbr.InsertLogApp("",DateTime.Now,"Pas de Relance du service");
						}

					}
					catch (Exception ex)
					{
						Console.Out.Write(ex);
					}
				}
				catch (Exception ex)
				{
					Console.Write(ex);
				}
			}
		}

		void bgService_DoWork_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			bgService.DoWork += new DoWorkEventHandler(bgService_DoWork);
		}
	}
}