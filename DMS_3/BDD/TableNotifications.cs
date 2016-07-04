using System;
using SQLite;
namespace DMS_3
{
	public class TableNotifications
	{
		[PrimaryKey, AutoIncrement, Column("_Id")]
		public int Id { get; set; }
		public int statutNotificationMessage { get; set; }
		public DateTime dateNotificationMessage { get; set; }
		public int numMessage { get; set; }
		public string numCommande { get; set; }
		public string groupage { get; set; }
	}
}

