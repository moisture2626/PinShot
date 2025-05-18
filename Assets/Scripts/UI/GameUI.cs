using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PinShot.UI {
    /// <summary>
    /// ゲーム中に表示するUI
    /// </summary>
    public class GameUI : MonoBehaviour {
        [SerializeField] private GameObject[] _life;
        [SerializeField] private TMP_Text _score;
        [SerializeField] private TMP_Text _highScore;
        [SerializeField] private TMP_Text _combo;
        [SerializeField] private Image _nextGauge;

        public void Initialize() {
            SetScore(0);
            SetCombo(0);
            SetNextGauge(1);
        }
        public void SetLife(int life) {
            for (int i = 0; i < _life.Length; i++) {
                _life[i].SetActive(i < life);
            }
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
        public void SetNextGauge(float value) {
            _nextGauge.fillAmount = value;
        }
    }
}
