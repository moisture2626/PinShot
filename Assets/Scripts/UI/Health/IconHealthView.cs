using System.Collections.Generic;
using UnityEngine;

namespace PinShot.UI {
    /// <summary>
    /// アイコンを並べる体力表示
    /// </summary>
    public class IconHealthView : MonoBehaviour, IHealthView {
        [SerializeField] private Transform _iconRoot;
        [SerializeField] private GameObject _lifeIconPrefab;
        private List<GameObject> _lifeIcons = new();
        private bool _isInitialized = false;

        public void InitializeHealth(float max) {
            if (_isInitialized) {
                SetHealth(0, max, max);
                return;
            }
            for (int i = 0; i < (int)max; i++) {
                var icon = Instantiate(_lifeIconPrefab, _iconRoot);
                icon.SetActive(true);
                _lifeIcons.Add(icon);
            }
            _isInitialized = true;
        }

        public void SetHealth(float prev, float current, float max) {
            if (!_isInitialized) {
                Debug.LogError("HealthView is not initialized.");
                return;
            }
            if (_lifeIcons.Count < (int)max) {
                // アイコンの数が足りない場合は追加する
                for (int i = _lifeIcons.Count; i < (int)max; i++) {
                    var icon = Instantiate(_lifeIconPrefab, _iconRoot);
                    _lifeIcons.Add(icon);
                }
            }
            else if (_lifeIcons.Count > (int)max) {
                // アイコンの数が多い場合は削除する
                for (int i = _lifeIcons.Count - 1; i >= (int)max; i--) {
                    Destroy(_lifeIcons[i]);
                    _lifeIcons.RemoveAt(i);
                }
            }
            // アイコンの表示を更新する
            for (int i = 0; i < (int)max; i++) {
                _lifeIcons[i].SetActive(i < (int)current);
            }
        }
    }
}
