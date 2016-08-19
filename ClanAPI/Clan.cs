using ClanAPI.DB;
using MySql.Data.MySqlClient;
using System;
using TShockAPI;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ClanAPI
{
	[DBTable("Clans")]
	public class Clan
	{
		public const string DefaultChatColor = "235,78,30";

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

		[DBColumn("Ranks", MySqlDbType.VarChar)]
		public string Ranks
		{
			get { return JsonConvert.SerializeObject(RankNames); }
			set { RankNames = JsonConvert.DeserializeObject<string[]>(value); }
		}

		public string[] RankNames { get; set; }

		public Clan() { }

		public Clan(string name)
		{
			Name = name;
			Description = "";
			Suffix = "";
			Prefix = "";
			ChatColor = DefaultChatColor;
			RankNames = new string[]
			{
				"(Recruit)",
				"(Helper)",
				"(Moderator)",
				"(Admin)",
				"(Owner)"
			};
		}

		public void SendMessage(string msg, params string[] args)
		{
			foreach (TSPlayer ts in TShock.Players)
			{
				if (ts == null || ts.GetClan() != this)
					continue;

				ts.SendMessage(string.Format(msg, args), ChatColor.Parse());
			}
		}
	}
}
