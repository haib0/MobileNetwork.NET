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
        [HttpPut("SetBaseStationTxPower/{bsID}")]
        public bool SetBaseStationTxPower(int bsID, double txPower)
        {
            return mobileNetwork.SetBaseStationTxPower(bsID, txPower);

        }

        [HttpPut("SetBaseStationChannelID/{channelID}")]
        public bool SetBaseStationChannelID(int bsID, int channelID)
        {
            return mobileNetwork.SetBaseStationChannelID(bsID, channelID);
        }

        [HttpPut("UserConnect")]
        public bool UserConnect(int ueID, int bsID)
        {
            return mobileNetwork.UserConnect(ueID, bsID);            
        }
    }
}

