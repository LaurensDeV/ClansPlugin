﻿using System;
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
using ClanAPI.Hooks;

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
#pragma warning disable
			ClanDB.Instance.Initialize();

			Commands.ChatCommands.Add(new Command(ClanCommand, "clan"));
			Commands.ChatCommands.Add(new Command(ClanCommands.CSayCommand, "c"));

			Commands.ChatCommands.Add(new Command(((e) => {
				e.Player.SendInfoMessage("In clan: " + e.Player.IsInClan());
				e.Player.SendInfoMessage("Is Member: " + (e.Player.GetMember() != null));
			}), "clantest"));

			PlayerHooks.PlayerPostLogin += PlayerHooks_PlayerPostLogin;
			PlayerHooks.PlayerLogout += PlayerHooks_PlayerLogout;


			ClanHooks.ClanCreated += ClanHooks_ClanCreated;
		}

		private void ClanHooks_ClanCreated(ClanCreatedEventArgs args)
		{
			TSPlayer.All.SendInfoMessage($"A new clan ({args.Clan.Name}) has been created by {args.Player.Name}!");
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
			if (!args.Player.IsLoggedIn)
			{
				args.Player.SendErrorMessage("You need to be logged in to use this command!");
				return;
			}
			string cmd = args.Parameters.Count > 0 ? args.Parameters[0] : "help";
			List<string> newargs = args.Parameters.Count == 0 ? args.Parameters : args.Parameters.GetRange(1, args.Parameters.Count - 1);
			CommandArgs newCmdArgs = new CommandArgs(args.Message.Remove(0, args.Message.IndexOf(' ') + 1), args.Player, newargs);

			ClanCommands.Execute(cmd, newCmdArgs);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		public ClanMain(Main game) : base(game)
		{
			Order = 1;
		}
	}
}
