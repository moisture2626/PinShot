using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using PinShot.Scenes.MainGame;
using PinShot.Singletons;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PinShot.UI {
    /// <summary>
    /// リザルト画面
    /// </summary>
    public class ResultWindow : BaseWindow {
        [SerializeField] private Button _retryButton;
        public Observable<Unit> OnRetryButtonClicked => _retryButton.OnClickAsObservable();

        [SerializeField] private Button _titleButton;
        public Observable<Unit> OnTitleButtonClicked => _titleButton.OnClickAsObservable();

        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private GameObject _highScoreText;

        /// <summary>
        /// 開いてから閉じるまで待機
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async UniTask OpenAsync(ScoreManager scoreManager, CancellationToken token) {
            var window = WindowManager.Open<ResultWindow>();
            var presenter = new ResultPresenter();
            presenter.Initialize(window, scoreManager);

            await window.OnDestroyAsync().AttachExternalCancellation(token);
        }

        void Awake() {
            _retryButton.gameObject.SetActive(false);
            _titleButton.gameObject.SetActive(false);
            _highScoreText.SetActive(false);
        }

        /// <summary>
        /// スコア表示演出
        /// </summary>
        /// <param name="score"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async UniTask ShowScore(int score, bool isHighScore, CancellationToken token) {
            _scoreText.text = $"SCORE: {0:D8}";
            await UniTask.Delay(500, cancellationToken: token);
            int value = 0;
            await DOTween.To(
                () => value,
                x => {
                    value = x;
                    _scoreText.text = $"SCORE: {x:D8}";
                },
                score,
                1f
            )
            .SetEase(Ease.Linear)
            .WithCancellation(token);

            _highScoreText.SetActive(isHighScore);
        }

        /// <summary>
        /// ボタンを表示
        /// </summary>
        public void ShowButtons() {
            _retryButton.gameObject.SetActive(true);
            _titleButton.gameObject.SetActive(true);
        }
    }
}
