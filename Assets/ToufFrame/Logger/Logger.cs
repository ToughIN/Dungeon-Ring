
using UnityEngine;

namespace ToufFrame
{
    public class TLogger : MonoSingletonBase<TLogger>
    {
        // 打印普通信息
        public void Log(string message)
        {
            Debug.Log("[INFO]"+message);
        }

        // 打印警告信息
        public void Warn(string message)
        {
            Debug.LogWarning("[WARN]"+message);
        }

        // 打印错误信息
        public void Error(string message)
        {
            Debug.LogError("[ERROR]"+message);
        }
    }
}