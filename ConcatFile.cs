using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;


namespace LckeyAnt
{
	/// <summary>
	/// 拼接多个文件
	/// </summary>
	public class ConcatFile
	{
		/// <summary>
		/// 根据uri详情，拼接文件中内容到目标文件k
		/// </summary>
		/// <param name="concatUriDetail">G:\ant::a.js,b.js=>a_b.js;</param>
		/// <param name="encoding"></param>
		/// <param name="outputencoding"></param>
		/// <returns></returns>
		public static bool concatFilesByUriDetail(string concatUriDetail, string encoding, string outputencoding) {
			FileStream fs = null;
			bool flag = false;
			try {
				string[] concatUriArr = concatUriDetail.Split(';');
				int concatLen = concatUriArr.Length;
				//每个合并组进行匹配
				for (int j = 0; j < concatLen; j++) {
					string[] hash = concatUriArr[j].Split(new string[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);
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
						string[] sourceFiles = hash0[1].Split(',');
						string targetFilePath = Path.Combine(rootPath, hash[1]);
						//创建目标文件
						fs = File.Open(targetFilePath, FileMode.Create, FileAccess.Write);
						fs.Flush();
						fs.Dispose();
						fs.Close();
						string sourceFilePath = string.Empty;
						int sourceLen = sourceFiles.Length;
						for (int k = 0; k < sourceLen; k++) {
							sourceFilePath = Path.Combine(rootPath, sourceFiles[k]);
							//一个一个拼接
							concatFileSource2Target(sourceFilePath, targetFilePath, encoding, outputencoding);
						}
					}
				}
				flag = true;
			} catch (Exception ex) {
				throw ex;
			} finally {

			}
			return flag;
		}
		/// <summary>
		/// 一个一个拼接文件到目标文件
		/// </summary>
		/// <param name="sourceFilePath"></param>
		/// <param name="targetFilePath"></param>
		/// <param name="encoding"></param>
		/// <param name="outputencoding"></param>
		/// <returns></returns>
		public static bool concatFileSource2Target(string sourceFilePath, string targetFilePath, string encoding, string outputEncoding) {
			Encoding _encoding = null;
			Encoding _outputEncoding = null;
			bool flag = false;
			try {
				_encoding = Encoding.GetEncoding(encoding);
			} catch (Exception ex) {
				_encoding = null;
				//log
			}
			try {
				_outputEncoding = Encoding.GetEncoding(outputEncoding);
			} catch (Exception ex) {
				_outputEncoding = null;
				//log
			}
			StreamReader sourceReader = null;
			StreamWriter sourceWriter = null;
			string sourceContent = string.Empty;

			//读取source file
			try {
				sourceReader = (_encoding == null) ? new StreamReader(sourceFilePath, true) : new StreamReader(sourceFilePath, _encoding, true);
				sourceContent = sourceReader.ReadToEnd();
			} catch (Exception ex) {
				throw ex;
			} finally {
				sourceReader.Close();
			}

			//追加内容到 targetFilePath
			try {
				sourceWriter = (_outputEncoding == null) ? new StreamWriter(targetFilePath, true) : new StreamWriter(targetFilePath, true, _outputEncoding);
				sourceWriter.Write(sourceContent);
				flag = true;
			} catch (Exception ex) {
				throw ex;
			} finally {
				sourceWriter.Flush();
				sourceWriter.Close();
			}
			return flag;
		}

	}
}
