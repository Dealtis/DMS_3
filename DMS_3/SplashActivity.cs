using System;
using System.ComponentModel;
using System.IO;
using System.Json;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Support.V7.App;
using Android.Telephony;
using Android.Widget;
using DMS_3.BDD;
using Xamarin;

namespace DMS_3
{
	[Activity (Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class SplashActivity : AppCompatActivity
	{
		BackgroundWorker bgService;


		public override void OnCreate (Bundle savedInstanceState, PersistableBundle persistentState)
		{
			base.OnCreate (savedInstanceState, persistentState);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			Task startupWork = new Task (() => {
				//INSTANCE DBREPOSITORY
				DBRepository dbr = new DBRepository ();
				//CREATION DE LA BDD
				dbr.CreateDB ();
				//CREATION DES TABLES
				dbr.CreateTable ();

				//suppression des logs trop anciens 3 jours
				//dbr.purgeLog();

				//TEST DE CONNEXION
				var connectivityManager = (ConnectivityManager)GetSystemService (ConnectivityService);
				var t = DateTime.Now.ToString ("dd_MM_yy");
				string dir_log = (Android.OS.Environment.GetExternalStoragePublicDirectory (Android.OS.Environment.DirectoryDownloads)).ToString ();
				//Shared Preference
				ISharedPreferences pref = Application.Context.GetSharedPreferences ("AppInfo", FileCreationMode.Private);
				string log = pref.GetString ("Log", String.Empty);
				//GetTelId
				TelephonyManager tel = (TelephonyManager)this.GetSystemService (Context.TelephonyService);
				var telId = tel.DeviceId;
				//Si il n'y a pas de shared pref
				if (log == String.Empty) {
					Data.log_file = Path.Combine (dir_log, t + "_" + telId + "_log.txt");
					ISharedPreferencesEditor edit = pref.Edit ();
					edit.PutString ("Log", Data.log_file);
					edit.Apply ();
				} else {
					//il y a des shared pref
					Data.log_file = pref.GetString ("Log", String.Empty);
					if (!(Data.log_file.Substring(26,Math.Min(Data.log_file.Length,2)).Equals(DateTime.Now.Day.ToString("00")))) {
						File.Delete (Data.log_file);
						Data.log_file = Path.Combine (dir_log, t + "_" + telId + "_log.txt");
						ISharedPreferencesEditor edit = pref.Edit ();
						edit.PutString ("Log", Data.log_file);
						edit.Apply ();
						Data.log_file = pref.GetString ("Log", String.Empty);
					}
				}
				File.AppendAllText (Data.log_file, "[LAUNCH] DMS lancée le " + DateTime.Now.ToString ("F") + "\n");
				bool App_Connec = false;
				while (!App_Connec) {
					var activeConnection = connectivityManager.ActiveNetworkInfo;
					if ((activeConnection != null) && activeConnection.IsConnected) {
						try {
							string _url = "http://dmsv3.jeantettransport.com/api/authen?chaufmdp=";
							var webClient = new WebClient ();
							webClient.Headers [HttpRequestHeader.ContentType] = "application/json";
							string userData = "";
							userData = webClient.DownloadString (_url);
							System.Console.WriteLine ("\n Webclient User Terminé ...");
							//GESTION DU XML
							JsonArray jsonVal = JsonArray.Parse (userData) as JsonArray;
							var jsonArr = jsonVal;
							foreach (var row in jsonArr) {
								var checkUser = dbr.user_AlreadyExist (row ["userandsoft"], row ["usertransics"], row ["mdpandsoft"], "true");
								Console.WriteLine ("\n" + checkUser + " " + row ["userandsoft"]);
								if (!checkUser) {
									var IntegUser = dbr.InsertDataUser (row ["userandsoft"], row ["usertransics"], row ["mdpandsoft"], "true");
									Console.WriteLine ("\n" + IntegUser);
								}
							}
							//execute de la requete
							Data.tableuserload = true;
							App_Connec = true;
						} catch (System.Exception ex) {
							System.Console.WriteLine (ex);
							Insights.Report (ex);
							App_Connec = false;
							//AndHUD.Shared.ShowError (this, "Une erreur c'est produite lors du lancement, réessaie dans 5 secondes", MaskType.Black, TimeSpan.FromSeconds (5));
							Toast.MakeText (this, "Une erreur c'est produite lors du lancement, réessaie dans 5 secondes", ToastLength.Long).Show ();
						}
					} else {
						App_Connec = false;
						//AndHUD.Shared.ShowError(this, "Pas de connexion, réessaie dans 5 secondes", MaskType.Black, TimeSpan.FromSeconds(5));
						Toast.MakeText (this, "Pas de connexion", ToastLength.Long).Show ();
						Thread.Sleep (5000);
					}
				}
			});
			startupWork.ContinueWith (t => {
				//Is a user login ?
				DBRepository dbr = new DBRepository ();
				var user_Login = dbr.is_user_Log_In ();
				if (!(user_Login == string.Empty)) {
					//Data.userAndsoft = user_Login;
					dbr.setUserdata (user_Login);
					File.AppendAllText (Data.log_file, "Connexion de " + Data.userAndsoft + " à " + DateTime.Now.ToString ("G") + "\n");

					//lancement du BgWorker Service
					StartService (new Intent (this, typeof(ProcessDMS)));
					bgService = new BackgroundWorker();
					bgService.DoWork += new DoWorkEventHandler(bgService_DoWork);
					bgService.RunWorkerAsync();
					StartActivity (new Intent (Application.Context, typeof(HomeActivity)));
				} else {
					StartActivity (new Intent (Application.Context, typeof(MainActivity)));
				}
			}, TaskScheduler.FromCurrentSynchronizationContext ());
			startupWork.Start ();
		}

		private void bgService_DoWork(object sender, DoWorkEventArgs e)
		{
		while (true) {
				Thread.Sleep(600000);
				try {
					//DBRepository dbr = new DBRepository();
					//dbr.InsertLogApp("",DateTime.Now,"Check Service Start");
					Console.WriteLine ("Check Service Start"+DateTime.Now.ToString("T"));
					//verification de la date de la pre Service
					//si la diff est > 10 min relancer le service
					//string dir_log = (Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads)).ToString();
					ISharedPreferences pref = Application.Context.GetSharedPreferences("AppInfo", FileCreationMode.Private);
					long servicedate = pref.GetLong("Service",0L);

					try {				
						if ((TimeSpan.FromTicks(DateTime.Now.Ticks-servicedate).TotalMinutes)>10){
							//LANCEMENT DU SERVICE
							if (Data.userAndsoft == null || Data.userAndsoft == "") {
							} else {
								StartService (new Intent (this, typeof(ProcessDMS)));					
								//dbr.InsertLogApp("",DateTime.Now,"Relance du service après 10 min d'inactivité");
								File.AppendAllText(Data.log_file, "["+DateTime.Now.ToString("t")+"]"+"[SERVICE] Relance du service après 10 min d'inactivité"+DateTime.Now.ToString("G")+"\n");
							}
						}else{
							//dbr.InsertLogApp("",DateTime.Now,"Pas de Relance du service");
						}

					} catch (Exception ex) {
						Console.Out.Write (ex);
					}
				} catch (Exception ex) {
					Console.Write(ex);
				}
			}
		}

		void bgService_DoWork_Completed (object sender, RunWorkerCompletedEventArgs e)
		{
			bgService.DoWork += new DoWorkEventHandler(bgService_DoWork);
		}
	}
}
