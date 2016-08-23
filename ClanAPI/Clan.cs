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
		public string Name { get; private set; }

		[DBColumn("Description", MySqlDbType.VarChar)]
		public string Description { get; private set; }

		[DBColumn("Prefix", MySqlDbType.VarChar)]
		public string Prefix { get; private set; }

		[DBColumn("Suffix", MySqlDbType.VarChar)]
		public string Suffix { get; private set; }

		[DBColumn("ChatColor", MySqlDbType.VarChar)]
		public string ChatColor { get; private set; }

		[DBColumn("Motd", MySqlDbType.VarChar)]
		public string Motd { get; private set; }

		[DBColumn("Ranks", MySqlDbType.VarChar)]
		public string Ranks
		{
			get { return JsonConvert.SerializeObject(RankNames); }
			set { RankNames = JsonConvert.DeserializeObject<string[]>(value); }
		}
		public string[] RankNames { get; set; }

		public void SetName(string name)
		{
			ClanDB.Instance.SetName(this, name);
			this.Name = name;
		}

		public void SetDescription(string description)
		{
			ClanDB.Instance.SetDescription(this, description);
			this.Description = description;
		}

		public void SetSuffix(string suffix)
		{
			ClanDB.Instance.SetSuffix(this, suffix);
			this.Suffix = suffix;
		}

		public void SetPrefix(string prefix)
		{
			ClanDB.Instance.SetPrefix(this, prefix);
			this.Prefix = prefix;
		}

		public void SetChatColor(string color)
		{
			ClanDB.Instance.SetChatColor(this, color);
			this.ChatColor = color;
		}

		public void SetMotd(string motd)
		{
			ClanDB.Instance.SetMotd(this, motd);
			this.Motd = motd;
		}

		public void SetRanks(string ranks)
		{
			//NOT IMPLEMENTED, TODO
			//ClanDB.Instance.(this, name);
			this.Ranks = ranks;
		}

		public Clan() { }

		public Clan(string name)
		{
			Name = name;
			Description = "No description.";
			Suffix = "";
			Prefix = $"{{{name}}}";
			ChatColor = DefaultChatColor;
			Motd = "";
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
				Color color;

				if(Utils.ParseColor(ChatColor, out color))
					ts.SendMessage(string.Format(msg, args), color);
				else
					Console.WriteLine($"Error parsing color {ChatColor} from clan {Name}");
			}
		}
	}
}
