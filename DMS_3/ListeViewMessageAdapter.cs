using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;

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
			TextView txtName;
			TextView txtdatestatut;

			if (mItems[position].typeMessage == 1)
			{

				row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowRight, null, false);
				txtName = row.FindViewById<TextView>(Resource.Id.txtName);
				txtdatestatut = row.FindViewById<TextView>(Resource.Id.textds);
				txtName.Text = "" + mItems[position].texteMessage + "";
				txtdatestatut.Text = "Le " + mItems[position].dateImportMessage.Day + " à " + mItems[position].dateImportMessage.Hour + ":" + mItems[position].dateImportMessage.Minute + " " + getTextStatut(mItems[position].statutMessage) + " par " + mItems[position].utilisateurEmetteur;
			}
			else {
				row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowLeft, null, false);
				txtName = row.FindViewById<TextView>(Resource.Id.txtName);
				txtdatestatut = row.FindViewById<TextView>(Resource.Id.textds);
				txtName.Text = "" + mItems[position].texteMessage + "";
				txtdatestatut.Text = "Le " + mItems[position].dateImportMessage.Day + " à " + mItems[position].dateImportMessage.Hour + ":" + mItems[position].dateImportMessage.Minute + " " + getTextStatut(mItems[position].statutMessage) + "";
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

