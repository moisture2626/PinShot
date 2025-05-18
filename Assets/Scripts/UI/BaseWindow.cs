using System;
using UnityEngine;

public abstract class BaseWindow : MonoBehaviour {
    private Action _onClose;
    public Action OnClose {
        get => _onClose;
        set {
            if (_onClose != null) {
                Debug.LogWarning("OnClose already set. Overwriting.");
            }
            _onClose = value;
        }
    }
    public void Close() {
        if (_onClose != null) {
            _onClose.Invoke();
        }
        Destroy(gameObject);
    }
}
