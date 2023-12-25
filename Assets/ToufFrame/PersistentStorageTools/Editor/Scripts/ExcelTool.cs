using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Excel;
using UnityEditor;
using UnityEngine;
using LitJson;

namespace ToufFrame
{
    public class ExcelTool
    {
        
        public static string EXCEL_PATH = Application.dataPath + "/BinarySaveLoadTool/Excels";

        public static string DATA_CLASS_PATH = Application.dataPath + "/BinarySaveLoadTool/";

        public static string DATA_CONTAINER_PATH = Application.dataPath + "/BinarySaveLoadTool/";

        public static string DATA_JSON_PATH = Application.streamingAssetsPath+ "/JsonData/";
       

        public static int BEGIN_INDEX = 4;
        
        //官网专门提供了dll文件来解析Excel文件
        
        //打开Excel表

        

        [MenuItem("GameTool/GenerateExcel")]
        private static void GenerateExcelInfo()
        {
            DirectoryInfo directoryInfo = Directory.CreateDirectory(EXCEL_PATH);
            FileInfo[] files = directoryInfo.GetFiles();
            DataTableCollection resultCollections;
            for (int i = 0; i < files.Length; i++)
            {
                if(files[i].Extension!=".xlsx" && files[i].Extension !=".xls")
                    continue;
                using (FileStream fs = files[i].Open(FileMode.Open, FileAccess.Read))
                {
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                    resultCollections = excelReader.AsDataSet().Tables;
                }

                foreach (DataTable table in resultCollections)
                {
                    //生成数据结构类
                    GenerateExcelDSClass(table);
                    //生成容器类
                    GenerateExcelContainer(table);
                    //生成2进制
                    GenerateExcelBinary(table);
                    //生成Json文件
                    GenerateExcelJson(table);
                }
                
                
            }
            
        }

       
        /// <summary>
        /// 从Excel生成数据结构类
        /// </summary>
        /// <param name="table"></param>
        private static void GenerateExcelDSClass(DataTable table)
        {
            //字段名行
            DataRow rowName = GetVariableNameRow(table);
            DataRow rowType = GetVariableTypeRow(table);
            if (!Directory.Exists(DATA_CLASS_PATH))
                Directory.CreateDirectory(DATA_CLASS_PATH);
            string classScriptStr =  "using System;\n"+
                                     "namespace BinarySaveLoadTool"+
                                     "\n{\n"+
                                     "[Serializable]\n" + 
                                     "public class " + table.TableName + "\n{\n";
            //变量进行字符串拼接
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (string.IsNullOrEmpty(rowType[i].ToString()) || string.IsNullOrEmpty(rowName[i].ToString()))continue;
                classScriptStr += "  public " + rowType[i].ToString() + " " + rowName[i].ToString()+";\n";
                
            }
            classScriptStr += "}\n}";
            

            File.WriteAllText(DATA_CLASS_PATH+table.TableName+".cs",classScriptStr);
            //刷新Project
            AssetDatabase.Refresh();
        }
        
        
        /// <summary>
        /// 生成数据类容器
        /// </summary>
        /// <param name="table"></param>
        private static void GenerateExcelContainer(DataTable table)
        {
            //得到主键索引
            int keyIndex = GetKeyIndex(table);
            
            //得到字段类型行
            DataRow rowType = GetVariableTypeRow(table);
            
            //没有路径创建路径
            if (!Directory.Exists(DATA_CONTAINER_PATH))
                Directory.CreateDirectory(DATA_CONTAINER_PATH);
            string str = "using System.Collections.Generic;\n";
            str += "using System;\n";
            str += "namespace BinarySaveLoadTool\n{\n";
            str += "[Serializable]\n";
            str += "public class " + table.TableName + "Container" + "\n{\n";

            str += "  ";
            str += "public Dictionary<" + rowType[keyIndex].ToString() + ", " + table.TableName + ">";
            str += "dataDic = new " + "Dictionary<" + rowType[keyIndex].ToString() + ", " + table.TableName + ">();\n";

            str += "}\n}";
            File.WriteAllText(DATA_CONTAINER_PATH+table.TableName+"Container.cs",str);
            AssetDatabase.Refresh();

        }
        
        
        /// <summary>
        /// 生成二进制文件
        /// </summary>
        private static void GenerateExcelBinary(DataTable table)
        {
            if (!Directory.Exists(BinaryDataMgr. DATA_BINARY_PATH)) Directory.CreateDirectory(BinaryDataMgr.DATA_BINARY_PATH);
            //创建一个2进制文件写入
            using (FileStream fs = new FileStream(BinaryDataMgr.DATA_BINARY_PATH + table.TableName + BinaryDataMgr.DATA_BINARY_SUFFIX, FileMode.OpenOrCreate,
                       FileAccess.Write))
            {
                //前几行不是数据，先写入需要读取的行数
                fs.Write(BitConverter.GetBytes(table.Rows.Count-BEGIN_INDEX),0,4);
                
                //再写入主键变量名的字符长度和二进制内容
                string keyName = GetVariableNameRow(table)[GetKeyIndex(table)].ToString();
                byte[] bytes = Encoding.UTF8.GetBytes(keyName);
                fs.Write(BitConverter.GetBytes(bytes.Length),0,4);
                fs.Write(bytes,0,bytes.Length);

                DataRow row;
                DataRow rowType = GetVariableTypeRow(table);
                for (int i = BEGIN_INDEX; i < table.Rows.Count; i++)
                {
                    row = table.Rows[i];
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        switch (rowType[j].ToString())
                        {
                            case "int":
                            {
                                if (int.TryParse(row[j].ToString(), out int intValue))
                                {
                                    fs.Write(BitConverter.GetBytes(intValue), 0, 4);
                                }
                                else
                                {
                                    fs.Write(BitConverter.GetBytes(0), 0, 4); // 写入默认值0
                                    Debug.Log("Cannot convert " + row[j].ToString() + " to int, defaulting to 0");
                                }
                            }
                                break;
                            case "float":
                            {
                                if (float.TryParse(row[j].ToString(), out float floatValue))
                                {
                                    fs.Write(BitConverter.GetBytes(floatValue), 0, 4);
                                }
                                else
                                {
                                    fs.Write(BitConverter.GetBytes(0.0f), 0, 4); // 写入默认值0.0f
                                    Debug.Log("Cannot convert " + row[j].ToString() + " to float, defaulting to 0.0");
                                }
                            }
                                break;
                            case"bool":
                                fs.Write(BitConverter.GetBytes(bool.Parse(row[j].ToString())),0,1);
                                break;
                            case "string":
                                bytes = Encoding.UTF8.GetBytes(row[j].ToString());
                                //写入长度
                                fs.Write(BitConverter.GetBytes(bytes.Length),0,4);
                                fs.Write(bytes,0,bytes.Length);
                                break;
                            
                        }
                    }
                }
            }
            AssetDatabase.Refresh();
        }
        
        
        /// <summary>
        /// 生成Json文件
        /// </summary>
        /// <param name="table"></param>
        private static void GenerateExcelJson(DataTable table)
        {
            string jsonFilePath = DATA_JSON_PATH;

            // 如果文件夹不存在，则创建文件夹
            string directory = Path.GetDirectoryName(jsonFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 创建一个Dictionary来存储所有行的数据,这里key用string存了
            Dictionary<string, object> allRows = new Dictionary<string, object>();

            // 获取主键索引
            int keyIndex = GetKeyIndex(table);
            DataRow rowType = GetVariableTypeRow(table);

            for (int i = BEGIN_INDEX; i < table.Rows.Count; i++)
            {
                // 获取每一行的数据
                DataRow row = table.Rows[i];
        
                // 创建一个Dictionary来存储每一行的数据
                Dictionary<string, object> rowData = new Dictionary<string, object>();

                for (int j = 0; j < table.Columns.Count; j++)
                {
                    object val = row[j];
            
                    // 获取每一列的列名
                    string colName = table.Columns[j].ColumnName;
            
                    switch (rowType[j].ToString())
                    {
                        case "int":
                            if (int.TryParse(row[j].ToString(), out int inValue))
                            {
                                rowData[colName] = inValue;
                            }
                            else
                            {
                                rowData[colName] = 0;
                                Debug.Log("Cannot convert " + row[j].ToString() + " to int, defaulting to 0");
                            }
                            break;
                        case "float":
                            if(float.TryParse(row[j].ToString(),out float floatValue))
                            {
                                rowData[colName] = floatValue;
                            }
                            else
                            {
                                rowData[colName] = 0.0f;
                                Debug.Log("Cannot convert " + row[j].ToString() + " to float, defaulting to 0.0");
                            }
                            break;
                        case "bool":
                            rowData[colName] = bool.Parse(val.ToString());
                            break;
                        case "string":
                            rowData[colName] = val.ToString();
                            break;
                    }
                }

                allRows[row[keyIndex].ToString()] = rowData;
            }

            // 使用 LitJson 库来序列化
            string jsonStr = JsonMapper.ToJson(allRows);

            // 将JSON字符串写入到文件
            File.WriteAllText(jsonFilePath + table.TableName + ".json", jsonStr);

            // 刷新AssetDatabase以在Unity编辑器中显示新文件
            UnityEditor.AssetDatabase.Refresh();
        }
        
        
        /// <summary>
        /// 获取变量名所在行
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static  DataRow GetVariableNameRow(DataTable table)
        {
            return table.Rows[0];
        }

       
        /// <summary>
        /// 获取变量类型所在行
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static DataRow GetVariableTypeRow(DataTable table)
        {
            return table.Rows[1];
        }

        
       

       
        private static int GetKeyIndex(DataTable table)
        {
            DataRow row = table.Rows[2];
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (row[i].ToString() == "key") return i;
            }

            return 0;
        }

      
        
        [MenuItem("GameTool/Open Excel(Test)")]
        private static void OpenExcel()
        {
            using (FileStream fs = File.Open(Application.dataPath + EXCEL_PATH+ "PlayerInfo.xlsx", FileMode.Open,
                       FileAccess.Read))
            {
                //通过文件流获得excel数据
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                //将excel表中的数据转换为DataSet数据类型，方便我们获取其中的内容
                DataSet result = excelDataReader.AsDataSet();
                //得到所有表信息,空表不会遍历
                for (int i = 0; i < result.Tables.Count; i++)
                {
                    //result.Tables[i].TableName;
                    //result.Tables[i].Rows.Count;
                    //result.Tables[i].Columns.Count;

                }
                fs.Close();
            }
            
        }

        [MenuItem("GameTool/Read Excel(Test)")]
        private static void ReadExcel()
        {
            using (FileStream fs = File.Open(Application.dataPath + "Excel/PlayerInfo.xlsx", FileMode.Open,
                       FileAccess.Read))
            {
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                //将excel表中的数据转换为DataSet数据类型，方便我们获取其中的内容
                DataSet result = excelDataReader.AsDataSet();
                
                //每张表读行列数据就差不多是这种感觉
                for (int i = 0; i < result.Tables.Count; i++)
                {
                    DataTable table = result.Tables[i];
                    DataRow row = table.Rows[0];
                    Debug.Log(row[1].ToString());
                }
            }
        }

        
        
    }
}