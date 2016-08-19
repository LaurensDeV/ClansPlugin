using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using ClanAPI;
using ClanAPI.DB;

namespace ClanAPI
{
	public static class Utils
	{
		public static Clan GetClanByName(string name)
		{
			if (ClanDB.Instance.ClanCache.ContainsKey(name))
				return ClanDB.Instance.ClanCache[name];
			return null;
		}

		public static Member GetMember(this TSPlayer ts)
		{
			return ts.GetData<Member>(ClanDB.MemberKey);
		}

		public static bool IsInClan(this TSPlayer ts)
		{
			Member mbr;
			return (mbr = ts.GetMember()) != null && GetClanByName(mbr.Clan) != null;
		}

		public static Clan GetClan(this TSPlayer ts)
		{
			if (!string.IsNullOrEmpty(ts.GetMember()?.Clan))
				return GetClanByName(ts.GetMember().Clan);
			
			return null;
		}

		public static Color Parse(this string colorString)
		{
			byte[] rgb = new byte[3];
			string[] split = colorString.Split(',');

			if (split.Length != 3 || !byte.TryParse(split[0], out rgb[0]) || !byte.TryParse(split[1], out rgb[1]) || !byte.TryParse(split[2], out rgb[2]))
				throw new Exception($"Error parsing color \"{colorString}\", required format: \"255,255,255\".");

			return new Color(rgb[0], rgb[1], rgb[2]);
		}
	}
}
