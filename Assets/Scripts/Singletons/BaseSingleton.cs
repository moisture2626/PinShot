using UnityEngine;
using VContainer;

namespace PinShot.Singletons {
    public abstract class BaseSingleton<T> : MonoBehaviour where T : BaseSingleton<T> {
        public static T Instance { get; private set; }
        [Inject]
        public void Construct() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Initialize();
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
