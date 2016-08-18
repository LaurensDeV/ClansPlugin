using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClanAPI.DB;
using MySql.Data.MySqlClient;

namespace ClanAPI
{
	[DBTable("Clans")]
	public class Clan
	{
		[DBColumn("Name", MySqlDbType.VarChar, Primary = true, Length = 30)]
		public string Name { get; set; }

		[DBColumn("Description", MySqlDbType.VarChar)]
		public string Description { get; set; }

		[DBColumn("Suffix", MySqlDbType.VarChar)]
		public string Suffix { get; set; }

		[DBColumn("Prefix", MySqlDbType.VarChar)]
		public string Prefix { get; set; }

		[DBColumn("ChatColor", MySqlDbType.VarChar)]
		public string ChatColor { get; set; }
	}
}
