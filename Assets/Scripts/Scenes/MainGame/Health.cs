using System;
using R3;

namespace PinShot.Scenes.MainGame {
    /// <summary>
    /// 耐久力
    /// </summary>
    public class Health : IDisposable {
        /// <summary>
        /// 耐久力
        /// </summary>
        private ReactiveProperty<float> _current;
        public ReactiveProperty<float> Current => _current ??= new();

        /// <summary>
        /// 最大耐久力
        /// </summary>
        public float MaxValue { get; private set; }

        /// <summary>
        /// 耐久力の初期化
        /// </summary>
        public void Initialize(float maxValue) {
            MaxValue = maxValue;
            Current.Value = maxValue;
        }

        /// <summary>
        /// ダメージを受ける
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(float damage) {
            if (Current == null) {
                throw new InvalidOperationException("Health is not initialized.");
            }

            var current = Current.Value;
            if (current <= 0) {
                return;
            }
            current = Math.Max(current - damage, 0);
            Current.Value = current;
        }

        /// <summary>
        /// 回復
        /// </summary>
        /// <param name="heal"></param>
        public void Heal(float heal) {
            if (Current == null) {
                throw new InvalidOperationException("Health is not initialized.");
            }

            var current = Current.Value;
            if (current >= MaxValue) {
                return;
            }
            current = Math.Min(current + heal, MaxValue);
            Current.Value = current;
        }


        public void Dispose() {
            Current?.Dispose();
        }
    }
}
