using PinShot.Singletons;
using UnityEngine;

namespace PinShot.Scenes.MainGame.Item {
    public class ItemManagerInjector : MonoBehaviour {
        [SerializeField] private ItemManager _itemManager;

        public void Initialize() {
            var settings = MasterDataManager.GetTable<ItemSettings>();
            var colors = MasterDataManager.GetTable<ItemTable>();
            _itemManager.Initialize(settings, colors);
        }
    }
}
