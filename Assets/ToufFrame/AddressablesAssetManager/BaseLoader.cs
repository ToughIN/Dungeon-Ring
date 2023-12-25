using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ToufFrame
{
    public class BaseLoader
    {
        private string assetKey;
        private AsyncOperationHandle handle;
        private bool isLoaded;

        public BaseLoader(string key)
        {
            assetKey = key;
        }
        
        public T LoadAsset<T>() where T : Object
        {
            if (!isLoaded)
            {
                handle = Addressables.LoadAssetAsync<T>(assetKey);
                handle.WaitForCompletion(); // 注意：这可能会阻塞主线程，谨慎使用
                isLoaded = true;
            }

            return handle.Result as T;
        }

        public async Task<T> LoadAssetAsync<T>() where T : Object
        {
            if (!isLoaded)
            {
                handle = Addressables.LoadAssetAsync<T>(assetKey);
                await handle.Task;
                isLoaded = true;
            }

            return handle.Result as T;
        }
        
        public void Release()
        {
            if (isLoaded)
            {
                Addressables.Release(handle);
                isLoaded = false;
            }
        }

        // 根据需要添加其他方法，比如异步加载、对象池支持等。
    }
}