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
			this.Name = name;
			ClanDB.Instance.UpdateName(this);
		}

		public void SetDescription(string description)
		{
			this.Description = description;
			ClanDB.Instance.UpdateDescription(this);
		}

		public void SetSuffix(string suffix)
		{
			this.Suffix = suffix;
			ClanDB.Instance.UpdateSuffix(this);
		}

		public void SetPrefix(string prefix)
		{
			this.Prefix = prefix;
			ClanDB.Instance.UpdatePrefix(this);
		}

		public void SetChatColor(string color)
		{
			this.ChatColor = color;
			ClanDB.Instance.UpdateChatColor(this);
		}

		public void SetMotd(string motd)
		{
			this.Motd = motd;
			ClanDB.Instance.UpdateMotd(this);
		}

		public void SetRank(int index, string name)
		{
			this.RankNames[index] = name;
			ClanDB.Instance.UpdateRanks(this);
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
