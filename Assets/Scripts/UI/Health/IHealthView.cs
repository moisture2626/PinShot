namespace PinShot.UI {
    /// <summary>
    /// 体力表示用Interface
    /// </summary>
    public interface IHealthView {

        public void Initialize(float max);
        /// <summary>
        /// 体力を表示する
        /// </summary>
        /// <param name="prev">前の体力</param>
        /// <param name="current">現在の体力</param>
        /// <param name="max">最大体力</param>
        public void SetHealth(float prev, float current, float max);
    }
}
