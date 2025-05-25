using PinShot.Event;
using UnityEngine;

namespace PinShot.Scenes.MainGame.Item {
    public struct ItemDropEvent : IEvent {
        public Vector3 Position;
        public ItemDropEvent(Vector3 position) {
            Position = position;
        }
    }
}
