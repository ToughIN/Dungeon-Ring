using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using UnityEngine;

namespace ToufFrame
{
    public class BinaryDataMgr
    {
        //从Excel中读取的数据生成的二进制文件保存路径
        public static string DATA_BINARY_PATH = Application.streamingAssetsPath + "/BinaryData/";
        
        //保存对象二进制文件路径
        public static string SAVE_PATH = Application.persistentDataPath + "/BinaryData/";
        
        //生成二进制文件后缀名
        public static string DATA_BINARY_SUFFIX = ".touf";
        
        
        private static BinaryDataMgr instance=new BinaryDataMgr();
        public static BinaryDataMgr Instance => instance;

        private BinaryDataMgr()
        {
            
        }

        
        /// <summary>
        /// 用于存储所有Excel表数据的容器
        /// </summary>
        private Dictionary<string, object> tableDic = new Dictionary<string, object>();

        
        
        
        

        /// <summary>
        /// 从二进制文件中读表
        /// </summary>
        /// <typeparam name="T">容器类名</typeparam>
        /// <typeparam name="K">数据结构体类名</typeparam>
        public void LoadTable<T,K>()
        {
            using (FileStream fs = File.Open(DATA_BINARY_PATH+typeof(K).Name+DATA_BINARY_SUFFIX,FileMode.Open,FileAccess.Read))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();

                //二进制中对数据指针
                int index = 0;

                //表示总共读取多少行
                int count = BitConverter.ToInt32(bytes, index);
                index += 4;

                //读取主键的名字
                int keyNameLength = BitConverter.ToInt32(bytes, index);
                index += 4;
                string keyName = Encoding.UTF8.GetString(bytes, index, keyNameLength);
                index += keyNameLength;
                
                //创建容器类对象
                Type containerType = typeof(T);
                object containeerObj = Activator.CreateInstance(containerType);
                //得到数据结构类的Type
                Type classType = typeof(K);
                //通过反射得到数据结构类的所有字段信息
                FieldInfo[] infos = classType.GetFields();
                //读取每一行的信息
                for (int i = 0; i < count;i++)
                {
                    //实例化一个数据结构类对象
                    object dataObj = Activator.CreateInstance(classType);
                    foreach (FieldInfo info in infos)
                    {
                        if (info.FieldType == typeof(int))
                        {
                            info.SetValue(dataObj,BitConverter.ToInt32(bytes,index));
                            index += 4;
                        }
                        else if (info.FieldType == typeof(float))
                        {
                            info.SetValue(dataObj,BitConverter.ToSingle(bytes,index));
                            index += 4;
                        }
                        else if (info.FieldType == typeof(bool))
                        {
                            info.SetValue(dataObj,BitConverter.ToBoolean(bytes,index));
                            index += 1;
                        }
                        else if (info.FieldType == typeof(string))
                        {
                            int length = BitConverter.ToInt32(bytes, index);
                            index += 4;
                            info.SetValue(dataObj,Encoding.UTF8.GetString(bytes,index,length));
                            index += length;

                        } 
                    }

                    //TODO:可以把生成的容器类的名字也改成静态字符串
                    //得到容器中的字典对象
                    object dicObj= containerType.GetField("dataDic").GetValue(containeerObj);
                    //得到字典对象的add方法
                    MethodInfo mInfo = dicObj.GetType().GetMethod("Add");
                    //得到数据结构类对象中 指定主键字段的值
                    object keyValue = classType.GetField(keyName).GetValue(dataObj);
                    mInfo.Invoke(dicObj, new object[] { keyValue, dataObj });
                }

                
                tableDic.Add(typeof(T).Name,containeerObj);
                

            }
        }

        /// <summary>
        /// 读取表的信息
        /// </summary>
        /// <typeparam name="T">容器类名</typeparam>
        /// <returns></returns>
        public T GetTable<T>() where T:class
        {
            string tableName = typeof(T).Name;
            if (tableDic.ContainsKey(tableName))
            {
                
                return tableDic[tableName] as T;
            }
            
            return null;
        }
        
        
        public void InitData()
        {
            LoadTable<PlayerInfoContainer,PlayerInfo>();
        }


        
        /// <summary>
        /// 二进制存储对象数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fileNmae"></param>
        public void Save(object obj, string fileNmae)
        {
            if (!Directory.Exists(SAVE_PATH))
                Directory.CreateDirectory(SAVE_PATH);

            using (FileStream fs = new FileStream(SAVE_PATH + fileNmae + DATA_BINARY_SUFFIX, FileMode.Create,
                       FileAccess.Write))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs,obj);
                fs.Close();
            }
        }
        
        /// <summary>
        /// 读取2进制数据转换成对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Load<T>(string fileName) where T : class
        {
            
            //如果不存在这个对象，就直接返回泛型对象的默认值
            if (!File.Exists(SAVE_PATH + fileName + DATA_BINARY_SUFFIX))
                return default(T);

            T obj;
            using (FileStream fs = File.Open(SAVE_PATH + fileName + DATA_BINARY_SUFFIX, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();
                obj = bf.Deserialize(fs) as T;
                fs.Close();
            }

            return obj;
        }
        

    }
}