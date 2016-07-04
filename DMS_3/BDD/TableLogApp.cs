using System;
using SQLite;

namespace DMS_3
{
	public class TableLogApp
	{		
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public String exeption{ get; set; }
		public DateTime date { get; set; }
		public String description { get; set; }
	}
}

