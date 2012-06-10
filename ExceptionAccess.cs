using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace LckeyAnt
{
	/// <summary>
	/// 异常字典信息处理类
	/// </summary>
	public class ExceptionAccess
	{
		/// <summary>
		/// 初始化异常信息字典，取得所有key,value;第一次使用时自动初始化字典信息
		/// </summary>
		public static void initExceptionInfo() {
			try {
				//xml路径
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
				throw new LogException(ex.StackTrace, ex).setLogEx(9999, "初始化异常信息列表异常");
			}
		}
		/// <summary>
		/// 根据异常key取得value
		/// </summary>
		/// <param name="exKey">异常key</param>
		/// <returns>异常value</returns>
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
			//没有找到返回null
			return "null";
		}
	}
}
