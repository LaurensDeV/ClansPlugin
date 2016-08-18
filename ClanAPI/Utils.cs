using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace ClanAPI
{
	public static class Utils
	{
		public static Member GetMember(this TSPlayer ts)
		{
			return ts.GetData<Member>("member");
		}
	}
}
