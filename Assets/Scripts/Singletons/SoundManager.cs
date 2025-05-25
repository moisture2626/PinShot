using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PinShot.Database;
using UnityEngine;
using VContainer;

namespace PinShot.Singletons {
    public class SoundManager : MonoBehaviour {
        public static SoundManager Instance { get; private set; }
        // 設定関連
        [Header("BGM設定")]
        [SerializeField] private BGMTable _bgmTable;
        private float _bgmMasterVolume = 0.5f;
        public float BGMVolume => _bgmMasterVolume;

        [Header("SE設定")]
        [SerializeField] private SETable _seTable;
        private float _seMasterVolume = 0.5f;
        public float SEVolume => _seMasterVolume;
        [SerializeField] private int _seAudioSourcePoolSize = 10;

        // コンポーネント
        private AudioSource _bgmSource;
        private readonly List<AudioSource> _seSourcePool = new();
        private readonly Queue<AudioSource> _availableSESourceQueue = new();

        // 状態管理
        private string _currentBGMKey;
        private CancellationTokenSource _fadeCancellation;

        // AudioSourceをキャッシュするためのディクショナリ
        private readonly Dictionary<string, AudioClip> _seClipCache = new();

        public bool IsLoaded { get; private set; } = false;
        private const string _saveDataKey = "AudioVolume";

        private SaveDataManager _saveDataManager;

        [Inject]
        public void Construct(SaveDataManager saveDataManager) {
            _saveDataManager = saveDataManager;
            InitializeAudioSources();

            // マスターデータの初期化
            if (_bgmTable != null)
                _bgmTable.Initialize();
            if (_seTable != null)
                _seTable.Initialize();

            GetSaveData();
            Instance = this;
        }

        private void InitializeAudioSources() {
            // BGM用のAudioSource
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.loop = true;
            _bgmSource.playOnAwake = false;

            // SE用のAudioSourceプール
            for (int i = 0; i < _seAudioSourcePoolSize; i++) {
                var seSource = gameObject.AddComponent<AudioSource>();
                seSource.loop = false;
                seSource.playOnAwake = false;
                _seSourcePool.Add(seSource);
                _availableSESourceQueue.Enqueue(seSource);
            }
        }
        /// <summary>
        /// セーブデータの読み込み
        /// </summary>
        /// <returns></returns>
        private void GetSaveData() {
            // セーブデータから音量を取得
            var saveData = _saveDataManager.Load<AudioVolume>(_saveDataKey);
            if (saveData != null) {
                _bgmMasterVolume = saveData.BGMVolume;
                _seMasterVolume = saveData.SEVolume;
            }
            else {
                // セーブデータがない場合はデフォルト値を使用
                _bgmMasterVolume = 0.5f;
                _seMasterVolume = 0.5f;
            }
            IsLoaded = true;
        }

        #region BGM操作
        /// <summary>
        /// BGMを再生する
        /// </summary>
        /// <param name="bgmKey"></param>
        /// <param name="fadeTime"></param>
        public void PlayBGM(string bgmKey, float fadeTime = 0.5f) {
            if (string.IsNullOrEmpty(bgmKey) || bgmKey == _currentBGMKey)
                return;

            if (_bgmTable == null) {
                Debug.LogError("BGMTableが設定されていません");
                return;
            }

            // 既存のフェードを停止

            var token = CreateNewCancellation();

            // BGM切り替え
            if (_bgmTable.TryGetItem(bgmKey, out BGMData bgmData)) {
                _currentBGMKey = bgmKey;

                if (fadeTime > 0) {
                    FadeBGM(bgmData, fadeTime, token).Forget();
                }
                else {
                    _bgmSource.Stop();
                    _bgmSource.clip = bgmData.Clip;
                    _bgmSource.volume = bgmData.Volume * _bgmMasterVolume;
                    _bgmSource.Play();
                }
            }
            else {
                Debug.LogWarning($"指定されたBGM: {bgmKey} が見つかりません");
            }
        }

        /// <summary>
        /// BGMをフェードイン/アウトで切り替える
        /// </summary>
        private async UniTask FadeBGM(BGMData nextBgm, float fadeTime, CancellationToken token) {
            // 現在のBGMがある場合はフェードアウト
            if (_bgmSource.isPlaying) {

                // フェードアウト
                await _bgmSource.DOFade(0, fadeTime / 2)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(true)  // タイムスケール影響なし
                    .ToUniTask(cancellationToken: token);

                _bgmSource.Stop();
            }

            // 次のBGMをセット
            _bgmSource.clip = nextBgm.Clip;
            _bgmSource.volume = 0;
            _bgmSource.Play();

            // フェードイン
            float targetVolume = nextBgm.Volume * _bgmMasterVolume;
            await _bgmSource.DOFade(targetVolume, fadeTime / 2)
                .SetEase(Ease.InQuad)
                .SetUpdate(true)  // タイムスケール影響なし
                .ToUniTask(cancellationToken: token);

            _bgmSource.volume = targetVolume;
        }

        /// <summary>
        /// BGMの音量を調整する
        /// </summary>
        /// <param name="volume">0-1の間の音量</param>
        public void SetBGMMasterVolume(float volume) {
            _bgmMasterVolume = Mathf.Clamp01(volume);

            if (_bgmSource != null && _currentBGMKey != null && _bgmTable.TryGetItem(_currentBGMKey, out BGMData bgmData)) {
                _bgmSource.volume = bgmData.Volume * _bgmMasterVolume;
            }
        }

        /// <summary>
        /// BGMを停止する
        /// </summary>
        /// <param name="fadeTime">フェードアウトにかかる時間（秒）</param>
        public void StopBGM(float fadeTime = 0.5f) {
            if (_bgmSource == null || !_bgmSource.isPlaying)
                return;

            // 既存のフェードを停止
            DisposeCancellation();

            if (fadeTime > 0) {
                FadeOutBGM(fadeTime, CreateNewCancellation()).Forget();
            }
            else {
                _bgmSource.Stop();
                _currentBGMKey = null;
            }
        }

        private async UniTask FadeOutBGM(float fadeTime, CancellationToken token) {

            // フェードアウト
            await _bgmSource.DOFade(0, fadeTime)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true)  // タイムスケール影響なし
                .ToUniTask(cancellationToken: token);

            _bgmSource.Stop();
            _currentBGMKey = null;
        }

        /// <summary>
        /// 現在のBGM情報を取得
        /// </summary>
        public (string key, float currentVolume) GetCurrentBGMInfo() {
            return (_currentBGMKey, _bgmSource.volume);
        }
        #endregion

        #region SE操作
        /// <summary>
        /// SEを再生する
        /// </summary>
        /// <param name="seKey">SEの識別子</param>
        /// <param name="volumeScale">音量スケーリング（0-1）</param>
        /// <returns>SEを再生しているAudioSourceのID（停止などに使用）</returns>
        public int PlaySE(string seKey, float volumeScale = 1f) {
            if (string.IsNullOrEmpty(seKey) || _seTable == null)
                return -1;

            // SE取得
            AudioClip clip = GetSEClip(seKey);
            if (clip == null) {
                Debug.LogWarning($"指定されたSE: {seKey} が見つかりません");
                return -1;
            }

            // 音量計算
            float finalVolume = _seMasterVolume * volumeScale;
            if (_seTable.TryGetItem(seKey, out SEData seData)) {
                finalVolume *= seData.Volume;
            }

            // AudioSource取得と再生
            AudioSource source = GetAvailableSESource();
            if (source == null) {
                Debug.LogWarning("利用可能なAudioSourceがありません");
                return -1;
            }

            source.clip = clip;
            source.volume = finalVolume;
            source.Play();

            // 再生終了後に自動でプールに戻す
            ReturnToPoolWhenFinished(source, clip.length, CreateNewCancellation()).Forget();

            return source.GetInstanceID();
        }

        /// <summary>
        /// 再生中のSEを停止する
        /// </summary>
        /// <param name="sourceId">PlaySEで返されたID</param>
        public void StopSE(int sourceId) {
            AudioSource source = _seSourcePool.FirstOrDefault(s => s.GetInstanceID() == sourceId);
            if (source != null) {
                source.Stop();
                ReturnSESourceToPool(source);
            }
        }

        /// <summary>
        /// SEをフェードアウトして停止する
        /// </summary>
        /// <param name="sourceId">PlaySEで返されたID</param>
        /// <param name="fadeTime">フェードアウト時間</param>
        public void FadeOutSE(int sourceId, float fadeTime = 0.5f) {
            AudioSource source = _seSourcePool.FirstOrDefault(s => s.GetInstanceID() == sourceId);
            if (source != null && source.isPlaying) {
                var token = CreateNewCancellation();
                FadeOutAndStopSE(source, fadeTime, token).Forget();
            }
        }

        /// <summary>
        /// SEをフェードアウトして停止する内部処理
        /// </summary>
        private async UniTask FadeOutAndStopSE(AudioSource source, float fadeTime, CancellationToken token) {
            float startVolume = source.volume;

            try {
                // フェードアウト
                await source.DOFade(0, fadeTime)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(true)
                    .ToUniTask(cancellationToken: token);

                source.Stop();
                ReturnSESourceToPool(source);
            }
            catch (OperationCanceledException) {
                // キャンセルされた場合は何もしない
            }
        }

        /// <summary>
        /// SEマスター音量を設定
        /// </summary>
        /// <param name="volume">0-1の間の音量</param>
        public void SetSEMasterVolume(float volume) {
            _seMasterVolume = Mathf.Clamp01(volume);
        }

        /// <summary>
        /// SEのクリップを取得（キャッシュを活用）
        /// </summary>
        private AudioClip GetSEClip(string seKey) {
            // キャッシュにあればそれを返す
            if (_seClipCache.TryGetValue(seKey, out AudioClip cachedClip)) {
                return cachedClip;
            }

            // キャッシュになければテーブルから取得
            if (_seTable.TryGetItem(seKey, out SEData seData)) {
                AudioClip clip = seData.Clip;

                // プール可能なSEならキャッシュする
                if (seData.IsPoolable && clip != null) {
                    _seClipCache[seKey] = clip;
                }

                return clip;
            }

            return null;
        }

        /// <summary>
        /// 利用可能なSE用AudioSourceを取得
        /// </summary>
        private AudioSource GetAvailableSESource() {
            if (_availableSESourceQueue.Count > 0) {
                return _availableSESourceQueue.Dequeue();
            }

            // キューが空の場合、再生中の中で一番長く再生されているものを停止して使う
            AudioSource oldestSource = _seSourcePool.OrderBy(s => s.time / (s.clip?.length ?? 1)).FirstOrDefault();
            if (oldestSource != null) {
                oldestSource.Stop();
                return oldestSource;
            }

            return null;
        }

        /// <summary>
        /// 使用済みのAudioSourceをプールに戻す
        /// </summary>
        private void ReturnSESourceToPool(AudioSource source) {
            if (source != null && !_availableSESourceQueue.Contains(source)) {
                source.clip = null;
                _availableSESourceQueue.Enqueue(source);
            }
        }

        /// <summary>
        /// 再生終了後にAudioSourceをプールに戻す
        /// </summary>
        private async UniTask ReturnToPoolWhenFinished(AudioSource source, float clipLength, CancellationToken token) {
            await UniTask.Delay(TimeSpan.FromSeconds(clipLength), cancellationToken: token);
            ReturnSESourceToPool(source);
        }

        /// <summary>
        /// メモリから不要なSEを解放
        /// </summary>
        public void ClearSECache() {
            _seClipCache.Clear();
        }
        #endregion

        /// <summary>
        /// すべての音を停止
        /// </summary>
        public void StopAllSounds() {
            StopBGM(0);

            foreach (AudioSource source in _seSourcePool) {
                source.Stop();
                ReturnSESourceToPool(source);
            }
        }

        private CancellationToken CreateNewCancellation() {
            _fadeCancellation?.Cancel();
            _fadeCancellation?.Dispose();
            _fadeCancellation = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            return _fadeCancellation.Token;
        }

        private void DisposeCancellation() {
            _fadeCancellation?.Cancel();
            _fadeCancellation?.Dispose();
            _fadeCancellation = null;
        }

        public void SaveVolumeSettings() {
            var volumeData = new AudioVolume {
                BGMVolume = _bgmMasterVolume,
                SEVolume = _seMasterVolume
            };
            _saveDataManager.Save(_saveDataKey, volumeData);
        }

        private void OnDestroy() {
            StopAllSounds();
            ClearSECache();
            Instance = null;
        }

        [Serializable]
        private class AudioVolume {
            public float BGMVolume = 0.5f;
            public float SEVolume = 0.5f;
        }


    }
}
