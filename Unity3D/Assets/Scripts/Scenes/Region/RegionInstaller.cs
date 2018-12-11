using UnityEngine;
using Zenject;

public class RegionInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        //InstallRenderEngineItems();
        
        InstallSignals();
        InstallFSM();
    }

    private void InstallSignals() {
    }

    private void InstallFSM() {

        Container.BindInterfacesAndSelfTo<StateFactory>().AsSingle();

        Container.BindInterfacesAndSelfTo<CreateRegion>().AsSingle();
        Container.BindInterfacesAndSelfTo<RegionUD>().AsSingle();
        Container.BindInterfacesAndSelfTo<SegmentCUD>().AsSingle();

        Container.BindFactory<CreateRegion, CreateRegion.Factory>().WhenInjectedInto<StateFactory>();
        Container.BindFactory<RegionUD, RegionUD.Factory>().WhenInjectedInto<StateFactory>();
        Container.BindFactory<SegmentCUD, SegmentCUD.Factory>().WhenInjectedInto<StateFactory>();

    }
}