using System;
using UnityEngine;

namespace PinShot.Database {
    /// <summary>
    /// BGMの設定を管理するクラス
    /// </summary>
    [CreateAssetMenu(fileName = "BGMTable", menuName = "Scriptable Objects/BGMTable")]

    public class BGMTable : BaseTable<BGMData, string> {
        protected override Func<BGMData, string> GetPrimaryKey => d => d.Key;
    }

    [Serializable]
    public class BGMData {
        [SerializeField] string _key;
        [SerializeField] private AudioClip _bgmClip;
        [SerializeField] private float _volume = 1f;

        public AudioClip Clip => _bgmClip;
        public string Key => _key;
        public float Volume => _volume;
    }
}
