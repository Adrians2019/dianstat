using UnityEngine;
using Zenject;

public class HomeInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InstallAuthSignals();
    }

    void InstallAuthSignals()
    {
        
    }
}