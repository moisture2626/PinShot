using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Singletons;
using UnityEngine;

namespace PinShot.Scenes.Title {
    /// <summary>
    /// タイトルシーンのDI
    /// </summary>
    public class TitleInjector : MonoBehaviour {
        [SerializeField] private TitleView _view;
        private TitlePresenter _presenter;
        void Awake() {
            Inject(destroyCancellationToken).Forget();
        }

        private async UniTask Inject(CancellationToken token) {
            // まずSaveDataManagerを待機
            await UniTask.WaitUntil(() => SaveDataManager.Instance, cancellationToken: token);
            // その後、Singletonを待機
            await UniTask.WhenAll(
                UniTask.WaitUntil(() => MasterDataManager.Instance, cancellationToken: token),
                UniTask.WaitUntil(() => WindowManager.Instance, cancellationToken: token),
                UniTask.WaitUntil(() => SoundManager.Instance, cancellationToken: token)
            );

            _presenter = new TitlePresenter();
            _presenter.Initialize(_view);
        }

        void OnDestroy() {

        }
    }
}
