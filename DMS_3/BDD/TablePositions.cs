
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

using SQLite;

namespace DMS_3
{
	[Table ("TablePositions")]			
	public class TablePositions
	{		
		//Table Positions

		[PrimaryKey, AutoIncrement, Column("_Id")]
		public int Id { get; set; }
		public String codeLivraison { get; set; }
		public String numCommande { get; set; }
		public String nomClient { get; set; }
		public String refClient { get; set; }
		public String nomPayeur { get; set; }
		public String adresseLivraison { get; set; }
		public String CpLivraison { get; set; }
		public String villeLivraison { get; set; }
		public String dateHeure { get; set; }
		public String dateExpe { get; set; }
		public String nbrColis { get; set; }
		public String nbrPallette { get; set; }
		public String poids { get; set; }
		public String adresseExpediteur { get; set; }
		public String CpExpediteur { get; set; }
		public String villeExpediteur { get; set; }
		public String nomExpediteur { get; set; }
		public String StatutLivraison { get; set; }
		public String instrucLivraison { get; set; }
		public String groupage { get; set; }
		public String ADRLiv { get; set; }
		public String ADRGrp { get; set; }
		public String planDeTransport { get; set; }
		public String typeMission { get; set; }
		public String typeSegment { get; set; }
		public int idSegment { get; set; }
		public String CR { get; set; }
		public String nomClientLivraison{ get; set; }
		public String villeClientLivraison{ get; set; }
		public String Datemission{ get; set; }
		public int Ordremission{ get; set; }
		public String Userandsoft{ get; set; }
		public String remarque { get; set; }
		public String codeAnomalie { get; set; }
		public String libeAnomalie { get; set; }
		public String imgpath{ get; set; }
		public int dateBDD { get; set; }
	}
}


