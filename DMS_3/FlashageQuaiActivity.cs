using System;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using ZXing.Mobile;
using Android.Views.InputMethods;

namespace DMS_3
{
	[Activity(Label = "",Theme = "@style/MyTheme.Base", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class FlashageQuaiActivity : Activity
	{
		EditText barcode;
		TextView infonumero;
		TextView infonomdest;
		TextView infocpdest;
		TextView infovilledest;
		TextView infoadrdest;
		TextView infonbcnbpP;
		TextView nbcolisflash;
		TextView zoneflash;
		TextView zonetheo;
		Button manuedit;
		Button btn_barcode;

		MobileBarcodeScanner scanner;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);


			SetContentView(Resource.Layout.FlashageQuai);

			//déclaration items
			barcode = FindViewById<EditText>(Resource.Id.barcode);
			btn_barcode = FindViewById<Button>(Resource.Id.btn_barcode);
			infonumero = FindViewById<TextView>(Resource.Id.infonumero);
			infonomdest = FindViewById<TextView>(Resource.Id.infonomdest);
			infocpdest = FindViewById<TextView>(Resource.Id.infocpdest);
			infovilledest = FindViewById<TextView>(Resource.Id.infovilledest);
			infoadrdest = FindViewById<TextView>(Resource.Id.infoadrdest);
			infonbcnbpP = FindViewById<TextView>(Resource.Id.infonbcnbpP);
			nbcolisflash = FindViewById<TextView>(Resource.Id.nbcolisflash);
			zoneflash = FindViewById<TextView>(Resource.Id.zoneflash);
			zonetheo = FindViewById<TextView>(Resource.Id.zonetheo);
			manuedit = FindViewById<Button>(Resource.Id.manuedit);

			//scan

			// Initialize the scanner first so we can track the current context
			MobileBarcodeScanner.Initialize(Application);

			//Create a new instance of our Scanner
			scanner = new MobileBarcodeScanner();

		}

		protected override void OnResume()
		{
			base.OnResume();
			barcode.RequestFocus();

			//barcode.Visibility = ViewStates.Invisible;
			infonumero.Visibility = ViewStates.Gone;
			infonomdest.Visibility = ViewStates.Gone;
			infocpdest.Visibility = ViewStates.Gone;
			infovilledest.Visibility = ViewStates.Gone;
			infoadrdest.Visibility = ViewStates.Gone;
			infonbcnbpP.Visibility = ViewStates.Gone;
			nbcolisflash.Visibility = ViewStates.Gone;
			zoneflash.Visibility = ViewStates.Gone;
			zonetheo.Visibility = ViewStates.Gone;

			barcode.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
			{
				if (e.Text.ToString() != String.Empty)
				{
					ShowProgress(progress => AndHUD.Shared.Show(this, "Chargement ... " + progress + "%", progress, MaskType.Clear), e.Text.ToString());
				}
				barcode.EditableText.Clear();
			};

			barcode.InputType = 0;
			InputMethodManager inputMethodManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
			barcode.Click += delegate
			{
				inputMethodManager.HideSoftInputFromWindow(barcode.WindowToken, 0);

			};

			btn_barcode.Click += delegate {
				//Start scanning
				scan();
			};

			manuedit.Click += delegate
			{
				Dialog dialog = new Dialog(this);
				dialog.Window.RequestFeature(WindowFeatures.NoTitle); 
				//dialog.Window.SetBackgroundDrawableResource(Resource.Drawable.bktransbox);
				dialog.SetContentView(Resource.Layout.BoxManuEdit);
				Button valid = dialog.FindViewById<Button>(Resource.Id.btn_valid);
				EditText barrecode = dialog.FindViewById<EditText>(Resource.Id.barrecode);

				valid.Click += delegate
				{
					ShowProgress(progress => AndHUD.Shared.Show(this, "Récupération ... " + progress + "%", progress, MaskType.Clear), barrecode.Text);
					dialog.Dismiss();
				};

				dialog.SetCancelable(true);
				dialog.Show();
			};

		}


		public override void OnBackPressed()
		{
			base.OnBackPressed();

			Intent intent = new Intent(this, typeof(HomeActivity));
			this.StartActivity(intent);
			Finish();
		}


		//public override bool OnCreatePanelMenu(int featureId, Android.Views.IMenu menu)
		//{
		//	var inflater = MenuInflater;
		//	inflater.Inflate(Resource.Menu.Flash, menu);
		//	return true;
		//}

		void HandleScanResult(ZXing.Result result)
		{

			if (result.Text != String.Empty)
			{
				ShowProgress(progress => AndHUD.Shared.Show(this, "Récupération ... " + progress + "%", progress, MaskType.Clear), result.Text);
			}
			barcode.EditableText.Clear();
		}

		public async Task scan()
		{
			var result = await scanner.Scan();

			HandleScanResult(result);
		}

		void ShowProgress(Action<int> action, string num)
		{

			Task.Factory.StartNew(() =>
			{
				try
				{
					int progress = 0;
					string dataWS;

					progress += 20;
					action(progress);

					string _url = "http://dms.jeantettransport.com/api/flash?val=" + num;
					//string _url = "http://10.1.2.70/mvcdms/api/flash?val=" + num;
					var webClient = new WebClient();
					webClient.Encoding = System.Text.Encoding.UTF8;
					webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
					dataWS = webClient.DownloadString(_url);

					progress += 50;
					action(progress);

					//.Replace("\\", "")).Replace("\"", "'")

					string json = @"" + dataWS + "";
					var bob = Newtonsoft.Json.Linq.JObject.Parse(json);
					if (bob["FLAOTSNUM"].ToString() != "")
					{
						RunOnUiThread(() => infonumero.Visibility = ViewStates.Visible);
						RunOnUiThread(() => infonumero.Text = (string)bob["FLAOTSNUM"]);

						if (bob["FLANOMDEST"].ToString() != "")
						{
							RunOnUiThread(() => infonomdest.Visibility = ViewStates.Visible);
							RunOnUiThread(() => infonomdest.Text = (string)bob["FLANOMDEST"]);
						}
						else
						{
							RunOnUiThread(() => infonomdest.Visibility = ViewStates.Gone);
						}

						if (bob["FLAADRDEST"].ToString() != "")
						{
							RunOnUiThread(() => infoadrdest.Visibility = ViewStates.Visible);
							RunOnUiThread(() => infoadrdest.Text = (string)bob["FLAADRDEST"]);
						}
						else
						{
							RunOnUiThread(() => infoadrdest.Visibility = ViewStates.Gone);
						}

						if (bob["FLACPDEST"].ToString() != "")
						{
							RunOnUiThread(() => infocpdest.Visibility = ViewStates.Visible);
							RunOnUiThread(() => infocpdest.Text = (string)bob["FLACPDEST"]);
						}
						else
						{
							RunOnUiThread(() => infocpdest.Visibility = ViewStates.Gone);
						}

						if (bob["FLAVILLEDEST"].ToString() != "")
						{
							RunOnUiThread(() => infovilledest.Visibility = ViewStates.Visible);
							RunOnUiThread(() => infovilledest.Text = (string)bob["FLAVILLEDEST"]);
						}
						else
						{
							RunOnUiThread(() => infovilledest.Visibility = ViewStates.Gone);
						}

						RunOnUiThread(() => infonbcnbpP.Visibility = ViewStates.Visible);
						RunOnUiThread(() => infonbcnbpP.Text = "NB COLIS: " + (string)bob[" "] + "NB PAL: " + (string)bob["FLAPAL"] + " POIDS: " + (string)bob["FLAPDS"]);


						RunOnUiThread(() => nbcolisflash.Visibility = ViewStates.Visible);
						RunOnUiThread(() => nbcolisflash.Text = "NB COLIS FLASHER: " + (string)bob["FLANBFLASHER"] + "/" + (string)bob["FLANBCOLIS"]);

						if (bob["FLAZONEFLASHER"].ToString() != "")
						{
							RunOnUiThread(() => zoneflash.Visibility = ViewStates.Visible);
							RunOnUiThread(() => zoneflash.Text = (string)bob["FLAZONEFLASHER"]);
						}
						else
						{
							RunOnUiThread(() => zoneflash.Visibility = ViewStates.Gone);
						}


						if (bob["FLAZONETHEORIQUE"].ToString() != null)
						{
							RunOnUiThread(() => zonetheo.Visibility = ViewStates.Visible);
							RunOnUiThread(() => zonetheo.Text = (string)bob["FLAZONETHEORIQUE"]);
						}
						else
						{
							RunOnUiThread(() => zonetheo.Visibility = ViewStates.Gone);
						}

						TableLayout tl = (TableLayout)FindViewById(Resource.Id.tableEvenement);


						RunOnUiThread(() => tl.RemoveAllViews());

						foreach (var item in bob["FLAEVE"])
						{
							TableRow row = new TableRow(this);
							TableLayout.LayoutParams layoutParams = new TableLayout.LayoutParams(TableLayout.LayoutParams.WrapContent, TableLayout.LayoutParams.WrapContent);

							//layoutParams.SetMargins(5, 5, 5, 5);
							row.LayoutParameters = layoutParams;
							row.SetGravity(Android.Views.GravityFlags.Center);
							TextView b = new TextView(this);
							b.Gravity = Android.Views.GravityFlags.Center;
							b.Text = (string)item["EVEDATE"] + " " + (string)item["EVECODE"] + " " + (string)item["EVEOTEVAL1"];
							row.AddView(b);
							RunOnUiThread(() => tl.AddView(row));
						}
					}
					else {
						RunOnUiThread(() => infonumero.Text = "Pas de résultat");
						RunOnUiThread(() => infonomdest.Visibility = ViewStates.Gone);
						RunOnUiThread(() => infocpdest.Visibility = ViewStates.Gone);
						RunOnUiThread(() => infovilledest.Visibility = ViewStates.Gone);
						RunOnUiThread(() => infoadrdest.Visibility = ViewStates.Gone);
						RunOnUiThread(() => infonbcnbpP.Visibility = ViewStates.Gone);
						RunOnUiThread(() => nbcolisflash.Visibility = ViewStates.Gone);
						RunOnUiThread(() => zoneflash.Visibility = ViewStates.Gone);
						RunOnUiThread(() => zonetheo.Visibility = ViewStates.Gone);
					}

					progress += 30;
					action(progress);

					AndHUD.Shared.Dismiss(this);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			});
		}
	}

}

