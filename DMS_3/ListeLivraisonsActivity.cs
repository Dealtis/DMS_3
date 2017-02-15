using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Graphics;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using DMS_3.BDD;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using RaygunClient = Mindscape.Raygun4Net.RaygunClient;

namespace DMS_3
{

	[Activity(Label = "ListeLivraisonsActivity", Theme = "@style/MyTheme.Base")]
	public class ListeLivraisonsActivity : AppCompatActivity
	{
		private List<TablePositions> bodyItems;
		private ListView bodyListView;
		private ListViewAdapterMenu adapter;
		LinearLayout btnsearch;
		LinearLayout layout_groupage;
		Button btntrait;
		string type;
		string tyS;
		string tyM;
		string trait;
		List<TablePositions> grp;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.ListeLivraisons);

			//declaration des clicks btns
			//btngrpAll = FindViewById<Button>(Resource.Id.btn_all);
			btnsearch = FindViewById<LinearLayout>(Resource.Id.btn_search);
			btntrait = FindViewById<Button>(Resource.Id.btn_traite);
			layout_groupage = FindViewById<LinearLayout>(Resource.Id.layout_groupage);



			btntrait.Click += delegate
			{
				btntrait_Click();
			};

			btnsearch.Click += delegate
			{
				btnsearch_Click();
			};
		}

		protected override void OnResume()
		{
			base.OnResume();
			try
			{
				initOnResume();
			}
			catch (Exception ex)
			{
				RaygunClient.Current.SendInBackground(ex);Xamarin.Insights.Report(ex);
			}
		}

		void initOnResume()
		{
			layout_groupage.RemoveAllViews();
			type = Intent.GetStringExtra("TYPE");
			if (type == "RAM")
			{
				tyS = "RAM";
				tyM = "C";
			}
			else {
				tyS = "LIV";
				tyM = "L";
			}
			trait = Intent.GetStringExtra("TRAIT");

			if (trait == "true")
			{
				btntrait.Text = "Livraisons";
				if (type == "RAM")
				{
					btntrait.Text = "Ramasses";
				}
			}
			DBRepository dbr = new DBRepository();
			//Mise dans un Array des Groupages
			if (trait == "false")
			{
				try
				{
					
					grp = dbr.QueryGRP("SELECT SUM(poidsADR) as poidsADR,SUM(poidsQL) as poidsQL, groupage FROM TablePositions WHERE StatutLivraison = ? AND typeMission= ? AND typeSegment= ?  AND Userandsoft = ?  GROUP BY groupage", tyM, tyS, Data.userAndsoft);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					RaygunClient.Current.SendInBackground(ex);Xamarin.Insights.Report(ex);
				}
			}
			else
			{
				try
				{
					grp = dbr.QueryGRPTRAIT("SELECT SUM(poidsADR) as poidsADR,SUM(poidsQL) as poidsQL, groupage FROM TablePositions WHERE StatutLivraison = ? AND typeMission= ? AND typeSegment= ?  AND Userandsoft = ? OR StatutLivraison = ? AND typeMission= ? AND typeSegment= ?  AND Userandsoft = ?  GROUP BY groupage", tyM, tyS, Data.userAndsoft, tyM, tyS, Data.userAndsoft);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					RaygunClient.Current.SendInBackground(ex);Xamarin.Insights.Report(ex);
				}

			}
			var allButton = new Button(this);
			allButton.Text = "#";
			allButton.SetTextColor(Color.DarkGray);
			allButton.SetTextSize(Android.Util.ComplexUnitType.Sp, 12);
			allButton.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent, .2f);
			allButton.SetBackgroundResource(Resource.Drawable.button_white_background);
			layout_groupage.AddView(allButton);
			allButton.Click += delegate
			{
				btngrpAll_Click();
			};
			foreach (var item in grp)
			{
				var aButton = new Button(this);
				aButton.Text = item.groupage;
				aButton.SetTextColor(Color.DarkGray);
				aButton.SetTextSize(Android.Util.ComplexUnitType.Sp, 12);

				aButton.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent, .1f);

				string poidsADR = item.poidsADR.Replace(".", ",");
				if (Convert.ToDouble(poidsADR) >= 1000)
				{
					aButton.SetBackgroundResource(Resource.Drawable.adr_background);
				}
				else
				{
					string poidsQL = item.poidsQL.Replace(".", ",");
					if (Convert.ToDouble(poidsQL) >= 8000)
					{
						aButton.SetBackgroundResource(Resource.Drawable.row_valide_background);
					}
					else
					{
						aButton.SetBackgroundResource(Resource.Drawable.button_white_background);
					}
				}

				aButton.Click += delegate
				{
					if (trait == "false")
					{
						initListView("SELECT * FROM TablePositions WHERE StatutLivraison = '0' AND typeMission= '" + tyM + "' AND typeSegment= '" + tyS + "'  AND Userandsoft = '" + Data.userAndsoft + "'AND groupage='" + item.groupage + "' ORDER BY Ordremission");
					}
					else
					{
						initListView("SELECT * FROM TablePositions WHERE StatutLivraison = '1' AND typeMission= '" + tyM + "' AND typeSegment= '" + tyS + "'  AND Userandsoft = '" + Data.userAndsoft + "'AND groupage='" + item.groupage + "' OR StatutLivraison = '2' AND typeMission= '" + tyM + "' AND typeSegment= '" + tyS + "'  AND Userandsoft = '" + Data.userAndsoft + "'AND groupage='" + item.groupage + "' ORDER BY Ordremission");
					}
				};
				layout_groupage.AddView(aButton);
			}

			//LISTVIEW
			bodyListView = FindViewById<ListView>(Resource.Id.bodylist);
			bodyItems = new List<TablePositions>();
			bodyListView.ItemClick += MListView_ItemClick;
			adapter = new ListViewAdapterMenu(this, bodyItems);
			bodyListView.Adapter = adapter;

			if (trait == "false")
			{
				initListView("SELECT * FROM TablePositions WHERE StatutLivraison = '0' AND typeMission= '" + tyM + "' AND typeSegment= '" + tyS + "'  AND Userandsoft = '" + Data.userAndsoft + "' ORDER BY Ordremission");
			}
			else
			{
				initListView("SELECT * FROM TablePositions WHERE StatutLivraison = '1' AND typeMission= '" + tyM + "' AND typeSegment= '" + tyS + "'  AND Userandsoft = '" + Data.userAndsoft + "' OR StatutLivraison = '2' AND typeMission= '" + tyM + "' AND typeSegment= '" + tyS + "'  AND Userandsoft = '" + Data.userAndsoft + "' ORDER BY Ordremission");
			}
		}
		void MListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			Intent intent = new Intent(this, typeof(DetailActivity));
			intent.PutExtra("ID", Convert.ToString(bodyItems[e.Position].Id));
			intent.PutExtra("TYPE", type);
			intent.PutExtra("TRAIT", trait);
			this.StartActivity(intent);
			Finish();
		}

		void btngrpAll_Click()
		{
			if (trait == "false")
			{
				initListView("SELECT * FROM TablePositions WHERE StatutLivraison = '0' AND typeMission= '" + tyM + "' AND typeSegment= '" + tyS + "'  AND Userandsoft = '" + Data.userAndsoft + "' ORDER BY Ordremission");
			}
			else
			{
				initListView("SELECT * FROM TablePositions WHERE StatutLivraison = '1' AND typeMission= '" + tyM + "' AND typeSegment= '" + tyS + "'  AND Userandsoft = '" + Data.userAndsoft + "' OR StatutLivraison = '2' AND typeMission= '" + tyM + "' AND typeSegment= '" + tyS + "'  AND Userandsoft = '" + Data.userAndsoft + "' ORDER BY Ordremission");
			}
		}

		void btnsearch_Click()
		{
			AlertDialog.Builder dialog = new AlertDialog.Builder(this);
			dialog.SetTitle("Rechercher");

			var viewAD = this.LayoutInflater.Inflate(Resource.Layout.boxsearch, null);
			EditText editrecherche = viewAD.FindViewById<EditText>(Resource.Id.editrecherche);
			dialog.SetView(viewAD);
			dialog.SetCancelable(true);
			dialog.SetNegativeButton("Chercher", delegate
			{
				initListView("SELECT * FROM TablePositions WHERE  typeMission='" + tyM + "' AND typeSegment='" + tyS + "' AND Userandsoft = '" + Data.userAndsoft + "' AND (numCommande LIKE '%" + editrecherche.Text + "%' OR  villeLivraison LIKE '%" + editrecherche.Text + "%' OR nomPayeur LIKE '%\"+input.Text+\"%'OR CpLivraison LIKE '%" + editrecherche.Text + "%' OR refClient LIKE '%" + editrecherche.Text + "%' OR nomClient LIKE'%" + editrecherche.Text + "%')");
			});
			dialog.SetPositiveButton("Non", delegate
			{
				AndHUD.Shared.ShowError(this, "Annulée!", AndroidHUD.MaskType.Clear, TimeSpan.FromSeconds(1));
			});
			dialog.Show();
		}

		void btntrait_Click()
		{
			Intent intent = new Intent(this, typeof(ListeLivraisonsActivity));
			intent.PutExtra("TYPE", type);
			if (trait == "true")
			{
				intent.PutExtra("TRAIT", "false");
			}
			else
			{
				intent.PutExtra("TRAIT", "true");
			}

			this.StartActivity(intent);

			Finish();
		}

		public void initListView(string requete)
		{
			try
			{
				bodyItems.Clear();
				DBRepository dbr = new DBRepository();
				var table = dbr.QueryPositions(requete);
				foreach (var item in table)
				{
					bodyItems.Add(new TablePositions()
					{
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
						poidsQL = item.poidsQL,
						poidsADR = item.poidsADR,
						positionPole = item.positionPole,
						CR = item.CR,
						ASSIGNE = item.ASSIGNE
					});

					RunOnUiThread(() => adapter.NotifyDataSetChanged());
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				RaygunClient.Current.SendInBackground(ex);Xamarin.Insights.Report(ex);
			}
		}

		public override void OnBackPressed()
		{
			Intent intent = new Intent(this, typeof(HomeActivity));
			this.StartActivity(intent);
			Finish();
		}
	}
}

