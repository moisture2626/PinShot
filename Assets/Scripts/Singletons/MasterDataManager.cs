using System;
using System.Collections.Generic;
using System.Linq;
using PinShot.Database;
using UnityEngine;
using VContainer;

namespace PinShot.Singletons {
    /// <summary>
    /// マスターデータを持っておくクラス
    /// </summary> 
    public class MasterDataManager : MonoBehaviour {
        // 全部SerializeFieldで突っ込むのも微妙だけどそんなに多くならないはずなので良しとしておく
        [SerializeField] private List<Table> _tables;
        private Dictionary<Type, Table> _tableDictionary;
        [Inject]
        public void Construct() {
            _tableDictionary = _tables.ToDictionary(v => v.GetType());
            foreach (var table in _tables) {
                table.Initialize();
            }
        }

        public T GetTable<T>() where T : Table {

            if (_tableDictionary.TryGetValue(typeof(T), out var table)) {
                return table as T;
            }
            return null;
        }
    }
}
