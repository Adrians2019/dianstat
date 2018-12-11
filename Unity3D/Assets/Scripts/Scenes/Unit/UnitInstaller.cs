using UnityEngine;
using Zenject;

public class UnitInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InstallAuthSignals();
        Container.BindInterfacesAndSelfTo<UnitFSM>().AsSingle().NonLazy();
    }
    void InstallAuthSignals()
    {

    }
}