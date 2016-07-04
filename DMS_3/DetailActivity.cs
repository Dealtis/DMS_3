using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using DMS_3.BDD;
using Xamarin;
namespace DMS_3
{
	[Activity(Label = "DetailActivity", Theme = "@android:style/Theme.Black.NoTitleBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, NoHistory = true)]
	public class DetailActivity : Activity, GestureDetector.IOnGestureListener
	{
		private GestureDetector _gestureDetector;
		private int SWIPE_MAX_OFF_PATH = 250;
		private int SWIPE_MIN_DISTANCE = 120;
		private int SWIPE_THRESHOLD_VELOCITY = 150;

		//RECUP ID 
		string id;
		int i;
		int idprev;
		int idnext;

		TablePositions data;

		TextView codelivraison;
		TextView commande;
		TextView infolivraison;
		TextView title;
		TextView infosupp;
		TextView infoclient;
		TextView client;
		TextView anomaliet;
		TextView anomalie;
		TextView destfinal;
		ImageView _imageView;

		Button btnvalide;

		string type;
		//string tyValide;

		Bitmap imgbitmap;

		//private AlertDialog.Builder dialog;


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			id = Intent.GetStringExtra("ID");
			i = int.Parse(id);

			type = Intent.GetStringExtra("TYPE");

			DBRepository dbr = new DBRepository();
			data = dbr.GetPositionsData(i);
			idprev = dbr.GetidPrev(i);
			idnext = dbr.GetidNext(i);

			SetContentView(Resource.Layout.DetailPosition);
			_gestureDetector = new GestureDetector(this);

			//AFFICHE DATA
			codelivraison = FindViewById<TextView>(Resource.Id.codelivraison);
			commande = FindViewById<TextView>(Resource.Id.commande);
			infolivraison = FindViewById<TextView>(Resource.Id.infolivraison);
			title = FindViewById<TextView>(Resource.Id.title);
			infosupp = FindViewById<TextView>(Resource.Id.infosupp);
			infoclient = FindViewById<TextView>(Resource.Id.infoclient);
			client = FindViewById<TextView>(Resource.Id.client);
			anomaliet = FindViewById<TextView>(Resource.Id.anomaliet);
			anomalie = FindViewById<TextView>(Resource.Id.infoanomalie);
			destfinal = FindViewById<TextView>(Resource.Id.destfinal);
			_imageView = FindViewById<ImageView>(Resource.Id._imageView);
			btnvalide = FindViewById<Button>(Resource.Id.valide);

			Button btnanomalie = FindViewById<Button>(Resource.Id.anomalie);



			btnvalide.Click += Btnvalide_Click;
			btnanomalie.Click += Btnanomalie_Click;

		}

		protected override void OnResume()
		{
			base.OnResume();

			codelivraison.Gravity = GravityFlags.Center;
			infolivraison.Gravity = GravityFlags.Center;
			title.Gravity = GravityFlags.Center;
			infosupp.Gravity = GravityFlags.Center;
			infoclient.Gravity = GravityFlags.Center;

			//hide Btn Valide

			if (data.StatutLivraison == "1")
			{
				btnvalide.Visibility = ViewStates.Gone;
			}


			//TITLE
			if (data.typeMission == "L")
			{
				title.Text = "Livraison";
			}
			else {
				title.Text = "Enlèvement";
			}


			infosupp.Text = data.instrucLivraison;
			codelivraison.Text = data.numCommande;
			infolivraison.Text = data.nomPayeur + "\n" + data.adresseLivraison + "\n" + data.CpLivraison + " " + data.villeLivraison + "\n" + data.nbrColis + " COLIS   " + data.nbrPallette + " PALETTE\n" + data.poids + "\n" + data.dateHeure + "\n" + data.CR; ;
			infoclient.Text = "\n" + data.nomClient + "\nRef: " + data.refClient + "\nTournee : " + data.planDeTransport;
			client.Text = "Client";



			//Gestion dest final
			destfinal.Visibility = ViewStates.Gone;
			destfinal.Text = "" + data.nomClientLivraison + "\n" + data.villeClientLivraison + "";

			if (data.typeMission == "C")
			{
				destfinal.Visibility = ViewStates.Visible;
			}

			//Hide box anomalie if no anomalie
			anomalie.Visibility = ViewStates.Gone;
			anomaliet.Visibility = ViewStates.Gone;

			//COLOR
			switch (data.StatutLivraison)
			{
				case "1":
					title.SetBackgroundColor(Color.LightGreen);
					commande.SetBackgroundColor(Color.LightGreen);
					client.SetBackgroundColor(Color.LightGreen);
					_imageView.Visibility = ViewStates.Gone;

					//set IMG
					_imageView.Visibility = ViewStates.Visible;
					imgbitmap = data.imgpath.LoadAndResizeBitmap(500, 500);
					_imageView.SetImageBitmap(imgbitmap);
					break;
				case "2":
					title.SetBackgroundColor(Color.IndianRed);
					commande.SetBackgroundColor(Color.IndianRed);
					client.SetBackgroundColor(Color.IndianRed);
					anomaliet.SetBackgroundColor(Color.IndianRed);

					anomalie.Visibility = ViewStates.Visible;
					anomaliet.Visibility = ViewStates.Visible;
					anomalie.Text = data.codeAnomalie + "\n" + data.libeAnomalie + "\n" + data.remarque;

					//set IMG
					_imageView.Visibility = ViewStates.Visible;
					imgbitmap = data.imgpath.LoadAndResizeBitmap(500, 500);
					_imageView.SetImageBitmap(imgbitmap);
					break;
				default:
					title.SetBackgroundColor(Color.CadetBlue);
					commande.SetBackgroundColor(Color.CadetBlue);
					client.SetBackgroundColor(Color.CadetBlue);
					_imageView.Visibility = ViewStates.Gone;
					break;
			}
		}


		void Btnvalide_Click(object sender, EventArgs e)
		{
			Intent intent = new Intent(this, typeof(ValidationActivity));
			intent.PutExtra("ID", Convert.ToString(i));
			intent.PutExtra("TYPE", type);
			this.StartActivity(intent);
			Finish();
			if (imgbitmap != null)
			{
				imgbitmap.Recycle();
			}
		}

		void Btnanomalie_Click(object sender, EventArgs e)
		{
			Intent intent = new Intent(this, typeof(AnomalieActivity));
			intent.PutExtra("ID", Convert.ToString(i));
			intent.PutExtra("TYPE", type);
			this.StartActivity(intent);
			Finish();
			if (imgbitmap != null)
			{
				imgbitmap.Recycle();
			}
			//this.OverridePendingTransition (Android.Resource.Animation.SlideInLeft,Android.Resource.Animation.SlideOutRight);
		}
		public override bool OnTouchEvent(MotionEvent e)
		{
			_gestureDetector.OnTouchEvent(e);
			return false;
		}

		public bool OnDown(MotionEvent e)
		{
			return false;
		}

		public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			try
			{
				if (Math.Abs(e1.GetY() - e2.GetY()) > SWIPE_MAX_OFF_PATH)
				{
					return false;
				}
				//				// right to left swipe, dvs gå till höger
				if ((e1.GetX() - e2.GetX() > SWIPE_MIN_DISTANCE) && (Math.Abs(velocityX) > SWIPE_THRESHOLD_VELOCITY))
				{
					//Toast.MakeText (this, "Left Swipe", ToastLength.Short).Show ();
				}
				// left to right swipe, dvs gå till vänster
				if ((e2.GetX() - e1.GetX() > SWIPE_MIN_DISTANCE) && (Math.Abs(velocityX) > SWIPE_THRESHOLD_VELOCITY))
				{
					if (data.StatutLivraison == "1" || data.StatutLivraison == "2")
					{
						Intent intent = new Intent(this, typeof(ListeTraitee));
						intent.PutExtra("TYPE", type);
						this.StartActivity(intent);
						//this.OverridePendingTransition (Android.Resource.Animation.SlideInLeft,Android.Resource.Animation.SlideOutRight);
					}
					else {
						Intent intent = new Intent(this, typeof(ListeLivraisonsActivity));
						intent.PutExtra("TYPE", type);
						this.StartActivity(intent);
						//this.OverridePendingTransition (Android.Resource.Animation.SlideInLeft,Android.Resource.Animation.SlideOutRight);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Insights.Report(ex);
			}
			return true;
		}

		public void OnLongPress(MotionEvent e) { }

		public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
		{
			return false;
		}

		public void OnShowPress(MotionEvent e) { }

		public bool OnSingleTapUp(MotionEvent e)
		{
			return false;
		}

		public override void OnBackPressed()
		{
			if (data.StatutLivraison == "1" || data.StatutLivraison == "2")
			{
				Intent intent = new Intent(this, typeof(ListeTraitee));
				intent.PutExtra("TYPE", type);
				this.StartActivity(intent);
				if (imgbitmap != null)
				{
					imgbitmap.Recycle();
				}
				_imageView.Dispose();
				GC.Collect();
				_imageView.Dispose();
				codelivraison.Dispose();
				commande.Dispose();
				infolivraison.Dispose();
				title.Dispose();
				infosupp.Dispose();
				infoclient.Dispose();
				client.Dispose();
				anomaliet.Dispose();
				anomalie.Dispose();
				destfinal.Dispose();
				_imageView.Dispose();
				btnvalide.Dispose();

				Finish();
				//this.OverridePendingTransition (Android.Resource.Animation.SlideInLeft,Android.Resource.Animation.SlideOutRight);
			}
			else {
				Intent intent = new Intent(this, typeof(ListeLivraisonsActivity));
				intent.PutExtra("TYPE", type);
				this.StartActivity(intent);

				if (imgbitmap != null)
				{
					imgbitmap.Recycle();
				}
				_imageView.Dispose();
				codelivraison.Dispose();
				commande.Dispose();
				infolivraison.Dispose();
				title.Dispose();
				infosupp.Dispose();
				infoclient.Dispose();
				client.Dispose();
				anomaliet.Dispose();
				anomalie.Dispose();
				destfinal.Dispose();
				_imageView.Dispose();
				btnvalide.Dispose();
				GC.Collect();
				Finish();
				//this.OverridePendingTransition (Android.Resource.Animation.SlideInLeft,Android.Resource.Animation.SlideOutRight);
			}
		}
	}
}

