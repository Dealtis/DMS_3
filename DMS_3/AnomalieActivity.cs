using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Widget;
using DMS_3.BDD;
using Android.Support.V7.App;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Uri = Android.Net.Uri;
using RaygunClient = Mindscape.Raygun4Net.RaygunClient;
namespace DMS_3
{
	[Activity(Label = "AnomalieActivity", Theme = "@style/MyTheme.Base", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class AnomalieActivity : AppCompatActivity
	{
		string id;
		int i;
		string txtspinner;
		string codeanomalie;
		EditText editText;
		EditText colis;
		EditText palette;
		EditText poids;
		EditText ML;
		TextView txtcolis;
		TextView txtpalette;
		TextView txttonnes;

		string txtRem;
		ImageView _imageView;
		TablePositions data;
		CheckBox checkP;
		string type;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Anomalie);

			id = Intent.GetStringExtra("ID");
			i = int.Parse(id);

			type = Intent.GetStringExtra("TYPE");

			DBRepository dbr = new DBRepository();
			data = dbr.GetPositionsData(i);

			Spinner spinner = FindViewById<Spinner>(Resource.Id.spinnerAnomalie);
			editText = FindViewById<EditText>(Resource.Id.edittext);
			colis = FindViewById<EditText>(Resource.Id.colis);
			palette = FindViewById<EditText>(Resource.Id.palette);
			poids = FindViewById<EditText>(Resource.Id.poids);
			ML = FindViewById<EditText>(Resource.Id.ML);
			_imageView = FindViewById<ImageView>(Resource.Id.imageView1);
			checkP = FindViewById<CheckBox>(Resource.Id.checkBoxPartic);

			txtcolis = FindViewById<TextView>(Resource.Id.TxtColis);
			txtpalette = FindViewById<TextView>(Resource.Id.TxtPalette);
			txttonnes = FindViewById<TextView>(Resource.Id.txtTonnes);


			Button buttonvalider = FindViewById<Button>(Resource.Id.valider);

			if (IsThereAnAppToTakePictures())
			{
				Data.Instance.CreateDirectoryForPictures();
				Button buttonphoto = FindViewById<Button>(Resource.Id.openCamera);
				_imageView = FindViewById<ImageView>(Resource.Id.imageView1);
				buttonphoto.Click += TakeAPicture;
			}

			buttonvalider.Click += delegate
			{
				Buttonvalider_Click();
			};

			spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
			ArrayAdapter adapter;
			if (type == "RAM")
			{
				adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.anomalieramasselist, Android.Resource.Layout.SimpleSpinnerItem);
			}
			else {
				adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.anomalielivraisonlist, Android.Resource.Layout.SimpleSpinnerItem);
			}

			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = adapter;
		}

		void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;
			txtspinner = string.Format("{0}", spinner.GetItemAtPosition(e.Position));

			switch (txtspinner)
			{
				case "Ramasse non conforme":
					colis.Visibility = Android.Views.ViewStates.Visible;
					colis.Text = data.nbrColis;
					palette.Visibility = Android.Views.ViewStates.Visible;
					palette.Text = data.nbrPallette;
					poids.Visibility = Android.Views.ViewStates.Visible;
					poids.Text = data.poids;
					ML.Visibility = Android.Views.ViewStates.Visible;
					ML.Hint = "Mètre lineaire";

					txtcolis.Visibility = Android.Views.ViewStates.Visible;
					txtpalette.Visibility = Android.Views.ViewStates.Visible;
					txttonnes.Visibility = Android.Views.ViewStates.Visible;
					break;
				case "Restaure en non traite":
					editText.Visibility = Android.Views.ViewStates.Gone;
					colis.Visibility = Android.Views.ViewStates.Gone;
					palette.Visibility = Android.Views.ViewStates.Gone;
					poids.Visibility = Android.Views.ViewStates.Gone;
					ML.Visibility = Android.Views.ViewStates.Gone;
					txtcolis.Visibility = Android.Views.ViewStates.Gone;
					txtpalette.Visibility = Android.Views.ViewStates.Gone;
					txttonnes.Visibility = Android.Views.ViewStates.Gone;
					break;
				case "Choisir une anomalie":
					editText.Visibility = Android.Views.ViewStates.Gone;
					colis.Visibility = Android.Views.ViewStates.Gone;
					palette.Visibility = Android.Views.ViewStates.Gone;
					poids.Visibility = Android.Views.ViewStates.Gone;
					ML.Visibility = Android.Views.ViewStates.Gone;
					txtcolis.Visibility = Android.Views.ViewStates.Gone;
					txtpalette.Visibility = Android.Views.ViewStates.Gone;
					txttonnes.Visibility = Android.Views.ViewStates.Gone;
					break;
				default:
					editText.Visibility = Android.Views.ViewStates.Visible;
					colis.Visibility = Android.Views.ViewStates.Gone;
					palette.Visibility = Android.Views.ViewStates.Gone;
					poids.Visibility = Android.Views.ViewStates.Gone;
					ML.Visibility = Android.Views.ViewStates.Gone;
					txtcolis.Visibility = Android.Views.ViewStates.Gone;
					txtpalette.Visibility = Android.Views.ViewStates.Gone;
					txttonnes.Visibility = Android.Views.ViewStates.Gone;
					break;
			}
			if (txtspinner == "Restaure en non traite" || txtspinner == "Choisir une anomalie")
			{
				editText.Visibility = Android.Views.ViewStates.Gone;
			}
			else {
				editText.Visibility = Android.Views.ViewStates.Visible;
			}
		}

		void Buttonvalider_Click()
		{
			if (txtspinner != "Choisir une anomalie")
			{
				txtRem = editText.Text;
				switch (txtspinner)
				{
					case "Livre avec manquant":
						codeanomalie = "LIVMQP";
						break;
					case "Livre avec reserves pour avaries":
						codeanomalie = "LIVRCA";
						break;
					case "Livre mais recepisse non rendu":
						codeanomalie = "LIVDOC";
						break;
					case "Livre avec manquants + avaries":
						codeanomalie = "LIVRMA";
						break;
					case "Refuse pour avaries":
						codeanomalie = "RENAVA";
						break;
					case "Avise (avis de passage)":
						codeanomalie = "RENAVI";
						break;
					case "Rendu non livre : complement adresse":
						codeanomalie = "RENCAD";
						break;
					case "Refus divers ou sans motifs":
						codeanomalie = "RENDIV";
						break;
					case "Refuse manque BL":
						codeanomalie = "RENDOC";
						break;
					case "Refuse manquant partiel":
						codeanomalie = "RENMQP";
						break;
					case "Refuse non commande":
						codeanomalie = "RENDIV";
						break;
					case "Refuse cause port du":
						codeanomalie = "RENSPD";
						break;
					case "Refuse cause contre remboursement":
						codeanomalie = "RENSRB";
						break;
					case "Refuse livraison trop tardive":
						codeanomalie = "RENTAR";
						break;
					case "Rendu non justifie":
						codeanomalie = "RENNJU";
						break;
					case "Fermeture hebdomadaire":
						codeanomalie = "RENFHB";
						break;
					case "Non charge":
						codeanomalie = "RENNCG";
						break;
					case "Inventaire":
						codeanomalie = "RENFCO";
						break;
					case "Ramasse pas faite":
						codeanomalie = "ENEDIV";
						break;
					case "Positions non chargees":
						codeanomalie = "RENNCG";
						break;
					case "Avis de passage":
						codeanomalie = "ENEAVI";
						break;
					case "Ramasse diverse":
						codeanomalie = "ENENJU";
						break;
					case "Ramasse non conforme":
						codeanomalie = "ECHNCF";
						break;
					case "Restaure en non traite":
						codeanomalie = "RESTNT";
						break;
					default:
						break;
				}

				//format mémo
				string formatrem = "";
				if (codeanomalie != "ECHNCF")
				{
					formatrem = txtRem.Replace("\"", " ").Replace("'", " ");
				}
				else
				{
					if (txtRem != "")
					{
						formatrem += "memo : " + txtRem.Replace("\"", " ").Replace("'", " ") + ", ";
					}
					if (data.nbrColis != colis.Text)
					{
						formatrem += "colis : " + colis.Text + "| ";
					}
					if (data.nbrPallette != palette.Text)
					{
						formatrem += "palette : " + palette.Text + "| ";
					}
					if (data.poids != poids.Text)
					{
						formatrem += "poids : " + poids.Text + "| ";
					}
					if (ML.Text != "")
					{
						formatrem += "ML : " + ML.Text + "| ";
					}
				}


				DBRepository dbr = new DBRepository();
				switch (txtspinner)
				{
					case "Restaure en non traite":

						dbr.updatePosition(i, "0", txtspinner, formatrem, codeanomalie, null);
						//if postion pole vider les colis flasher
						if (data.positionPole != "0")
						{
							dbr.resetColis(data.numCommande);
						}
						break;
					default:
						dbr.updatePosition(i, "2", txtspinner, formatrem, codeanomalie, null);
						break;
				}



				//creation du JSON
				string JSON = "{\"codesuiviliv\":\"" + codeanomalie + "\",\"memosuiviliv\":\"" + formatrem + "\",\"libellesuiviliv\":\"" + txtspinner + "\",\"commandesuiviliv\":\"" + data.numCommande + "\",\"groupagesuiviliv\":\"" + data.groupage + "\",\"datesuiviliv\":\"" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\",\"posgps\":\"" + Data.GPS + "\"}";
				//création de la notification webservice // statut de position
				dbr.insertDataStatutpositions(codeanomalie, "2", txtspinner, data.numCommande, formatrem, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), JSON);

				if (checkP.Checked)
				{
					var typecr = "PARTIC";
					string JSONPARTIC = "{\"codesuiviliv\":\"" + typecr + "\",\"memosuiviliv\":\"particulier\",\"libellesuiviliv\":\"\",\"commandesuiviliv\":\"" + data.numCommande + "\",\"groupagesuiviliv\":\"" + data.groupage + "\",\"datesuiviliv\":\"" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\",\"posgps\":\"" + Data.GPS + "\"}";
					dbr.insertDataStatutpositions(typecr, "2", typecr, data.numCommande, formatrem, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), JSONPARTIC);
				}

				if (txtspinner != "Restaure en non traite")
				{
					Data.Instance.traitImg(i, type, this);
				}

				dbr.SETBadges(Data.userAndsoft);

				bool sign = dbr.is_user_Sign(Data.userAndsoft);
				if (sign)
				{
					Intent intent = new Intent();
					switch (codeanomalie)
					{
						case "RENAVI":
							intent = new Intent(this, typeof(SignatureActivity));
							break;
						case "RENCAD":
							intent = new Intent(this, typeof(SignatureActivity));
							break;
						case "RENFHB":
							intent = new Intent(this, typeof(SignatureActivity));
							break;
						case "RENNCG":
							intent = new Intent(this, typeof(SignatureActivity));
							break;
						case "RENFCO":
							intent = new Intent(this, typeof(SignatureActivity));
							break;
						case "ENEDIV":
							intent = new Intent(this, typeof(SignatureActivity));
							break;
						case "ENEAVI":
							intent = new Intent(this, typeof(SignatureActivity));
							break;
						default:
							intent = new Intent(this, typeof(ListeLivraisonsActivity));
							break;
					}
					intent.PutExtra("TYPE", type);
					intent.PutExtra("TRAIT", "false");
					intent.PutExtra("NUM", data.numCommande);
					this.StartActivity(intent);
				}
				else
				{
					Intent intent = new Intent(this, typeof(ListeLivraisonsActivity));
					intent.PutExtra("TYPE", type);
					intent.PutExtra("TRAIT", "false");
					this.StartActivity(intent);
				}
			}
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			// Make it available in the gallery
			Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
			Uri contentUri = Uri.FromFile(Data._file);
			mediaScanIntent.SetData(contentUri);
			SendBroadcast(mediaScanIntent);

			int height = Resources.DisplayMetrics.HeightPixels;
			int width = _imageView.Height;
			Data.bitmap = Data._file.Path.LoadAndResizeBitmap(width, height);
			if (Data.bitmap != null)
			{
				_imageView.SetImageBitmap(Data.bitmap);

				DBRepository dbr = new DBRepository();
				dbr.updateposimgpath(i, Data._file.Path);
				Data.bitmap = null;
			}
			GC.Collect();
		}

		public void TakeAPicture(object sender, EventArgs eventArgs)
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			Data._file = new Java.IO.File(Data._dir, String.Format("" + DateTime.Now.ToString("ddMM") + "_" + data.numCommande + ".jpg", Guid.NewGuid()));
			intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(Data._file));
			StartActivityForResult(intent, 0);
		}

		private bool IsThereAnAppToTakePictures()
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			IList<ResolveInfo> availableActivities =
				PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
			return availableActivities != null && availableActivities.Count > 0;
		}

		public override void OnBackPressed()
		{
			if (Intent.GetBooleanExtra("FLASH", false))
			{

				DBRepository dbr = new DBRepository();
				dbr.resetColis(data.numCommande);
				Intent intent;
				if (Intent.GetBooleanExtra("noColis", false))
				{
					intent = new Intent(this, typeof(DetailActivity));
					intent.PutExtra("TRAIT", "false");
				}
				else
				{
					intent = new Intent(this, typeof(FlashageQuaiActivity));
				}
				intent.PutExtra("ID", Convert.ToString(i));
				intent.PutExtra("NUMCOM", data.numCommande);
				intent.PutExtra("TYPE", type);
				intent.PutExtra("ACTION", Intent.GetStringExtra("ACTION"));
				this.StartActivity(intent);
			}
			else
			{
				AlertDialog.Builder builder = new AlertDialog.Builder(this);
				builder.SetTitle("Annulation");
				builder.SetMessage("Voulez-vous annulée l'anomalie ?");
				builder.SetCancelable(false);
				builder.SetPositiveButton("Oui", delegate
				{
					Intent intent = new Intent(this, typeof(DetailActivity));
					intent.PutExtra("ID", Convert.ToString(i));
					intent.PutExtra("TYPE", type);
					intent.PutExtra("TRAIT", "false");
					this.StartActivity(intent);
					Finish();
					_imageView.Dispose();
				});
				builder.SetNegativeButton("Non", delegate { });
				builder.Show();
			}

		}
	}
}