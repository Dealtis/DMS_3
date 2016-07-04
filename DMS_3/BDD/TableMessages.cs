using System;
using SQLite;

namespace DMS_3
{
	public class TableMessages
	{		
			[PrimaryKey, AutoIncrement, Column("_Id")]
			public int Id { get; set; }
			public String codeChauffeur{ get; set; }
			public String texteMessage { get; set; }
			public String utilisateurEmetteur { get; set; }
			public int statutMessage { get; set; }
			public DateTime dateImportMessage { get; set; }
			public int typeMessage { get; set; }
			public int numMessage { get; set; }
	}
}


