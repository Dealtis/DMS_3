using System;
using SQLite;

namespace DMS_3
{
	public class TableLogService
	{		
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public String exeption{ get; set; }
		public DateTime date { get; set; }
		public String description { get; set; }
	}
}


