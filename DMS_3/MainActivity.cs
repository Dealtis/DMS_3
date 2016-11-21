using System;
using System.ComponentModel;
using System.Json;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Telephony;
using Android.Widget;
using AndroidHUD;
using DMS_3.BDD;


namespace DMS_3
{
	[Activity(Label = "DMS_3",Theme = "@style/MyTheme.Base", Icon = "@mipmap/icon", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : AppCompatActivity
	{
		Button btn_Login;
		EditText user;
		EditText password;
		TextView tableload;
		BackgroundWorker bgService;

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
				DBRepository dbr = new DBRepository();
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
			Task.Factory.StartNew(() =>
			{

				int progress = 0;
				progress += 20;
				action(progress);
				DBRepository dbr = new DBRepository();
				string _url = "http://dmsv3.jeantettransport.com/api/authenWsv4";
				var telephonyManager = (TelephonyManager)GetSystemService(TelephonyService);
				var IMEI = telephonyManager.DeviceId;
				var webClient = new WebClient();
				webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
				string userData = "";
				webClient.QueryString.Add("IMEI", IMEI);
				userData = webClient.DownloadString(_url);
				progress += 30;
				action(progress);
				System.Console.WriteLine("\n Webclient User Terminé ...");

				//GESTION DU XML
				JsonArray jsonVal = JsonValue.Parse(userData) as JsonArray;
				var jsonArr = jsonVal;
				foreach (var row in jsonArr)
				{
					var checkUser = dbr.user_AlreadyExist(row["userandsoft"], row["usertransics"], row["mdpandsoft"], "true");
					Console.WriteLine("\n" + checkUser + " " + row["userandsoft"]);
					if (!checkUser)
					{
						var IntegUser = dbr.InsertDataUser(row["userandsoft"], row["usertransics"], row["mdpandsoft"], "true");
						Console.WriteLine("\n" + IntegUser);
					}
				}

				progress += 50;
				action(progress);
				RunOnUiThread(() => tableload.Text = "Table chargée");
				RunOnUiThread(() => tableload.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.Val, 0, 0, 0));
				Data.tableuserload = "true";
				AndHUD.Shared.Dismiss(this);
				AndHUD.Shared.ShowSuccess(this, "Table mise à jour", MaskType.Black, TimeSpan.FromSeconds(1));
			});
		}
	}
}
