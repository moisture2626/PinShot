using System;
using UnityEngine;

namespace PinShot.Database {
    /// <summary>
    /// SEの設定を管理するクラス
    /// </summary>
    [CreateAssetMenu(fileName = "SETable", menuName = "Scriptable Objects/SETable")]
    public class SETable : BaseTable<SEData, string> {
        protected override Func<SEData, string> GetPrimaryKey => d => d.Key;
    }

    [Serializable]
    public class SEData {
        [SerializeField] string _key;
        [SerializeField] private AudioClip _seClip;
        [SerializeField] private float _volume = 1f;
        [SerializeField] private bool _isPoolable = true; // 頻繁に再生されるSEはプールしておく

        public AudioClip Clip => _seClip;
        public string Key => _key;
        public float Volume => _volume;
        public bool IsPoolable => _isPoolable;
    }
}
