using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SecondLive.Core.Logging;
using MySql.Data.MySqlClient;

namespace SecondLive.Core
{
    #region Tattoo
    public enum TattooZones
    {
        Torso = 0,
        Head = 1,
        LeftArm = 2,
        RightArm = 3,
        LeftLeg = 4,
        RightLeg = 5,
    }

    public class Tattoo
    {
        public string Dictionary { get; set; }
        public string Hash { get; set; }
        public List<int> Slots { get; set; }

        public Tattoo(string dictionary, string hash, List<int> slots)
        {
            Dictionary = dictionary;
            Hash = hash;
            Slots = slots;
        }
    }
    #endregion

    #region ComponentItem

    public class ComponentItem
    {
        public int Variation;
        public int Texture;

        public ComponentItem(int variation, int texture)
        {
            Variation = variation;
            Texture = texture;
        }
    }

    #endregion

    #region ClothesData
    public class ClothesData
    {
        public ComponentItem Mask { get; set; }
        public ComponentItem Gloves { get; set; }
        public ComponentItem Torso { get; set; }
        public ComponentItem Leg { get; set; }
        public ComponentItem Bag { get; set; }
        public ComponentItem Feet { get; set; }
        public ComponentItem Accessory { get; set; }
        public ComponentItem Undershit { get; set; }
        public ComponentItem Bodyarmor { get; set; }
        public ComponentItem Decals { get; set; }
        public ComponentItem Top { get; set; }

        public ClothesData()
        {
            Mask = new ComponentItem(0, 0);
            Gloves = new ComponentItem(0, 0);
            Torso = new ComponentItem(15, 0);
            Leg = new ComponentItem(21, 0);
            Bag = new ComponentItem(0, 0);
            Feet = new ComponentItem(34, 0);
            Accessory = new ComponentItem(0, 0);
            Undershit = new ComponentItem(15, 0);
            Bodyarmor = new ComponentItem(0, 0);
            Decals = new ComponentItem(0, 0);
            Top = new ComponentItem(15, 0);
        }
    }
    #endregion

    #region AccessoryData
    public class AccessoryData
    {
        public ComponentItem Hat { get; set; }
        public ComponentItem Glasses { get; set; }
        public ComponentItem Ear { get; set; }
        public ComponentItem Watches { get; set; }
        public ComponentItem Bracelets { get; set; }

        public AccessoryData()
        {
            Hat = new ComponentItem(-1, 0);
            Glasses = new ComponentItem(-1, 0);
            Ear = new ComponentItem(-1, 0);
            Watches = new ComponentItem(-1, 0);
            Bracelets = new ComponentItem(-1, 0);
        }
    }
    #endregion

    #region ParentData
    public class ParentData
    {
        public int Father;
        public int Mother;
        public float Similarity;
        public float SkinSimilarity;

        public ParentData(int father, int mother, float similarity, float skinsimilarity)
        {
            Father = father;
            Mother = mother;
            Similarity = similarity;
            SkinSimilarity = skinsimilarity;
        }
    }
    #endregion

    #region AppearanceItem
    public class AppearanceItem
    {
        public int Value;
        public float Opacity;

        public AppearanceItem(int value, float opacity)
        {
            Value = value;
            Opacity = opacity;
        }
    }
    #endregion

    #region HairData
    public class HairData
    {
        public int Hair;
        public int Color;
        public int HighlightColor;

        public HairData(int hair, int color, int highlightcolor)
        {
            Hair = hair;
            Color = color;
            HighlightColor = highlightcolor;
        }
    }
    #endregion

    #region PlayerCustomization Class
    public class PlayerCustomization
    {
        // Parents
        public ParentData Parents;

        // Features
        public float[] Features = new float[20];

        // Appearance
        public AppearanceItem[] Appearance = new AppearanceItem[10];

        // Hair & Colors
        public HairData Hair;

        public ClothesData Clothes = new ClothesData();

        public AccessoryData Accessory = new AccessoryData();

        public Dictionary<int, List<Tattoo>> Tattoos = new Dictionary<int, List<Tattoo>>()
        {
            { 0, new List<Tattoo>() },
            { 1, new List<Tattoo>() },
            { 2, new List<Tattoo>() },
            { 3, new List<Tattoo>() },
            { 4, new List<Tattoo>() },
            { 5, new List<Tattoo>() },
        };


        public int EyebrowColor;
        public int BeardColor;
        public int EyeColor;
        public int BlushColor;
        public int LipstickColor;
        public int ChestHairColor;

        public bool IsCreated = false;

        public PlayerCustomization()
        {
            Parents = new ParentData(0, 0, 1.0f, 1.0f);
            for (int i = 0; i < Features.Length; i++) Features[i] = 0f;
            for (int i = 0; i < Appearance.Length; i++) Appearance[i] = new AppearanceItem(255, 1.0f);
            Hair = new HairData(0, 0, 0);
        }
    }
    #endregion

    #region Underwear Class
    class Underwear
    {
        public Underwear(int top, int price, List<int> colors, int bodyArmor = -1)
        {
            Top = top;
            Price = price;
            Colors = colors;
            BodyArmor = bodyArmor;
        }
        public Underwear(int top, int price, Dictionary<int, int> undershirtIDs, List<int> colors, int bodyArmor = -1)
        {
            Top = top;
            UndershirtIDs = undershirtIDs;
            Price = price;
            Colors = colors;
            BodyArmor = bodyArmor;
        }

        public int Top { get; }
        public int Price { get; }
        public Dictionary<int, int> UndershirtIDs { get; } = new Dictionary<int, int>(); // key - тип undershirt'а, value - id-шник
        public List<int> Colors { get; }
        public int BodyArmor { get; }
}
    #endregion

    #region Clothes Class
    class Clothes
    {
        public Clothes(int variation, List<int> colors, int price, int type = -1, int bodyArmor = -1)
        {
            Variation = variation;
            Colors = colors;
            Price = price;
            Type = type;
            BodyArmor = bodyArmor;
        }

        public int Variation { get; }
        public List<int> Colors { get; }
        public int Price { get; }
        public int Type { get; }
        public int BodyArmor { get; }
    }
    #endregion

    class Customization : Script
    {
        public Customization()
        {
            foreach (var gender in Underwears) {
                foreach (var underwear in gender.Value) {
                    foreach (var undershirt in underwear.Value.UndershirtIDs) {
                        if (!Undershirts[gender.Key].ContainsKey(undershirt.Value)) {
                            Undershirts[gender.Key].Add(undershirt.Value, underwear.Key);
                        }
                    }
                }
            }

            /*MySqlCommand queryCommand = new MySqlCommand(@"
                SELECT * FROM `customization`
            ");

            DataTable result = MySQL.QueryResult(queryCommand);

            if (result == null)
            {
                Log.Write("DB return null result.", nLog.Type.Warn);
                return;
            }

            foreach (DataRow Row in result.Rows)
            {
                var uuid = Convert.ToInt32(Row["uuid"]);
                CustomPlayerData.Add(uuid, new PlayerCustomization());

                CustomPlayerData[uuid].Parents = JsonConvert.DeserializeObject<ParentData>(Row["parents"].ToString());
                CustomPlayerData[uuid].Features = JsonConvert.DeserializeObject<float[]>(Row["features"].ToString());
                CustomPlayerData[uuid].Appearance = JsonConvert.DeserializeObject<AppearanceItem[]>(Row["appearance"].ToString());
                CustomPlayerData[uuid].Hair = JsonConvert.DeserializeObject<HairData>(Row["hair"].ToString());
                CustomPlayerData[uuid].Clothes = JsonConvert.DeserializeObject<ClothesData>(Row["clothes"].ToString());
                CustomPlayerData[uuid].Accessory = JsonConvert.DeserializeObject<AccessoryData>(Row["accessory"].ToString());
                CustomPlayerData[uuid].Tattoos = JsonConvert.DeserializeObject<Dictionary<int, List<Tattoo>>>(Row["tattoos"].ToString());
                CustomPlayerData[uuid].EyebrowColor = Convert.ToInt32(Row["eyebrowc"]);
                CustomPlayerData[uuid].BeardColor = Convert.ToInt32(Row["beardc"]);
                CustomPlayerData[uuid].EyeColor = Convert.ToInt32(Row["eyec"]);
                CustomPlayerData[uuid].BlushColor = Convert.ToInt32(Row["blushc"]);
                CustomPlayerData[uuid].LipstickColor = Convert.ToInt32(Row["lipstickc"]);
                CustomPlayerData[uuid].ChestHairColor = Convert.ToInt32(Row["chesthairc"]);
                CustomPlayerData[uuid].IsCreated = Convert.ToBoolean(Row["iscreated"]);

                CustomPlayerData[uuid].Clothes.Bag.Variation = 0;
                //Main.Max_UUID = Convert.ToInt32(Row["max_uuid"]) + 1;

                //CustomPlayerData.Add(Row["name"].ToString(), JsonConvert.DeserializeObject<PlayerCustomization>(Row["data"].ToString()));
            }

            Log.Write($"ALL Customization {result.Rows.Count}");*/
        }

        private static nLog Log = new nLog("Character");

        public static Dictionary<int, PlayerCustomization> CustomPlayerData = new Dictionary<int, PlayerCustomization>();

        public static Dictionary<bool, Dictionary<int, int>> CorrectUndershitTorso = new Dictionary<bool, Dictionary<int, int>>()
        {
            {
                true, new Dictionary<int, int>()
                {
                    { 4, 14 },
                    { 6, 11 },
                    { 21, 4 },
                    { 22, 4 },
                    { 26, 14 },
                    { 50, 11 },
                    { 93, 11 },
                    { 94, 4 },

                }
            },
            {
                false, new Dictionary<int, int>()
                {
                    
                }
            },
        };
        public static Dictionary<bool, Dictionary<int, int>> CorrectTorso = new Dictionary<bool, Dictionary<int, int>>()
        {
            {
                true, new Dictionary<int, int>()
                {
                        { 0, 0 },
                        { 1, 0 },
                        { 2, 2 },
                        { 3, 14 },
                        { 4, 14 },
                        { 5, 5 },
                        { 6, 14 },
                        { 7, 14 },
                        { 8, 8 },
                        { 9, 0 },
                        { 10, 14 },
                        { 11, 15 },
                        { 12, 12 },
                        { 13, 11 },
                        { 14, 12 },
                        { 15, 15 },
                        { 16, 0 },
                        { 17, 5 },
                        { 18, 0 },
                        { 19, 14 },
                        { 20, 14 },
                        { 21, 15 },
                        { 22, 0 },
                        { 23, 14 },
                        { 24, 14 },
                        { 25, 15 },
                        { 26, 11 },
                        { 27, 14 },
                        { 28, 14 },
                        { 29, 14 },
                        { 30, 14 },
                        { 31, 14 },
                        { 32, 14 },
                        { 33, 0 },
                        { 34, 0 },
                        { 35, 14 },
                        { 36, 5 },
                        { 37, 14 },
                        { 38, 8 },
                        { 39, 0 },
                        { 40, 15 },
                        { 41, 12 },
                        { 42, 11 },
                        { 43, 11 },
                        { 44, 0 },
                        { 45, 15 },
                        { 46, 14 },
                        { 47, 0 },
                        { 48, 1 },
                        { 49, 1 },
                        { 50, 1 },
                        { 51, 1 },
                        { 52, 2 },
                        { 53, 0 },
                        { 54, 1 },
                        { 55, 0 },
                        { 56, 0 },
                        { 57, 0 },
                        { 58, 14 },
                        { 59, 14 },
                        { 60, 15 },
                        { 61, 0 },
                        { 62, 14 },
                        { 63, 5 },
                        { 64, 14 },
                        { 65, 14 },
                        { 66, 15 },
                        { 67, 1 },
                        { 68, 14 },
                        { 69, 14 },
                        { 70, 14 },
                        { 71, 0 },
                        { 72, 14 },
                        { 73, 0 },
                        { 74, 14 },
                        { 75, 11 },
                        { 76, 14 },
                        { 77, 14 },
                        { 78, 14 },
                        { 79, 14 },
                        { 80, 0 },
                        { 81, 0 },
                        { 82, 0 },
                        { 83, 0 },
                        { 84, 1 },
                        { 85, 1 },
                        { 86, 1 },
                        { 87, 1 },
                        { 88, 14 },
                        { 89, 14 },
                        { 90, 14 },
                        { 91, 15 },
                        { 92, 6 },
                        { 93, 0 },
                        { 94, 0 },
                        { 95, 11 },
                        { 96, 11 },
                        { 97, 0 },
                        { 98, 0 },
                        { 99, 14 },
                        { 100, 14 },
                        { 101, 14 },
                        { 102, 14 },
                        { 103, 14 },
                        { 104, 14 },
                        { 105, 11 },
                        { 106, 14 },
                        { 107, 14 },
                        { 108, 14 },
                        { 109, 5 },
                        { 110, 1 },
                        { 111, 4 },
                        { 112, 14 },
                        { 113, 6 },
                        { 114, 14 },
                        { 115, 14 },
                        { 116, 14 },
                        { 117, 6 },
                        { 118, 14 },
                        { 119, 14 },
                        { 120, 5 },
                        { 121, 14 },
                        { 122, 14 },
                        { 123, 11 },
                        { 124, 14 },
                        { 125, 14 },
                        { 126, 1 },
                        { 127, 14 },
                        { 128, 0 },
                        { 129, 0 },
                        { 130, 14 },
                        { 131, 0 },
                        { 132, 0 },
                        { 133, 11 },
                        { 134, 0 },
                        { 135, 0 },
                        { 136, 14 },
                        { 137, 6 },
                        { 138, 14 },
                        { 139, 12 },
                        { 140, 14 },
                        { 141, 6 },
                        { 142, 14 },
                        { 143, 14 },
                        { 144, 6 },
                        { 145, 14 },
                        { 146, 0 },
                        { 147, 4 },
                        { 148, 4 },
                        { 149, 14 },
                        { 150, 14 },
                        { 151, 14 },
                        { 152, 14 },
                        { 153, 14 },
                        { 154, 14 },
                        { 155, 14 },
                        { 156, 14 },
                        { 157, 15 },
                        { 158, 15 },
                        { 159, 15 },
                        { 160, 15 },
                        { 161, 14 },
                        { 162, 15 },
                        { 163, 14 },
                        { 164, 0 },
                        { 165, 0 },
                        { 166, 14 },
                        { 167, 14 },
                        { 168, 14 },
                        { 169, 14 },
                        { 170, 15 },
                        { 171, 1 },
                        { 172, 14 },
                        { 173, 15 },
                        { 174, 14 },
                        { 175, 15 },
                        { 176, 15 },
                        { 177, 15 },
                        { 178, 1 },
                        { 179, 15 },
                        { 180, 15 },
                        { 181, 15 },
                        { 182, 1 },
                        { 183, 14 },
                        { 184, 14 },
                        { 185, 14 },
                        { 186, 14 },
                        { 187, 14 },
                        { 188, 14 },
                        { 189, 14 },
                        { 190, 14 },
                        { 191, 14 },
                        { 192, 14 },
                        { 193, 0 },
                        { 194, 1 },
                        { 195, 1 },
                        { 196, 1 },
                        { 197, 1 },
                        { 198, 1 },
                        { 199, 1 },
                        { 200, 1 },
                        { 201, 3 },
                        { 202, 4 },
                        { 203, 1 },
                        { 204, 6 },
                        { 205, 5 },
                        { 206, 5 },
                        { 207, 5 },
                        { 208, 0 },
                        { 209, 0 },
                        { 210, 0 },
                        { 211, 0 },
                        { 212, 14 },
                        { 213, 15 },
                        { 214, 14 },
                        { 215, 14 },
                        { 216, 15 },
                        { 217, 14 },
                        { 218, 14 },
                        { 219, 15 },
                        { 220, 14 },
                        { 221, 14 },
                        { 222, 11 },
                        { 223, 5 },
                        { 224, 1 },
                        { 225, 8 },
                        { 226, 0 },
                        { 227, 4 },
                        { 228, 4 },
                        { 229, 14 },
                        { 230, 14 },
                        { 231, 4 },
                        { 232, 14 },
                        { 233, 14 },
                        { 234, 11 },
                        { 235, 0 },
                        { 236, 0 },
                        { 237, 5 },
                        { 238, 2 },
                        { 239, 2 },
                        { 240, 4 },
                        { 251, 0 },
                        { 249, 6 },
                        { 254, 4 },
                        { 255, 14 },
                }
            },
            {
                false, new Dictionary<int, int>()
                {
                    { 0, 0  },
                    { 1, 5  },
                    { 2, 2  },
                    { 3, 3  },
                    { 4, 4 },
                    { 5, 4 },
                    { 6, 5 },
                    { 7, 5 },
                    { 8, 5 },
                    { 9, 0 },
                    { 10, 5 },
                    { 11, 4 },
                    { 12, 12 },
                    { 13, 15 },
                    { 14, 14 },
                    { 15, 15 },
                    { 16, 15 },
                    { 17, 0 },
                    { 18, 15 },
                    { 19, 15 },
                    { 20, 5 },
                    { 21, 4 },
                    { 22, 4 },
                    { 23, 4 },
                    { 24, 5 },
                    { 25, 5 },
                    { 26, 12 },
                    { 27, 0 },
                    { 28, 15 },
                    { 29, 9 },
                    { 30, 2 },
                    { 31, 5 },
                    { 32, 4 },
                    { 33, 4 },
                    { 34, 6 },
                    { 35, 5 },
                    { 36, 4 },
                    { 37, 4 },
                    { 38, 2 },
                    { 39, 1 },
                    { 40, 2 },
                    { 41, 5 },
                    { 42, 5 },
                    { 43, 3 },
                    { 44, 3 },
                    { 45, 3 },
                    { 46, 3 },
                    { 47, 3 },
                    { 48, 14 },
                    { 49, 14 },
                    { 50, 14 },
                    { 51, 6 },
                    { 52, 6 },
                    { 53, 5 },
                    { 54, 5 },
                    { 55, 5 },
                    { 56, 14 },
                    { 57, 5 },
                    { 58, 5 },
                    { 59, 5 },
                    { 60, 14 },
                    { 61, 3 },
                    { 62, 5 },
                    { 63, 5 },
                    { 64, 5 },
                    { 65, 5 },
                    { 66, 6 },
                    { 67, 2 },
                    { 68, 0 },
                    { 69, 0 },
                    { 70, 0 },
                    { 71, 0 },
                    { 72, 0 },
                    { 73, 14 },
                    { 74, 15 },
                    { 75, 9 },
                    { 76, 9 },
                    { 77, 9 },
                    { 78, 9 },
                    { 79, 9 },
                    { 80, 9 },
                    { 81, 9 },
                    { 82, 15 },
                    { 83, 9 },
                    { 84, 14 },
                    { 85, 14 },
                    { 86, 9 },
                    { 87, 9 },
                    { 88, 0 },
                    { 89, 0 },
                    { 90, 6 },
                    { 91, 6 },
                    { 92, 5 },
                    { 93, 5 },
                    { 94, 5 },
                    { 95, 5 },
                    { 96, 4 },
                    { 97, 5 },
                    { 98, 5 },
                    { 99, 5 },
                    { 100, 0 },
                    { 101, 15 },
                    { 102, 3 },
                    { 103, 3 },
                    { 104, 5 },
                    { 105, 4 },
                    { 106, 6 },
                    { 107, 6 },
                    { 108, 6 },
                    { 109, 6 },
                    { 110, 6 },
                    { 111, 4 },
                    { 112, 4 },
                    { 113, 4 },
                    { 114, 4 },
                    { 115, 4 },
                    { 116, 4 },
                    { 117, 11 },
                    { 118, 11 },
                    { 119, 11 },
                    { 120, 6 },
                    { 121, 6 },
                    { 122, 2 },
                    { 123, 2 },
                    { 124, 0 },
                    { 125, 14 },
                    { 126, 14 },
                    { 127, 14 },
                    { 128, 14 },
                    { 129, 14 },
                    { 130, 0 },
                    { 131, 3 },
                    { 132, 2 },
                    { 133, 5 },
                    { 134, 0 },
                    { 135, 3 },
                    { 136, 3 },
                    { 137, 5 },
                    { 138, 6 },
                    { 139, 5 },
                    { 140, 5 },
                    { 141, 14 },
                    { 142, 9 },
                    { 143, 5 },
                    { 144, 3 },
                    { 145, 3 },
                    { 146, 7 },
                    { 147, 1 },
                    { 148, 5 },
                    { 149, 5 },
                    { 150, 0 },
                    { 151, 0 },
                    { 152, 7 },
                    { 153, 5 },
                    { 154, 15 },
                    { 155, 15 },
                    { 156, 15 },
                    { 157, 15 },
                    { 158, 15 },
                    { 159, 15 },
                    { 160, 15 },
                    { 161, 11 },
                    { 162, 0 },
                    { 163, 5 },
                    { 164, 5 },
                    { 165, 5 },
                    { 166, 5 },
                    { 167, 15 },
                    { 168, 15 },
                    { 169, 15 },
                    { 170, 15 },
                    { 171, 15 },
                    { 172, 14 },
                    { 173, 15 },
                    { 174, 15 },
                    { 175, 15 },
                    { 176, 15 },
                    { 177, 15 },
                    { 178, 15 },
                    { 179, 11 },
                    { 180, 3 },
                    { 181, 15 },
                    { 182, 15 },
                    { 183, 15 },
                    { 184, 14 },
                    { 185, 6 },
                    { 186, 6 },
                    { 187, 6 },
                    { 188, 6 },
                    { 189, 6 },
                    { 190, 6 },
                    { 191, 6 },
                    { 192, 5 },
                    { 193, 5 },
                    { 194, 4 },
                    { 195, 4 },
                    { 196, 1 },
                    { 197, 1 },
                    { 198, 1 },
                    { 199, 1 },
                    { 200, 1 },
                    { 201, 1 },
                    { 202, 2 },
                    { 203, 8 },
                    { 204, 4 },
                    { 205, 2 },
                    { 206, 1 },
                    { 207, 4 },
                    { 208, 11 },
                    { 209, 11 },
                    { 210, 11 },
                    { 211, 11 },
                    { 212, 0 },
                    { 213, 1 },
                    { 214, 1 },
                    { 215, 1 },
                    { 216, 5 },
                    { 217, 4 },
                    { 218, 0 },
                    { 219, 5 },
                    { 220, 15 },
                    { 221, 15 },
                    { 222, 15 },
                    { 223, 15 },
                    { 224, 14 },
                    { 225, 15 },
                    { 226, 11 },
                    { 227, 3 },
                    { 228, 3 },
                    { 229, 4 },
                    { 230, 0 },
                    { 231, 0 },
                    { 232, 0 },
                    { 233, 11 },
                    { 234, 6 },
                    { 235, 1 },
                    { 236, 14 },
                    { 237, 3 },
                    { 238, 3 },
                    { 239, 3 },
                    { 240, 5 },
                    { 241, 3 },
                    { 242, 6 },
                    { 243, 6 },
                    { 244, 9 },
                    { 245, 14 },
                    { 246, 14 },
                    { 247, 4 },
                    { 248, 5 },
                    { 249, 14 },
                }
            },
        };
        public static Dictionary<bool, Dictionary<int, int>> EmtptySlots = new Dictionary<bool, Dictionary<int, int>>()
        {
            { true, new Dictionary<int, int>() {
                { 1, 0 },
                { 3, 15 },
                { 4, 21 },
                { 5, 0 },
                { 6, 34 },
                { 7, 0 },
                { 8, 15 },
                { 9, 0 },
                { 10, 0 },
                { 11, 15 },
            }},
            { false, new Dictionary<int, int>() {
                { 1, 0 },
                { 3, 15 },
                { 4, 15 },
                { 5, 0 },
                { 6, 35 },
                { 7, 0 },
                { 8, 6 },
                { 9, 0 },
                { 10, 0 },
                { 11, 15 },
            }}
        };
        public static Dictionary<bool, Dictionary<int, Dictionary<int, int>>> CorrectGloves = new Dictionary<bool, Dictionary<int, Dictionary<int, int>>>()
        {
            { true, new Dictionary<int, Dictionary<int, int>>() {
                { 1, new Dictionary<int, int>() {
                    { 4, 16 },
                }},
                { 2, new Dictionary<int, int>() {
                    { 4, 17 },
                }},
                { 3, new Dictionary<int, int>() {
                    { 4, 18 },
                }},
                { 4, new Dictionary<int, int>() {
                    { 0, 19 },
                    { 1, 20 },
                    { 2, 21 },
                    { 4, 22 },
                    { 5, 23 },
                    { 6, 24 },
                    { 8, 25 },
                    { 11, 26 },
                    { 12, 27 },
                    { 14, 28 },
                    { 15, 29 },
                    { 112, 115 },
                    { 113, 122 },
                    { 114, 129 },
                }},
                { 5, new Dictionary<int, int>() {
                    { 0, 30 },
                    { 1, 31 },
                    { 2, 32 },
                    { 4, 33 },
                    { 5, 34 },
                    { 6, 35 },
                    { 8, 36 },
                    { 11, 37 },
                    { 12, 38 },
                    { 14, 39 },
                    { 15, 40 },
                    { 112, 116 },
                    { 113, 123 },
                    { 114, 130 },
                }},
                { 6, new Dictionary<int, int>() {
                    { 0, 41 },
                    { 1, 42 },
                    { 2, 43 },
                    { 4, 44 },
                    { 5, 45 },
                    { 6, 46 },
                    { 8, 47 },
                    { 11, 48 },
                    { 12, 49 },
                    { 14, 50 },
                    { 15, 51 },
                    { 112, 117 },
                    { 113, 124 },
                    { 114, 131 },
                }},
                { 7, new Dictionary<int, int>() {
                    { 0, 52 },
                    { 1, 53 },
                    { 2, 54 },
                    { 4, 55 },
                    { 5, 56 },
                    { 6, 57 },
                    { 8, 58 },
                    { 11, 59 },
                    { 12, 60 },
                    { 14, 61 },
                    { 15, 62 },
                    { 112, 118 },
                    { 113, 125 },
                    { 114, 132 },
                }},
                { 8, new Dictionary<int, int>() {
                    { 0, 63 },
                    { 1, 64 },
                    { 2, 65 },
                    { 4, 66 },
                    { 5, 67 },
                    { 6, 68 },
                    { 8, 69 },
                    { 11, 70 },
                    { 12, 71 },
                    { 14, 72 },
                    { 15, 73 },
                    { 112, 119 },
                    { 113, 126 },
                    { 114, 133 },
                }},
                { 9, new Dictionary<int, int>() {
                    { 0, 74 },
                    { 1, 75 },
                    { 2, 76 },
                    { 4, 77 },
                    { 5, 78 },
                    { 6, 79 },
                    { 8, 80 },
                    { 11, 81 },
                    { 12, 82 },
                    { 14, 83 },
                    { 15, 84 },
                    { 112, 120 },
                    { 113, 127 },
                    { 114, 134 },
                }},
                { 10, new Dictionary<int, int>() {
                    { 0, 85 },
                    { 1, 86 },
                    { 2, 87 },
                    { 4, 88 },
                    { 5, 89 },
                    { 6, 90 },
                    { 8, 91 },
                    { 11, 92 },
                    { 12, 93 },
                    { 14, 94 },
                    { 15, 95 },
                    { 112, 121 },
                    { 113, 128 },
                    { 114, 135 },
                }},
                { 11, new Dictionary<int, int>() {
                    { 4, 96 },
                }},
                { 12, new Dictionary<int, int>() {
                    { 4, 97 },
                }},
                { 13, new Dictionary<int, int>() {
                    { 0, 99 },
                    { 1, 100 },
                    { 2, 101 },
                    { 4, 102 },
                    { 5, 103 },
                    { 6, 104 },
                    { 8, 105 },
                    { 11, 106 },
                    { 12, 107 },
                    { 14, 108 },
                    { 15, 109 },
                }},
                { 14, new Dictionary<int, int>() {
                    { 4, 110 },
                }},
            }},
            { false, new Dictionary<int, Dictionary<int, int>>() {
                { 1, new Dictionary<int, int>(){
                    { 3, 17 },
                }},
                { 2, new Dictionary<int, int>(){
                    { 3, 18 },
                }},
                { 3, new Dictionary<int, int>(){
                    { 3, 19 },
                }},
                { 4, new Dictionary<int, int>(){
                    { 0, 20 },
                    { 1, 21 },
                    { 2, 22 },
                    { 3, 23 },
                    { 4, 24 },
                    { 5, 25 },
                    { 6, 26 },
                    { 7, 27 },
                    { 9, 28 },
                    { 11, 29 },
                    { 12, 30 },
                    { 14, 31 },
                    { 15, 32 },
                    { 129, 132 },
                    { 130, 139 },
                    { 131, 146 },
                    { 153, 154 },
                    { 161, 162 },
                }},
                { 5, new Dictionary<int, int>(){
                    { 0, 33 },
                    { 1, 34 },
                    { 2, 35 },
                    { 3, 36 },
                    { 4, 37 },
                    { 5, 38 },
                    { 6, 39 },
                    { 7, 40 },
                    { 9, 41 },
                    { 11, 42 },
                    { 12, 43 },
                    { 14, 44 },
                    { 15, 45 },
                    { 129, 133 },
                    { 130, 140 },
                    { 131, 147 },
                    { 153, 155 },
                    { 161, 163 },
                }},
                { 6, new Dictionary<int, int>(){
                    { 0, 46 },
                    { 1, 47 },
                    { 2, 48 },
                    { 3, 49 },
                    { 4, 50 },
                    { 5, 51 },
                    { 6, 52 },
                    { 7, 53 },
                    { 9, 54 },
                    { 11, 55 },
                    { 12, 56 },
                    { 14, 57 },
                    { 15, 58 },
                    { 129, 134 },
                    { 130, 141 },
                    { 131, 148 },
                    { 153, 156 },
                    { 161, 164 },
                }},
                { 7, new Dictionary<int, int>(){
                    { 0, 59 },
                    { 1, 60 },
                    { 2, 61 },
                    { 3, 62 },
                    { 4, 63 },
                    { 5, 64 },
                    { 6, 65 },
                    { 7, 66 },
                    { 9, 67 },
                    { 11, 68 },
                    { 12, 69 },
                    { 14, 70 },
                    { 15, 71 },
                    { 129, 135 },
                    { 130, 142 },
                    { 131, 149 },
                    { 153, 157 },
                    { 161, 165 },
                }},
                { 8, new Dictionary<int, int>(){
                    { 0, 72 },
                    { 1, 73 },
                    { 2, 74 },
                    { 3, 75 },
                    { 4, 76 },
                    { 5, 77 },
                    { 6, 78 },
                    { 7, 79 },
                    { 9, 80 },
                    { 11, 81 },
                    { 12, 82 },
                    { 14, 83 },
                    { 15, 84 },
                    { 129, 136 },
                    { 130, 143 },
                    { 131, 150 },
                    { 153, 158 },
                    { 161, 166 },
                }},
                { 9, new Dictionary<int, int>(){
                    { 0, 85 },
                    { 1, 86 },
                    { 2, 87 },
                    { 3, 88 },
                    { 4, 89 },
                    { 5, 90 },
                    { 6, 91 },
                    { 7, 92 },
                    { 9, 93 },
                    { 11, 94 },
                    { 12, 95 },
                    { 14, 96 },
                    { 15, 97 },
                    { 129, 137 },
                    { 130, 144 },
                    { 131, 151 },
                    { 153, 159 },
                    { 161, 167 },
                }},
                { 10, new Dictionary<int, int>(){
                    { 0, 98 },
                    { 1, 99 },
                    { 2, 100 },
                    { 3, 101 },
                    { 4, 102 },
                    { 5, 103 },
                    { 6, 104 },
                    { 7, 105 },
                    { 9, 106 },
                    { 11, 107 },
                    { 12, 108 },
                    { 14, 109 },
                    { 15, 110 },
                    { 129, 138 },
                    { 130, 145 },
                    { 131, 152 },
                    { 153, 160 },
                    { 161, 168 },
                }},
                { 11, new Dictionary<int, int>(){
                    { 3, 111 },
                }},
                { 12, new Dictionary<int, int>(){
                    { 0, 114 },
                    { 1, 115 },
                    { 2, 116 },
                    { 3, 117 },
                    { 4, 118 },
                    { 5, 119 },
                    { 6, 120 },
                    { 7, 121 },
                    { 9, 122 },
                    { 11, 123 },
                    { 12, 124 },
                    { 14, 125 },
                    { 15, 126 },
                }},
            }},
        };

        // словарь, в котором находятся соответствующие для key undershirts IDшники Underwears
        public static Dictionary<bool, Dictionary<int, int>> Undershirts = new Dictionary<bool, Dictionary<int, int>>()
        {
            { true, new Dictionary<int, int>() },
            { false, new Dictionary<int, int>() }
        };
        public static Dictionary<bool, Dictionary<int, Underwear>> Underwears = new Dictionary<bool, Dictionary<int, Underwear>>()
        {
            { true, new Dictionary<int, Underwear>(){
                { 0, new Underwear(0, 1500, new Dictionary<int, int>(){ { 0, 0 }, { 1, 2 } }, new List<int>() { 0, 1, 2, 3, 4 }, 1) },
                { 1, new Underwear(1, 1600, new Dictionary<int, int>(){ { 0, 1 }, { 1, 14 } }, new List<int>() { 0, 1, 3, 4, 5, 6, 7, 8 }, 1) },
                { 2, new Underwear(5, 1400, new Dictionary<int, int>(){ { 0, 5 } }, new List<int>() { 0, 1, 2 }, 1) },
                { 3, new Underwear(8, 1500, new Dictionary<int, int>(){ { 0, 8 } }, new List<int>() { 0 }, 1) },
                { 4, new Underwear(9, 1500, new Dictionary<int, int>(){ { 0, 9 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7}, 1) },
                { 5, new Underwear(12, 3500, new Dictionary<int, int>(){ { 0, 12 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}, 1) },
                { 6, new Underwear(13, 3500, new List<int>() { 0, 1, 2, 3}, 4) },
                { 7, new Underwear(14, 3500, new Dictionary<int, int>(){ { 0, 29 }, { 1, 30 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15}, 2) },
                { 8, new Underwear(16, 1600, new Dictionary<int, int>(){ { 0, 16 }, { 1, 18 } }, new List<int>() { 0, 1, 2 }, 1) },
                { 9, new Underwear(17, 1600, new Dictionary<int, int>(){ { 0, 17 } }, new List<int>() { 0, 1, 2, 3, 4, 5 }, 1) },
                { 10, new Underwear(22, 1500, new Dictionary<int, int>(){ { 0, 23 }, { 1, 24 } }, new List<int>() { 0, 1, 2 }, 1) },
                { 11, new Underwear(26, 5000, new Dictionary<int, int>(){ { 0, 27 }, { 1, 27 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9}, 4) },
                { 12, new Underwear(33, 1500, new Dictionary<int, int>(){ { 0, 37 }, { 1, 39 } }, new List<int>() { 0 }) },
                { 13, new Underwear(34, 1500, new Dictionary<int, int>(){ { 0, 38 }, { 1, 44 } }, new List<int>() { 0, 1}) },
                { 14, new Underwear(36, 1500, new Dictionary<int, int>(){ { 0, 40 } }, new List<int>() { 0, 1, 2, 3, 4, 5}) },
                { 15, new Underwear(38, 3500, new Dictionary<int, int>(){ { 0, 41 } }, new List<int>() { 0, 1, 2, 3, 4 }, 1) },
                { 16, new Underwear(39, 2500, new Dictionary<int, int>(){ { 0, 42 } }, new List<int>() { 0, 1}, 1) },
                { 17, new Underwear(41, 3500, new Dictionary<int, int>(){ { 0, 43 } }, new List<int>() { 0, 1, 2, }, 1) },
                { 18, new Underwear(44, 1500, new Dictionary<int, int>(){ { 0, 47 }, { 1, 48 } }, new List<int>() { 0, 1, 2, 3 }, 1) },
                { 19, new Underwear(47, 1500, new Dictionary<int, int>(){ { 0, 53 }, { 1, 54 } }, new List<int>() { 0, 1 }, 1) },
                { 20, new Underwear(71, 25000, new Dictionary<int, int>(){ { 0, 67 }, { 1, 68 } }, new List<int>() { 0}, 1) },
                { 21, new Underwear(73, 100000, new Dictionary<int, int>(){ { 0, 65 }, { 1, 66 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15}, 1) },
                { 22, new Underwear(208, 7000, new Dictionary<int, int>(){ { 0, 101 }, { 1, 102 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }, 1) },
                { 23, new Underwear(81, 15000, new List<int>() { 0, 1, 2 }, 12) },
                { 24, new Underwear(82, 100000, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 11, 12, 13, 14, 15 }, 10) },
                { 25, new Underwear(83, 15000, new List<int>() { 0, 1, 2, 3, 4 }, 10) },
                { 26, new Underwear(92, 25000, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, 10) },
                { 27, new Underwear(117, 12500, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 10) },
                { 28, new Underwear(126, 5000, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }, 12) },
                { 29, new Underwear(128, 12500, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 10) },
                { 30, new Underwear(135, 20000, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, 12) },
                { 31, new Underwear(144, 25000, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, 10) },
                { 32, new Underwear(123, 7000, new List<int>() { 0, 1, 2 }, 10) },
                { 33, new Underwear(139, 5000, new Dictionary<int, int>(){ { 0, 75 }, { 1, 74 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 10) },
                { 34, new Underwear(225, 3000, new List<int>() { 0, 1 }, 10) },
                { 35, new Underwear(226, 4000, new Dictionary<int, int>(){ { 0, 106 }, { 1, 107 } }, new List<int>() { 0 }, 10) },
                { 36, new Underwear(235, 8000, new Dictionary<int, int>(){ { 0, 109 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 11 }, 10) },
                { 37, new Underwear(255, 25000, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 10) },
                { 38, new Underwear(-1, 5000, new Dictionary<int, int>(){ { 3, 6 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }) },
                { 39, new Underwear(-1, 5000, new Dictionary<int, int>(){ { 0, 21 }, { 3, 22 } }, new List<int>() { 0, 1, 2, 3, 4 }) },
                { 40, new Underwear(-1, 8000, new Dictionary<int, int>(){ { 3, 50 } }, new List<int>() { 0, 1 }) },
                { 41, new Underwear(-1, 8000, new Dictionary<int, int>(){ { 0, 94 }, { 3, 93 } }, new List<int>() { 0, 1 }) },
                { 42, new Underwear(-1, 15000, new Dictionary<int, int>(){ { 0, 4 } }, new List<int>() { 0, 1, 2 }) },
                { 43, new Underwear(-1, 15000, new Dictionary<int, int>(){ { 0, 26 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }) },
            }},
            { false, new Dictionary<int, Underwear>(){
                { 0, new Underwear(0, 1500, new Dictionary<int, int>() { { 0, 0 }, { 1, 1 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15}, 13) },
                { 1, new Underwear(5, 1500, new Dictionary<int, int>() { { 0, 5 }, { 1, 5 } }, new List<int>() { 0, 1, 7, 9}, 13) },
                { 2, new Underwear(11, 1500, new Dictionary<int, int>() { { 0, 11 }, { 1, 11 } }, new List<int>() { 0, 2, 10, 11, 15}, 13) },
                { 3, new Underwear(13, 20000, new Dictionary<int, int>() { { 0, 13 }, { 1, 13 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15}, 13) },
                { 4, new Underwear(16, 2000, new Dictionary<int, int>() { { 0, 16 }, { 1, 16 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6}, 13) },
                { 5, new Underwear(22, 20000, new Dictionary<int, int>() { { 0, 22 }, { 1, 22 } }, new List<int>() { 0, 1, 2, 3, 4}, 13) },
                { 6, new Underwear(23, 2000, new Dictionary<int, int>() { { 0, 20 }, { 1, 21 } }, new List<int>() { 0, 1, 2}, 13) },
                { 7, new Underwear(26, 5000, new Dictionary<int, int>() { { 0, 23 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12}, 13) },
                { 8, new Underwear(30, 1500, new Dictionary<int, int>() { { 0, 26 } }, new List<int>() { 0, 1, 2, 13}) },
                { 9, new Underwear(32, 5000, new Dictionary<int, int>() { { 0, 27 }, { 1, 27 } }, new List<int>() { 0, 1, 2}, 13) },
                { 10, new Underwear(33, 15000, new Dictionary<int, int>() { { 0, 28 }, { 1, 28 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8}, 13) },
                { 11, new Underwear(36, 5000, new Dictionary<int, int>() { { 0, 29 }, { 1, 29 } }, new List<int>() { 0, 1, 2, 3, 4}, 13) },
                { 12, new Underwear(38, 1500, new Dictionary<int, int>() { { 0, 30 } }, new List<int>() { 0, 1, 2, 3}, 13) },
                { 13, new Underwear(40, 1500, new Dictionary<int, int>() { { 0, 31 } }, new List<int>() { 0}, 13) },
                { 14, new Underwear(49, 10000, new Dictionary<int, int>() { { 0, 51 } }, new List<int>() { 0, 1}, 13) },
                { 15, new Underwear(67, 6000, new Dictionary<int, int>() { { 0, 49 } }, new List<int>() { 0}, 13) },
                { 16, new Underwear(68, 10000, new Dictionary<int, int>() { { 0, 50 }, { 1, 45 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19}, 13) },
                { 17, new Underwear(73, 1500, new Dictionary<int, int>() { { 0, 57 }, { 1, 59 } }, new List<int>() { 0, 1, 2}, 13) },
                { 18, new Underwear(74, 5000, new Dictionary<int, int>() { { 0, 60 }, { 1, 60 } }, new List<int>() { 0, 1, 2}, 13) },
                { 19, new Underwear(75, 5000, new Dictionary<int, int>() { { 0, 61 }, { 1, 63 } }, new List<int>() { 0, 1, 2, 3}, 13) },
                { 20, new Underwear(103, 3500, new Dictionary<int, int>() { { 1, 67 } }, new List<int>() { 0, 1, 2, 3, 4, 5}, 13) },
                { 21, new Underwear(111, 50000, new Dictionary<int, int>() { { 0, 68 }, { 1, 68 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}, 13) },
                { 22, new Underwear(117, 3000, new Dictionary<int, int>() { { 0, 71 }, { 1, 71 } }, new List<int>() { 0, 1, 2}, 13) },
                { 23, new Underwear(118, 3000, new Dictionary<int, int>() { { 0, 74 }, { 1, 74 } }, new List<int>() { 0, 1, 2}, 13) },
                { 24, new Underwear(141, 1500, new Dictionary<int, int>() { { 0, 80 }, { 1, 80 } }, new List<int>() { 0, 1, 2, 3, 4, 5}, 13) },
                { 25, new Underwear(149, 5000, new Dictionary<int, int>() { { 0, 82 }, { 1, 83 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15}, 12) },
                { 26, new Underwear(208, 10000, new Dictionary<int, int>() { { 0, 111 }, { 1, 108 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16}, 12) },
                { 27, new Underwear(9, 10000, new Dictionary<int, int>() { }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }, 12) },
                { 28, new Underwear(83, 20000, new Dictionary<int, int>() { }, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, 12) },
                { 29, new Underwear(142, 25000, new Dictionary<int, int>() { }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, 12) },
                { 30, new Underwear(171, 20000, new Dictionary<int, int>() { }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 12) },
                { 31, new Underwear(17, 5000, new Dictionary<int, int>() { }, new List<int>() { 0 }, 12) },
                { 32, new Underwear(76, 12500, new Dictionary<int, int>() { }, new List<int>() { 0, 1, 2, 3, 4 }, 13) },
                { 33, new Underwear(79, 5000, new Dictionary<int, int>() { }, new List<int>() { 0, 1, 2, 3 }, 12) },
                { 34, new Underwear(109, 5000, new Dictionary<int, int>() { }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 12) },
                { 35, new Underwear(198, 12500, new Dictionary<int, int>() { }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 12) },
            }},
        };

        public static Dictionary<bool, List<Clothes>> Hats = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){
                new Clothes(2, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 500),
                new Clothes(4, new List<int>() { 0, 1 }, 500),
                new Clothes(5, new List<int>() { 0, 1,}, 1000),
                new Clothes(7, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 3100),
                new Clothes(12, new List<int>() { 0, 1, 2, 4, 6, 7 }, 3100),
                new Clothes(13, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 3750),
                new Clothes(14, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 1250),
                new Clothes(20, new List<int>() { 0, 1, 2, 3, 4, 5 }, 1750),
                new Clothes(21, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 2500),
                new Clothes(25, new List<int>() { 0, 1, 2 }, 1000),
                new Clothes(26, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, 5000),
                new Clothes(27, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, 10000),
                new Clothes(28, new List<int>() { 0, 1, 2, 3, 4, 5 }, 2500),
                new Clothes(29, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 2500),
                new Clothes(37, new List<int>() { 0, 1, 2, 3, 4, 5 }, 3750),
                new Clothes(40, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 2500),
                new Clothes(44, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 2500),
                new Clothes(45, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 2500),
            }},
            { false, new List<Clothes>(){
                new Clothes(4, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 2500),
                new Clothes(5, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 2500),
                new Clothes(7, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 3750),
                new Clothes(9, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 3500),
                new Clothes(12, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 3500),
                new Clothes(13, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 2500),
                new Clothes(14, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 5000),
                new Clothes(22, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 4000),
            }},
        };
        public static Dictionary<bool, List<Clothes>> Legs = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){
                new Clothes(0, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 2500),
                new Clothes(1, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 2500),
                new Clothes(3, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 2500),
                new Clothes(4, new List<int>() { 0, 1, 2, 4 }, 7500),
                new Clothes(5, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 2500),
                new Clothes(6, new List<int>() { 0, 1, 2, }, 2500),
                new Clothes(7, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 4000),
                new Clothes(8, new List<int>() { 0, 3, 4 }, 5000),
                new Clothes(9, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 5000),
                new Clothes(13, new List<int>() { 0, 1, 2 }, 7000),
                new Clothes(14, new List<int>() { 0, 1 }, 1500),
                new Clothes(15, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 3000),
                new Clothes(16, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 3500),
                new Clothes(17, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 2500),
                new Clothes(20, new List<int>() { 0, 1, 2, 3 }, 10000),
                new Clothes(22, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, 10000),
                new Clothes(24, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, 3750),
                new Clothes(25, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, 5000),
                new Clothes(26, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 8000),
                new Clothes(27, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 3500),
                new Clothes(28, new List<int>() { 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }, 10000),
                new Clothes(32, new List<int>() { 0, 1, 2, 3}, 15000),
                new Clothes(43, new List<int>() { 0, 1}, 6000),
                new Clothes(42, new List<int>() { 0, 1, 2, 3, 4, 5 }, 15000),
                new Clothes(54, new List<int>() { 0, 1, 2, 3, 4, 5 }, 10000),
                new Clothes(69, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }, 20000),
                new Clothes(63, new List<int>() { 0 }, 4000),
                new Clothes(78, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 10000),
                new Clothes(79, new List<int>() { 0, 1, 2 }, 12000),
                new Clothes(80, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 7000),
                new Clothes(81, new List<int>() { 0, 1, 2 }, 9000),
                new Clothes(82, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 10000),
                new Clothes(83, new List<int>() { 0, 1, 2, 3 }, 12000),
                new Clothes(90, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 10000),
                new Clothes(94, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 10000),
                new Clothes(96, new List<int>() { 0, 1 }, 5000),
                new Clothes(100, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, 15000),
                new Clothes(103, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 }, 12000),
                new Clothes(105, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 15000),
            }},
            { false, new List<Clothes>(){
                new Clothes(0, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 7000),
                new Clothes(1, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 5000),
                new Clothes(2, new List<int>() { 0, 1, 2 }, 5000),
                new Clothes(3, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 2500),
                new Clothes(4, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 3000),
                new Clothes(6, new List<int>() { 0, 1, 2 }, 10000),
                new Clothes(7, new List<int>() { 0, 1, 2 }, 10000),
                new Clothes(8, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, 15000),
                new Clothes(9, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 5000),
                new Clothes(10, new List<int>() { 0, 1, 2 }, 2500),
                new Clothes(11, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 2500),
                new Clothes(12, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 10000),
                new Clothes(16, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 5000),
                new Clothes(23, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 8000),
                new Clothes(24, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, 5000),
                new Clothes(25, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, 5000),
                new Clothes(27, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 7000),
                new Clothes(11, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 5000),
                new Clothes(18, new List<int>() { 0, 1 }, 2000),
                new Clothes(53, new List<int>() { 0 }, 4000),
                new Clothes(60, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 5000),
                new Clothes(71, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 }, 5000),
                new Clothes(73, new List<int>() { 0, 1, 2, 3, 4, 5 }, 20000),
                new Clothes(75, new List<int>() { 0, 1, 2 }, 5000),
                new Clothes(78, new List<int>() { 0, 1, 2, 3 }, 20000),
                new Clothes(102, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }, 25000),
                new Clothes(104, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, 10000),
                new Clothes(106, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 15000),
                new Clothes(108, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 7000),
                new Clothes(43, new List<int>() { 0, 1, 2, 3, 4 }, 10000),
                new Clothes(44, new List<int>() { 0, 1, 2, 3, 4 }, 15000),
                new Clothes(74, new List<int>() { 0, 1, 2, 3, 4, 5 }, 3000),
                new Clothes(77, new List<int>() { 0, 1, 2 }, 15000),
            }},
        };
        public static Dictionary<bool, List<Clothes>> Feets = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){
                new Clothes(1, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 2500),
                new Clothes(3, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 5000),
                new Clothes(4, new List<int>() { 0, 1, 2, 4,  }, 3100),
                new Clothes(5, new List<int>() { 0, 1, 2, 3 }, 2500),
                new Clothes(6, new List<int>() { 0, 1 }, 7000),
                new Clothes(7, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 10000),
                new Clothes(8, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 3000),
                new Clothes(9, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 2500),
                new Clothes(12, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 10000),
                new Clothes(14, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 6000),
                new Clothes(15, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 5000),
                new Clothes(16, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, 2500),
                new Clothes(20, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 10000),
                new Clothes(21, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 3600),
                new Clothes(22, new List<int>() { 0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11 }, 5000),
                new Clothes(23, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 15000),
                new Clothes(26, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 13, 14, 15 }, 3600),
                new Clothes(28, new List<int>() { 0, 1, 2, 3, 4, 5 }, 15000),
                new Clothes(32, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 9000),
                new Clothes(42, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 10000),
                new Clothes(18, new List<int>() { 0 }, 20000),
                new Clothes(19, new List<int>() { 0 }, 4000),
                new Clothes(29, new List<int>() { 0 }, 10000),
                new Clothes(31, new List<int>() { 0, 1, 2, 3, 4 }, 10000),
                new Clothes(36, new List<int>() { 0, 1, 2, 3 }, 15000),
                new Clothes(37, new List<int>() { 0, 1, 2, 3, 4 }, 7000),
                new Clothes(41, new List<int>() { 0 }, 4000),
                new Clothes(43, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 10000),
                new Clothes(46, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 8000),
                new Clothes(48, new List<int>() { 0, 1 }, 5000),
                new Clothes(49, new List<int>() { 0, 1 }, 5000),
                new Clothes(50, new List<int>() { 0, 1, 2, 3, 4, 5 }, 15000),
                new Clothes(52, new List<int>() { 0, 1 }, 6000),
                new Clothes(53, new List<int>() { 0, 1, 2, 3, 4, 5 }, 12000),
                new Clothes(57, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 10000),
                new Clothes(59, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 15000),
                new Clothes(64, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, 4000),
                new Clothes(65, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, 12000),
                new Clothes(74, new List<int>() { 0, 1 }, 4000),
                new Clothes(75, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 35000),
                new Clothes(77, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 25000),
                new Clothes(79, new List<int>() { 0, 1 }, 30000),
                new Clothes(81, new List<int>() { 0, 1, 2 }, 8000),
            }},
            { false, new List<Clothes>(){
                new Clothes(0, new List<int>() { 0, 1, 2, 3 }, 5000),
                new Clothes(1, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 5000),
                new Clothes(2, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 5000),
                new Clothes(3, new List<int>() { 0, 1, 2, 3, 4, 5 }, 3600),
                new Clothes(6, new List<int>() { 0, 1, 2, 3 }, 3600),
                new Clothes(7, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 5000),
                new Clothes(8, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 5000),
                new Clothes(14, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 7000),
                new Clothes(15, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 3600),
                new Clothes(9, new List<int>() { 0, 1, 2, 3 }, 10000),
                new Clothes(21, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 7000),
                new Clothes(32, new List<int>() { 0, 1, 2, 3, 4 }, 10000),
                new Clothes(33, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 5000),
                new Clothes(4, new List<int>() { 0, 1, 2, 3 }, 5000),
                new Clothes(16, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 5000),
                new Clothes(49, new List<int>() { 0, 1 }, 10000),
                new Clothes(50, new List<int>() { 0, 1 }, 10000),
                new Clothes(10, new List<int>() { 0, 1, 2, 3 }, 7500),
                new Clothes(11, new List<int>() { 0, 1, 2, 3 }, 10000),
                new Clothes(23, new List<int>() { 0, 1, 2 }, 20000),
                new Clothes(77, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, 5000),
                new Clothes(43, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 5000),
                new Clothes(44, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 5000),
                new Clothes(81, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 15000),
            }},
        };
        public static Dictionary<bool, List<Clothes>> Tops = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){
                new Clothes(3, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 2500, 0, 3),
                new Clothes(4, new List<int>() { 0, 2, 3, 11, 14 }, 10000, 0, 3),
                new Clothes(7, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 3500, 0, 3),
                new Clothes(10, new List<int>() { 0, 1, 2 }, 5000, 0, 8),
                new Clothes(20, new List<int>() { 0, 1, 2, 3 }, 15000, 0, 8),
                new Clothes(23, new List<int>() { 0, 1, 2, 3}, 8000, 0, 3),
                new Clothes(24, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, 15000, 0, 8),
                new Clothes(27, new List<int>() { 0, 1, 2 }, 8000, 0, 8),
                new Clothes(28, new List<int>() { 0, 1, 2 }, 8000, 0, 8),
                new Clothes(35, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, 10000, 0, 3),
                new Clothes(58, new List<int>() { 0 }, 5000, 0, 3),
                new Clothes(59, new List<int>() { 0, 1, 2, 3 }, 5000, 0, 3),
                new Clothes(62, new List<int>() { 0 }, 25000, 0, 3),
                new Clothes(70, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}, 50000, 0, 3),
                new Clothes(72, new List<int>() { 0, 1, 2, 3 }, 20000, 0, 3),
                new Clothes(74, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 100000, 0, 3),
                new Clothes(77, new List<int>() { 0, 1, 2, 3 }, 20000, 0, 3),
                new Clothes(88, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}, 5000, 0, 3),
                new Clothes(106, new List<int>() { 0 }, 20000, 0, 3),
                new Clothes(115, new List<int>() { 0 }, 20000, 0, 3),
                new Clothes(118, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 3600, 0, 3),
                new Clothes(119, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 5000, 0, 3),
                new Clothes(122, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, 5000, 0, 3),
                new Clothes(124, new List<int>() { 0 }, 5000, 0, 8),
                new Clothes(127, new List<int>() {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }, 5000, 0, 3),
                new Clothes(130, new List<int>() { 0 }, 5000, 0, 3),
                new Clothes(136, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, 5000, 0, 3),
                new Clothes(142, new List<int>() { 0, 1, 2 }, 10000, 0, 3),
                new Clothes(151, new List<int>() { 0, 1, 2, 3, 4, 5 }, 15000, 0, 3),
                new Clothes(156, new List<int>() { 0, 1, 2, 3, 4, 5 }, 15000, 0, 3),
                new Clothes(163, new List<int>() { 0 }, 10000, 0, 3),
                new Clothes(166, new List<int>() { 0, 1, 2, 3, 4, 5 }, 5000, 0, 3),
                new Clothes(167, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 10000, 0, 3),
                new Clothes(169, new List<int>() { 0, 1, 2, 3 }, 5000, 0, 3),
                new Clothes(172, new List<int>() { 0, 1, 2, 3 }, 7000, 0, 3),
                new Clothes(181, new List<int>() { 0, 1, 2, 3, 4, 5 }, 5000, 0, 3),
                new Clothes(185, new List<int>() { 0, 1, 2, 3 }, 5000, 0, 3),
                new Clothes(189, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 5000, 0, 3),
                new Clothes(191, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 10000, 0, 3),
                new Clothes(192, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 5000, 0, 3),
                new Clothes(115, new List<int>() { 0 }, 10000, 0, 3),
                new Clothes(157, new List<int>() { 0, 1, 2, 3 }, 5000, 0, 3),
                new Clothes(160, new List<int>() { 0, 1 }, 5000, 0, 3),
                new Clothes(6, new List<int>() { 0, 1, 3, 4, 5, 6, 8, 9, 11 }, 8000, 1, 9),
                new Clothes(37, new List<int>() { 0, 1, 2 }, 10500, 1, 9),
                new Clothes(64, new List<int>() { 0 }, 8000, 1, 9),
                new Clothes(68, new List<int>() { 0, 1, 2, 3, 4, 5 }, 5000, 1, 9),
                new Clothes(69, new List<int>() { 0, 1, 2, 3, 4, 5 }, 5000, 1, 9),
                new Clothes(76, new List<int>() { 0, 1, 2, 3, 4 }, 10000, 1, 9),
                new Clothes(161, new List<int>() { 0, 1, 2, 3}, 15000, 1, 9),
                new Clothes(168, new List<int>() { 0, 1, 2 }, 5000, 1, 12),
                new Clothes(174, new List<int>() { 0, 1, 2, 3 }, 17000, 1, 9),
                new Clothes(61, new List<int>() { 0, 1, 2, 3 }, 5000, 2, 10),
                new Clothes(75, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 100000, 2, 10),
                new Clothes(78, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 75000, 2, 10),
                new Clothes(79, new List<int>() { 0 }, 5000, 2, 12),
                new Clothes(84, new List<int>() { 0, 1, 2, 3, 4, 5 }, 5000, 2, 12),
                new Clothes(86, new List<int>() { 0, 1, 2, 3, 4 }, 5000, 2, 12),
                new Clothes(90, new List<int>() { 0 }, 5000, 2, 12),
                new Clothes(96, new List<int>() { 0 }, 3600, 2, 12),
                new Clothes(107, new List<int>() { 0, 1, 2, 3, 4 }, 10000, 2, 12),
                new Clothes(110, new List<int>() { 0 }, 5000, 2, 12),
                new Clothes(113, new List<int>() { 0, 1, 2, 3 }, 5000, 2, 10),
                new Clothes(125, new List<int>() { 0 }, 5000, 2, 10),
                new Clothes(129, new List<int>() { 0 }, 5000, 2, 12),
                new Clothes(134, new List<int>() { 0, 1, 2 }, 10000, 2, 12),
                new Clothes(138, new List<int>() { 0, 1, 2 }, 5000, 2, 12),
                new Clothes(141, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 5000, 2, 10),
                new Clothes(143, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 5000, 2, 12),
                new Clothes(147, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 5000, 2, 12),
                new Clothes(148, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 3100, 2, 12),
                new Clothes(150, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 8000, 2, 12),
                new Clothes(152, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 5000, 2, 10),
                new Clothes(153, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 5000, 2, 12),
                new Clothes(154, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 5000, 2, 12),
                new Clothes(171, new List<int>() { 0, 1 }, 5000, 2, 12),
                new Clothes(182, new List<int>() { 0, 1 }, 5000, 2, 12),
                new Clothes(184, new List<int>() { 0, 1, 2, 3 }, 5000, 2, 12),
                new Clothes(187, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12}, 10000, 2, 12),
                new Clothes(188, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 10000, 2, 12),
                new Clothes(200, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 10000, 2, 12),
                new Clothes(203, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 10000, 2, 12),
                new Clothes(204, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, 7500, 2, 12),
                new Clothes(85, new List<int>() { 0 }, 5000, 2, 10),
                new Clothes(87, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 10000, 2, 12),
                new Clothes(89, new List<int>() { 0, 1, 2, 3 }, 5000, 2, 10),
                new Clothes(190, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 10000, 2, 10),
                new Clothes(111, new List<int>() { 0, 1, 2, 3, 4, 5 }, 5000, 2, 10),
                new Clothes(121, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 10000, 2, 12),
                new Clothes(251, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 20000, 2, 10),
                new Clothes(249, new List<int>() { 0, 1 }, 10000, 2, 12),
                new Clothes(254, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, 5000, 2, 12),
                new Clothes(11, new List<int>() { 0, 1 }, 10000, 3, 10),
                new Clothes(21, new List<int>() { 0, 1, 2, 3 }, 20000, 3, 10),
                new Clothes(25, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 15000, 3, 10),
                new Clothes(40, new List<int>() { 0, 1 }, 10000, 3, 10),
                new Clothes(45, new List<int>() { 0, 1, 2 }, 3000, 3, 10),
                new Clothes(109, new List<int>() { 0 }, 3000, 3, 12),
                new Clothes(120, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 20000, 3, 10),
                new Clothes(159, new List<int>() { 0, 1 }, 8000, 3, 9),
                new Clothes(162, new List<int>() { 0, 1, 2, 3 }, 8000, 3, 9),
                new Clothes(176, new List<int>() { 0 }, 8000, 3, 9),
            }},
            { false, new List<Clothes>(){
                new Clothes(1, new List<int>() { 0, 1, 2, 4, 5, 6, 9, 11, 14 }, 3500, 0, 4),
                new Clothes(6, new List<int>() { 0, 1, 2, 4 }, 5000, 0, 5),
                new Clothes(7, new List<int>() { 0, 1, 2, 8 }, 10000, 0, 4),
                new Clothes(8, new List<int>() { 0, 1, 2, 12 }, 3500, 0, 4),
                new Clothes(24, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 10000, 0, 5),
                new Clothes(25, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 10000, 0, 4),
                new Clothes(31, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, 3500, 0, 4),
                new Clothes(34, new List<int>() { 0 }, 20000, 0, 4),
                new Clothes(35, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 3500, 0, 4),
                new Clothes(53, new List<int>() { 0, 1, 2, 3}, 5000, 0, 4),
                new Clothes(64, new List<int>() { 0, 1, 2, 3, 4 }, 10000, 0, 4),
                new Clothes(65, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 35000, 0, 5),
                new Clothes(90, new List<int>() { 0, 1, 2, 3, 4 }, 10000, 0, 4),
                new Clothes(91, new List<int>() { 0, 1, 2, 3, 4 }, 10000, 0, 12),
                new Clothes(92, new List<int>() { 0, 1, 2, 3 }, 3750, 0, 4),
                new Clothes(94, new List<int>() { 0 }, 20000, 0, 4),
                new Clothes(97, new List<int>() { 0 }, 5000, 0, 4),
                new Clothes(107, new List<int>() { 0 }, 10000, 0, 4),
                new Clothes(120, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }, 5000, 0, 4),
                new Clothes(148, new List<int>() { 0, 1, 2, 3, 4, 5 }, 10000, 0, 4),
                new Clothes(153, new List<int>() { 0, 1, 2, 3, 4, 5 }, 5000, 0, 4),
                new Clothes(160, new List<int>() { 0 }, 5000, 0, 4),
                new Clothes(163, new List<int>() { 0, 1, 2, 3, 4, 5 }, 10000, 0, 4),
                new Clothes(164, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 10000, 0, 4),
                new Clothes(166, new List<int>() { 0, 1, 2, 3 }, 5000, 0, 4),
                new Clothes(174, new List<int>() { 0, 1, 2, 3 }, 10000, 0, 4),
                new Clothes(183, new List<int>() { 0, 1, 2, 3, 4, 5 }, 5000, 0, 4),
                new Clothes(185, new List<int>() { 0, 1, 2, 3, 4, 5 }, 8000, 0, 12),
                new Clothes(187, new List<int>() { 0, 1, 2, 3 }, 5000, 0, 4),
                new Clothes(191, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 5000, 0, 4),
                new Clothes(193, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 10000, 0, 4),
                new Clothes(194, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 25000, 0, 4),
                new Clothes(133, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, 3750, 0, 4),
                new Clothes(139, new List<int>() { 0, 1, 2 }, 10000, 0, 4),
                new Clothes(10, new List<int>() { 0, 1, 2, 7, 10, 11, 13, 15 }, 5000, 1, 5),
                new Clothes(58, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, 5000, 1, 5),
                new Clothes(99, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 15000, 1, 5),
                new Clothes(143, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, 50000, 1, 5),
                new Clothes(158, new List<int>() { 0, 1, 2, 3 }, 3500, 1, 5),
                new Clothes(176, new List<int>() { 0, 1, 2, 3 }, 5000, 1, 5),
                new Clothes(3, new List<int>() { 0, 1, 2, 3, 4, 10, 11, 12, 13, 14 }, 5000, 2, 12),
                new Clothes(21, new List<int>() { 0, 1, 2, 3, 4, 5 }, 7500, 2, 12),
                new Clothes(37, new List<int>() { 0, 1, 2, 3, 4, 5 }, 10000, 2, 12),
                new Clothes(50, new List<int>() { 0 }, 2500, 2, 12),
                new Clothes(54, new List<int>() { 0, 1, 2, 3 }, 2500, 2, 12),
                new Clothes(55, new List<int>() { 0 }, 2500, 2, 12),
                new Clothes(62, new List<int>() { 0, 1, 2, 3, 4, 5 }, 2500, 2, 5),
                new Clothes(63, new List<int>() { 0, 1, 2, 3, 4, 5 }, 10000, 2 ,5),
                new Clothes(66, new List<int>() { 0, 1, 2, 3 }, 3750, 2, 5),
                new Clothes(69, new List<int>() { 0 }, 5000, 2, 5),
                new Clothes(70, new List<int>() { 0, 1, 2, 3, 4 }, 4000, 2, 5),
                new Clothes(72, new List<int>() { 0 }, 5000, 2, 12),
                new Clothes(78, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 5000, 2, 12),
                new Clothes(80, new List<int>() { 0 }, 7500, 2, 12),
                new Clothes(81, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 5000, 2, 12),
                new Clothes(87, new List<int>() { 0 }, 5000, 2, 12),
                new Clothes(102, new List<int>() { 0 }, 5000, 2, 12),
                new Clothes(103, new List<int>() { 0, 1, 2, 3, 4, 5 }, 5000, 2, 12),
                new Clothes(110, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 5000, 2, 12),
                new Clothes(113, new List<int>() { 0, 1, 2 }, 10000, 2, 12),
                new Clothes(114, new List<int>() { 0, 1, 2 }, 5000, 2, 12),
                new Clothes(115, new List<int>() { 0, 1, 2 }, 3750, 2, 12),
                new Clothes(116, new List<int>() { 0, 1, 2 }, 3750, 2, 12),
                new Clothes(121, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }, 2500, 2, 12),
                new Clothes(122, new List<int>() { 0 }, 5000, 2, 13),
                new Clothes(127, new List<int>() { 0 }, 2500, 2, 12),
                new Clothes(131, new List<int>() { 0, 1, 2 }, 5000, 2, 12),
                new Clothes(135, new List<int>() { 0, 1, 2 }, 5000, 2, 12),
                new Clothes(138, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 5000, 2, 12),
                new Clothes(140, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 5000, 2, 12),
                new Clothes(144, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 7500, 2, 12),
                new Clothes(145, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 5000, 2, 13),
                new Clothes(147, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 10000, 2, 13),
                new Clothes(149, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 5000, 2, 12),
                new Clothes(150, new List<int>() {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 5000, 2, 12),
                new Clothes(151, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 7500, 2, 5),
                new Clothes(165, new List<int>() { 0, 1, 2 }, 5000, 2, 5),
                new Clothes(172, new List<int>() { 0, 1 }, 2500, 2, 12),
                new Clothes(184, new List<int>() { 0, 1 }, 2500, 2, 12),
                new Clothes(186, new List<int>() { 0, 1, 2, 3 }, 2500, 2, 12),
                new Clothes(189, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, 5000, 2, 12),
                new Clothes(190, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 5000, 2, 12),
                new Clothes(202, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 10000, 2, 12),
                new Clothes(205, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25}, 10000, 2, 12),
                new Clothes(206, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, 5000, 2, 12),
                new Clothes(71, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 75000, 2, 12),
                new Clothes(77, new List<int>() { 0 }, 10000, 2, 12),
                new Clothes(106, new List<int>() { 0, 1, 2, 3 }, 5000, 2, 12),
                new Clothes(123, new List<int>() { 0, 2, 3, 4, 1, 5, 6, 7, 8, 9, 10, 11 }, 10000, 2, 12),
                new Clothes(162, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, 10000, 2, 12),
                new Clothes(192, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 10000, 2, 12),
                new Clothes(98, new List<int>() { 0, 1, 2, 3, 4 }, 10000, 2, 12),
            }},
        };
        public static Dictionary<bool, List<Clothes>> Gloves = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){
                new Clothes(4, new List<int>() { 0, 1 }, 20000),
                new Clothes(5, new List<int>() { 0, 1 }, 21000),
                new Clothes(6, new List<int>() { 0, 1 }, 21000),
                new Clothes(7, new List<int>() { 0, 1 }, 21000),
                new Clothes(9, new List<int>() { 0 }, 21000),
                new Clothes(10, new List<int>() { 0, 1 }, 15000),
                new Clothes(13, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 15000),
            }},
            { false, new List<Clothes>(){
                new Clothes(4, new List<int>() { 0, 1 }, 15000),
                new Clothes(5, new List<int>() { 0, 1 }, 17500),
                new Clothes(6, new List<int>() { 0 }, 15000),
                new Clothes(7, new List<int>() { 0, 1 }, 15000),
                new Clothes(8, new List<int>() { 0 }, 12550),
                new Clothes(9, new List<int>() { 0 }, 10000),
                new Clothes(10, new List<int>() { 0, 1 }, 55000),
                new Clothes(12, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7,8,9 }, 40000),
            }},
        };
        public static Dictionary<bool, List<Clothes>> Accessories = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){
                new Clothes(0, new List<int>() { 0 }, 75000),
                new Clothes(1, new List<int>() { 0 }, 50000),
                new Clothes(3, new List<int>() { 0, 1, 2, 3 }, 40000),
                new Clothes(4, new List<int>() { 0, 1, 2, 3 }, 30000),
                new Clothes(5, new List<int>() { 0, 1, 2, 3 }, 60000),
                new Clothes(6, new List<int>() { 0, 1, 2 }, 140000),
                new Clothes(7, new List<int>() { 0, 1, 2 }, 250000),
                new Clothes(8, new List<int>() { 0, 1, 2 }, 140000),
                new Clothes(9, new List<int>() { 0, 1, 2 }, 350000),
                new Clothes(10, new List<int>() { 0, 1, 2 }, 150000),
                new Clothes(11, new List<int>() { 0, 1, 2 }, 200000),
                new Clothes(12, new List<int>() { 0, 1, 2 }, 30000),
                new Clothes(13, new List<int>() { 0, 1, 2 }, 30000),
                new Clothes(14, new List<int>() { 0, 1, 2 }, 30000),
                new Clothes(15, new List<int>() { 0, 1, 2 }, 200000),
                new Clothes(16, new List<int>() { 0, 1, 2 }, 150000),
                new Clothes(17, new List<int>() { 0, 1, 2 }, 150000),
                new Clothes(18, new List<int>() { 0, 1, 2 }, 45000),
                new Clothes(19, new List<int>() { 0, 1, 2 }, 550000),
                new Clothes(20, new List<int>() { 0, 1, 2 }, 350000),
                new Clothes(21, new List<int>() { 0, 1, 2 }, 35000),
                new Clothes(22, new List<int>() { 0 }, 30000),
                new Clothes(23, new List<int>() { 0 }, 30000),
                new Clothes(24, new List<int>() { 0 }, 40000),
                new Clothes(25, new List<int>() { 0 }, 45000),
                new Clothes(26, new List<int>() { 0 }, 35000),
                new Clothes(27, new List<int>() { 0 }, 30000),
                new Clothes(28, new List<int>() { 0 }, 15000),
                new Clothes(29, new List<int>() { 0, 1, 2, 3 }, 15000),
            }},
            { false, new List<Clothes>(){
                new Clothes(2, new List<int>() { 0, 1, 2, 3 }, 400000),
                new Clothes(3, new List<int>() { 0, 1, 2 }, 600000),
                new Clothes(4, new List<int>() { 0, 1, 2 }, 650000),
                new Clothes(5, new List<int>() { 0, 1, 2 }, 400000),
                new Clothes(6, new List<int>() { 0, 1, 2 }, 350000),
                new Clothes(7, new List<int>() { 0, 1, 2 }, 450000),
                new Clothes(8, new List<int>() { 0, 1, 2 }, 350000),
                new Clothes(9, new List<int>() { 0, 1, 2 }, 400000),
                new Clothes(10, new List<int>() { 0, 1, 2 }, 150000),
                new Clothes(11, new List<int>() { 0 }, 30000),
                new Clothes(12, new List<int>() { 0 }, 30000),
                new Clothes(13, new List<int>() { 0 }, 50000),
                new Clothes(14, new List<int>() { 0 }, 40000),
                new Clothes(15, new List<int>() { 0 }, 75000),
                new Clothes(16, new List<int>() { 0 }, 75000),
                new Clothes(17, new List<int>() { 0 }, 65000),
                new Clothes(18, new List<int>() { 0, 1, 2, 3 }, 75000),

            }},
        };
        public static Dictionary<bool, List<Clothes>> Glasses = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){
                new Clothes(1, new List<int>() { 1 }, 3750),
                new Clothes(2, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 10000),
                new Clothes(3, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 13500),
                new Clothes(4, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 15000),
                new Clothes(5, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 45000),
                new Clothes(7, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 13500),
                new Clothes(8, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 30000),
                new Clothes(9, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 37500),
                new Clothes(10, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 46000),
                new Clothes(12, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 50000),
                new Clothes(13, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 35000),
                new Clothes(15, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 37500),
                new Clothes(16, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 15000),
                new Clothes(17, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 52500),
                new Clothes(18, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 60000),
                new Clothes(19, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 46500),
                new Clothes(20, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 37500),
                new Clothes(23, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 30000),
            }},
            { false, new List<Clothes>(){
                new Clothes(0, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 3750),
                new Clothes(1, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 10000),
                new Clothes(2, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 14000),
                new Clothes(3, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 15000),
                new Clothes(4, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 30000),
                new Clothes(6, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 21000),
                new Clothes(7, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 31000),
                new Clothes(8, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 37500),
                new Clothes(9, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 45000),
                new Clothes(10, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 22500),
                new Clothes(11, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 37500),
                new Clothes(14, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 45000),
                new Clothes(16, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 30000),
                new Clothes(17, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 48000),
                new Clothes(18, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 30000),
                new Clothes(19, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 30000),
                new Clothes(20, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 22500),
                new Clothes(21, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }, 37500),
                new Clothes(24, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 37500),
                new Clothes(25, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 37500),
            }},
        };
        public static Dictionary<bool, List<Clothes>> Jewerly = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){
                new Clothes(17, new List<int>() { 0,1,2 }, 75000),
                new Clothes(49, new List<int>() { 0,1 }, 105000),
                new Clothes(50, new List<int>() { 0,1 }, 15000),
                new Clothes(51, new List<int>() { 0 }, 10000),
                new Clothes(52, new List<int>() { 0,1 }, 15000),
                new Clothes(53, new List<int>() { 0,1 }, 15500),
                new Clothes(54, new List<int>() { 0,1 }, 17000),
                new Clothes(55, new List<int>() { 0,1 }, 24000),
                new Clothes(85, new List<int>() { 0,1 }, 105000),
                new Clothes(86, new List<int>() { 0,1 }, 105000),
                new Clothes(87, new List<int>() { 0,1 }, 108000),
                new Clothes(88, new List<int>() { 0,1 }, 90000),
                new Clothes(89, new List<int>() { 0,1 }, 110000),
                new Clothes(90, new List<int>() { 0,1 }, 125000),
                new Clothes(91, new List<int>() { 0,1 }, 130000),
                new Clothes(92, new List<int>() { 0,1 }, 122500),
                new Clothes(93, new List<int>() { 0,1 }, 122500),
                new Clothes(94, new List<int>() { 0,1 }, 130000),
                new Clothes(111, new List<int>() { 0,1 }, 195000),
                new Clothes(123, new List<int>() { 0,1 }, 225000),
                new Clothes(120, new List<int>() { 0,1 }, 180000),
                new Clothes(122, new List<int>() { 0,1 }, 165000),
            }},
            { false, new List<Clothes>(){
                new Clothes(6, new List<int>() { 0,1,2,3,4,5, }, 150000),
                new Clothes(7, new List<int>() { 0,1 }, 140000),
                new Clothes(11, new List<int>() { 0,1,2,3 }, 150000),
                new Clothes(12, new List<int>() { 0,1,2 }, 75000),
                new Clothes(36, new List<int>() { 0,1 }, 165000),
                new Clothes(37, new List<int>() { 0,1 }, 180000),
                new Clothes(38, new List<int>() { 0 }, 180000),
                new Clothes(39, new List<int>() { 0,1 }, 195000),
                new Clothes(40, new List<int>() { 0,1 }, 180000),
                new Clothes(41, new List<int>() { 0,1 }, 180000),
                new Clothes(42, new List<int>() { 0,1 }, 190000),
                new Clothes(64, new List<int>() { 0,1 }, 205000),
                new Clothes(65, new List<int>() { 0,1 }, 110000),
                new Clothes(66, new List<int>() { 0,1 }, 115000),
                new Clothes(67, new List<int>() { 0,1 }, 125000),
                new Clothes(68, new List<int>() { 0,1 }, 114000),
                new Clothes(69, new List<int>() { 0,1 }, 125000),
                new Clothes(70, new List<int>() { 0,1 }, 125000),
                new Clothes(71, new List<int>() { 0,1 }, 105000),
                new Clothes(72, new List<int>() { 0,1 }, 105000),
                new Clothes(73, new List<int>() { 0,1 }, 125000),
                new Clothes(82, new List<int>() { 0,1 }, 180000),
                new Clothes(90, new List<int>() { 0,1 }, 180000),
                new Clothes(92, new List<int>() { 0,1 }, 190000),
                new Clothes(93, new List<int>() { 0,1 }, 190000),
            }},
        };
        public static List<Clothes> Masks = new List<Clothes>()
        {
            new Clothes(111, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25 }, 17500),
            new Clothes(51, new List<int>() { 0,1,2,3,4,5,6,7,8,9 }, 17500),
            new Clothes(54, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10 }, 20000),
            new Clothes(118, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25 }, 17500),
            new Clothes(119, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24 }, 18500),
            new Clothes(57, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21 }, 18500),
            new Clothes(58, new List<int>() { 0,1,2,3,4,5,6,7,8,9 }, 20000),
            new Clothes(117, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20 }, 18500),
            new Clothes(52, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10 }, 20000),
            new Clothes(53, new List<int>() { 0,1,2,3,4,5,6,7,8 }, 20000),
            new Clothes(113, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21 }, 22500),
        };

        public static Dictionary<int, Tuple<bool, bool, bool>> MaskTypes = new Dictionary<int, Tuple<bool, bool, bool>>()
        {
            { 111, new Tuple<bool, bool, bool>(false, false, false) },
            { 51, new Tuple<bool, bool, bool>(false, false, false) },
            { 54, new Tuple<bool, bool, bool>(true, true, true) },
            { 118, new Tuple<bool, bool, bool>(true, true, true) },
            { 119, new Tuple<bool, bool, bool>(true, true, true) },
            { 57, new Tuple<bool, bool, bool>(true, true, true) },
            { 58, new Tuple<bool, bool, bool>(true, true, true) },
            { 117, new Tuple<bool, bool, bool>(true, true, true) },
            { 52, new Tuple<bool, bool, bool>(true, true, false) },
            { 53, new Tuple<bool, bool, bool>(true, true, false) },
            { 113, new Tuple<bool, bool, bool>(true, true, false) },
        };

        public static Dictionary<bool, Dictionary<int, int>> AccessoryRHand = new Dictionary<bool, Dictionary<int, int>>()
        {
            { true, new Dictionary<int, int>(){
                { 22, 0 },
                { 23, 1 },
                { 24, 2 },
                { 25, 3 },
                { 26, 4 },
                { 27, 5 },
                { 28, 6 },
                { 29, 7 },
            }},
            { false, new Dictionary<int, int>(){
                { 11, 7 },
                { 12, 8 },
                { 13, 9 },
                { 14, 10 },
                { 15, 11 },
                { 16, 12 },
                { 17, 13 },
                { 18, 14 },
            }},
        };

        public static Vector3 CreatorCharPos = new Vector3(402.8664, -996.4108, -99.00027);
        public static Vector3 CreatorPos = new Vector3(402.8664, -997.5515, -98.5);
        public static Vector3 CameraLookAtPos = new Vector3(402.8664, -996.4108, -98.5);
        public static float FacingAngle = -185.0f;
        public static int DimensionID = 1;

        #region Methods
        public static void ApplyCorrectArmor(Player player)
        {
            /*try {
                var armorItem = nInventory.Items[Main.Players[player].UUID].Find(i => i.Type == ItemType.BodyArmor && i.IsActive);
                if (armorItem != null)
                {
                    var gender = Main.Players[player].Gender;

                    string data = armorItem.Data;
                    var split = data.Split('_');

                    var variation = Convert.ToInt32(split[1]);
                    var texture = Convert.ToInt32(split[2]);

                    if (split.Length < 3)
                    {
                        variation = 0;
                        texture = 0;
                    }
                    else
                    {
                        variation = Convert.ToInt32(split[1]);
                        texture = Convert.ToInt32(split[2]);
                    }

                    int variationCycle = (gender) ? 16 : 18;

                    if (!player.HasData("ARMOR_TYPE") || NAPI.Data.GetEntityData(player, "ARMOR_TYPE") == -1)
                    {
                        var custom = CustomPlayerData[Main.Players[player].UUID];
                        var clothes = custom.Clothes;

                        if (clothes.Top.Variation != EmtptySlots[gender][11])
                        {
                            Log.Write("4");
                            var top = Tops[gender].Find(t => t.Variation == clothes.Top.Variation);

                            int type = -2;

                            if (top == null) {
                                Underwear foundUnderwear = Underwears[gender].First(pair => pair.Value.Top == clothes.Top.Variation).Value;

                                if (foundUnderwear != null) {
                                    type = foundUnderwear.BodyArmor;
                                }
                            } else {
                                type = top.BodyArmor;
                            }

                            if (type != -2) {
                                player.SetClothes(9, (variation * variationCycle) + type, texture);
                            }
                        } else {
                            player.SetClothes(9, (variation * variationCycle) + 1, texture);
                        }
                    } else {
                        int type = NAPI.Data.GetEntityData(player, "ARMOR_TYPE");
                        player.SetClothes(9, (variation * variationCycle) + type, texture);
                    }
                }
            } catch(Exception e) {
                Console.WriteLine(e);
            }*/
        }
        public static void ApplyCorrectTorso(Player player)
        {
            var custom = CustomPlayerData[Main.Players[player].UUID];
            var gender = Main.Players[player].Gender;
            var clothes = custom.Clothes;

            int torsoV, torsoT = 0;
            var noneGloves = (CorrectUndershitTorso[gender].ContainsKey(clothes.Undershit.Variation)) ? CorrectUndershitTorso[gender][clothes.Undershit.Variation] : CorrectTorso[gender][clothes.Top.Variation];
            if (clothes.Gloves.Variation == 0)
                torsoV = noneGloves;
            else
            {
                torsoV = CorrectGloves[gender][clothes.Gloves.Variation][noneGloves];
                torsoT = clothes.Gloves.Texture;
            }

            player.SetClothes(3, torsoV, torsoT);
        }
        public static bool ApplyCharacter(Player player)
        {
            if (!Main.Players.ContainsKey(player)) {
                return false;
            }

            if (!CustomPlayerData.ContainsKey(Main.Players[player].UUID)) {
                return false;
            }

            var custom = CustomPlayerData[Main.Players[player].UUID];

            var gender = Main.Players[player].Gender;
            player.SetSharedData("GENDER", gender);

            var clothes = custom.Clothes;
            for (int i = 0; i <= 8; i++) {
                Customization.SetProp(player, i, -1, 0);
            }

            if (!MaskTypes.ContainsKey(clothes.Mask.Variation) || !MaskTypes[clothes.Mask.Variation].Item1) {
                player.SetClothes(2, custom.Hair.Hair, 0);

                NAPI.Player.SetPlayerHairColor(player, (byte)custom.Hair.Color, (byte)custom.Hair.HighlightColor);
            }

            ApplyCorrectTorso(player);
            player.SetClothes(4, clothes.Leg.Variation, clothes.Leg.Texture);
            player.SetClothes(5, clothes.Bag.Variation, clothes.Bag.Texture);
            player.SetClothes(6, clothes.Feet.Variation, clothes.Feet.Texture);
            player.SetClothes(7, clothes.Accessory.Variation, clothes.Accessory.Texture);
            player.SetClothes(8, clothes.Undershit.Variation, clothes.Undershit.Texture);
            player.SetData("ARMOR_TYPE", -1);
            ApplyCorrectArmor(player);
            player.SetClothes(10, clothes.Decals.Variation, clothes.Decals.Texture);
            player.SetClothes(11, clothes.Top.Variation, clothes.Top.Texture);
            // loading tattoos
            foreach (var list in custom.Tattoos.Values)
            {
                foreach (var t in list)
                {
                    if (t == null) continue;
                    var decoration = new Decoration();
                    decoration.Collection = NAPI.Util.GetHashKey(t.Dictionary);
                    decoration.Overlay = NAPI.Util.GetHashKey(t.Hash);
                    player.SetDecoration(decoration);
                }
            }

            player.SetSharedData("TATTOOS", JsonConvert.SerializeObject(custom.Tattoos));

            var accesory = custom.Accessory;
            SetProp(player, 0, accesory.Hat.Variation, accesory.Hat.Texture);

            if (accesory.Glasses.Variation != -1 && !player.HasData("HEAD_POCKET")) {
                SetProp(player, 1, accesory.Glasses.Variation, accesory.Glasses.Texture);
            }

            if (accesory.Ear.Variation != -1) {
                SetProp(player, 2, accesory.Ear.Variation, accesory.Ear.Texture);
            }

            if (accesory.Watches.Variation != -1) {
                SetProp(player, 6, accesory.Watches.Variation, accesory.Watches.Texture);
            }

            if (accesory.Bracelets.Variation != -1) {
                SetProp(player, 7, accesory.Bracelets.Variation, accesory.Bracelets.Texture);
            }

            ApplyCharacterFace(player);

            if (!player.HasData("HEAD_POCKET") && clothes.Mask.Variation != 0) {
                SetMask(player, clothes.Mask.Variation, clothes.Mask.Texture);
            }

            return true;
        }

        public static void ApplyCharacterFace(Player player)
        {
            var custom = CustomPlayerData[Main.Players[player].UUID];

            var parents = custom.Parents;
            var headBlend = new HeadBlend();
            headBlend.ShapeFirst = (byte)parents.Mother;
            headBlend.ShapeSecond = (byte)parents.Father;
            headBlend.ShapeThird = 0;

            headBlend.SkinFirst = (byte)parents.Mother;
            headBlend.SkinSecond = (byte)parents.Father;
            headBlend.SkinThird = 0;

            headBlend.ShapeMix = parents.Similarity;
            headBlend.SkinMix = parents.SkinSimilarity;
            headBlend.ThirdMix = 0.0f;

            NAPI.Player.SetPlayerHeadBlend(player, headBlend);
            for (int i = 0; i < custom.Features.Count(); i++) NAPI.Player.SetPlayerFaceFeature(player, i, custom.Features[i]);
            for (int i = 0; i < custom.Appearance.Count(); i++)
            {
                var headOverlay = new HeadOverlay();
                headOverlay.Index = (byte)custom.Appearance[i].Value;
                headOverlay.Opacity = (byte)custom.Appearance[i].Opacity;
                if (i == 1) headOverlay.Color = (byte)custom.BeardColor;
                else if (i == 2) headOverlay.Color = (byte)custom.EyebrowColor;
                else if (i == 5) headOverlay.Color = (byte)custom.BlushColor;
                else if (i == 8) headOverlay.Color = (byte)custom.LipstickColor;
                else if (i == 10) headOverlay.Color = (byte)custom.ChestHairColor;
                headOverlay.SecondaryColor = 100;
                NAPI.Player.SetPlayerHeadOverlay(player, i, headOverlay);
            }
            NAPI.Player.SetPlayerEyeColor(player, (byte)custom.EyeColor);
        }

        public static void SaveCharacter(Player player)
        {
            if (!CustomPlayerData.ContainsKey(Main.Players[player].UUID)) return;

            var data = CustomPlayerData[Main.Players[player].UUID];
            var Parents = JsonConvert.SerializeObject(data.Parents);
            var Features = JsonConvert.SerializeObject(data.Features);
            var Appearance = JsonConvert.SerializeObject(data.Appearance);
            var Hair = JsonConvert.SerializeObject(data.Hair);
            var Clothes = JsonConvert.SerializeObject(data.Clothes);
            var Accessory = JsonConvert.SerializeObject(data.Accessory);
            var Tattoos = JsonConvert.SerializeObject(data.Tattoos);
            var EyebrowColor = data.EyebrowColor;
            var BeardColor = data.BeardColor;
            var EyeColor = data.EyeColor;
            var BlushColor = data.BlushColor;
            var LipstickColor = data.LipstickColor;
            var ChestHairColor = data.ChestHairColor;
            var IsCreated = data.IsCreated;

            MySqlCommand queryCommand = new MySqlCommand(@"
                UPDATE `customization`
                SET
                    `parents` = @PARENTS,
                    `features` = @FETURES,
                    `appearance` = @APPEARANCE,
                    `hair` = @HAIR,
                    `clothes` = @CLOTHES,
                    `accessory` = @ACCESSORY,
                    `tattoos` = @TATTOOS,
                    `eyebrowc` = @EYEBROWC,
                    `beardc` = @BEARDC,
                    `eyec` = @EYEC,
                    `blushc` = @BLUSHC,
                    `lipstickc` = @LIPSTICKC,
                    `chesthairc` = @CHESTHAIRC,
                    `iscreated` = @ISCREATED
                WHERE
                    `uuid` = @UUID
            ");

            queryCommand.Parameters.AddWithValue("@PARENTS", Parents);
            queryCommand.Parameters.AddWithValue("@FETURES", Features);
            queryCommand.Parameters.AddWithValue("@APPEARANCE", Appearance);
            queryCommand.Parameters.AddWithValue("@HAIR", Hair);
            queryCommand.Parameters.AddWithValue("@CLOTHES", Clothes);
            queryCommand.Parameters.AddWithValue("@ACCESSORY", Accessory);
            queryCommand.Parameters.AddWithValue("@TATTOOS", Tattoos);
            queryCommand.Parameters.AddWithValue("@EYEBROWC", EyebrowColor);
            queryCommand.Parameters.AddWithValue("@BEARDC", BeardColor);
            queryCommand.Parameters.AddWithValue("@EYEC", EyeColor);
            queryCommand.Parameters.AddWithValue("@BLUSHC", BlushColor);
            queryCommand.Parameters.AddWithValue("@LIPSTICKC", LipstickColor);
            queryCommand.Parameters.AddWithValue("@CHESTHAIRC", ChestHairColor);
            queryCommand.Parameters.AddWithValue("@ISCREATED", IsCreated);
            queryCommand.Parameters.AddWithValue("@UUID", Main.Players[player].UUID);

            MySQL.Query(queryCommand);
        }

        public static void LoadCharacter(Player player)
        {
            if (CustomPlayerData.ContainsKey(Main.Players[player].UUID)) return;

            MySqlCommand queryCommand = new MySqlCommand(@"
                SELECT * FROM `customization` WHERE `uuid` = @UUID
            ");

            queryCommand.Parameters.AddWithValue("@UUID", Main.Players[player].UUID);

            DataTable result = MySQL.QueryResult(queryCommand);

            if (result == null)
            {
                Log.Write("DB return null result.", nLog.Type.Warn);
                return;
            }

            foreach (DataRow Row in result.Rows)
            {
                var uuid = Convert.ToInt32(Row["uuid"]);
                CustomPlayerData.Add(uuid, new PlayerCustomization());

                CustomPlayerData[uuid].Parents = JsonConvert.DeserializeObject<ParentData>(Row["parents"].ToString());
                CustomPlayerData[uuid].Features = JsonConvert.DeserializeObject<float[]>(Row["features"].ToString());
                CustomPlayerData[uuid].Appearance = JsonConvert.DeserializeObject<AppearanceItem[]>(Row["appearance"].ToString());
                CustomPlayerData[uuid].Hair = JsonConvert.DeserializeObject<HairData>(Row["hair"].ToString());
                CustomPlayerData[uuid].Clothes = JsonConvert.DeserializeObject<ClothesData>(Row["clothes"].ToString());
                CustomPlayerData[uuid].Accessory = JsonConvert.DeserializeObject<AccessoryData>(Row["accessory"].ToString());
                CustomPlayerData[uuid].Tattoos = JsonConvert.DeserializeObject<Dictionary<int, List<Tattoo>>>(Row["tattoos"].ToString());
                CustomPlayerData[uuid].EyebrowColor = Convert.ToInt32(Row["eyebrowc"]);
                CustomPlayerData[uuid].BeardColor = Convert.ToInt32(Row["beardc"]);
                CustomPlayerData[uuid].EyeColor = Convert.ToInt32(Row["eyec"]);
                CustomPlayerData[uuid].BlushColor = Convert.ToInt32(Row["blushc"]);
                CustomPlayerData[uuid].LipstickColor = Convert.ToInt32(Row["lipstickc"]);
                CustomPlayerData[uuid].ChestHairColor = Convert.ToInt32(Row["chesthairc"]);
                CustomPlayerData[uuid].IsCreated = Convert.ToBoolean(Row["iscreated"]);

                CustomPlayerData[uuid].Clothes.Bag.Variation = 0;
            }
        }

        public static Dictionary<string, object> CreateCharacter(Player player)
        {
            Dictionary<string, object> createCharacterResponse = new Dictionary<string, object>();

            if (!CustomPlayerData.ContainsKey(Main.Players[player].UUID))
            {
                CustomPlayerData.Add(Main.Players[player].UUID, new PlayerCustomization());

                // var data = CustomPlayerData[Main.Players[player].UUID];
                // var Parents = JsonConvert.SerializeObject(data.Parents);
                // var Features = JsonConvert.SerializeObject(data.Features);
                // var Appearance = JsonConvert.SerializeObject(data.Appearance);
                // var Hair = JsonConvert.SerializeObject(data.Hair);
                // var Clothes = JsonConvert.SerializeObject(data.Clothes);
                // var Accessory = JsonConvert.SerializeObject(data.Accessory);
                // var Tattoos = JsonConvert.SerializeObject(data.Tattoos);
                // var EyebrowColor = data.EyebrowColor;
                // var BeardColor = data.BeardColor;
                // var EyeColor = data.EyeColor;
                // var BlushColor = data.BlushColor;
                // var LipstickColor = data.LipstickColor;
                // var ChestHairColor = data.ChestHairColor;
                // var IsCreated = data.IsCreated;

                // MySqlCommand queryCommand = new MySqlCommand(@"
                //     INSERT INTO
                //         `customization` (
                //             `uuid`,
                //             `parents`,
                //             `features`,
                //             `appearance`,
                //             `hair`,
                //             `clothes`,
                //             `accessory`,
                //             `tattoos`,
                //             `eyebrowc`,
                //             `beardc`,
                //             `eyec`,
                //             `blushc`,
                //             `lipstickc`,
                //             `chesthairc`,
                //             `iscreated`
                //         )
                //     VALUES
                //         (
                //             @UUID,
                //             @PARENTS,
                //             @FEATURES,
                //             @APPEARANCE,
                //             @HAIR,
                //             @CLOTHES,
                //             @ACCESSORY,
                //             @TATTOOS,
                //             @EYEBROWC,
                //             @BEARDC,
                //             @EYEC,
                //             @BLUSHC,
                //             @LIPSTICKC,
                //             @CHESTHAIRC,
                //             @ISCREATED
                //         )
                // ");

                // queryCommand.Parameters.AddWithValue("@UUID", Main.Players[player].UUID);
                // queryCommand.Parameters.AddWithValue("@PARENTS", Parents);
                // queryCommand.Parameters.AddWithValue("@FEATURES", Features);
                // queryCommand.Parameters.AddWithValue("@APPEARANCE", Appearance);
                // queryCommand.Parameters.AddWithValue("@HAIR", Hair);
                // queryCommand.Parameters.AddWithValue("@CLOTHES", Clothes);
                // queryCommand.Parameters.AddWithValue("@ACCESSORY", Accessory);
                // queryCommand.Parameters.AddWithValue("@TATTOOS", Tattoos);
                // queryCommand.Parameters.AddWithValue("@EYEBROWC", EyebrowColor);
                // queryCommand.Parameters.AddWithValue("@BEARDC", BeardColor);
                // queryCommand.Parameters.AddWithValue("@EYEC", EyeColor);
                // queryCommand.Parameters.AddWithValue("@BLUSHC", BlushColor);
                // queryCommand.Parameters.AddWithValue("@LIPSTICKC", LipstickColor);
                // queryCommand.Parameters.AddWithValue("@CHESTHAIRC", ChestHairColor);
                // queryCommand.Parameters.AddWithValue("@ISCREATED", IsCreated);

                // MySQL.Query(queryCommand);
            }
            SendToCreator(player);
            player.SetData("Creator_PrevPos", player.Position);
            player.SetData("inCreator", true);
            player.SetData("CHANGING_CHARACTER", false);

            //NAPI.Entity.SetEntityDimension(player, Convert.ToUInt32(DimensionID));

            // player.Rotation = new Vector3(0f, 0f, FacingAngle);
            // NAPI.Entity.SetEntityPosition(player, CreatorCharPos);

            createCharacterResponse.Add("heading", FacingAngle);
            createCharacterResponse.Add("position", CreatorCharPos);

            bool gender = Main.Players[player].Gender;

            SetDefaultFeatures(player, gender);

            //DimensionID++;

            return createCharacterResponse;
        }

        public static void SendToCreator(Player player)
        {
            if (player.HasData("inCreator")) {
                return;
            }
            ;
            player.SetData("Creator_PrevPos", player.Position);
            player.SetData("inCreator", true);

            NAPI.Entity.SetEntityDimension(player, Convert.ToUInt32(DimensionID));
            // player.Rotation = new Vector3(0f, 0f, FacingAngle);
            // NAPI.Entity.SetEntityPosition(player, CreatorCharPos);

            var gender = Main.Players[player].Gender;
            
            player.SetData("CHANGING_CHARACTER", true);
            var tattoos = CustomPlayerData[Main.Players[player].UUID].Tattoos;

            CustomPlayerData[Main.Players[player].UUID] = new PlayerCustomization();
            player.SetData("CHANGING_TATTOOS", tattoos);
            SetCreatorClothes(player, gender);

            Dictionary<string, object> data = new Dictionary<string, object>();

            data["heading"] = FacingAngle;
            data["position"] = CreatorCharPos;

            Trigger.ClientEvent(player, "characterCreator.client.init", JsonConvert.SerializeObject(data));

            DimensionID++;
        }

        public static Vector3 SendBackToWorld(Player player)
        {
            player.ResetData("inCreator");
            player.ResetData("Creator_PrevPos");

            //Vector3 pos = GlobalCollections.RandomSpawns[Jobs.WorkManager.rnd.Next(0, GlobalCollections.RandomSpawns.Count - 1)] + new Vector3(Jobs.WorkManager.rnd.Next(-5, 5), Jobs.WorkManager.rnd.Next(-5, 5), 0);
            Vector3 pos = GlobalCollections.RandomSpawns[0];
            NAPI.Entity.SetEntityDimension(player, 0);
            // NAPI.Entity.SetEntityPosition(player, pos);

            player.SetSkin((Main.Players[player].Gender) ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01);

            Main.Players[player].IsSpawned = true;
            Main.Players[player].IsAlive = true;

            return pos;
        }

        public static void SetDefaultFeatures(Player player, bool gender, bool reset = false)
        {
            if (reset)
            {
                CustomPlayerData[Main.Players[player].UUID] = new PlayerCustomization();

                CustomPlayerData[Main.Players[player].UUID].Parents.Father = 0;
                CustomPlayerData[Main.Players[player].UUID].Parents.Mother = 21;
                CustomPlayerData[Main.Players[player].UUID].Parents.Similarity = (gender) ? 1.0f : 0.0f;
                CustomPlayerData[Main.Players[player].UUID].Parents.SkinSimilarity = (gender) ? 1.0f : 0.0f;
            }

            // will apply the resetted data
            ApplyCharacter(player);

            // clothes
            SetCreatorClothes(player, gender);
        }

        public static void SetCreatorClothes(Player player, bool gender)
        {
            if (!CustomPlayerData.ContainsKey(Main.Players[player].UUID)) {
                return;
            }

            // clothes
            //player.clo();
            for (int i = 0; i < 10; i++) {
                player.ClearAccessory(i);
            }

            if (gender) {
                player.SetClothes(3, 15, 0);
                player.SetClothes(4, 21, 0);
                player.SetClothes(6, 34, 0);
                player.SetClothes(8, 15, 0);
                player.SetClothes(11, 15, 0);
            } else {
                player.SetClothes(3, 15, 0);
                player.SetClothes(4, 10, 0);
                player.SetClothes(6, 35, 0);
                player.SetClothes(8, 15, 0);
                player.SetClothes(11, 15, 0);
            }

            player.SetClothes(2, CustomPlayerData[Main.Players[player].UUID].Hair.Hair, 0);
        }

        public static void ClearClothes(Player player, bool gender)
        {
            if (gender)
            {
                player.SetClothes(3, 15, 0);
                player.SetClothes(4, 21, 0);
                player.SetClothes(6, 34, 0);
                player.SetClothes(8, 15, 0);
                player.SetClothes(11, 15, 0);
            }
            else
            {
                player.SetClothes(3, 15, 0);
                player.SetClothes(4, 15, 0);
                player.SetClothes(6, 35, 0);
                player.SetClothes(8, 15, 0);
                player.SetClothes(11, 15, 0);
            }
            if (!player.HasData("HEAD_POCKET"))
            {
                player.SetClothes(1, 0, 0);
                SetProp(player, 0, -1, 0);
                for (int i = 0; i <= 3; i++) player.ClearAccessory(i);
            }
        }

        /*public static void AddClothes(Player player, ItemType type, int variation, int texture, bool isActive = false)
        {
            if (!Main.Players.ContainsKey(player)) return;
            if (!nInventory.Items.ContainsKey(Main.Players[player].UUID)) return;

            var item = new nItem(type, 1, $"{variation}_{texture}_{Main.Players[player].Gender}", isActive);
            nInventory.Add(player, item);
        }*/

            public static void SetMask(Player player, int variation, int texture)
        {
            if (variation == 0)
            {
                player.SetData("IS_MASK", false);
                ApplyCharacterFace(player);

                player.SetClothes(2, CustomPlayerData[Main.Players[player].UUID].Hair.Hair, 0);
                NAPI.Player.SetPlayerHairColor(player, (byte)CustomPlayerData[Main.Players[player].UUID].Hair.Color, (byte)CustomPlayerData[Main.Players[player].UUID].Hair.HighlightColor);
            }
            else
            {
                player.SetData("IS_MASK", true);
                ApplyMaskFace(player);
            }
            player.SetClothes(1, variation, texture);
        }
       
        public static void ApplyMaskFace(Player player)
        {
            var parents = CustomPlayerData[Main.Players[player].UUID].Parents;
            var headBlend = new HeadBlend();
            headBlend.ShapeFirst = (byte)parents.Mother;
            headBlend.ShapeSecond = (byte)parents.Father;
            headBlend.ShapeThird = 0;

            headBlend.SkinFirst = (byte)parents.Mother;
            headBlend.SkinSecond = (byte)parents.Father;
            headBlend.SkinThird = 0;

            headBlend.ShapeMix = 0.0f;
            headBlend.SkinMix = parents.SkinSimilarity;
            headBlend.ThirdMix = 0.0f;

            NAPI.Player.SetPlayerHeadBlend(player, headBlend);

            NAPI.Player.SetPlayerFaceFeature(player, 0, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 2, 1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 9, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 10, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 13, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 14, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 15, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 16, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 17, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 18, 1.5f);

            for (int i = 0; i < CustomPlayerData[Main.Players[player].UUID].Appearance.Count(); i++)
            {
                if (i != 2 && i != 10)
                {
                    var headOverlay = new HeadOverlay();
                    headOverlay.Index = 255;
                    headOverlay.Opacity = 0;
                    headOverlay.SecondaryColor = 100;
                    NAPI.Player.SetPlayerHeadOverlay(player, i, headOverlay);
                }
            }
        }

        public static void SetProp(Player player, int id, int variation, int texture)
        {
            player.SetAccessories(id, variation, texture);
            player.SetSharedData("PROP_" + id, new int[] { variation, texture });
        }
        public static void SetArmorTop(Player player, int variation, int texture, int armor)
        {
            player.SetClothes(11, variation, texture);
            player.SetData("ARMOR_TYPE", armor);
            ApplyCorrectArmor(player);
        }
        #endregion
        
        #region Events

        [RemoteEvent("characterCreator.server.submit")]
        public void ClientEvent_saveCharacter(Player player, params object[] args)
        {
            try
            {
                if (args.Length < 8 || !CustomPlayerData.ContainsKey(Main.Players[player].UUID)) {
                    return;
                }

                //player.SetDefaultClothes();
                // gender
                var gender = (Convert.ToInt32(args[0]) == 0);

                var isChanging = player.GetData<bool>("CHANGING_CHARACTER");
                var genderChanged = true;
                if (isChanging && Main.Players[player].Gender == gender) genderChanged = false;

                Main.Players[player].Gender = gender;
                var skin = (Main.Players[player].Gender) ? "FreemodeMale01" : "FreemodeFemale01";

                Main.Players[player].FirstName = Convert.ToString(args[8]);
                Main.Players[player].LastName = Convert.ToString(args[9]);
                player.Name = Main.Players[player].FirstName + "_" + Main.Players[player].LastName;

                // parents
                CustomPlayerData[Main.Players[player].UUID].Parents.Father = Convert.ToInt32(args[1]);
                CustomPlayerData[Main.Players[player].UUID].Parents.Mother = Convert.ToInt32(args[2]);
                CustomPlayerData[Main.Players[player].UUID].Parents.Similarity = (float)Convert.ToDouble(args[3]);
                CustomPlayerData[Main.Players[player].UUID].Parents.SkinSimilarity = (float)Convert.ToDouble(args[4]);

                // features
                float[] feature_data = JsonConvert.DeserializeObject<float[]>(args[5].ToString());
                CustomPlayerData[Main.Players[player].UUID].Features = feature_data;

                // appearance
                AppearanceItem[] appearance_data = JsonConvert.DeserializeObject<AppearanceItem[]>(args[6].ToString());
                CustomPlayerData[Main.Players[player].UUID].Appearance = appearance_data;

                // hair & colors
                int[] hair_and_color_data = JsonConvert.DeserializeObject<int[]>(args[7].ToString());
                for (int i = 0; i < hair_and_color_data.Length; i++)
                {
                    switch (i)
                    {
                        // Hair
                        case 0:
                            {
                                CustomPlayerData[Main.Players[player].UUID].Hair.Hair = hair_and_color_data[i];
                                break;
                            }

                        // Hair Color
                        case 1:
                            {
                                CustomPlayerData[Main.Players[player].UUID].Hair.Color = hair_and_color_data[i];
                                break;
                            }

                        // Hair Highlight Color
                        case 2:
                            {
                                CustomPlayerData[Main.Players[player].UUID].Hair.HighlightColor = hair_and_color_data[i];
                                break;
                            }

                        // Eyebrow Color
                        case 3:
                            {
                                CustomPlayerData[Main.Players[player].UUID].EyebrowColor = hair_and_color_data[i];
                                break;
                            }

                        // Beard Color
                        case 4:
                            {
                                CustomPlayerData[Main.Players[player].UUID].BeardColor = hair_and_color_data[i];
                                break;
                            }

                        // Eye Color
                        case 5:
                            {
                                CustomPlayerData[Main.Players[player].UUID].EyeColor = hair_and_color_data[i];
                                break;
                            }

                        // Blush Color
                        case 6:
                            {
                                CustomPlayerData[Main.Players[player].UUID].BlushColor = hair_and_color_data[i];
                                break;
                            }

                        // Lipstick Color
                        case 7:
                            {
                                CustomPlayerData[Main.Players[player].UUID].LipstickColor = hair_and_color_data[i];
                                break;
                            }

                        // Chest Hair Color
                        case 8:
                            {
                                CustomPlayerData[Main.Players[player].UUID].ChestHairColor = hair_and_color_data[i];
                                break;
                            }
                    }
                }

                if (!gender)
                {
                    CustomPlayerData[Main.Players[player].UUID].Clothes.Leg = new ComponentItem(10, 0);
                    CustomPlayerData[Main.Players[player].UUID].Clothes.Feet = new ComponentItem(35, 0);
                }

                // clothes

                try
                {
                    if (!genderChanged && isChanging && player.HasData("CHANGING_TATTOOS"))
                        CustomPlayerData[Main.Players[player].UUID].Tattoos = NAPI.Data.GetEntityData(player, "CHANGING_TATTOOS");
                }
                catch { }

                if (genderChanged)
                {
                    /*if (isChanging){
                        nInventory.ClearAllClothes(player);
                    } else {
                        nInventory.GiveStarterItems(player);
                    }*/

                    switch (Main.Players[player].Gender)
                    {
                        case true:
                            /*AddClothes(player, ItemType.Undershit, 1, 1, true);
                            AddClothes(player, ItemType.Leg, 0, 1, true);
                            AddClothes(player, ItemType.Feet, 1, 1, true);*/

                            CustomPlayerData[Main.Players[player].UUID].Clothes.Top = new ComponentItem(1, 1);
                            CustomPlayerData[Main.Players[player].UUID].Clothes.Leg = new ComponentItem(0, 1);
                            CustomPlayerData[Main.Players[player].UUID].Clothes.Feet = new ComponentItem(1, 1);
                            break;
                        case false:
                            /*AddClothes(player, ItemType.Undershit, 1, 1, true);
                            AddClothes(player, ItemType.Leg, 0, 1, true);
                            AddClothes(player, ItemType.Feet, 3, 1, true);*/

                            CustomPlayerData[Main.Players[player].UUID].Clothes.Top = new ComponentItem(5, 1);
                            CustomPlayerData[Main.Players[player].UUID].Clothes.Leg = new ComponentItem(0, 1);
                            CustomPlayerData[Main.Players[player].UUID].Clothes.Feet = new ComponentItem(3, 1);
                            break;
                    }
                }

                CustomPlayerData[Main.Players[player].UUID].IsCreated = true;

                Vector3 position = SendBackToWorld(player);
                ApplyCharacter(player);
                SaveCharacter(player);

                Trigger.ClientEvent(player, "characterCreator.client.submit.response", JsonConvert.SerializeObject(position), isChanging);
                return;
            } catch (Exception e) {
                Log.Write("SaveCharacter: " + e.Message, nLog.Type.Error);
            }
        }
        #endregion
    }
}
