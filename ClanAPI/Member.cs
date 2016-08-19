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

		[DBColumn("Rank", MySqlDbType.Int64)]
		public Rank Rank { get; set; }
	}

	public enum Rank : long
	{
		Recruit,
		Helper,
		Moderator,
		Admin,
		Owner
	}
}