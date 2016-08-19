using ClanAPI.Hooks;
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
		internal Dictionary<string, Clan> ClanCache;
		public const string MemberKey = "member";
		public const string ClanKey = "clan";

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

		[Obsolete("Please do not use this method.")]
		public void Initialize()
		{
			ClanCache = new Dictionary<string, Clan>();
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

			EnsureTableStructure<Clan>();
			EnsureTableStructure<Member>();

			LoadClans();
		}

		public void UnLoadMember(TSPlayer ts)
		{
			ts.RemoveData(MemberKey);
		}

		/// <summary>
		/// Loads a <paramref name="Member"/> from the database. If not found inserts a new <paramref name="Member"/>.
		/// </summary>
		/// <param name="ts"></param>
		public async void LoadMember(TSPlayer ts)
		{
			Member mbr = await GetObjectFromDatabase<Member>("SELECT * FROM Members WHERE Username = @0", ts.Name);
			if (mbr != null)
				ts.SetData(MemberKey, mbr);
		}

		public async void AddMember(TSPlayer ts, string clan, bool owner = false)
		{
			Member mbr = new Member() { Username = ts.Name, Clan = clan, Rank = (owner ? Rank.Owner : Rank.Recruit) };
			await InsertObjectInDatabase<Member>(mbr);
			ts.SetData(MemberKey, mbr);
		}

		public async Task<bool> AddClan(TSPlayer ts, string name)
		{
			Clan clan = await GetObjectFromDatabase<Clan>("SELECT * FROM Clans WHERE Name = @0", name);
			if (clan != null)
				return false;

			clan = new Clan(name);
			await InsertObjectInDatabase<Clan>(clan);
			ClanCache.Add(clan.Name, clan);
			AddMember(ts, name, true);
			ClanHooks.OnClanCreated(clan, ts);
			return true;
		}

		public async void LoadClans()
		{
			ClanCache.Clear();
			Clan[] clans = await GetObjectArrayFromDatabase<Clan>("SELECT * FROM Clans");
			for (int i = 0; i < clans.Length; i++)
				ClanCache.Add(clans[i].Name, clans[i]);
		}

		public async Task InsertObjectInDatabase<T>(object obj) where T : class
		{
			await Task.Run(() =>
			{
				try
				{
					var tableName = (typeof(T).GetCustomAttribute(typeof(DBTableAttribute)) as DBTableAttribute).TableName;
					var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(f => f.GetCustomAttribute(typeof(DBColumnAttribute)) != null);
					var values = properties.Select(f => f.GetValue(obj)).ToArray();

					StringBuilder sb = new StringBuilder("INSERT INTO ");
					sb.Append(tableName).Append(" (");
					string names = string.Join(", ", properties.Select(f => (f.GetCustomAttribute(typeof(DBColumnAttribute)) as DBColumnAttribute).Name));
					sb.Append(names).Append(") ");
					sb.Append("VALUES (");
					sb.Append(string.Join(", ", values.Select((v, i) => $"@{i}")));
					sb.Append(")");
					Console.WriteLine(sb.ToString());
					connection.Query(sb.ToString(), values);
				}
				catch (Exception ex) { TShock.Log.Error(ex.ToString()); }
			});
		}

		public Task<T[]> GetObjectArrayFromDatabase<T>(string query, params object[] args) where T : class
		{
			List<T> results = new List<T>();
			return Task.Run<T[]>(() =>
			{
				try
				{
					var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(f => f.GetCustomAttribute(typeof(DBColumnAttribute)) != null);

					using (QueryResult reader = connection.QueryReader(query, args))
					{
						while (reader.Read())
						{
							if (typeof(T).GetConstructor(Type.EmptyTypes) == null)
								throw new Exception($"{typeof(T)} has no parameterless constructor!");

							var instance = (T)Activator.CreateInstance(typeof(T));
							foreach (var info in properties)
							{
								var attrib = (info.GetCustomAttribute(typeof(DBColumnAttribute)) as DBColumnAttribute);
								object value = reader.Get<object>(attrib.Name);
								instance.GetType().GetProperty(attrib.Name).SetValue(instance, value);
							}
							results.Add(instance);
						}
					}
				}
				catch (Exception ex) { TShock.Log.Error(ex.ToString()); }
				return results.ToArray();
			});
		}

		public Task<T> GetObjectFromDatabase<T>(string query, params object[] args) where T : class
		{
			return Task.Run<T>(() =>
			{
				try
				{
					var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(f => f.GetCustomAttribute(typeof(DBColumnAttribute)) != null);
					var instance = (T)Activator.CreateInstance(typeof(T));
					using (QueryResult reader = connection.QueryReader(query, args))
					{
						if (reader.Read())
						{
							foreach (var info in properties)
							{
								var attrib = (info.GetCustomAttribute(typeof(DBColumnAttribute)) as DBColumnAttribute);
								object value = reader.Get<object>(attrib.Name);
								instance.GetType().GetProperty(attrib.Name).SetValue(instance, value);
							}
							return instance;
						}
					}
				}
				catch (Exception ex) { TShock.Log.Error(ex.ToString()); }
				return default(T);
			});
		}

		public void EnsureTableStructure<T>() where T : class
		{
			try
			{
				var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(f => f.GetCustomAttribute(typeof(DBColumnAttribute)) != null);

				if (properties.Count() == 0)
					throw new Exception($"{typeof(T)} has no public properties.");

				List<SqlColumn> columns = new List<SqlColumn>();

				foreach (var info in properties)
				{
					var attrib = (info.GetCustomAttribute(typeof(DBColumnAttribute)) as DBColumnAttribute);
					columns.Add(attrib.GetColumn());
				}

				SqlTableCreator sqlcreator = new SqlTableCreator(connection, connection.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());

				var tableName = (typeof(T).GetCustomAttribute(typeof(DBTableAttribute)) as DBTableAttribute).TableName;

				sqlcreator.EnsureTableStructure(new SqlTable(tableName, columns.ToArray()));
			}
			catch (Exception ex) { TShock.Log.Error(ex.ToString()); }
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
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
