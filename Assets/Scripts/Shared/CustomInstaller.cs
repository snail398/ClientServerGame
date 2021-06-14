using Zenject;

public class CustomInstaller : MonoInstaller
{
    protected void InstallService<TService>()
    {
        Container.BindInterfacesAndSelfTo<TService>().AsSingle();
        var service = Container.Resolve<TService>();
        (service as ILoadable)?.Load();
    }

    protected void InstallAbstractService<TAbstract, TService>() where TService : TAbstract
    {
        Container.Bind<TAbstract>().To<TService>().AsSingle();
        var service = Container.Resolve<TAbstract>();
        (service as ILoadable)?.Load();
    }
}
