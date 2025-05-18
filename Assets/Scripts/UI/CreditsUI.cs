using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PinShot.UI {
    /// <summary>
    /// クレジット表示用UIコンポーネント
    /// </summary>
    public class CreditsUI : MonoBehaviour {
        [SerializeField] private Button _closeButton;
        [SerializeField] private TextMeshProUGUI _creditsText;

        private void Awake() {
            if (_closeButton != null) {
                _closeButton.onClick.AddListener(OnCloseButtonClicked);
            }

            if (_creditsText != null) {
                SetupCreditsText();
            }
        }

        private void OnCloseButtonClicked() {
            gameObject.SetActive(false);
        }
        private void SetupCreditsText() {
            // クレジットテキストの設定
            _creditsText.text = @"## サードパーティライセンス

このゲームでは以下のライブラリ、アセットを使用しています：

### UniTask
MIT License
Copyright (c) 2019 Yoshifumi Kawai / Cysharp, Inc.
https://github.com/Cysharp/UniTask

### R3
MIT License
Copyright (c) 2020 Yoshifumi Kawai / Cysharp, Inc.
https://github.com/Cysharp/R3

### DOTween (Free版)
MIT License
Copyright (c) 2014 Daniele Giardini - Demigiant
http://dotween.demigiant.com/

## Unity Asset Store アセット
標準Unity Asset Store EULA

### Dark Theme UI
作者/パブリッシャー: Giniel Villacote

## 音楽

### BGM
曲名: Sanctuary (music only), Barely
作曲 : RYU ITO
https://ryu110.com/

詳細なライセンス情報はプロジェクトルートの
THIRD_PARTY_NOTICES.mdをご確認ください。";
        }

        /// <summary>
        /// クレジット画面を表示する
        /// </summary>
        public void Show() {
            gameObject.SetActive(true);
        }
    }
}
