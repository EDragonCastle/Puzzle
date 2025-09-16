using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class InitalizeSetting : MonoBehaviour
{
    public async UniTask Load(IProgress<float> progress = null)
    {
        const int index = 3;
        int loadingNumber = 1;
        ResourceManager resourceManager = new ResourceManager();
        await resourceManager.Load();
        progress.Report((float)loadingNumber / index);
        loadingNumber++;

        AdManager adManager = new AdManager();
        adManager.LoadRewardedAd();
        progress.Report((float)loadingNumber / index);
        loadingNumber++;

        EDCServer server = new EDCServer();
        await server.InitalizeFirebase();
        progress.Report((float)loadingNumber / index);

        Locator<EDCServer>.Provide(server);
        Locator<AdManager>.Provide(adManager);
        Locator<ResourceManager>.Provide(resourceManager);
    }
}
