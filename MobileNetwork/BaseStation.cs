using System.Linq;
using System.Threading.Channels;

namespace MobileNetwork.NET.MobileNetwork
{
    public class BaseStationConfig
    {
        public int ID { get; set; } // basestation id (do not been duplicated)
        public string? Name { get; set; } // basestation name, any one              
        public double Frequency { get; set; } // in Hz
        public int SubcarrierNum { get; set; } // number of sub-channel
        public double SubcarrierBandwidth { get; set; } // in Hz
        public double SubcarrierMaxTxPower { get; set; } // max transmitter output power, in dBm                                                  
        public double Height { get; set; } // in m
        public double PositionX { get; set; } // in m
        public double PositionY { get; set; } // in m
    }

    public class BaseStation
    {
        public BaseStationConfig Config { get; set; }
        public Dictionary<int, Subcarrier> AllSubcarrier { get; set; } // index is subcarrier's id while value is itself
        public Dictionary<UserEquipment, Subcarrier> ConnectedUserEquipment => AllSubcarrier.ToDictionary(x => x.Value.UserEquipment, x => x.Value);
        public double TxPowerSum => Tools.ToDB(AllSubcarrier.Values.Sum(x => Tools.FromDB(x.TxPower))); // transmitter output power in dBm
        public BaseStation(BaseStationConfig config)
        {
            Config = config;
            AllSubcarrier ??= new Dictionary<int, Subcarrier>();
        }

        public void SetTxPower(int carrier, double txPower)
        {
            if (txPower > Config.SubcarrierMaxTxPower) return;
            if (!AllSubcarrier.ContainsKey(carrier)) return;
            AllSubcarrier[carrier].TxPower = txPower;
        }

        public void SetTxPower(UserEquipment ue, double txPower)
        {
            if (txPower > Config.SubcarrierMaxTxPower) return;
            if (!ConnectedUserEquipment.ContainsKey(ue)) return;
            ConnectedUserEquipment[ue].TxPower = txPower;
        }

        public void SetSubcarrier(UserEquipment ue, int carrier)
        {
            if (carrier < 0 || carrier >= Config.SubcarrierNum) return;
            if (!ConnectedUserEquipment.ContainsKey(ue)) return; // not serving ue
            AllSubcarrier[carrier] = new Subcarrier { ID = carrier, TxPower = Config.SubcarrierMaxTxPower, UserEquipment = ue }; // notion: will disconnect user that already using this subcarrier!           
        }

        public void SetSubcarrierSafe(UserEquipment ue, int carrier)
        {
            if (carrier < 0 || carrier >= Config.SubcarrierNum) return;
            if (AllSubcarrier.ContainsKey(carrier)) return; // wont disconnect the user already connected
            if (!ConnectedUserEquipment.ContainsKey(ue)) return; // not serving ue
            AllSubcarrier[carrier] = new Subcarrier { ID = carrier, TxPower = Config.SubcarrierMaxTxPower, UserEquipment = ue };
        }

        public void SetSubcarrierAuto(UserEquipment ue)
        {
            if (!ConnectedUserEquipment.ContainsKey(ue)) return; // not serving ue            
            SetSubcarrierAutoForce(ue);
        }

        private void SetSubcarrierAutoForce(UserEquipment ue)
        {
            for (var carrier = 0; carrier < Config.SubcarrierNum; carrier++)
            {
                if (AllSubcarrier.ContainsKey(carrier)) continue;
                AllSubcarrier[carrier] = new Subcarrier { ID = carrier, TxPower = Config.SubcarrierMaxTxPower, UserEquipment = ue };
                return;
            }
            // all subcarriers are busy
        }

        public void Connect(UserEquipment ue)
        {
            if (ConnectedUserEquipment.ContainsKey(ue)) return;
            SetSubcarrierAutoForce(ue);
        }

        public void Disconnect(UserEquipment ue)
        {
            if (!ConnectedUserEquipment.ContainsKey(ue)) return;
            AllSubcarrier.Remove(ConnectedUserEquipment[ue].ID);
        }

        public BaseStationStatus Status()
        {
            return new BaseStationStatus
            {
                Config = Config,
                TxPowerSum = TxPowerSum,
                ConnectedUserEquipment = ConnectedUserEquipment.ToDictionary(x => x.Key.Config.ID, x => x.Value.ID),
                AllSubcarrier = AllSubcarrier.ToDictionary(x => x.Key, x => x.Value.Status())
            };
        }
    }

    public class BaseStationStatus
    {
        public DateTime Time => DateTime.Now;
        public BaseStationConfig? Config { get; set; }
        public double TxPowerSum { get; set; } // transmitter output power in dBm
        public Dictionary<int, int> ConnectedUserEquipment { get; set; }
        public Dictionary<int, SubcarrierStatus> AllSubcarrier { get; set; }
    }
}
