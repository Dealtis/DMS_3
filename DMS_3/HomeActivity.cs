using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DMS_3.BDD;
using HockeyApp.Android;
using HockeyApp.Android.Metrics;

namespace DMS_3
{
	[Activity(Label = "HomeActivity", Theme = "@android:style/Theme.Black.NoTitleBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class HomeActivity : Activity
	{
		TextView lblTitle;
		TextView peekupBadgeText;
		TextView newMsgBadgeText;
		TextView deliveryBadgeText;
		TextView txtLivraison;
		TextView txtEnlevement;
		RelativeLayout deliveryBadge;
		RelativeLayout peekupBadge;
		RelativeLayout newMsgBadge;
		System.Timers.Timer indicatorTimer;
		ProcessDMSBinder binder;
		ProcessDMSConnection processDMSConnection;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Home);

			//DECLARATION DES ITEMS
			lblTitle = FindViewById<TextView>(Resource.Id.lblTitle);
			peekupBadgeText = FindViewById<TextView>(Resource.Id.peekupBadgeText);
			newMsgBadgeText = FindViewById<TextView>(Resource.Id.newMsgBadgeText);
			txtLivraison = FindViewById<TextView>(Resource.Id.txtLivraison);
			txtEnlevement = FindViewById<TextView>(Resource.Id.txtEnlevement);
			deliveryBadgeText = FindViewById<TextView>(Resource.Id.deliveryBadgeText);
			deliveryBadge = FindViewById<RelativeLayout>(Resource.Id.deliveryBadge);
			peekupBadge = FindViewById<RelativeLayout>(Resource.Id.peekupBadge);
			newMsgBadge = FindViewById<RelativeLayout>(Resource.Id.newMsgBadge);
			peekupBadge.Visibility = ViewStates.Gone;
			deliveryBadge.Visibility = ViewStates.Gone;
			newMsgBadge.Visibility = ViewStates.Gone;

			//click button
			LinearLayout btn_Livraison = FindViewById<LinearLayout>(Resource.Id.columnlayout1_1);
			LinearLayout btn_Enlevement = FindViewById<LinearLayout>(Resource.Id.columnlayout1_2);
			LinearLayout btn_Message = FindViewById<LinearLayout>(Resource.Id.columnlayout2_1);
			LinearLayout btn_Flash = FindViewById<LinearLayout>(Resource.Id.columnlayout2_2);
			LinearLayout btn_Config = FindViewById<LinearLayout>(Resource.Id.columnlayout4_2);

			btn_Livraison.Click += delegate { btn_Livraison_Click(); };
			btn_Enlevement.Click += delegate { btn_Enlevement_Click(); };
			btn_Config.LongClick += Btn_Config_LongClick;
			btn_Message.Click += delegate { btn_Message_Click(); };
			btn_Flash.Click += delegate { btn_Flash_Click(); };

			string APP_ID = "337f4f12782f47e590a7e84867bc087a";

			//Hockey APP
			CrashManager.Register(this, "337f4f12782f47e590a7e84867bc087a");
			MetricsManager.Register(Application, "337f4f12782f47e590a7e84867bc087a");
			MetricsManager.EnableUserMetrics();

			CrashManager.Register(this, APP_ID, new HockeyCrashManagerSettings());



			//FONTS
			txtLivraison.SetTypeface(Data.LatoBlack, Android.Graphics.TypefaceStyle.Normal);
			txtEnlevement.SetTypeface(Data.LatoBlack, Android.Graphics.TypefaceStyle.Normal);
			if (processDMSConnection != null)
				binder = processDMSConnection.Binder;

			var processServiceIntent = new Intent("com.dealtis.dms_3.ProcessDMS");
			processDMSConnection = new ProcessDMSConnection(this);
			ApplicationContext.BindService(processServiceIntent, processDMSConnection, Bind.AutoCreate);
		}

		void Btn_Livraison_LongClick(object sender, View.LongClickEventArgs e)
		{
			DBRepository dbr = new DBRepository();
			dbr.resetColis();
		}

		void Btn_Config_LongClick(object sender, View.LongClickEventArgs e)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);

			builder.SetTitle("Deconnexion");

			builder.SetMessage("Voulez-vous vous déconnecter ?");
			builder.SetCancelable(false);
			builder.SetPositiveButton("Annuler", delegate { });
			builder.SetNegativeButton("Déconnexion", delegate
			{
				DBRepository dbr = new DBRepository();
				dbr.logout();
				Data.userAndsoft = null;
				Data.userTransics = null;
				StopService(new Intent(this, typeof(ProcessDMS)));
				Data.Is_Service_Running = false;
				Intent intent = new Intent(this, typeof(MainActivity));
				this.StartActivity(intent);
		//this.OverridePendingTransition (Resource.Animation.abc_slide_in_top,Resource.Animation.abc_slide_out_bottom);
	});
			builder.Show();
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnResume()
		{
			base.OnResume();

			//var user = dbr.getUserAndsoft();
			//dbr.setUserdata(user);
			//dbr.SETBadges(Data.userAndsoft);

			DBRepository dbr = new DBRepository();
			var user_Login = dbr.is_user_Log_In();
			if (user_Login == "false")
			{
				StartActivity(new Intent(Application.Context, typeof(MainActivity)));
			}

			var version = this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName;
			lblTitle.Text = Data.userAndsoft + " " + version;
			indicatorTimer = new System.Timers.Timer();

			indicatorTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnIndicatorTimerHandler);
			indicatorTimer.Interval = 1000;
			indicatorTimer.Enabled = true;
			indicatorTimer.Start();
		}

		void OnIndicatorTimerHandler(object sender, System.Timers.ElapsedEventArgs e)
		{
			//cacher les badges si inférieur à 1 else afficher et mettre le nombre
			if (Data.Instance.getLivraisonIndicator() < 1)
			{
				RunOnUiThread(() => deliveryBadge.Visibility = ViewStates.Gone);
			}
			else {
				RunOnUiThread(() => deliveryBadgeText.Text = Data.Instance.getLivraisonIndicator().ToString());
				RunOnUiThread(() => deliveryBadge.Visibility = ViewStates.Visible);
			}

			if (Data.Instance.getEnlevementIndicator() < 1)
			{
				RunOnUiThread(() => peekupBadge.Visibility = ViewStates.Gone);
			}
			else {
				RunOnUiThread(() => peekupBadgeText.Text = Data.Instance.getEnlevementIndicator().ToString());
				RunOnUiThread(() => peekupBadge.Visibility = ViewStates.Visible);
			}

			if (Data.Instance.getMessageIndicator() < 1)
			{
				RunOnUiThread(() => newMsgBadge.Visibility = ViewStates.Gone);
			}
			else {
				RunOnUiThread(() => newMsgBadgeText.Text = Data.Instance.getMessageIndicator().ToString());
				RunOnUiThread(() => newMsgBadge.Visibility = ViewStates.Visible);
			}
		}

		protected override void OnStop()
		{
			indicatorTimer.Stop();
			base.OnStop();
		}

		protected override void OnPause()
		{
			indicatorTimer.Stop();
			base.OnPause();
		}

		protected override void OnRestart()
		{
			base.OnRestart();
		}

		void btn_Livraison_Click()
		{
			Intent intent = new Intent(this, typeof(ListeLivraisonsActivity));
			intent.PutExtra("TYPE", "LIV");
			intent.PutExtra("TRAIT", "false");
			this.StartActivity(intent);
			Finish();
		}

		void btn_Flash_Click()
		{
			Intent intent = new Intent(this, typeof(FlashageQuaiActivity));
			this.StartActivity(intent);
			Finish();
		}

		void btn_Enlevement_Click()
		{
			Intent intent = new Intent(this, typeof(ListeLivraisonsActivity));
			intent.PutExtra("TYPE", "RAM");
			intent.PutExtra("TRAIT", "false");
			this.StartActivity(intent);
			Finish();
		}

		void btn_Message_Click()
		{
			Intent intent = new Intent(this, typeof(MessageActivity));
			this.StartActivity(intent);
			Finish();
		}

		public override void OnBackPressed()
		{
			System.Console.WriteLine("Do nothing");
		}

		class ProcessDMSConnection : Java.Lang.Object, IServiceConnection
		{
			HomeActivity activity;
			ProcessDMSBinder binder;

			public ProcessDMSBinder Binder
			{
				get
				{
					return binder;
				}
			}
			public ProcessDMSConnection(HomeActivity activity)
			{
				this.activity = activity;
			}
			public void OnServiceConnected(ComponentName name, IBinder service)
			{
				var demoServiceBinder = service as ProcessDMSBinder;
				if (demoServiceBinder != null)
				{
					activity.binder = (ProcessDMSBinder)service;
					if (Data.userAndsoft != null || Data.userAndsoft != "")
					{
						Data.Is_Service_Running = true;
					}
					this.binder = (ProcessDMSBinder)service;
				}
			}
			public void OnServiceDisconnected(ComponentName name)
			{
				Data.Is_Service_Running = false;
			}
		}
	}
}