using PinShot.Const;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PinShot.Scenes.Common {
    /// <summary>
    /// ゲーム起動時の初期化を行うクラス
    /// </summary>

    public class Startup {
        [RuntimeInitializeOnLoadMethod]
        static void OnStart() {
            SceneManager.LoadScene(ConstScene.Common, LoadSceneMode.Additive);
        }
    }
}
