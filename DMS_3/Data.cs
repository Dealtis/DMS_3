using System;
using System.IO;
using Java.IO;
using System.Net;
using Android.Graphics;
using System.Threading;
using File = System.IO.File;
using Console = System.Console;
using SQLite;
using DMS_3.BDD;
using Xamarin;
using System.Json;
using Android.Media;
using Android.App;

namespace DMS_3
{
	class Data
	{
		//Instance
		private static Data instance;
		DBRepository dbr = new DBRepository();

		//DATA User
		public static string userAndsoft;
		public static string userTransics;

		//Table user
		public static bool tableuserload = false;


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

		public int isMatdang(string groupage)
		{
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
				Console.Out.Write("Upload file" + fileName + " good "+res);
				return true;

			}
			catch (Exception ex)
			{
				Console.Out.Write("Upload file" + fileName + " error\n" + ex);
				return false;
			}
		}
	}
}

