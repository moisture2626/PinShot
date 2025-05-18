using PinShot.Singletons;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PinShot.UI {
    public class ButtonPush : MonoBehaviour, IPointerClickHandler {

        public void OnPointerClick(PointerEventData eventData) {
            if (SoundManager.Instance != null) {
                SoundManager.Instance.PlaySE("ButtonPush");
            }
        }
    }
}
