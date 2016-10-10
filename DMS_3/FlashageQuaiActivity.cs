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
using Android.Provider;
using System.Collections.Generic;
using Android.Content.PM;
using Uri = Android.Net.Uri;
using Android.Graphics;
using DMS_3.BDD;
using System.Threading;
using System.IO;

namespace DMS_3
{
	[Activity(Label = "", Theme = "@style/MyTheme.Base", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
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
		Button btn_photo;
		Button btn_detail;
		Button btn_valider;
		Button btn_anomalie;
		Button btn_termine;
		ImageView _imageView;
		ToggleButton tbtnTorch;

		TablePositions data;

		string numero;
		string id;
		string numCommande;
		string actionP;
		string type;

		bool flashinprogress;


		MobileBarcodeScanner scanner;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			MobileBarcodeScanner.Initialize(Application);

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
			btn_termine = FindViewById<Button>(Resource.Id.btn_termine);

			btn_detail = FindViewById<Button>(Resource.Id.btn_detail);
			btn_valider = FindViewById<Button>(Resource.Id.btn_valider);
			btn_anomalie = FindViewById<Button>(Resource.Id.btn_anomalie);



			if (IsThereAnAppToTakePictures())
			{
				CreateDirectoryForPictures();
				btn_photo = FindViewById<Button>(Resource.Id.btn_photo);
				_imageView = FindViewById<ImageView>(Resource.Id._imageView);
				btn_photo.Click += TakeAPicture;
			}

			btn_detail.Visibility = ViewStates.Gone;
			btn_valider.Visibility = ViewStates.Gone;
			btn_photo.Visibility = ViewStates.Gone;
			btn_anomalie.Visibility = ViewStates.Gone;
			btn_termine.Visibility = ViewStates.Gone;

			//_imageView.Visibility = ViewStates.Gone;


			btn_termine.Click += delegate
			{
				flashinprogress = false;

				Intent intent;
				if (actionP == "VALID")
				{
					intent = new Intent(this, typeof(ValidationActivity));
				}
				else
				{
					intent = new Intent(this, typeof(AnomalieActivity));
				}

				intent.PutExtra("ID", id);
				intent.PutExtra("TYPE", type);
				this.StartActivity(intent);
			};

			//scan
			//instance overlay
			var zxingOverlay = LayoutInflater.FromContext(this).Inflate(Resource.Layout.overlay, null);

			//Create a new instance of our Scanner
			scanner = new MobileBarcodeScanner();



			btn_barcode.Click += async delegate
			{

				scanner.UseCustomOverlay = true;

				var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
				options.AutoRotate = false;

				tbtnTorch = zxingOverlay.FindViewById<ToggleButton>(Resource.Id.tbtnTorch);

				tbtnTorch.Click += delegate
				{
					scanner.ToggleTorch();
				};

				scanner.CustomOverlay = zxingOverlay;

				var result = await scanner.Scan(this, options);



				HandleScanResult(result);
			};
		}

		protected override void OnResume()
		{
			base.OnResume();
			barcode.RequestFocus();

			if (numero != null)
			{
				btn_photo.Visibility = ViewStates.Visible;
			}

			id = Intent.GetStringExtra("ID");
			numCommande = Intent.GetStringExtra("NUMCOM");
			type = Intent.GetStringExtra("TYPE");
			actionP = Intent.GetStringExtra("ACTION");
			if (numCommande != null)
			{
				flashinprogress = true;
				GetInfoNumcommande(progress => AndHUD.Shared.Show(this, "Chargement ... " + progress + "%", progress, MaskType.Clear), numCommande);
			}
			else
			{
				flashinprogress = false;
			}

			barcode.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
			{
				if (e.Text.ToString() != String.Empty)
				{
					numero = e.Text.ToString();
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
					numero = barrecode.Text;
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

			Intent intent;
			if (actionP != null)
			{
				intent = new Intent(this, typeof(DetailActivity));
				intent.PutExtra("ID", Convert.ToString(data.Id));
				intent.PutExtra("TYPE", data.typeSegment);
				this.StartActivity(intent);
				Finish();
			}
			else
			{
				intent = new Intent(this, typeof(HomeActivity));
				this.StartActivity(intent);
				Finish();
			}

		}

		void HandleScanResult(ZXing.Result result)
		{
			if (tbtnTorch.Checked)
			{
				tbtnTorch.Toggle();
			}
			if (result != null && !string.IsNullOrEmpty(result.Text))
			{
				numero = result.Text;
				ShowProgress(progress => AndHUD.Shared.Show(this, "Récupération ... " + progress + "%", progress, MaskType.Clear), result.Text);
				barcode.EditableText.Clear();
			}
			else {
				this.RunOnUiThread(() => Toast.MakeText(this, "Annuler", ToastLength.Short).Show());
			}


		}

		void ShowProgress(Action<int> action, string num)
		{
			Data.bitmap = null;
			_imageView.SetImageBitmap(null);

			Task.Factory.StartNew(() =>
			{
				try
				{
					int progress = 0;
					string dataWS;

					progress += 20;
					action(progress);


					DBRepository dbr = new DBRepository();


					//check is_colis_in_truck
					if (num.IndexOf("POLE") != -1)
					{
						var numSplit = num.Split('.');
						num = numSplit[1].Remove(numSplit[1].Length - 1);
					}

					var is_colis_in_truck = dbr.is_colis_in_truck(num);
					if (is_colis_in_truck != int.MinValue)
					{
						data = dbr.GetPositionsData(is_colis_in_truck);

						if (dbr.is_colis_in_currentPos(num, numCommande))
						{

							dbr.updateColisFlash(num);
							string JSONNOTIF = "{\"codesuiviliv\":\"FLASHAGE\",\"memosuiviliv\":\"" + num + "\",\"libellesuiviliv\":\"\",\"commandesuiviliv\":\"" + data.numCommande + "\",\"groupagesuiviliv\":\"" + data.groupage + "\",\"datesuiviliv\":\"" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\",\"posgps\":\"" + Data.GPS + "\"}";
							dbr.insertDataStatutpositions("FLASHAGE", "1", "FLASHAGE", data.numCommande, num, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), JSONNOTIF);

							RunOnUiThread(() => btn_detail.Click += delegate
								{
									Intent intent = new Intent(this, typeof(DetailActivity));
									intent.PutExtra("ID", Convert.ToString(data.Id));
									intent.PutExtra("TYPE", data.typeSegment);
									this.StartActivity(intent);
									Finish();
								});

							RunOnUiThread(() => btn_detail.Visibility = ViewStates.Gone);

							RunOnUiThread(() => infovilledest.Visibility = ViewStates.Visible);
							RunOnUiThread(() => btn_photo.Visibility = ViewStates.Visible);

							//get info de la tablePositions

							RunOnUiThread(() => infonomdest.Visibility = ViewStates.Visible);
							RunOnUiThread(() => infocpdest.Visibility = ViewStates.Visible);
							RunOnUiThread(() => infonbcnbpP.Visibility = ViewStates.Visible);
							RunOnUiThread(() => nbcolisflash.Visibility = ViewStates.Visible);
							RunOnUiThread(() => infoadrdest.Visibility = ViewStates.Visible);
							RunOnUiThread(() => infonumero.Visibility = ViewStates.Visible);
							RunOnUiThread(() => nbcolisflash.Visibility = ViewStates.Visible);

							RunOnUiThread(() => infonomdest.Text = data.nomClientLivraison);
							RunOnUiThread(() => infocpdest.Text = data.CpLivraison);
							RunOnUiThread(() => infovilledest.Text = data.villeLivraison);
							RunOnUiThread(() => infoadrdest.Text = data.adresseLivraison);
							RunOnUiThread(() => infonumero.Text = data.numCommande);

							int colisFlasher = dbr.CountColisFlash(data.numCommande);

							int colisPosition = dbr.CountColis(data.numCommande);


							RunOnUiThread(() => nbcolisflash.Text = "NB COLIS FLASHES: " + colisFlasher + "/" + colisPosition);

							TableLayout tl = (TableLayout)FindViewById(Resource.Id.tableEvenement);
							RunOnUiThread(() => tl.Visibility = ViewStates.Gone);

							if (action == null)
							{
								RunOnUiThread(() => btn_termine.Visibility = ViewStates.Gone);
							}
							else
							{
								RunOnUiThread(() => btn_termine.Visibility = ViewStates.Visible);
							}
							if (colisFlasher == colisPosition)
							{
								if (action == null)
								{
									RunOnUiThread(() => btn_termine.Visibility = ViewStates.Gone);
									//afficher les btn valider et anomalie
								}
								else
								{
									flashinprogress = false;

									Intent intent;
									if (actionP == "VALID")
									{
										intent = new Intent(this, typeof(ValidationActivity));
									}
									else
									{
										intent = new Intent(this, typeof(AnomalieActivity));
									}

									intent.PutExtra("ID", id);
									intent.PutExtra("TYPE", type);
									this.StartActivity(intent);
								}
							}

						}
						else
						{
							if (actionP == null)
							{
								afficherInformations(is_colis_in_truck, numCommande);
							}
							else
							{
								RunOnUiThread(() => Toast.MakeText(this, "Attention mauvais colis !", ToastLength.Long).Show());
							}
						}
					}
					else {
						var is_pos_in_truck = dbr.is_position_in_truck(num);
						if (is_pos_in_truck == int.MinValue)
						{
							if (!flashinprogress)
							{
								RunOnUiThread(() => btn_detail.Visibility = ViewStates.Gone);


								//get infos  WS
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
									RunOnUiThread(() => infonbcnbpP.Text = "NB COLIS: " + (string)bob["FLANBCOLIS"] + "NB PAL: " + (string)bob["FLAPAL"] + " POIDS: " + (string)bob["FLAPDS"]);


									RunOnUiThread(() => nbcolisflash.Visibility = ViewStates.Visible);
									RunOnUiThread(() => nbcolisflash.Text = "NB COLIS FLASHEE: " + (string)bob["FLANBFLASHER"] + "/" + (string)bob["FLANBCOLIS"]);

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
									RunOnUiThread(() => btn_photo.Visibility = ViewStates.Visible);
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
							}
							else
							{
								//Show an error image with a message with a Dimmed background, and auto-dismiss after 2 seconds
								//AndHUD.Shared.ShowError(this, "Attention mauvais colis !", MaskType.Black, TimeSpan.FromSeconds(2));


								RunOnUiThread(() => Toast.MakeText(this, "Attention mauvais colis !", ToastLength.Long).Show());
							}
						}
						else
						{
							afficherInformations(is_pos_in_truck, numCommande);
						}
					}


					progress += 30;
					action(progress);

					AndHUD.Shared.Dismiss(this);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					AndHUD.Shared.Dismiss(this);
				}
			});
		}

		void GetInfoNumcommande(Action<int> action, string num)
		{
			Data.bitmap = null;
			_imageView.SetImageBitmap(null);
			Task.Factory.StartNew(() =>
			{
				try
				{
					int progress = 0;
					string dataWS;

					progress += 20;
					action(progress);

					DBRepository dbr = new DBRepository();

					//check is_colis_in_truck
					var is_colis_in_truck = dbr.is_position_in_truck(num);
					if (is_colis_in_truck != int.MinValue)
					{
						flashinprogress = true;
						afficherInformations(is_colis_in_truck, numCommande);
					}
					else
					{
						if (!flashinprogress)
						{
							//RunOnUiThread(() => btn_valider.Visibility = ViewStates.Gone);
							//RunOnUiThread(() => btn_anomalie.Visibility = ViewStates.Gone);
							RunOnUiThread(() => btn_detail.Visibility = ViewStates.Gone);


							//get infos  WS
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
								RunOnUiThread(() => infonbcnbpP.Text = "NB COLIS: " + (string)bob["FLANBCOLIS"] + "NB PAL: " + (string)bob["FLAPAL"] + " POIDS: " + (string)bob["FLAPDS"]);


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
								RunOnUiThread(() => btn_photo.Visibility = ViewStates.Visible);


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
						}
						else
						{
							RunOnUiThread(() => Toast.MakeText(this, "Attention mauvais colis !", ToastLength.Long).Show());
						}

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

		void afficherInformations(int idPos, string numCommande)
		{
			DBRepository dbr = new DBRepository();
			data = dbr.GetPositionsData(idPos);

			RunOnUiThread(() => btn_detail.Click += delegate
				{
					Intent intent = new Intent(this, typeof(DetailActivity));
					intent.PutExtra("ID", Convert.ToString(data.Id));
					intent.PutExtra("TYPE", data.typeSegment);
					this.StartActivity(intent);
					Finish();
				});
			if (numCommande == null)
			{
				RunOnUiThread(() => btn_detail.Visibility = ViewStates.Visible);
			}

			RunOnUiThread(() => infovilledest.Visibility = ViewStates.Visible);
			RunOnUiThread(() => btn_photo.Visibility = ViewStates.Visible);

			//get info de la tablePositions

			RunOnUiThread(() => infonomdest.Visibility = ViewStates.Visible);
			RunOnUiThread(() => infocpdest.Visibility = ViewStates.Visible);
			RunOnUiThread(() => infonbcnbpP.Visibility = ViewStates.Visible);
			RunOnUiThread(() => nbcolisflash.Visibility = ViewStates.Visible);
			RunOnUiThread(() => infoadrdest.Visibility = ViewStates.Visible);
			RunOnUiThread(() => infonumero.Visibility = ViewStates.Visible);
			RunOnUiThread(() => nbcolisflash.Visibility = ViewStates.Visible);

			RunOnUiThread(() => infonomdest.Text = data.nomClientLivraison);
			RunOnUiThread(() => infocpdest.Text = data.CpLivraison);
			RunOnUiThread(() => infovilledest.Text = data.villeLivraison);
			RunOnUiThread(() => infoadrdest.Text = data.adresseLivraison);
			RunOnUiThread(() => infonumero.Text = data.numCommande);

			int colisFlasher = dbr.CountColisFlash(data.numCommande);

			int colisPosition = dbr.CountColis(data.numCommande);


			RunOnUiThread(() => nbcolisflash.Text = "NB COLIS FLASHES: " + colisFlasher + "/" + colisPosition);

			TableLayout tl = (TableLayout)FindViewById(Resource.Id.tableEvenement);
			RunOnUiThread(() => tl.Visibility = ViewStates.Gone);

			if (actionP == null)
			{
				RunOnUiThread(() => btn_termine.Visibility = ViewStates.Gone);
			}
			else
			{
				RunOnUiThread(() => btn_termine.Visibility = ViewStates.Visible);
			}

			if (colisFlasher == colisPosition)
			{
				if (actionP == null)
				{
					RunOnUiThread(() => btn_termine.Visibility = ViewStates.Gone);
					//afficher les btn valider et anomalie
				}
				else
				{
					flashinprogress = false;

					Intent intent;
					if (actionP == "VALID")
					{
						intent = new Intent(this, typeof(ValidationActivity));
					}
					else
					{
						intent = new Intent(this, typeof(AnomalieActivity));
					}

					intent.PutExtra("ID", id);
					intent.PutExtra("TYPE", type);
					this.StartActivity(intent);
				}


			}
		}

		private void CreateDirectoryForPictures()
		{
			Data._dir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(
					Android.OS.Environment.DirectoryPictures), "DMSIMG");
			if (!Data._dir.Exists())
			{
				Data._dir.Mkdirs();
			}
		}

		private bool IsThereAnAppToTakePictures()
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			IList<ResolveInfo> availableActivities =
				PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
			return availableActivities != null && availableActivities.Count > 0;
		}

		private void TakeAPicture(object sender, EventArgs eventArgs)
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			Data._file = new Java.IO.File(Data._dir, String.Format("" + DateTime.Now.ToString("ddMM") + "_" + numero + ".jpg", Guid.NewGuid()));
			intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(Data._file));
			StartActivityForResult(intent, 0);
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			// Make it available in the gallery
			if (resultCode == Result.Ok)
			{
				Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
				Uri contentUri = Uri.FromFile(Data._file);
				mediaScanIntent.SetData(contentUri);
				SendBroadcast(mediaScanIntent);

				// Display in ImageView. We will resize the bitmap to fit the display.
				// Loading the full sized image will consume to much memory
				// and cause the application to crash.

				int height = Resources.DisplayMetrics.HeightPixels;
				int width = _imageView.Height;

				Thread threadUpload = new Thread(() =>
					{
						try
						{
							Android.Graphics.Bitmap bmp = DecodeSmallFile(Data._file.Path, 1000, 1000);
							Bitmap rbmp = Bitmap.CreateScaledBitmap(bmp, bmp.Width / 2, bmp.Height / 2, true);
							string compImg = Data._file.Path.Replace(".jpg", "-1_1.jpg");
							using (var fs = new FileStream(compImg, FileMode.OpenOrCreate))
							{
								rbmp.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, fs);
							}
							//ftp://77.158.93.75 ftp://10.1.2.75
							Data.Instance.UploadFile("ftp://77.158.93.75", compImg, "DMS", "Linuxr00tn", "");
							bmp.Recycle();
							rbmp.Recycle();
						}
						catch (Exception ex)
						{
							Console.WriteLine("\n" + ex);
						}
					});
				threadUpload.Start();

				_imageView.Visibility = Android.Views.ViewStates.Visible;
				Data.bitmap = Data._file.Path.LoadAndResizeBitmap(width, height);
				if (Data.bitmap != null)
				{
					_imageView.SetImageBitmap(Data.bitmap);
				}
				else
				{

					_imageView.Visibility = Android.Views.ViewStates.Gone;
				}
				GC.Collect();
			}
			else {

			}

		}
		private Bitmap DecodeSmallFile(String filename, int width, int height)
		{
			var options = new BitmapFactory.Options { InJustDecodeBounds = true };
			BitmapFactory.DecodeFile(filename, options);
			options.InSampleSize = CalculateInSampleSize(options, width, height);
			options.InJustDecodeBounds = false;
			return BitmapFactory.DecodeFile(filename, options);
		}

		public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
		{
			int height = options.OutHeight;
			int width = options.OutWidth;
			int inSampleSize = 1;

			if (height > reqHeight || width > reqWidth)
			{
				var heightRatio = (int)Math.Round(height / (double)reqHeight);
				var widthRatio = (int)Math.Round(width / (double)reqWidth);
				inSampleSize = heightRatio < widthRatio ? heightRatio : widthRatio;
			}
			return inSampleSize;
		}
	}

}

