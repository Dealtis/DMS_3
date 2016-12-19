using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
namespace DMS_3
{
	[Activity(Label = "ListViewAdapterMenu")]
	public class ListViewAdapterMenu : BaseAdapter<TablePositions>
	{
		private List<TablePositions> mItems;
		private Context mContext;
		public ListViewAdapterMenu(Context context, List<TablePositions> items) : base()
		{
			mItems = items;
			mContext = context;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override TablePositions this[int position]
		{
			get { return mItems[position]; }
		}
		public override int Count
		{
			get { return mItems.Count; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View row = convertView;
			LayoutInflater inflater = (LayoutInflater)mContext.GetSystemService(Context.LayoutInflaterService);
			int xml_type = 0;

			switch (mItems[position].StatutLivraison)
			{
				default:
					break;
				case "0":
					if (mItems[position].typeMission == "L")
					{
						if (mItems[position].CR == "" || mItems[position].CR == "0")
						{
							xml_type = Resource.Layout.ListeViewRow;
							if (mItems[position].ASSIGNE == "" || mItems[position].ASSIGNE == "0")
							{
								xml_type = Resource.Layout.ListeViewRow;
							}
							else
							{
								xml_type = Resource.Layout.ListeViewRowEuro;
							}
						}
						else
						{
							xml_type = Resource.Layout.ListeViewRowEuro;

						}

					}
					else {
						xml_type = Resource.Layout.ListeViewRowEnlevement;
					}


					break;
				case "1":
					if (mItems[position].imgpath == null || mItems[position].imgpath == "" || mItems[position].imgpath == "null")
					{
						xml_type = Resource.Layout.ListeViewRowValide;
					}
					else {
						xml_type = Resource.Layout.ListeViewRowValidePJ;
					}

					break;
				case "2":
					if (mItems[position].imgpath == null || mItems[position].imgpath == "" || mItems[position].imgpath == "null")
					{
						xml_type = Resource.Layout.ListeViewRowAnomalie;
					}
					else {
						xml_type = Resource.Layout.ListeViewRowAnomaliePJ;
					}
					break;
			}
			if (mItems[position].imgpath == "SUPPLIV")
			{
				xml_type = Resource.Layout.ListeViewRowStroke;
			}

			row = inflater.Inflate(xml_type, parent, false);

			TextView textLeft = row.FindViewById<TextView>(Resource.Id.textleft);
			TextView textMid = row.FindViewById<TextView>(Resource.Id.textmid);
			TextView textMidBis = row.FindViewById<TextView>(Resource.Id.textmidbis);
			TextView textRight = row.FindViewById<TextView>(Resource.Id.txtright);
			ImageView ico = row.FindViewById<ImageView>(Resource.Id.imageView1);
			ImageView pole = row.FindViewById<ImageView>(Resource.Id.logo_pole);

			textLeft.SetTypeface(Data.LatoBlack, Android.Graphics.TypefaceStyle.Normal);
			textMid.SetTypeface(Data.LatoBold, Android.Graphics.TypefaceStyle.Normal);
			textRight.SetTypeface(Data.LatoBold, Android.Graphics.TypefaceStyle.Normal);
			textMidBis.SetTypeface(Data.LatoBold, Android.Graphics.TypefaceStyle.Normal);

			textLeft.Text = "OT: " + mItems[position].numCommande + " " + mItems[position].planDeTransport;
			textMid.Text = mItems[position].CpLivraison + " " + mItems[position].villeLivraison + "\tCol: " + mItems[position].nbrColis + " Pal:" + mItems[position].nbrPallette;
			textRight.Text = mItems[position].instrucLivraison;
			textMidBis.Text = mItems[position].nomPayeur;

			if (mItems[position].positionPole == "0")
			{
				pole.Visibility = ViewStates.Gone;
			}
			string poidsADR = mItems[position].poidsADR.Replace(".", ",");
			if (Convert.ToDouble(poidsADR) >= 1000)
			{
				ico.SetImageResource(Resource.Drawable.LivADR_100);
			}
			else
			{
				string poidsQL = mItems[position].poidsQL.Replace(".", ",");
				if (Convert.ToDouble(poidsQL) >= 8000)
				{
					ico.SetImageResource(Resource.Drawable.ql_100);
				}
			}

			return row;
		}
	}
}
