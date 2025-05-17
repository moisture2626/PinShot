using UnityEngine;

namespace PinShot.Database {
    [CreateAssetMenu(fileName = "BallSettings", menuName = "Scriptable Objects/BallSettings")]
    public class BallSettings : BaseSingleTable<BallSettings> {
        [SerializeField] private float _gravityScale = 1f;
        [SerializeField] private float _life = 10;
    }
}
