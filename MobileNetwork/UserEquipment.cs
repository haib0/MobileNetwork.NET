using System.Collections;
using System.Timers;

namespace MobileNetwork.NET.MobileNetwork
{

    public class UserEquipmentConfig
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public double SNIRThreshold { get; set; }
        public double Noise { get; set; } // in dBm
        public double Height { get; set; } // todo: users' height can be a range
        public UserEquipmentPosition Position { get; set; }
        public double UpdateInterval { get; set; }
        public bool AutoConnect { get; set; } // if auto connect to bs //TODO: Connection class
    }

    public class UserEquipment
    {
        public UserEquipmentConfig Config { get; set; }
        public List<BaseStation> AllBaseStation { get; set; }
        public Dictionary<BaseStation, double> AllDistance { get; set; }
        public Dictionary<BaseStation, IChannelModel> AllChannelModel { get; set; }
        public BaseStation TheBS { get; set; } // the BS that connected.
        public double TheDistance => AllDistance[TheBS];  // distance from TheBS
        public IChannelModel TheChannelModel => AllChannelModel[TheBS];
        public int TheCarrierID => TheBS.ConnectedUserEquipment[this].ID;

        public UserEquipment(UserEquipmentConfig config, List<BaseStation> baseStations)
        {
            Config = config;
            AllBaseStation = baseStations;
            SetALLChannelModel();
            Update();
        }

        private void SetALLChannelModel()
        {
            AllChannelModel ??= new Dictionary<BaseStation, IChannelModel>();
            foreach (var bs in AllBaseStation)
            {
                AllChannelModel[bs] = new JakesChannelModel(this, bs);
            }
        }


        private void Update()
        {
            // update distances from all bs
            UpdateAllDistance();
            // update connection if set to auto-connect mode
            if (Config.AutoConnect)
            {
                UpdateConnect();
            }
            // update channel status infomation
            UpdateCSI();

            //Console.WriteLine($"----------------------");
            //Console.WriteLine($"{Config.Name}: x={Config.Position.PositionX}\ty={Config.Position.PositionY}");
            //Console.WriteLine($"{TheBS.Config.Name}: x={TheBS.Config.PositionX}\ty={TheBS.Config.PositionY}");
            //Console.WriteLine($"Dis={AllDistance[TheBS]}");
            //Console.WriteLine($"SpectralEfficiency = {SpectralEfficiency}");
            //Console.WriteLine($"----------------------");
        }



        private void UpdateAllDistance()
        {
            AllDistance ??= new Dictionary<BaseStation, double>();
            foreach (var bs in AllBaseStation)
            {
                AllDistance[bs] = Tools.Distance(this, bs);
            }
        }

        /// <summary>
        /// Connect to BS (TheBS) by distance.
        /// </summary>
        private void UpdateConnect()
        {
            var newBS = AllDistance.MinBy(kvp => kvp.Value).Key;
            if (newBS == null) return;
            ConnectTo(newBS);
        }

        public void ConnectTo(BaseStation baseStation)
        {
            if (!AllBaseStation.Contains(baseStation)) return;
            if (baseStation == TheBS) return;
            TheBS?.Disconnect(this);
            TheBS = baseStation;
            TheBS.Connect(this);
        }

        public Dictionary<BaseStation, double> AllRxPower { get; set; } // in dBm
        public double SINR { get; set; }
        public double SpectralEfficiency => Math.Log2(1 + SINR); // bit/s/Hz
        public double DataRate => TheBS.Config.SubcarrierBandwidth * SpectralEfficiency; // bit/s
        private void UpdateCSI()
        {
            if (!TheBS.ConnectedUserEquipment.ContainsKey(this))
            {
                AllRxPower = new Dictionary<BaseStation, double>();
                SINR = 0;
                return;
            }

            AllRxPower = AllChannelModel.ToDictionary(x => x.Key, x => x.Value.RxPower(TheCarrierID));
            var rx = AllRxPower.ToDictionary(x => x.Key, x => Tools.FromDB(x.Value)); // dBm to mW         
            var s = rx[TheBS];
            //Console.WriteLine($"S={s}");
            var i = rx.Values.Sum() - s;
            //Console.WriteLine($"i={i}");
            var n = Tools.FromDB(Config.Noise);
            //Console.WriteLine($"n={n}");
            SINR = s / (i + n);
            //Console.WriteLine($"SINR={SINR}");
        }

        public UserEquipmentStatus Status()
        {
            Update();
            return new UserEquipmentStatus
            {
                Config = Config,
                AllDistance = AllDistance.ToDictionary(x => x.Key.Config.ID, x => x.Value),
                AllRxPower = AllRxPower.ToDictionary(x => x.Key.Config.ID, x => x.Value),
                TheBSID = TheBS.Config.ID,
                TheCarrierID = TheCarrierID,
                TheBSPositionX = TheBS.Config.PositionX,
                TheBSPositionY = TheBS.Config.PositionY,
                TheBSPositionZ = TheBS.Config.Height,
                TheTxFreq = TheBS.Config.Frequency,
                TheTxBand = TheBS.Config.SubcarrierBandwidth,
                TheTxPower = TheBS.ConnectedUserEquipment[this].TxPower,
                TheDistance = TheDistance,
                TheRxPower = AllRxPower[TheBS],
                SINR = SINR,
                SpectralEfficiency = SpectralEfficiency,
                DataRate = DataRate
            };
        }
    }

    public class UserEquipmentStatus
    {
        public DateTime Time => DateTime.Now;
        public UserEquipmentConfig Config { get; set; }
        public Dictionary<int, double> AllDistance { get; set; }
        public Dictionary<int, double> AllRxPower { get; set; } // in dBm
        public int TheBSID { get; set; } // the BS that connected.
        public int TheCarrierID { get; set; }
        public double TheBSPositionX { get; set; }
        public double TheBSPositionY { get; set; }
        public double TheBSPositionZ { get; set; }
        public double TheTxFreq { get; set; }
        public double TheTxBand { get; set; }
        public double TheTxPower { get; set; }
        public double TheDistance { get; set; }  // distance from TheBS
        public double TheRxPower { get; set; }
        public double SINR { get; set; }
        public double SpectralEfficiency { get; set; }// bit/s/Hz
        public double DataRate { get; set; }// bit/s
    }
}
