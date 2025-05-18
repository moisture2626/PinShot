using UnityEngine;

namespace PinShot.Scenes.MainGame.Ball {
    public interface IBallEnter {

        (float offset, float velocity) OnEnterBall(Vector2 hitPosition);

        void OnExitBall(Vector2 hitPosition);
    }
}
