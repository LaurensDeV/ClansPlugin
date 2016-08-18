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
	}

	[Flags]
	public enum Rank : int
	{
		Owner = 0x10,
		Admin = 0x8,
		Moderator = 0x4,
		Helper = 0x2,
		Recruit = 0x0
	}
}