using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;
using SecondLive.Core;
using SecondLive.Core.Logging;

namespace SecondLive.GUI
{
    class Dashboard :Script
    {
        private static nLog Log = new nLog("Dashboard");
        public static void sendStats(Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                var acc = Main.Players[player];

                string status = "Игрок"
                    /*(acc.AdminLVL >= 1) ? "Администратор" :
                    (Main.Accounts[player].VipLvl > 0) ? $"{Group.GroupNames[Main.Accounts[player].VipLvl]} до {Main.Accounts[player].VipDate.ToString("dd.MM.yyyy")}" :
                    $"{Group.GroupNames[Main.Accounts[player].VipLvl]}"*/;

                //long bank = (acc.Bank != 0) ? Bank.Accounts[acc.Bank].Balance : 0;

                var lic = "";
                for (int i = 0; i < acc.Licenses.Count; i++)
                    if (acc.Licenses[i]) lic += $"{GlobalCollections.LicWords[i]} / ";
                if (lic == "") lic = "Отсутствуют";

                string work = /*(acc.WorkID > 0) ? Jobs.WorkManager.JobStats[acc.WorkID - 1] :*/ "Безработный";
                string fraction = /*(acc.FractionID > 0) ? Fractions.Manager.FractionNames[acc.FractionID] :*/ "Нет";

                var number = (acc.Sim == -1) ? "Нет сим-карты" : Main.Players[player].Sim.ToString();

                List<object> data = new List<object>
                {
                    acc.LVL,
                    $"{acc.EXP}/{3 + acc.LVL * 3}",
                    number,
                    status,
                    0,
                    acc.Warns,
                    lic,
                    acc.CreateDate.ToString("dd.MM.yyyy"),
                    acc.UUID,
                    acc.Bank,
                    work,
                    fraction,
                    acc.FractionLVL,
                };

                string json = JsonConvert.SerializeObject(data);
                Log.Debug("data is: " + json.ToString());
                Trigger.ClientEvent(player, "board", 2, json);

                data.Clear();

            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"DASHBOARD_SENDSTATS\":\n" + e.ToString(), nLog.Type.Error);
            }
        }
    }
}
