using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GTANetworkAPI;
using SecondLive.Core.Logging;
using SecondLive.GUI;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;

namespace SecondLive.Core.nAccount
{
    //TODO: public class
    public class nAccount
    {
        private static nLog Log = new nLog("Account");

        public int ID { get; private set; }
        public string Login { get; private set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string HWID { get; private set; }
        public string IP { get; private set; }
        public string SocialClubName { get; private set; }
        public ulong SocialClubId { get; private set; }

        public long DonatePoints { get; set; } = 0;
        public int VipLvl { get; set; }
        public DateTime VipDate { get; set; } = DateTime.Now;

        public List<string> PromoCodes { get; set; }
        public bool RefAward { get; set; }
        public List<int> Characters { get; private set; } // characters uuids

        public bool IsBcryptPassword = true;

        public bool IsStreamer = false;

        public async Task<RegisterEvent> Register(Player player, string login_, string pass_, string email_, string promo_) 
        {
            try {
                if (login_.Length < 1 || pass_.Length < 1 || email_.Length < 1)
                {
                    return RegisterEvent.DataError;
                }

                Login = login_;
                Email = email_;
                if (IsBcryptPassword)
                    Password = BCrypt.Net.BCrypt.HashPassword(pass_, BCrypt.Net.BCrypt.GenerateSalt());
                else
                    Password = GetSha256(pass_);

                MySqlCommand queryCommand = new MySqlCommand(@"SELECT * FROM `accounts` WHERE `login` = @LOGIN OR `email` = @EMAIL OR `socialclubid` = @SOCIALID");

                queryCommand.Parameters.AddWithValue("@LOGIN", login_);
                queryCommand.Parameters.AddWithValue("@EMAIL", email_);
                queryCommand.Parameters.AddWithValue("@SOCIALID", player.SocialClubId);

                DataTable result = await MySQL.QueryResultAsync(queryCommand);

                if (result != null)
                {
                    foreach (DataRow Row in result.Rows)
                    {
                        if (Convert.ToString(Row["login"]) == login_)
                        {
                            return RegisterEvent.UserReg;
                        }
                        if (Convert.ToString(Row["email"]) == email_)
                        {
                            return RegisterEvent.EmailReg;
                        }
                        if (Convert.ToUInt64(Row["socialclubid"]) == player.SocialClubId)
                        {
                            return RegisterEvent.SocialReg;
                        }
                    }
                }

                Characters = new List<int>() {-1, -1, -1};

                HWID = player.Serial;
                IP = player.Address;
                SocialClubName = player.SocialClubName;
                SocialClubId = player.SocialClubId;

                MySqlCommand queryInsertCommand = new MySqlCommand(@"
                    INSERT INTO
                        `accounts` (
                            `login`,
                            `email`,
                            `password`,
                            `hwid`,
                            `ip`,
                            `socialclubname`,
                            `socialclubid`,
                            `donate`,
                            `viplvl`,
                            `vipdate`,
                            `promocodes`,
                            `refaward`,
                            `character1`,
                            `character2`,
                            `character3`,
                            `registerDate`,
                            `isBcryptPassword`
                        )
                    VALUES
                        (
                            @LOGIN,
                            @EMAIL,
                            @PASSWORD,
                            @HWID,
                            @IP,
                            @SCNAME,
                            @SCID,
                            @DONATE,
                            @VIPLVL,
                            @VIPDATE,
                            @PROMOCODES,
                            @REFAWARD,
                            @CHARACTERONE,
                            @CHARACTERTWO,
                            @CHARACTERTHREE,
                            @REGISTERDATE,
                            @ISBCRYPTPASSWORD
                        )
                ");

                queryInsertCommand.Parameters.AddWithValue("@LOGIN", Login);
                queryInsertCommand.Parameters.AddWithValue("@EMAIL", Email);
                queryInsertCommand.Parameters.AddWithValue("@PASSWORD", Password);
                queryInsertCommand.Parameters.AddWithValue("@HWID", HWID);
                queryInsertCommand.Parameters.AddWithValue("@IP", IP);
                queryInsertCommand.Parameters.AddWithValue("@SCNAME", SocialClubName);
                queryInsertCommand.Parameters.AddWithValue("@SCID", SocialClubId);
                queryInsertCommand.Parameters.AddWithValue("@DONATE", 0);
                queryInsertCommand.Parameters.AddWithValue("@VIPLVL", 0);
                queryInsertCommand.Parameters.AddWithValue("@VIPDATE", MySQL.ConvertTime(VipDate));
                queryInsertCommand.Parameters.AddWithValue("@PROMOCODES", JsonConvert.SerializeObject(PromoCodes));
                queryInsertCommand.Parameters.AddWithValue("@REFAWARD", 0);
                queryInsertCommand.Parameters.AddWithValue("@CHARACTERONE", -1);
                queryInsertCommand.Parameters.AddWithValue("@CHARACTERTWO", -1);
                queryInsertCommand.Parameters.AddWithValue("@CHARACTERTHREE", -2);
                queryInsertCommand.Parameters.AddWithValue("@REGISTERDATE", MySQL.ConvertTime(DateTime.Now));
                queryInsertCommand.Parameters.AddWithValue("@ISBCRYPTPASSWORD", IsBcryptPassword);

                await MySQL.QueryAsync(queryInsertCommand);

                MySqlCommand querySelectCommand = new MySqlCommand(@"
                    SELECT `id`
                    FROM `accounts`
                    WHERE
                        `login` = @LOGIN
                ");

                querySelectCommand.Parameters.AddWithValue("@LOGIN", login_);

                result = await MySQL.QueryResultAsync(querySelectCommand);

                ID = Convert.ToInt32(result.Rows[0]["id"]);

                //Main.Usernames.Add(Login);
                //Main.Emails.Add(Email, Login);
                Main.Accounts.Add(player, this);

                return RegisterEvent.Registered;
            } catch (Exception ex)
            {
                await Log.WriteAsync(ex.ToString(), nLog.Type.Error);
                return RegisterEvent.Error;
            }
        }
        public async Task<LoginEvent> LoginIn(Player client, string login_, string InputPassword)
        {
            try
            {
                //if (Main.LoggedIn.Contains(login_)) return LoginEvent.Already;

                MySqlCommand querySelectCommand = new MySqlCommand(@"
                    SELECT *
                    FROM `accounts`
                    WHERE
                        `login` = @LOGIN
                ");

                querySelectCommand.Parameters.AddWithValue("@LOGIN", login_);

                DataTable result = await MySQL.QueryResultAsync(querySelectCommand);

                //Если база не вернула таблицу, то отправляем сброс
                if (result == null) {
                    return LoginEvent.Refused;
                }

                //Иначе, парсим строку
                DataRow row = result.Rows[0];

                int IsBcryptPasswordFromDatabase = Convert.ToInt32(row["isBcryptPassword"]);

                IsBcryptPassword = IsBcryptPasswordFromDatabase > 0;

                int IsStreamerFromDatabase = Convert.ToInt32(row["isstreamer"]);

                IsStreamer = IsStreamerFromDatabase > 0;

                string OutputPassword = Convert.ToString(row["password"]);

                if (IsBcryptPassword && !BCrypt.Net.BCrypt.Verify(InputPassword, OutputPassword)) {
                    return LoginEvent.Refused;
                }

                if (!IsBcryptPassword && OutputPassword != GetSha256(InputPassword)) {
                    return LoginEvent.Refused;
                }

                //Далее делаем разбор и оперируем данными
                ID = Convert.ToInt32(row["id"]);
                Login = Convert.ToString(row["login"]);
                Email = Convert.ToString(row["email"]);
                Password = OutputPassword;
                //Служебные данные
                HWID = client.Serial;
                IP = client.Address;
                SocialClubName = Convert.ToString(row["socialclubname"]);
                SocialClubId = Convert.ToUInt64(row["socialclubid"]);
                //if (SocialClub != client.SocialClubName) return LoginEvent.SclubError;

                DonatePoints = Convert.ToInt32(row["donate"]);
                VipLvl = Convert.ToInt32(row["viplvl"]);
                VipDate = (DateTime)row["vipdate"];

                PromoCodes = JsonConvert.DeserializeObject<List<string>>(row["promocodes"].ToString());
                RefAward = Convert.ToBoolean(row["refaward"]);
                var char1 = Convert.ToInt32(row["character1"]);
                var char2 = Convert.ToInt32(row["character2"]);
                var char3 = Convert.ToInt32(row["character3"]);
                Characters = new List<int>() { char1, char2, char3 };

                if(SocialClubId != client.SocialClubId)
                {
                    return LoginEvent.SclubError;
                }

                Main.Accounts.Add(client, this);

                return LoginEvent.Authorized;
            }
            catch(Exception ex)
            {
                await Log.WriteAsync(ex.ToString(), nLog.Type.Error);
                return LoginEvent.Error;
            }
        }
        /*public async Task<TokenEvent> TokenIn(Client client, string serial)
        {
            try
            {
                Token token = Token.Get(serial);
                //Если токен найти не удалось, то вернем "Not Found"
                if (token == null) return TokenEvent.NotFound;
                //Проверка вернет свои токен-ивенты
                TokenEvent check = token.Check(client.Serial, client.SocialClubName);
                if(check != TokenEvent.Authorized)
                {
                    Token.tokens.Remove(token);
                    return check;
                }
                //Получаем данные клиента на основе id токена

                MySqlCommand querySelectCommand = new MySqlCommand(@"
                    SELECT *
                    FROM `accounts`
                    WHERE
                        `token` = @TOKEN
                ");

                querySelectCommand.Parameters.AddWithValue("@TOKEN", token.Serial);

                DataTable result = await MySQL.QueryResultAsync(querySelectCommand);

                //Если база не вернула таблицу, то отправляем ошибку
                if (result == null) return TokenEvent.Error;
                //Иначе, парсим строку
                DataRow row = result.Rows[0];
                //Далее делаем разбор и оперируем данными
                Login = Convert.ToString(row["login"]);
                Email = Convert.ToString(row["email"]);
                Password = Convert.ToString(row["pass"]);
                //Служебные данные
                HWID = client.Serial;
                SocialClubName = client.SocialClubName;
                SocialClubId = Main.SocialClubIds[client];

                return TokenEvent.Authorized;
            }
            catch(Exception ex)
            {
                await Log.WriteAsync(ex.ToString(), nLog.Type.Error);
                return TokenEvent.Error;
            }
        }*/
        public async Task<bool> Save()
        {
            try
            {
                MySqlCommand querySelectCommand = new MySqlCommand(@"
                    UPDATE `accounts`
                    SET
                        `password` = @PASSWORD,
                        `email` = @EMAIL,
                        `viplvl` = @VIPLVL,
                        `hwid` = @HWID,
                        `ip` = @IP,
                        `vipdate` = @VIPDATE,
                        `socialclubname` = @SCNAME,
                        `socialclubid` = @SCID,
                        `promocodes` = @PROMOCODES,
                        `refaward` = @REFAWARD,
                        `character1` = @CHARACTERONE,
                        `character2` = @CHARACTERTWO,
                        `character3` = @CHARACTERTHREE
                    WHERE
                        `login` = @LOGIN
                ");

                querySelectCommand.Parameters.AddWithValue("@PASSWORD", Password);
                querySelectCommand.Parameters.AddWithValue("@EMAIL", Email);
                querySelectCommand.Parameters.AddWithValue("@VIPLVL", VipLvl);
                querySelectCommand.Parameters.AddWithValue("@HWID", HWID);
                querySelectCommand.Parameters.AddWithValue("@IP", IP);
                querySelectCommand.Parameters.AddWithValue("@VIPDATE", MySQL.ConvertTime(VipDate));
                querySelectCommand.Parameters.AddWithValue("@SCNAME", SocialClubName);
                querySelectCommand.Parameters.AddWithValue("@SCID", SocialClubId);
                querySelectCommand.Parameters.AddWithValue("@PROMOCODES", JsonConvert.SerializeObject(PromoCodes));
                querySelectCommand.Parameters.AddWithValue("@REFAWARD", RefAward);
                querySelectCommand.Parameters.AddWithValue("@CHARACTERONE", Characters[0]);
                querySelectCommand.Parameters.AddWithValue("@CHARACTERTWO", Characters[1]);
                querySelectCommand.Parameters.AddWithValue("@CHARACTERTHREE", Characters[2]);
                querySelectCommand.Parameters.AddWithValue("@LOGIN", Login);

                await MySQL.QueryAsync(querySelectCommand);

                return true;
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"Save\":\n" + e.ToString(), nLog.Type.Error);
                return false;
            }
        }
       /* public Dictionary<string, object> LoadSlots(Client player) {
            try {
                Dictionary<string, object> data = new Dictionary<string, object>();

                List<object> charactersList = new List<object>();

                // List<object> data = new List<object>();
                foreach (var uuid in Characters) {
                    Dictionary<string, object> subData = new Dictionary<string, object>();

                    subData["id"] = uuid;
                    subData["banExists"] = false;

                    if (uuid > -1) {
                        subData["exists"] = true;
                        subData["canCreate"] = false;

                        var ban = Ban.Get2(uuid);

                        if (ban != null && ban.CheckDate()) {
                            Dictionary<string, object> banData = new Dictionary<string, object>();

                            subData["banExists"] = true;
                            banData["reason"] = ban.Reason;
                            banData["admin"] = ban.ByAdmin;
                            banData["from"] = $"{ban.Time.ToShortDateString()} {ban.Time.ToShortTimeString()}";
                            banData["to"] = $"{ban.Until.ToShortDateString()} {ban.Until.ToShortTimeString()}";

                            subData["ban"] = banData;
                        } else {
                            var name = Main.PlayerNames[uuid];
                            var split = name.Split('_');
                            var tuple = Main.PlayerSlotsInfo[uuid];

                            subData["firstName"] = split[0];
                            subData["lastName"] = split[1];
                            subData["level"] = tuple.Item1;
                            subData["exp"] = tuple.Item2;
                            subData["faction"] = Fractions.Manager.FractionNames[tuple.Item3];
                            subData["money"] = tuple.Item4;
                            subData["bankMoney"] = MoneySystem.Bank.Get(uuid).Balance;
                        }
                    } else {
                        subData["exists"] = false;
                        subData["canCreate"] = uuid == -1;
                    }

                    charactersList.Add(subData);
                }

                data["characters"] = charactersList;
                data["donate"] = DonatePoints;
                data["login"] = Login;

                return data;
            } catch (Exception e) {
                Console.WriteLine(e);

                Log.Write($"NAccount:LoadSlots:\n{e.ToString()}\n${e.StackTrace.ToString()}", nLog.Type.Error);
                return null;
            }
        }

        public async Task<Dictionary<string, object>> CreateCharacter(Client player, int slotIndex, string firstName, string lastName)
        {
            Dictionary<string, object> CreateCharacterData = new Dictionary<string, object>();

            if (Characters[slotIndex] != -1) {
                CreateCharacterData["code"] = 0;
                return CreateCharacterData;
            }

            Character.Character character = new Character.Character();

            int result = await character.Create(player, firstName, lastName);

            if (result == -1) {
                CreateCharacterData["code"] = 1;
                return CreateCharacterData;
            } else if (result == -2) {
                CreateCharacterData["code"] = 2;
                return CreateCharacterData;
            } else if (result == -3) {
                CreateCharacterData["code"] = 3;
                return CreateCharacterData;
            } else if (result == -4) {
                CreateCharacterData["code"] = 4;
                return CreateCharacterData;
            } else if (result == -5) {
                CreateCharacterData["code"] = 5;
                return CreateCharacterData;
            }

            Characters[slotIndex] = result;

            MySqlCommand querySelectCommand = new MySqlCommand($@"
                UPDATE `accounts`
                SET
                    `character{slotIndex + 1}` = @RESULT
                WHERE
                    `login` = @LOGIN
            ");

            querySelectCommand.Parameters.AddWithValue("@RESULT", result);
            querySelectCommand.Parameters.AddWithValue("@LOGIN", Login);

            await MySQL.QueryAsync(querySelectCommand);

            GameLog.CharacterCreate($"{firstName}_{lastName}", result, Login, ID, Main.Players[player].Bank, SocialClubId);

            Dictionary<string, object> spawnData = await Main.Players[player].Spawn(player);

            if (spawnData == null) {
                CreateCharacterData["code"] = 5;
                return CreateCharacterData;
            }

            CreateCharacterData["code"] = 6;
            CreateCharacterData.Add("spawnData", spawnData);

            Dictionary<string, object> characterData = new Dictionary<string, object>();

            characterData.Add("firstName", firstName);
            characterData.Add("lastName", lastName);

            CreateCharacterData.Add("characterData", characterData);

            return CreateCharacterData;
        }

        public async Task<DeleteCharacterEvent> DeleteCharacter(Client player, int slotIndex, string InputFirstName, string InputLastName, string InputPassword)
        {
            if (Characters[slotIndex] == -1 || Characters[slotIndex] == -2) {
                return DeleteCharacterEvent.CharacterNotFound;
            }

            MySqlCommand querySelectCommand = new MySqlCommand(@"
                SELECT `firstname`, `lastname`, `biz`, `sim`, `bank`, `arrest`, `demorganData`
                FROM `characters`
                WHERE
                    `uuid` = @UUID
            ");

            querySelectCommand.Parameters.AddWithValue("@UUID", Characters[slotIndex]);

            DataTable result = await MySQL.QueryResultAsync(querySelectCommand);

            if (result == null) {
                return DeleteCharacterEvent.CharacterNotFound;
            }

            DataRow row = result.Rows[0];
            string firstName = row["firstname"].ToString();
            string lastName = row["lastname"].ToString();
            List<int> biz = JsonConvert.DeserializeObject<List<int>>(row["biz"].ToString());
            int sim = Convert.ToInt32(row["sim"]);
            int bank = Convert.ToInt32(row["bank"]);
            int uuid = Characters[slotIndex];

            int arrestTime = Convert.ToInt32(row["arrest"]);
            var demorganData = JsonConvert.DeserializeObject<Character.Demorgan>(row["demorganData"].ToString());

            if (demorganData == null) {
                demorganData = new Character.Demorgan(0, "nope", "nope", 0);
            }

            if (firstName != InputFirstName || lastName != InputLastName) {
                return DeleteCharacterEvent.NickNameDoesNotMatch;
            }

            if (
                (IsBcryptPassword && !BCrypt.Net.BCrypt.Verify(InputPassword, Password)) ||
                (!IsBcryptPassword && Password != await GetSha256(InputPassword))
            ) {
                return DeleteCharacterEvent.PasswordDoesNotMatch;
            }

            if (arrestTime > 0 || demorganData.Time > 0) {
                return DeleteCharacterEvent.CharacterInJail;
            }

            foreach (var b in biz) {
                BusinessManager.BizList[b].OwnerUUID = -1;
                BusinessManager.BizList[b].UpdateLabel();
                BusinessManager.BizList[b].Save();
            }

            MySqlCommand queryDeleteCharacterCtommand = new MySqlCommand(@"
                DELETE FROM `characters`
                WHERE
                    `uuid` = @UUID
            ");

            queryDeleteCharacterCtommand.Parameters.AddWithValue("@UUID", uuid);

            await MySQL.QueryAsync(queryDeleteCharacterCtommand);

            nInventory.Items.Remove(uuid);

            MoneySystem.Bank.Remove(bank, false);

            var vehicles = VehicleManager.getAllPlayerVehicles(Main.PlayerUUIDs[$"{firstName}_{lastName}"]);

            foreach (var v in vehicles) {
                VehicleManager.Remove(v, false);
            }

            Main.UUIDs.Remove(uuid);
            Main.PlayerNames.Remove(uuid);
            Main.PlayerUUIDs.Remove($"{firstName}_{lastName}");
            Main.SimCards.Remove(sim);
            Main.PlayerSlotsInfo.Remove(uuid);
            Customization.CustomPlayerData.Remove(uuid);

            Characters[slotIndex] = -1;

            GameLog.CharacterDelete($"{firstName}_{lastName}", uuid, Login, ID);

            // Trigger.ClientEvent(player, "delCharSuccess", slot);

            return DeleteCharacterEvent.DeletedSuccessful;
        }

        public async Task changePassword(string newPass)
        {
            this.Password = BCrypt.Net.BCrypt.HashPassword(newPass);
            this.IsBcryptPassword = true;

            MySqlCommand queryCommand = new MySqlCommand(@"
                UPDATE `accounts`
                SET
                    `password` = @PASSWORD,
                    `isBcryptPassword` = @ISBCRYPTPASSWORD
                WHERE
                    `id` = @ID
            ");

            queryCommand.Parameters.AddWithValue("@PASSWORD", this.Password);
            queryCommand.Parameters.AddWithValue("@ISBCRYPTPASSWORD", true);
            queryCommand.Parameters.AddWithValue("@ID", this.ID);

            await MySQL.QueryAsync(queryCommand);

            //TODO: Logging ths action
        }*/
        private static string GetSha256(string strData)
        {
            var message = Encoding.ASCII.GetBytes(strData);
            var hashString = new SHA256Managed();
            var hex = "";

            var hashValue = hashString.ComputeHash(message);
            foreach (var x in hashValue)
                hex += string.Format("{0:x2}", x);
            return hex;
        }
    }
    class Token
    {
        public static List<Token> tokens = new List<Token>();

        public string Serial { get; set; }
        public string HWID { get; set; }
        public string SocialClub { get; set; }
        public DateTime Expires { get; set; }

        public static Token Get(string Serial)
        {
            return tokens.Find(x => x.Serial == Serial);
        }
        public TokenEvent Check(string hwid_, string socialclub_)
        {
            if (HWID != hwid_) return TokenEvent.HWID_mismatch;
            if (SocialClub != socialclub_) return TokenEvent.SocialClub_mismatch;
            if (Expires > DateTime.Now) return TokenEvent.Livetime_End;
            return TokenEvent.Authorized;
        }

        public static Token New(string HWID_, string Socialclub_)
        {
            return new Token()
            {
                Serial = GenRandomString("QWERTYUIOPASDFGHJKLZXCVBNM", 20),
                Expires = DateTime.Now.AddDays(30),
                SocialClub = Socialclub_,
                HWID = HWID_
            };
        }

        //TODO: Переместить в Utils
        private static string GenRandomString(string Alphabet, int Length)
        {
            //string Ret = "";
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder(Length - 1);
            int Position = 0;

            for (int i = 0; i < Length; i++)
            {
                Position = rnd.Next(0, Alphabet.Length - 1);
                sb.Append(Alphabet[Position]);
                //Ret = Ret + Alphabet[Position]; //- так делать не стоит, в данном случае StringBuilder дает явный выигрыш в скорости
            }

            //return Ret;

            return sb.ToString();
        }
    }

    public enum LoginEvent
    {
        Already,
        Authorized,
        Refused,
        SclubError,
        Error
    }

    public enum RegisterEvent
    {
        Registered,
        SocialReg,
        UserReg,
        EmailReg,
        DataError,
        Error
    }

    public enum TokenEvent
    {
        Authorized,
        HWID_mismatch,
        SocialClub_mismatch,
        Livetime_End,
        NotFound,
        Error
    }

    public enum DeleteCharacterEvent {
        CharacterNotFound = 0,
        NickNameDoesNotMatch = 1,
        PasswordDoesNotMatch = 2,
        CharacterInJail = 3,
        DeletedSuccessful = 4
    }

    public enum CreateCharacterEvent {
        NotFreeSlot,
        NotAuthorized,
        BadFirstName,
        BadLastName,
        NickNameAlreadyUsed,
        UnknownError
    }
}