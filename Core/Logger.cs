using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using GTANetworkAPI;

namespace SecondLive.Core.Logging
{
    public class nLog
    {
        public nLog(string reference = null, bool canDebug = true)
        {
            if (reference == null) reference = "Logger";
            Ref = reference;
            CanDebug = canDebug;
        }
        public string Ref { get; set; }
        public bool CanDebug { get; set; }

        public enum Type
        {
            Info,
            Warn,
            Error,
            Success
        };

        internal void Write(string text, Type logType = Type.Info)
        {
            try
            {
                Console.ResetColor();
#if DEBUG
                Console.Write($"{DateTime.Now.ToString("HH':'mm':'ss.fff")} | ");
#else
                Console.Write($"{DateTime.Now.ToString("HH':'mm':'ss")} | ");
#endif

                switch (logType)
                {
                    case Type.Error:
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("Error");

                            MySqlCommand queryCommand = new MySqlCommand(@"
                            INSERT INTO `syslog` (`msg`)
                            VALUES (@MSG)
                        ");

                            queryCommand.Parameters.AddWithValue("@MSG", Ref + "//" + text);

                            MySQL.Query(queryCommand);

                            break;
                        }
                    case Type.Warn:
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(" Warn");
                            break;
                        }
                    case Type.Info:
                        {
                            Console.Write(" Info");
                            break;
                        }
                    case Type.Success:
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(" Succ");
                            break;
                        }
                    default:
                        {
                            return;
                        }
                }

                Console.ResetColor();
                Console.Write($" | {Ref} | {text}\n");
            }
            catch (Exception e)
            {
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Logger Error:\n" + e.ToString());
                Console.ResetColor();
            }
        }
        internal async Task WriteAsync(string text, Type logType = Type.Info)
        {
            try
            {
                Console.ResetColor();
#if DEBUG
                Console.Write($"{DateTime.Now.ToString("HH':'mm':'ss.fff")} | ");
#else
                Console.Write($"{DateTime.Now.ToString("HH':'mm':'ss")} | ");
#endif
                switch (logType)
                {
                    case Type.Error:
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("Error");

                            MySqlCommand queryCommand = new MySqlCommand(@"
                            INSERT INTO `syslog` (`msg`)
                            VALUES (@MSG)
                        ");

                            queryCommand.Parameters.AddWithValue("@MSG", text);

                            await MySQL.QueryAsync(queryCommand);
                            break;
                        }
                    case Type.Warn:
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(" Warn");
                            break;
                        }
                    case Type.Info:
                        {
                            Console.Write(" Info");
                            break;
                        }
                    case Type.Success:
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(" Succ");
                            break;
                        }
                    default:
                        {
                            return;
                        }
                }

                Console.ResetColor();
                Console.Write($" | {Ref} | {text}\n");
            }
            catch (Exception e)
            {
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Logger Error:\n" + e.ToString());
                Console.ResetColor();
            }
        }

        internal void Debug(string text, Type logType = Type.Info)
        {
#if DEBUG
            try
            {
                if (!CanDebug) return;
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("DEBUG");
                Console.ResetColor();
                Console.Write($" | {DateTime.Now.ToString("HH':'mm':'ss.fff")} | ");
                switch (logType)
                {
                    case Type.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Error");
                        break;
                    case Type.Warn:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(" Warn");
                        break;
                    case Type.Info:
                        Console.Write(" Info");
                        break;
                    case Type.Success:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(" Succ");
                        break;
                    default:
                        return;
                }
                Console.ResetColor();
                Console.Write($" | {Ref} | {text}\n");
            }
            catch (Exception e)
            {
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Logger Error:\n" + e.ToString());
                Console.ResetColor();
            }
#endif
        }
    }
    public class GameLog
    {
        private const string insert = "insert into {0}({1}) values ({2})";

        private static Thread thread;
        private static nLog Log = new nLog("GameLog");
        private static Queue<string> queue = new Queue<string>();
        private static Dictionary<int, DateTime> OnlineQueue = new Dictionary<int, DateTime>();

        private static string DB = "logs";
        private static string CONNSTRING;
        private static string EscapeString(string value)
        {
            return value.Replace(@"\\", @"\\\\").Replace(@"'", @"\'").Replace(@"\x00", @"\\x00").Replace(@"\x1a", @"\\x1a").Replace(@"\r", @"\\r").Replace(@"\n", @"\\n");
        }

        public static string PlayerFormat(Player player)
        {
            return $"{Main.Accounts[player].ID},{Main.Accounts[player].Login},{Main.Players[player].UUID},{player.Name}";
        }
        public static void Explote(string Player, string explote)
        {
            if (thread == null) return;

            queue.Enqueue(string.Format(
                insert, "explotelog", "`time`,`player`,`explote`",
                $"'{DateTime.UtcNow.ToString("s")}' , '{Player}', '{explote}'"
            ));
        }

        public static void Death(string Player, string Killer, uint Weapon)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "deathlog", "`time`,`player`,`killer`,`weapon`", $"'{DateTime.UtcNow.ToString("s")}','{Player}','{Killer}',{Weapon}"));
        }
        public static void Votes(uint ElectionId, string Login, string VoteFor)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "votelog", "`election`,`login`,`votefor`,`time`", $"'{ElectionId}','{Login}','{VoteFor}','{DateTime.UtcNow.ToString("s")}'"));
        }
        public static void Stock(int Frac, string Player, string Type, int Amount, bool In)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "stocklog", "`time`,`frac`,`player`,`type`,`amount`,`in`", $"'{DateTime.UtcNow.ToString("s")}',{Frac},'{Player}','{Type}',{Amount},{In}"));
        }
        public static void Admin(string Admin, string Action, string Player)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "adminlog", "`time`,`admin`,`action`,`player`", $"'{DateTime.UtcNow.ToString("s")}','{Admin}','{Action}','{Player}'"));
        }
        /// <summary>
        /// Формат для From и To:
        /// Для игрока - player(UUID).
        /// Для бизнеса - biz(ID).
        /// Для банка - bank(UUID).
        /// Для сервисов и услуг - произвольно.
        /// Пример: Money("donate","player(1)",100500)
        /// </summary>
        public static void Money(string From, string To, long Amount, string Comment)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "moneylog", "`time`,`from`,`to`,`amount`,`comment`", $"'{DateTime.UtcNow.ToString("s")}','{From}','{To}',{Amount.ToString()},'{Comment}'"));
        }
        public static void Items(string From, string To, int Type, int Amount, string Data)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "itemslog", "`time`,`from`,`to`,`type`,`amount`,`data`", $"'{DateTime.UtcNow.ToString("s")}','{From}','{To}',{Type},{Amount},'{Data}'"));
        }
        public static void Name(int Uuid, string Old, string New)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "namelog", "`time`,`uuid`,`old`,`new`", $"'{DateTime.UtcNow.ToString("s")}',{Uuid},'{Old}','{New}'"));
        }
        /// <summary>
        /// Лог банов
        /// </summary>
        /// <param name="Admin">UUID админа</param>
        /// <param name="Player">UUID игрока</param>
        public static void Ban(string Admin, string Player, DateTime Until, string Reason, bool isHard)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "banlog", "`time`,`admin`,`player`,`until`,`reason`,`ishard`", $"'{DateTime.UtcNow.ToString("s")}','{Admin}','{Player}','{Until.ToString("s")}','{EscapeString(Reason)}',{isHard}"));
        }
        public static void Ticket(string player, string target, int sum, string reason)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "ticketlog", "`time`,`player`,`target`,`sum`,`reason`", $"'{DateTime.UtcNow.ToString("s")}','{player}','{target}',{sum},'{EscapeString(reason)}'"));
        }
        public static void Arrest(string player, string target, string reason, int stars)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "arrestlog", "`time`,`player`,`target`,`reason`,`stars`", $"'{DateTime.UtcNow.ToString("s")}','{player}','{target}','{EscapeString(reason)}',{stars}"));
        }
        public static void Connected(Player player, string SClub, string Hwid, int Id, string Ip)
        {
            if (thread == null || OnlineQueue.ContainsKey(Main.Players[player].UUID)) return;
            DateTime now = DateTime.UtcNow;
            queue.Enqueue(string.Format(
                insert, "connlog", "`in`,`out`,`player`,`sclub`,`hwid`,`ip`,`id`", $"'{now.ToString("s")}',null,'{PlayerFormat(player)}','{SClub}','{Hwid}','{Ip}',{Id}"));
            OnlineQueue.Add(Main.Players[player].UUID, now);
        }
        /*public static void Disconnected(Player player)
        {
            if (thread == null || !OnlineQueue.ContainsKey(Main.Players[player].UUID)) return;
            DateTime conn = OnlineQueue[Main.Players[player].UUID];
            if (conn == null) return;
            OnlineQueue.Remove(Main.Players[player].UUID);
            queue.Enqueue($"update connlog set `out`='{DateTime.UtcNow.ToString("s")}' WHERE `in`='{conn.ToString("s")}' and `player`='{PlayerFormat(player)}'");
            //queue.Enqueue($"update masklog set `out`='{DateTime.UtcNow.ToString("s")}' WHERE `in`='{conn.ToString("s")}' and `uuid`={Uuid}");
        }*/
        public static void CharacterDelete(string name, int uuid, string account, int accountid)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "deletelog", "`time`,`uuid`,`name`,`account`,`accountid`", $"'{DateTime.UtcNow.ToString("s")}',{uuid},'{name}','{account}',{accountid}"));
        }

        public static void CharacterCreate(string name, int uuid, string account, int accountid, int bankId, string socialClubId)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert,
                "createCharacterLogs",
                "`time`,`uuid`,`name`,`account`,`accountId`,`bank`, `socialClubId`",
                $"'{DateTime.UtcNow.ToString("s")}',{uuid},'{name}','{account}',{accountid},{bankId},{socialClubId}"));
        }

        #region Логика потока
        public static void Start()
        {
            CONNSTRING = string.Format("Host=localhost;Uid={0};Pwd={1};" +
                "Database={2};Port=3306;SslMode=None;CHARSET=UTF8;",
                Main.config.DBUid, Main.config.DBPassword, Main.config.DBName + DB);
            thread = new Thread(Worker);
            thread.IsBackground = true;
            thread.Start();
        }
        private static void Worker()
        {
            string CMD = "";
            try
            {
                Log.Debug("Worker started");
                while (true)
                {
                    if (queue.Count < 1) {
                        continue;
                    }

                    Log.Debug("Queue > 1");

                    using (MySqlConnection conn = new MySqlConnection(CONNSTRING))
                    {
                        Log.Debug("Connecting to MySQL server");
                        try
                        {
                            conn.OpenAsync();
                            while (queue.Count > 0)
                            {
                                CMD = queue.Dequeue();
                                Log.Debug("Dequeueing new command " + CMD);
                                MySqlCommand cmd = conn.CreateCommand();
                                cmd.CommandText = CMD;
                                cmd.ExecuteNonQueryAsync();
                                Log.Debug("Command executed", nLog.Type.Success);
                            }
                            conn.CloseAsync();

                        }
                        catch (MySqlException e)
                        {
                            Log.Write($"MySQL Error:\n{e.ToString()}\n{CMD}", nLog.Type.Error);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"{e.ToString()}\n{CMD}", nLog.Type.Error);
            }
        }
        public static void Stop()
        {
            thread.Join();
        }
        #endregion
    }
}