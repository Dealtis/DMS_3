using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Preferences;
using Android.Provider;
using DMS_3.BDD;
using Console = System.Console;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
namespace DMS_3
{
	class Data
	{
		//Instance
		private static Data instance;
		//DATA User
		public static string userAndsoft;
		public static string userTransics;

		//Table user
		public static string tableuserload = "false";


		//GPS
		public static string GPS;

		//PHOTO
		public static Java.IO.File _file;
		public static Java.IO.File _dir;
		public static Bitmap bitmap;

		//BADGES
		private int livraisonIndicator;
		private int enlevementIndicator;
		private int messageIndicator;

		//FONT

		public static Typeface LatoBlack = Typeface.CreateFromAsset(Application.Context.Assets, "fonts/Lato-Black.ttf");
		public static Typeface LatoBold = Typeface.CreateFromAsset(Application.Context.Assets, "fonts/Lato-Bold.ttf");
		public static Typeface LatoLight = Typeface.CreateFromAsset(Application.Context.Assets, "fonts/Lato-Light.ttf");
		public static Typeface LatoMedium = Typeface.CreateFromAsset(Application.Context.Assets, "fonts/Lato-Medium.ttf");
		public static Typeface LatoRegular = Typeface.CreateFromAsset(Application.Context.Assets, "fonts/Lato-Regular.ttf");

		public static bool Is_Service_Running = false;
		public static Data Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new Data();
				}
				return instance;
			}
		}
		public int getLivraisonIndicator()
		{ return livraisonIndicator; }

		public void setLivraisonIndicator(int _livraisonIndicator)
		{
			livraisonIndicator = _livraisonIndicator;
		}

		public int getEnlevementIndicator()
		{ return enlevementIndicator; }

		public void setEnlevementIndicator(int _enlevementIndicator)
		{
			enlevementIndicator = _enlevementIndicator;
		}

		public int getMessageIndicator()
		{ return messageIndicator; }

		public void setMessageIndicator(int _messageIndicator)
		{
			messageIndicator = _messageIndicator;
		}

		public Bitmap DecodeSmallFile(String filename, int width, int height)
		{
			var options = new BitmapFactory.Options { InJustDecodeBounds = true };
			BitmapFactory.DecodeFile(filename, options);
			options.InSampleSize = CalculateInSampleSize(options, width, height);
			options.InJustDecodeBounds = false;
			return BitmapFactory.DecodeFile(filename, options);
		}

		private static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
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
		public int isMatdang(string groupage)
		{

			DBRepository dbr = new DBRepository();
			var isornot = dbr.CountMatiereDang(groupage);

			if (Convert.ToInt32(isornot[0].poidsADR) >= 1000)
			{
				return 1;
			}
			else
			{
				if (Convert.ToInt32(isornot[0].poidsQL) >= 8000)
				{
					return 2;
				}
				else
				{
					return 0;
				}
			}

		}

		public bool UploadFile(string FtpUrl, string fileName, string userName, string password, string UploadDirectory)
		{
			try
			{
				string PureFileName = new FileInfo(fileName).Name;
				String uploadUrl = String.Format("{0}{1}/{2}", FtpUrl, UploadDirectory, PureFileName);
				FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(uploadUrl);
				req.Proxy = null;
				req.Method = WebRequestMethods.Ftp.UploadFile;
				req.Credentials = new NetworkCredential(userName, password);
				req.UseBinary = true;
				req.UsePassive = true;
				byte[] data = System.IO.File.ReadAllBytes(fileName);
				req.ContentLength = data.Length;
				System.IO.Stream stream = req.GetRequestStream();
				stream.Write(data, 0, data.Length);
				stream.Close();
				FtpWebResponse res = (FtpWebResponse)req.GetResponse();
				Console.Out.Write("Upload file" + fileName + " good " + res);
				return true;

			}
			catch (Exception ex)
			{
				Console.Out.Write("Upload file" + fileName + " error\n" + ex);
				return false;
			}
		}

		internal void traitImg(int i, string type, Context context)
		{
			DBRepository dbr = new DBRepository();
			var imgpath = dbr.GetPositionsData(i);
			string compImg = String.Empty;
			if (imgpath.imgpath != "null")
			{
				Thread threadUpload = new Thread(() =>
				{
					try
					{
						Android.Graphics.Bitmap bmp = Data.Instance.DecodeSmallFile(imgpath.imgpath, 1000, 1000);
						Bitmap rbmp = Bitmap.CreateScaledBitmap(bmp, bmp.Width / 2, bmp.Height / 2, true);
						compImg = imgpath.imgpath.Replace(".jpg", "-1_1.jpg");
						using (var fs = new FileStream(compImg, FileMode.OpenOrCreate))
						{
							rbmp.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, fs);
						}
						//ftp://77.158.93.75 ftp://10.1.2.75
						ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
						//if jeantet
						if ( prefs.GetString("API_LOCATION", null) == "JEANTET")
						{
							Data.Instance.UploadFile("ftp://77.158.93.75", compImg, "DMS", "Linuxr00tn", "");
						}
						else
						{
							if (prefs.GetString("API_LOCATION", null) != null)
							{
								Data.Instance.UploadFile("ftp://176.31.10.169", compImg, "DMSPHOTO", "DMS25000", "");
							}else
							{
								dbr.InsertDataStatutMessage(11, DateTime.Now, 1, imgpath.numCommande, "");
							}
						}

						bmp.Recycle();
						rbmp.Recycle();
					}
					catch (Exception ex)
					{
						Console.WriteLine("\n" + ex);

					}
				});
				threadUpload.Start();
			}
			if (Data.bitmap != null)
			{
				Data.bitmap.Recycle();
			}
		}

		public void CreateDirectoryForPictures()
		{
			Data._dir = new Java.IO.File(Environment.GetExternalStoragePublicDirectory(
					Environment.DirectoryPictures), "DMSIMG");
			if (!Data._dir.Exists())
			{
				Data._dir.Mkdirs();
			}
		}
	}

	public static class BitmapHelpers
	{
		public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
		{
			// First we get the the dimensions of the file on disk
			BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
			BitmapFactory.DecodeFile(fileName, options);

			// Next we calculate the ratio that we need to resize the image by
			// in order to fit the requested dimensions.
			int outHeight = options.OutHeight;
			int outWidth = options.OutWidth;
			int inSampleSize = 1;

			if (outHeight > height || outWidth > width)
			{
				inSampleSize = outWidth > outHeight
								   ? outHeight / height
								   : outWidth / width;
			}

			// Now we will load the image and have BitmapFactory resize it for us.
			options.InSampleSize = inSampleSize;
			options.InJustDecodeBounds = false;
			Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

			return resizedBitmap;
		}
	}
}

