using Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectInstaller : CustomInstaller
{
    public override void InstallBindings()
    {
        var coroutineExecutor = new GameObject("CoroutineExecutor").AddComponent<CoroutineExecutor>();
        DontDestroyOnLoad(coroutineExecutor);
        Container.Bind<ICoroutineExecutor>().To<CoroutineExecutor>().FromInstance(coroutineExecutor).AsSingle();
        
        var unityEventProvider = new GameObject("UnityEventProvider").AddComponent<UnityEventProvider>();
        DontDestroyOnLoad(unityEventProvider);
        Container.Bind<UnityEventProvider>().FromInstance(unityEventProvider).AsSingle();

        Container.Bind<SignalBus>().AsSingle();
        Container.Bind<ResourceLoaderService>().AsSingle();
        Container.BindInterfacesAndSelfTo<SettingsService>().AsSingle();
        
        SceneManager.LoadScene("GameScene");
    }
}
