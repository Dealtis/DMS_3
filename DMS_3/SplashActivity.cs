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
using Android.Telephony;
using Android.Widget;
using DMS_3.BDD;
namespace DMS_3
{
	[Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class SplashActivity : Activity
	{
		BackgroundWorker bgService;

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
				DBRepository.Instance.CreateDB();
				//CREATION DES TABLES
				DBRepository.Instance.CreateTable();

				//TEST DE CONNEXION
				var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);

				//GetTelId
				TelephonyManager tel = (TelephonyManager)this.GetSystemService(Context.TelephonyService);
				var telId = tel.DeviceId;
				var activeConnection = connectivityManager.ActiveNetworkInfo;
				if ((activeConnection != null) && activeConnection.IsConnected)
				{
					try
					{
						string _url = "http://dmsv3.jeantettransport.com/api/authenWsv4";
						var telephonyManager = (TelephonyManager)GetSystemService(TelephonyService);
						var IMEI = telephonyManager.DeviceId;
						var webClient = new TimeoutWebclient();
						webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
						string userData = "";
						webClient.QueryString.Add("IMEI", IMEI);
						userData = webClient.DownloadString(_url);
						System.Console.WriteLine("\n Webclient User Terminé ...");
						//GESTION DU XML
						JsonArray jsonVal = JsonValue.Parse(userData) as JsonArray;
						var jsonArr = jsonVal;
						foreach (var row in jsonArr)
						{
							var checkUser = DBRepository.Instance.user_AlreadyExist(row["userandsoft"], row["usertransics"], row["mdpandsoft"], row["User_Usesigna"], row["User_Societe"]);
							Console.WriteLine("\n" + checkUser + " " + row["userandsoft"]);
							if (!checkUser)
							{
								var IntegUser = DBRepository.Instance.InsertDataUser(row["userandsoft"], row["usertransics"], row["mdpandsoft"], row["User_Usesigna"], row["User_Usepartic"], row["User_Societe"]);
								Console.WriteLine("\n" + IntegUser);
							}
						}
						//execute de la requete
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
				}

			});
			startupWork.ContinueWith(t =>
			{
				//Is a user login ?
				var user_Login = DBRepository.Instance.is_user_Log_In();
				if (!(user_Login == "false"))
				{
					//Data.userAndsoft = user_Login;
					DBRepository.Instance.setUserdata(user_Login);

					//lancement du BgWorker Service
					StartService(new Intent(this, typeof(ProcessDMS)));
					bgService = new BackgroundWorker();
					bgService.DoWork += new DoWorkEventHandler(bgService_DoWork);
					bgService.RunWorkerAsync();
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
								//DBRepository.Instance.InsertLogApp("",DateTime.Now,"Relance du service après 10 min d'inactivité");
							}
						}
						else {
							//DBRepository.Instance.InsertLogApp("",DateTime.Now,"Pas de Relance du service");
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