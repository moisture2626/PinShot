using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Database;
using PinShot.Scenes.MainGame.Ball;
using PinShot.Scenes.MainGame.Player;
using UnityEngine;

namespace PinShot.Scenes.MainGame {
    /// <summary>
    /// メインゲームシーンのDI
    /// </summary>
    public class MainGameInjector : MonoBehaviour {
        [SerializeField] private PlayerInjector _playerInjector;
        [SerializeField] private BallLauncherInjector _ballLauncherInjector;

        private void Awake() {
            Inject(destroyCancellationToken).Forget();
        }

        private async UniTask Inject(CancellationToken token) {
            await UniTask.WaitUntil(() => MasterDataManager.Instance, cancellationToken: token);
            _playerInjector.Initialize();
            _ballLauncherInjector.Initialize();
        }
    }
}
