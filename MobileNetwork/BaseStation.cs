namespace MobileNetwork.NET.MobileNetwork
{
    public class BaseStationConfig
    {
        public int ID { get; set; } // basestation id (do not been duplicated)
        public string? Name { get; set; } // basestation name, any one
        public int InitChannelID { get; set; } // initial sub-channel id
        public double MaxTxPower { get; set; } // max transmitter output power, in dBm
        public double InitBandwidth { get; set; } // in Hz
        public double InitFrequency { get; set; } // in Hz        
        public double Height { get; set; } // in m
        public double PositionX { get; set; } // in m
        public double PositionY { get; set; } // in m
    }

    public class BaseStation
    {
        public BaseStationConfig Config { get; set; }
        public double TxPower { get; set; } // transmitter output power in dBm
        public int ChannelID { get; set; } // sub-channel id
        public double Bandwidth { get; set; } // in Hz
        public double Frequency { get; set; } // in Hz
        public LinkedList<UserEquipment> ConnectedUserEquipment { get; set; } // user equipments that connects

        public BaseStation(BaseStationConfig config)
        {
            Config = config;
            TxPower = Config.MaxTxPower;
            ChannelID = Config.InitChannelID; Bandwidth = Config.InitBandwidth; Frequency = Config.InitFrequency;
            ConnectedUserEquipment ??= new LinkedList<UserEquipment>();
        }

        public void SetTxPower(double txPower)
        {
            if (txPower < 0 || txPower > Config.MaxTxPower) return;
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

        public BaseStationStatus Status()
        {
            return new BaseStationStatus
            {
                Config = Config,
                TxPower = TxPower,
                ChannelID = ChannelID,
                Bandwidth = Bandwidth,
                Frequency = Frequency,
                ConnectedUserEquipment = ConnectedUserEquipment.Select(x => x.Config.ID).ToList(),
            };
        }
    }

    public class BaseStationStatus
    {
        public DateTime Time => DateTime.Now;
        public BaseStationConfig? Config { get; set; }
        public double TxPower { get; set; } // transmitter output power in dBm
        public int ChannelID { get; set; }
        public double Bandwidth { get; set; } // in Hz
        public double Frequency { get; set; } // in Hz
        public List<int>? ConnectedUserEquipment { get; set; }
    }
}
