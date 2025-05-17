using System;
using System.Collections.Generic;
using System.Linq;
using PinShot.Singletons;
using UnityEngine;

namespace PinShot.Database {
    /// <summary>
    /// マスターデータを持っておくクラス
    /// </summary> 
    public class MasterDataManager : BaseSingleton<MasterDataManager> {
        // 全部SerializeFieldで突っ込むのも微妙だけどそんなに多くならないはずなので良しとしておく
        [SerializeField] private List<Table> _tables;
        private Dictionary<Type, Table> _tableDictionary;

        protected override void Awake() {
            base.Awake();

            _tableDictionary = _tables.ToDictionary(v => v.GetType());
            foreach (var table in _tables) {
                table.Initialize();
            }
        }

        public static T GetTable<T>() where T : Table {
            if (Instance._tableDictionary.TryGetValue(typeof(T), out var table)) {
                return table as T;
            }
            return null;
        }
    }
}
