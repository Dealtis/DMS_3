using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using DMS_3.BDD;

namespace DMS_3
{
	[Activity(Label = "MessageActivity", Theme = "@style/MyTheme.Base")]
	public class MessageActivity : AppCompatActivity
	{
		private List<TableMessages> mItems;
		private ListView mListView;
		public ListeViewMessageAdapter adapter;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.ChatLayout);

			//LISTVIEW
			mListView = FindViewById<ListView>(Resource.Id.listViewBox);

			mItems = new List<TableMessages>();

			var table = DBRepository.Instance.QueryMessage("SELECT * FROM TableMessages where codeChauffeur='" + Data.userAndsoft + "' and typeMessage != 5");
			var i = 0;

			foreach (var item in table)
			{
				mItems.Add(new TableMessages()
				{
					texteMessage = item.texteMessage,
					utilisateurEmetteur = item.utilisateurEmetteur,
					statutMessage = item.statutMessage,
					dateImportMessage = item.dateImportMessage,
					typeMessage = item.typeMessage,
					Id = item.Id
				});
				i++;
			}

			if (i > 6)
			{
				View view = LayoutInflater.From(this).Inflate(Resource.Layout.ListeViewDelete, null, false);
				mListView.AddHeaderView(view);
				view.Click += Btndeletemsg_Click;
			}

			adapter = new ListeViewMessageAdapter(this, mItems);
			mListView.Adapter = adapter;

			//EDITTEXT
			var btnsend = FindViewById<LinearLayout>(Resource.Id.btn_send);
			btnsend.Click += Btnsend_Click;

			//STATUT DES MESSAGES RECU TO 1
			var tablemsgrecu = DBRepository.Instance.QueryMessage("SELECT * FROM TableMessages where statutMessage = 0");
			foreach (var item in tablemsgrecu)
			{
				DBRepository.Instance.QueryMessage("UPDATE TableMessages SET statutMessage = 1 WHERE statutMessage = 0");
				DBRepository.Instance.InsertDataStatutMessage(1, DateTime.Now, item.numMessage, "", "");
			}
			DBRepository.Instance.SETBadges(Data.userAndsoft);
		}

		void Btnsend_Click(object sender, EventArgs e)
		{
			var newmessage = FindViewById<TextView>(Resource.Id.editnewmsg);
			if (newmessage.Text == "")
			{

			}
			else {
				string formatmsg = newmessage.Text.Replace("\"", " ").Replace("'", " ");
				DBRepository.Instance.insertDataMessage(Data.userAndsoft, "", formatmsg, 2, DateTime.Now, 2, 0);

			}

			Intent intent = new Intent(this, typeof(MessageActivity));
			this.StartActivity(intent);
		}


		void Btndeletemsg_Click(object sender, EventArgs e)
		{

			DBRepository.Instance.DropTableMessage();
			StartActivity(typeof(MessageActivity));

		}

		public override void OnBackPressed()
		{
			Intent intent = new Intent(this, typeof(HomeActivity));
			this.StartActivity(intent);
			Finish();
		}
	}
}


