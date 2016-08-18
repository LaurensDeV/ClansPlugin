using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace ClanAPI.DB
{
	public class ClanDB
	{
		private static IDbConnection connection;
		private static ClanDB _instance;
		public static ClanDB Instance
		{
			get
			{
				if (_instance == null)
					_instance = new ClanDB();
				return _instance;
			}
		}

		public void Initialize()
		{
			switch (TShock.Config.StorageType.ToLower())
			{
				case "mysql":
					string[] dbHost = TShock.Config.MySqlHost.Split(':');
					connection = new MySqlConnection()
					{
						ConnectionString = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
							dbHost[0],
							dbHost.Length == 1 ? "3306" : dbHost[1],
							TShock.Config.MySqlDbName,
							TShock.Config.MySqlUsername,
							TShock.Config.MySqlPassword)

					};
					break;

				case "sqlite":
					string sql = Path.Combine(TShock.SavePath, "Clans.sqlite");
					connection = new SqliteConnection(string.Format("uri=file://{0},Version=3", sql));
					break;
			}

			InsertTable<Clan>();
			InsertTable<Member>();
		}

		public Member GetMemberByName(string name)
		{
			return GetObjectFromDatabase<Member>("SELECT * FROM Members WHERE Name = @0", name);
		}

		public T GetObjectFromDatabase<T>(string query, params object[] args)
		{
			var fields = typeof(T).GetFields().Where(f => f.GetCustomAttribute(typeof(DBColumnAttribute)) != null);

			var instance = (T)Activator.CreateInstance(typeof(T));

			using (QueryResult reader = connection.QueryReader(query, args))
			{
				if (reader.Read())
				{
					foreach (var finfo in fields)
					{
						var attrib = (finfo.GetCustomAttribute(typeof(DBColumnAttribute)) as DBColumnAttribute);
						instance.GetType().GetField(attrib.Name).SetValue(instance, Convert.ChangeType(reader.Get<object>(attrib.Name), finfo.GetType()));
					}
					return instance;
				}
			}
			return default(T);
		}

		public void InsertTable<T>()
		{
			var fields = typeof(T).GetFields().Where(f => f.GetCustomAttribute(typeof(DBColumnAttribute)) != null);

			List<SqlColumn> columns = new List<SqlColumn>();

			foreach (var finfo in fields)
			{
				var attrib = (finfo.GetCustomAttribute(typeof(DBColumnAttribute)) as DBColumnAttribute);
				columns.Add(attrib.GetColumn());
			}

			SqlTableCreator sqlcreator = new SqlTableCreator(connection, connection.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());

			var tableName = (typeof(T).GetCustomAttribute(typeof(DBTableAttribute)) as DBTableAttribute).TableName;

			sqlcreator.EnsureTableStructure(new SqlTable(tableName, columns.ToArray()));
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class DBColumnAttribute : Attribute
	{
		public string Name;
		public int Length;
		public MySqlDbType Type;
		public bool Primary;
		public bool Unique;
		public bool AutoIncrement;
		public string DefaultValue;
		public bool NotNull;

		public DBColumnAttribute(string name, MySqlDbType type)
		{
			this.Name = name;
			this.Type = type;
		}

		public SqlColumn GetColumn()
		{
			int? temp = null;
			if (Length != 0)
				temp = Length;

			return new SqlColumn(Name, Type, temp) { AutoIncrement = AutoIncrement, DefaultValue = DefaultValue, NotNull = NotNull, Primary = Primary, Unique = Unique };
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class DBTableAttribute : Attribute
	{
		public string TableName;
		public DBTableAttribute(string tableName)
		{
			this.TableName = tableName;
		}
	}
}
