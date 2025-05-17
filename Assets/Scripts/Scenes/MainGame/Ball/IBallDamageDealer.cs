using UnityEngine;

namespace PinShot.Scenes.MainGame.Ball {

    /// <summary>
    /// 隕石にダメージを与えるクラスのインターフェース
    /// </summary>
    public interface IBallDamageDealer {
        /// <summary>
        /// ぶつかったときの処理
        /// </summary>
        /// <returns>ダメージ量と衝撃量</returns>
        (float damage, float impact) OnHitBall(Vector2 hitPosition);

    }
}
