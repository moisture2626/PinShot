using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PinShot.Database {
    /// <summary>
    /// マスターデータ用のテーブルクラス
    /// </summary>
    /// <typeparam name="TRecord"></typeparam>
    /// <typeparam name="TPrimary"></typeparam>
    public abstract class BaseTable<TRecord, TPrimary> : Table where TRecord : class {
        [SerializeField] private List<TRecord> _dataList;
        protected Dictionary<TPrimary, TRecord> _dataDict;
        public override void Initialize() {
            _dataDict = new();
            if (_dataList != null) {
                foreach (var item in _dataList) {
                    _dataDict.Add(GetPrimaryKey.Invoke(item), item);
                }
            }
        }
        protected abstract Func<TRecord, TPrimary> GetPrimaryKey { get; }

        public bool TryGetItem(TPrimary key, out TRecord data) {
            return _dataDict.TryGetValue(key, out data);
        }
        public IReadOnlyDictionary<TPrimary, TRecord> GetAllItem() {
            return _dataDict;
        }
        public IReadOnlyList<TRecord> GetAllItemList() {
            return _dataList;
        }
        public IReadOnlyList<TRecord> GetFilteredItem(Func<TRecord, bool> predicate) {
            return _dataDict.Where(v => predicate(v.Value)).Select(v => v.Value).ToList();
        }

    }

    /// <summary>
    /// レコードが一つの場合
    /// </summary>
    /// <typeparam name="TRecord"></typeparam>
    public abstract class BaseSingleTable<TRecord> : Table where TRecord : BaseSingleTable<TRecord> {
        public TRecord GetItem() {
            return (TRecord)this;
        }
    }

    public abstract class Table : ScriptableObject {
        public virtual void Initialize() { }
    }
}
