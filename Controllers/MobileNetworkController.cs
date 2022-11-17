using Microsoft.AspNetCore.Mvc;
using MobileNetwork.NET.MobileNetwork;

namespace MobileNetwork.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileNetworkController : ControllerBase
    {
        private static readonly MobileNetwork.MobileNetwork mobileNetwork = new(10, 30);

        [HttpGet("AllStatus")]
        public MobileNetworkStatus AllStatus() => mobileNetwork.AllStatus();

        [HttpGet("AllBaseStationStatus")]
        public List<BaseStationStatus> AllBaseStationStatus()
        {
            return mobileNetwork.AllBaseStationStatus();
        }

        [HttpGet("BaseStationStatus")]
        public BaseStationStatus? BaseStationStatus(int id)
        {
            return mobileNetwork.BaseStationStatus(id);
        }

        [HttpGet("AllUserEquipmentStatus")]
        public List<UserEquipmentStatus> AllUserEquipmentStatus()
        {
            return mobileNetwork.AllUserEquipmentStatus();
        }

        [HttpGet("UserEquipmentStatus")]
        public UserEquipmentStatus GetUEStatus(int id)
        {
            return mobileNetwork.AllUE[id].Status();
        }

        // todo: more details of false
        [HttpPut("SetBaseStationTxPower")]
        public void SetBaseStationTxPowers(Dictionary<int, double> txPowers)
        {
            mobileNetwork.SetBaseStationTxPowers(txPowers);
        }

        [HttpPut("SetBaseStationTxPower/{bsID}")]
        public void SetBaseStationTxPower(int bsID, double txPower)
        {
            mobileNetwork.SetBaseStationTxPower(bsID, txPower);
        }

        [HttpPut("SetBaseStationChannelIDs")]
        public void SetBaseStationChannelIDs(Dictionary<int, int>channelIDs)
        {
            mobileNetwork.SetBaseStationChannelIDs(channelIDs);
        }

        [HttpPut("SetBaseStationChannelID/{channelID}")]
        public void SetBaseStationChannelID(int bsID, int channelID)
        {
            mobileNetwork.SetBaseStationChannelID(bsID, channelID);
        }

        [HttpPut("SetUserConnects")]
        public void SetUserConnects(Dictionary<int, int> connects)
        {
            mobileNetwork.SetUserConnects(connects);
        }

        [HttpPut("SetUserConnect")]
        public void SetUserConnect(int ueID, int bsID)
        {
            mobileNetwork.SetUserConnect(ueID, bsID);            
        }
    }
}

