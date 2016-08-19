using System;
using SQLite;
namespace DMS_3
{
	public class TableColis
	{
		[PrimaryKey, AutoIncrement, Column("_Id")]
		public int Id { get; set; }
		public String numColis { get; set; }
		public String numCommande { get; set; }
		public DateTime dateflashage { get; set; }
		public bool flashage { get; set; }
	}
}

