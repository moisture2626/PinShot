using Cysharp.Threading.Tasks;
using PinShot.Extensions;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PinShot.Event {
    /// <summary>
    /// イベント用のInterface
    /// </summary>
    public interface IEvent {

    }

    /// <summary>
    /// ゲームで起きたイベント内容を通知したりするクラス
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public class EventManager<TEvent> : IDisposable where TEvent : struct, IEvent {
        private static EventManager<TEvent> _instance;
        private Subject<TEvent> _subject;
        private static readonly object _lock = new object();
        private TEvent _lastEvent;
        public static TEvent LastEvent => GetInstance()._lastEvent;

        /// <summary>
        /// インスタンスを取得
        /// </summary>
        /// <returns></returns>
        private static EventManager<TEvent> GetInstance() {
            if (_instance == null) {
                // スレッドセーフにインスタンスを生成(念の為)
                lock (_lock) {
                    if (_instance == null) {
                        _instance = new EventManager<TEvent>();
                        _instance._subject = new Subject<TEvent>();
                    }
                }
            }
            return _instance;
        }

        /// <summary>
        /// イベントを発火
        /// </summary>
        /// <param name="newEvent"></param>
        /// <typeparam name="TEvent"></typeparam>
        public static void TriggerEvent(TEvent newEvent) {
            var instance = GetInstance();
            instance._lastEvent = newEvent;
            instance._subject?.OnNext(newEvent);
        }

        /// <summary>
        /// イベントの購読
        /// </summary>
        /// <param name="addToComponent"></param>
        /// <param name="onNext"></param>
        /// <returns></returns>
        public static IDisposable Subscribe(MonoBehaviour addToComponent, Action<TEvent> onNext) {
            var subscription = Subscribe(onNext);
            if (addToComponent != null) {
                subscription.RegisterTo(addToComponent.destroyCancellationToken);
            }
            return subscription;
        }
        public static IDisposable Subscribe(Action<TEvent> onNext) {
            var instance = GetInstance();
            var subject = instance._subject;

            var subscription = subject.Subscribe(ev => {
                onNext(ev);
            });
            return subscription;
        }

        /// <summary>
        /// イベントを購読(async)
        /// </summary>
        /// <param name="addToComponent"></param>
        /// <param name="onNext"></param>
        /// <returns></returns>
        public static IDisposable SubscribeAwait(MonoBehaviour addToComponent, Func<TEvent, CancellationToken, ValueTask> onNext) {
            return SubscribeAwait(onNext).AddTo(addToComponent);
        }
        
        public static IDisposable SubscribeAwait(Func<TEvent, CancellationToken, ValueTask> onNext) {
            var instance = GetInstance();
            var subject = instance._subject;
            var entity = new Entity<bool>(false);
            return subject.SubscribeAwait(entity, onNext);
        }

        /// <summary>
        /// 指定した状態のイベントが発火するまで待機する
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="token"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async UniTask WaitForEvent(Func<TEvent, bool> predicate, CancellationToken token, TimeSpan? timeout = null) {
            var instance = GetInstance();
            var subject = instance._subject;

            var tcs = new UniTaskCompletionSource();
            var disposable = subject.Subscribe(e => {
                if (predicate(e)) {
                    tcs.TrySetResult();
                }
            }).RegisterTo(token);

            // タイムアウトが指定されている場合
            if (timeout.HasValue) {
                var timeoutTask = UniTask.Delay(timeout.Value, cancellationToken: token);
                var resultTask = UniTask.WhenAny(tcs.Task, timeoutTask);
                var winner = await resultTask;
                if (winner == 1) {
                    // タイムアウトした場合
                    throw new TimeoutException("WaitForEvent timed out");
                }
            }
            else {
                await tcs.Task;
            }
            disposable.Dispose();
        }

        public void Dispose() {
            _subject?.Dispose();
            if (_instance != null) {
                _instance = null;
            }
        }

    }
}
