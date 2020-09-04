using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading;
using GTANetworkAPI;
using Newtonsoft.Json;
using SecondLive.Core.Logging;
using SecondLive.GUI;
using MySql.Data.MySqlClient;
using Object = System.Object;
using Timer = System.Threading.Timer;

namespace SecondLive.Core
{
    enum ItemType
    {
        Mask = -1, // Маска
        Gloves = -3, // Перчатки
        Leg = -4, // Штанишки
        Bag = -5, // Рюкзачок
        Feet = -6, // Обуточки
        Jewelry = -7, // Аксессуарчики всякие там
        Undershit = -8, // Рубашечки
        BodyArmor = -9, // Бронька
        Unknown = -10, // Вообще хер пойми что это
        Top = -11, // Верх
        Hat = -12, // Шляпы
        Glasses = -13, // Очочки
        Accessories = -14, // Часы/Браслеты

        Debug = 0,
        BagWithMoney = 12,// Сумка с деньгами
        Material = 13,    // Материалы
        Drugs = 14,       // Наркота
        BagWithDrill = 15,// Сумка с дрелью
        HealthKit = 1,    // Аптечка
        GasCan = 2,       // Канистра
        Сrisps = 3,       // Чипсы
        Beer = 4,         // Пиво
        Pizza = 5,        // Пицца
        Burger = 6,       // Бургер
        HotDog = 7,       // Хот-Дог
        Sandwich = 8,     // Сэндвич
        eCola = 9,        // Кока-Кола
        Sprunk = 10,      // Спрайт
        Lockpick = 11,    // Отмычка для замка
        ArmyLockpick = 16,// Военная отмычка
        Pocket = 17,      // Мешок
        Cuffs = 18,       // Стяжки
        CarKey = 19,      // Ключи от личной машины
        Present = 40,     // Подарок
        KeyRing = 41,     // Связка ключей
        Cigarette = 42,   // Сигарета
        Bandage = 43,     // Бинты 
        MiniMedkit = 44,  // Мини аптечка
        Toksikanol = 45, //  Тфксиканол (анти-нарко)
        Antinark = 46,    // Антинаркотическая таблетка

        /* Drinks */
        RusDrink1 = 20,
        RusDrink2 = 21,
        RusDrink3 = 22,

        YakDrink1 = 23,
        YakDrink2 = 24,
        YakDrink3 = 25,

        LcnDrink1 = 26,
        LcnDrink2 = 27,
        LcnDrink3 = 28,

        ArmDrink1 = 29,
        ArmDrink2 = 30,
        ArmDrink3 = 31,

        /* Weapons */
        /* Pistols */
        Pistol = 100,
        CombatPistol = 101,
        Pistol50 = 102,
        SNSPistol = 103,
        HeavyPistol = 104,
        VintagePistol = 105,
        MarksmanPistol = 106,
        Revolver = 107,
        APPistol = 108,
        FlareGun = 110,
        DoubleAction = 111,
        PistolMk2 = 112,
        SNSPistolMk2 = 113,
        RevolverMk2 = 114,
        /* SMG */
        MicroSMG = 115,
        MachinePistol = 116,
        SMG = 117,
        AssaultSMG = 118,
        CombatPDW = 119,
        MG = 120,
        CombatMG = 121,
        Gusenberg = 122,
        MiniSMG = 123,
        SMGMk2 = 124,
        CombatMGMk2 = 125,
        /* Rifles */
        AssaultRifle = 126,
        CarbineRifle = 127,
        AdvancedRifle = 128,
        SpecialCarbine = 129,
        BullpupRifle = 130,
        CompactRifle = 131,
        AssaultRifleMk2 = 132,
        CarbineRifleMk2 = 133,
        SpecialCarbineMk2 = 134,
        BullpupRifleMk2 = 135,
        /* Sniper */
        SniperRifle = 136,
        HeavySniper = 137,
        MarksmanRifle = 138,
        HeavySniperMk2 = 139,
        MarksmanRifleMk2 = 140,
        /* Shotguns */
        PumpShotgun = 141,
        SawnOffShotgun = 142,
        BullpupShotgun = 143,
        AssaultShotgun = 144,
        Musket = 145,
        HeavyShotgun = 146,
        DoubleBarrelShotgun = 147,
        SweeperShotgun = 148,
        PumpShotgunMk2 = 149,
        /* MELEE WEAPONS */
        StunGun = 109,
        Knife = 180,
        Nightstick = 181,
        Hammer = 182,
        Bat = 183,
        Crowbar = 184,
        GolfClub = 185,
        Bottle = 186,
        Dagger = 187,
        Hatchet = 188,
        KnuckleDuster = 189,
        Machete = 190,
        Flashlight = 191,
        SwitchBlade = 192,
        PoolCue = 193,
        Wrench = 194,
        BattleAxe = 195,
        /* Ammo */
        PistolAmmo = 200,
        SMGAmmo = 201,
        RiflesAmmo = 202,
        SniperAmmo = 203,
        ShotgunsAmmo = 204,
        DrugSeed = 230,
    }

    class nInventory : Script
    {
        public static Dictionary<int, string> ItemsNames = new Dictionary<int, string>
        {
            {-1, "Маска" },
            {-3, "Перчатки" },
            {-4, "Штаны"},
            {-5, "Рюкзак"},
            {-6, "Обувь"},
            {-7, "Аксессуар"},
            {-8, "Нижняя одежда"},
            {-9, "Бронежилет"},
            {-10, "Украшения"},
            {-11, "Верхняя одежда" },
            {-12, "Головной убор" },
            {-13, "Очки" },
            {-14, "Аксессуар" },
            {1, "Аптечка"},
            {2, "Канистра"},
            {3, "Чипсы"},
            {4, "Пиво"},
            {5, "Пицца"},
            {6, "Бургер"},
            {7, "Хот-Дог"},
            {8, "Сэндвич"},
            {9, "eCola"},
            {10, "Sprunk"},
            {11, "Отмычка для замков"},
            {12, "Сумка с деньгами"},
            {13, "Материалы"},
            {14, "Наркотики"},
            {15, "Сумка с дрелью"},
            {16, "Военная отмычка"},
            {17, "Мешок"},
            {18, "Стяжки"},
            {19, "Ключи от машины"},
            {40, "Подарок"},
            {41, "Связка ключей"},
            {42, "Сигарета"},
            {43, "Бинт"},
            {44, "Мини-аптечка"},
            {45, "Токсиканол"},
            {46, "Антинарк"},

            {20, "«На корке лимона»"},
            {21, "«На бруснике»"},
            {22, "«Русский стандарт»"},
            {23, "«Asahi»"},
            {24, "«Midori»"},
            {25, "«Yamazaki»"},
            {26, "«Martini Asti»"},
            {27, "«Sambuca»"},
            {28, "«Campari»"},
            {29, "«Дживан»"},
            {30, "«Арарат»"},
            {31, "«Noyan Tapan»"},

            {100, "Pistol" },
            {101, "Combat Pistol" },
            {102, "Pistol 50" },
            {103, "SNS Pistol" },
            {104, "Heavy Pistol" },
            {105, "Vintage Pistol" },
            {106, "Marksman Pistol" },
            {107, "Revolver" },
            {108, "AP Pistol" },
            {109, "Stun Gun" },
            {110, "Flare Gun" },
            {111, "Double Action" },
            {112, "Pistol Mk2" },
            {113, "SNSPistol Mk2" },
            {114, "Revolver Mk2" },

            {115, "Micro SMG" },
            {116, "Machine Pistol" },
            {117, "SMG" },
            {118, "Assault SMG" },
            {119, "Combat PDW" },
            {120, "MG" },
            {121, "Combat MG" },
            {122, "Gusenberg" },
            {123, "Mini SMG" },
            {124, "SMG Mk2" },
            {125, "Combat MG Mk2" },

            {126, "Assault Rifle" },
            {127, "Carbine Rifle" },
            {128, "Advanced Rifle" },
            {129, "Special Carbine" },
            {130, "Bullpup Rifle" },
            {131, "Compact Rifle" },
            {132, "Assault Rifle Mk2" },
            {133, "Carbine Rifle Mk2" },
            {134, "Special Carbine Mk2" },
            {135, "Bullpup Rifle Mk2" },

            {136, "Sniper Rifle" },
            {137, "Heavy Sniper" },
            {138, "Marksman Rifle" },
            {139, "Heavy Sniper Mk2" },
            {140, "Marksman Rifle Mk2" },

            {141, "Pump Shotgun" },
            {142, "SawnOff Shotgun" },
            {143, "Bullpup Shotgun" },
            {144, "Assault Shotgun" },
            {145, "Musket" },
            {146, "Heavy Shotgun" },
            {147, "Double Barrel Shotgun" },
            {148, "Sweeper Shotgun" },
            {149, "Pump Shotgun Mk2" },

            {180, "Нож" },
            {181, "Дубинка" },
            {182, "Молоток" },
            {183, "Бита" },
            {184, "Лом" },
            {185, "Гольф клюшка" },
            {186, "Бутылка" },
            {187, "Кинжал" },
            {188, "Топор" },
            {189, "Кастет" },
            {190, "Мачете" },
            {191, "Фонарик" },
            {192, "Швейцарский нож" },
            {193, "Кий" },
            {194, "Ключ" },
            {195, "Боевой топор" },

            {200, "Пистолетный калибр" },
            {201, "Малый калибр" },
            {202, "Автоматный калибр" },
            {203, "Снайперский калибр" },
            {204, "Дробь" },

            {230, "Семена наркотиков"},
        };
        public static Dictionary<int, string> ItemsDescriptions = new Dictionary<int, string>();
        public static Dictionary<ItemType, uint> ItemModels = new Dictionary<ItemType, uint>()
        {
            { ItemType.Hat, 1619813869 },
            { ItemType.Mask, 3887136870 },
            { ItemType.Gloves, 3125389411 },
            { ItemType.Leg, 2086911125 },
            { ItemType.Bag, 0000000 },
            { ItemType.Feet, 1682675077 },
            { ItemType.Jewelry, 2329969874 },
            { ItemType.Undershit, 578126062 },
            { ItemType.BodyArmor, 701173564 },
            { ItemType.Unknown, 0000000 },
            { ItemType.Top, 3038378640 },
            { ItemType.Glasses, 2329969874 },
            { ItemType.Accessories, 2329969874 },

            { ItemType.Drugs, 4293279169 },
            { ItemType.Material, 3045218749 },
            { ItemType.Debug, 0000000 },
            { ItemType.HealthKit, 678958360 },
            { ItemType.GasCan, 786272259 },
            { ItemType.Сrisps, 2564432314 },
            { ItemType.Beer, 1940235411 },
            { ItemType.Pizza, 604847691 },
            { ItemType.Burger, 2240524752 },
            { ItemType.HotDog, 2565741261 },
            { ItemType.Sandwich, 987331897 },
            { ItemType.eCola, 144995201 },
            { ItemType.Sprunk, 2973713592 },
            { ItemType.Lockpick, 977923025 },
            { ItemType.ArmyLockpick, 977923025 },
            { ItemType.Pocket, 3887136870 },
            { ItemType.Cuffs, 3887136870 },
            { ItemType.CarKey, 977923025 },
            { ItemType.Present, NAPI.Util.GetHashKey("prop_box_ammo07a") },
            { ItemType.KeyRing, 977923025 },
            { ItemType.Cigarette, NAPI.Util.GetHashKey("ng_proc_cigpak01a") },
            { ItemType.Bandage, 4293279169 },
            { ItemType.MiniMedkit, 4293279169 },
            { ItemType.Toksikanol, 4293279169 },
            { ItemType.Antinark, 4293279169 },

            { ItemType.RusDrink1, NAPI.Util.GetHashKey("prop_vodka_bottle") },
            { ItemType.RusDrink2, NAPI.Util.GetHashKey("prop_vodka_bottle") },
            { ItemType.RusDrink3, NAPI.Util.GetHashKey("prop_vodka_bottle") },
            { ItemType.YakDrink1, NAPI.Util.GetHashKey("prop_cs_beer_bot_02") },
            { ItemType.YakDrink2, NAPI.Util.GetHashKey("prop_wine_red") },
            { ItemType.YakDrink3, NAPI.Util.GetHashKey("p_whiskey_bottle_s") },
            { ItemType.LcnDrink1, NAPI.Util.GetHashKey("prop_wine_white") },
            { ItemType.LcnDrink2, NAPI.Util.GetHashKey("prop_vodka_bottle") },
            { ItemType.LcnDrink3, NAPI.Util.GetHashKey("prop_wine_red") },
            { ItemType.ArmDrink1, NAPI.Util.GetHashKey("prop_bottle_cognac") },
            { ItemType.ArmDrink2, NAPI.Util.GetHashKey("prop_bottle_cognac") },
            { ItemType.ArmDrink3, NAPI.Util.GetHashKey("prop_bottle_cognac") },

            { ItemType.Pistol, NAPI.Util.GetHashKey("w_pi_pistol") },
            { ItemType.CombatPistol, NAPI.Util.GetHashKey("w_pi_combatpistol") },
            { ItemType.Pistol50, NAPI.Util.GetHashKey("w_pi_pistol50") },
            { ItemType.SNSPistol, NAPI.Util.GetHashKey("w_pi_sns_pistol") },
            { ItemType.HeavyPistol, NAPI.Util.GetHashKey("w_pi_heavypistol") },
            { ItemType.VintagePistol, NAPI.Util.GetHashKey("w_pi_vintage_pistol") },
            { ItemType.MarksmanPistol, NAPI.Util.GetHashKey("w_pi_singleshot") },
            { ItemType.Revolver, NAPI.Util.GetHashKey("w_pi_revolver") },
            { ItemType.APPistol, NAPI.Util.GetHashKey("w_pi_appistol") },
            { ItemType.StunGun, NAPI.Util.GetHashKey("w_pi_stungun") },
            { ItemType.FlareGun, NAPI.Util.GetHashKey("w_pi_flaregun") },
            { ItemType.DoubleAction, NAPI.Util.GetHashKey("mk2") },
            { ItemType.PistolMk2, NAPI.Util.GetHashKey("w_pi_pistolmk2") },
            { ItemType.SNSPistolMk2, NAPI.Util.GetHashKey("w_pi_sns_pistolmk2") },
            { ItemType.RevolverMk2, NAPI.Util.GetHashKey("w_pi_revolvermk2") },

            { ItemType.MicroSMG, NAPI.Util.GetHashKey("w_sb_microsmg") },
            { ItemType.MachinePistol, NAPI.Util.GetHashKey("w_sb_compactsmg") },
            { ItemType.SMG, NAPI.Util.GetHashKey("w_sb_smg") },
            { ItemType.AssaultSMG, NAPI.Util.GetHashKey("w_sb_assaultsmg") },
            { ItemType.CombatPDW, NAPI.Util.GetHashKey("w_sb_pdw") },
            { ItemType.MG, NAPI.Util.GetHashKey("w_mg_mg") },
            { ItemType.CombatMG, NAPI.Util.GetHashKey("w_mg_combatmg") },
            { ItemType.Gusenberg, NAPI.Util.GetHashKey("w_sb_gusenberg") },
            { ItemType.MiniSMG, NAPI.Util.GetHashKey("w_sb_minismg") },
            { ItemType.SMGMk2, NAPI.Util.GetHashKey("w_sb_smgmk2") },
            { ItemType.CombatMGMk2, NAPI.Util.GetHashKey("w_mg_combatmgmk2") },

            { ItemType.AssaultRifle, NAPI.Util.GetHashKey("w_ar_assaultrifle") },
            { ItemType.CarbineRifle, NAPI.Util.GetHashKey("w_ar_carbinerifle") },
            { ItemType.AdvancedRifle, NAPI.Util.GetHashKey("w_ar_advancedrifle") },
            { ItemType.SpecialCarbine, NAPI.Util.GetHashKey("w_ar_specialcarbine") },
            { ItemType.BullpupRifle, NAPI.Util.GetHashKey("w_ar_bullpuprifle") },
            { ItemType.CompactRifle, NAPI.Util.GetHashKey("w_ar_assaultrifle_smg") },
            { ItemType.AssaultRifleMk2, NAPI.Util.GetHashKey("w_ar_assaultriflemk2") },
            { ItemType.CarbineRifleMk2, NAPI.Util.GetHashKey("w_ar_carbineriflemk2") },
            { ItemType.SpecialCarbineMk2, NAPI.Util.GetHashKey("w_ar_specialcarbinemk2") },
            { ItemType.BullpupRifleMk2, NAPI.Util.GetHashKey("w_ar_bullpupriflemk2") },

            { ItemType.SniperRifle, NAPI.Util.GetHashKey("w_sr_sniperrifle") },
            { ItemType.HeavySniper, NAPI.Util.GetHashKey("w_sr_heavysniper") },
            { ItemType.MarksmanRifle, NAPI.Util.GetHashKey("w_sr_marksmanrifle") },
            { ItemType.HeavySniperMk2, NAPI.Util.GetHashKey("w_sr_heavysnipermk2") },
            { ItemType.MarksmanRifleMk2, NAPI.Util.GetHashKey("w_sr_marksmanriflemk2") },

            { ItemType.PumpShotgun, NAPI.Util.GetHashKey("w_sg_pumpshotgun") },
            { ItemType.SawnOffShotgun, NAPI.Util.GetHashKey("w_sg_sawnoff") },
            { ItemType.BullpupShotgun, NAPI.Util.GetHashKey("w_sg_bullpupshotgun") },
            { ItemType.AssaultShotgun, NAPI.Util.GetHashKey("w_sg_assaultshotgun") },
            { ItemType.Musket, NAPI.Util.GetHashKey("w_ar_musket") },
            { ItemType.HeavyShotgun, NAPI.Util.GetHashKey("w_sg_heavyshotgun") },
            { ItemType.DoubleBarrelShotgun, NAPI.Util.GetHashKey("w_sg_doublebarrel") },
            { ItemType.SweeperShotgun, NAPI.Util.GetHashKey("mk2") },
            { ItemType.PumpShotgunMk2, NAPI.Util.GetHashKey("w_sg_pumpshotgunmk2") },

            { ItemType.Knife, NAPI.Util.GetHashKey("w_me_knife_01") },
            { ItemType.Nightstick, NAPI.Util.GetHashKey("w_me_nightstick") },
            { ItemType.Hammer, NAPI.Util.GetHashKey("w_me_hammer") },
            { ItemType.Bat, NAPI.Util.GetHashKey("w_me_bat") },
            { ItemType.Crowbar, NAPI.Util.GetHashKey("w_me_crowbar") },
            { ItemType.GolfClub, NAPI.Util.GetHashKey("w_me_gclub") },
            { ItemType.Bottle, NAPI.Util.GetHashKey("w_me_bottle") },
            { ItemType.Dagger, NAPI.Util.GetHashKey("w_me_dagger") },
            { ItemType.Hatchet, NAPI.Util.GetHashKey("w_me_hatchet") },
            { ItemType.KnuckleDuster, NAPI.Util.GetHashKey("w_me_knuckle") },
            { ItemType.Machete, NAPI.Util.GetHashKey("prop_ld_w_me_machette") },
            { ItemType.Flashlight, NAPI.Util.GetHashKey("w_me_flashlight") },
            { ItemType.SwitchBlade, NAPI.Util.GetHashKey("w_me_switchblade") },
            { ItemType.PoolCue, NAPI.Util.GetHashKey("prop_pool_cue") },
            { ItemType.Wrench, NAPI.Util.GetHashKey("prop_cs_wrench") },
            { ItemType.BattleAxe, NAPI.Util.GetHashKey("w_me_battleaxe") },

            { ItemType.PistolAmmo, NAPI.Util.GetHashKey("w_am_case") },
            { ItemType.RiflesAmmo, NAPI.Util.GetHashKey("w_am_case") },
            { ItemType.ShotgunsAmmo, NAPI.Util.GetHashKey("w_am_case") },
            { ItemType.SMGAmmo, NAPI.Util.GetHashKey("w_am_case") },
            { ItemType.SniperAmmo, NAPI.Util.GetHashKey("w_am_case") },

            { ItemType.DrugSeed, NAPI.Util.GetHashKey("bkr_prop_weed_dry_02b")},
        };

        public static Dictionary<ItemType, nItemSize> ItemsSize = new Dictionary<ItemType, nItemSize>()
        {
            {ItemType.Top, new nItemSize(2, 2)},
            {ItemType.Leg, new nItemSize(2, 3)},
        };

        public static Dictionary<ItemType, Vector3> ItemsPosOffset = new Dictionary<ItemType, Vector3>()
        {
            { ItemType.Hat, new Vector3(0, 0, -0.93) },
            { ItemType.Mask, new Vector3(0, 0, -1) },
            { ItemType.Gloves, new Vector3(0, 0, -1) },
            { ItemType.Leg, new Vector3(0, 0, -0.85) },
            { ItemType.Bag, new Vector3() },
            { ItemType.Feet, new Vector3(0, 0, -0.95) },
            { ItemType.Jewelry, new Vector3(0, 0, -0.98) },
            { ItemType.Undershit, new Vector3(0, 0, -0.98) },
            { ItemType.BodyArmor, new Vector3(0, 0, -0.88) },
            { ItemType.Unknown, new Vector3() },
            { ItemType.Top, new Vector3(0, 0, -0.96) },
            { ItemType.Glasses, new Vector3(0, 0, -0.98) },
            { ItemType.Accessories, new Vector3(0, 0, -0.98) },

            { ItemType.Drugs, new Vector3(0, 0, -0.95) },
            { ItemType.Material, new Vector3(0, 0, -0.6) },
            { ItemType.Debug, new Vector3() },
            { ItemType.HealthKit, new Vector3(0, 0, -0.9) },
            { ItemType.Toksikanol, new Vector3(0, 0, -0.9) },
            { ItemType.GasCan, new Vector3(0, 0, -1) },
            { ItemType.Сrisps, new Vector3(0, 0, -1) },
            { ItemType.Beer, new Vector3(0, 0, -1) },
            { ItemType.Pizza, new Vector3(0, 0, -1) },
            { ItemType.Burger, new Vector3(0, 0, -0.97) },
            { ItemType.HotDog, new Vector3(0, 0, -0.97) },
            { ItemType.Sandwich, new Vector3(0, 0, -0.99) },
            { ItemType.eCola, new Vector3(0, 0, -1) },
            { ItemType.Sprunk, new Vector3(0, 0, -1) },
            { ItemType.Lockpick, new Vector3(0, 0, -0.98) },
            { ItemType.ArmyLockpick, new Vector3(0, 0, -0.98) },
            { ItemType.Pocket, new Vector3(0, 0, -0.98) },
            { ItemType.Cuffs, new Vector3(0, 0, -0.98) },
            { ItemType.CarKey, new Vector3(0, 0, -0.98) },
            { ItemType.Present, new Vector3(0, 0, -0.98) },
            { ItemType.KeyRing, new Vector3(0, 0, -0.98) },
            { ItemType.Cigarette, new Vector3(0, 0, -0.98) },
            { ItemType.Bandage, new Vector3(0, 0, -0.98) },
            { ItemType.MiniMedkit, new Vector3(0, 0, -0.98) },
            { ItemType.Antinark, new Vector3(0, 0, -0.98) },

            { ItemType.RusDrink1, new Vector3(0, 0, -1) },
            { ItemType.RusDrink2, new Vector3(0, 0, -1) },
            { ItemType.RusDrink3, new Vector3(0, 0, -1) },
            { ItemType.YakDrink1, new Vector3(0, 0, -0.87) },
            { ItemType.YakDrink2, new Vector3(0, 0, -1) },
            { ItemType.YakDrink3, new Vector3(0, 0, -0.87) },
            { ItemType.LcnDrink1, new Vector3(0, 0, -1) },
            { ItemType.LcnDrink2, new Vector3(0, 0, -1) },
            { ItemType.LcnDrink3, new Vector3(0, 0, -1) },
            { ItemType.ArmDrink1, new Vector3(0, 0, -1) },
            { ItemType.ArmDrink2, new Vector3(0, 0, -1) },
            { ItemType.ArmDrink3, new Vector3(0, 0, -1) },

            { ItemType.Pistol, new Vector3(0, 0, -0.99) },
            { ItemType.CombatPistol, new Vector3(0, 0, -0.99) },
            { ItemType.Pistol50, new Vector3(0, 0, -0.99) },
            { ItemType.SNSPistol, new Vector3(0, 0, -0.99) },
            { ItemType.HeavyPistol, new Vector3(0, 0, -0.99) },
            { ItemType.VintagePistol, new Vector3(0, 0, -0.99) },
            { ItemType.MarksmanPistol, new Vector3(0, 0, -0.99) },
            { ItemType.Revolver, new Vector3(0, 0, -0.99) },
            { ItemType.APPistol, new Vector3(0, 0, -0.99) },
            { ItemType.StunGun, new Vector3(0, 0, -0.99) },
            { ItemType.FlareGun, new Vector3(0, 0, -0.99) },
            { ItemType.DoubleAction, new Vector3(0, 0, -0.99) },
            { ItemType.PistolMk2, new Vector3(0, 0, -0.99) },
            { ItemType.SNSPistolMk2, new Vector3(0, 0, -0.99) },
            { ItemType.RevolverMk2, new Vector3(0, 0, -0.99) },

            { ItemType.MicroSMG, new Vector3(0, 0, -0.99) },
            { ItemType.MachinePistol, new Vector3(0, 0, -0.99) },
            { ItemType.SMG, new Vector3(0, 0, -0.99) },
            { ItemType.AssaultSMG, new Vector3(0, 0, -0.99) },
            { ItemType.CombatPDW, new Vector3(0, 0, -0.99) },
            { ItemType.MG, new Vector3(0, 0, -0.99) },
            { ItemType.CombatMG, new Vector3(0, 0, -0.99) },
            { ItemType.Gusenberg, new Vector3(0, 0, -0.99) },
            { ItemType.MiniSMG, new Vector3(0, 0, -0.99) },
            { ItemType.SMGMk2, new Vector3(0, 0, -0.99) },
            { ItemType.CombatMGMk2, new Vector3(0, 0, -0.99) },

            { ItemType.AssaultRifle, new Vector3(0, 0, -0.99) },
            { ItemType.CarbineRifle, new Vector3(0, 0, -0.99) },
            { ItemType.AdvancedRifle, new Vector3(0, 0, -0.99) },
            { ItemType.SpecialCarbine, new Vector3(0, 0, -0.99) },
            { ItemType.BullpupRifle, new Vector3(0, 0, -0.99) },
            { ItemType.CompactRifle, new Vector3(0, 0, -0.99) },
            { ItemType.AssaultRifleMk2, new Vector3(0, 0, -0.99) },
            { ItemType.CarbineRifleMk2, new Vector3(0, 0, -0.99) },
            { ItemType.SpecialCarbineMk2, new Vector3(0, 0, -0.99) },
            { ItemType.BullpupRifleMk2, new Vector3(0, 0, -0.99) },

            { ItemType.SniperRifle, new Vector3(0, 0, -0.99) },
            { ItemType.HeavySniper, new Vector3(0, 0, -0.99) },
            { ItemType.MarksmanRifle, new Vector3(0, 0, -0.99) },
            { ItemType.HeavySniperMk2, new Vector3(0, 0, -0.99) },
            { ItemType.MarksmanRifleMk2, new Vector3(0, 0, -0.99) },

            { ItemType.PumpShotgun, new Vector3(0, 0, -0.99) },
            { ItemType.SawnOffShotgun, new Vector3(0, 0, -0.99) },
            { ItemType.BullpupShotgun, new Vector3(0, 0, -0.99) },
            { ItemType.AssaultShotgun, new Vector3(0, 0, -0.99) },
            { ItemType.Musket, new Vector3(0, 0, -0.99) },
            { ItemType.HeavyShotgun, new Vector3(0, 0, -0.99) },
            { ItemType.DoubleBarrelShotgun, new Vector3(0, 0, -0.99) },
            { ItemType.SweeperShotgun, new Vector3(0, 0, -0.99) },
            { ItemType.PumpShotgunMk2, new Vector3(0, 0, -0.99) },

            { ItemType.Knife, new Vector3(0, 0, -0.99) },
            { ItemType.Nightstick, new Vector3(0, 0, -0.99) },
            { ItemType.Hammer, new Vector3(0, 0, -0.99) },
            { ItemType.Bat, new Vector3(0, 0, -0.99) },
            { ItemType.Crowbar, new Vector3(0, 0, -0.99) },
            { ItemType.GolfClub, new Vector3(0, 0, -0.99) },
            { ItemType.Bottle, new Vector3(0, 0, -0.99) },
            { ItemType.Dagger, new Vector3(0, 0, -0.99) },
            { ItemType.Hatchet, new Vector3(0, 0, -0.99) },
            { ItemType.KnuckleDuster, new Vector3(0, 0, -0.99) },
            { ItemType.Machete, new Vector3(0, 0, -0.99) },
            { ItemType.Flashlight, new Vector3(0, 0, -0.99) },
            { ItemType.SwitchBlade, new Vector3(0, 0, -0.99) },
            { ItemType.PoolCue, new Vector3(0, 0, -0.99) },
            { ItemType.Wrench, new Vector3(0, 0, -0.985) },
            { ItemType.BattleAxe, new Vector3(0, 0, -0.99) },

            { ItemType.PistolAmmo, new Vector3(0, 0, -1) },
            { ItemType.RiflesAmmo, new Vector3(0, 0, -1) },
            { ItemType.ShotgunsAmmo, new Vector3(0, 0, -1) },
            { ItemType.SMGAmmo, new Vector3(0, 0, -1) },
            { ItemType.SniperAmmo, new Vector3(0, 0, -1) },

            { ItemType.DrugSeed, new Vector3(0, 0, -0.95)},
        };
        public static Dictionary<ItemType, Vector3> ItemsRotOffset = new Dictionary<ItemType, Vector3>()
        {
            { ItemType.Hat, new Vector3() },
            { ItemType.Mask, new Vector3() },
            { ItemType.Gloves, new Vector3(90, 0, 0) },
            { ItemType.Leg, new Vector3() },
            { ItemType.Bag, new Vector3() },
            { ItemType.Feet, new Vector3() },
            { ItemType.Jewelry, new Vector3() },
            { ItemType.Undershit, new Vector3() },
            { ItemType.BodyArmor, new Vector3(90, 90, 0) },
            { ItemType.Unknown, new Vector3() },
            { ItemType.Top, new Vector3() },
            { ItemType.Glasses, new Vector3() },
            { ItemType.Accessories, new Vector3() },

            { ItemType.Drugs, new Vector3() },
            { ItemType.Material, new Vector3() },
            { ItemType.Debug, new Vector3() },
            { ItemType.HealthKit, new Vector3() },
            { ItemType.Toksikanol, new Vector3() },
            { ItemType.GasCan, new Vector3() },
            { ItemType.Сrisps, new Vector3(90, 90, 0) },
            { ItemType.Beer, new Vector3() },
            { ItemType.Pizza, new Vector3() },
            { ItemType.Burger, new Vector3() },
            { ItemType.HotDog, new Vector3() },
            { ItemType.Sandwich, new Vector3() },
            { ItemType.eCola, new Vector3() },
            { ItemType.Sprunk, new Vector3() },
            { ItemType.Lockpick, new Vector3() },
            { ItemType.ArmyLockpick, new Vector3() },
            { ItemType.Pocket, new Vector3() },
            { ItemType.Cuffs, new Vector3() },
            { ItemType.CarKey, new Vector3() },
            { ItemType.Present, new Vector3() },
            { ItemType.KeyRing, new Vector3() },
            { ItemType.Cigarette, new Vector3() },
            { ItemType.Bandage, new Vector3() },
            { ItemType.MiniMedkit, new Vector3() },
            { ItemType.Antinark, new Vector3() },

            { ItemType.RusDrink1, new Vector3() },
            { ItemType.RusDrink2, new Vector3() },
            { ItemType.RusDrink3, new Vector3() },
            { ItemType.YakDrink1, new Vector3() },
            { ItemType.YakDrink2, new Vector3() },
            { ItemType.YakDrink3, new Vector3() },
            { ItemType.LcnDrink1, new Vector3() },
            { ItemType.LcnDrink2, new Vector3() },
            { ItemType.LcnDrink3, new Vector3() },
            { ItemType.ArmDrink1, new Vector3() },
            { ItemType.ArmDrink2, new Vector3() },
            { ItemType.ArmDrink3, new Vector3() },

            { ItemType.Pistol, new Vector3(90, 0, 0) },
            { ItemType.CombatPistol, new Vector3(90, 0, 0) },
            { ItemType.Pistol50, new Vector3(90, 0, 0) },
            { ItemType.SNSPistol, new Vector3(90, 0, 0) },
            { ItemType.HeavyPistol, new Vector3(90, 0, 0) },
            { ItemType.VintagePistol, new Vector3(90, 0, 0) },
            { ItemType.MarksmanPistol, new Vector3(90, 0, 0) },
            { ItemType.Revolver, new Vector3(90, 0, 0) },
            { ItemType.APPistol, new Vector3(90, 0, 0) },
            { ItemType.StunGun, new Vector3(90, 0, 0) },
            { ItemType.FlareGun, new Vector3(90, 0, 0) },
            { ItemType.DoubleAction, new Vector3(90, 0, 0) },
            { ItemType.PistolMk2, new Vector3(90, 0, 0) },
            { ItemType.SNSPistolMk2, new Vector3(90, 0, 0) },
            { ItemType.RevolverMk2, new Vector3(90, 0, 0) },

            { ItemType.MicroSMG, new Vector3(90, 0, 0) },
            { ItemType.MachinePistol, new Vector3(90, 0, 0) },
            { ItemType.SMG, new Vector3(90, 0, 0) },
            { ItemType.AssaultSMG, new Vector3(90, 0, 0) },
            { ItemType.CombatPDW, new Vector3(90, 0, 0) },
            { ItemType.MG, new Vector3(90, 0, 0) },
            { ItemType.CombatMG, new Vector3(90, 0, 0) },
            { ItemType.Gusenberg, new Vector3(90, 0, 0) },
            { ItemType.MiniSMG, new Vector3(90, 0, 0) },
            { ItemType.SMGMk2, new Vector3(90, 0, 0) },
            { ItemType.CombatMGMk2, new Vector3(90, 0, 0) },

            { ItemType.AssaultRifle, new Vector3(90, 0, 0) },
            { ItemType.CarbineRifle, new Vector3(90, 0, 0) },
            { ItemType.AdvancedRifle, new Vector3(90, 0, 0) },
            { ItemType.SpecialCarbine, new Vector3(90, 0, 0) },
            { ItemType.BullpupRifle, new Vector3(90, 0, 0) },
            { ItemType.CompactRifle, new Vector3(90, 0, 0) },
            { ItemType.AssaultRifleMk2, new Vector3(90, 0, 0) },
            { ItemType.CarbineRifleMk2, new Vector3(90, 0, 0) },
            { ItemType.SpecialCarbineMk2, new Vector3(90, 0, 0) },
            { ItemType.BullpupRifleMk2, new Vector3(90, 0, 0) },

            { ItemType.SniperRifle, new Vector3(90, 0, 0) },
            { ItemType.HeavySniper, new Vector3(90, 0, 0) },
            { ItemType.MarksmanRifle, new Vector3(90, 0, 0) },
            { ItemType.HeavySniperMk2, new Vector3(90, 0, 0) },
            { ItemType.MarksmanRifleMk2, new Vector3(90, 0, 0) },

            { ItemType.PumpShotgun, new Vector3(90, 0, 0) },
            { ItemType.SawnOffShotgun, new Vector3(90, 0, 0) },
            { ItemType.BullpupShotgun, new Vector3(90, 0, 0) },
            { ItemType.AssaultShotgun, new Vector3(90, 0, 0) },
            { ItemType.Musket, new Vector3(90, 0, 0) },
            { ItemType.HeavyShotgun, new Vector3(90, 0, 0) },
            { ItemType.DoubleBarrelShotgun, new Vector3(90, 0, 0) },
            { ItemType.SweeperShotgun, new Vector3(90, 0, 0) },
            { ItemType.PumpShotgunMk2, new Vector3(90, 0, 0) },

            { ItemType.Knife, new Vector3(90, 0, 0) },
            { ItemType.Nightstick, new Vector3(90, 0, 0) },
            { ItemType.Hammer, new Vector3(90, 0, 0) },
            { ItemType.Bat, new Vector3(90, 0, 0) },
            { ItemType.Crowbar, new Vector3(90, 0, 0) },
            { ItemType.GolfClub, new Vector3(90, 0, 0) },
            { ItemType.Bottle, new Vector3(90, 0, 0) },
            { ItemType.Dagger, new Vector3(90, 0, 0) },
            { ItemType.Hatchet, new Vector3(90, 0, 0) },
            { ItemType.KnuckleDuster, new Vector3(90, 0, 0) },
            { ItemType.Machete, new Vector3(90, 0, 0) },
            { ItemType.Flashlight, new Vector3(90, 0, 0) },
            { ItemType.SwitchBlade, new Vector3(90, 0, 0) },
            { ItemType.PoolCue, new Vector3(90, 0, 0) },
            { ItemType.Wrench, new Vector3(-12, 0, 0) },
            { ItemType.BattleAxe, new Vector3(90, 0, 0) },

            { ItemType.PistolAmmo, new Vector3(90, 0, 0) },
            { ItemType.RiflesAmmo, new Vector3(90, 0, 0) },
            { ItemType.ShotgunsAmmo, new Vector3(90, 0, 0) },
            { ItemType.SMGAmmo, new Vector3(90, 0, 0) },
            { ItemType.SniperAmmo, new Vector3(90, 0, 0) },

            { ItemType.DrugSeed, new Vector3()},
        };

        public static Dictionary<ItemType, int> ItemsStacks = new Dictionary<ItemType, int>()
        {
            { ItemType.BagWithMoney, 1 },
            { ItemType.Material, 300 },
            { ItemType.Drugs, 50 },
            { ItemType.BagWithDrill, 1 },
            { ItemType.Debug, 10000 },
            { ItemType.HealthKit, 10 },
            { ItemType.Toksikanol, 5 },
            { ItemType.GasCan, 2 },
            { ItemType.Сrisps, 4 },
            { ItemType.Beer, 5 },
            { ItemType.Pizza, 3 },
            { ItemType.Burger, 4 },
            { ItemType.HotDog, 5 },
            { ItemType.Sandwich, 7 },
            { ItemType.eCola, 5 },
            { ItemType.Sprunk, 5 },
            { ItemType.Lockpick, 10 },
            { ItemType.ArmyLockpick, 10 },
            { ItemType.Pocket, 5 },
            { ItemType.Cuffs, 5 },
            { ItemType.CarKey, 1 },
            { ItemType.Present, 1 },
            { ItemType.KeyRing, 1 },
            { ItemType.Cigarette, 20 },
            { ItemType.Bandage, 2 },
            { ItemType.MiniMedkit, 5 },
            { ItemType.Antinark, 5 },

            { ItemType.Mask, 1 },
            { ItemType.Gloves, 1 },
            { ItemType.Leg, 1 },
            { ItemType.Bag, 1 },
            { ItemType.Feet, 1 },
            { ItemType.Jewelry, 1 },
            { ItemType.Undershit, 1 },
            { ItemType.BodyArmor, 1 },
            { ItemType.Unknown, 1 },
            { ItemType.Top, 1 },
            { ItemType.Hat, 1 },
            { ItemType.Glasses, 1 },
            { ItemType.Accessories, 1 },

            { ItemType.RusDrink1, 5 },
            { ItemType.RusDrink2, 5 },
            { ItemType.RusDrink3, 5 },

            { ItemType.YakDrink1, 5 },
            { ItemType.YakDrink2, 5 },
            { ItemType.YakDrink3, 5 },

            { ItemType.LcnDrink1, 5 },
            { ItemType.LcnDrink2, 5 },
            { ItemType.LcnDrink3, 5 },

            { ItemType.ArmDrink1, 5 },
            { ItemType.ArmDrink2, 5 },
            { ItemType.ArmDrink3, 5 },

            { ItemType.Pistol, 1 },
            { ItemType.CombatPistol, 1 },
            { ItemType.Pistol50, 1 },
            { ItemType.SNSPistol, 1 },
            { ItemType.HeavyPistol, 1 },
            { ItemType.VintagePistol, 1 },
            { ItemType.MarksmanPistol, 1 },
            { ItemType.Revolver, 1 },
            { ItemType.APPistol, 1 },
            { ItemType.StunGun, 1 },
            { ItemType.FlareGun, 1 },
            { ItemType.DoubleAction, 1 },
            { ItemType.PistolMk2, 1 },
            { ItemType.SNSPistolMk2, 1 },
            { ItemType.RevolverMk2, 1 },

            { ItemType.MicroSMG, 1 },
            { ItemType.MachinePistol, 1 },
            { ItemType.SMG, 1 },
            { ItemType.AssaultSMG, 1 },
            { ItemType.CombatPDW, 1 },
            { ItemType.MG, 1 },
            { ItemType.CombatMG, 1 },
            { ItemType.Gusenberg, 1 },
            { ItemType.MiniSMG, 1 },
            { ItemType.SMGMk2, 1 },
            { ItemType.CombatMGMk2, 1 },

            { ItemType.AssaultRifle, 1 },
            { ItemType.CarbineRifle, 1 },
            { ItemType.AdvancedRifle, 1 },
            { ItemType.SpecialCarbine, 1 },
            { ItemType.BullpupRifle, 1 },
            { ItemType.CompactRifle, 1 },
            { ItemType.AssaultRifleMk2, 1 },
            { ItemType.CarbineRifleMk2, 1 },
            { ItemType.SpecialCarbineMk2, 1 },
            { ItemType.BullpupRifleMk2, 1 },

            { ItemType.SniperRifle, 1 },
            { ItemType.HeavySniper, 1 },
            { ItemType.MarksmanRifle, 1 },
            { ItemType.HeavySniperMk2, 1 },
            { ItemType.MarksmanRifleMk2, 1 },

            { ItemType.PumpShotgun, 1 },
            { ItemType.SawnOffShotgun, 1 },
            { ItemType.BullpupShotgun, 1 },
            { ItemType.AssaultShotgun, 1 },
            { ItemType.Musket, 1 },
            { ItemType.HeavyShotgun, 1 },
            { ItemType.DoubleBarrelShotgun, 1 },
            { ItemType.SweeperShotgun, 1 },
            { ItemType.PumpShotgunMk2, 1 },

            { ItemType.Knife, 1 },
            { ItemType.Nightstick, 1 },
            { ItemType.Hammer, 1 },
            { ItemType.Bat, 1 },
            { ItemType.Crowbar, 1 },
            { ItemType.GolfClub, 1 },
            { ItemType.Bottle, 1 },
            { ItemType.Dagger, 1 },
            { ItemType.Hatchet, 1 },
            { ItemType.KnuckleDuster, 1 },
            { ItemType.Machete, 1 },
            { ItemType.Flashlight, 1 },
            { ItemType.SwitchBlade, 1 },
            { ItemType.PoolCue, 1 },
            { ItemType.Wrench, 1 },
            { ItemType.BattleAxe, 1 },

            { ItemType.PistolAmmo, 150 },
            { ItemType.RiflesAmmo, 300 },
            { ItemType.ShotgunsAmmo, 150 },
            { ItemType.SMGAmmo, 200 },
            { ItemType.SniperAmmo, 20 },

            { ItemType.DrugSeed, 1},
        };

        public static List<ItemType> ClothesItems = new List<ItemType>()
        {
            ItemType.Mask,
            ItemType.Gloves,
            ItemType.Leg,
            ItemType.Bag,
            ItemType.Feet,
            ItemType.Jewelry,
            ItemType.Undershit,
            ItemType.BodyArmor,
            ItemType.Unknown,
            ItemType.Top,
            ItemType.Hat,
            ItemType.Glasses,
            ItemType.Accessories,
        };
        public static List<ItemType> WeaponsItems = new List<ItemType>()
        {
            ItemType.Pistol,
            ItemType.CombatPistol,
            ItemType.Pistol50,
            ItemType.SNSPistol,
            ItemType.HeavyPistol,
            ItemType.VintagePistol,
            ItemType.MarksmanPistol,
            ItemType.Revolver,
            ItemType.APPistol,
            ItemType.FlareGun,
            ItemType.DoubleAction,
            ItemType.PistolMk2,
            ItemType.SNSPistolMk2,
            ItemType.RevolverMk2,

            ItemType.MicroSMG,
            ItemType.MachinePistol,
            ItemType.SMG,
            ItemType.AssaultSMG,
            ItemType.CombatPDW,
            ItemType.MG,
            ItemType.CombatMG,
            ItemType.Gusenberg,
            ItemType.MiniSMG,
            ItemType.SMGMk2,
            ItemType.CombatMGMk2,

            ItemType.AssaultRifle,
            ItemType.CarbineRifle,
            ItemType.AdvancedRifle,
            ItemType.SpecialCarbine,
            ItemType.BullpupRifle,
            ItemType.CompactRifle,
            ItemType.AssaultRifleMk2,
            ItemType.CarbineRifleMk2,
            ItemType.SpecialCarbineMk2,
            ItemType.BullpupRifleMk2,

            ItemType.SniperRifle,
            ItemType.HeavySniper,
            ItemType.MarksmanRifle,
            ItemType.HeavySniperMk2,
            ItemType.MarksmanRifleMk2,

            ItemType.PumpShotgun,
            ItemType.SawnOffShotgun,
            ItemType.BullpupShotgun,
            ItemType.AssaultShotgun,
            ItemType.Musket,
            ItemType.HeavyShotgun,
            ItemType.DoubleBarrelShotgun,
            ItemType.SweeperShotgun,
            ItemType.PumpShotgunMk2,
        };
        public static List<ItemType> MeleeWeaponsItems = new List<ItemType>()
        {
            ItemType.Knife,
            ItemType.Nightstick,
            ItemType.Hammer,
            ItemType.Bat,
            ItemType.Crowbar,
            ItemType.GolfClub,
            ItemType.Bottle,
            ItemType.Dagger,
            ItemType.Hatchet,
            ItemType.KnuckleDuster,
            ItemType.Machete,
            ItemType.Flashlight,
            ItemType.SwitchBlade,
            ItemType.PoolCue,
            ItemType.Wrench,
            ItemType.BattleAxe,
            ItemType.StunGun,
        };
        public static List<ItemType> AmmoItems = new List<ItemType>()
        {
            ItemType.PistolAmmo,
            ItemType.RiflesAmmo,
            ItemType.ShotgunsAmmo,
            ItemType.SMGAmmo,
            ItemType.SniperAmmo
        };
        public static List<ItemType> AlcoItems = new List<ItemType>()
        {
            ItemType.LcnDrink1,
            ItemType.LcnDrink2,
            ItemType.LcnDrink3,
            ItemType.RusDrink1,
            ItemType.RusDrink2,
            ItemType.RusDrink3,
            ItemType.YakDrink1,
            ItemType.YakDrink2,
            ItemType.YakDrink3,
            ItemType.ArmDrink1,
            ItemType.ArmDrink2,
            ItemType.ArmDrink3,
        };

        public static List<List<ItemType>> TypeItems = new List<List<ItemType>>()
        {
            ClothesItems,
            WeaponsItems,
            MeleeWeaponsItems,
            AmmoItems,
            AlcoItems
        };

        // UUID, Items by index
        public static Dictionary<int, List<nItem>> Items = new Dictionary<int, List<nItem>>();
        private static nLog Log = new nLog("nInventory");
        private static Timer SaveTimer;

        private static int Size_X = 5;
        private static int Size_Y = 10;
        private static Dictionary<int, List<bool>> Cells = new Dictionary<int, List<bool>>();

        #region Constructor
        /*[ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                Log.Write("Loading player items...", nLog.Type.Info);

                MySqlCommand queryCommand = new MySqlCommand(@"
                    SELECT * FROM `inventory`
                ");


                DataTable result = MySQL.QueryResult(queryCommand);

                if (result == null)
                {
                    Log.Write("DB return null result", nLog.Type.Warn);
                    return;
                }

                foreach (DataRow Row in result.Rows)
                {
                    int UUID = Convert.ToInt32(Row["uuid"]);
                    string json = Convert.ToString(Row["items"]);

                    List<nItem> items = JsonConvert.DeserializeObject<List<nItem>>(json);

                    items.RemoveAll(i => i.Type == ItemType.Revolver || i.Type == ItemType.HeavyShotgun || i.Type == ItemType.Gusenberg);
                    Items.Add(UUID, items);
                }

                SaveTimer = new Timer(new TimerCallback(SaveAll), null, 0, 1800000);

                Log.Write("Items loaded.", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"INVENTORY_CONSTRUCT\":\n" + e.ToString(), nLog.Type.Error);
            }
        }*/
        #endregion

        #region Add/Remove item
        public static void Add(Player player, nItem item)
        {
            try
            {
                int UUID = Main.Players[player].UUID;
                Trigger.ClientEvent(player, "add_item", JsonConvert.SerializeObject(item));
                /*int UUID = Main.Players[player].UUID;
                int index = FindIndex(UUID, item.Type);
                if (ClothesItems.Contains(item.Type) || WeaponsItems.Contains(item.Type) || item.Type == ItemType.CarKey || item.Type == ItemType.KeyRing)
                {
                    //if (WeaponsItems.Contains(item.Type)) Weapons.ChangeBackWeapon(player, item.Type, false);

                    Items[UUID].Add(item);
                    //GUI.Dashboard.Update(player, item, Items[UUID].IndexOf(item));
                }
                else
                {
                    if (index != -1)
                    {
                        int count = Items[UUID][index].Count;
                        Items[UUID][index].Count = count + item.Count;
                        //GUI.Dashboard.Update(player, Items[UUID][index], index);
                        Log.Debug($"Added existing item! {UUID.ToString()}:{index.ToString()}");
                    }
                    else
                    {
                        Items[UUID].Add(item);
                        //GUI.Dashboard.Update(player, item, Items[UUID].IndexOf(item));
                    }
                }
                Log.Debug($"Item added. {UUID.ToString()}:{index.ToString()}");*/
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"INVENTORY_ADD\":\n" + e.ToString(), nLog.Type.Error);
            }
        }
        /*public static int TryAdd(Player client, nItem item)
        {
            try
            {
                int UUID = Main.Players[client].UUID;
                int index = FindIndex(UUID, item.Type);
                int tail = 0;
                if (ClothesItems.Contains(item.Type) || item.Type == ItemType.CarKey || item.Type == ItemType.KeyRing)
                {
                    if (isFull(UUID))
                        return -1;

                    if (item.Type == ItemType.BodyArmor && index != -1)
                        return -1;
                }
                else if (WeaponsItems.Contains(item.Type))
                {
                    if (isFull(UUID))
                        return -1;

                    /*var ammoType = Weapons.WeaponsAmmoTypes[item.Type];
                    var sameTypeWeapon = Items[UUID].FirstOrDefault(i => WeaponsItems.Contains(i.Type) && Weapons.WeaponsAmmoTypes[i.Type] == ammoType);
                    if (sameTypeWeapon != null)*/
                        /*return -1;
                }
                else if (MeleeWeaponsItems.Contains(item.Type))
                {
                    if (isFull(UUID))
                        return -1;

                    var sameWeapon = Items[UUID].FirstOrDefault(i => i.Type == item.Type);
                    if (sameWeapon != null)
                        return -1;
                }
                else
                {
                    if (index != -1)
                    {
                        int max = (ItemsStacks.ContainsKey(item.Type)) ? ItemsStacks[item.Type] : 1;
                        int count = Items[UUID][index].Count;
                        int temp = count + item.Count;
                        if (temp > max)
                        {
                            tail = temp - max;
                            return tail;
                        }
                    }
                    else
                    {
                        if (item.Count > ItemsStacks[item.Type])
                        {
                            tail = item.Count - ItemsStacks[item.Type];
                            return tail;
                        }
                        else if (isFull(UUID))
                            return -1;
                    }
                }
                return tail;
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"INVENTORY_ADD\":\n" + e.ToString(), nLog.Type.Error);
                return 0;
            }
        }*/
        public static void Remove(Player player, ItemType type, int count)
        {
            try
            {
                int UUID = Main.Players[player].UUID;
                int Index = FindIndex(UUID, type);
                if (Index != -1)
                {
                    int temp = Items[UUID][Index].Count - count;
                    if (temp > 0)
                    {
                        Items[UUID][Index].Count = temp;
                        //GUI.Dashboard.Update(player, Items[UUID][Index], Index);
                    }
                    else
                    {
                        Items[UUID].RemoveAt(Index);
                        //GUI.Dashboard.sendItems(player);
                    }
                }
                Log.Debug($"Item removed. {UUID.ToString()}:{Index.ToString()}");
                return;
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"INVENTORY_REMOVE\":\n" + e.ToString(), nLog.Type.Error);
            }

        }
        public static void Remove(Player player, nItem item)
        {
            try
            {
                int UUID = Main.Players[player].UUID;

                if (ClothesItems.Contains(item.Type) || WeaponsItems.Contains(item.Type) || MeleeWeaponsItems.Contains(item.Type) || item.Type == ItemType.BagWithDrill 
                    || item.Type == ItemType.BagWithMoney || item.Type == ItemType.CarKey || item.Type == ItemType.KeyRing)
                {
                    //if (WeaponsItems.Contains(item.Type)) Weapons.ChangeBackWeapon(player, item.Type, true);

                    Items[UUID].Remove(item);
                    //GUI.Dashboard.sendItems(player);
                    Log.Debug($"Item removed. {UUID.ToString()}:TYPE {(int)item.Type}");
                }
                else
                {
                    int Index = FindIndex(UUID, item.Type);
                    if (Index != -1)
                    {
                        int temp = Items[UUID][Index].Count - item.Count;
                        if (temp > 0)
                        {
                            Items[UUID][Index].Count = temp;
                            //GUI.Dashboard.Update(player, Items[UUID][Index], Index);
                        }
                        else
                        {
                            Items[UUID].RemoveAt(Index);
                            //GUI.Dashboard.sendItems(player);
                        }
                    }
                    Log.Debug($"Item removed. {UUID.ToString()}:{Index.ToString()}");
                }
                return;
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"INVENTORY_REMOVE\":\n" + e.ToString(), nLog.Type.Error);
            }

        }
        #endregion

        #region Save/Load items
        public static void Load(Player player)
        {
            List<bool> active = new List<bool>();

            for (int i = 0; i < Size_X * Size_Y; i++)
            {
                active.Add(false);
            }

            int UUID = Main.Players[player].UUID;
            Cells.Add(UUID, active);

            Items.Add(UUID, new List<nItem>());
        }
        public static void SaveAll(object state = null)
        {
            try
            {
                Log.Write("Saving items...", nLog.Type.Info);

                if (Items.Count == 0) {
                    return;
                }

                Dictionary<int, List<nItem>> cItems = new Dictionary<int, List<nItem>>(Items);

                foreach (KeyValuePair<int, List<nItem>> kvp in cItems)
                {
                    int UUID = kvp.Key;
                    string itemsJson = JsonConvert.SerializeObject(kvp.Value);

                    MySqlCommand queryCommand = new MySqlCommand(@"
                        UPDATE `inventory`
                        SET
                            `items` = @ITEMS
                        WHERE
                            `uuid` = @UUID
                    ");

                    queryCommand.Parameters.AddWithValue("@ITEMS", itemsJson);
                    queryCommand.Parameters.AddWithValue("@UUID", UUID);

                    MySQL.Query(queryCommand);
                }

                Log.Write("Items has been saved to DB.", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"INVENTORY_SAVEALL\":\n" + e.ToString(), nLog.Type.Error);
            }
        }
        public static void Save(int UUID)
        {
            try
            {
                if (!Items.ContainsKey(UUID)) {
                    return;
                }

                Log.Write($"Saving items for {UUID}", nLog.Type.Info);

                string itemsJson = JsonConvert.SerializeObject(Items[UUID]);

                MySqlCommand queryCommand = new MySqlCommand(@"
                    UPDATE `inventory`
                    SET
                        `items` = @ITEMS
                    WHERE
                        `uuid` = @UUID
                ");

                queryCommand.Parameters.AddWithValue("@ITEMS", itemsJson);
                queryCommand.Parameters.AddWithValue("@UUID", UUID);

                MySQL.Query(queryCommand);

                Log.Write("Items has been saved to DB.", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"INVENTORY_SAVE\":\n" + e.ToString(), nLog.Type.Error);
            }
        }
        #endregion

        #region SPECIAL
        
        //public static void GetFreeCell()
        /*public static void GiveStarterItems(Player player)
        {
            nInventory.Add(player, new nItem(ItemType.Burger));
            nInventory.Add(player, new nItem(ItemType.Burger));
            nInventory.Add(player, new nItem(ItemType.Pizza));
        
        }*/
        public static nItem Find(int UUID, ItemType type)
        {
            List<nItem> items = Items[UUID];
            nItem result = items.Find(i => i.Type == type);
            return result;
        }
        public static int FindIndex(int UUID, ItemType type)
        {
            List<nItem> items = Items[UUID];
            int result = items.FindIndex(i => i.Type == type);
            return result;
        }

        /*public static bool isFull(int UUID)
        {
            
        }*/

        public static void Check(int uuid)
        {
            //if items dict does not contains account uuid, then add him
            if (!Items.ContainsKey(uuid))
            {
                Items.Add(uuid, new List<nItem>());

                // MySqlCommand queryCommand = new MySqlCommand(@"
                //     INSERT INTO
                //         `inventory` (
                //             `uuid`,
                //             `items`
                //         )
                //         VALUES
                //         (
                //             @UUID,
                //             @ITEMS
                //         )
                // ");

                // queryCommand.Parameters.AddWithValue("@UUID", uuid);
                // queryCommand.Parameters.AddWithValue("@ITEMS", "[]");

                // MySQL.Query(queryCommand);

                Log.Debug("Player added");
            }
        }

        public static void UnActiveItem(Player player, ItemType type)
        {
            var items = Items[Main.Players[player].UUID];
            foreach (var i in items)
                if (i.Type == type && i.IsActive)
                {
                    i.IsActive = false;
                    //GUI.Dashboard.Update(player, i, items.IndexOf(i));
                }
            Items[Main.Players[player].UUID] = items;
        }

        public static void ClearWithoutClothes(Player player)
        {
            try
            {
                int uuid = Main.Players[player].UUID;
                List<nItem> items = Items[uuid];
                List<nItem> upd = new List<nItem>();
                foreach (nItem item in items)
                    if (ClothesItems.Contains(item.Type) || item.Type == ItemType.CarKey || item.Type == ItemType.KeyRing) upd.Add(item);

                Items[uuid] = upd;
                //GUI.Dashboard.sendItems(player);
            }
            catch (Exception e)
            {
                Log.Write(e.ToString(), nLog.Type.Error);
            }
        }
        public static void ClearAllClothes(Player client)
        {
            try
            {
                int uuid = Main.Players[client].UUID;
                List<nItem> items = Items[uuid];
                List<nItem> upd = new List<nItem>();
                foreach (nItem item in items)
                    if (!ClothesItems.Contains(item.Type)) upd.Add(item);

                Items[uuid] = upd;
                //GUI.Dashboard.sendItems(client);
            }
            catch (Exception e)
            {
                Log.Write(e.ToString(), nLog.Type.Error);
            }
        }
        public static ItemType GetType(int id)
        {
            for(int i = 0; i < TypeItems.Count; i++)
            {
                for(int k = 0; k < TypeItems[i].Count; k++)
                {
                    if(Convert.ToInt32(TypeItems[i][k]) == id)
                    {
                        return TypeItems[i][k];
                    }
                }
            }
            return ItemType.Debug;
        }
        #endregion

        #region Events
        [RemoteEvent("server.inventory.update")]
        public static void Update(Player player, params object[] args)
        {
            int UUID = Main.Players[player].UUID;

            Items[UUID].Clear();
            Items[UUID] = JsonConvert.DeserializeObject<List<nItem>>(Convert.ToString(args[0]));

            NAPI.Util.ConsoleOutput(JsonConvert.SerializeObject(Items[UUID]));
        }
        #endregion
    }
    class Items : Script
    {
        private static nLog Log = new nLog("Items");

        // config
        private static readonly int AntNarkEfficiency = 5;
        private static readonly string UseAntiNarkKey = "USE_ANTI_NARK";
        private static readonly string AdditionalHPKey = "ADD_HP";

        public static List<int> ItemsDropped = new List<int>();
        public static List<int> InProcessering = new List<int>();
        [ServerEvent(Event.EntityDeleted)]
        public void Event_OnEntityDeleted(Entity entity)
        {
            try
            {
                if (NAPI.Entity.GetEntityType(entity) == EntityType.Object && NAPI.Data.HasEntityData(entity, "DELETETIMER"))
                {
                    Timers.Stop(NAPI.Data.GetEntityData(entity, "DELETETIMER"));
                    ItemsDropped.Remove(NAPI.Data.GetEntityData(entity, "ID"));
                    InProcessering.Remove(NAPI.Data.GetEntityData(entity, "ID"));
                }
            }
            catch (Exception e)
            {
                Log.Write("Event_OnEntityDeleted: " + e.Message, nLog.Type.Error);
            }
        }

        public static void deleteObject(GTANetworkAPI.Object obj)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    //Main.StopT(obj.GetData("DELETETIMER"), "timer_33");
                    obj.ResetData("DELETETIMER");
                    ItemsDropped.Remove(obj.GetData<int>("ID"));
                    InProcessering.Remove(obj.GetData<int>("ID"));
                    obj.Delete();
                }
                catch (Exception e)
                {
                    Log.Write("UpdateObject: " + e.Message, nLog.Type.Error);
                }
            }, 0);
        }

        /*public static void onUse(Player player, nItem item, int index)
        {
            try
            {
                var UUID = Main.Players[player].UUID;
                if (nInventory.ClothesItems.Contains(item.Type) && item.Type != ItemType.BodyArmor && item.Type != ItemType.Mask)
                {
                    var data = (string)item.Data;
                    var clothesGender = Convert.ToBoolean(data.Split('_')[2]);
                    if (clothesGender != Main.Players[player].Gender)
                    {
                        var error_gender = (clothesGender) ? "мужская" : "женская";
                        GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, $"Это {error_gender} одежда", 3000);
                        GUI.Dashboard.Close(player);
                        return;
                    }
                    if ((player.GetData("ON_DUTY") && Fractions.Manager.FractionTypes[Main.Players[player].FractionID] == 2 && Main.Players[player].FractionID != 9) || player.GetData("ON_WORK"))
                    {
                        GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, "Вы не можете использовать это сейчас", 3000);
                        GUI.Dashboard.Close(player);
                        return;
                    }
                }

                if (nInventory.WeaponsItems.Contains(item.Type) || nInventory.MeleeWeaponsItems.Contains(item.Type))
                {
                    if (item.IsActive)
                    {
                        var wHash = Weapons.GetHash(item.Type.ToString());
                        Trigger.ClientEvent(player, "takeOffWeapon", (int)wHash);
                        Commands.RPChat("me", player, $"убрал(а) {nInventory.ItemsNames[(int)item.Type]}");
                        Weapons.ChangeBackWeapon(player, item.Type, false);
                    }
                    else
                    {
                        var oldwItem = nInventory.Items[UUID].FirstOrDefault(i => (nInventory.WeaponsItems.Contains(i.Type) || nInventory.MeleeWeaponsItems.Contains(i.Type)) && i.IsActive);
                        if (oldwItem != null)
                        {
                            var oldwHash = Weapons.GetHash(oldwItem.Type.ToString());
                            Trigger.ClientEvent(player, "serverTakeOffWeapon", (int)oldwHash);
                            Weapons.ChangeBackWeapon(player, oldwItem.Type, false);
                            oldwItem.IsActive = false;
                            GUI.Dashboard.Update(player, oldwItem, nInventory.Items[UUID].IndexOf(oldwItem));
                            Commands.RPChat("me", player, $"убрал(а) {nInventory.ItemsNames[(int)oldwItem.Type]}");
                        }

                        var wHash = Weapons.GetHash(item.Type.ToString());
                        if (Weapons.WeaponsAmmoTypes.ContainsKey(item.Type))
                        {
                            var ammoItem = nInventory.Find(UUID, Weapons.WeaponsAmmoTypes[item.Type]);
                            var ammo = (ammoItem == null) ? 0 : ammoItem.Count;
                            if (ammo > Weapons.WeaponsClipsMax[item.Type]) ammo = Weapons.WeaponsClipsMax[item.Type];
                            if (ammoItem != null) nInventory.Remove(player, ammoItem.Type, ammo);
                            Trigger.ClientEvent(player, "wgive", (int)wHash, ammo, false, true);
                        }
                        else
                        {
                            Trigger.ClientEvent(player, "wgive", (int)wHash, 1, false, true);
                        }

                        Weapons.ChangeBackWeapon(player, item.Type, true);
                        Commands.RPChat("me", player, $"достал(а) {nInventory.ItemsNames[(int)item.Type]}");
                        item.IsActive = true;
                        player.SetData("LastActiveWeap", item.Type);
                        GUI.Dashboard.Update(player, item, index);
                        GUI.Dashboard.Close(player);
                    }
                    return;
                }

                if (nInventory.AmmoItems.Contains(item.Type)) return;

                if (nInventory.AlcoItems.Contains(item.Type))
                {
                    int stage = Convert.ToInt32(item.Type.ToString().Split("Drink")[1]);
                    int curStage = player.GetData("RESIST_STAGE");

                    if (player.HasData("RESIST_BAN"))
                    {
                        GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, $"Вы пьяны до такой степени, что не можете открыть бутылку", 3000);
                        return;
                    }

                    var stageTimes = new List<int>() { 0, 300, 420, 600 };

                    if (curStage == 0 || curStage == stage)
                    {
                        player.SetData("RESIST_STAGE", stage);
                        player.SetData("RESIST_TIME", player.GetData("RESIST_TIME") + stageTimes[stage]);
                    }
                    else if (curStage < stage)
                    {
                        player.SetData("RESIST_STAGE", stage);
                    }
                    else if (curStage > stage)
                    {
                        player.SetData("RESIST_TIME", player.GetData("RESIST_TIME") + stageTimes[stage]);
                    }

                    if (player.GetData("RESIST_TIME") >= 1500)
                        player.SetData("RESIST_BAN", true);

                    Trigger.ClientEvent(player, "setResistStage", player.GetData("RESIST_STAGE"));
                    BasicSync.AddAttachmnet(player, nInventory.ItemModels[item.Type], false);

                    Main.OnAntiAnim(player);
                    player.PlayAnimation("amb@world_human_drinking@beer@male@idle_a", "idle_c", 49);
                    NAPI.Task.Run(() => {
                        try
                        {
                            if (player != null)
                            {
                                if (!player.IsInVehicle) player.StopAnimation();
                                else player.SetData("ToResetAnimPhone", true);
                                Main.OffAntiAnim(player);
                                Trigger.ClientEvent(player, "startScreenEffect", "PPFilter", player.GetData("RESIST_TIME") * 1000, false);
                                BasicSync.AddAttachmnet(player, nInventory.ItemModels[item.Type], true);
                            }
                        } catch { }
                    }, 5000);

                    if (Main.Players[player].ResistTimer == null)
                        Main.Players[player].ResistTimer = Timers.StartTask(1000, () => Fractions.AlcoFabrication.ResistTimer(player));

                    Commands.RPChat("me", player, "выпил бутылку " + nInventory.ItemsNames[(int)item.Type]);
                    GameLog.Items($"player({UUID},{player.Name})", "use", Convert.ToInt32(item.Type), 1, $"{item.Data}");
                }

                var gender = Main.Players[player].Gender;
                Log.Debug("item used");

                Hunger HungerPlayer = new Hunger(Main.Players[player].HungerData.Value, Main.Players[player].HungerData.HungerTime);

                var isUsingItemSuccess = true;

                switch (item.Type)
                {
                    #region Clothes
                    case ItemType.Glasses:
                        {
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[UUID].Accessory.Glasses.Variation = -1;
                                Customization.SetProp(player, 1, -1, 0);
                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var mask = Customization.CustomPlayerData[UUID].Clothes.Mask.Variation;
                                if (Customization.MaskTypes.ContainsKey(mask) && Customization.MaskTypes[mask].Item3)
                                {
                                    GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, "Вы не можете надеть эти очки с маской", 3000);
                                    return;
                                }
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                Customization.CustomPlayerData[UUID].Accessory.Glasses = new ComponentItem(variation, texture);
                                Customization.SetProp(player, 1, variation, texture);

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            return;
                        }
                    case ItemType.Hat:
                        {
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[UUID].Accessory.Hat.Variation = -1;

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var mask = Customization.CustomPlayerData[UUID].Clothes.Mask.Variation;
                                if (Customization.MaskTypes.ContainsKey(mask) && Customization.MaskTypes[mask].Item2)
                                {
                                    GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, "Вы не можете надеть этот головной убор с маской", 3000);
                                    return;
                                }
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                Customization.CustomPlayerData[UUID].Accessory.Hat = new ComponentItem(variation, texture);

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            Customization.SetProp(player, 0, Customization.CustomPlayerData[UUID].Accessory.Hat.Variation, Customization.CustomPlayerData[UUID].Accessory.Hat.Texture);
                            return;
                        }
                    case ItemType.Mask:
                        {
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[UUID].Clothes.Mask = new ComponentItem(Customization.EmtptySlots[gender][1], 0);

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                Customization.CustomPlayerData[UUID].Clothes.Mask = new ComponentItem(variation, texture);

                                if (Customization.MaskTypes.ContainsKey(variation))
                                {
                                    if (Customization.MaskTypes[variation].Item1)
                                    {
                                        player.SetClothes(2, 0, 0);
                                    }
                                    if (Customization.MaskTypes[variation].Item2)
                                    {
                                        Customization.CustomPlayerData[UUID].Accessory.Hat.Variation = -1;
                                        nInventory.UnActiveItem(player, ItemType.Hat);
                                        Customization.SetProp(player, 0, -1, 0);
                                    }
                                    if (Customization.MaskTypes[variation].Item3)
                                    {
                                        Customization.CustomPlayerData[UUID].Accessory.Glasses.Variation = -1;
                                        nInventory.UnActiveItem(player, ItemType.Glasses);
                                        Customization.SetProp(player, 1, -1, 0);
                                    }
                                }

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            Customization.SetMask(player, Customization.CustomPlayerData[UUID].Clothes.Mask.Variation, Customization.CustomPlayerData[UUID].Clothes.Mask.Texture);
                            return;
                        }
                    case ItemType.Gloves:
                        {
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[UUID].Clothes.Gloves = new ComponentItem(0, 0);
                                Customization.CustomPlayerData[UUID].Clothes.Torso = new ComponentItem(Customization.CorrectTorso[gender][Customization.CustomPlayerData[UUID].Clothes.Top.Variation], 0);

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                if (!Customization.CorrectGloves[gender][variation].ContainsKey(Customization.CustomPlayerData[UUID].Clothes.Torso.Variation)) return;
                                Customization.CustomPlayerData[UUID].Clothes.Gloves = new ComponentItem(variation, texture);
                                Customization.CustomPlayerData[UUID].Clothes.Torso = new ComponentItem(Customization.CorrectGloves[gender][variation][Customization.CustomPlayerData[UUID].Clothes.Torso.Variation], texture);

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            player.SetClothes(3, Customization.CustomPlayerData[UUID].Clothes.Torso.Variation, Customization.CustomPlayerData[UUID].Clothes.Torso.Texture);
                            return;
                        }
                    case ItemType.Leg:
                        {
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[UUID].Clothes.Leg = new ComponentItem(Customization.EmtptySlots[gender][4], 0);

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                Customization.CustomPlayerData[UUID].Clothes.Leg = new ComponentItem(variation, texture);

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            player.SetClothes(4, Customization.CustomPlayerData[UUID].Clothes.Leg.Variation, Customization.CustomPlayerData[UUID].Clothes.Leg.Texture);
                            return;
                        }
                    case ItemType.Bag:
                        {
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[UUID].Clothes.Bag = new ComponentItem(Customization.EmtptySlots[gender][5], 0);

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                Customization.CustomPlayerData[UUID].Clothes.Bag = new ComponentItem(variation, texture);

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            player.SetClothes(5, Customization.CustomPlayerData[UUID].Clothes.Bag.Variation, Customization.CustomPlayerData[UUID].Clothes.Bag.Texture);
                            return;
                        }
                    case ItemType.Feet:
                        {
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[UUID].Clothes.Feet = new ComponentItem(Customization.EmtptySlots[gender][6], 0);

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                Customization.CustomPlayerData[UUID].Clothes.Feet = new ComponentItem(variation, texture);

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            player.SetClothes(6, Customization.CustomPlayerData[UUID].Clothes.Feet.Variation, Customization.CustomPlayerData[UUID].Clothes.Feet.Texture);
                            return;
                        }
                    case ItemType.Jewelry:
                        {
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[UUID].Clothes.Accessory = new ComponentItem(Customization.EmtptySlots[gender][7], 0);

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                Customization.CustomPlayerData[UUID].Clothes.Accessory = new ComponentItem(variation, texture);

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            player.SetClothes(7, Customization.CustomPlayerData[UUID].Clothes.Accessory.Variation, Customization.CustomPlayerData[UUID].Clothes.Accessory.Texture);
                            return;
                        }
                    case ItemType.Accessories:
                        {
                            var itemData = (string)item.Data;
                            var variation = Convert.ToInt32(itemData.Split('_')[0]);
                            var texture = Convert.ToInt32(itemData.Split('_')[1]);

                            if (item.IsActive)
                            {
                                var watchesSlot = Customization.CustomPlayerData[UUID].Accessory.Watches;
                                if (watchesSlot.Variation == variation && watchesSlot.Texture == texture)
                                {
                                    Customization.CustomPlayerData[UUID].Accessory.Watches = new ComponentItem(-1, 0);
                                    Customization.SetProp(player, 6, -1, 0);
                                }
                                else
                                {
                                    Customization.CustomPlayerData[UUID].Accessory.Bracelets = new ComponentItem(-1, 0);
                                    Customization.SetProp(player, 7, -1, 0);
                                }

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                if (Customization.CustomPlayerData[UUID].Accessory.Watches.Variation == -1)
                                {
                                    Customization.CustomPlayerData[UUID].Accessory.Watches = new ComponentItem(variation, texture);
                                    Customization.SetProp(player, 6, variation, texture);

                                    nInventory.Items[UUID][index].IsActive = true;
                                    GUI.Dashboard.Update(player, item, index);
                                }
                                else if (Customization.AccessoryRHand[gender].ContainsKey(variation))
                                {
                                    if (Customization.CustomPlayerData[UUID].Accessory.Bracelets.Variation == -1)
                                    {
                                        Customization.CustomPlayerData[UUID].Accessory.Bracelets = new ComponentItem(Customization.AccessoryRHand[gender][variation], texture);
                                        Customization.SetProp(player, 7, Customization.AccessoryRHand[gender][variation], texture);

                                        nInventory.Items[UUID][index].IsActive = true;
                                        GUI.Dashboard.Update(player, item, index);
                                    }
                                    else
                                    {
                                        GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, "Заняты обе руки", 3000);
                                        return;
                                    }
                                } else {
                                    GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, "Левая рука занята, а на правой никто часы не носит", 3000);
                                    return;
                                }
                            }
                            return;
                        }
                    case ItemType.Undershit:
                        {
                            var itemData = (string)item.Data;
                            var underwearID = Convert.ToInt32(itemData.Split('_')[0]);
                            var underwear = Customization.Underwears[gender][underwearID];
                            var texture = Convert.ToInt32(itemData.Split('_')[1]);
                            if (item.IsActive)
                            {
                                if (underwear.Top == Customization.CustomPlayerData[UUID].Clothes.Top.Variation)
                                    Customization.CustomPlayerData[UUID].Clothes.Top = new ComponentItem(Customization.EmtptySlots[gender][11], 0);
                                Customization.CustomPlayerData[UUID].Clothes.Undershit = new ComponentItem(Customization.EmtptySlots[gender][8], 0);

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                if (Customization.CustomPlayerData[UUID].Clothes.Top.Variation == Customization.EmtptySlots[gender][11])
                                {
                                    if (underwear.Top == -1)
                                    {
                                        GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, "Эту одежду можно одеть только под низ верхней", 3000);
                                        return;
                                    }
                                    Customization.CustomPlayerData[UUID].Clothes.Top = new ComponentItem(underwear.Top, texture);

                                    nInventory.UnActiveItem(player, item.Type);
                                    nInventory.Items[UUID][index].IsActive = true;
                                    GUI.Dashboard.Update(player, item, index);
                                }
                                else
                                {
                                    var nowTop = Customization.Tops[gender].FirstOrDefault(t => t.Variation == Customization.CustomPlayerData[UUID].Clothes.Top.Variation);
                                    if (nowTop != null)
                                    {
                                        var topType = nowTop.Type;
                                        if (!underwear.UndershirtIDs.ContainsKey(topType))
                                        {
                                            GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, "Эта одежда несовместима с Вашей верхней одеждой", 3000);
                                            return;
                                        }
                                        Customization.CustomPlayerData[UUID].Clothes.Undershit = new ComponentItem(underwear.UndershirtIDs[topType], texture);

                                        nInventory.UnActiveItem(player, item.Type);
                                        nInventory.Items[UUID][index].IsActive = true;
                                        GUI.Dashboard.Update(player, item, index);
                                    }
                                    else
                                    {
                                        if (underwear.Top == -1)
                                        {
                                            GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, "Эту одежду можно одеть только под низ верхней", 3000);
                                            return;
                                        }
                                        Customization.CustomPlayerData[UUID].Clothes.Top = new ComponentItem(underwear.Top, texture);

                                        nInventory.UnActiveItem(player, item.Type);
                                        nInventory.Items[UUID][index].IsActive = true;
                                        GUI.Dashboard.Update(player, item, index);
                                    }
                                }
                            }

                            var gloves = Customization.CustomPlayerData[UUID].Clothes.Gloves.Variation;
                            if (gloves != 0 &&
                                !Customization.CorrectGloves[gender][gloves].ContainsKey(Customization.CorrectTorso[gender][Customization.CustomPlayerData[UUID].Clothes.Top.Variation]))
                            {
                                nInventory.UnActiveItem(player, ItemType.Gloves);
                                Customization.CustomPlayerData[UUID].Clothes.Gloves = new ComponentItem(0, 0);
                            }

                            player.SetClothes(8, Customization.CustomPlayerData[UUID].Clothes.Undershit.Variation, Customization.CustomPlayerData[UUID].Clothes.Undershit.Texture);
                            player.SetClothes(11, Customization.CustomPlayerData[UUID].Clothes.Top.Variation, Customization.CustomPlayerData[UUID].Clothes.Top.Texture);
                            Customization.ApplyCorrectArmor(player);
                            Customization.ApplyCorrectTorso(player);
                            return;
                        }
                    case ItemType.BodyArmor:
                        {
                            if (item.IsActive)
                            {
                                string iData = item.Data;
                                var split = iData.Split('_');
                                item.Data = $"{player.Armor}_{split[1]}_{split[2]}";
                                player.Armor = 0;
                                player.ResetSharedData("HASARMOR");

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                                player.SetClothes(9, 0, 0);
                            }
                            else
                            {
                                string iData = item.Data;
                                var split = iData.Split('_');
                                var armor = Convert.ToInt32(split[0]);

                                player.Armor = armor;
                                player.SetSharedData("HASARMOR", true);

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;

                                Customization.ApplyCorrectArmor(player);
                                GUI.Dashboard.Update(player, item, index);
                            }
                            return;
                        }
                    case ItemType.Unknown:
                        {
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[UUID].Clothes.Decals = new ComponentItem(Customization.EmtptySlots[gender][10], 0);
                            }
                            else
                            {
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                Customization.CustomPlayerData[UUID].Clothes.Decals = new ComponentItem(variation, texture);
                            }
                            player.SetClothes(10, Customization.CustomPlayerData[UUID].Clothes.Decals.Variation, Customization.CustomPlayerData[UUID].Clothes.Decals.Texture);
                            return;
                        }
                    case ItemType.Top:
                        {
                            if (item.IsActive)
                            {
                                if (Customization.CustomPlayerData[UUID].Clothes.Undershit.Variation == Customization.EmtptySlots[gender][8] || (!gender && Customization.CustomPlayerData[UUID].Clothes.Undershit.Variation == 15))
                                    Customization.CustomPlayerData[UUID].Clothes.Top = new ComponentItem(Customization.EmtptySlots[gender][11], 0);
                                else
                                {
                                    var underwearID = Customization.Undershirts[gender][Customization.CustomPlayerData[UUID].Clothes.Undershit.Variation];
                                    var underwear = Customization.Underwears[gender][underwearID];
                                    if (underwear.Top == -1)
                                    {
                                        Customization.CustomPlayerData[UUID].Clothes.Top = new ComponentItem(Customization.EmtptySlots[gender][11], 0);
                                        var uItem = nInventory.Items[UUID].Find(i => nInventory.ClothesItems.Contains(i.Type) && i.IsActive && ((string)i.Data).Contains($"{underwearID}_{Customization.CustomPlayerData[UUID].Clothes.Undershit.Texture}"));
                                        uItem.IsActive = false;

                                        GUI.Dashboard.Update(player, uItem, nInventory.Items[UUID].IndexOf(uItem));
                                    }
                                    else
                                        Customization.CustomPlayerData[UUID].Clothes.Top = new ComponentItem(underwear.Top, Customization.CustomPlayerData[UUID].Clothes.Undershit.Texture);
                                    Customization.CustomPlayerData[UUID].Clothes.Undershit = new ComponentItem(Customization.EmtptySlots[gender][8], 0);
                                }

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);

                                if (Customization.Tops[gender].FirstOrDefault(t => t.Variation == Customization.CustomPlayerData[UUID].Clothes.Top.Variation) != null || Customization.CustomPlayerData[UUID].Clothes.Top.Variation == Customization.EmtptySlots[gender][11])
                                {
                                    if (Customization.CustomPlayerData[UUID].Clothes.Undershit.Variation == Customization.EmtptySlots[gender][8] || (!gender && Customization.CustomPlayerData[UUID].Clothes.Undershit.Variation == 15))
                                        Customization.CustomPlayerData[UUID].Clothes.Top = new ComponentItem(variation, texture);
                                    else
                                    {
                                        var underwearID = Customization.Undershirts[gender][Customization.CustomPlayerData[UUID].Clothes.Undershit.Variation];
                                        var underwear = Customization.Underwears[gender][underwearID];
                                        var underwearTexture = Customization.CustomPlayerData[UUID].Clothes.Undershit.Texture;
                                        var topType = Customization.Tops[gender].FirstOrDefault(t => t.Variation == variation).Type;
                                        Log.Debug($"UnderwearID: {underwearID} | TopType: {topType}");
                                        if (!underwear.UndershirtIDs.ContainsKey(topType))
                                        {
                                            Customization.CustomPlayerData[UUID].Clothes.Undershit = new ComponentItem(Customization.EmtptySlots[gender][8], 0);
                                            nInventory.UnActiveItem(player, ItemType.Undershit);
                                        }
                                        else
                                            Customization.CustomPlayerData[UUID].Clothes.Undershit = new ComponentItem(underwear.UndershirtIDs[topType], underwearTexture);
                                        Customization.CustomPlayerData[UUID].Clothes.Top = new ComponentItem(variation, texture);
                                    }
                                }
                                else
                                {
                                    var underwearID = 0;
                                    var underwear = Customization.Underwears[gender].Values.FirstOrDefault(u => u.Top == Customization.CustomPlayerData[UUID].Clothes.Top.Variation);
                                    var underwearTexture = Customization.CustomPlayerData[UUID].Clothes.Top.Texture;
                                    if (underwear != null)
                                    {
                                        var topType = Customization.Tops[gender].FirstOrDefault(t => t.Variation == variation).Type;
                                        Log.Debug($"UnderwearID: {underwearID} | TopType: {topType}");
                                        if (!underwear.UndershirtIDs.ContainsKey(topType))
                                        {
                                            Customization.CustomPlayerData[UUID].Clothes.Undershit = new ComponentItem(Customization.EmtptySlots[gender][8], 0);
                                            nInventory.UnActiveItem(player, ItemType.Undershit);
                                        }
                                        else
                                            Customization.CustomPlayerData[UUID].Clothes.Undershit = new ComponentItem(underwear.UndershirtIDs[topType], underwearTexture);
                                    }
                                    Customization.CustomPlayerData[UUID].Clothes.Top = new ComponentItem(variation, texture);
                                }

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }

                            var gloves = Customization.CustomPlayerData[UUID].Clothes.Gloves.Variation;
                            if (gloves != 0 &&
                                !Customization.CorrectGloves[gender][gloves].ContainsKey(Customization.CorrectTorso[gender][Customization.CustomPlayerData[UUID].Clothes.Top.Variation]))
                            {
                                nInventory.UnActiveItem(player, ItemType.Gloves);
                                Customization.CustomPlayerData[UUID].Clothes.Gloves = new ComponentItem(0, 0);
                            }

                            player.SetClothes(8, Customization.CustomPlayerData[UUID].Clothes.Undershit.Variation, Customization.CustomPlayerData[UUID].Clothes.Undershit.Texture);
                            player.SetClothes(11, Customization.CustomPlayerData[UUID].Clothes.Top.Variation, Customization.CustomPlayerData[UUID].Clothes.Top.Texture);
                            Customization.ApplyCorrectArmor(player);
                            Customization.ApplyCorrectTorso(player);
                            return;
                        }
                    #endregion
                    case ItemType.BagWithDrill:
                    case ItemType.BagWithMoney:
                    case ItemType.Pocket:
                    case ItemType.Cuffs:
                    case ItemType.CarKey:
                        return;
                    case ItemType.KeyRing:
                        List<nItem> items = new List<nItem>();
                        string data = item.Data;
                        List<string> keys = (data.Length == 0) ? new List<string>() : new List<string>(data.Split('/'));
                        if (keys.Count > 0 && string.IsNullOrEmpty(keys[keys.Count - 1]))
                            keys.RemoveAt(keys.Count - 1);

                        foreach (var key in keys)
                            items.Add(new nItem(ItemType.CarKey, 1, key));
                        player.SetData("KEYRING", nInventory.Items[UUID].IndexOf(item));
                        GUI.Dashboard.OpenOut(player, items, "Связка ключей", 7);
                        return;
                    case ItemType.Material:
                        Trigger.ClientEvent(player, "board", "close");
                        GUI.Dashboard.isopen[player] = false;
                        GUI.Dashboard.Close(player);
                        Fractions.Manager.OpenGunCraftMenu(player);
                        return;
                    case ItemType.Beer:
                        player.Health = (player.Health + 10 > 100) ? 100 : player.Health + 10;
                        Commands.RPChat("me", player, $"выпил(а) {nInventory.ItemsNames[(int)item.Type]}");
                        break;
                    case ItemType.Burger:
                        //player.Health = (player.Health + 30 > 100) ? 100 : player.Health + 30;
                        HungerPlayer.ChangeHunger(player, -70);
                        if (player.GetData("RESIST_TIME") < 600) Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                        Commands.RPChat("me", player, $"съел(а) {nInventory.ItemsNames[(int)item.Type]}");
                        break;
                    case ItemType.Drugs:
                        if (player != null && (!player.HasData("USING_DRUGS") ||  DateTime.Now > player.GetData("USING_DRUGS")) && !RedisService.IsPlayerUserDrugsRecently(player))
                        {
                            if (player.HasData(GlobalVariables.DrugAddictionKey))
                            {
                                var currentAddiction = Convert.ToDouble(player.GetData(GlobalVariables.DrugAddictionKey));
                                var newAddiction = currentAddiction + 0.25;

                                Main.Players[player].DrugAddiction = newAddiction;

                                player.SetData(GlobalVariables.DrugAddictionKey, newAddiction);

                                if (newAddiction < 15) {
                                    player.SetSharedData("ADD_HP", 0);
                                } else if (newAddiction < 30) {
                                    player.SetSharedData("ADD_HP", 10);
                                } else if (newAddiction < 50) {
                                    player.SetSharedData("ADD_HP", 25);
                                } else if (newAddiction < 75) {
                                    player.SetSharedData("ADD_HP", 40);
                                } else if (newAddiction < 100) {
                                    player.SetSharedData("ADD_HP", 60);
                                } else {
                                    player.SetSharedData("ADD_HP", 70);
                                    newAddiction = 100;
                                }

                                if (newAddiction >= 15) {
                                    GUI.Notify.Send(player, Notify.Type.Info, Notify.Position.BottomCenter, $"Вы стали зависимы от наркотиков. Обратитесь к медикам для лечения", 3000);
                                }

                                if (newAddiction >= 15) {
                                    StartAddictionEffect(player);
                                }
                            } else {
                                player.SetData(GlobalVariables.DrugAddictionKey, 1);
                            }

                            player.Health = (player.Health + 40 > 100) ? 100 : player.Health + 30;
                            player.SetData("USING_DRUGS", DateTime.Now.AddMinutes(7));
                            RedisService.PutPlayerDrugsDataInRedisCashe(player);

                            Commands.RPChat("me", player, $"закурил(а) косяк");
                        }
                        else
                        {
                            isUsingItemSuccess = false;

                            GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, $"Не так быстро. Раз в 15 минут.", 3000);
                            return;
                        }
                        break;
                    case ItemType.eCola:
                        //player.Health = (player.Health + 10 > 100) ? 100 : player.Health + 10;
                        HungerPlayer.ChangeHunger(player, -10);
                        Commands.RPChat("me", player, $"выпил(а) {nInventory.ItemsNames[(int)item.Type]}");
                        break;
                    case ItemType.GasCan:
                        if (!player.IsInVehicle)
                        {
                            GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, $"Вы должны находиться в машине", 3000);
                            GUI.Dashboard.Close(player);
                            return;
                        }
                        var veh = player.Vehicle;
                        if (!veh.HasSharedData("PETROL")) return;
                        var fuel = veh.GetSharedData("PETROL");
                        if (fuel == VehicleManager.VehicleTank[veh.Class])
                        {
                            GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, $"В машине полный бак", 3000);
                            GUI.Dashboard.Close(player);
                            return;
                        }
                        fuel += 30;
                        if (fuel > VehicleManager.VehicleTank[veh.Class]) fuel = VehicleManager.VehicleTank[veh.Class];
                        veh.SetSharedData("PETROL", fuel);
                        if (player.Vehicle.HasData("ACCESS") && player.Vehicle.GetData("ACCESS") == "GARAGE")
                        {
                            var number = player.Vehicle.NumberPlate;
                            VehicleManager.Vehicles[number].Fuel = fuel;
                        }
                        break;
                    case ItemType.HealthKit:
                        if (!player.HasData("USE_MEDKIT") || DateTime.Now > player.GetData("USE_MEDKIT"))
                        {
                            player.Health = 100;
                            player.SetData("USE_MEDKIT", DateTime.Now.AddMinutes(5));
                            Main.OnAntiAnim(player);
                            player.PlayAnimation("amb@code_human_wander_texting_fat@female@enter", "enter", 49);
                            NAPI.Task.Run(() => {
                                try
                                {
                                    if (player == null) return;
                                    if (!player.IsInVehicle) player.StopAnimation();
                                    else player.SetData("ToResetAnimPhone", true);
                                    Main.OffAntiAnim(player);
                                    Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                                } catch { }
                            }, 5000);
                            Commands.RPChat("me", player, $"использовал(а) аптечку");
                        }
                        else
                        {
                            GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, $"Попробуйте использовать позже", 3000);
                            return;
                        }
                        break;
                    case ItemType.Toksikanol: // todo EUGENE Закончить применение токсиканола

                        var currentPlayerAddiction = Convert.ToInt32(player.GetData(GlobalVariables.DrugAddictionKey));

                        if (currentPlayerAddiction <= 0)
                        {
                            isUsingItemSuccess = false;
                            GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, $"У вас нет зависимости", 3000);
                            return;
                        }

                        if (!player.HasData("USE_TOKSIKANOL") || DateTime.Now > player.GetData("USE_TOKSIKANOL"))
                        {
                            player.SetData($"USE_TOKSIKANOL", DateTime.Now.AddMinutes(20));
                            Main.OnAntiAnim(player);
                            player.PlayAnimation("amb@code_human_wander_texting_fat@female@enter", "enter", 49);

                            NAPI.Task.Run(() => {
                                try
                                {
                                    if (!player.IsInVehicle) player.StopAnimation();

                                    else player.SetData("ToResetAnimPhone", true);
                                    Main.OffAntiAnim(player);

                                    if (currentPlayerAddiction - 40 <= 0)
                                    {
                                        player.SetData(GlobalVariables.DrugAddictionKey, 0);
                                        Main.Players[player].DrugAddiction = 0;
                                    }
                                    else
                                    {
                                        player.SetData(GlobalVariables.DrugAddictionKey, currentPlayerAddiction - 40);
                                        Main.Players[player].DrugAddiction = currentPlayerAddiction - 40;
                                    }

                                    Main.Players[player].ApplyAddiction(player);
                                }
                                catch (Exception exception)
                                {
                                    Log.Write($"Error in using player Toksikanol. Ex: {exception.ToString()}", nLog.Type.Error);
                                }
                            }, 5000);

                            Commands.RPChat("me", player, $"использовал(а) токсиканол");
                        }
                        else
                        {
                            GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, $"Применение раз в 20 минут.", 3000);
                            return;
                        }

                        break;
                    case ItemType.HotDog:
                        //player.Health = (player.Health + 30 > 100) ? 100 : player.Health + 30;
                        HungerPlayer.ChangeHunger(player, -45);
                        if (player.GetData("RESIST_TIME") < 600) Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                        Commands.RPChat("me", player, $"съел(а) {nInventory.ItemsNames[(int)item.Type]}");
                        break;
                    case ItemType.Lockpick:
                        if (player.GetData("INTERACTIONCHECK") != 3)
                        {
                            GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, $"Невозможно использовать в данный момент", 3000);
                            GUI.Dashboard.Close(player);
                            return;
                        }
                        //player.SetData("LOCK_TIMER", Main.StartT(10000, 999999, (o) => SafeMain.lockCrack(player, player.Name), "LOCK_TIMER"));
                        player.SetData("LOCK_TIMER", Timers.StartOnce(10000, () => SafeMain.lockCrack(player, player.Name)));
                        player.FreezePosition = true;
                        Trigger.ClientEvent(player, "showLoader", "Идёт взлом", 1);
                        GUI.Notify.Send(player, GUI.Notify.Type.Info, GUI.Notify.Position.BottomCenter, $"Вы начали взламывать дверь", 3000);
                        break;
                    case ItemType.ArmyLockpick:
                        if (!player.IsInVehicle || player.Vehicle.DisplayName != "Barracks")
                        {
                            GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, $"Вы должны находиться в военном перевозчике материалов", 3000);
                            return;
                        }
                        if (VehicleStreaming.GetEngineState(player.Vehicle))
                        {
                            GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, $"Машину уже заведена", 3000);
                            return;
                        }
                        var lucky = new Random().Next(0, 5);
                        Log.Debug(lucky.ToString());
                        if (lucky == 5)
                            GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, $"У Вас не получилось завести машину. Попробуйте ещё раз", 3000);
                        else
                        {
                            VehicleStreaming.SetEngineState(player.Vehicle, true);
                            GUI.Notify.Send(player, GUI.Notify.Type.Success, GUI.Notify.Position.BottomCenter, $"У Вас получилось завести машину", 3000);
                        }
                        break;
                    case ItemType.Pizza:
                        //player.Health = (player.Health + 30 > 100) ? 100 : player.Health + 30;
                        HungerPlayer.ChangeHunger(player, -75);
                        if (player.GetData("RESIST_TIME") < 600) Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                        Commands.RPChat("me", player, $"съел(а) {nInventory.ItemsNames[(int)item.Type]}");
                        break;
                    case ItemType.Sandwich:
                        //player.Health = (player.Health + 30 > 100) ? 100 : player.Health + 30;
                        HungerPlayer.ChangeHunger(player, -40);
                        if (player.GetData("RESIST_TIME") < 600) Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                        Commands.RPChat("me", player, $"съел(а) {nInventory.ItemsNames[(int)item.Type]}");
                        break;
                    case ItemType.Sprunk:
                        //player.Health = (player.Health + 10 > 100) ? 100 : player.Health + 10;
                        HungerPlayer.ChangeHunger(player, -20);
                        if (player.GetData("RESIST_TIME") < 600) Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                        Commands.RPChat("me", player, $"выпил(а) {nInventory.ItemsNames[(int)item.Type]}");
                        break;
                    case ItemType.Сrisps:
                        //player.Health = (player.Health + 30 > 100) ? 100 : player.Health + 30;
                        HungerPlayer.ChangeHunger(player, -50);
                        if (player.GetData("RESIST_TIME") < 600) Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                        Commands.RPChat("me", player, $"съел(а) {nInventory.ItemsNames[(int)item.Type]}");
                        break;
                    case ItemType.Cigarette:
                        Main.OnAntiAnim(player);
                        player.PlayAnimation("amb@world_human_smoking@male@male_a@base", "base", 49);
                        BasicSync.AddAttachmnet(player, NAPI.Util.GetHashKey("ng_proc_cigarette01a"), false);

                        NAPI.Task.Run(() => {
                            try
                            {
                                if (player == null) return;
                                if (!player.IsInVehicle) player.StopAnimation();
                                else player.SetData("ToResetAnimPhone", true);
                                BasicSync.AddAttachmnet(player, NAPI.Util.GetHashKey("ng_proc_cigarette01a"), true);
                                Main.OffAntiAnim(player);
                            }
                            catch { }
                        }, 60000);
                        Commands.RPChat("me", player, $"закурил(а) сигарету");
                        break;
                    case ItemType.DrugSeed:
                        if(Fractions.FarmDrugs.PlantTree(player))
                            break;
                        else return;
                    case ItemType.Bandage:
                        if (player.HasData("HEAL_BANDAGE_TIMER")) {
                            isUsingItemSuccess = false;
                            GUI.Notify.Send(player, Notify.Type.Warning, Notify.Position.BottomCenter, $"Вы уже лечитесь с помощью бинтов", 3000);
                            break;
                        }
                        if (NAPI.Player.GetPlayerHealth(player) > 99) {
                            isUsingItemSuccess = false;
                            GUI.Notify.Send(player, Notify.Type.Warning, Notify.Position.BottomCenter, $"Вам не нужно лечение", 3000);
                            break;
                        }

                        Commands.RPChat("me", player, $"использовал бинт");
                        GUI.Notify.Send(player, Notify.Type.Warning, Notify.Position.BottomCenter, $"Вы начали лечение бинтами", 3000);
                        player.SetData("HEAL_BANDAGE_TIMER", Timers.Start(15000, () => HealPlayerBandage(player)));

                        break;
                    case ItemType.MiniMedkit:
                        if (player.HasData("USE_MINI_MEDKIT") && DateTime.Now < player.GetData("USE_MINI_MEDKIT")) {
                            GUI.Notify.Send(player, GUI.Notify.Type.Error, GUI.Notify.Position.BottomCenter, $"Не так часто. Раз в 2 минуты", 3000);
                            return;
                        }
                        if (player.Health >= 100) {
                            isUsingItemSuccess = false;
                            GUI.Notify.Send(player, Notify.Type.Info, Notify.Position.BottomCenter, $"У вас полное здоровье", 3000);
                        } else {
                            Commands.RPChat("me", player, $"использовал маленькую аптечку");
                            HealPlayerMiniMedkit(player);
                        }
                        break;
                    case ItemType.Antinark:
                        if (!player.HasData(GlobalVariables.DrugAddictionKey))
                        {
                            GUI.Notify.Send(player, Notify.Type.Info, Notify.Position.BottomCenter, $"У вас нет зависимости от наркотиков", 3000);
                            Log.Write($"Нет зависимости", nLog.Type.Info);
                            isUsingItemSuccess = false;
                            return;
                        }

                        if (player.HasData(UseAntiNarkKey) && DateTime.Now < player.GetData(UseAntiNarkKey))
                        {
                            GUI.Notify.Send(player, Notify.Type.Info, Notify.Position.BottomCenter, $"Нельзя слишком часто использовать антинарк, раз в 30 мин", 3000);
                            Log.Write($"Раз в 30 мин!", nLog.Type.Error);
                            isUsingItemSuccess = false;
                            return;
                        }

                        int currentPlayerAddict = Convert.ToInt32(player.GetData(GlobalVariables.DrugAddictionKey));
                        int newAddictionAfterUsingAntiNark = currentPlayerAddict - AntNarkEfficiency;

                        if (newAddictionAfterUsingAntiNark <= 0)
                        {
                            player.SetData(GlobalVariables.DrugAddictionKey, 0);
                            Log.Write($"Использовали антинарк, теперь зависимости нет", nLog.Type.Info);
                        }
                        else
                        {
                            player.SetData(GlobalVariables.DrugAddictionKey, newAddictionAfterUsingAntiNark);
                            player.SetData(UseAntiNarkKey, DateTime.Now.AddMinutes(30));

                            Log.Write($"Использовали антинарк, теперь зависимость {newAddictionAfterUsingAntiNark}");
                        }

                        break;
                    case ItemType.Present:
                        /*
                        player.Health = (player.Health + 10 > 100) ? 100 : player.Health + 10;
                        GUI.Notify.Send(player, GUI.Notify.Type.Success, GUI.Notify.Position.BottomCenter, $"Вы открыли подарок, в нём были:", 3000);

                        Tuple<int,int> types = PresentsTypes[Convert.ToInt32(item.Data)];
                        if (types.Item1 <= 2)
                        {
                            Main.Players[player].EXP += TypesCounts[types.Item1];
                            if (Main.Players[player].EXP >= 3 + Main.Players[player].LVL * 3)
                            {
                                Main.Players[player].EXP = Main.Players[player].EXP - (3 + Main.Players[player].LVL * 3);
                                Main.Players[player].LVL += 1;
                            }

                            GUI.Notify.Send(player, GUI.Notify.Type.Info, GUI.Notify.Position.BottomCenter, $"{TypesCounts[types.Item1]} EXP", 3000);

                            MoneySystem.Wallet.Change(player, TypesCounts[types.Item2]);

                            GUI.Notify.Send(player, GUI.Notify.Type.Info, GUI.Notify.Position.BottomCenter, $"$ {TypesCounts[types.Item2]}", 3000);
                        }
                        else
                        {
                            MoneySystem.Wallet.Change(player, TypesCounts[types.Item1]);

                            GUI.Notify.Send(player, GUI.Notify.Type.Info, GUI.Notify.Position.BottomCenter, $"$ {TypesCounts[types.Item1]}", 3000);

                            Main.Players[player].EXP += TypesCounts[types.Item2];
                            if (Main.Players[player].EXP >= 3 + Main.Players[player].LVL * 3)
                            {
                                Main.Players[player].EXP = Main.Players[player].EXP - (3 + Main.Players[player].LVL * 3);
                                Main.Players[player].LVL += 1;
                            }

                            GUI.Notify.Send(player, GUI.Notify.Type.Info, GUI.Notify.Position.BottomCenter, $"{TypesCounts[types.Item2]} EXP", 3000);
                        }

                        Commands.RPChat("me", player, $"открыл(а) подарок");
                        */
                        /*break;
                }

                if (isUsingItemSuccess)
                {
                    nInventory.Remove(player, item.Type, 1);
                    GUI.Notify.Send(player, GUI.Notify.Type.Info, GUI.Notify.Position.BottomCenter, $"Вы использовали {nInventory.ItemsNames[item.ID]}", 3000);
                    GameLog.Items($"player({UUID},{player.Name})", "use", Convert.ToInt32(item.Type), 1, $"{item.Data}");
                }

                GUI.Dashboard.Close(player);
            }
            catch (Exception e)
            {
                Log.Write($"EXCEPTION AT\"ITEM_USE\"/{item.Type}/{index}/{player.Name}/:\n" + e.ToString(), nLog.Type.Error);
            }
        }*/

        /*public static string GetTimerKeyForAddiction(Player player)
        {
            return $"{Main.PlayerUUIDs[player.Name]}:AddictionEffect";
        }*/

        public static void StartAddictionEffect(Player player)
        {
            NAPI.Task.Run(() =>
            {
                Trigger.ClientEvent(player, "startScreenEffect", "DrugsTrevorClownsFight", 300000, false);
            });
        }

        private static void HealPlayerBandage(Player player)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    if (player.Health == 100)
                    {
                        Timers.Stop(player.GetData<string>("HEAL_BANDAGE_TIMER"));
                        player.ResetData("HEAL_BANDAGE_TIMER");
                        
                        GUI.Notify.Send(player, $"Вы закончили лечение бинтами", Notify.Type.Success, Notify.Position.BottomCenter, 3000);
                        return;
                    }
                    player.Health = player.Health + 5 >= 100 ? 100 : player.Health + 5;
                }
                catch (Exception ex) 
                {
                    Log.Write($"Error in healing player with bandage. Ex: {ex.ToString()}", nLog.Type.Error);
                }
            });
        }

        private static void HealPlayerMiniMedkit(Player player)
        {
            NAPI.Task.Run(() =>
            {
                if (player.Health + 50 >= 100)
                {
                    NAPI.Player.SetPlayerHealth(player, 100);
                    player.SetData("USE_MINI_MEDKIT", DateTime.Now.AddMinutes(2));
                }
                else
                {
                    NAPI.Player.SetPlayerHealth(player, player.Health + 50);
                }
            });
        }

        // TO DELETE
        private static List<int> TypesCounts = new List<int>()
        {
            5, 10, 15, 3000, 5000, 10000
        };
        private static List<Tuple<int, int>> PresentsTypes = new List<Tuple<int, int>>()
        {
            new Tuple<int, int>(0, 5),
            new Tuple<int, int>(1, 4),
            new Tuple<int, int>(2, 3),
            new Tuple<int, int>(5, 0),
            new Tuple<int, int>(4, 1),
            new Tuple<int, int>(3, 2),
        };
        //
        public static void onDrop(Player player, nItem item, dynamic data)
        {
            try
            {
                /*if (item.Type == ItemType.KeyRing)
                {
                    Trigger.ClientEvent(player, "board", "close");
                    GUI.Dashboard.isopen[player] = false;
                    GUI.Dashboard.Close(player);
                }*/

                if (item.Type == ItemType.GasCan)
                {
                    GUI.Notify.Send(player, $"Невозможно выбросить канистру", Notify.Type.Error, Notify.Position.BottomCenter, 3000);
                    return;
                }

                if (item.Type == ItemType.HealthKit)
                {
                    GUI.Notify.Send(player, $"Невозможно выбросить аптечку", Notify.Type.Error, Notify.Position.BottomCenter, 3000);
                    return;
                }

                if (item.Type == ItemType.Toksikanol)
                {
                    GUI.Notify.Send(player, $"Невозможно выбросить токсиканол", Notify.Type.Error, Notify.Position.BottomCenter, 3000);
                    return;
                }

                var rnd = new Random();
                /*if (data != null && (int)data != 1)
                    Commands.RPChat("me", player, $"выбросил(а) {nInventory.ItemsNames[(int)item.Type]}");*/

                GameLog.Items($"player({GameLog.PlayerFormat(player)})", "ground", Convert.ToInt32(item.Type), 1, $"{item.Data}");

                if (!nInventory.ClothesItems.Contains(item.Type) && !nInventory.WeaponsItems.Contains(item.Type) && item.Type != ItemType.CarKey && item.Type != ItemType.KeyRing)
                {
                    foreach (var o in NAPI.Pools.GetAllObjects())
                    {
                        if (player.Position.DistanceTo(o.Position) > 2) continue;
                        if (!o.HasSharedData("TYPE") || o.GetSharedData<string>("TYPE") != "DROPPED" || !o.HasData("ITEM")) continue;
                        nItem oItem = o.GetData<nItem>("ITEM");
                        if (oItem.Type == item.Type)
                        {
                            oItem.Count += item.Count;
                            o.SetData("ITEM", oItem);
                            o.SetData("WILL_DELETE", DateTime.Now.AddMinutes(2));
                            return;
                        }
                    }
                }
                item.IsActive = false;

                var xrnd = rnd.NextDouble();
                var yrnd = rnd.NextDouble();
                var obj = NAPI.Object.CreateObject(nInventory.ItemModels[item.Type], player.Position + nInventory.ItemsPosOffset[item.Type] + new Vector3(xrnd, yrnd, 0), player.Rotation + nInventory.ItemsRotOffset[item.Type], 255, player.Dimension);

                var id = rnd.Next(100000, 999999);
                while (ItemsDropped.Contains(id)) id = rnd.Next(100000, 999999);
                obj.SetData("ID", id);

                obj.SetSharedData("TYPE", "DROPPED");
                obj.SetData("ITEM", item);
                //obj.SetData("DELETETIMER", Main.StartT(14400000, 99999999, (o) => deleteObject(obj), "ODELETE_TIMER"));
                obj.SetData("DELETETIMER", Timers.StartOnce(14400000, () => deleteObject(obj)));
            }
            catch (Exception e) { Log.Write("onDrop: " + e.Message, nLog.Type.Error); }
        }
        public static void onTransfer(Player player, nItem item, dynamic data)
        {
            //
        }
        public static void Update(Player client, nItem item, int index)
        {
            List<object> idata = new List<object>
                    {
                        item.ID,
                        item.Count,
                        (item.IsActive) ? 1 : 0,
                        (nInventory.WeaponsItems.Contains(item.Type) || item.Type == ItemType.StunGun) ? "Serial: " + item.Data : (item.Type == ItemType.CarKey) ? $"{(string)item.Data.Split('_')[0]}" : ""
                    };
            string json = JsonConvert.SerializeObject(idata);
            Trigger.ClientEvent(client, "board", 6, json, index);
        }
    }

    class nItemSize
    {
        public int x;
        public int y;

        public nItemSize(int szx, int szy)
        {
            x = szx;
            y = szy;
        }
    }

    class nItem
    {
        public int ID { get; internal set; }
        public nItemSize Size { get; internal set; }
        public ItemType Type { get; internal set; }
        public int Cell { get; set; }
        public int Count { get; set; }
        public bool IsActive { get; set; }
        public dynamic Data;

        public nItem(int id, int cell, int count = 1, dynamic data = null, bool isActive = false)
        {
            ID = id;
            Size = nInventory.ItemsSize[nInventory.GetType(id)];
            Cell = cell;
            Count = count;
            Data = data;
            IsActive = isActive;
        }
    }
}
