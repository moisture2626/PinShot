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
                var prefab = Resources.Load<T>($"UI/Windows/{typeof(T).Name}");
                if (prefab == null) {
                    Debug.LogError($"Window prefab not found: {typeof(T).Name}");
                    return null;
                }
                window = Instantiate(prefab, Instance.transform);
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
