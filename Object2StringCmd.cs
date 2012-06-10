using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace LckeyAnt
{
	/// <summary>
	/// 将配置信息集合属性转变为文字命令
	/// </summary>
	public class Object2StringCmd
	{
		#region exec 方法接口,执行单个配置文件节点
		
		/// <summary>
		/// 执行route merge命令，替换页面中多个src地址 为合并后的1个src地址
		/// </summary>
		/// <param name="confTarRoute"></param>
		/// <param name="rootPath">rootPath目前只在查找相对根目录位置的对应目录中html等文件情况时有效</param>
		/// <returns></returns>
		public bool execRouteMergeCmd(ConfigTargetRoute confTarRoute, string rootPath) {
			try {
				//转变为父类List
				List<BaseConfigTargetFilterFile> baseConfTarFilterFileList = parseTList<ConfigTargetRouteMergePage, BaseConfigTargetFilterFile>(confTarRoute.MergePageList);
				//pages; rootPath只在查找相对根目录位置的对应目录中html等文件情况时有效
				string pageFiles = getFilesInDir(baseConfTarFilterFileList, rootPath);
				//src
				string mergeUriDetail = getRouteMergeUriDetail(confTarRoute.RouteSourceList, confTarRoute.TargetFile, rootPath);
				//具体操作，附带 encoding,outputencoding
				MergeRoute.repMultiPagesMultiItems(pageFiles, mergeUriDetail, confTarRoute.Encoding, confTarRoute.OutputEncoding);
				return true;
			} catch (Exception ex) {
				return false;
			}
		}

		/// <summary>
		///  执行concat文件合并命令
		/// </summary>
		/// <param name="confTarConcat"></param>
		/// <param name="rootPath"></param>
		/// <returns></returns>
		public bool execConcatCmd(ConfigTargetConcat confTarConcat, string rootPath) {
			try {
				//src
				string concatUriDetail = getConcatUriDetail(confTarConcat.SourceList, confTarConcat.TargetFile, rootPath);
				//encoding,outputencoding
				ConcatFile.concatFilesByUriDetail(concatUriDetail, confTarConcat.Encoding, confTarConcat.OutputEncoding);
				return true;
			} catch (Exception ex) {
				throw ex;
				return false;
			}
		}

		/// <summary>
		/// delete，删除命令执行
		/// </summary>
		/// <param name="confTarDelList"></param>
		/// <param name="rootPath"></param>
		/// <returns></returns>
		public bool execDeleteCmd(List<ConfigTargetDelete> confTarDelList, string rootPath) {
			try {
				//转变为父类List
				List<BaseConfigTargetFilterFile> baseConfTarFilterFileList = parseTList<ConfigTargetDelete, BaseConfigTargetFilterFile>(confTarDelList);
				string deleteFiles = getFilesInDir(baseConfTarFilterFileList, rootPath);
				string[] delFileArr = deleteFiles.Split(',');
				int delLen = delFileArr.Length;
				for (int i = 0; i < delLen; i++) {
					if (File.Exists(delFileArr[i])) {
						File.Delete(delFileArr[i]);
					}
				}
				return true;
			} catch (Exception ex) {
				throw ex;
				return false;
			}
		}

		/// <summary>
		/// 执行压缩js,css命令
		/// </summary>
		/// <param name="confTarCompress"></param>
		/// <param name="rootPath"></param>
		/// <returns></returns>
		public bool execCompressCmd(ConfigTargetCompress confTarCompress, string rootPath) {
			try {
				//需要压缩的代码文件
				string pageFiles = getFilesInDir(confTarCompress.BaseConfTarFilterFileList, rootPath);
				string[] pageFilesArr = pageFiles.Split(',');
				string currentFile = string.Empty;
				string fromFile = string.Empty;
				string toFile = string.Empty;
				//指向的路径
				string toFilePath = string.Empty;
				string currentRoot = string.Empty;
				string SPACE = " ";
				string destDirPath = string.Empty;
				//mapper 情况下
				#region mapper
				List<string> cmdList = new List<string>();
				int pageLen = pageFilesArr.Length;
				for (int i = 0; i < pageLen; i++) {
					//当前压缩文件
					currentFile = pageFilesArr[i];
					//当前根目录
					currentRoot = currentFile.Substring(0, currentFile.LastIndexOf('\\'));
					fromFile = currentFile.Substring(currentFile.LastIndexOf('\\') + 1);
					//to
					if (confTarCompress.MapperType == "regexp") {
						Regex _fromReg = new Regex(confTarCompress.MapperFrom);
						//"aa.js".replace(RegExp("^(.*)\.js$"), "\\1-min.js".replace(/\\/g, '$'));
						/*toFile = _from.Replace(fromFile, delegate(Match m) {
							return confTarCompress.MapperTo.Replace("\\1", m.Value);
						});*/
						toFile = _fromReg.Replace(fromFile, confTarCompress.MapperTo.Replace('\\', '$'));
					}
					//在读取xml时处理
					destDirPath = Path.GetFullPath(Path.Combine(currentRoot, confTarCompress.DestDir));
					//确认存在destDirPath目录
					if (!Directory.Exists(destDirPath)) {
						Directory.CreateDirectory(destDirPath);
					}
					toFilePath = Path.GetFullPath(Path.Combine(destDirPath, toFile));
					//添加压缩命令语句
					cmdList.Add(confTarCompress.BeforeSrcFile + SPACE + currentFile + SPACE + confTarCompress.OutputArg + SPACE + toFilePath);
				}
				#endregion
				//srcFile ,targetFile 情况下
				#region srcFile , targetFile
				if (confTarCompress.SrcFile != string.Empty && confTarCompress.TargetFile != string.Empty) {
					string targetDir = confTarCompress.TargetFile.Substring(0, confTarCompress.TargetFile.LastIndexOf('\\'));
					string targetFile = confTarCompress.TargetFile.Substring(confTarCompress.TargetFile.LastIndexOf('\\') + 1);
					//dest目录
					string targetDestDirPath = Path.GetFullPath(Path.Combine(targetDir, confTarCompress.DestDir));
					//确保存在targetDestDirPath目录
					if (!Directory.Exists(targetDestDirPath)) {
						Directory.CreateDirectory(targetDestDirPath);
					}
					string targetFilePath = Path.GetFullPath(Path.Combine(targetDestDirPath, targetFile));
					//添加压缩命令语句
					cmdList.Add(confTarCompress.BeforeSrcFile + SPACE + confTarCompress.SrcFile + SPACE + confTarCompress.OutputArg + SPACE + targetFilePath);
				}
				#endregion
				//执行
				foreach (string compressCmdStr in cmdList) {
					RunCommand.callCmdSync(rootPath, compressCmdStr);
				}
				return true;
			} catch (Exception ex) {
				throw ex;
				return false;
			}
		}

		/// <summary>
		/// 执行单条命令语句
		/// </summary>
		/// <param name="confTarCommand"></param>
		/// <param name="rootPath"></param>
		/// <returns></returns>
		public bool execCommandCmd(ConfigTargetCommand confTarCommand, string rootPath) {
			try {
				string workDir = confTarCommand.WorkDir;
				List<string> cmdList = confTarCommand.CommandList;
				foreach (string cmd in cmdList) {
					RunCommand.callCmdSync(workDir, cmd);
				}
				return true;
			} catch (Exception ex) {
				throw ex;
				return false;
			}
		}
		
		/// <summary>
		/// 执行批处理命令
		/// </summary>
		/// <param name="confTarBatch"></param>
		/// <param name="rootPath"></param>
		/// <returns></returns>
		public bool execBatchCmd(List<ConfigTargetBatch> confTarBatchList, string rootPath) {
			try {
				foreach (ConfigTargetBatch batPath in confTarBatchList) {
					//如果路径文件bat存在，则执行
					if (File.Exists(batPath.Path)) {
						RunCommand.callBat(batPath.Path, batPath.Args);
					}
				}
				return true;
			} catch (Exception ex) {
				throw ex;
				return false;
			}
		}
		
		/// <summary>
		/// 执行替换命令
		/// </summary>
		/// <param name="confTarReplace"></param>
		/// <param name="rootPath"></param>
		/// <returns></returns>
		public bool execReplaceCmd(ConfigTargetReplace confTarReplace, string rootPath) {
			bool flag = false;
			try {
				//待替换的页面
				string repFiles = getFilesInDir(confTarReplace.FileSetList, rootPath);
				string newValue = string.Empty;
				//从路径path获取marknote待替换的值
				foreach (MarkNoteValueReplace markNoteValRep in confTarReplace.MarkNoteRepList) {
					newValue = ReplaceMarkContent.readFileContent(markNoteValRep.Path, confTarReplace.Encoding);
					if (newValue != string.Empty) {
						//保存值
						markNoteValRep.NewValue = newValue;
					}
				}
				//从路径path获取mark待替换的值
				foreach (MarkValueReplace markValRep in confTarReplace.MarkRepList) {
					newValue = ReplaceMarkContent.readFileContent(markValRep.Path, confTarReplace.Encoding);
					if (newValue != string.Empty) {
						//保存值
						markValRep.NewValue = newValue;
					}
				}
				//按页面 替换 marknote,mark内容
				string[] repFileArr = repFiles.Split(',');
				int arrLen = repFileArr.Length;
				for (int i = 0; i < arrLen; i++) {
					string currrentFile = repFileArr[i];
					string sourceContent = ReplaceMarkContent.readFileContent(currrentFile, confTarReplace.Encoding);
					//替换marknote 
					foreach (MarkNoteValueReplace markNoteValRep in confTarReplace.MarkNoteRepList) {
						//是否空字符就不替换?
						//if (markNoteValRep.NewValue != string.Empty) {
						//替换字符
						sourceContent = ReplaceMarkContent.replaceMarkNoteContent(sourceContent, markNoteValRep.Start, markNoteValRep.End, markNoteValRep.NewValue);
						//}
					}
					//替换mark 
					foreach (MarkValueReplace markValRep in confTarReplace.MarkRepList) {
						sourceContent = ReplaceMarkContent.replaceMarkContent(sourceContent, markValRep.MarkKey, markValRep.NewValue);
					}
					//保存覆盖替换内容到页面
					ReplaceMarkContent.writeFileContent(currrentFile, sourceContent, confTarReplace.OutputEncoding,false);
				}
				flag = true;
			} catch (Exception ex) {
				throw ex;
			}
			return flag;
		}

		#endregion


		/// <summary>
		/// List集合内的类转换为继承的父类k		
		/// </summary>
		/// <typeparam name="T1">源数据类</typeparam>
		/// <typeparam name="T2">结果数据类</typeparam>
		/// <param name="t1List">源数据列表</param>
		/// http://www.cnblogs.com/lin614/archive/2008/05/17/1201438.html
		/// <returns>List<T2>结果数据列表</returns>
		public List<T2> parseTList<T1, T2>(List<T1> t1List) where T1 : T2 {
			List<T2> t2List = new List<T2>();
			foreach (T1 t1 in t1List) {
				t2List.Add((T2)t1);
			}
			return t2List;
		}

		#region 页面集合
		/// <summary>
		/// 根据路径和类型获取文件集合字符串, 逗号分隔
		/// </summary>
		/// <param name="path"></param>
		/// <param name="pageType"></param>
		/// <returns></returns>
		public string getFilesByType(string path, string pageType) {
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
				return htmlFiles.Length > 0 ? htmlFiles.Substring(1) : htmlFiles;
			}
			return "";
		}

		/// <summary>
		/// 默认不包含子目录
		/// </summary>
		/// <param name="baseFilterFileList">提取信息类集合</param>
		/// <param name="rootPath">根目录</param>
		/// <returns>用逗号分隔的绝对路径拼接字符串</returns>
		public string getFilesInDir(List<BaseConfigTargetFilterFile> baseFilterFileList, string rootPath) {
			int pageCount = baseFilterFileList.Count;
			string htmlFiles = string.Empty;

			for (int i = 0; i < pageCount; i++) {
				BaseConfigTargetFilterFile baseFilterFilePage = baseFilterFileList[i];
				//替换src文件的目录
				string pageDir = Path.Combine(rootPath, baseFilterFilePage.Dir);
				//确保是本地绝对路径
				pageDir = Path.GetFullPath(pageDir);
				DirectoryInfo dirInfo = new DirectoryInfo(pageDir);

				//includes
				string[] types = baseFilterFilePage.Includes.Split(',');
				List<FileInfo> fileList = new List<FileInfo>();
				int tLen = types.Length;
				for (int m = 0; m < tLen; m++) {
					//默认不检索子目录
					fileList.AddRange(dirInfo.GetFiles(types[m], SearchOption.TopDirectoryOnly).ToList<FileInfo>());
				}
				//excludes获取不符合的文件集合
				string[] exTypes = baseFilterFilePage.Excludes.Split(',');
				List<FileInfo> exFileList = new List<FileInfo>();
				int exLen = exTypes.Length;
				for (int n = 0; n < exLen; n++) {
					//默认不检索子目录
					exFileList.AddRange(dirInfo.GetFiles(exTypes[n], SearchOption.TopDirectoryOnly).ToList<FileInfo>());
				}

				List<FileInfo> retFileList = new List<FileInfo>();
				//filter
				foreach (FileInfo fileInfo in fileList) {
					retFileList.Add(fileInfo);
					foreach (FileInfo exFileInfo in exFileList) {
						if (fileInfo.Name == exFileInfo.Name) {
							retFileList.Remove(fileInfo);
							break;
						}
					}
				}
				int len = retFileList.Count;

				for (int j = 0; j < len; j++) {
					htmlFiles += "," + retFileList[j].FullName;
				}
			}
			return htmlFiles.Length > 0 ? htmlFiles.Substring(1) : htmlFiles;
		}



		#endregion

		#region url地址替换
		/// <summary>
		/// 读取xml合并关系配置文件， 获取js文件的合并关系
		/// 使用分号分隔多项，内部使用::分隔绝对路径，使用=>分隔选项和选项合并结果,如：
		/// C:\ss\ss::a.js,b.js=>a_b.js;D:\ss\ss::b.js,c.js=>b_c.js;
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
		/// route merge 获取单个合并文件src的字符串详情
		/// </summary>
		/// <param name="routeSourceList"></param>
		/// <param name="targetFile"></param>
		/// <param name="rootPath"></param>
		/// <returns></returns>
		public string getRouteMergeUriDetail(List<string> routeSourceList, string targetFile, string rootPath) {
			string mergeDetail = string.Empty;//rootPath+"::";
			foreach (string routeSource in routeSourceList) {
				mergeDetail += "," + routeSource;
			}
			mergeDetail = rootPath + "::" + ((mergeDetail.Length > 0) ? mergeDetail.Substring(1) : mergeDetail);
			mergeDetail += "=>" + targetFile + ";";
			return mergeDetail;
		}

		/// <summary>
		/// concat 获取单个拼接文件uri的详情，需要确保先后依赖顺序
		/// </summary>
		/// <param name="concatSourceList"></param>
		/// <param name="targetFile"></param>
		/// <param name="rootPath"></param>
		/// <returns></returns>
		public string getConcatUriDetail(List<string> concatSourceList, string targetFile, string rootPath) {
			string concatUriDetail = string.Empty;//rootPath+"::";
			int concatCount = concatSourceList.Count;
			//必须确保合并文件内容在页面中显示的先后顺序,有依赖关系
			for (int i = 0; i < concatCount; i++) {
				concatUriDetail += "," + concatSourceList[i];
			}
			concatUriDetail = rootPath + "::" + ((concatUriDetail.Length > 0) ? concatUriDetail.Substring(1) : concatUriDetail);
			concatUriDetail += "=>" + targetFile + ";";
			return concatUriDetail;
		}
		#endregion

	}
}
/**
 * 目前compressor的mapper只支持regexp类型
 ** 路径：
 *	 路径替换，是否在configAccess.cs还是本cs中执行。。凌乱=>趋向于在configAccess读取时就确定路径，本页面中判断路径文件是否存在
 *	 FileSetList的路径一律在getFilesInDir方法中才替换
 **
 * 使用了很多外部变量，在内部替换，，，是否异常下会引起错乱
 * 约束条件判断，是否只在根方法进行，是否在调用处，每次也判断
 * bat执行，cmd执行中发生的错误无法获取
 * 错误的cmd命令如dirxxx 也无法显示到log
 * 子目录目前全部不去自动包含
 * 添加节点ID，执行时根据依赖关系判断是否发生异常情况下往下执行?
 */

/*
  public String substitute(String input, String argument, int options)
        throws BuildException {
        // translate \1 to $(1) so that the Matcher will work
        StringBuffer subst = new StringBuffer();
        for (int i = 0; i < argument.length(); i++) {
            char c = argument.charAt(i);
            if (c == '$') {
                subst.append('\\');
                subst.append('$');
            } else if (c == '\\') {
                if (++i < argument.length()) {
                    c = argument.charAt(i);
                    int value = Character.digit(c, DECIMAL);
                    if (value > -1) {
                        subst.append("$").append(value);
                    } else {
                        subst.append(c);
                    }
                } else {
                    // XXX - should throw an exception instead?
                    subst.append('\\');
                }
            } else {
                subst.append(c);
            }
        }
        argument = subst.toString();

        int sOptions = getSubsOptions(options);
        Pattern p = getCompiledPattern(options);
        StringBuffer sb = new StringBuffer();

        Matcher m = p.matcher(input);
        if (RegexpUtil.hasFlag(sOptions, REPLACE_ALL)) {
            sb.append(m.replaceAll(argument));
        } else {
            boolean res = m.find();
            if (res) {
                m.appendReplacement(sb, argument);
                m.appendTail(sb);
            } else {
                sb.append(input);
            }
        }
        return sb.toString();
    }
 
 */