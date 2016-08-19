using ClanAPI.DB;
using MySql.Data.MySqlClient;
using System;

namespace ClanAPI
{
	[DBTable("Members")]
	public class Member
	{
		[DBColumn("Username", MySqlDbType.VarChar, Primary = true)]
		public string Username { get; set; }

		[DBColumn("Clan", MySqlDbType.VarChar)]
		public string Clan { get; set; }

		[DBColumn("Rank", MySqlDbType.Int32)]
		public Rank Rank { get; set; }
	}

	//For some reason the database stores Rank as an Int64, 
	//so we need the enum to be 64bits in order to not crash on Convert.ChangeType
	public enum Rank : long
	{
		None = -1,
		Recruit,
		Helper,
		Moderator,
		Admin,
		Owner
	}
}