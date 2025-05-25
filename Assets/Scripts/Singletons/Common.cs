using UnityEngine;

namespace PinShot.Scenes.Common {
    /// <summary>
    /// 特に何もしない
    /// カメラを持っているので各シーンで使い回すためのもの
    /// </summary>
    public class Common : MonoBehaviour {
        void Awake() {
            DontDestroyOnLoad(gameObject);
        }
    }
}
