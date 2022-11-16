namespace MobileNetwork.NET.MobileNetwork;

public class MobileNetworkConfig
{
    public string? Name { get; set; }
    public int Seed { get; set; }
    public double BorderX { get; set; }
    public double BorderY { get; set; }
    public int BaseStationNum { get; set; }
    public int UserEquipmentNum { get; set; }
}

public class MobileNetwork
{
    public MobileNetworkConfig Config { get; set; }
    public List<BaseStation> AllBS { get; set; }
    public List<UserEquipment> AllUE { get; set; }
    public MobileNetwork(int nBS, int nUE)
    {
        Config = Default.MobileNetworkConfig("Default", nBS, nUE);
        GenerateEntities();
    }
    public MobileNetwork(MobileNetworkConfig config)
    {
        Config = config;
        GenerateEntities();
    }
    public MobileNetwork(MobileNetworkConfig config, List<BaseStation> allBS, List<UserEquipment> allUE) : this(config)
    {
        Config = config;
        AllBS = allBS;
        AllUE = allUE;
    }

    private void GenerateEntities()
    {
        GenerateAllBS();
        GenerateAllUE();
    }

    private void GenerateAllBS()
    {
        AllBS = new List<BaseStation>();
        var rand = new Random();       
        for (int id = 0; id < Config.BaseStationNum; id++)
        {
            var posX = rand.NextDouble() * Config.BorderX;
            var posY = rand.NextDouble() * Config.BorderY;
            AllBS.Add(new BaseStation(Default.BaseStationConfig(id, $"BS-{id}", posX, posY)));
        }
    }

    private void GenerateAllUE()
    {
        AllUE = new List<UserEquipment>();
        var rand = new Random();
        for (int id = 0; id < Config.UserEquipmentNum; id++)
        {
            var posX = rand.NextDouble() * Config.BorderX;
            var posY = rand.NextDouble() * Config.BorderY;
            AllUE.Add(new UserEquipment(Default.UserEquipmentConfig(id, $"UE-{id}", posX, posY), AllBS));
        }
    }
}
