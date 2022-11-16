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
        public double UpdateInterval { get; set; } // in ms
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

        private System.Timers.Timer? _UETimer;

        public UserEquipment(UserEquipmentConfig config, List<BaseStation> baseStations)
        {
            Config = config;
            AllBaseStation = baseStations;
            SetALLChannelModel();
            UpdateAllDistance();
            SetTimer();
        }

        private void SetALLChannelModel()
        {
            AllChannelModel ??= new Dictionary<BaseStation, IChannelModel>();
            foreach (var bs in AllBaseStation)
            {
                AllChannelModel[bs] = new JakesChannelModel(this, bs);
            }
        }
        private void SetTimer()
        {
            _UETimer = new System.Timers.Timer(Config.UpdateInterval * 1e3);
            _UETimer.Elapsed += OnTimedUpdate;
            _UETimer.AutoReset = true;
            _UETimer.Enabled = true;
        }

        private void OnTimedUpdate(object? source, ElapsedEventArgs e)
        {
            // move
            UpdatePosition();           
            
            // update connection if set to auto-connect mode
            if (Config.AutoConnect)
            {
                UpdateConnect();
            }

            // update channel status infomation
            UpdateCSI();

            Console.WriteLine($"----------{e.SignalTime}------------");
            Console.WriteLine($"{Config.Name}: x={Config.Position.PositionX}\ty={Config.Position.PositionY}");
            Console.WriteLine($"{TheBS.Config.Name}: x={TheBS.Config.PositionX}\ty={TheBS.Config.PositionY}");
            Console.WriteLine($"Dis={AllDistance[TheBS]}");
            Console.WriteLine($"SpectralEfficiency = {SpectralEfficiency}");

            Console.WriteLine($"----------{e.SignalTime}------------");
        }

        private void UpdatePosition()
        {
            Config.Position.Move(Config.UpdateInterval);
            // update distances from all bs
            UpdateAllDistance();
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
            ConnectTo(newBS);
        }

        public void ConnectTo(BaseStation baseStation)
        {
            if (!AllBaseStation.Contains(baseStation)) return;
            if (baseStation == TheBS) return;
            if (TheBS != null)
            {
                TheBS.Disconnect(this);
            }
            TheBS = baseStation;
            TheBS.Connect(this);
        }

        public Dictionary<BaseStation, double> AllRxPower { get; set; } // in dBm

        public double SINR { get; set; }
        public double SpectralEfficiency => Math.Log2(1 + SINR); // bit/s/Hz
        public double DataRate => TheBS.Bandwidth * SpectralEfficiency; // bit/s
        public void UpdateCSI()
        {
            AllRxPower = AllChannelModel.ToDictionary(x => x.Key, x => x.Value.BS.TxPower - x.Value.ChannelLoss());
            var rx = AllRxPower.ToDictionary(x => x.Key, x => Tools.FromDB(x.Value)); // dBm to mW         
            var s = rx[TheBS];
            //Console.WriteLine($"S={s}");
            var i = 0.0;
            var rxi = rx.Where(x => x.Key.ChannelID == TheBS.ChannelID);
            if (rxi.Any()) { i = rx.Values.Sum() - s; }
            //Console.WriteLine($"i={i}");
            var n = Tools.FromDB(Config.Noise);
            //Console.WriteLine($"n={n}");
            SINR = s / (i + n);
            //Console.WriteLine($"SINR={SINR}");
        }
    }

    public class UserEquipmentStatus
    {
        public UserEquipmentConfig Config { get; set; }

    }
}
