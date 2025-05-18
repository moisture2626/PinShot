using UnityEngine;

namespace PinShot.Scenes.MainGame.Ball {
    public interface IBallEnter {
        /// <summary>
        /// 隕石にぶつかったときの処理
        /// </summary>
        /// <param name="hitPosition">ぶつかった位置</param>
        /// <returns>押し返す力</returns>
        Vector2 OnStayBall(Vector2 hitPosition);

        float OnEnterBall(Vector2 hitPosition);

        void OnExitBall(Vector2 hitPosition);
    }
}
