using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PinShot.UI {
    /// <summary>
    /// ゲーム中に表示するUI
    /// </summary>
    public class GameUI : MonoBehaviour, IGameUIInitializer, IHealthView, IScoreView, IComboView, IHighScoreView, IIntervalView {
        [SerializeField] private IconHealthView _healthView;
        [SerializeField] private TMP_Text _score;
        [SerializeField] private TMP_Text _highScore;
        [SerializeField] private TMP_Text _combo;
        [SerializeField] private Image _nextGauge;

        public void Initialize() {
            SetScore(0);
            SetCombo(0);
            SetNextGauge(1);
        }
        public void InitializeHealth(float max) {
            _healthView.InitializeHealth(max);
        }
        public void SetHealth(float prev, float current, float max) {
            _healthView.SetHealth(prev, current, max);
        }
        public void SetScore(int score) {
            _score.text = score.ToString();
        }
        public void SetHighScore(int highScore) {
            _highScore.text = highScore.ToString();
        }
        public void SetCombo(int combo) {
            _combo.text = combo.ToString();
        }

        public void SetInterval(float current, float max) {
            SetNextGauge(current / max);
        }
        private void SetNextGauge(float value) {
            _nextGauge.fillAmount = value;
        }
    }
}
