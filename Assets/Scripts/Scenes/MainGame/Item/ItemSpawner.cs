using PinShot.Singletons;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace PinShot.Scenes.MainGame.Item {
    /// <summary>
    /// 強化アイテムのプール
    /// </summary> 
    public class ItemSpawner : MonoBehaviour {
        [SerializeField] private ItemObject _itemPrefab;
        private ItemSettings _settings;

        private ObjectPool<ItemObject> _pool;

        [Inject]
        public void Construct(MasterDataManager mst) {
            _settings = mst.GetTable<ItemSettings>();
            _pool = new ObjectPool<ItemObject>(
                CreateItem,
                OnGetItem,
                OnReleaseItem,
                OnDestroyItem,
                maxSize: 10
            );
        }

        public ItemObject GetItem() {
            return _pool.Get();
        }
        public void ReleaseItem(ItemObject item) {
            _pool.Release(item);
        }
        public void Clear() {
            // プール内のアイテムを全て破棄
            _pool.Clear();
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
