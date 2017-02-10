using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using DMS_3.BDD;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using RaygunClient = Mindscape.Raygun4Net.RaygunClient;
using RaygunIdentifierMessage = Mindscape.Raygun4Net.Messages.RaygunIdentifierMessage;
using Android.Telephony;
using Android.Bluetooth;

namespace DMS_3
{
	[Activity(Label = "HomeActivity", Theme = "@style/MyTheme.Base", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class HomeActivity : AppCompatActivity
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
		DBRepository dbr = new DBRepository();

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
			//btn_Livraison.LongClick += Btn_Livraison_LongClick;
			btn_Message.Click += delegate { btn_Message_Click(); };
			btn_Flash.Click += delegate { btn_Flash_Click(); };

			//FONTS
			txtLivraison.SetTypeface(Data.LatoBlack, Android.Graphics.TypefaceStyle.Normal);
			txtEnlevement.SetTypeface(Data.LatoBlack, Android.Graphics.TypefaceStyle.Normal);

			StartService(new Intent(this, typeof(ProcessDMS)));
		}

		//void Btn_Livraison_LongClick(object sender, View.LongClickEventArgs e)
		//{
		//	RaygunClient.Current.SendInBackground(new Exception("Something has gone horribly wrong"));
		//}

		void Btn_Config_LongClick(object sender, View.LongClickEventArgs e)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);

			builder.SetTitle("Deconnexion");

			builder.SetMessage("Voulez-vous vous déconnecter ?");
			builder.SetCancelable(false);
			builder.SetPositiveButton("Annuler", delegate { });
			builder.SetNegativeButton("Déconnexion", delegate
			{
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

			try
			{
				var user_Login = dbr.is_user_Log_In();
				dbr.SETBadges(Data.userAndsoft);
				if (user_Login == "false")
				{
					StartActivity(new Intent(Application.Context, typeof(MainActivity)));
				}
			}
			catch (System.Exception ex)
			{
				System.Console.WriteLine("Erreur sur is_user_Log_In" + ex);
				RaygunClient.Current.SendInBackground(ex);
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
	}
}