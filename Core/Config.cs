
public class Config
    {
        public string ServerName { get; set; } = "RP1";
        public string ServerNumber { get; set; } = "1";
        public string DBServer { get; set; } = "185.231.153.63";
        public string DBName { get; set; } = "ragemp";
        public string DBUid { get; set; } = "ragemp";
        public string DBPassword { get; set; } = "8dggn3ODKPcJurqf";
        public bool VoIPEnabled { get; set; } = false;
        public bool RemoteControl { get; set; } = false;
        public bool DonateChecker { get; set; } = false;
        public bool DonateSaleEnable { get; set; } = false;
        public int PaydayMultiplier { get; set; } = 1;
        public int ExpMultiplier { get; set; } = 1;
        public int DonationPointPrice { get; set; } = 500;

        public string RedisConnect { get; set; } = "127.0.0.1:6379";
        public string LocalRedisConnect { get; set; } = "127.0.0.1:6379";
    }