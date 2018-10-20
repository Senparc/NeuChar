using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Senparc.NeuChar
{
    /// <summary>
    /// 数据处理引擎
    /// </summary>
    public class DataEngine
    {
        /// <summary>
        /// 文件路径（相对路径）
        /// </summary>
        public string FullFilePath { get; set; }

        /// <summary>
        /// NeuralSystem 单例对象
        /// </summary>
        public NeuralSystem NeuralSystem { get { return NeuralSystem.Instance; } }

        /// <summary>
        /// 数据处理引擎 构造函数
        /// </summary>
        /// <param name="filePath">文件相对路径</param>
        public DataEngine(string filePath = "~/App_Data/NeuChar")
        {
              FullFilePath = filePath;
        }
    }
}
