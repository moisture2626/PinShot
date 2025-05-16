using System;
using Cysharp.Threading.Tasks;
using PinShot.Const;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PinShot.Singletons {
    /// <summary>
    /// シーン切り替えを制御するクラス
    /// </summary>
    public class SceneChanger : BaseSingleton<SceneChanger> {
        [SerializeField] private Color _fadeColor = Color.black;
        [SerializeField] private float _fadeDuration = 0.5f;

        private bool _isLoading = false;


        /// <summary>
        /// シーン切り替え
        /// キャンセルされる想定じゃないのでCancellationTokenは渡さない
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public async UniTask ChangeScene(string sceneName) {
            if (_isLoading) {
                return;
            }
            _isLoading = true;

            // フェードアウト
            await ScreenFade.Instance.FadeOutAsync(_fadeDuration, _fadeColor, default);

            // 一度空のシーンを読み込んでGCを実行する
            await SceneManager.LoadSceneAsync(ConstScene.Empty, LoadSceneMode.Additive).ToUniTask();
            await SceneManager.UnloadSceneAsync(ConstScene.Empty).ToUniTask();
            GC.Collect();

            // 目的のシーンをロード
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single).ToUniTask();

            // フェードイン
            await ScreenFade.Instance.FadeInAsync(_fadeDuration, _fadeColor, default);

            _isLoading = false;
        }
    }
}
