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
        /// �쳣��Ŵ���ʱ��Ĭ�ϱ��
        /// </summary>
        public static int EXCEPTION_DEFAULT_TYPE = 9999;
      
        /// <summary>
        /// �쳣��Ϣxml, ���·��������·��������
        /// </summary>
        public static string EXCEPTION_INFO_XML = "exceptionInfo.xml";
        /// <summary>
        /// �쳣��Ϣ�ֵ� Dictionary<string,string>
        /// </summary>
        public static Dictionary<int, string> EXCEPTION_INFO_DICTIONARY = null;//new Dictionary<int, string>();
        /// <summary>
        /// ������Ŀ¼
        /// </summary>
        public static string PROJECT_WORKSPACE = AppDomain.CurrentDomain.BaseDirectory;
       
        /// <summary>
        /// ��־�ļ���
        /// </summary>
        public static string LOG_ROOT_DIR = "\\logs";
        
        /// <summary>
        /// ִ���ڼ������LogException����
		/// ���ܲ�׼ȷ����batִ�У�cmdִ���з����Ĵ����޷���ȡ
        /// </summary>
        public static List<LogException> RUNTIME_LOGEXCEPTION_LIST=new List<LogException>();

        #endregion
    }
}
