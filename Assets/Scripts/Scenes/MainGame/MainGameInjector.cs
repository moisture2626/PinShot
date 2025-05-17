using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Database;
using PinShot.Event;
using PinShot.Scenes.MainGame.Ball;
using PinShot.Scenes.MainGame.Player;
using UnityEngine;

namespace PinShot.Scenes.MainGame {
    /// <summary>
    /// メインゲームシーンのDI
    /// </summary>
    public class MainGameInjector : MonoBehaviour {
        [SerializeField] private PlayerInjector _playerInjector;
        [SerializeField] private BallManagerInjector _ballManagerInjector;
        [SerializeField] private MainGameManager _mainGameManager;

        private void Awake() {
            Inject(destroyCancellationToken).Forget();
        }

        private async UniTask Inject(CancellationToken token) {
            await UniTask.WaitUntil(() => MasterDataManager.Instance, cancellationToken: token);
            _ = new EventManager<GameStateEvent>();

            _playerInjector.Initialize();
            _ballManagerInjector.Initialize();
            _mainGameManager.Initialize();

            // 初期化終了後、ゲーム開始
            _mainGameManager.BeginGame();
        }
    }
}
