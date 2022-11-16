namespace MobileNetwork.NET.MobileNetwork
{

    public static class Tools
    {
        public const double PropagetionSpeed = 299792458;
        public static double Distance(double x1, double y1, double x2, double y2)
        {
            var dx2 = Math.Pow(Math.Abs(x1 - x2), 2);
            var dy2 = Math.Pow(Math.Abs(y1 - y2), 2);
            return Math.Sqrt(dx2 + dy2);
        }

        public static double Distance(UserEquipment ue, BaseStation bs)
        {
            return Distance(
                ue.Config.Position.PositionX,
                ue.Config.Position.PositionY,
                bs.Config.PositionX,
                bs.Config.PositionY
                );
        }

        /// <summary>
        /// Calculate Dopler frequency change
        /// </summary>
        /// <param name="v">relative speed</param>
        /// <param name="f">emitted frequency</param>
        /// <returns>fd</returns>
        public static double DoplerFrequency(double v, double f)
        {
            return f * v / PropagetionSpeed;
        }

        public static double FromDB(double db)
        {
            return Math.Pow(10, db / 10);
        }


        public static double ToDB(double x)
        {
            return 10 * Math.Log10(x);
        }
    }
}
