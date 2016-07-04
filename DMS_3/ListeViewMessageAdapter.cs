
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
	[Activity (Label = "ListeViewMessageAdapter")]			
	public class ListeViewMessageAdapter : BaseAdapter<TableMessages> {
		private List<TableMessages> mItems;
		private Context mContext;

		public ListeViewMessageAdapter(Context context,List<TableMessages> items){
			mItems = items;
			mContext = context;
		}
		public override long GetItemId(int position)
		{
			return position;
		}

		public override TableMessages this[int position] {  
			get { return mItems[position]; }
		}
		public override int Count {
			get { return mItems.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View row = convertView;
			var txtstatut = "";
			if(mItems [position].typeMessage == 1){

				row = LayoutInflater.From (mContext).Inflate (Resource.Layout.RowRight, null, false);


				TextView txtName = row.FindViewById<TextView> (Resource.Id.txtName);
				TextView txtdatestatut = row.FindViewById<TextView> (Resource.Id.textds);
				txtName.Text = ""+mItems[position].texteMessage+"";

				if(mItems[position].statutMessage == 0){
					txtstatut ="Nouveau";
				}
				if(mItems[position].statutMessage == 1){
					txtstatut ="Lu";
				}
				if(mItems[position].statutMessage == 2){
					txtstatut ="En attente";
				}
				if(mItems[position].statutMessage == 3){
					txtstatut ="Envoyé";
				}
				txtdatestatut.Text="Le "+mItems[position].dateImportMessage.Day+" à "+mItems[position].dateImportMessage.Hour+":"+mItems[position].dateImportMessage.Minute+" "+txtstatut+" par "+mItems[position].utilisateurEmetteur;
			}else{
				
				row = LayoutInflater.From (mContext).Inflate (Resource.Layout.RowLeft, null, false);


				TextView txtName = row.FindViewById<TextView> (Resource.Id.txtName);
				TextView txtdatestatut = row.FindViewById<TextView> (Resource.Id.textds);

				txtName.Text = ""+mItems[position].texteMessage+"";

				if(mItems[position].statutMessage == 0){
					txtstatut ="Nouveau";
				}
				if(mItems[position].statutMessage == 1){
					txtstatut ="Lu";
				}
				if(mItems[position].statutMessage == 2){
					txtstatut ="En attente";
				}
				if(mItems[position].statutMessage == 3){
					txtstatut ="Envoyé";
				}

				txtdatestatut.Text="Le "+mItems[position].dateImportMessage.Day+" à "+mItems[position].dateImportMessage.Hour+":"+mItems[position].dateImportMessage.Minute+" "+txtstatut+"";
			}

			return row;
		}
	}
}

