
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
	[Table ("TableStatutPositions")]			
	public class TableStatutPositions
	{		
		//Table StatutPositions
		[PrimaryKey, AutoIncrement, Column("_Id")]
		public int Id { get; set; }
		public String codesuiviliv { get; set; }
		public String statut { get; set; }
		public String commandesuiviliv { get; set; }
		public String datesuiviliv { get; set; }
		public String libellesuiviliv { get; set; }
		public String memosuiviliv { get; set; }
		public String datajson { get; set; }


	}
}


