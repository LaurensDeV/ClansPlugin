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
using TShockAPI.Hooks;

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

			Commands.ChatCommands.Add(new Command(ClanCommand, "clan"));

			PlayerHooks.PlayerPostLogin += PlayerHooks_PlayerPostLogin;
			PlayerHooks.PlayerLogout += PlayerHooks_PlayerLogout;
		}

		private void PlayerHooks_PlayerPostLogin(PlayerPostLoginEventArgs e)
		{
			ClanDB.Instance.LoadMember(e.Player);
		}

		private void PlayerHooks_PlayerLogout(PlayerLogoutEventArgs e)
		{
			ClanDB.Instance.UnLoadMember(e.Player);
		}

		private void ClanCommand(CommandArgs args)
		{
			string cmd = args.Parameters.Count > 0 ? args.Parameters[0] : "help";
			List<string> newargs = args.Parameters.GetRange(1, args.Parameters.Count - 1);
			CommandArgs newCmdArgs = new CommandArgs(args.Message.Remove(0, args.Message.IndexOf(' ')), args.Player, newargs);

			switch (cmd)
			{
				case "create":
					ClanCommands.MotdCommand(newCmdArgs);
					break;
				case "disband":
					ClanCommands.DisbandCommand(newCmdArgs);
					break;
				case "motd":
					ClanCommands.MotdCommand(newCmdArgs);
					break;
				case "prefix":
					ClanCommands.PrefixCommand(newCmdArgs);
					break;
				case "color":
					ClanCommands.ColorCommand(newCmdArgs);
					break;
				case "promote":
					ClanCommands.PromoteCommand(newCmdArgs);
					break;
				case "demote":
					ClanCommands.DemoteCommand(newCmdArgs);
					break;
				case "kick":
					ClanCommands.KickCommand(newCmdArgs);
					break;
				case "quit":
					ClanCommands.QuitCommand(newCmdArgs);
					break;
				case "leave":
					ClanCommands.LeaveCommand(newCmdArgs);
					break;
				case "invite":
					ClanCommands.InviteCommand(newCmdArgs);
					break;
				case "accept":
					ClanCommands.AcceptCommand(newCmdArgs);
					break;
				case "members":
					ClanCommands.MembersCommand(newCmdArgs);
					break;
				case "list":
					ClanCommands.ListCommand(newCmdArgs);
					break;
				case "csay":
				case "c":
					ClanCommands.CSayCommand(newCmdArgs);
					break;
				case "help":
				default:
					ClanCommands.HelpCommand(newCmdArgs);
					break;
			}
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
