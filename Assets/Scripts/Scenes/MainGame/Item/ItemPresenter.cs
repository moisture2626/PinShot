using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Event;
using PinShot.Singletons;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PinShot.Scenes.MainGame.Item {
    public class ItemPresenter : IInitializable, IDisposable {
        [Inject] private ItemSpawner _itemSpawner;
        private ItemSettings _settings;
        private ItemTable _colors;
        private CancellationDisposable _cancellation;
        private float _deadLinePosition;
        private bool _isActive = false;
        public ItemPresenter(MasterDataManager mst, float deadLine) {
            _deadLinePosition = deadLine;
            _settings = mst.GetTable<ItemSettings>();
            _colors = mst.GetTable<ItemTable>();
            _cancellation = new CancellationDisposable();
        }

        public void Initialize() {
            // ゲームが終わったらプールをクリアする
            EventManager<GameStateEvent>.Subscribe(ev => {
                _isActive = ev.State == GameState.Play;

                if (ev.State == GameState.GameOver) {
                    _itemSpawner.Clear();
                }
            }).AddTo(_cancellation.Token);

            // アイテムドロップイベント
            EventManager<ItemDropEvent>.Subscribe(ev => {
                DropItem(ev.Position);
            }).AddTo(_cancellation.Token);
        }

        /// <summary>
        /// アイテムをドロップする
        /// </summary>
        /// <param name="position"></param>
        private void DropItem(Vector2 position) {
            if (!_isActive) return;

            var item = _itemSpawner.GetItem();
            item.transform.position = position;
            var data = _colors.GetRandomData();
            var value = data.Type switch {
                ItemType.SpeedUp => _settings.AddSpeed,
                ItemType.PowerUp => _settings.AddPower,
                _ => 0
            };
            item.SetItemType(data.Type, value, data.Color);
            MoveFlow(item, _cancellation.Token).Forget();
        }

        private async UniTask MoveFlow(ItemObject item, CancellationToken token) {
            await item.MoveFlow(_deadLinePosition, token);
            // アイテムが画面外に出たらプールに戻す
            _itemSpawner.ReleaseItem(item);
        }

        public void Dispose() {
            _cancellation?.Dispose();
        }
    }
}
