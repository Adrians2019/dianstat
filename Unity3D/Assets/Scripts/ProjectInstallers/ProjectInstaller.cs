using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        InstallAuthSignals();

        Container.BindInterfacesAndSelfTo<RegionProxy>().FromNewComponentOnNewGameObject().WithGameObjectName("RegionProxy").AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AppRegion>().FromNewComponentOnNewGameObject().WithGameObjectName("AppRegion").AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<UnitProxy>().FromNewComponentOnNewGameObject().WithGameObjectName("UnitProxy").AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AppUnit>().FromNewComponentOnNewGameObject().WithGameObjectName("AppUnit").AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<ImgProxy>().FromNewComponentOnNewGameObject().WithGameObjectName("ImgProxy").AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AppImg>().FromNewComponentOnNewGameObject().WithGameObjectName("AppImg").AsSingle().NonLazy();


    }

    void InstallAuthSignals()
    {
        // each of Home, Unit and Region scenes fire this event, submitting a url
        Container.DeclareSignal<HomeInitEvt>();
        Container.DeclareSignal<QueryTextureEvt>();
        
        Container.DeclareSignal<ImgUploadPermissionEvt>();
        Container.DeclareSignal<ImgUploadRevokePermissionEvt>();
        

        Container.DeclareSignal<UrlsUpdateEvt>();

        Container.DeclareSignal<UnitLatestDSUpdateEvt>();
        Container.DeclareSignal<UnitInitEvt>();
        Container.DeclareSignal<ReqSaveUnitDSEvt>();

        // unit ctrl:
        Container.DeclareSignal<UnitOriginPointEvt>();
        Container.DeclareSignal<UnitTerminusPointEvt>();
        Container.DeclareSignal<UnitLineImgUpdateEvt>();

        Container.DeclareSignal<RegionInitEvt>();
        Container.DeclareSignal<NoUrlImgEvt>();
        Container.DeclareSignal<AppImgCurUrlImgEvt>();
        Container.DeclareSignal<ReqRegionDSSaveEvt>();

        Container.DeclareSignal<RegionUpdatedEvt>();
        Container.DeclareSignal<RegionsDownloaded>();
    }
}