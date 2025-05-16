using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace PinShot.Singletons {
    /// <summary>
    /// シーン切り替えを制御するクラス
    /// </summary>
    public class SceneChanger : BaseSingleton<SceneChanger> {

        public async UniTask ChangeScene(string sceneName, CancellationToken token) {
            await SceneManager.LoadSceneAsync(sceneName).ToUniTask(cancellationToken: token);
        }
    }
}
