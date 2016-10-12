﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using DMS_3.BDD;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;

namespace DMS_3
{
	[Activity(Label = "ValidationActivity", Theme = "@android:style/Theme.Black.NoTitleBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class ValidationActivity : Activity
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
			DBRepository dbr = new DBRepository();
			data = dbr.GetPositionsData(i);

			if (data.CR == "" || data.CR == "0" || type == "RAM")
			{
				check1.Visibility = ViewStates.Gone;
				check2.Visibility = ViewStates.Gone;
				txtCR.Visibility = ViewStates.Gone;
				//dialog.SetMessage("Voulez-vous valider cette position ?");
			}
			else {
				check1.Visibility = ViewStates.Visible;
				check2.Visibility = ViewStates.Visible;
				txtCR.Visibility = ViewStates.Visible;
				txtCR.Text = data.CR;
				//dialog.SetMessage("Avez vous perçu le CR,?\n Si oui, valider cette livraison ?");
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
			DBRepository dbr = new DBRepository();
			//format mémo
			string formatmémo = mémo.Text.Replace("\"", " ").Replace("'", " ");
			//case btn check
			string typecr;
			if (check2.Checked)
			{
				typecr = "CHEQUE";
				string JSONCHEQUE = "{\"codesuiviliv\":\"" + typecr + "\",\"memosuiviliv\":\"cheque\",\"libellesuiviliv\":\"\",\"commandesuiviliv\":\"" + data.numCommande + "\",\"groupagesuiviliv\":\"" + data.groupage + "\",\"datesuiviliv\":\"" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\",\"posgps\":\"" + Data.GPS + "\"}";
				dbr.insertDataStatutpositions(typecr, "1", typecr, data.numCommande, formatmémo, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), JSONCHEQUE);
			}
			if (check1.Checked)
			{
				typecr = "ESPECE";
				string JSONESPECE = "{\"codesuiviliv\":\"" + typecr + "\",\"memosuiviliv\":\"espece\",\"libellesuiviliv\":\"\",\"commandesuiviliv\":\"" + data.numCommande + "\",\"groupagesuiviliv\":\"" + data.groupage + "\",\"datesuiviliv\":\"" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\",\"posgps\":\"" + Data.GPS + "\"}";
				dbr.insertDataStatutpositions(typecr, "1", typecr, data.numCommande, formatmémo, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), JSONESPECE);
			}
			if (checkP.Checked)
			{
				typecr = "PARTIC";
				string JSONPARTIC = "{\"codesuiviliv\":\"" + typecr + "\",\"memosuiviliv\":\"particulier\",\"libellesuiviliv\":\"\",\"commandesuiviliv\":\"" + data.numCommande + "\",\"groupagesuiviliv\":\"" + data.groupage + "\",\"datesuiviliv\":\"" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\",\"posgps\":\"" + Data.GPS + "\"}";
				dbr.insertDataStatutpositions(typecr, "1", typecr, data.numCommande, formatmémo, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), JSONPARTIC);
			}

			//mise du statut de la position à 1
			dbr.updatePosition(i, "1", "Validée", formatmémo, tyValide, null);
			//creation du JSON
			string JSON = "{\"codesuiviliv\":\"" + tyValide + "\",\"memosuiviliv\":\"" + (formatmémo).Replace("\"", " ").Replace("\'", " ") + "\",\"libellesuiviliv\":\"\",\"commandesuiviliv\":\"" + data.numCommande + "\",\"groupagesuiviliv\":\"" + data.groupage + "\",\"datesuiviliv\":\"" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\",\"posgps\":\"" + Data.GPS + "\"}";
			//création de la notification webservice // statut de position
			dbr.insertDataStatutpositions(tyValide, "1", "Validée", data.numCommande, formatmémo, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), JSON);

			Data.Instance.traitImg(i, type, this);
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
			DBRepository dbr = new DBRepository();
			// Make it available in the gallery

			Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
			Uri contentUri = Uri.FromFile(Data._file);
			mediaScanIntent.SetData(contentUri);
			SendBroadcast(mediaScanIntent);

			// Display in ImageView. We will resize the bitmap to fit the display.
			// Loading the full sized image will consume to much memory
			// and cause the application to crash.

			int height = Resources.DisplayMetrics.HeightPixels;
			int width = _imageView.Height;
			Data.bitmap = Data._file.Path.LoadAndResizeBitmap(width, height);
			if (Data.bitmap != null)
			{
				_imageView.SetImageBitmap(Data.bitmap);
				dbr.updateposimgpath(i, Data._file.Path);
				Data.bitmap = null;
			}
			GC.Collect();
		}

		public override void OnBackPressed()
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.SetTitle("Annulation");
			builder.SetMessage("Voulez-vous annulée la validation ?");
			builder.SetCancelable(false);
			builder.SetPositiveButton("Oui", delegate
			{
				if (data.StatutLivraison == "1" || data.StatutLivraison == "2")
				{
					Intent intent = new Intent(this, typeof(ListeLivraisonsActivity));
					intent.PutExtra("TYPE", type);
					this.StartActivity(intent);
					Finish();
					_imageView.Dispose();
					//this.OverridePendingTransition (Android.Resource.Animation.SlideInLeft,Android.Resource.Animation.SlideOutRight);
				}
				else {
					Intent intent = new Intent(this, typeof(ListeLivraisonsActivity));
					intent.PutExtra("TYPE", type);
					this.StartActivity(intent);
					Finish();
					_imageView.Dispose();
					//this.OverridePendingTransition (Android.Resource.Animation.SlideInLeft,Android.Resource.Animation.SlideOutRight);
				}
			});
			builder.SetNegativeButton("Non", delegate { });

			builder.Show();

		}
	}
}

