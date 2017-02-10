using System;
using System.ComponentModel;
using System.Json;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Telephony;
using Android.Widget;
using AndroidHUD;
using DMS_3.BDD;


namespace DMS_3
{
	[Activity(Label = "DMS_3", Theme = "@style/MyTheme.Base", Icon = "@mipmap/icon", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : AppCompatActivity
	{
		Button btn_Login;
		EditText user;
		EditText password;
		TextView tableload;
		BackgroundWorker bgService;
		DBRepository dbr = new DBRepository();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);

			//DECLARATION DES ITEMS
			btn_Login = FindViewById<Button>(Resource.Id.btnlogin);
			user = FindViewById<EditText>(Resource.Id.user);
			password = FindViewById<EditText>(Resource.Id.password);
			tableload = FindViewById<TextView>(Resource.Id.tableload);

			if (!(Data.tableuserload == "true"))
			{
				tableload.Text = "Table user non chargée";
				tableload.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.Anom, 0, 0, 0);
			}
			//APPEL DES FONCTIONS
			btn_Login.LongClick += delegate
			{
				btn_Login_LongClick();
			};
			btn_Login.Click += delegate
			{
				btn_Login_Click();
			};
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnResume()
		{
			base.OnResume();
		}
		protected override void OnPause()
		{
			base.OnPause();
		}
		protected override void OnStop()
		{
			base.OnStop();
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		void btn_Login_Click()
		{
			if (!(user.Text == ""))
			{
				//INSTANCE DBREPOSITORY
				var usercheck = dbr.user_Check(user.Text.ToUpper(), password.Text);
				if (usercheck)
				{
					//UPDATE DE LA BDD AVEC CE USER
					dbr.setUserdata(user.Text.ToUpper());
					//lancement du BgWorker Service
					StartService(new Intent(this, typeof(ProcessDMS)));
					bgService = new BackgroundWorker();
					bgService.DoWork += new DoWorkEventHandler(bgService_DoWork);
					bgService.RunWorkerAsync();
					StartActivity(typeof(HomeActivity));
				}
				else {
					AndHUD.Shared.ShowError(this, "Mauvais mot de passe", MaskType.Black, TimeSpan.FromSeconds(2));
				}
			}
			else {
				AndHUD.Shared.ShowError(this, "Champ user obligatoire", MaskType.Black, TimeSpan.FromSeconds(2));
			}
		}

		private void bgService_DoWork(object sender, DoWorkEventArgs e)
		{
			while (true)
			{
				Thread.Sleep(600000);
				try
				{
					Console.WriteLine("Check Service Start" + DateTime.Now.ToString("T"));
					//si la diff est > 10 min relancer le service
					ISharedPreferences pref = Application.Context.GetSharedPreferences("AppInfo", FileCreationMode.Private);
					long servicedate = pref.GetLong("Service", 0L);
					try
					{
						if ((TimeSpan.FromTicks(DateTime.Now.Ticks - servicedate).TotalMinutes) > 10)
						{
							//LANCEMENT DU SERVICE
							if (Data.userAndsoft != null || Data.userAndsoft != "")
							{
								StartService(new Intent(this, typeof(ProcessDMS)));
							}
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

		void btn_Login_LongClick()
		{
			try
			{
				ShowProgress(progress => AndHUD.Shared.Show(this, "Chargement ... " + progress + "%", progress, MaskType.Clear));

			}
			catch (System.Exception ex)
			{
				Console.WriteLine(ex);
			}
		}



		void ShowProgress(Action<int> action)
		{
			int progress = 0;
			try
			{
				Task.Factory.StartNew(() =>
				{
					try
					{
						progress += 20;
						action(progress);

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


						//try jeantet
						try
						{
							_url = "http://dmsv3.jeantettransport.com/api/authenWsv4";
							userData = webClient.DownloadString(_url);

							if (userData == "[]")
							{
								_url = "***URLOVH****";
								userData = webClient.DownloadString(_url);
								if (userData != "[]")
								{
									//set pref API_LOCATION OVH
									editor.PutString("API_LOCATION", "OVH");
									editor.PutString("API_DOMAIN", "http://*****************");
									editor.Apply();
									Console.WriteLine("SET OVH");
								}
							}
							else
							{
								//set pref API_LOCATION JEANTET
								editor.PutString("API_LOCATION", "JEANTET");
								editor.PutString("API_DOMAIN", "http://dmsv3.jeantettransport.com");
								editor.Apply();
								Console.WriteLine("SET JEANTET");
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex);
						}


						if (userData != "[]")
						{
							RunOnUiThread(() => traitResponse(userData));
							RunOnUiThread(() => tableload.Text = "Table chargée");
							RunOnUiThread(() => tableload.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.Val, 0, 0, 0));
						}
						else
						{
							RunOnUiThread(() => tableload.Text = "Table non chargée");
							RunOnUiThread(() => tableload.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.Anom, 0, 0, 0));
						}

						progress += 80;
						action(progress);

						System.Console.WriteLine("\n Webclient User Terminé ...");

						AndHUD.Shared.Dismiss(this);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
						AndHUD.Shared.Dismiss(this);
					}


				});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				AndHUD.Shared.Dismiss(this);
			}

		}

		void traitResponse(string response)
		{
			//GESTION DU XML
			JsonArray jsonVal = JsonValue.Parse(response) as JsonArray;
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
		}
	}
}
