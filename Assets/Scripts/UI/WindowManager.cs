using System.Collections.Generic;
using PinShot.Singletons;
using UnityEngine;

namespace PinShot.UI {
    public class WindowManager : BaseSingleton<WindowManager> {
        private Dictionary<string, BaseWindow> _windows = new();

        /// <summary>
        /// ウィンドウを開く
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Open<T>() where T : BaseWindow {
            if (!Instance._windows.TryGetValue(typeof(T).Name, out var window)) {
                var prefabGO = Resources.Load<GameObject>($"UI/Windows/{typeof(T).Name}");
                if (prefabGO == null) {
                    Debug.LogError($"Window prefab not found: {typeof(T).Name}");
                    return null;
                }
                var windowInstance = Instantiate(prefabGO, Instance.transform);
                window = windowInstance.GetComponent<T>();
                if (window == null) {
                    Debug.LogError($"Window component {typeof(T).Name} not found on prefab");
                    Destroy(windowInstance);
                    return null;
                }
                Instance._windows.Add(typeof(T).Name, window);
            }
            else {
                window.transform.SetAsLastSibling();
            }
            window.OnClose = () => {
                Instance._windows.Remove(typeof(T).Name);
            };
            return window as T;
        }

    }
}
