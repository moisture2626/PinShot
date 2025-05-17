using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Event;
using UnityEngine;

namespace PinShot.Scenes.MainGame {
    public class MainGameManager : MonoBehaviour {

        private CancellationTokenSource _gameFlowCancellation;
        public void Initialize() {
            _gameFlowCancellation = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        }

        /// <summary>
        /// ゲーム開始
        /// </summary>
        public void BeginGame() {
            EventManager<GameStateEvent>.TriggerEvent(GameStateEvent.Create(GameState.Standby));
            GameFlow(_gameFlowCancellation.Token).Forget();
        }

        /// <summary>
        /// ゲームループ
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask GameFlow(CancellationToken token) {
            while (!token.IsCancellationRequested) {
                // ゲームのメインループ
                // 開始前のカウントダウンを待機
                await EventManager<GameStateEvent>.WaitForEvent(ev => ev.State == GameState.Ready, token);
                token.ThrowIfCancellationRequested();

                //　カウントダウン開始
                await UniTask.Delay(1000, cancellationToken: token);
                token.ThrowIfCancellationRequested();
                // ゲーム開始
                EventManager<GameStateEvent>.TriggerEvent(GameStateEvent.Create(GameState.Play));


                await EventManager<GameStateEvent>.WaitForEvent(ev => ev.State == GameState.GameOver, token);
                token.ThrowIfCancellationRequested();

                // ゲームオーバー演出

                await EventManager<GameStateEvent>.WaitForEvent(ev => ev.State == GameState.Result, token);
                token.ThrowIfCancellationRequested();

                // 結果画面表示

                await EventManager<GameStateEvent>.WaitForEvent(ev => ev.State == GameState.Standby, token);
                token.ThrowIfCancellationRequested();
            }
        }

        void OnDestroy() {
            // Clean up the event manager
            EventManager<GameStateEvent>.DisposeAll();
            Debug.Log("Main Game Manager Destroyed");
        }
    }
}
