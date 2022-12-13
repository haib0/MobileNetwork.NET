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
            };
        }

        public static BaseStationConfig BaseStationConfig(int id, string name, double posX, double posY)
        {
            return new BaseStationConfig
            {
                ID = id,
                Name = name,
                Frequency = 3.5e9,
                SubcarrierNum = 16,
                SubcarrierBandwidth = 1e8,
                SubcarrierMaxTxPower = 30,
                Height = 10,
                PositionX = posX,
                PositionY = posY
            };
        }

        public static UserEquipmentConfig UserEquipmentConfig(int id, string name, double posX, double posY)
        {
            var updateInterval = 60;
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
