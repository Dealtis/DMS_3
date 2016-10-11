
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
using Android.Graphics;

namespace DMS_3
{
	[Activity(Label = "ListeViewMessageAdapter")]
	public class ListeViewMessageAdapter : BaseAdapter<TableMessages>
	{
		private List<TableMessages> mItems;
		private Context mContext;

		public ListeViewMessageAdapter(Context context, List<TableMessages> items)
		{
			mItems = items;
			mContext = context;
		}
		public override long GetItemId(int position)
		{
			return position;
		}

		public override TableMessages this[int position]
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
			var txtstatut = "";

			TextView txtName = row.FindViewById<TextView>(Resource.Id.txtName);
			TextView txtdatestatut = row.FindViewById<TextView>(Resource.Id.textds);

			if (mItems[position].typeMessage == 1)
			{

				row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowRight, null, false);

				txtName.Text = "" + mItems[position].texteMessage + "";

				txtdatestatut.Text = "Le " + mItems[position].dateImportMessage.Day + " à " + mItems[position].dateImportMessage.Hour + ":" + mItems[position].dateImportMessage.Minute + " " + getTextStatut() + " par " + mItems[position].utilisateurEmetteur;
			}
			else {

				row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowLeft, null, false);

				txtName.Text = "" + mItems[position].texteMessage + "";

				txtdatestatut.Text = "Le " + mItems[position].dateImportMessage.Day + " à " + mItems[position].dateImportMessage.Hour + ":" + mItems[position].dateImportMessage.Minute + " " + getTextStatut() + "";
			}

			return row;
		}

		string getTextStatut(int statut)
		{
			switch (statut)
			{
				case 0:
					return "Nouveau";
				case 1:
					return "Lu";
				case 2:
					return "En attente";
				case 3:
					return "Envoyé";
				default:
					return "null";
			}
		}
	}
}

