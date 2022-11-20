using MathNet.Numerics;
using MathNet.Numerics.Distributions;

namespace MobileNetwork.NET.MobileNetwork
{
    public interface IChannelModel
    {

        public UserEquipment UE { get; set; }
        public BaseStation BS { get; set; }

        /// <summary>
        /// Calculate channel loss.
        /// </summary>
        /// <param name="ue"></param>
        /// <param name="bs"></param>
        /// <returns>loss, in dB</returns>
        public double ChannelLoss();
        public double RxPower(int carrier);
    }

    public class JakesChannelModel : IChannelModel
    {
        public UserEquipment UE { get; set; }
        public BaseStation BS { get; set; }

        private double _smallScaleLoss;

        /// <summary>
        /// Jakes Channel Model
        /// </summary>
        public JakesChannelModel(UserEquipment ue, BaseStation bs)
        {
            UE = ue;
            BS = bs;
            _smallScaleLoss = Normal.Sample(0, 1);
        }
        public void Test()
        {
            Console.WriteLine($"{ChannelLoss()}");
        }
        public double SNR()
        {
            return Tools.FromDB(BS.TxPower - ChannelLoss() - UE.Config.Noise);
        }
        public double RateWithoutInterference()
        {
            return BS.Config.SubcarrierBandwidth * Math.Log2(1 + SNR());
        }
        public double ChannelLoss()
        {
            return LargeScaleLoss(Tools.Distance(UE, BS)) + Math.Pow(SmallScaleLoss(), 2);
        }

        /// <summary>
        /// Calculate pathloss.
        /// </summary>
        /// <param name="distance">distance between bs and ue, in m</param>
        /// <returns></returns>
        public double Pathloss(double distance)
        {
            return 120.9 + 37.6 * Math.Log10(distance * 1e-3);
        }

        public double LargeScaleLoss(double distance)
        {
            var shadowing = Normal.Sample(0, 1) * 8; // in dB
            return Pathloss(distance) + shadowing;
        }

        public double SmallScaleLoss()
        {
            var fd = Tools.DoplerFrequency(UE.Config.Position.Velocity, BS.Config.Frequency);
            //Console.WriteLine($"fd={fd}");
            var correlation = SpecialFunctions.BesselJ(0, 2 * Math.PI * fd * UE.Config.Position.UpdateInterval);
            //Console.WriteLine($"correlation={correlation}");
            var delta = Normal.Sample(0, 1);
            //Console.WriteLine($"delta={delta}");
            _smallScaleLoss = correlation * _smallScaleLoss + Math.Sqrt(1 - Math.Pow(correlation, 2)) * delta;
            return _smallScaleLoss;
        }

        public double RxPower(int carrier)
        {
            if (!BS.AllSubcarrier.ContainsKey(carrier)) return 0.0;
            return BS.AllSubcarrier[carrier].TxPower - ChannelLoss();
        }
    }
}
