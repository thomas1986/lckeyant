using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace LckeyAnt
{
	public class ReplaceMarkContent
	{
		/// <summary>
		/// 从path路径读取文件中所有文字内容
		/// </summary>
		/// <param name="path"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static string readFileContent(string path, string encoding) {
			string content = string.Empty;
			try {

				Encoding _encoding = null;
				try {
					//获取读取文件使用的编码格式
					_encoding = Encoding.GetEncoding(encoding);
				} catch (Exception ex) {
					_encoding = null;
					//log
				}
				if (File.Exists(path)) {
					content = (_encoding == null) ? File.ReadAllText(path) : File.ReadAllText(path, _encoding);
				}
			} catch (Exception ex) {
				throw ex;
			}
			return content;
		}
		/// <summary>
		/// 写入指定路径文件的内容
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="newContent"></param>
		/// <param name="outputEncoding"></param>
		/// <param name="append">true,追加；false,覆盖</param>
		/// <returns></returns>
		public static bool writeFileContent(string filePath, string newContent, string outputEncoding, bool append) {
			bool flag = false;
			Encoding _outputEncoding = null;
			try {
				_outputEncoding = Encoding.GetEncoding(outputEncoding);
			} catch (Exception ex) {
				_outputEncoding = null;
				//log
			}

			StreamWriter fileWriter = null;
			//追加内容到 targetFilePath
			try {
				/*if (_outputEncoding == null) {
					File.WriteAllText(filePath, newContent);
				} else {
					File.WriteAllText(filePath, newContent, _outputEncoding);
				}*/
				fileWriter = (_outputEncoding == null) ? new StreamWriter(filePath, false) : new StreamWriter(filePath, false, _outputEncoding);
				fileWriter.Write(newContent);
				flag = true;
			} catch (Exception ex) {
				throw ex;
			} finally {
				fileWriter.Flush();
				fileWriter.Close();
			}
			return flag;
		}

		/// <summary>
		/// 替换所有符合:来源字符串中的start<-->end之间内容为新字符串；
		/// </summary>
		/// <param name="sourceContent">来源字符串内容</param>
		/// <param name="start">起始标志字符，如<!--start var a--></param>
		/// <param name="end">结束标志字符，如<!--end var a--></param>
		/// <param name="newReplaceContent">新字符串</param>
		/// <returns></returns>
		public static string replaceMarkNoteContent(string sourceContent, string start, string end, string newContent) {
			try {
				//正则替换，区分大小写
				Regex regReplace = new Regex(start + @".*" + end, RegexOptions.Compiled);
				string newReplaceContent = start + newContent + end;
				sourceContent = regReplace.Replace(sourceContent, newReplaceContent);

				#region 索引方式替换
				/**索引的方式替换： 相同字符的所有替换，或者只替换第一个符合的，其他内容不同的不替换
				int startRepIndex = sourceContent.IndexOf(start);
				//内容起始索引
				int startRepContentIndex = startRepIndex + start.Length;
				int endRepContentIndex = sourceContent.IndexOf(end);
				int endRepIndex = endRepContentIndex + end.Length;
				//判断是否存在起始、结束标记,存在才替换；极限情况下都为0，不处理
				if (startRepIndex > -1 && endRepContentIndex >= startRepContentIndex) {
					string oldContent = sourceContent.Substring(startRepContentIndex, endRepIndex - startRepContentIndex);
					//sourceContent = sourceContent.Replace(oldContent, newReplaceContent);//导致与内容相同的部分都被一次性替换
					//包含start,end标志在内的内容
					string oldReplaceCotent = sourceContent.Substring(startRepIndex, endRepIndex - startRepIndex);
					string newReplaceContent1 = start + newContent + end;
					sourceContent = sourceContent.Replace(oldReplaceCotent, newReplaceContent1);
				}
				 * */
				#endregion
				
			} catch (Exception ex) {
				throw ex;
			}
			return sourceContent;
		}

		/// <summary>
		/// 直接替换oldMark为newReplaceContent,用于发布模板页面，只可以替换一次……
		/// </summary>
		/// <param name="sourceContent"></param>
		/// <param name="oldMark">待替换字符标志，替换后消失</param>
		/// <param name="newReplaceContent"></param>
		/// <returns></returns>
		public static string replaceMarkContent(string sourceContent, string oldMark, string newReplaceContent) {
			try {
				if (oldMark != string.Empty) {
					sourceContent = sourceContent.Replace(oldMark, newReplaceContent);
				}
			} catch (Exception ex) {
				throw ex;
			}
			return sourceContent;
		}

		/// <summary>
		/// 一次打开文件，替换所有
		/// </summary>
		/// <param name="strNewsHtml"></param>
		/// <param name="strOldHtml"></param>
		/// <param name="strModeFilePath"></param>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static bool CreatHtmlPage(string[] strNewsHtml, string[] strOldHtml, string strModeFilePath, string filePath) {
			bool Flage = false;
			StreamReader ReaderFile = null;
			StreamWriter WrirteFile = null;
			Encoding Code = Encoding.GetEncoding("gb2312");
			string strFile = string.Empty;
			try {
				ReaderFile = new StreamReader(filePath, Code);
				strFile = ReaderFile.ReadToEnd();

			} catch (Exception ex) {
				throw ex;
			} finally {
				ReaderFile.Close();
			}
			try {
				int intLengTh = strNewsHtml.Length;
				for (int i = 0; i < intLengTh; i++) {
					strFile = strFile.Replace(strOldHtml[i], strNewsHtml[i]);
				}
				WrirteFile = new StreamWriter(filePath, false, Code);
				WrirteFile.Write(strFile);
				Flage = true;
			} catch (Exception ex) {
				throw ex;
			} finally {

				WrirteFile.Flush();
				WrirteFile.Close();
			}
			return Flage;
		}

		/// <summary>
		/// 一次读取文件，一次性添加所有
		/// </summary>
		/// <param name="strNewsHtml"></param>
		/// <param name="strStartHtml"></param>
		/// <param name="strEndHtml"></param>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static bool UpdateHtmlPage(string[] strNewsHtml, string[] strStartHtml, string[] strEndHtml, string filePath) {
			bool Flage = false;
			StreamReader ReaderFile = null;
			StreamWriter WrirteFile = null;
			Encoding Code = Encoding.GetEncoding("gb2312");
			string strFile = string.Empty;
			try {
				ReaderFile = new StreamReader(filePath, Code);
				strFile = ReaderFile.ReadToEnd();

			} catch (Exception ex) {
				throw ex;
			} finally {
				ReaderFile.Close();
			}
			try {
				int intLengTh = strNewsHtml.Length;
				for (int i = 0; i < intLengTh; i++) {
					int intStart = strFile.IndexOf(strStartHtml[i]) + strStartHtml[i].Length;
					int intEnd = strFile.IndexOf(strEndHtml[i]);
					string strOldHtml = strFile.Substring(intStart, intEnd - intStart);
					strFile = strFile.Replace(strOldHtml, strNewsHtml[i]);
				}
				WrirteFile = new StreamWriter(filePath, false, Code);
				WrirteFile.Write(strFile);
				Flage = true;
			} catch (Exception ex) {
				throw ex;
			} finally {
				WrirteFile.Flush();
				WrirteFile.Close();
			}
			return Flage;
		}

	}
}
