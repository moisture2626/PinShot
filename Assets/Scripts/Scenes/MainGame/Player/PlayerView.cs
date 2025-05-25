using PinShot.Event;
using PinShot.Scenes.MainGame.Item;
using R3;
using UnityEngine;
using VContainer;

namespace PinShot.Scenes.MainGame.Player {
    public class PlayerView : MonoBehaviour {
        private Subject<(ItemType type, float value)> _itemEffectSubject;
        public Observable<(ItemType type, float value)> ItemEffectObservable => _itemEffectSubject ??= new();
        [Inject]
        public void Construct() {
            // 今のところ特にすることない
        }
        void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.GetComponent<ItemObject>() is ItemObject item) {
                var itemEffect = item.GetItemData();
                _itemEffectSubject.OnNext((itemEffect.itemType, itemEffect.addValue));
                item.gameObject.SetActive(false);
                // アイテムのスコアを加算
                EventManager<ScoreEvent>.TriggerEvent(new ScoreEvent(itemEffect.score, 0));
            }
        }

        void OnDestroy() {
            _itemEffectSubject?.Dispose();
        }
    }
}
