using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace LckeyAnt
{
	/// <summary>
	/// �쳣�ֵ���Ϣ������
	/// </summary>
	public class ExceptionAccess
	{
		/// <summary>
		/// ��ʼ���쳣��Ϣ�ֵ䣬ȡ������key,value;��һ��ʹ��ʱ�Զ���ʼ���ֵ���Ϣ
		/// </summary>
		public static void initExceptionInfo() {
			try {
				//xml·��
				string exceptionInfoPath = Path.Combine(GlobalCurrent.PROJECT_WORKSPACE, GlobalCurrent.EXCEPTION_INFO_XML);
				if (!File.Exists(exceptionInfoPath)) {
					return;
				}

				Dictionary<int, string> exInfoDic = new Dictionary<int, string>();

				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(exceptionInfoPath);

				XmlNode exceptionsNode = xmlDoc.SelectSingleNode("Exceptions");
				XmlNodeList exceptionNodeList = exceptionsNode.ChildNodes;
				for (int i = 0; i < exceptionNodeList.Count; i++) {
					XmlNode exceptionNode = exceptionNodeList[i];
					int exKey = GlobalCurrent.EXCEPTION_DEFAULT_TYPE;
					string exKeyText = exceptionNode["key"].InnerText;
					string exValue = exceptionNode["value"].InnerText;
					Int32.TryParse(exKeyText, out exKey);
					//if (exInfoDic.ContainsKey(exKey)) {
					exInfoDic[exKey] = exValue;
					//} else {
					//	exInfoDic.Add(exKey, exValue);
					//}
				}
				GlobalCurrent.EXCEPTION_INFO_DICTIONARY = exInfoDic;
			} catch (Exception ex) {
				throw new LogException(ex.StackTrace, ex).setLogEx(9999, "��ʼ���쳣��Ϣ�б��쳣");
			}
		}
		/// <summary>
		/// �����쳣keyȡ��value
		/// </summary>
		/// <param name="exKey">�쳣key</param>
		/// <returns>�쳣value</returns>
		public static string getExValueByKey(int exKey) {
			if (GlobalCurrent.EXCEPTION_INFO_DICTIONARY != null) {
				if (GlobalCurrent.EXCEPTION_INFO_DICTIONARY.ContainsKey(exKey)) {
					return GlobalCurrent.EXCEPTION_INFO_DICTIONARY[exKey];
				}
			} else {
				initExceptionInfo();
				if (GlobalCurrent.EXCEPTION_INFO_DICTIONARY.ContainsKey(exKey)) {
					return GlobalCurrent.EXCEPTION_INFO_DICTIONARY[exKey];
				}
			}
			//û���ҵ�����null
			return "null";
		}
	}
}
