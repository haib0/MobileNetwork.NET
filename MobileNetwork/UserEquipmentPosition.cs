namespace MobileNetwork.NET.MobileNetwork
{
    public interface UserEquipmentPosition
    {
        public double Velocity { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        void Move(double t);
    }

    public class RandomMove : UserEquipmentPosition
    {
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public double Velocity { get; set; }
        public double BorderX { get; set; }
        public double BorderY { get; set; }
        public RandomMove(double initPositionX, double initPositionY, double velocity, double borderX, double borderY)
        {
            PositionX = initPositionX;
            PositionY = initPositionY;
            Velocity = velocity;
            BorderX = borderX;
            BorderY = borderY;            
        }

        public void Move(double t)
        {
            if (t < 0) return;

            Random rand = new Random();
            if (rand.NextSingle() < 0.2) // threshold
            {
                return;
            }

            var dir = rand.Next(360);
            PositionX += Velocity * t * Math.Sin(dir);
            PositionY += Velocity * t * Math.Cos(dir);

            while (PositionX > BorderX)
            {
                PositionX -= BorderX;
            }
            while (PositionY > BorderY)
            {
                PositionY -= BorderY;
            }

            if (PositionX <= 0)
            {
                PositionX = 0;
            }
            if (PositionY <= 0)
            {
                PositionY = 0;
            }
        }
    }
}
