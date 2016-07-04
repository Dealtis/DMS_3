using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using DMS_3.BDD;
namespace DMS_3
{
	[Activity (Label = "ListViewAdapterMenu")]			
	public class ListViewAdapterMenu : BaseAdapter<TablePositions> {
		private List<TablePositions> mItems;
		private Context mContext;
		string txtspinner;
		public ListViewAdapterMenu(Context context,List<TablePositions> items) : base() {
			mItems = items;
			mContext = context;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override TablePositions this[int position] {  
			get { return mItems[position]; }
		}
		public override int Count {
			get { return mItems.Count; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View row = convertView;
			LayoutInflater inflater = (LayoutInflater) mContext.GetSystemService(Context.LayoutInflaterService);
			int xml_type = 0;

			switch (mItems [position].StatutLivraison) {
			default:
				break;
			case "0":
				if (mItems [position].typeMission == "L") {
					xml_type = Resource.Layout.ListeViewRow;
				} else {
					xml_type = Resource.Layout.ListeViewRowEnlevement;
				}
				break;
			case "1":
				xml_type = Resource.Layout.ListeViewRowValide;
				break;
			case "2":
				if (mItems[position].imgpath == null || mItems[position].imgpath == "" || mItems[position].imgpath == "null") {
					xml_type = Resource.Layout.ListeViewRowAnomalie;
				} else {
					xml_type = Resource.Layout.ListeViewRowAnomaliePJ;
				}
				break;
			}
			if(mItems[position].imgpath == "SUPPLIV"){
				xml_type = Resource.Layout.ListeViewRowStroke;
			}

			row = inflater.Inflate(xml_type,parent,false);


			TextView textLeft = row.FindViewById<TextView> (Resource.Id.textleft);
			TextView textMid = row.FindViewById<TextView> (Resource.Id.textmid);
			TextView textMidBis = row.FindViewById<TextView> (Resource.Id.textmidbis);
			TextView textRight = row.FindViewById<TextView> (Resource.Id.txtright);
			//Button btnvalid = row.FindViewById<Button> (Resource.Id.btn_valider);
			//Button btnanomalie = row.FindViewById<Button> (Resource.Id.btn_anomalie);

			textLeft.SetTypeface (Data.LatoBlack, Android.Graphics.TypefaceStyle.Normal);
			textMid.SetTypeface (Data.LatoBold, Android.Graphics.TypefaceStyle.Normal);
			textRight.SetTypeface (Data.LatoBold, Android.Graphics.TypefaceStyle.Normal);
			textMidBis.SetTypeface (Data.LatoBold, Android.Graphics.TypefaceStyle.Normal);

			textLeft.Text = "OT: "+mItems[position].numCommande+" "+mItems[position].planDeTransport;
			textMid.Text = mItems[position].CpLivraison+" "+mItems[position].villeLivraison+"\tCol: "+mItems[position].nbrColis+" Pal:"+mItems[position].nbrPallette;
			textRight.Text = mItems [position].instrucLivraison;
			textMidBis.Text = mItems [position].nomPayeur;

			return row;
		}
	}
}
