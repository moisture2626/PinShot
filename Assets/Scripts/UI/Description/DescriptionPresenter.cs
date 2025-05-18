using R3;

namespace PinShot.UI {
    public class DescriptionPresenter {
        private DescriptionWindow _window;

        public void Initialize(DescriptionWindow window) {
            _window = window;
            _window.CloseButton
                .Subscribe(_ => {
                    // 閉じるボタンが押されたときの処理
                    _window.Close();
                }).AddTo(_window);
        }

    }
}
