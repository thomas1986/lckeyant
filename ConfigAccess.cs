using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace LckeyAnt
{
	public class ConfigAccess
	{

		#region 读取配置信息
		/// <summary>
		/// 读取build配置文件，生成属性集合
		/// </summary>
		/// <param name="path"></param>
		/// <param name="rootpath">执行环境所在的相对根目录,xml中节点属性值相对的根目录，默认与xml文件夹一致</param>
		/// <returns></returns>
		public ConfigInfo getConfigInfo(string path, string rootpath) {
			ConfigInfo confInfo = new ConfigInfo();
			try {
				LogOutput.logConsoleNow("start read config info");
				path = Path.GetFullPath(Path.Combine(GlobalCurrent.PROJECT_WORKSPACE, path));
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(path);
				if (rootpath != string.Empty) {
					confInfo.RootPath = rootpath;
				} else {
					confInfo.RootPath = path.Substring(0, path.LastIndexOf('\\'));
				}
				XmlNode projectNode = xmlDoc.SelectSingleNode("project");
				XmlNodeList projectChildNodes = projectNode.ChildNodes;

				//property集合
				Dictionary<string, string> dictProperty = new Dictionary<string, string>();
				//target节点集合
				List<XmlNode> targetList = new List<XmlNode>();

				int projectChildNodesLen = projectChildNodes.Count;
				XmlNode currentNode;

				for (int i = 0; i < projectChildNodesLen; i++) {
					currentNode = projectChildNodes[i];
					switch (currentNode.Name) {
						case "property":
							string proKey = string.Empty;
							string proValue = string.Empty;
							//property节点 必须有name,value属性
							if (currentNode.Attributes["name"] != null) {
								proKey = currentNode.Attributes["name"].Value;

								string dynamic = getXmlNodeAttrVal(currentNode, "dynamic");
								if (dynamic == "true") {
									string type = getXmlNodeAttrVal(currentNode, "type");
									//动态版本号
									switch (type) {
										case "datetime":
											//时间格式
											string _format = getXmlNodeAttrVal(currentNode, "format");
											if (_format == string.Empty) _format = "yyyyMMddHHmmss";
											proValue = DateTime.Now.ToString(_format);
											break;
										case "number":
											proValue = Math.Floor((new Random().NextDouble() * 1e8)).ToString();
											break;
										case "guid":
											proValue = Guid.NewGuid().ToString();
											break;
										default:
											proValue = DateTime.Now.ToString();
											break;
									}
									dictProperty[proKey] = proValue;
									break;
								}
								proValue = getXmlNodeAttrVal(currentNode, "value");
								if (proValue == string.Empty) {

									proValue = getXmlNodeAttrVal(currentNode, "location");
								}

								//无则添加key,有则覆盖value
								dictProperty[proKey] = proValue;

							}
							break;
						case "target":
							//target节点
							targetList.Add(currentNode);
							break;
						default: break;


					}
				}

				confInfo.DictProperty = repKeyInValue(dictProperty);
				//所有target节点属性的集合，有顺序
				List<List<Dictionary<string, object>>> allTargetNodesList = new List<List<Dictionary<string, object>>>();
				//读取target
				foreach (XmlNode targetNode in targetList) {
					allTargetNodesList.Add(getTargetNodeInfo(targetNode, confInfo.DictProperty, confInfo.RootPath));
				}
				confInfo.AllTargetList = allTargetNodesList;
				LogOutput.logConsoleNow("end   read config info");
			} catch (Exception ex) {
				throw new LogException("读取配置文件" + path, ex);
			}
			return confInfo;
		}
		/// <summary>
		/// 替换property里面的${key}值
		/// </summary>
		/// <param name="dictProperty"></param>
		/// <returns></returns>
		public Dictionary<string, string> repKeyInValue(Dictionary<string, string> dictProperty) {
			LogOutput.logConsoleNow("start read config info property");
			Dictionary<string, string> _dictProperty = new Dictionary<string, string>();
			foreach (string key in dictProperty.Keys) {
				_dictProperty[key] = dictProperty[key];
				//有类似符号才匹配
				Regex preReg = new Regex(@"\$\{[^\}]+\}");
				if (preReg.IsMatch(dictProperty[key])) {
					foreach (string _key in dictProperty.Keys) {
						//string old = "${" + _key + "}";
						Regex reg = new Regex("\\$\\{" + _key + "\\}");
						//string repAfter = _dictProperty[key].Replace(old,dictProperty[_key]);						
						string repAfter = reg.Replace(_dictProperty[key], dictProperty[_key]);
						_dictProperty[key] = repAfter; //dictProperty[_key].Replace(old, dictProperty[key]);
					}
				}
			}
			LogOutput.logConsoleNow("end   read config info property");
			return _dictProperty;
		}

		/// <summary>
		///  具体某target节点下,获取操作细节
		/// </summary>
		/// <param name="targetNode"></param>
		/// <returns></returns>
		public List<Dictionary<string, object>> getTargetNodeInfo(XmlNode targetNode, Dictionary<string, string> dictProperty, string rootPath) {
			LogOutput.logConsoleNow("start read config info target");
			XmlNodeList targetChildNodes = targetNode.ChildNodes;
			int targetChildNodesLen = targetNode.ChildNodes.Count;
			//本target节点内属性集合,有先后顺序
			List<Dictionary<string, object>> currentTargetList = new List<Dictionary<string, object>>();

			for (int i = 0; i < targetChildNodesLen; i++) {
				XmlNode targetChildNodesI = targetChildNodes[i];
				switch (targetChildNodesI.Name) {
					//合并页面中文件路径引用
					case "route":
						#region route
						//目标文件
						string routeTargetFile = filterValueByKey(targetChildNodesI.Attributes["destfile"].Value, dictProperty);
						string routeEncoding = string.Empty;
						string routeOutputEncoding = string.Empty;
						//encoding
						routeEncoding = getFilteredXmlNodeAttrVal(targetChildNodesI, "encoding", dictProperty);
						//output encoding
						routeOutputEncoding = getFilteredXmlNodeAttrVal(targetChildNodesI, "outputencoding", dictProperty);
						//文件来源集合
						XmlNodeList routeSourceNodes = targetChildNodesI.ChildNodes;
						int routeSourceNodesLen = routeSourceNodes.Count;
						List<string> routeSourceList = new List<string>();
						List<ConfigTargetRouteMergePage> mergePageList = new List<ConfigTargetRouteMergePage>();
						for (int j = 0; j < routeSourceNodesLen; j++) {
							if (routeSourceNodes[j].Name == "path") {
								//页面中src内地址，不需要替换为本地绝对路径
								routeSourceList.Add(filterValueByKey(routeSourceNodes[j].Attributes["path"].Value, dictProperty));
							} else if (routeSourceNodes[j].Name == "fileset") {
								//需要进行处理的页面地址集合
								ConfigTargetRouteMergePage mergePage = new ConfigTargetRouteMergePage();
								mergePage.Dir = getFilteredXmlNodeAttrVal(routeSourceNodes[j], "dir", dictProperty);
								mergePage.Includes = getFilteredXmlNodeAttrVal(routeSourceNodes[j], "includes", dictProperty);
								mergePage.Excludes = getFilteredXmlNodeAttrVal(routeSourceNodes[j], "excludes", dictProperty);

								mergePageList.Add(mergePage);
							}
						}
						Dictionary<string, object> dictRouteSub = new Dictionary<string, object>();
						//concat这里添加的单个对象
						dictRouteSub.Add("route", new ConfigTargetRoute(routeSourceList, routeTargetFile, routeEncoding, routeOutputEncoding, mergePageList));
						currentTargetList.Add(dictRouteSub);
						#endregion
						break;
					//合并文件
					case "concat":
						#region concat
						List<string> sourceList = new List<string>();
						//目标文件
						string targetFile = filterValueByKey(targetChildNodesI.Attributes["destfile"].Value, dictProperty);
						string encoding = string.Empty;
						string outputEncoding = string.Empty;

						encoding = getFilteredXmlNodeAttrVal(targetChildNodesI, "encoding", dictProperty);

						outputEncoding = getFilteredXmlNodeAttrVal(targetChildNodesI, "outputencoding", dictProperty);

						//文件来源集合
						XmlNodeList sourceNodes = targetChildNodesI.ChildNodes;
						int sourceNodesLen = sourceNodes.Count;
						for (int j = 0; j < sourceNodesLen; j++) {
							if (sourceNodes[j].Name == "path") {
								sourceList.Add(filterValueByKey(sourceNodes[j].Attributes["path"].Value, dictProperty));
							}
						}
						Dictionary<string, object> dictConcatSub = new Dictionary<string, object>();
						//concat这里添加的单个对象
						dictConcatSub.Add("concat", new ConfigTargetConcat(sourceList, targetFile, encoding, outputEncoding));
						currentTargetList.Add(dictConcatSub);
						#endregion
						break;
					//删除文件
					case "delete":
						#region delete
						XmlNodeList deleteNodes = targetChildNodesI.ChildNodes;
						int deleteNodesLen = deleteNodes.Count;
						//要删除的集合
						List<ConfigTargetDelete> filesetList = new List<ConfigTargetDelete>();
						for (int k = 0; k < deleteNodesLen; k++) {
							XmlNode deleteNodesK = deleteNodes[k];
							if (deleteNodes[k].Name == "fileset") {
								ConfigTargetDelete confTargetDelete = new ConfigTargetDelete();

								confTargetDelete.Dir = getFilteredXmlNodeAttrVal(deleteNodesK, "dir", dictProperty);

								confTargetDelete.Includes = getFilteredXmlNodeAttrVal(deleteNodesK, "includes", dictProperty);

								confTargetDelete.Excludes = getFilteredXmlNodeAttrVal(deleteNodesK, "excludes", dictProperty);

								filesetList.Add(confTargetDelete);
							}
						}
						Dictionary<string, object> dictDeleteSub = new Dictionary<string, object>();
						//delete这里是添加的一个list
						dictDeleteSub.Add("delete", filesetList);
						currentTargetList.Add(dictDeleteSub);
						#endregion
						break;
					//压缩(yui,gcc)
					case "compress":
						#region compress
						ConfigTargetCompress confTarCompress = new ConfigTargetCompress();
						//命令头
						confTarCompress.ExecuTable = getFilteredXmlNodeAttrVal(targetChildNodesI, "executable", dictProperty);
						confTarCompress.DestDir = getFilteredXmlNodeAttrVal(targetChildNodesI, "dest", dictProperty);
						XmlNodeList compressChildNodes = targetChildNodesI.ChildNodes;
						//拼接命令语句srcFile前部分
						confTarCompress.BeforeSrcFile = confTarCompress.ExecuTable;
						//遍历子节点
						int compressChildNodesCount = compressChildNodes.Count;
						bool hasSrcFile = false;
						for (int compressIndex = 0; compressIndex < compressChildNodesCount; compressIndex++) {
							XmlNode compressChiNodeI = compressChildNodes[compressIndex];
							switch (compressChiNodeI.Name) {
								case "arg":
									if (!hasSrcFile) {
										confTarCompress.BeforeSrcFile += " " + getFilteredXmlNodeAttrVal(compressChiNodeI, "line", dictProperty);
										if (compressChiNodeI.Attributes["path"] != null) {
											//compiler文件路径
											confTarCompress.BeforeSrcFile += " " + Path.GetFullPath(Path.Combine(rootPath, getFilteredXmlNodeAttrVal(compressChiNodeI, "path", dictProperty)));
										}
										confTarCompress.BeforeSrcFile += " " + getFilteredXmlNodeAttrVal(compressChiNodeI, "value", dictProperty);
									} else {
										confTarCompress.OutputArg += " " + getFilteredXmlNodeAttrVal(compressChiNodeI, "value", dictProperty);
									}
									break;
								case "fileset":
									//需要进行处理的页面地址集合
									BaseConfigTargetFilterFile baseConfTarFilterFile = new BaseConfigTargetFilterFile();
									baseConfTarFilterFile.Dir = getFilteredXmlNodeAttrVal(compressChiNodeI, "dir", dictProperty);
									baseConfTarFilterFile.Includes = getFilteredXmlNodeAttrVal(compressChiNodeI, "includes", dictProperty);
									baseConfTarFilterFile.Excludes = getFilteredXmlNodeAttrVal(compressChiNodeI, "excludes", dictProperty);
									confTarCompress.BaseConfTarFilterFileList.Add(baseConfTarFilterFile);
									break;
								case "srcfile":
									hasSrcFile = true;
									//本地绝对路径
									confTarCompress.SrcFile = Path.GetFullPath(Path.Combine(rootPath, getFilteredXmlNodeAttrVal(compressChiNodeI, "value", dictProperty)));
									break;
								case "mapper":
									confTarCompress.MapperType = getFilteredXmlNodeAttrVal(compressChiNodeI, "type", dictProperty);
									confTarCompress.MapperFrom = getFilteredXmlNodeAttrVal(compressChiNodeI, "from", dictProperty);
									confTarCompress.MapperTo = getFilteredXmlNodeAttrVal(compressChiNodeI, "to", dictProperty);
									break;
								case "targetfile":
									confTarCompress.TargetFile = Path.GetFullPath(Path.Combine(rootPath, getFilteredXmlNodeAttrVal(compressChiNodeI, "value", dictProperty)));
									break;
								default: break;
							}
						}
						Dictionary<string, object> dictCompressSub = new Dictionary<string, object>();
						//compress这里添加的单个对象
						dictCompressSub.Add("compress", confTarCompress);
						currentTargetList.Add(dictCompressSub);
						#endregion
						break;
					//单条命令
					case "command":
						#region command
						//工作目录
						string workDir = Path.GetFullPath(Path.Combine(rootPath, getFilteredXmlNodeAttrVal(targetChildNodesI, "workdir", dictProperty)));
						XmlNodeList commandNodeList = targetChildNodesI.ChildNodes;
						int commandListLen = commandNodeList.Count;
						List<string> commandList = new List<string>();
						string commandVal = string.Empty;
						//list
						for (int j = 0; j < commandListLen; j++) {
							XmlNode commandNode = commandNodeList[j];
							if (commandNode.Name == "arg") {
								commandVal = getFilteredXmlNodeAttrVal(commandNode, "value", dictProperty);
								if (commandVal != string.Empty) {
									commandList.Add(commandVal);
								}
							}
						}
						ConfigTargetCommand confTarCommand = new ConfigTargetCommand(workDir, commandList);
						Dictionary<string, object> dictCommandSub = new Dictionary<string, object>();
						//command这里添加的单个对象
						dictCommandSub.Add("command", confTarCommand);
						currentTargetList.Add(dictCommandSub);
						#endregion
						break;
					//批处理
					case "batch":
						#region batch
						XmlNodeList batchNodeList = targetChildNodesI.ChildNodes;
						int batchListLen = batchNodeList.Count;
						//bat集合
						List<ConfigTargetBatch> batchList = new List<ConfigTargetBatch>();
						string batchPath = string.Empty;
						string args = string.Empty;
						//list
						for (int j = 0; j < batchListLen; j++) {
							XmlNode batchNode = batchNodeList[j];
							if (batchNode.Name == "path") {
								batchPath = Path.GetFullPath(Path.Combine(rootPath, getFilteredXmlNodeAttrVal(batchNode, "path", dictProperty)));
								args = getFilteredXmlNodeAttrVal(batchNode, "args", dictProperty);
								batchList.Add(new ConfigTargetBatch(batchPath, args));
							}
						}
						Dictionary<string, object> dictBatchSub = new Dictionary<string, object>();
						//这里添加的bat文件信息列表
						dictBatchSub.Add("batch", batchList);
						currentTargetList.Add(dictBatchSub);

						#endregion
						break;
					//替换
					case "replace":
						#region replace
						ConfigTargetReplace confTarRep = new ConfigTargetReplace();
						//读取编码格式
						confTarRep.Encoding = getFilteredXmlNodeAttrVal(targetChildNodesI, "encoding", dictProperty);
						confTarRep.OutputEncoding = getFilteredXmlNodeAttrVal(targetChildNodesI, "outputencoding", dictProperty);
						//读取子节点
						XmlNodeList replaceNodeList = targetChildNodesI.ChildNodes;
						int replaceNodeCount = replaceNodeList.Count;
						for (int j = 0; j < replaceNodeCount; j++) {
							XmlNode repNode = replaceNodeList[j];
							switch (repNode.Name) {
								case "fileset":
									//需要进行处理的页面地址集合
									BaseConfigTargetFilterFile repFileSet = new BaseConfigTargetFilterFile();
									repFileSet.Dir = getFilteredXmlNodeAttrVal(repNode, "dir", dictProperty);
									repFileSet.Includes = getFilteredXmlNodeAttrVal(repNode, "includes", dictProperty);
									repFileSet.Excludes = getFilteredXmlNodeAttrVal(repNode, "excludes", dictProperty);

									confTarRep.FileSetList.Add(repFileSet);
									break;
								case "marknote":
									//起始标志的替换
									MarkNoteValueReplace markNoteValRep = new MarkNoteValueReplace();
									markNoteValRep.Start = getFilteredXmlNodeAttrVal(repNode, "start", dictProperty);
									markNoteValRep.End = getFilteredXmlNodeAttrVal(repNode, "end", dictProperty);
									markNoteValRep.NewValue = getFilteredXmlNodeAttrVal(repNode, "value", dictProperty);
									markNoteValRep.Path = Path.GetFullPath(Path.Combine(rootPath, getFilteredXmlNodeAttrVal(repNode, "path", dictProperty)));
									confTarRep.MarkNoteRepList.Add(markNoteValRep);
									break;
								case "mark":
									//直接替换标志key
									MarkValueReplace markValRep = new MarkValueReplace();
									markValRep.MarkKey = getFilteredXmlNodeAttrVal(repNode, "key", dictProperty);
									markValRep.NewValue = getFilteredXmlNodeAttrVal(repNode, "value", dictProperty);
									markValRep.Path = Path.GetFullPath(Path.Combine(rootPath, getFilteredXmlNodeAttrVal(repNode, "path", dictProperty)));
									confTarRep.MarkRepList.Add(markValRep);
									break;
								default: break;
							}
						}
						Dictionary<string, object> dictReplaceSub = new Dictionary<string, object>();
						//replace这里添加的单个对象
						dictReplaceSub.Add("replace", confTarRep);
						currentTargetList.Add(dictReplaceSub);
						#endregion
						break;
					default: break;
				}
			}
			LogOutput.logConsoleNow("end   read config info target");
			return currentTargetList;
		}
		/// <summary>
		/// 获取过滤后的xmlNode的属性值,无则返回空字符串
		/// </summary>
		/// <param name="xmlNode"></param>
		/// <param name="attr"></param>
		/// <returns></returns>
		public string getFilteredXmlNodeAttrVal(XmlNode xmlNode, string attr, Dictionary<string, string> dictProperty) {
			if (xmlNode.Attributes[attr] != null) {
				return filterValueByKey(xmlNode.Attributes[attr].Value, dictProperty);
			}
			return string.Empty;
		}
		/// <summary>
		/// 获取未过滤的原始属性值
		/// </summary>
		/// <param name="xmlNode"></param>
		/// <param name="attr"></param>
		/// <returns></returns>
		public string getXmlNodeAttrVal(XmlNode xmlNode, string attr) {
			if (xmlNode.Attributes[attr] != null) {
				return xmlNode.Attributes[attr].Value;
			}
			return string.Empty;
		}

		/// <summary>
		/// 替换值中的key
		/// </summary>
		/// <param name="filterValue"></param>
		/// <param name="dictProperty"></param>
		/// <returns></returns>
		public string filterValueByKey(string filterValue, Dictionary<string, string> dictProperty) {
			Regex preReg = new Regex(@"\$\{[^\}]+\}");
			if (preReg.IsMatch(filterValue)) {
				foreach (string key in dictProperty.Keys) {
					Regex reg = new Regex("\\$\\{" + key + "\\}");
					filterValue = reg.Replace(filterValue, dictProperty[key]);
				}
			}
			return filterValue;
		}
		#endregion

		/// <summary>
		/// 执行所有命令
		/// </summary>
		/// <param name="configInfo"></param>
		public void execByConfigInfo(ConfigInfo configInfo) {
			LogOutput.logConsoleNow("********start  exec command **********");
			List<List<Dictionary<string, object>>> allTargetList = configInfo.AllTargetList;
			int allTarLen = allTargetList.Count;
			string exStr = string.Empty;
			for (int i = 0; i < allTarLen; i++) {
				//单个target内信息集合，可能包括多个concat,route,delete等
				List<Dictionary<string, object>> singleTargetList = allTargetList[i];
				int dictListLen = singleTargetList.Count;
				for (int j = 0; j < dictListLen; j++) {
					//具体操作信息节点concat,delete
					Dictionary<string, object> dictItem = singleTargetList[j];
					//原则上只有1个<key,value>节点，dictionary的键值对不是按序排列的
					foreach (string dictKey in dictItem.Keys) {
						//定义switch中公共变量
						object dictValue = dictItem[dictKey];
						Object2StringCmd o2scmd = new Object2StringCmd();
						//执行属性节点
						switch (dictKey) {
							case "route":
								try {
									ConfigTargetRoute confTarRoute = (ConfigTargetRoute)dictValue;
									o2scmd.execRouteMergeCmd(confTarRoute, configInfo.RootPath);
									LogOutput.logConsoleNow("route finished");
								} catch (LogException logEx) {
									handleRuntimeEx(logEx);
								}
								break;
							case "concat":
								try {
									ConfigTargetConcat confTarConcat = (ConfigTargetConcat)dictValue;
									o2scmd.execConcatCmd(confTarConcat, configInfo.RootPath);
									LogOutput.logConsoleNow("concat finished");
								} catch (LogException logEx) {
									handleRuntimeEx(logEx);
								}
								break;
							case "delete":
								try {
									List<ConfigTargetDelete> cfdList = (List<ConfigTargetDelete>)dictValue;
									o2scmd.execDeleteCmd(cfdList, configInfo.RootPath);
									LogOutput.logConsoleNow("delete finished");
								} catch (LogException logEx) {
									handleRuntimeEx(logEx);
								}
								break;
							case "compress":
								try {
									ConfigTargetCompress confTarCompress = (ConfigTargetCompress)dictValue;
									o2scmd.execCompressCmd(confTarCompress, configInfo.RootPath);
									LogOutput.logConsoleNow("compress finished");
								} catch (LogException logEx) {
									handleRuntimeEx(logEx);
								}
								break;
							case "command":
								try {
									ConfigTargetCommand confTarCommand = (ConfigTargetCommand)dictValue;
									o2scmd.execCommandCmd(confTarCommand, configInfo.RootPath);
									LogOutput.logConsoleNow("command finished");
								} catch (LogException logEx) {
									handleRuntimeEx(logEx);
								}
								break;
							case "batch":
								try {									
									List<ConfigTargetBatch> confTarBatchList = (List<ConfigTargetBatch>)dictValue;
									o2scmd.execBatchCmd(confTarBatchList, configInfo.RootPath);
									LogOutput.logConsoleNow("batch finished");
								} catch (LogException logEx) {
									handleRuntimeEx(logEx);
								}
								break;
							case "replace":
								try {
									ConfigTargetReplace confTarReplace = (ConfigTargetReplace)dictValue;
									o2scmd.execReplaceCmd(confTarReplace, configInfo.RootPath);
									LogOutput.logConsoleNow("replace finished");
								} catch (LogException logEx) {
									handleRuntimeEx(logEx);
								}
								break;
							default: break;
						}
					}
				}
			}
			LogOutput.logConsoleNow("********All command  finished!**********");
		}

		/// <summary>
		/// 执行某个节点导致异常后的中断、继续判断
		/// </summary>
		/// <param name="logEx"></param>
		public void handleRuntimeEx(LogException logEx) {
			Console.WriteLine("节点执行出现异常，是否中止执行：");
			Console.WriteLine("--输入 continue 继续执行下面内容"); 
			Console.WriteLine("--其他输入将中止执行");
			//异常后接受输入字符,判断是否继续执行
			if (Console.ReadLine() != "continue") {
				throw new LogException("ConfigAccess 执行节点失败", logEx);
			} else {
				LogOutput.logConsole(new LogException("ConfigAccess 执行节点失败", logEx));
			}
		}

	}
}
