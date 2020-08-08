using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using GTANetworkAPI;
using SecondLive.Core.Logging;
using System.Threading.Tasks;
using Newtonsoft.Json;
//using FivestarRevo.Houses;
using SecondLive.GUI;
using MySql.Data.MySqlClient;

namespace SecondLive.Core.Character
{
    public class Character
    {
        private static nLog Log = new nLog("Character");
        private static Random Rnd = new Random();

        public int UUID { get; set; } = -1;
        public Vector3 SpawnPos { get; set; } = new Vector3(0, 0, 0);
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime BirthDate { get; set; } = DateTime.Now;
        public string FirstName { get; set; } = null;
        public string LastName { get; set; } = null;
        public bool Gender { get; set; } = false;
        public int Health { get; set; } = 100;
        public int Armor { get; set; } = 0;
        public int LVL { get; set; } = 0;
        public int EXP { get; set; } = 0;
        public long Money { get; set; } = 2000;
        public int Bank { get; set; } = 0;
        public int WorkID { get; set; } = 0;
        public int FractionID { get; set; } = 0;
        public int FractionLVL { get; set; } = 0;
        public int ArrestTime { get; set; } = 0;
        public WantedLevel WantedLVL { get; set; } = null;
        public List<int> BizIDs { get; set; } = new List<int>();
        public int AdminLVL { get; set; } = 0;
        public List<bool> Licenses { get; set; } = new List<bool>();
        public DateTime Unwarn { get; set; } = DateTime.Now;
        public DateTime Unmute { get; set; } = DateTime.Now;
        public int Warns { get; set; } = 0;
        public string LastVeh { get; set; } = null;
        public bool OnDuty { get; set; } = false;
        public int LastHourMin { get; set; } = 0;
        public int HotelID { get; set; } = -1;
        public int HotelLeft { get; set; } = 0;
        public int Sim { get; set; } = -1;

        private double _drugAddiction = 0;
        public double DrugAddiction {
            get
            {
                return Math.Clamp(_drugAddiction, 0, 100);
            }
            set
            {
                _drugAddiction = Math.Clamp(value, 0, 100);
            }
        }

        public Dictionary<int, string> Contacts = new Dictionary<int, string>();
        //public Hunger HungerData { get; set; } = new Hunger(50, 0);

        public bool VoiceMuted = false;

        // temperory data
        public int InsideHouseID = -1;
        public int InsideGarageID = -1;
        public Vector3 ExteriorPos = new Vector3();
        public int InsideHotelID = -1;
        public int TuningShop = -1;
        public bool IsAlive = false;
        public bool IsSpawned = false;
        public string ResistTimer = null;
        public int DemorganTime { get; set; } = 0;
        public Demorgan DemorganData { get; set; } = null;

        public Dictionary<string, object> Spawn(Player player)
        {
            try
            {
                Dictionary<string, object> ResponseData = new Dictionary<string, object>();

                NAPI.Task.Run(() =>
                {
                    try
                    {
                        Player tmp = null;

                        foreach (KeyValuePair<Player, Character> p in Main.Players)
                        {
                            tmp = p.Key;

                            if (p.Value.UUID == Main.Players[player].UUID && tmp.Value != player.Value)
                            {
                                GameLog.Explote(GameLog.PlayerFormat(player), $"Duplicate connect for ({GameLog.PlayerFormat(tmp)})");

                                GUI.Notify.Send(tmp, Notify.Type.Alert, Notify.Position.BottomCenter, $"Кто-то пытался получить доступ к персонажу! Сообщите администрации!", 5000);
                                GUI.Notify.Send(player, Notify.Type.Error, Notify.Position.BottomCenter, $"Вы не можете сейчас подключиться, попробуйте позже!", 5000);
                                
                                player.Kick("!");
                                tmp.Kick("!");

                                return;
                            }
                        }

                        player.SetSharedData("IS_MASK", false);

                        // Logged in state, money, phone init
                        player.SetData("LOGGED_IN", true);

                        // Trigger.ClientEvent(player, "initPhone");

                        //Jobs.WorkManager.load(player);

                        // Skin, Health, Armor, RemoteID
                        player.SetSkin((Gender) ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01);
                        player.Health = (Health > 5) ? Health : 5;
                        player.Armor = Armor;

                        //Voice.Voice.PlayerJoin(player);

                        if (Unmute > DateTime.Now)
                        {
                            player.SetSharedData("voice.muted", true);
                        }

                        player.SetData("RESIST_STAGE", 0);
                        player.SetData("RESIST_TIME", 0);

                        if (AdminLVL > 0)
                        {
                            player.SetSharedData("IS_ADMIN", true);
                        }

                        /*Dashboard.sendStats(player);
                        Dashboard.sendItems(player);*/
                    }
                    catch (Exception e)
                    {
                        Log.Write($"EXCEPTION AT \"Spawn.NAPI.Task.Run\":\n" + e.ToString(), nLog.Type.Error);
                    }
                });

                if (WantedLVL != null)
                {
                    ResponseData.Add("wantedLevel", WantedLVL.Level);
                }

                ResponseData.Add("logged", true);

                ResponseData.Add("money", Money);
                //ResponseData.Add("bank", MoneySystem.Bank.Accounts[Bank].Balance);

                ResponseData["blipsList"] = new List<int>();

                /*if (Fractions.Manager.FractionTypes[FractionID] == 1 || AdminLVL > 0) {
                    List<int> blipsList = Fractions.GangsCapture.LoadBlips(player, false);

                    ResponseData["blipsList"] = blipsList;
                }

                House house = HouseManager.GetHouse(player);
                if (house != null) {
                    // House blips & checkpoints
                    // Trigger.ClientEvent(player, "changeBlipColor", house.blip, 73);

                    // Trigger.ClientEvent(player, "createCheckpoint", 333, 1, GarageManager.Garages[house.GarageID].Position - new Vector3(0, 0, 1.12), 1, NAPI.GlobalDimension, 220, 220, 0);
                    // Trigger.ClientEvent(player, "createGarageBlip", GarageManager.Garages[house.GarageID].Position);


                    Dictionary<string, object> HouseResponseData = new Dictionary<string, object>();

                    HouseResponseData.Add("blip", house.Position);
                    HouseResponseData.Add("garagePosition", GarageManager.Garages[house.GarageID].Position - new Vector3(0, 0, 1.12));

                    ResponseData.Add("house", HouseResponseData);
                }*/

                Dictionary<string, object> spawnMenu = new Dictionary<string, object>();
                Dictionary<string, object> createCharacter = new Dictionary<string, object>();

                spawnMenu["open"] = false;
                createCharacter["open"] = false;
                Customization.LoadCharacter(player);
                if (!Customization.CustomPlayerData.ContainsKey(UUID) || !Customization.CustomPlayerData[UUID].IsCreated)
                {
                    ResponseData.Add("nextState", "createCharacter");
                    createCharacter["open"] = true;

                    Dictionary<string, object> createCharacterData = Customization.CreateCharacter(player);

                    createCharacter.Add("data", createCharacterData);
                }
                else
                {
                    try
                    {
                        ResponseData.Add("nextState", "spawnMenu");

                        spawnMenu["open"] = true;
                        spawnMenu.Add("startpos", GlobalCollections.RandomSpawns[Rnd.Next(0, GlobalCollections.RandomSpawns.Count)]);
                        spawnMenu.Add("pos", SpawnPos);
                        spawnMenu.Add("faction", (FractionID > 0) ? true : false);
                        spawnMenu.Add("house", (/*house != null ||*/ HotelID != -1) ? true : false);

                        NAPI.Task.Run(() => Customization.ApplyCharacter(player));
                    }
                    catch (Exception e)
                    {
                        Log.Write($"{e.ToString()}");
                    }
                }

                ResponseData.Add("spawnMenu", spawnMenu);
                ResponseData.Add("characterCreator", createCharacter);
                ResponseData.Add("characterId", UUID);
                ResponseData.Add("characterFirstName", FirstName);
                ResponseData.Add("characterLastName", LastName);

                Dictionary<string, object> warnData = new Dictionary<string, object>();

                warnData.Add("hasBeenDecrease", false);

                if (Warns > 0 && DateTime.Now > Unwarn)
                {
                    Warns--;

                    if (Warns > 0)
                    {
                        Unwarn = DateTime.Now.AddDays(14);
                    }

                    warnData.Add("hasBeenDecrease", true);
                    warnData.Add("currentWarns", Warns);
                    // Notify.Send(player, Notify.Type.Warning, Notify.Position.BottomCenter, $"Одно предупреждение было снято. У Вас осталось {Warns}", 3000);
                }

                ResponseData.Add("warn", warnData);

                /*if (!Dashboard.isopen.ContainsKey(player)) {
                    // TODO: maybe need remove it?)
                    Dashboard.isopen.Add(player, false);
                }

                nInventory.Check(UUID);

                if (nInventory.Find(UUID, ItemType.BagWithMoney) != null) {
                    nInventory.Remove(player, ItemType.BagWithMoney, 1);
                }

                if (nInventory.Find(UUID, ItemType.BagWithDrill) != null) {
                    nInventory.Remove(player, ItemType.BagWithDrill, 1);
                }*/

                return ResponseData;
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"Spawn\":\n" + e.ToString());

                Console.WriteLine(e);

                return null;
            }
        }

        public async Task<Dictionary<string, object>> Load(Player player, int uuid) {
            try {
                if (Main.Players.ContainsKey(player)) {
                    Main.Players.Remove(player);
                }

                MySqlCommand queryCommand = new MySqlCommand(@"
                    SELECT * FROM `characters`
                    WHERE `uuid` = @UUID
                ");

                queryCommand.Parameters.AddWithValue("@UUID", uuid);

                DataTable result = await MySQL.QueryResultAsync(queryCommand);

                if (result == null) {
                    return null;
                }

                foreach (DataRow Row in result.Rows) {
                    UUID = Convert.ToInt32(Row["uuid"]);
                    FirstName = Convert.ToString(Row["firstname"]);
                    LastName = Convert.ToString(Row["lastname"]);
                    Gender = Convert.ToBoolean(Row["gender"]);
                    Health = Convert.ToInt32(Row["health"]);
                    Armor = Convert.ToInt32(Row["armor"]);
                    LVL = Convert.ToInt32(Row["lvl"]);
                    EXP = Convert.ToInt32(Row["exp"]);
                    Money = Convert.ToInt64(Row["money"]);
                    Bank = Convert.ToInt32(Row["bank"]);
                    WorkID = Convert.ToInt32(Row["work"]);
                    FractionID = Convert.ToInt32(Row["fraction"]);
                    FractionLVL = Convert.ToInt32(Row["fractionlvl"]);
                    ArrestTime = Convert.ToInt32(Row["arrest"]);
                    DemorganTime = Convert.ToInt32(Row["demorgan"]);
                    DemorganData = JsonConvert.DeserializeObject<Demorgan>(Row["demorganData"].ToString());

                    if (DemorganData == null) {
                        DemorganData = new Demorgan(0, "nope", "nope", 0);
                    }

                    if (DemorganTime > 0) {
                        DemorganData.Time = DemorganTime;
                        DemorganTime = 0;
                        Demorgan.DeleteOldDemorgan(UUID);
                    }

                    WantedLVL = JsonConvert.DeserializeObject<WantedLevel>(Row["wanted"].ToString());
                    BizIDs = JsonConvert.DeserializeObject<List<int>>(Row["biz"].ToString());
                    AdminLVL = Convert.ToInt32(Row["adminlvl"]);
                    Licenses = JsonConvert.DeserializeObject<List<bool>>(Row["licenses"].ToString());

                    if (Licenses.Count == 8) {
                        Licenses.Add(false);
                    }

                    Unwarn = ((DateTime)Row["unwarn"]);
                    Unmute = ((DateTime)Row["unmute"]);
                    Warns = Convert.ToInt32(Row["warns"]);
                    LastVeh = Convert.ToString(Row["lastveh"]);
                    OnDuty = Convert.ToBoolean(Row["onduty"]);
                    LastHourMin = Convert.ToInt32(Row["lasthour"]);
                    HotelID = Convert.ToInt32(Row["hotel"]);
                    HotelLeft = Convert.ToInt32(Row["hotelleft"]);
                    Contacts = JsonConvert.DeserializeObject<Dictionary<int, string>>(Row["contacts"].ToString());
                    Sim = Convert.ToInt32(Row["sim"]);
                    DrugAddiction = Convert.ToDouble(Row["drugsAddiction"]);
                    CreateDate = ((DateTime)Row["createdate"]);

                    if (DrugAddiction >= 100) {
                        DrugAddiction = 100;
                    }

                    /*HungerData = JsonConvert.DeserializeObject<Hunger>(Row["hunger"].ToString());

                    if (HungerData == null) {
                        HungerData = new Hunger(50, 0);
                    }*/

                    SpawnPos = JsonConvert.DeserializeObject<Vector3>(Row["pos"].ToString());

                    if (Row["pos"].ToString().Contains("NaN") || SpawnPos == new Vector3(0, 0, 0)) {
                        Log.Debug("Detected wrong coordinates!", nLog.Type.Warn);
                        SpawnPos = GlobalCollections.RandomSpawns[Rnd.Next(0, GlobalCollections.RandomSpawns.Count)];
                    }

                    ApplyAddiction(player);
                }

                player.Name = FirstName + "_" + LastName;

                player.SetData(GlobalVariables.DrugAddictionKey, DrugAddiction);

                Main.Players.Add(player, this);

                if (player.SocialClubName != "kemperrr") {
                    GameLog.Connected(player, player.SocialClubName, player.Serial, player.Value, player.Address);
                }

                Dictionary<string, object> spawnData = Spawn(player);

                return spawnData;
            } catch (Exception e) {
                Log.Write("EXCEPTION AT \"Load\":\n" + e.ToString());

                Console.WriteLine(e);

                return null;
            }
        }

        /*public async Task<bool> SaveAuthSchoolProgress(Client player)
        {
            try
            {
                MySqlCommand queryCommand = new MySqlCommand(@"
                    UPDATE `characters`
                    SET
                        `money` = @MONEY,
                        `licenses` = @LICENSES
                    WHERE
                        `uuid` = @UUID
                ");

                queryCommand.Parameters.AddWithValue("@MONEY", Money);
                queryCommand.Parameters.AddWithValue("@LICENSES", JsonConvert.SerializeObject(Licenses));
                queryCommand.Parameters.AddWithValue("@UUID", UUID);

                await MySQL.QueryAsync(queryCommand);

                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"Error in saving users AutoSchool progress. Msg: {ex.Message.ToString()}");
                return false;
            }
        }*/

        public void ApplyAddiction(Player player) {
            if (DrugAddiction < 15) {
                player.SetSharedData("ADD_HP", 0);
            } else if (DrugAddiction < 30) {
                player.SetSharedData("ADD_HP", 10);
            } else if (DrugAddiction < 50) {
                player.SetSharedData("ADD_HP", 25);
            } else if (DrugAddiction < 75) {
                player.SetSharedData("ADD_HP", 40);
            } else if (DrugAddiction < 100) {
                player.SetSharedData("ADD_HP", 60);
            } else {
                player.SetSharedData("ADD_HP", 70);
                DrugAddiction = 100;
            }
        }

        public async Task<bool> Save(Player player)
        {
            try
            {
                Customization.SaveCharacter(player);

                Vector3 LPos = (player.IsInVehicle) ? player.Vehicle.Position + new Vector3(0, 0, 0.5) : player.Position;
                string pos = JsonConvert.SerializeObject(LPos);
                try
                {
                    /*if (InsideHouseID != -1)
                    {
                        House house = HouseManager.Houses.FirstOrDefault(h => h.ID == InsideHouseID);
                        if (house != null)
                            pos = JsonConvert.SerializeObject(house.Position + new Vector3(0, 0, 1.12));
                    }

                    if (InsideGarageID != -1)
                    {
                        Garage garage = GarageManager.Garages[InsideGarageID];
                        pos = JsonConvert.SerializeObject(garage.Position + new Vector3(0, 0, 1.12));
                    }*/

                    if (ExteriorPos != new Vector3())
                    {
                        Vector3 position = ExteriorPos;
                        pos = JsonConvert.SerializeObject(position + new Vector3(0, 0, 1.12));
                    }

                    /*if (InsideHotelID != -1)
                    {
                        Vector3 position = Houses.Hotel.HotelEnters[InsideHotelID];
                        pos = JsonConvert.SerializeObject(position + new Vector3(0, 0, 1.12));
                    }

                    if (TuningShop != -1)
                    {
                        Vector3 position = BusinessManager.BizList[TuningShop].EnterPoint;
                        pos = JsonConvert.SerializeObject(position + new Vector3(0, 0, 1.12));
                    }*/
                }
                catch (Exception e) {
                    Log.Write("EXCEPTION AT \"UnLoadPos\":\n" + e.ToString());
                }

               /* try
                {
                    if (IsSpawned && !IsAlive)
                    {
                        pos = JsonConvert.SerializeObject(Fractions.Ems.emsCheckpoints[2]);
                        Health = 20;
                        Armor = 0;
                    }
                    else
                    {
                        Health = player.Health;
                        Armor = player.Armor;
                    }
                }
                catch (Exception e) { Log.Write("EXCEPTION AT \"UnLoadHP\":\n" + e.ToString()); }
               
                try
                {
                    var aItem = nInventory.Find(UUID, ItemType.BodyArmor);
                    if (aItem != null && aItem.IsActive)
                    {
                        string data = aItem.Data;
                        var split = data.Split('_');
                        if (split.Length < 3)
                            aItem.Data = $"{Armor}_0_0";
                        else
                            aItem.Data = $"{Armor}_{split[1]}_{split[2]}";
                    }
                }
                catch (Exception e) { Log.Write("EXCEPTION AT \"UnLoadArmorItem\":\n" + e.ToString()); }

                try
                {
                    var all_vehicles = VehicleManager.getAllPlayerVehicles(UUID);
                    foreach (var number in all_vehicles)
                        VehicleManager.Save(number);
                }
                catch (Exception e) { Log.Write("EXCEPTION AT \"UnLoadVehicles\":\n" + e.ToString()); }
               */
                if (!IsSpawned)
                    pos = JsonConvert.SerializeObject(SpawnPos);

                //Main.PlayerSlotsInfo[UUID] = new Tuple<int, int, int, long>(LVL, EXP, FractionID, Money);

                MySqlCommand queryCommand = new MySqlCommand(@"
                    UPDATE `characters`
                    SET
                        `pos` = @POSITION,
                        `gender` = @GENDER,
                        `health` = @HEALTH,
                        `armor` = @ARMOR,
                        `lvl` = @LEVEL,
                        `exp` = @EXP,
                        `money` = @MONEY,
                        `bank` = @BANK,
                        `work` = @WORK,
                        `fraction` = @FRACTION,
                        `fractionlvl` = @FRACTION_LEVEL,
                        `arrest` = @ARREST_TIME,
                        `wanted` = @WANTED,
                        `biz` = @BIZ,
                        `adminlvl` = @ADMIN_LEVEL,
                        `licenses` = @LICENSES,
                        `unwarn` = @UNWARN_TIME,
                        `unmute` = @UNMUTE_TIME,
                        `warns` = @WARNS,
                        `hotel` = @HOTEL,
                        `hotelleft` = @HOTELLEFT,
                        `lastveh` = @LASTVEH,
                        `onduty` = @ONDUTY,
                        `lasthour` = @LASTHOUR,
                        `demorganData` = @DEMORGAN,
                        `contacts` = @CONTACTS,
                        `sim` = @SIM_CARD,
                        `drugsAddiction` = @DRUG_ADDICTION,
                        `hunger` = @HUNGER,
                        `firstname` = @FIRSTNAME,
                        `lastname` = @LASTNAME
                    WHERE
                        `uuid` = @UUID
                ");

                queryCommand.Parameters.AddWithValue("@POSITION", pos);
                queryCommand.Parameters.AddWithValue("@GENDER", Gender);
                queryCommand.Parameters.AddWithValue("@HEALTH", Math.Clamp(Health, 0, 126));
                queryCommand.Parameters.AddWithValue("@ARMOR", Math.Clamp(Armor, 0, 126));
                queryCommand.Parameters.AddWithValue("@LEVEL", LVL);
                queryCommand.Parameters.AddWithValue("@EXP", EXP);
                queryCommand.Parameters.AddWithValue("@MONEY", Money);
                queryCommand.Parameters.AddWithValue("@BANK", Bank);
                queryCommand.Parameters.AddWithValue("@WORK", WorkID);
                queryCommand.Parameters.AddWithValue("@FRACTION", FractionID);
                queryCommand.Parameters.AddWithValue("@FRACTION_LEVEL", FractionLVL);
                queryCommand.Parameters.AddWithValue("@ARREST_TIME", ArrestTime);
                queryCommand.Parameters.AddWithValue("@WANTED", JsonConvert.SerializeObject(WantedLVL));
                queryCommand.Parameters.AddWithValue("@BIZ", JsonConvert.SerializeObject(BizIDs));
                queryCommand.Parameters.AddWithValue("@ADMIN_LEVEL", AdminLVL);
                queryCommand.Parameters.AddWithValue("@LICENSES", JsonConvert.SerializeObject(Licenses));
                queryCommand.Parameters.AddWithValue("@UNWARN_TIME", MySQL.ConvertTime(Unwarn));
                queryCommand.Parameters.AddWithValue("@UNMUTE_TIME", MySQL.ConvertTime(Unmute));
                queryCommand.Parameters.AddWithValue("@WARNS", Warns);
                queryCommand.Parameters.AddWithValue("@HOTEL", HotelID);
                queryCommand.Parameters.AddWithValue("@HOTELLEFT", HotelLeft);
                queryCommand.Parameters.AddWithValue("@LASTVEH", LastVeh);
                queryCommand.Parameters.AddWithValue("@ONDUTY", OnDuty);
                queryCommand.Parameters.AddWithValue("@LASTHOUR", LastHourMin);
                queryCommand.Parameters.AddWithValue("@DEMORGAN", JsonConvert.SerializeObject(DemorganData));
                queryCommand.Parameters.AddWithValue("@CONTACTS", JsonConvert.SerializeObject(Contacts));
                queryCommand.Parameters.AddWithValue("@SIM_CARD", Sim);
                queryCommand.Parameters.AddWithValue("@UUID", UUID);
                queryCommand.Parameters.AddWithValue("@DRUG_ADDICTION", DrugAddiction);
                queryCommand.Parameters.AddWithValue("@HUNGER", /*JsonConvert.SerializeObject(HungerData)*/0);
                queryCommand.Parameters.AddWithValue("@FIRSTNAME", FirstName);
                queryCommand.Parameters.AddWithValue("@LASTNAME", LastName);

                await MySQL.QueryAsync(queryCommand);

                //MoneySystem.Bank.Save(Bank);
                Log.Debug($"Player [{FirstName}:{LastName}] was saved.");
                return true;
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"Save\":\n" + e.ToString());
                return false;
            }
        }

        public async Task<int> Create(Player player) {
            try {
                if (Main.Players.ContainsKey(player)) {
                    Log.Debug("Main.Players.ContainsKey(player)", nLog.Type.Error);
                    return -1;
                }

                /*if (firstName.Length < 1) {
                    // Notify.Send(player, Notify.Type.Error, Notify.Position.BottomCenter, "Ошибка в длине имени/фамилии", 3000);
                    return -2;
                }

                if (lastName.Length < 1) {
                    // Notify.Send(player, Notify.Type.Error, Notify.Position.BottomCenter, "Ошибка в длине имени/фамилии", 3000);
                    return -3;
                }*/

                /*if (Main.PlayerNames.ContainsValue($"{firstName}_{lastName}")) {
                    // Notify.Send(player, Notify.Type.Error, Notify.Position.BottomCenter, "Данное имя уже занято", 3000);
                    return -4;
                }*/
                //Bank = MoneySystem.Bank.Create(UUID);
                Licenses = new List<bool>() { false, false, false, false, false, false, false, false, false };
                SpawnPos = GlobalCollections.RandomSpawns[Rnd.Next(0, GlobalCollections.RandomSpawns.Count)];

                /*Main.PlayerSlotsInfo.Add(UUID, new Tuple<int, int, int, long>(LVL, EXP, FractionID, Money));
                Main.PlayerUUIDs.Add($"{firstName}_{lastName}", UUID);
                Main.PlayerNames.Add(UUID, $"{firstName}_{lastName}");*/

                FirstName = "Нет";
                LastName = "Имени";

                MySqlCommand queryCommand = new MySqlCommand(@"
                    INSERT INTO
                        `characters` (
                            `firstname`,
                            `lastname`,
                            `gender`,
                            `health`,
                            `armor`,
                            `lvl`,
                            `exp`,
                            `money`,
                            `bank`,
                            `work`,
                            `fraction`,
                            `fractionlvl`,
                            `arrest`,
                            `demorgan`,
                            `demorganData`,
                            `wanted`,
                            `biz`,
                            `adminlvl`,
                            `licenses`,
                            `unwarn`,
                            `unmute`,
                            `warns`,
                            `lastveh`,
                            `onduty`,
                            `lasthour`,
                            `hotel`,
                            `hotelleft`,
                            `contacts`,
                            `sim`,
                            `pos`,
                            `createdate`,
                            `drugsAddiction`,
                            `hunger`
                        )
                    VALUES
                        (
                            @FIRST_NAME,
                            @LAST_NAME,
                            @GENDER,
                            @HEALTH,
                            @ARMOR,
                            @LVL,
                            @EXP,
                            @MONEY,
                            @BANK,
                            @WORK,
                            @FRACTION,
                            @FRACTIONLVL,
                            @ARREST,
                            @DEMORGAN,
                            @DEMORGANDATA,
                            @WANTED,
                            @BIZ,
                            @ADMINLVL,
                            @LICENSES,
                            @UNWARN,
                            @UNMUTE,
                            @WARNS,
                            @LASTVEH,
                            @ONDUTY,
                            @LASTHOUR,
                            @HOTEL,
                            @HOTELLEFT,
                            @CONTACTS,
                            @SIM,
                            @POS,
                            @CREATEDATE,
                            @DRUGSADDICTION,
                            @HUNGER
                        )
                ");

                queryCommand.Parameters.AddWithValue("@FIRST_NAME", FirstName);
                queryCommand.Parameters.AddWithValue("@LAST_NAME", LastName);
                queryCommand.Parameters.AddWithValue("@GENDER", Gender);
                queryCommand.Parameters.AddWithValue("@HEALTH", Health);
                queryCommand.Parameters.AddWithValue("@ARMOR", Armor);
                queryCommand.Parameters.AddWithValue("@LVL", LVL);
                queryCommand.Parameters.AddWithValue("@EXP", EXP);
                queryCommand.Parameters.AddWithValue("@MONEY", Money);
                queryCommand.Parameters.AddWithValue("@BANK", Bank);
                queryCommand.Parameters.AddWithValue("@WORK", WorkID);
                queryCommand.Parameters.AddWithValue("@FRACTION", FractionID);
                queryCommand.Parameters.AddWithValue("@FRACTIONLVL", FractionLVL);
                queryCommand.Parameters.AddWithValue("@ARREST", ArrestTime);
                queryCommand.Parameters.AddWithValue("@DEMORGAN", 0);
                queryCommand.Parameters.AddWithValue("@DEMORGANDATA", JsonConvert.SerializeObject(DemorganData));
                queryCommand.Parameters.AddWithValue("@WANTED", JsonConvert.SerializeObject(WantedLVL));
                queryCommand.Parameters.AddWithValue("@BIZ", JsonConvert.SerializeObject(BizIDs));
                queryCommand.Parameters.AddWithValue("@ADMINLVL", AdminLVL);
                queryCommand.Parameters.AddWithValue("@LICENSES", JsonConvert.SerializeObject(Licenses));
                queryCommand.Parameters.AddWithValue("@UNWARN", MySQL.ConvertTime(Unwarn));
                queryCommand.Parameters.AddWithValue("@UNMUTE", MySQL.ConvertTime(Unmute));
                queryCommand.Parameters.AddWithValue("@WARNS", Warns);
                queryCommand.Parameters.AddWithValue("@LASTVEH", LastVeh);
                queryCommand.Parameters.AddWithValue("@ONDUTY", OnDuty);
                queryCommand.Parameters.AddWithValue("@LASTHOUR", LastHourMin);
                queryCommand.Parameters.AddWithValue("@HOTEL", HotelID);
                queryCommand.Parameters.AddWithValue("@HOTELLEFT", HotelLeft);
                queryCommand.Parameters.AddWithValue("@CONTACTS", JsonConvert.SerializeObject(Contacts));
                queryCommand.Parameters.AddWithValue("@SIM", Sim);
                queryCommand.Parameters.AddWithValue("@POS", JsonConvert.SerializeObject(SpawnPos));
                queryCommand.Parameters.AddWithValue("@CREATEDATE", MySQL.ConvertTime(CreateDate));
                queryCommand.Parameters.AddWithValue("@DRUGSADDICTION", 0);
                queryCommand.Parameters.AddWithValue("@HUNGER", /*JsonConvert.SerializeObject(HungerData)*/0);

                await MySQL.QueryAsync(queryCommand);

                UUID = Convert.ToInt32(queryCommand.LastInsertedId);
                
                NAPI.Task.Run(() => {
                    player.Name = FirstName + "_" + LastName;
                });

                //nInventory.Check(UUID);

                Main.Players.Add(player, this);

                return UUID;
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"Create\":\n" + e.ToString());
                return -5;
            }
        }

        /*private async Task<int> GenerateUUID()
        {
            var result = 333333;
            while (Main.UUIDs.Contains(result))
                result = Rnd.Next(000001, 999999);

            Main.UUIDs.Add(result);
            return result;
        }*/

        // COMMON

        public static Dictionary<string, string> toChange = new Dictionary<string, string>();
        private static MySqlCommand nameCommand;

        public Character()
        {
            nameCommand = new MySqlCommand("UPDATE `characters` SET `firstname`=@fn, `lastname`=@ln WHERE `uuid`=@uuid");
        }
        /*
        public static async Task changeName(string oldName)
        {
            try
            {
                if (!toChange.ContainsKey(oldName)) return;

                string newName = toChange[oldName];

                //int UUID = Main.PlayerNames.FirstOrDefault(u => u.Value == oldName).Key;
                int Uuid = Main.PlayerUUIDs.GetValueOrDefault(oldName);
                if (Uuid <= 0)
                {
                    await Log.WriteAsync($"Cant'find UUID of player [{oldName}]", nLog.Type.Warn);
                    return;
                }

                string[] split = newName.Split("_");
                await Log.DebugAsync($"UUID: {Uuid}");
                await Log.DebugAsync($"SPLIT: {split[0]} {split[1]}");

                Main.PlayerNames[Uuid] = newName;
                MySqlCommand cmd = nameCommand;
                cmd.Parameters.AddWithValue("@fn", split[0]);
                cmd.Parameters.AddWithValue("@ln", split[1]);
                cmd.Parameters.AddWithValue("@uuid", Uuid);
                await MySQL.QueryAsync(cmd);

                lock (Main.PlayerNames)
                {
                    lock (Main.PlayerUUIDs)
                    {
                        Main.PlayerNames[Uuid] = newName;
                        Main.PlayerUUIDs.Remove(oldName);
                        Main.PlayerUUIDs.Add(newName, Uuid);
                    }
                }

                await Log.DebugAsync("Nickname has been changed!", nLog.Type.Success);
                toChange.Remove(oldName);
                MoneySystem.Donations.Rename(oldName, newName);
                GameLog.Name(Uuid, oldName, newName);
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"CHANGENAME\":\n" + e.ToString(), nLog.Type.Error);
            }
        }*/
    }
    public class WantedLevel
    {
        public int Level { get; set; }
        public string WhoGive { get; set; }
        public DateTime Date { get; set; }
        public string Reason { get; set; }

        public WantedLevel(int level, string whoGive, DateTime date, string reason)
        {
            Level = level;
            WhoGive = whoGive;
            Date = date;
            Reason = reason;
        }
    }

    public class Demorgan
    {
        private static nLog Log = new nLog("Character.Demorgan");
        public int Time { get; set; }
        public string Reason { get; set; }
        public string Admin { get; set; }
        public int MoreTime { get; set; }
        public Demorgan(int time, string reason, string admin, int moretime = 0)
        {
            Time = time;
            Reason = reason;
            Admin = admin;
            MoreTime = moretime;
        }

        public static void DeleteOldDemorgan(int UUID)
        {
            try {
                MySqlCommand queryCommand = new MySqlCommand(@"UPDATE `characters`
                SET
                    `demorgan` = 0
                WHERE
                    `UUID` = @UUID");

                queryCommand.Parameters.AddWithValue("@UUID", UUID);

                MySQL.Query(queryCommand);
            }
            catch (Exception ex) {
                Log.Write($"Exception at DeleteOldDemorgan with {ex.Message}");
            }
        }
    }

   /*public class Hunger
    {
        private static nLog Log = new nLog("Character.Hunger");
        #region Hunger logic

        public static void Hunger_tick() {
            try {
                Log.Write($"Hunger tick - start");

                foreach (var player in Main.Players.Keys.ToList()) {
                    if (Main.DebugHunger) {
                        Log.Write($"Hunger change start - {player.Name}");
                    }

                    if (!Main.Players.ContainsKey(player) || (player.HasSharedData("IS_ADMIN") && player.GetSharedData("IS_ADMIN"))) {
                        if (Main.DebugHunger) {
                            Log.Write($"Hunger change continue - {player.Name}");
                        }

                        continue;
                    }

                    Hunger_timer(player);

                    if (Main.DebugHunger) {
                        Log.Write($"Hunger change end - {player.Name}");
                    }
                }
            } catch (Exception ex) {
                Log.Write($"Exception at Hunger_tick at {ex.ToString()}");
            }
        }

        private static void Hunger_timer(Client player) {
            try
            {
                Hunger hungerData = Main.Players[player].HungerData;

                int thirts = (int)hungerData.Value;

                if (thirts >= 100) {
                    hungerData.Value = (float)100.0;

                    if (hungerData.HungerTime++ >= 3) {
                        if ((hungerData.HungerTime % 3) == 0) {
                            NAPI.Player.SetPlayerHealth(player, player.Health - (100 * 2 / player.Health));
                        }

                        if (hungerData.HungerTime >= 21) {
                            hungerData.HungerTime = 1;
                            GUI.Notify.Send(player, Notify.Type.Alert, Notify.Position.BottomCenter, $"Вы сильно страдаете от голода, вам нужно поесть.", 5000);
                        }
                    }
                } else {
                    switch (thirts) {
                        case 60: {
                            if (!player.HasData("HUNGER_NOTIFY") || player.GetData("HUNGER_NOTIFY") < 60) {
                                GUI.Notify.Send(player, Notify.Type.Info, Notify.Position.BottomCenter, $"Вы чувствуете легкий голод, надо бы поесть.", 5000);
                            }
                            break;
                        }
                        case 80: {
                            if (!player.HasData("HUNGER_NOTIFY") || player.GetData("HUNGER_NOTIFY") < 80) {
                                GUI.Notify.Send(player, Notify.Type.Alert, Notify.Position.BottomCenter, $"Чувствуется сильный голод, начинает урчать в животе, нужно поесть.", 5000);
                            }
                            break;
                        }
                        case 90: {
                            if (!player.HasData("HUNGER_NOTIFY") || player.GetData("HUNGER_NOTIFY") < 90) {
                                GUI.Notify.Send(player, Notify.Type.Alert, Notify.Position.BottomCenter, $"Вы сильно голодны, вот-вот начнете страдать от голода, нужно поесть.", 5000);
                            }
                            break;
                        }
                    }

                    hungerData.ChangeHunger(player, (float)0.833333333);

                    if (Main.DebugHunger) {
                        Log.Write($"Hunger changed - {player.Name}");
                    }
                }
            } catch (Exception e) {
                Log.Write($"Exception at Hunger_timer with: {e.Message}");
            }
        }
        #endregion

        public static float GetHunger(Client player, bool invert = false)
        {
            return invert ? (100 - Main.Players[player].HungerData.Value) : Main.Players[player].HungerData.Value;
        }

        public void ChangeHunger(Client player, float value)
        {
            if (Main.DebugHunger) {
                Log.Write($"Hunger change request - {player.Name}");
            }

            Main.Players[player].HungerData.Value = Math.Clamp(Main.Players[player].HungerData.Value + (float)value, 0, 100);

            player.SetData("HUNGER_NOTIFY", (int)Main.Players[player].HungerData.Value);

            Dictionary<string, object> updatedData = new Dictionary<string, object>(){
                {"hunger", GetHunger(player, true)}
            };

            Trigger.ClientEvent(player, "account.client.update", JsonConvert.SerializeObject(updatedData));
        }

        public bool SetHunger(Client player, float value)
        {
            try{
                if (player == null){
                    return false;
                }

                Hunger HungerData = Main.Players[player].HungerData;

                HungerData.Value = Math.Clamp(value, 0, 100);

                return true;
            }
            catch (Exception ex){
                Log.Write($"Exception at SetHunger with {ex.Message}");
                return false;
            }
        }

        public float Value { get; set; } = 50;

        public int HungerTime { get; set; } = 0;

        public Hunger(float value, int time){
            Value = value;
            HungerTime = time;
        }
    }*/
}
