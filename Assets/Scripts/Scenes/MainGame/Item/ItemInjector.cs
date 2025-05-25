using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PinShot.Scenes.MainGame.Item {
    public class ItemInjector : MonoBehaviour {
        [SerializeField] private ItemSpawner _itemSpawner;
        [SerializeField] private Transform _deadLine;

        public void Configure(IContainerBuilder builder) {
            builder.RegisterComponent(_itemSpawner);
            builder.RegisterEntryPoint<ItemPresenter>(Lifetime.Scoped)
                .WithParameter("deadLine", _deadLine.position.y);
        }
    }
}
