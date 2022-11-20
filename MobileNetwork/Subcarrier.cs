namespace MobileNetwork.NET.MobileNetwork
{
    public class Subcarrier
    {
        public int ID { get; set; }
        public double TxPower { get; set; }
        public UserEquipment UserEquipment { get; set; }

        public SubcarrierStatus Status()
        {
            return new SubcarrierStatus
            {
                ID = ID,
                TxPower = TxPower,
                UserEquipment = UserEquipment.Config.ID
            };
        }
    }

    public class SubcarrierStatus
    {
        public int ID { get; set; }
        public double TxPower { get; set; }
        public int UserEquipment { get; set; }
    }
}
