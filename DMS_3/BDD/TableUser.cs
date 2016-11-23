
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
	[Table ("TableUser")]			
	public class TableUser
	{		
			//Table USER
			[PrimaryKey, AutoIncrement, Column("_Id")]
			public int Id { get; set; }			
			public string user_AndsoftUser { get; set; }			
			public string user_TransicsUser { get; set; }			
			public DateTime user_LoginDate { get; set; }
			public bool user_IsLogin { get; set; }
			public string user_Password { get; set; }
			public string user_UseSigna { get; set; }
			public string user_UsePartic { get; set; }
	}
}

