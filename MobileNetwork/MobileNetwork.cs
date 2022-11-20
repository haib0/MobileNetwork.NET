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
    public double SumDataRate => AllUE.Sum(x => x.DataRate);
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
        //Console.WriteLine("GenerateAllBS");
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

    public MobileNetworkStatus AllStatus()
    {
        return new MobileNetworkStatus
        {
            Config = Config,
            AllBSStatus = AllBS.Select(x => x.Status()).ToList(),
            AllUEStatus = AllUE.Select(x => x.Status()).ToList(),
            SumDataRate = SumDataRate
        };
    }
    public List<BaseStationStatus> AllBaseStationStatus()
    {
        return AllBS.Select(x => x.Status()).ToList();
    }

    public BaseStationStatus? BaseStationStatus(int bsID)
    {
        if (bsID < 0 || bsID >= AllBS.Count) return null;
        return AllBS[bsID].Status();
    }

    public List<UserEquipmentStatus> AllUserEquipmentStatus()
    {
        return AllUE.Select(x => x.Status()).ToList();
    }

    public UserEquipmentStatus? UserEquipmentStatus(int ueID)
    {
        if (ueID < 0 || ueID >= AllUE.Count) return null;
        return AllUE[ueID].Status();
    }

    // todo: more details of false
    public void SetTxPower(Dictionary<int, double> ueTxPowers)
    {
        foreach (var i in ueTxPowers) { SetTxPower(i.Key, i.Value); }
    }
    public void SetTxPower(int ueID, double txPower)
    {
        if (ueID < 0 || ueID >= AllUE.Count) return;
        AllUE[ueID].TheBS.SetTxPower(AllUE[ueID], txPower);
    }

    public void SetSubcarrier(Dictionary<int, int> ueSubcarrier)
    {
        foreach (var i in ueSubcarrier) { SetSubcarrier(i.Key, i.Value); }
    }

    public void SetSubcarrier(int ueID, int carrier)
    {
        if (ueID < 0 || ueID >= AllUE.Count) return;
        AllUE[ueID].TheBS.SetSubcarrier(AllUE[ueID], carrier);
    }

    public void SetUserConnects(Dictionary<int, int> connections)
    {
        foreach (var c in connections) { SetUserConnect(c.Key, c.Value); }
    }

    public void SetUserConnect(int ueID, int bsID)
    {
        if (ueID < 0 || ueID >= AllUE.Count) return ;
        if (bsID < 0 || bsID >= AllBS.Count) return ;
        AllUE[ueID].ConnectTo(AllBS[bsID]);
    }
}

public class MobileNetworkStatus
{
    public DateTime Time => DateTime.Now;
    public MobileNetworkConfig? Config { get; set; }
    public List<BaseStationStatus>? AllBSStatus { get; set; }
    public List<UserEquipmentStatus>? AllUEStatus { get; set; }
    public double SumDataRate { get; set; }
}
