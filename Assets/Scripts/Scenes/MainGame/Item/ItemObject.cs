using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PinShot.Scenes.MainGame.Item {
    public class ItemObject : MonoBehaviour {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private float _addValue;
        private int _addScore;
        private float _moveSpeed;
        private ItemType _itemType;

        public void Initialize(float moveSpeed, int addScore) {
            _addScore = addScore;
            _moveSpeed = moveSpeed;
        }

        public void SetItemType(ItemType itemType, float addValue, Color color) {
            _itemType = itemType;
            _addValue = addValue;
            _spriteRenderer.color = color;
        }

        public (ItemType itemType, float addValue, int score) GetItemData() {
            return (_itemType, _addValue, _addScore);
        }

        public async UniTask MoveFlow(float limit, CancellationToken token) {
            while (!token.IsCancellationRequested && gameObject.activeSelf) {
                transform.position += Vector3.down * _moveSpeed * Time.deltaTime;
                if (transform.position.y < limit) {
                    gameObject.SetActive(false);
                    return;
                }
                await UniTask.Yield(token);
            }
        }

    }
}
