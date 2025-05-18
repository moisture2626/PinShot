using System;
using PinShot.Database;
using UnityEngine;

namespace PinShot.Scenes.MainGame.Item {
    [CreateAssetMenu(fileName = "ItemTable", menuName = "Scriptable Objects/ItemTable")]
    public class ItemTable : BaseTable<ItemTableData, ItemType> {
        protected override Func<ItemTableData, ItemType> GetPrimaryKey => d => d.Type;
        public ItemTableData GetRandomData() {
            var all = GetAllItem();
            var randomIndex = UnityEngine.Random.Range(0, all.Count);
            return all[(ItemType)randomIndex];
        }
    }

    [Serializable]
    public class ItemTableData {
        [SerializeField] private ItemType _type;
        [SerializeField] private Color _itemColor;

        public ItemType Type => _type;
        public Color Color => _itemColor;
    }

    public enum ItemType {
        SpeedUp,
        PowerUp,
    }
}
