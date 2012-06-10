using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace LckeyAnt
{
	/** 合并js，修改html，script.src
	 * *修改合并文件src，是否需要配置每个页面需要合并的xml信息,确保一份js实例在页面只加载一次
	 * 压缩js文件，1对1修改名字 
	 * 合并多个js文件，多对1
	 * 合并多个js文件，1个页面中有多个合并，多对多
	 * 多对多的情况还是应该串行一个一个来同步执行
	 * ansycLoad,是否考虑合并
	 **根据xml,合并js源文件
	 * 
	 * *方法
	 * 列举所有<script js.src
	 * 列举所有 xx.js
	 * 
	 */
	public class MergeRoute
	{

		#region 根据xml进行完整的合并操作
		/*
		/// <summary>
		/// 一次完成所有操作，根据xml来合并
		/// </summary>
		/// <param name="xmlpath">xml路径</param>
		/// <param name="rootpath">xml中配置的节点文件的根目录</param>
		/// <param name="pageType">使用|分隔如*.html|*.aspx|*.*</param>
		public static void combinePagesItemsByXml(string xmlpath, string rootpath, string pageType) {

			string combineItems = getCombineFilesFromXml(xmlpath, rootpath);
			string pages = getHtmlFiles(rootpath, pageType);
			repMultiPagesMultiItems(pages, combineItems, "", "");
		}
		/// <summary>
		/// 一次完成所有操作，根据xml及提供的网页uri集合
		/// </summary>
		/// <param name="xmlpath">xml绝对路径</param>
		/// <param name="rootpath">xml配置中节点文件根目录</param>
		/// <param name="webPages">web页面uri集合，用逗号,分隔</param>
		public static void combineSupplyPagesItemsByXml(string xmlpath, string rootpath, string webPages) {
			string combineItems = getCombineFilesFromXml(xmlpath, rootpath);
			repMultiPagesMultiItems(webPages, combineItems, "", "");
		}
*/
		#endregion

		#region 生成带替换文件信息
		/*
		/// <summary>
		/// 读取xml合并关系配置文件， 获取js文件的合并关系
		/// </summary>
		/// <param name="path">xml文件绝对路径</param>
		/// <param name="rootpath">xml文件所在的根目录</param>
		/// <returns></returns>
		public static string getCombineFilesFromXml(string path, string rootpath) {
			string xmlFiles = "";
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(path);
			XmlNode projectNode = xmlDoc.SelectSingleNode("project");
			XmlNodeList targetNodeList = projectNode.SelectNodes("target");// .ChildNodes;
			int tLen = targetNodeList.Count;
			//target
			for (int i = 0; i < tLen; i++) {
				XmlNodeList concatNodeList = targetNodeList[i].SelectNodes("concat");
				int concatLen = concatNodeList.Count;
				//concat
				for (int j = 0; j < concatLen; j++) {
					string destFile = concatNodeList[j].Attributes["destfile"].Value;
					XmlNodeList pathNodeList = concatNodeList[j].SelectNodes("path");
					int pathLen = pathNodeList.Count;
					xmlFiles += rootpath + "::";
					//path
					for (int k = 0; k < pathLen; k++) {
						if (k != 0) {
							xmlFiles += ",";
						}
						xmlFiles += pathNodeList[k].Attributes["path"].Value;
					}

					xmlFiles += "=>" + destFile + ";";//\r\n
				}

			}
			return xmlFiles;
		}
		/// <summary>
		/// 获取html文件集合
		/// </summary>
		/// <param name="path"></param>
		/// <param name="pageType">*.html，使用|分隔</param>
		/// <returns></returns>
		public static string getHtmlFiles(string path, string pageType) {
			if (Directory.Exists(path)) {
				DirectoryInfo dir = new DirectoryInfo(path);
				string[] types = pageType.Split('|');
				List<FileInfo> fileList = new List<FileInfo>();
				int tLen = types.Length;
				for (int m = 0; m < tLen; m++) {
					fileList.AddRange(dir.GetFiles(types[m]).ToList<FileInfo>());
				}
				int len = fileList.Count;
				string htmlFiles = string.Empty;
				for (int i = 0; i < len; i++) {
					htmlFiles += "," + fileList[i].FullName;
				}
				return htmlFiles.Length>0? htmlFiles.Substring(1):htmlFiles;
			}
			return "";
		}
		 * */
		#endregion

		#region 替换
		/// <summary>
		/// 多个页面拆分,多种替换、匹配
		/// </summary>
		/// <param name="htmlPageFiles">使用逗号分隔多个绝对路径的html文件</param>
		/// <param name="combineFiles">使用分号分隔多项，内部使用::分隔绝对路径，使用=>分隔选项和选项合并结果</param>
		/// <param name="encoding">源文件编码</param>
		/// <param name="outputencoding">新编码</param>
		public static void repMultiPagesMultiItems(string htmlPageFiles, string combineFiles, string encoding, string outputencoding) {
			string[] htmlArr = htmlPageFiles.Split(',');
			int htmlLen = htmlArr.Length;
			//int combineLen = combineArr.Length;

			//每个html进行匹配
			for (int i = 0; i < htmlLen; i++) {
				//List<CombineInfo> validCombInfos = new List<CombineInfo>();
				string currentPage = htmlArr[i];
				repSinglePageMultiItems(currentPage, combineFiles, encoding, outputencoding);
			}
		}
		
		/// <summary>
		/// 单个页面拆分，多种替换、匹配
		/// </summary>
		/// <param name="currentPage">C:\ss\ss\index.html</param>
		/// <param name="combineFiles">
		/// 使用分号分隔多项，内部使用::分隔绝对路径，使用=>分隔选项和选项合并结果,如：
		/// C:\ss\ss::a.js,b.js=>a_b.js;D:\ss\ss::b.js,c.js=>b_c.js;
		/// </param>
		/// <param name="encoding">源文件编码</param>
		/// <param name="outputencoding">新编码</param>
		public static void repSinglePageMultiItems(string currentPage, string combineFiles, string encoding, string outputencoding) {
			string[] combineArr = combineFiles.Split(';');
			int combineLen = combineArr.Length;
			//每个合并组进行匹配
			for (int j = 0; j < combineLen; j++) {
				string[] hash = combineArr[j].Split(new string[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);
				if (hash.Length > 0) {
					/* *
					 * 不确定是否需要删除原来的src
					 * c:\s.html::a.js,b.js=>a_b.js;
					 * c:\s.html::b.js,c.js=>b_c.js;
					 * c:\s.html::a.js,c.js=>a_c.js;
					 * *是否同一个页面下不应该有重复的文件合并
					 * 只有有这合并的子js实例了，都删除原来的实例么
					 * 去重
					 * */
					string[] hash0 = hash[0].Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
					string rootPath = hash0[0];
					//string[] files = hash0[1].Split(',');
					string target = hash[1];

					repSinglePageSingleItem(currentPage, hash0[1], target, rootPath, encoding, outputencoding);
				}
			}
		}
		
		/// <summary>
		/// 串行替换js,css文件的src,确保不重复
		/// </summary>
		/// <param name="currentPage">html页面的绝对地址</param>
		/// <param name="srcs">例如: a.js,b.js,c.js,js/d.js</param>
		/// <param name="target">合并后的src，如:a_b_c_d.js</param>
		/// <param name="rootPath">js、css文件根目录,默认与html文件根目录相同k</param>
		/// <param name="encoding">源文件编码</param>
		/// <param name="outputencoding">新编码</param>
		public static void repSinglePageSingleItem(string currentPage, string srcs, string target, string rootPath, string encoding, string outputencoding) {
			string[] files = srcs.Split(',');
			//去重复js引用
			for (var n = 0; n < files.Length; n++) {
				string key = files[n];
				//Regex reg = new Regex("<script[\\s\\S]*?src=[\"\']?(" + key + ")[\"\']?[\\s\\S]*?>[\\s\\S]*?</script>");
				//string reg = "<script[\\s\\S]*?src=[\"\']?(" + key + ")[\"\']?[\\s\\S]*?>[\\s\\S]*?</script>";
				string reg = getRegexpStringByFileName(key);
				if (reg != string.Empty) {
					ReplaceFileStringByRegexStr(currentPage, reg, key, target, DoReplace.None, encoding, outputencoding);
				}
			}
			//匹配
			bool srcMtch = matchHtmlVal(srcs, target, currentPage, rootPath, true);
			if (srcMtch) {
				//validCombInfos.Add(new CombineInfo(rootPath, files.ToList<string>(), target, currentPage));
				//先到先删，保持最小粒度
				DoReplace doReplace = DoReplace.InnerRegex;
				for (var n = 0; n < files.Length; n++) {
					string key = files[n];
					//Regex reg = new Regex("<script[\\s\\S]*?src=[\"\']?(" + key + ")[\"\']?[\\s\\S]*?>[\\s\\S]*?</script>");
					//string reg = "<script[\\s\\S]*?src=[\"\']?(" + key + ")[\"\']?[\\s\\S]*?>[\\s\\S]*?</script>";
					string reg = getRegexpStringByFileName(key);
					if (reg == string.Empty) continue;
					/*
					 * 如果这组已经匹配成功过了，就清除该选项内存在的其他js文件：
					 *	a.js,b.js,c.js=>a_b_c.js
					 *  现在a.js=>a_b_c.js完成后
					 *  清除 b.js,c.js
					 */
					if (ReplaceFileStringByRegexStr(currentPage, reg, key, target, doReplace, encoding, outputencoding)) {
						//去除同组的其他js文件
						doReplace = DoReplace.Empty;
					}
				}
				/*
				清除该选项在页面内仍存在的js.src
				for (var n = 0; n < files.Length; n++) {
				    string key = files[n];
				    //Regex reg = new Regex("<script[\\s\\S]*?src=[\"\']?(" + key + ")[\"\']?[\\s\\S]*?>[\\s\\S]*?</script>");
					string reg="<script[\\s\\S]*?src=[\"\']?(" + key + ")[\"\']?[\\s\\S]*?>[\\s\\S]*?</script>";
				    ReplaceFileStringByRegexStr(currentPage, reg, key, target, DoReplace.Empty);
				}
				*/
			}
		}
		
		/// <summary>
		/// 根据要route的文件名后缀来确定正则，目前只支持：js,css格式
		/// </summary>
		/// <param name="fileName">a.js  a.css</param>
		/// <returns></returns>
		public static string getRegexpStringByFileName(string fileName) {
			string regStr = string.Empty;
			if (Path.HasExtension(fileName)) {
				string extension = Path.GetExtension(fileName);

				//目前支持js,css扩展名
				switch (extension) {
					case ".js":
						regStr = "<script[\\s\\S]*?src=[\"\']?(" + fileName + ")[\"\']?[\\s\\S]*?>[\\s\\S]*?</script>";
						break;
					case ".css":
						//<link href=(['"\s]?)([^'">\s]*)(['"\s]?).*(/>|</link>)
						//严格模式的第一分组引号，空格 <link href=(['"\s]?)([^'">\s]*)(\1).*(/>|</link>)
						regStr = "<link href=[\'\"\\s]?(" + fileName + ")[\'\"\\s]?.*(/>|</link>)";
						break;
					default: break;
				}

			}
			return regStr;
		}

		/// <summary>
		/// 确认该合并项是否匹配html中内容，合并
		/// </summary>
		/// <param name="srcs">[a.js,b.js,c.js]</param>
		/// <param name="target">a_b_c.js</param>
		/// <param name="currentPage">c:\ss\ss\index.html</param>
		/// <param name="rootPath">C:\ss\ss</param>
		/// <param name="onlyHeadScripts">是否只检索<head>标签内的内容...暂时实现</param>
		/// <returns>是否匹配</returns>
		public static bool matchHtmlVal(string srcs, string target, string currentPage, string rootPath, bool onlyHeadScripts) {
			string[] sources = srcs.Split(',');
			bool flag = true;
			int sLen = sources.Length;
			//获取html页面文字
			StreamReader htmlSrr = null;
			string fileStr = string.Empty;
			try {
				htmlSrr = new StreamReader(currentPage);
				fileStr = htmlSrr.ReadToEnd();
			} catch (Exception ex) {
				//error
			} finally {
				htmlSrr.Close();
			}
			//http://stackoverflow.com/questions/1750567/regex-to-get-attributes-and-body-of-script-tags
			//匹配所有script列表
			//Regex allScript = new Regex("<script[\\s\\S]*?src=[\"\']?([^\"\']+.js)[\"\']?[\\s\\S]*?>[\\s\\S]*?</script>");

			//查找是否有跟当前合并js全匹配的组
			for (int i = 0; i < sLen; i++) {
				//暂不匹配异步加载的js,暂时匹配全页面中，不光匹配head中
				string regStr = getRegexpStringByFileName(sources[i]);
				if (regStr == string.Empty) {
					flag = false;
					break;
				}
				//Regex reg = new Regex("<script[\\s\\S]*?src=[\"\']?(" + sources[i] + ")[\"\']?[\\s\\S]*?>[\\s\\S]*?</script>");
				Regex reg = new Regex(regStr);
				if (!reg.IsMatch(fileStr)) {
					flag = false;
					break;
				}
			}
			//该页面符合条件
			return flag;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strPath">文件路径</param>
		/// <param name="strOld">原始字符串(可为正则表达式)</param>
		/// <param name="strNew">新字符串</param>

		/// <summary>
		/// 用新的字符串替换制定文件中的旧字符串，旧字符串可以为正则表达式
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <param name="regStr">需要替换内容符合的正则表达式字符串</param>
		/// <param name="strOld">匹配一行内容中，需要替换的部分旧字符</param>
		/// <param name="strNew">替换旧字符的新字符</param>
		/// <param name="_doReplace">替换状态:None,InnerRegex,Empty</param>
		/// <param name="encoding">源文件编码</param>
		/// <param name="outputencoding">新编码</param>
		/// <returns>是否替换成功</returns>
		public static bool ReplaceFileStringByRegexStr(string filePath, string regStr, string strOld, string strNew, DoReplace _doReplace, string encoding, string outputencoding) {
			bool replaced = false;
			//编码转换，未知编码，是否触发异常
			Encoding _encoding = null;
			Encoding _outputEncoding = null;
			try {
				_encoding = Encoding.GetEncoding(encoding);
			} catch (Exception ex) {
				_encoding = null;
				//log
			}
			try {
				_outputEncoding = Encoding.GetEncoding(outputencoding);
			} catch (Exception ex) {
				_outputEncoding = null;
				//log
			}
			try {
				Regex reg = new Regex(regStr);
				StreamReader streamReader = (_encoding == null) ? new StreamReader(filePath, true) : new StreamReader(filePath, _encoding, true);
				StringBuilder strFileContent = new StringBuilder();

				while (!streamReader.EndOfStream)//读取文件
                {
					string strFileLine = streamReader.ReadLine();
					string newStr = string.Empty;
					//对符合当前正则的js名文件进行匹配处理
					newStr = reg.Replace(strFileLine, new MatchEvaluator(delegate(Match m) {
						string ret = string.Empty;
						switch (_doReplace) {
							case DoReplace.InnerRegex:
								ret = m.Value.Replace(strOld, strNew).Replace("\t", "");
								replaced = true;
								//去除本页面其他同名的js
								_doReplace = DoReplace.Empty;
								break;
							case DoReplace.None:
								//去除本页面其他同名的js
								_doReplace = DoReplace.Empty;
								ret = strFileLine.Replace("\t", "");
								break;
							case DoReplace.Empty:
								ret = string.Empty;
								break;
							default: break;

						}
						return ret;
					}));
					Regex r = new Regex("^[\\s\\t]*$");
					if (!r.IsMatch(newStr)) {
						strFileContent.Append(newStr + "\r\n");
					}
				}
				streamReader.Close();
				//把修改过的内容写入文件,此处为修改原页面内容，所以是替换，不是append
				StreamWriter streamWriter = (_outputEncoding == null) ? new StreamWriter(filePath) : new StreamWriter(filePath, false, _outputEncoding);
				streamWriter.Write(strFileContent.ToString());
				streamWriter.Close();

			} catch (Exception ex) {
				throw ex;
				return false;
			}
			return replaced;
		}

		#endregion

	}
	/// <summary>
	/// 替换方式
	/// </summary>
	public enum DoReplace
	{
		None = 1,
		InnerRegex = 2,
		Empty = 3
	}
}

/**
 * 文件类型的问题
 * 是否包含子目录
 * xml中的路径与页面路径之间的关联
 * 根目录
 */
