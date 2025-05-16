using UnityEngine;

namespace PinShot.Singletons {
    public abstract class BaseSingleton<T> : MonoBehaviour {
        public static BaseSingleton<T> Instance { get; private set; }

        protected virtual void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
