using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Event;
using UnityEngine;

namespace PinShot.Scenes.MainGame {
    public class MainGameManager : MonoBehaviour {

        private CancellationTokenSource _gameFlowCancellation;
        public void Initialize() {
            _gameFlowCancellation = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

            EventManager<GameStateEvent>.SubscribeAwait(this, async (sv, t) => {

            });
        }

        public void BeginGame() {
            EventManager<GameStateEvent>.TriggerEvent(GameStateEvent.Create(GameState.Standby));
            GameFlow(_gameFlowCancellation.Token).Forget();
        }

        private async UniTask GameFlow(CancellationToken token) {
            while (!token.IsCancellationRequested) {
                // ゲームのメインループ
                // 開始前のカウントダウンを待機
                await EventManager<GameStateEvent>.WaitForEvent(ev => ev.State == GameState.Ready, token);

                //　カウントダウン開始
                await UniTask.Delay(1000, cancellationToken: token);
                // ゲーム開始
                EventManager<GameStateEvent>.TriggerEvent(GameStateEvent.Create(GameState.Play));


                await EventManager<GameStateEvent>.WaitForEvent(ev => ev.State == GameState.GameOver, token);

                // ゲームオーバー演出

                await EventManager<GameStateEvent>.WaitForEvent(ev => ev.State == GameState.Result, token);

                // 結果画面表示

                await EventManager<GameStateEvent>.WaitForEvent(ev => ev.State == GameState.Standby, token);
            }
        }

        void OnDestroy() {
            // Clean up the event manager
            EventManager<GameStateEvent>.DisposeAll();
            Debug.Log("Main Game Manager Destroyed");
        }
    }
}
