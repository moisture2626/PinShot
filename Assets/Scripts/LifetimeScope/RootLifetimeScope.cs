using PinShot.Singletons;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PinShot.DI {
    public class RootLifetimeScope : LifetimeScope {
        [SerializeField] private MasterDataManager _masterDataManager;
        [SerializeField] private SaveDataManager _saveDataManager;
        [SerializeField] private SoundManager _soundManager;
        protected override void Configure(IContainerBuilder builder) {
            Debug.Log("Starting registration in RootLifeTimeScope");
            builder.RegisterComponent(_masterDataManager);
            builder.RegisterComponent(_saveDataManager);
            builder.RegisterComponent(_soundManager);
        }
    }

}
