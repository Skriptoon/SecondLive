using System;
using System.Collections.Generic;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using SecondLive.Core.Character;
using SecondLive.Core.nAccount;
using SecondLive.Core.Logging;
using SecondLive.Core;
using SecondLive.GUI;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SecondLive
{
    public class Main : Script
    {
        public static Config config = new Config();
        public static Dictionary<Player, Character> Players = new Dictionary<Player, Character>();
        // Accounts
        //public static List<string> Usernames = new List<string>(); // usernames
        //public static Dictionary<string, string> Emails = new Dictionary<string, string>(); // emails
        public static Dictionary<Player, nAccount> Accounts = new Dictionary<Player, nAccount>(); // client's accounts

        public static bool DebugHunger = false;

        private static nLog Log = new nLog("GM");

        public Main()
        {
            MySQL.Set(config.DBServer, config.DBName, config.DBUid, config.DBPassword);

            /*if (!MySQL.Test())
            {
                Log.Write("Initialization has been aborted.", nLog.Type.Warn);
                Environment.Exit(0);
            }*/
        }

        /*[Command("creator")]
        public static void CMD_creator(Player player)
        {
            Customization.SendToCreator(player);
        }*/

        [Command("add_item")]
        public static void CMD_add_item(Player player, int type)
        {
            nInventory.Add(player, type);
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public async Task OnPlayerDisconnectedAsync(Player player, DisconnectionType type, string reason)
        {
            if(Customization.CustomPlayerData.ContainsKey(Players[player].UUID))
            {
                await Players[player].Save(player);
                Customization.SaveCharacter(player);
            }
            if (Accounts.ContainsKey(player))
            {
                await Accounts[player].Save();
            }

            Customization.CustomPlayerData.Remove(Players[player].UUID);
            Accounts.Remove(player);
            Players.Remove(player);

        }
        [ServerEvent(Event.PlayerConnected)]
        public void Event_OnPlayerConnected(Player player)
        {
            NAPI.Entity.SetEntityDimension(player, 9999);
        }

        #region AuthEvents
        [RemoteEvent("client.login")]
        public async Task ClientLoginAsync(Player player, string login, string pass)
        {
            var acc = new nAccount();
            var request = await acc.LoginIn(player, login, pass);

            if(request == LoginEvent.Authorized)
            {
                List<object> characters = new List<object>();
                for(int i = 0; i < 3; i++)
                {

                    if(acc.Characters[i] > 0)
                    {
                        Dictionary<string, string> name = new Dictionary<string, string>();
                        MySqlCommand sqlCommand = new MySqlCommand(@"SELECT `firstname`, `lastname` FROM `characters` WHERE `uuid` = @UUID");

                        sqlCommand.Parameters.AddWithValue("@UUID", Accounts[player].Characters[i]);

                        DataTable result = await MySQL.QueryResultAsync(sqlCommand);
                        DataRow row = result.Rows[0];

                        name.Add("firstname", Convert.ToString(row["firstname"]));
                        name.Add("lastname", Convert.ToString(row["lastname"]));

                        characters.Add(name);
                    }
                }
                Trigger.ClientEvent(player, "client.login.response", JsonConvert.SerializeObject(characters));
            }
            else if(request == LoginEvent.Refused)
            {
                Notify.Send(player, "Не верный логин или пароль", Notify.Type.Error);
            }
            else if (request == LoginEvent.SclubError)
            {
                Notify.Send(player, "Аккаунт привязан к другому SocialClub", Notify.Type.Error);
            }
            else
            {
                Notify.Send(player, "Ошибка авторизации. Попробуйте позже", Notify.Type.Error);
            }
        }

        [RemoteEvent("client.selectCharacter")]
        public async Task SelectCharacterAsync(Player player, int number)
        {
            var acc = Accounts[player];
            var character = new Character();
            Dictionary<string, object> spawnData = await character.Load(player, acc.Characters[number]);
            if (Convert.ToString(spawnData["nextState"]) == "createCharacter")
            {
                Trigger.ClientEvent(player, "client.selectCharacter.response");
                //Customization.SendToCreator(player);
            }
            else if (Convert.ToString(spawnData["nextState"]) == "spawnMenu")
            {
                Trigger.ClientEvent(player, "client.selectCharacter.response", JsonConvert.SerializeObject(spawnData));
                Players[player].IsSpawned = true;
            }
        }

        [RemoteEvent("client.register")]
        public async Task ClientRegisterAsync(Player player, string login, string email, string pass)
        {
            var acc = new nAccount();
            var request =  await acc.Register(player, login, pass, email, "");

            if(request == RegisterEvent.Registered)
            {
                var character = new Character();
                var uuid = await character.Create(player);
                Accounts[player].Characters[0] = uuid;
                if (uuid > 0)
                {
                    Customization.CreateCharacter(player);
                }
                Trigger.ClientEvent(player, "client.register.response");
            }
            else if(request == RegisterEvent.DataError)
            {
                Notify.Send(player, "Заполните все поля", Notify.Type.Error);
            }
            else if (request == RegisterEvent.SocialReg)
            {
                Notify.Send(player, "Этот SocialClub уже используется", Notify.Type.Error);
            }
            else if (request == RegisterEvent.UserReg)
            {
                Notify.Send(player, "Этот логин уже используется", Notify.Type.Error);
            }
            else if (request == RegisterEvent.EmailReg)
            {
                Notify.Send(player, "Эта почта уже используется", Notify.Type.Error);
            }
            else
            {
                Notify.Send(player, "Ошибка регистрации. Попробуйте позже");
            }
        }
        [RemoteEvent("client.createCharacter")]
        public async Task createCharacterAsync(Player player, int num)
        {
            var character = new Character();
            var uuid = await character.Create(player);
            if (uuid > 0)
            {
                Accounts[player].Characters[num] = uuid;
                Customization.CreateCharacter(player);
            }
        }
        [RemoteEvent("client.spawn")]
        public static void ClientSpawn(Player player)
        {
            NAPI.Entity.SetEntityDimension(player, NAPI.GlobalDimension);
        }
        #endregion
    }
}
