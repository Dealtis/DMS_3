using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using DMS_3.BDD;
using Uri = Android.Net.Uri;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace DMS_3
{
	[Activity(Label = "ValidationActivity", Theme = "@style/MyTheme.Base", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class ValidationActivity : AppCompatActivity
	{
		//RECUP ID 
		string id;
		int i;
		string type;
		string tyValide;

		TablePositions data;
		RadioButton check1;
		RadioButton check2;
		CheckBox checkP;
		TextView txtCR;
		EditText mémo;
		ImageView _imageView;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.valideDialBox);

			check1 = FindViewById<RadioButton>(Resource.Id.radioButton1);
			check2 = FindViewById<RadioButton>(Resource.Id.radioButton2);
			checkP = FindViewById<CheckBox>(Resource.Id.checkBox1);
			txtCR = FindViewById<TextView>(Resource.Id.textcr);
			mémo = FindViewById<EditText>(Resource.Id.edittext);
			_imageView = FindViewById<ImageView>(Resource.Id.imageView1);

			if (IsThereAnAppToTakePictures())
			{
				Data.Instance.CreateDirectoryForPictures();
				Button buttonphoto = FindViewById<Button>(Resource.Id.openCamera);
				_imageView = FindViewById<ImageView>(Resource.Id.imageView1);
				buttonphoto.Click += TakeAPicture;
			}

			id = Intent.GetStringExtra("ID");
			i = int.Parse(id);

			type = Intent.GetStringExtra("TYPE");
			if (type == "RAM")
			{
				tyValide = "ECHCFM";
			}
			else {
				tyValide = "LIVCFM";
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
			data = DBRepository.Instance.GetPositionsData(i);

			if (data.CR == "" || data.CR == "0" || type == "RAM" || data.ASSIGNE == "" || data.ASSIGNE == "0")
			{
				check1.Visibility = ViewStates.Gone;
				check2.Visibility = ViewStates.Gone;
				txtCR.Visibility = ViewStates.Gone;
			}
			else {
				check1.Visibility = ViewStates.Visible;
				check2.Visibility = ViewStates.Visible;
				txtCR.Visibility = ViewStates.Visible;
				txtCR.Text = data.CR + "" + data.ASSIGNE;
			}
			if (type == "RAM")
			{
				checkP.Visibility = ViewStates.Gone;
			}

			Button btnvalide = FindViewById<Button>(Resource.Id.valider);
			btnvalide.Click += Btnvalide_Clik;
		}

		void Btnvalide_Clik(object sender, EventArgs e)
		{
			if (data.CR == "" || data.CR == "0" || type == "RAM")
			{
				if (data.ASSIGNE == "" || data.ASSIGNE == "0")
				{
					valideAction();
				}else
				{
					traitDial();
				}
			}
			else
			{
				traitDial();
			}
		}

		void traitDial() {
			string title;
			string typeMo;

			if (data.CR == "" || data.CR == "0")
			{
				title = "ASSIGNE";
				typeMo = "l'assigne";
			}
			else
			{
				title = "CR";
				typeMo = "le cr";
			}
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.SetTitle(title);
			builder.SetMessage("Avez vous perçu " + typeMo + " ?\n Si oui, valider cette livraison ?");
			builder.SetCancelable(false);
			builder.SetPositiveButton("Oui", delegate
			{
				valideAction();
			});
			builder.SetNegativeButton("Non", delegate { });
			builder.Show();
		}

		void valideAction()
		{
			//format mémo
			string formatmémo = mémo.Text.Replace("\"", " ").Replace("'", " ");
			//case btn check
			string typecr;
			if (check2.Checked)
			{
				typecr = "CHEQUE";
				string JSONCHEQUE = "{\"codesuiviliv\":\"" + typecr + "\",\"memosuiviliv\":\"cheque\",\"libellesuiviliv\":\"\",\"commandesuiviliv\":\"" + data.numCommande + "\",\"groupagesuiviliv\":\"" + data.groupage + "\",\"datesuiviliv\":\"" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\",\"posgps\":\"" + Data.GPS + "\"}";
				DBRepository.Instance.insertDataStatutpositions(typecr, "1", typecr, data.numCommande, formatmémo, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), JSONCHEQUE);
			}
			if (check1.Checked)
			{
				typecr = "ESPECE";
				string JSONESPECE = "{\"codesuiviliv\":\"" + typecr + "\",\"memosuiviliv\":\"espece\",\"libellesuiviliv\":\"\",\"commandesuiviliv\":\"" + data.numCommande + "\",\"groupagesuiviliv\":\"" + data.groupage + "\",\"datesuiviliv\":\"" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\",\"posgps\":\"" + Data.GPS + "\"}";
				DBRepository.Instance.insertDataStatutpositions(typecr, "1", typecr, data.numCommande, formatmémo, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), JSONESPECE);
			}
			if (checkP.Checked)
			{
				typecr = "PARTIC";
				string JSONPARTIC = "{\"codesuiviliv\":\"" + typecr + "\",\"memosuiviliv\":\"particulier\",\"libellesuiviliv\":\"\",\"commandesuiviliv\":\"" + data.numCommande + "\",\"groupagesuiviliv\":\"" + data.groupage + "\",\"datesuiviliv\":\"" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\",\"posgps\":\"" + Data.GPS + "\"}";
				DBRepository.Instance.insertDataStatutpositions(typecr, "1", typecr, data.numCommande, formatmémo, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), JSONPARTIC);
			}

			//mise du statut de la position à 1
			DBRepository.Instance.updatePosition(i, "1", "Validée", formatmémo, tyValide, null);
			//creation du JSON
			string JSON = "{\"codesuiviliv\":\"" + tyValide + "\",\"memosuiviliv\":\"" + (formatmémo).Replace("\"", " ").Replace("\'", " ") + "\",\"libellesuiviliv\":\"\",\"commandesuiviliv\":\"" + data.numCommande + "\",\"groupagesuiviliv\":\"" + data.groupage + "\",\"datesuiviliv\":\"" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\",\"posgps\":\"" + Data.GPS + "\"}";
			//création de la notification webservice // statut de position
			DBRepository.Instance.insertDataStatutpositions(tyValide, "1", "Validée", data.numCommande, formatmémo, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), JSON);

			Data.Instance.traitImg(i, type, this);

			DBRepository.Instance.SETBadges(Data.userAndsoft);

			//si user got signature true take signature then go to listliv
			bool sign = DBRepository.Instance.is_user_Sign(Data.userAndsoft);
			if (sign)
			{
				Intent intent = new Intent(this, typeof(SignatureActivity));
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
			Data._file = new Java.IO.File(Data._dir, String.Format("" + DateTime.Now.ToString("ddMM") + "_" + data.numCommande + ".jpg", Guid.NewGuid()));
			intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(Data._file));
			StartActivityForResult(intent, 0);
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
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
				DBRepository.Instance.updateposimgpath(i, Data._file.Path);
				Data.bitmap = null;
			}
			GC.Collect();
		}

		public override void OnBackPressed()
		{
			if (Intent.GetBooleanExtra("FLASH", false))
			{
				DBRepository.Instance.resetColis(data.numCommande);

				Intent intent = new Intent(this, typeof(FlashageQuaiActivity));
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
				builder.SetMessage("Voulez-vous annulée la validation ?");
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

