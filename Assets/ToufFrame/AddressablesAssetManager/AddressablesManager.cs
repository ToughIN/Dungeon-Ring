using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToufFrame
{
    public class AddressablesAssetManager : SingletonBase<AddressablesAssetManager>
    {
        private Dictionary<string, BaseLoader> loaders = new Dictionary<string, BaseLoader>();

        public T LoadAsset<T>(string assetKey) where T : Object
        {
            if (!loaders.TryGetValue(assetKey, out BaseLoader loader))
            {
                loader = new BaseLoader(assetKey);
                loaders[assetKey] = loader;
            }

            return loader.LoadAsset<T>();
        }
        
        public async Task LoadAssetAsync<T>(string assetKey, System.Action<T> onLoaded) where T : Object
        {
            if (!loaders.TryGetValue(assetKey, out BaseLoader loader))
            {
                loader = new BaseLoader(assetKey);
                loaders[assetKey] = loader;
            }

            T result = await loader.LoadAssetAsync<T>();
            if (typeof(T) == typeof(GameObject))
            {
                // 实例化预制体
                var instantiated = Object.Instantiate(result as GameObject);
                onLoaded?.Invoke(instantiated as T);
            }
            else
            {
                // 直接返回非 GameObject 类型的资源
                onLoaded?.Invoke(result);
            }
        }

        public void ReleaseAsset(string assetKey)
        {
            if (loaders.TryGetValue(assetKey, out BaseLoader loader))
            {
                loader.Release();
            }
        }

    }

}