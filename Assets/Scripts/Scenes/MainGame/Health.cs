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
        private ReactiveProperty<float> _current = new();
        public Observable<(float prev, float current, float max)> OnChangeValue
            => _current.Pairwise().Select(pair => (pair.Previous, pair.Current, MaxValue));

        /// <summary>
        /// 現在の値
        /// </summary>
        public float Current => _current.Value;

        /// <summary>
        /// 最大耐久力
        /// </summary>
        public float MaxValue { get; private set; }

        /// <summary>
        /// 耐久力の初期化
        /// </summary>
        public void Initialize(float maxValue) {
            MaxValue = maxValue;
            _current.Value = maxValue;
        }

        /// <summary>
        /// ダメージを受ける
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(float damage) {
            if (_current == null) {
                throw new InvalidOperationException("Health is not initialized.");
            }

            var current = _current.Value;
            if (current <= 0) {
                return;
            }
            current = Math.Max(current - damage, 0);
            _current.Value = current;
        }

        /// <summary>
        /// 回復
        /// </summary>
        /// <param name="heal"></param>
        public void Heal(float heal) {
            if (_current == null) {
                throw new InvalidOperationException("Health is not initialized.");
            }

            var current = _current.Value;
            if (current >= MaxValue) {
                return;
            }
            current = Math.Min(current + heal, MaxValue);
            _current.Value = current;
        }


        public void Dispose() {
            _current?.Dispose();
        }
    }
}
