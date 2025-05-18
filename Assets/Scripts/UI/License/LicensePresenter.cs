using R3;

namespace PinShot.UI {
    public class LicensePresenter {
        private LicenseWindow _window;

        public void Initialize(LicenseWindow window) {
            _window = window;
            _window.CloseButton
                .Subscribe(_ => _window.Close())
                .AddTo(_window);
        }
    }
}
