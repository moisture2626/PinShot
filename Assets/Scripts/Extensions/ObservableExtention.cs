using System;
using System.Threading;
using System.Threading.Tasks;
using R3;
using UnityEngine;

namespace PinShot.Extensions {
    public static class ObservableExtensions {

        /// <summary>
        /// Observableの購読（非同期）
        /// 連打されても前の処理が終わるまで次の処理を破棄する
        /// </summary>
        /// <param name="source"></param>
        /// <param name="addToComponent"></param>
        /// <param name="onNext"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IDisposable SubscribeAwait<T>(this Observable<T> source, MonoBehaviour addToComponent, Func<T, CancellationToken, ValueTask> onNext) {
            var entity = new Entity<bool>(false);
            return source.SubscribeAwait(
                addToComponent,
                entity,
                onNext
            );
        }
        /// <summary>
        /// Observableの購読（非同期）
        /// entityを共有していれば同時押しできない
        /// </summary>
        /// <param name="source"></param>
        /// <param name="addToComponent"></param>
        /// <param name="onNext"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IDisposable SubscribeAwait<T>(this Observable<T> source, MonoBehaviour addToComponent, Entity<bool> entity, Func<T, CancellationToken, ValueTask> onNext) {
            var subscription = source.SubscribeAwait(entity, onNext);

            if (addToComponent != null) {
                subscription.RegisterTo(addToComponent.destroyCancellationToken);
            }
            return subscription;
        }

        /// <summary>
        /// Observableの購読（非同期）
        /// entityを共有していれば同時押しできない
        /// </summary>
        /// <param name="source"></param>
        /// <param name="entity"></param>
        /// <param name="onNext"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IDisposable SubscribeAwait<T>(this Observable<T> source, Entity<bool> entity, Func<T, CancellationToken, ValueTask> onNext) {

            var subscription = source.SubscribeAwait(async (ev, t) => {
                if (entity.Value) {
                    return;
                }
                entity.Value = true;
                try {
                    await onNext(ev, t);
                }
                catch (OperationCanceledException) {
                    throw;
                }
                catch (Exception e) {
                    Debug.LogError(e);
                    throw e;
                }
                finally {
                    entity.Value = false;
                }
            });

            return subscription;
        }
    }
}
