
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Graphics;
using Android.Widget;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Android.Support.V7.App;
using SignaturePad;
using System.Threading;
using System.IO;
using System.Net;

namespace DMS_3
{

	[Activity(Label = "SignatureActivity", Theme = "@style/MyTheme.Base")]
	public class SignatureActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.Signature);
			SignaturePadView signature = FindViewById<SignaturePadView>(Resource.Id.signatureView);
			signature.Caption.Text = "Signé ici";

			Button btnSave = FindViewById<Button>(Resource.Id.btnSave);
			btnSave.Click += delegate
			{
				if (signature.IsBlank)
				{
					AlertDialog.Builder alert = new AlertDialog.Builder(this);
					alert.SetMessage("Pas de signature");
					alert.SetNeutralButton("Ok", delegate { });
					alert.Create().Show();
				}
				else
				{
					Thread threadUpload = new Thread(() =>
					{
						try
						{
							Bitmap signbmp = signature.GetImage();
							string FtpUrl = "ftp://77.158.93.75";
							string UploadDirectory = "/Signature_TEST";
							String uploadUrl = String.Format("{0}{1}/{2}", FtpUrl, UploadDirectory, "upload.png");
							FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(uploadUrl);
							req.Proxy = null;
							req.Method = WebRequestMethods.Ftp.UploadFile;
							req.Credentials = new NetworkCredential("DMS", "Linuxr00tn");
							req.UseBinary = true;
							req.UsePassive = true;
							byte[] bitmapData;
							using (var streamF = new MemoryStream())
							{
								signbmp.Compress(Bitmap.CompressFormat.Png, 100, streamF);
								bitmapData = streamF.ToArray();
							}
							req.ContentLength = bitmapData.Length;
							System.IO.Stream stream = req.GetRequestStream();
							stream.Write(bitmapData, 0, bitmapData.Length);
							stream.Close();
						}
						catch (Exception ex)
						{
							Console.WriteLine("\n" + ex);
						}
					});
					threadUpload.Start();

					Intent intent = new Intent(this, typeof(ListeLivraisonsActivity));
					intent.PutExtra("TYPE", Intent.GetStringExtra("TYPE"));
					intent.PutExtra("TRAIT", "false");
					this.StartActivity(intent);
				}
			};
			btnSave.Dispose();

		}

		public override void OnBackPressed()
		{

		}
	}
}
