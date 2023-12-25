namespace ToufFrame
{
    public class SingletonBase<T> where T : new()
    {
        // 使用一个静态变量来保存类的实例
        private static T _instance;

        // 锁对象，用于确保线程安全
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                // 双重检查锁定以确保线程安全
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }

                return _instance;
            }
        }

        // 保护构造方法，防止外部实例化
        protected SingletonBase() { }
    }
}