using System;
using PinShot.Event;
using PinShot.Singletons;
using R3;

namespace PinShot.Scenes.MainGame {
    public class ScoreManager : IDisposable {

        private ReactiveProperty<int> _score;
        public int Score => _score.Value;
        public Observable<int> OnChangeScore => _score;
        private int _highScore;
        public int HighScore => _highScore;
        private IDisposable _scoreEventSubscription;
        private SaveDataManager _saveDataManager;

        public ScoreManager(SaveDataManager saveDataManager) {
            _saveDataManager = saveDataManager;
            _highScore = saveDataManager.Load<ScoreData>("HighScore").HighScore;
            // スコア計算
            _scoreEventSubscription = EventManager<ScoreEvent>.Subscribe(
                ev => {
                    _score.Value += ev.AddScore + ev.Combo * 10;
                    if (_score.Value > _highScore) {
                        _highScore = _score.Value;
                    }
                }
            );
        }

        public void Save() {
            // スコアを保存
            var scoreData = new ScoreData {
                HighScore = _highScore
            };
            _saveDataManager.Save("HighScore", scoreData);
        }

        /// <summary>
        /// ゲーム開始時などのスコアリセット
        /// </summary>
        public void Reset() {
            _score?.Dispose();
            _score = new ReactiveProperty<int>(0);
        }

        public void Dispose() {
            _scoreEventSubscription?.Dispose();
            _score?.Dispose();
        }

        [Serializable]
        private class ScoreData {
            public int HighScore = 0;
        }
    }
}
