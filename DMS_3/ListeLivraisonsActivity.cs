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
using DMS_3.BDD;
using SQLite;
using AndroidHUD;
namespace DMS_3
{
	
	[Activity (Label = "ListeLivraisonsActivity",Theme = "@android:style/Theme.Black.NoTitleBar")]			
	public class ListeLivraisonsActivity : Activity
	{

		string[] Arraygrp = new string[10];
		public List<TablePositions> bodyItems;
		public ListView bodyListView;
		public bool insertdone;
		public ListViewAdapterMenu adapter;

		Button btngrpAll;
		Button btngrp1;
		Button btngrp2;
		Button btngrp3;
		Button btngrp4;
		LinearLayout btnsearch;
		Button btntrait;
		string type;
		string tyS;
		string tyM;


		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.ListeLivraisons);

			type = Intent.GetStringExtra ("TYPE");
			if (type == "RAM") {
				tyS = "RAM";
				tyM = "C";
			} else {				
				tyS = "LIV";
				tyM = "L";
			}

			//declaration des clicks btns
			btngrpAll = FindViewById<Button> (Resource.Id.btn_all);
			btngrp1 = FindViewById<Button> (Resource.Id.btn_1);
			btngrp2 = FindViewById<Button> (Resource.Id.btn_2);
			btngrp3 = FindViewById<Button> (Resource.Id.btn_3);
			btngrp4 = FindViewById<Button> (Resource.Id.btn_4);
			btnsearch = FindViewById<LinearLayout> (Resource.Id.btn_search);
			btntrait = FindViewById<Button> (Resource.Id.btn_traite);	

		}

		protected override void OnResume()
		{
			base.OnResume();

			btngrpAll.Click += delegate {
				btngrpAll_Click();
			};
			btngrp1.Click += delegate {
				btngrp1_Click();
			};
			btngrp2.Click += delegate {
				btngrp2_Click();
			};
			btngrp3.Click += delegate {
				btngrp3_Click();
			};
			btnsearch.Click += delegate {
				btnsearch_Click();
			};
			btntrait.Click += delegate {
				btntrait_Click();
			};

			//Mise dans un Array des Groupage
			string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath
				(System.Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			var grp = db.Query<TablePositions> ("SELECT * FROM TablePositions WHERE StatutLivraison = ? AND typeMission= ? AND typeSegment= ?  AND Userandsoft = ?  GROUP BY groupage",0,tyM,tyS,Data.userAndsoft);

			int i = 1;
			int countGrp = 0;
			foreach (var item in grp){				
				Arraygrp[i] = item.groupage;
				i++;
				countGrp++;
			}

			switch (countGrp) {
			case 0:
				//btn all big size
				btngrpAll.SetWidth (5000);
				//afficher pas de tournée au milieu et sur le btnall
				btngrpAll.Text = "Aucune position";
				break;
			case 1:
				//btn all avec le num de grp
				btngrpAll.SetWidth (5000);
				btngrpAll.Text = Arraygrp[1];
				break;
			case 2:
				//afficher le btn 1 et 2
				btngrp1.Visibility = ViewStates.Visible;
				btngrp1.Text = Arraygrp[1];
				btngrp2.Visibility = ViewStates.Visible;
				btngrp2.Text = Arraygrp[2];
				break;
			case 3:
				//afficher le btn 1,2 et 3
				btngrp1.Visibility = ViewStates.Visible;
				btngrp1.Text = Arraygrp[1];
				btngrp2.Visibility = ViewStates.Visible;
				btngrp2.Text = Arraygrp[2];
				btngrp3.Visibility = ViewStates.Visible;
				btngrp3.Text = Arraygrp[3];
				break;
			case 4:
				//afficher le btn 1,2,3 et 4
				btngrp1.Visibility = ViewStates.Visible;
				btngrp1.Text = Arraygrp[1];

				btngrp2.Visibility = ViewStates.Visible;
				btngrp2.Text = Arraygrp[2];

				btngrp3.Visibility = ViewStates.Visible;
				btngrp3.Text = Arraygrp[3];

				btngrp4.Visibility = ViewStates.Visible;
				btngrp4.Text = Arraygrp[4];
				break;
			default:
				//btn all big size
				btngrpAll.SetWidth (5000);
				//afficher pas de tournée au milieu et sur le btnall
				btngrpAll.Text = "Aucune position";
				break;
			}

			//LISTVIEW
			bodyListView = FindViewById<ListView> (Resource.Id.bodylist);
			bodyItems = new List<TablePositions> ();

			bodyListView.ItemClick += MListView_ItemClick;
			bodyListView.ItemLongClick += MListView_ItemLongClick;


			adapter = new ListViewAdapterMenu (this, bodyItems);
			bodyListView.Adapter = adapter;

			initListView ("SELECT * FROM TablePositions WHERE StatutLivraison = '0' AND typeMission= '"+tyM+"' AND typeSegment= '"+tyS+"'  AND Userandsoft = '"+Data.userAndsoft+"' ORDER BY Ordremission");
		}

		void MListView_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			if(bodyItems[e.Position].imgpath == "SUPPLIV"){
				AndHUD.Shared.ShowError(this, "Cette position a été supprimée de ce groupage", MaskType.Black, TimeSpan.FromSeconds(3));
			}else{
				Intent intent = new Intent (this, typeof(DetailActivity));
				intent.PutExtra("ID",Convert.ToString(bodyItems[e.Position].Id));
				intent.PutExtra("TYPE",type);
				this.StartActivity (intent);
				Finish ();
			}
		}

		void MListView_ItemLongClick (object sender, AdapterView.ItemLongClickEventArgs e)
		{

		}


		void btngrpAll_Click ()
		{
			initListView ("SELECT * FROM TablePositions WHERE StatutLivraison = '0' AND typeMission= '"+tyM+"' AND typeSegment= '"+tyS+"'  AND Userandsoft = '"+Data.userAndsoft+"' ORDER BY Ordremission");
		}

		void btngrp1_Click ()
		{
			initListView ("SELECT * FROM TablePositions WHERE StatutLivraison = '0' AND typeMission= '"+tyM+"' AND typeSegment= '"+tyS+"'  AND Userandsoft = '"+Data.userAndsoft+"'AND groupage='"+Arraygrp[1]+"' ORDER BY Ordremission");
		}

		void btngrp2_Click ()
		{
			initListView ("SELECT * FROM TablePositions WHERE StatutLivraison = '0' AND typeMission= '"+tyM+"' AND typeSegment= '"+tyS+"'  AND Userandsoft = '"+Data.userAndsoft+"'AND groupage='"+Arraygrp[2]+"' ORDER BY Ordremission");
		}

		void btngrp3_Click ()
		{
			initListView ("SELECT * FROM TablePositions WHERE StatutLivraison = '0' AND typeMission= '"+tyM+"' AND typeSegment= '"+tyS+"'  AND Userandsoft = '"+Data.userAndsoft+"'AND groupage='"+Arraygrp[3]+"' ORDER BY Ordremission");
		}

		void btngrp4_Click ()
		{
			initListView ("SELECT * FROM TablePositions WHERE StatutLivraison = '0' AND typeMission= '"+tyM+"' AND typeSegment= '"+tyS+"'  AND Userandsoft = '"+Data.userAndsoft+"'AND groupage='"+Arraygrp[4]+"' ORDER BY Ordremission");
		}

		void btnsearch_Click ()
		{
			AlertDialog.Builder dialog = new AlertDialog.Builder(this);		
			dialog.SetTitle("Rechercher");

			var viewAD = this.LayoutInflater.Inflate (Resource.Layout.boxsearch, null);
			EditText editrecherche =  viewAD.FindViewById<EditText> (Resource.Id.editrecherche);
			dialog.SetView (viewAD);
			dialog.SetCancelable (true);
			dialog.SetNegativeButton("Chercher", delegate {
				initListView("SELECT * FROM TablePositions WHERE  typeMission='"+tyM+"' AND typeSegment='"+tyS+"' AND Userandsoft = '"+Data.userAndsoft+"' AND (numCommande LIKE '%"+editrecherche.Text+"%' OR  villeLivraison LIKE '%"+editrecherche.Text+"%' OR nomPayeur LIKE '%\"+input.Text+\"%'OR CpLivraison LIKE '%"+editrecherche.Text+"%' OR refClient LIKE '%"+editrecherche.Text+"%' OR nomClient LIKE'%"+editrecherche.Text+"%')");
			});
			dialog.SetPositiveButton("Non", delegate {
				AndHUD.Shared.ShowError(this, "Annulée!", AndroidHUD.MaskType.Clear, TimeSpan.FromSeconds(1));
			});
			dialog.Show ();
		}

		void btntrait_Click ()
		{
			Intent intent = new Intent (this, typeof(ListeTraitee));
			intent.PutExtra("TYPE",type);
			this.StartActivity (intent);
//			btngrpAll.Dispose();
//			btngrp1.Dispose();
//			btngrp2.Dispose();
//			btngrp3.Dispose();
//			btngrp4.Dispose();
//			btnsearch.Dispose();
//			btntrait.Dispose();
//
//			bodyListView.Dispose();
//
//			btngrpAll.Click -= delegate {
//				btngrpAll_Click();
//			};
//			btngrp1.Click -= delegate {
//				btngrp1_Click();
//			};
//			btngrp2.Click -= delegate {
//				btngrp2_Click();
//			};
//			btngrp3.Click -= delegate {
//				btngrp3_Click();
//			};
//			btnsearch.Click -= delegate {
//				btnsearch_Click();
//			};
//			btntrait.Click -= delegate {
//				btntrait_Click();
//			};

			Finish ();
		}

		public void initListView (string requete)
		{
			string dbPath = System.IO.Path.Combine (System.Environment.GetFolderPath
				(System.Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection (dbPath);

			bodyItems.Clear ();
			var table = db.Query<TablePositions> (requete);
			foreach (var item in table) {

				bodyItems.Add (new TablePositions () {
					Id = item.Id,
					numCommande = item.numCommande,
					typeMission = item.typeMission,
					typeSegment = item.typeSegment,
					StatutLivraison = item.StatutLivraison,
					nomClient = item.nomClient,
					refClient = item.refClient,
					nomPayeur = item.nomPayeur,
					nbrColis = item.nbrColis,
					nbrPallette = item.nbrPallette,
					poids = item.poids,
					instrucLivraison = item.instrucLivraison,
					adresseLivraison = item.adresseLivraison,
					CpLivraison = item.CpLivraison,
					villeLivraison = item.villeLivraison,
					adresseExpediteur = item.adresseExpediteur,
					CpExpediteur = item.CpExpediteur,
					villeExpediteur = item.villeLivraison,
					nomClientLivraison = item.nomClientLivraison,
					villeClientLivraison = item.villeClientLivraison,
					imgpath = item.imgpath,

				});

				RunOnUiThread(() => adapter.NotifyDataSetChanged());
			}

		}

		public override void OnBackPressed ()
		{		
			Intent intent = new Intent (this, typeof(HomeActivity));
			this.StartActivity (intent);
			Finish ();
		}
	}
}

