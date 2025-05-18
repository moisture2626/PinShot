using PinShot.Database;
using UnityEngine;

namespace PinShot.Scenes.MainGame.Item {
    [CreateAssetMenu(fileName = "ItemSettings", menuName = "Scriptable Objects/ItemSettings")]
    public class ItemSettings : BaseSingleTable<ItemSettings> {
        [SerializeField] private float _moveSpeed = 1;
        [SerializeField] private float _addPower = 1;
        [SerializeField] private float _addSpeed = 0.1f;
        [SerializeField] private int _score = 100;
        public float MoveSpeed => _moveSpeed;
        public float AddPower => _addPower;
        public float AddSpeed => _addSpeed;
        public int Score => _score;

    }
}
