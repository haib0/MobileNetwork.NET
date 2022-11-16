namespace MobileNetwork.NET.MobileNetwork
{
    public class BaseStationConfig
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public int InitChannelID { get; set; }
        public double MaxTxPower { get; set; } // in dBm
        public double InitBandwidth { get; set; } // in Hz
        public double InitFrequency { get; set; } // in Hz        
        public double Height { get; set; } // in m
        public double PositionX { get; set; }
        public double PositionY { get; set; }
    }

    public class BaseStation
    {
        public BaseStationConfig Config { get; set; }
        public double TxPower { get; set; } // transmitter output power in dBm
        public int ChannelID { get; set; }
        public double Bandwidth { get; set; } // in Hz
        public double Frequency { get; set; } // in Hz
        public LinkedList<UserEquipment> ConnectedUserEquipment { get; set; }
        public BaseStation(BaseStationConfig config)
        {
            Config = config;
            TxPower = Config.MaxTxPower;
            ChannelID = Config.InitChannelID; Bandwidth = Config.InitBandwidth; Frequency = Config.InitFrequency;
            ConnectedUserEquipment ??= new LinkedList<UserEquipment>();
        }


        public void SetTxPower(double txPower)
        {
            TxPower = txPower;
        }

        public void SetChannelID(int channelID)
        {
            ChannelID = channelID;
        }

        public void Connect(UserEquipment ue)
        {
            if (ConnectedUserEquipment.Contains(ue))
            {
                return;
            }
            ConnectedUserEquipment.AddLast(ue);
        }

        public void Disconnect(UserEquipment ue)
        {
            if (ConnectedUserEquipment.Contains(ue))
            {
                ConnectedUserEquipment.Remove(ue);
            }
        }
    }

}
