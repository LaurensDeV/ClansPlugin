using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using ClanAPI;
using ClanAPI.DB;

namespace ClansPlugin
{
	[ApiVersion(1, 23)]
	public class ClanMain : TerrariaPlugin
	{
		public override string Author => "Laurens";
		public override string Description => "Clans plugin";
		public override string Name => "Clans";
		public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

		public override void Initialize()
		{		
			ClanDB.Instance.Initialize();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		public ClanMain(Main game) : base(game)
		{
			Order = 0;
		}
	}
}
