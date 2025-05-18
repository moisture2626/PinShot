namespace PinShot.Const {
    public class ConstLayer {
        public enum Layer {
            Default = 0,


            UI = 5,
            Ball = 6,
            CannotEnter = 7,
            Player = 8,
            Obstacle = 9,
            Bullet = 10,
            Item = 11,
        }

        public static int GetLayerMask(params Layer[] layer) {
            int mask = 0;
            foreach (var l in layer) {
                mask |= 1 << (int)l;
            }
            return mask;
        }
    }
}
