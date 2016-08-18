using ClanAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace ClansPlugin
{
	public class ClanCommands
	{
		[ClanCommand("motd", Parameters = "[new motd]", Description = "Gets or sets the clan's MotD.")]
		internal static void MotdCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("disband", Parameters = "[confirm]", Description = "Disbands your current clan.")]
		internal static void DisbandCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("prefix", Parameters = "[new prefix]", Description = "Gets or sets your clan's prefix.")]
		internal static void PrefixCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("color", Parameters = "[new color (255,255,255)]", Description = "Gets or sets your clan's chatcolor.")]
		internal static void ColorCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("promote", Parameters = "<player>", Description = "Promotes a player in your clan.")]
		internal static void PromoteCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("demote", Parameters = "<player>", Description = "Demotes a player in your clan.")]
		internal static void DemoteCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("kick", Parameters = "<player> [reason]", Description = "Kicks a player from your clan.")]
		internal static void KickCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("leave", Description = "Leave your current clan.")]
		internal static void LeaveCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("invite", Parameters = "<player>", Description = "Invite a player to your clan.")]
		internal static void InviteCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("accept", Description = "Accept your current clan invitation.")]
		internal static void AcceptCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("members", Description = "Lists all your clan's members.")]
		internal static void MembersCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("list", Parameters = "[message]", Description = "Lists all the clans on the server.")]
		internal static void ListCommand(CommandArgs args)
		{
			throw new NotImplementedException();
		}

		[ClanCommand("c", Parameters = "(or /c) <message>", Description = "Sends a message to your clan.")]
		internal static void CSayCommand(CommandArgs args)
		{
			Clan clan = args.Player.GetClan();
			if (clan == null)
			{
				args.Player.SendErrorMessage("You are not in a clan!");
				return;
			}
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("You must enter a message.");
				return;
			}
			if (args.Player.mute)
			{
				args.Player.SendErrorMessage("You are muted!");
				return;
			}
			string message = string.Join(" ", args.Parameters);
			args.Player.GetClan().SendMessage("{clans - {0}} {1}: {2}", clan.Name, args.Player.Name, message);
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

		internal static Action<CommandArgs> GetCommand(string name)
		{
			var methods = typeof(ClanCommands).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Where(m => m.GetCustomAttribute(typeof(ClanCommandAttribute)) != null);
			var attributes = methods.Select(m => (m.GetCustomAttribute(typeof(ClanCommandAttribute)) as ClanCommandAttribute));

			foreach (var mi in methods)
			{
				var attrib = (mi.GetCustomAttribute(typeof(ClanCommandAttribute)) as ClanCommandAttribute);

				if (attrib.Name == name)
					return (Action<CommandArgs>)Delegate.CreateDelegate(typeof(Action<CommandArgs>), mi);
			}
			return null;
		}
	}

	public class ClanCommandAttribute : Attribute
	{
		public string Name;
		public string Parameters;
		public string Description;

		public ClanCommandAttribute(string name)
		{
			this.Name = name;
		}
	}
}
