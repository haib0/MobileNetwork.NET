using Microsoft.VisualBasic;

namespace MobileNetwork.NET.MobileNetwork
{
    public static class Default
    {
        public static double BorderX = 100;
        public static double BorderY = 100;
        public static MobileNetworkConfig MobileNetworkConfig(string name, int nBS, int nUE)
        {
            return new MobileNetworkConfig
            {
                Name = name,
                Seed = 214,
                BorderX = BorderX,
                BorderY = BorderY,
                BaseStationNum = nBS,
                UserEquipmentNum = nUE,
                ChannelNum = 1
            };
        }

        public static BaseStationConfig BaseStationConfig(int id, string name, double posX, double posY)
        {
            return new BaseStationConfig
            {
                ID = id,
                Name = name,
                InitChannelID = 0,
                MaxTxPower = 30,
                InitBandwidth = 9e6,
                InitFrequency = 2.5e9,
                Height = 10,
                PositionX = posX,
                PositionY = posY
            };
        }

        public static UserEquipmentConfig UserEquipmentConfig(int id, string name, double posX, double posY)
        {
            var updateInterval = 0.02;
            var velocity = 1.5;
            return new UserEquipmentConfig
            {
                ID = id,
                Name = name,
                SNIRThreshold = 0,
                Noise = -114, //dBm
                Height = 1.5,
                Position = new RandomMove(posX, posY, updateInterval, velocity, BorderX, BorderY),
                AutoConnect = true,
            }; ;
        }
    }
}
