using ClanAPI.DB;
using MySql.Data.MySqlClient;

namespace ClanAPI
{
	[DBTable("Members")]
	public class Member
	{
		[DBColumn("Username", MySqlDbType.VarChar, Primary = true)]
		public string Name { get; set; }
	}
}