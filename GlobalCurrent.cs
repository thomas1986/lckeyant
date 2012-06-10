using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LckeyAnt
{
    public class GlobalCurrent
    {
        #region var
      
        /// <summary>
        /// 异常编号错误时的默认编号
        /// </summary>
        public static int EXCEPTION_DEFAULT_TYPE = 9999;
      
        /// <summary>
        /// 异常信息xml, 相对路径，绝对路径都可以
        /// </summary>
        public static string EXCEPTION_INFO_XML = "exceptionInfo.xml";
        /// <summary>
        /// 异常信息字典 Dictionary<string,string>
        /// </summary>
        public static Dictionary<int, string> EXCEPTION_INFO_DICTIONARY = null;//new Dictionary<int, string>();
        /// <summary>
        /// 工作根目录
        /// </summary>
        public static string PROJECT_WORKSPACE = AppDomain.CurrentDomain.BaseDirectory;
       
        /// <summary>
        /// 日志文件夹
        /// </summary>
        public static string LOG_ROOT_DIR = "\\logs";
        
        /// <summary>
        /// 执行期间产生的LogException集合
		/// 可能不准确，如bat执行，cmd执行中发生的错误无法获取
        /// </summary>
        public static List<LogException> RUNTIME_LOGEXCEPTION_LIST=new List<LogException>();

        #endregion
    }
}
