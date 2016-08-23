﻿using ClanAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using ClanAPI.DB;

namespace ClansPlugin
{
	public class ClanCommands
	{
		[ClanCommand("create", Parameters = "<name>", Description = "Create a clan.")]
		internal static void CreateCommand(CommandArgs args)
		{
			if (!args.Player.HasPermission(Permission.CreateClan))
			{
				args.Player.SendErrorMessage("You do not have permission to create a clan.");
				return;
			}
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage($"Invalid syntax! Proper syntax: {Commands.Specifier}clan create <name>");
				return;
			}
			if (args.Player.IsInClan())
			{
				args.Player.SendErrorMessage("You are already in a clan!");
				return;
			}
			string clanName = string.Join(" ", args.Parameters);
			if (clanName.Length > 30)
			{
				args.Player.SendErrorMessage("The clan name cannot be longer than 30 characters.");
				return;
			}
			Task.Run(async () =>
			{
				if (await ClanDB.Instance.AddClan(args.Player, clanName))
					args.Player.SendInfoMessage("Your clan was created successfully.");
				else
					args.Player.SendErrorMessage("A clan with this name already exists.");
			});
		}

		[ClanCommand("disband", Parameters = "[confirm]", Description = "Disbands your current clan.", Permission = Rank.Owner)]
		internal static void DisbandCommand(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Player.SendInfoMessage("Are you sure you want to disband your clan?");
				args.Player.SendInfoMessage($"type \"{Commands.Specifier}clan disband confirm\" to disband your clan.");
				return;
			}
			if (args.Parameters[0] != "confirm")
			{
				args.Player.SendErrorMessage($"Invalid syntax: proper syntax: {Commands.Specifier}clan disband [confirm]");
				return;
			}
			ClanDB.Instance.RemoveClan(args.Player, args.Player.GetClan());
		}

		[ClanCommand("motd", Parameters = "[new motd]", Description = "Gets or sets the clan's MotD.", Permission = Rank.Owner)]
		internal static void MotdCommand(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Player.SendInfoMessage($"The current motd is: \"{args.Player.GetClan().Motd}\".");
				args.Player.SendInfoMessage($"Type {Commands.Specifier}clan motd [new motd] to change the motd");
				return;
			}
			string motd = string.Join(" ", args.Parameters);
			if (motd.Length > 100)
			{
				args.Player.SendErrorMessage("The motd cannot be longer than 100 characters.");
				return;
			}
			Clan clan = args.Player.GetClan();
			clan.SetMotd(motd);
			args.Player.GetClan().SendMessage("{0} changed the motd to {1}", args.Player.Name, motd);
		}

		[ClanCommand("prefix", Parameters = "[new prefix]", Description = "Gets or sets your clan's prefix.", Permission = Rank.Owner)]
		internal static void PrefixCommand(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Player.SendInfoMessage($"The current prefix is: \"{args.Player.GetClan().Prefix}\".");
				args.Player.SendInfoMessage($"Type {Commands.Specifier}clan prefix [new prefix] to change the prefix");
				return;
			}
			string prefix = string.Join(" ", args.Parameters);
			if (prefix.Length > 30)
			{
				args.Player.SendErrorMessage("The prefix cannot be longer than 30 characters.");
				return;
			}
			if (prefix.Length == 0)
			{
				args.Player.SendErrorMessage("The prefix cannot be empty!");
			}
			Clan clan = args.Player.GetClan();
			clan.SetPrefix(prefix);
			args.Player.GetClan().SendMessage("{0} changed the prefix to {1}", args.Player.Name, prefix);
		}

		[ClanCommand("color", Parameters = "[new color (255,255,255)]", Description = "Gets or sets your clan's chatcolor.", Permission = Rank.Owner)]
		internal static void ColorCommand(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Player.SendInfoMessage($"The current chatcolor is: \"{args.Player.GetClan().ChatColor}\".");
				return;
			}
			Color clr;
			string color = string.Join(" ", args.Parameters);
			if (!ClanAPI.Utils.ParseColor(color, out clr))
			{
				args.Player.SendErrorMessage($"Invalid syntax! proper syntax: {Commands.Specifier}clan color <255,255,255>");
				return;
			}
			Clan clan = args.Player.GetClan();
			clan.SetChatColor(color);
			args.Player.GetClan().SendMessage("{0} changed the chatcolor to {1}", args.Player.Name, color);
		}

		[ClanCommand("promote", Parameters = "<player>", Description = "Promotes a player in your clan.", Permission = Rank.Owner)]
		internal static void PromoteCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("demote", Parameters = "<player>", Description = "Demotes a player in your clan.", Permission = Rank.Owner)]
		internal static void DemoteCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("kick", Parameters = "<player> [reason]", Description = "Kicks a player from your clan.", Permission = Rank.Admin)]
		internal static void KickCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("mute", Parameters = "<player> [reason]", Description = "Mute a player in the clan.", Permission = Rank.Moderator)]
		internal static void MuteCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("leave", Description = "Leave your current clan.", Permission = Rank.Recruit)]
		internal static void LeaveCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("invite", Parameters = "<player>", Description = "Invite a player to your clan.", Permission = Rank.Helper)]
		internal static void InviteCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("accept", Description = "Accept your current clan invitation.")]
		internal static void AcceptCommand(CommandArgs args)
		{
			string clan = args.Player.GetData<string>("invite");

			if (string.IsNullOrWhiteSpace(clan))
			{
				args.Player.SendErrorMessage("You do not have a pending clan invite!");
				return;
			}
			ClanDB.Instance.AddMember(args.Player, clan);
		}

		[ClanCommand("members", Description = "Lists all your clan's members.", Permission = Rank.Recruit)]
		internal static void MembersCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("list", Parameters = "[message]", Description = "Lists all the clans on the server.")]
		internal static void ListCommand(CommandArgs args)
		{
			int pageNum;
			if (!PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out pageNum))
				return;

			PaginationTools.SendPage(args.Player, pageNum, PaginationTools.BuildLinesFromTerms(
				ClanDB.Instance.Clans,
				(o) => {
					var clan = (Clan)o;
					var msg = $"{clan.Name} - {clan.Description}";
					msg += msg.Length < 80 ? new string(' ', 80 - msg.Length) : "";
					return msg;
				}
			), new PaginationTools.Settings()
			{
				HeaderFormat = "Clan List ({0}/{1})",
				FooterFormat = "Type {0}clan list {{0}} for more.".SFormat(TShock.Config.CommandSpecifier),
				NothingToDisplayString = "There are currently no clans to list.",
			});
		}

		[ClanCommand("c", Parameters = "(or /c) <message>", Description = "Sends a message to your clan.", Permission = Rank.Recruit)]
		internal static void CSayCommand(CommandArgs args)
		{
			Clan clan = args.Player.GetClan();
			if (!args.Player.IsInClan())
			{
				args.Player.SendErrorMessage("You are not in a clan!");
				return;
			}
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("You must enter a message.");
				return;
			}
			if (args.Player.mute || args.Player.GetMember().Muted)
			{
				args.Player.SendErrorMessage("You are muted!");
				return;
			}
			string message = string.Join(" ", args.Parameters);
			clan.SendMessage("{0} {1} {2}: {3} {4}", clan.Prefix, clan.RankNames[(int)args.Player.GetMember().Rank], args.Player.Name, message, clan.Suffix);
		}

		[ClanCommand("help", Description = "gives info on clan commands.")]
		internal static void HelpCommand(CommandArgs args)
		{
			int pageNum;
			if (!PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out pageNum))
				return;

			var methods = typeof(ClanCommands).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Where(m => m.GetCustomAttribute(typeof(ClanCommandAttribute)) != null);
			var attributes = methods.Select(m => (m.GetCustomAttribute(typeof(ClanCommandAttribute)) as ClanCommandAttribute));
			var help = attributes.Select(a => $"{Commands.Specifier}clan {a.Name} {a.Parameters} - {a.Description}").ToList();
			PaginationTools.SendPage(args.Player, pageNum, help, new PaginationTools.Settings()
			{
				HeaderFormat = "Clan Help ({0}/{1})",
				FooterFormat = "Type {0}clan help {{0}} for more.".SFormat(TShock.Config.CommandSpecifier)
			});
		}

		internal static void Execute(string name, CommandArgs args)
		{
			var methods = typeof(ClanCommands).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Where(m => m.GetCustomAttribute(typeof(ClanCommandAttribute)) != null);
			var attributes = methods.Select(m => (m.GetCustomAttribute(typeof(ClanCommandAttribute)) as ClanCommandAttribute));

			foreach (var mi in methods)
			{
				var attrib = (mi.GetCustomAttribute(typeof(ClanCommandAttribute)) as ClanCommandAttribute);

				if (attrib.Name == name)
				{
					Action<CommandArgs> action = (Action<CommandArgs>)Delegate.CreateDelegate(typeof(Action<CommandArgs>), mi);

					Member mbr = args.Player.GetMember();

					Rank rank = mbr?.Rank ?? Rank.None;

					if (HasPermission(rank, attrib.Permission))
					{
						try
						{
							action.Invoke(args);
						}
						catch (NotImplementedException)
						{
							args.Player.SendErrorMessage("This command is not implemented yet!");
						}
					}
					else if (mbr == null)
						args.Player.SendErrorMessage("You are not in a clan!");
					else
						args.Player.SendErrorMessage("Your rank is not high enough to use this command!");
					return;
				}
			}
			Execute("help", args);
		}

		internal static bool HasPermission(Rank rank, Rank required)
		{
			return (int)rank >= (int)required;
		}
	}

	public class ClanCommandAttribute : Attribute
	{
		public Rank Permission;
		public string Name;
		public string Parameters;
		public string Description;

		public ClanCommandAttribute(string name)
		{
			this.Name = name;
			Permission = Rank.None;
		}
	}
}
