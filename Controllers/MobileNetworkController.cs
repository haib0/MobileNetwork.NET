using Microsoft.AspNetCore.Mvc;
using MobileNetwork.NET.MobileNetwork;

namespace MobileNetwork.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileNetworkController : ControllerBase
    {
        static readonly MobileNetwork.MobileNetwork mobileNetwork = TheMobileNetwork.mobileNetwork;

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
        [HttpPut("SetTxPower")]
        public void SetTxPower(Dictionary<int, double> txPowers)
        {
            mobileNetwork.SetTxPower(txPowers);
        }

        [HttpPut("SetTxPower/{ueID}")]
        public void SetTxPower(int ueID, double txPower)
        {
            mobileNetwork.SetTxPower(ueID, txPower);
        }

        [HttpPut("SetSubcarrier")]
        public void SetSubcarrier(Dictionary<int, int> channelIDs)
        {
            mobileNetwork.SetSubcarrier(channelIDs);
        }

        [HttpPut("SetSubcarrier/{channelID}")]
        public void SetSubcarrier(int ueID, int channelID)
        {
            mobileNetwork.SetSubcarrier(ueID, channelID);
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

