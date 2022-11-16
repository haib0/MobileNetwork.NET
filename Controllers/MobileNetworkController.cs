using Microsoft.AspNetCore.Mvc;
using MobileNetwork.NET.MobileNetwork;

namespace MobileNetwork.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileNetworkController : ControllerBase
    {
        private readonly MobileNetwork.MobileNetwork mobileNetwork = new MobileNetwork.MobileNetwork(100, 300);

        public static void Test()
        {

        }
    }
}
