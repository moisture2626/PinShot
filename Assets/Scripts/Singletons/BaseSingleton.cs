using UnityEngine;

namespace PinShot.Singletons {
    public abstract class BaseSingleton<T> : MonoBehaviour where T : BaseSingleton<T> {
        public static T Instance { get; private set; }

        protected virtual void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Initialize();
            DontDestroyOnLoad(gameObject);
            Instance = (T)this;
        }

        protected virtual void Initialize() { }

        protected virtual void OnDestroy() {
            if (Instance == this) {
                Instance = null;
            }
        }
    }
}
