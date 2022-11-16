using System.Timers;

namespace MobileNetwork.NET.MobileNetwork
{
    public interface UserEquipmentPosition
    {
        public double Velocity { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public double UpdateInterval { get; set; }
    }

    public class RandomMove : UserEquipmentPosition
    {
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public double UpdateInterval { get; set; }
        public double Velocity { get; set; }
        public double BorderX { get; set; }
        public double BorderY { get; set; }

        private System.Timers.Timer? _UETimer;

        public RandomMove(double initPositionX, double initPositionY, double updateInterval, double velocity, double borderX, double borderY)
        {
            PositionX = initPositionX;
            PositionY = initPositionY;
            UpdateInterval = updateInterval;
            Velocity = velocity;
            BorderX = borderX;
            BorderY = borderY;
            SetTimer();
        }

        private void SetTimer()
        {
            _UETimer = new System.Timers.Timer(UpdateInterval * 1e3);
            _UETimer.Elapsed += OnTimedUpdate;
            _UETimer.AutoReset = true;
            _UETimer.Enabled = true;
        }

        private void OnTimedUpdate(object? source, ElapsedEventArgs e)
        {
            Move(UpdateInterval);
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
