using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Event;
using UnityEngine;
using UnityEngine.Pool;

namespace PinShot.Scenes.MainGame.Item {
    public class ItemManager : MonoBehaviour {
        private static ItemManager _instance;
        private ItemSettings _settings;
        private ItemTable _colors;
        [SerializeField] private ItemObject _itemPrefab;
        [SerializeField] private Transform _limit;
        private ObjectPool<ItemObject> _pool;
        public void Initialize(ItemSettings settings, ItemTable colors) {
            _settings = settings;
            _colors = colors;
            _pool = new ObjectPool<ItemObject>(
                CreateItem,
                OnGetItem,
                OnReleaseItem,
                OnDestroyItem,
                maxSize: 10
            );

            // ゲームが終わったらプールをクリアする
            EventManager<GameStateEvent>.Subscribe(this, ev => {
                if (ev.State == GameState.GameOver) {
                    _pool.Clear();
                }
            });

            _instance = this;
        }

        /// <summary>
        /// アイテムをドロップする
        /// </summary>
        /// <param name="position"></param>
        public static void DropItem(Vector2 position) {
            var item = _instance._pool.Get();
            item.transform.position = position;
            var data = _instance._colors.GetRandomData();
            var value = data.Type switch {
                ItemType.SpeedUp => _instance._settings.AddSpeed,
                ItemType.PowerUp => _instance._settings.AddPower,
                _ => 0
            };
            item.SetItemType(data.Type, value, data.Color);
            _instance.MoveFlow(item, _instance.destroyCancellationToken).Forget();
        }

        private async UniTask MoveFlow(ItemObject item, CancellationToken token) {
            var limit = _limit.position.y;
            await item.MoveFlow(limit, token);
            // アイテムが画面外に出たらプールに戻す
            _pool.Release(item);
        }


        #region Pool
        private ItemObject CreateItem() {
            var item = Instantiate(_itemPrefab, transform);
            item.Initialize(_settings.MoveSpeed, _settings.Score);
            return item;
        }
        private void OnGetItem(ItemObject item) {
            item.gameObject.SetActive(true);
        }
        private void OnReleaseItem(ItemObject item) {
            item.gameObject.SetActive(false);
        }
        private void OnDestroyItem(ItemObject item) {
            Destroy(item.gameObject);
        }
        #endregion
    }
}
