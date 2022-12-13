using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileNetwork.NET.MobileNetwork;

namespace MobileNetwork.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvController : ControllerBase
    {
        static readonly MobileNetwork.MobileNetwork mobileNetwork = TheMobileNetwork.mobileNetwork;
        private static int ObservationNum =  mobileNetwork.Config.UserEquipmentNum* (2 + mobileNetwork.Config.BaseStationNum);
        private static readonly int TxPowerClassNum = 6;
        private static int ueID = 0;

        [HttpGet("EnvInfo")]
        public Dictionary<string, int> EnvInfo()
        {
            return new Dictionary<string, int>
            {
                { "BSNum", mobileNetwork.Config.BaseStationNum},
                { "UENum", mobileNetwork.Config.UserEquipmentNum },
                { "TxPowerClassNum", TxPowerClassNum },
                { "ObservationNum", ObservationNum }
            };
        }

        [HttpGet("ObservationGlobal")]
        public double[] ObservationGlobal()
        {
            int obNum1 = 2 + mobileNetwork.Config.BaseStationNum;
            
            double[] ob = new double[ObservationNum];
            for (int i = 0; i < mobileNetwork.Config.UserEquipmentNum; i++)
            {
                var ue = mobileNetwork.AllUE[i];
                ob[i * obNum1] = ue.TheBS.Config.ID;
                ob[i * obNum1 + 1] = ue.TheCarrierID;
                foreach (var x in ue.AllChannelModel.ToDictionary(x => x.Key.Config.ID, x => x.Value.ChannelLoss()))
                {
                    ob[i * obNum1 + 2 + x.Key] = x.Value;
                }
            }             
            return ob;
        }        

        [HttpGet("ObservationA")]
        public double[] ObservationA()
        {
            ObservationNum = 16 + 2 * mobileNetwork.Config.BaseStationNum;

            double[] ob = new double[ObservationNum];

            var ueStatus = mobileNetwork.UserEquipmentStatus(ueID);
            if (ueStatus == null) { return ob; }

            ob[0] = ueStatus.Config.ID;
            ob[1] = ueStatus.Config.Position.PositionX;
            ob[2] = ueStatus.Config.Position.PositionY;
            ob[3] = ueStatus.Config.Height;
            ob[4] = ueStatus.SINR;
            ob[5] = ueStatus.SpectralEfficiency;
            ob[6] = ueStatus.DataRate;
            ob[7] = ueStatus.TheBSID;
            ob[8] = ueStatus.TheBSPositionX;
            ob[9] = ueStatus.TheBSPositionY;
            ob[10] = ueStatus.TheBSPositionZ;
            ob[11] = ueStatus.TheDistance;
            ob[12] = ueStatus.TheTxFreq;
            ob[13] = ueStatus.TheTxBand;
            ob[14] = ueStatus.TheTxPower;
            ob[15] = ueStatus.TheRxPower;

            var idx = 16;
            for (int i = 0; i < mobileNetwork.Config.BaseStationNum; i++)
            {
                ob[idx] = ueStatus.AllRxPower[i];
                idx++;
            }
            for (int i = 0; i < mobileNetwork.Config.BaseStationNum; i++)
            {
                ob[idx] = ueStatus.AllDistance[i];
                idx++;
            }

            return ob;
        }


        [HttpPost("StepA")]
        public StepReturn StepA(int txPowerClass)
        {
            var maxP = Tools.FromDB(mobileNetwork.AllUE[ueID].TheBS.Config.SubcarrierMaxTxPower);
            double reward = -1;
            if ((txPowerClass >= 0) && (txPowerClass < TxPowerClassNum))
            {
                var txPower = Tools.ToDB(txPowerClass * maxP / (txPowerClass - 1));
                mobileNetwork.SetTxPower(ueID, txPower);
                reward = mobileNetwork.AverageSpectralEfficiency;
            }
            UpdateUEID();
            Console.WriteLine($"Step({txPowerClass}): R={reward}\tNextUE={ueID}");
            return new StepReturn
            {
                Observation = ObservationA(),
                Reward = reward,
                IsDone = false
            };
        }

        private static void UpdateUEID()
        {
            ueID++;
            if (ueID >= mobileNetwork.Config.UserEquipmentNum) ueID = 0;
        }
    }

    public class StepReturn
    {
        public double[]? Observation { get; set; }
        public double Reward { get; set; }
        public bool IsDone { get; set; }
        public MobileNetworkStatus? Info { get; set; }
    }
}
