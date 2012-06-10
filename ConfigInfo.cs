using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LckeyAnt
{
	/// <summary>
	/// 完整config信息
	/// </summary>
	public class ConfigInfo
	{
		string rootPath = string.Empty;
		Dictionary<string, string> dictProperty = new Dictionary<string, string>();
		List<List<Dictionary<string, object>>> allTargetList = new List<List<Dictionary<string, object>>>();

		public ConfigInfo() { }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="_dictProperty">所有定义的属性键值对</param>
		/// <param name="_allTargetList">所有target节点属性集合</param>
		public ConfigInfo(Dictionary<string, string> _dictProperty, List<List<Dictionary<string, object>>> _allTargetList) {
			dictProperty = _dictProperty;
			allTargetList = _allTargetList;
		}
		/// <summary>
		/// 配置文件执行的根目录，默认为xml配置文件所在文件夹
		/// </summary>
		public string RootPath {
			get { return rootPath; }
			set { rootPath = value; }
		}

		/// <summary>
		/// 所有定义的属性键值对
		/// </summary>
		public Dictionary<string, string> DictProperty {
			get { return dictProperty; }
			set { dictProperty = value; }
		}
		/// <summary>
		/// 所有target节点属性集合
		/// </summary>
		public List<List<Dictionary<string, object>>> AllTargetList {
			get { return allTargetList; }
			set { allTargetList = value; }
		}

	}

	/// <summary>
	/// route merge 操作，路径合并
	/// </summary>
	public class ConfigTargetRoute
	{
		private List<string> routeSourceList = new List<string>();
		private string routeTargetFile = string.Empty;
		private string encoding = string.Empty;
		private string outputEncoding = string.Empty;
		private List<ConfigTargetRouteMergePage> mergePageList = new List<ConfigTargetRouteMergePage>();

		public ConfigTargetRoute() { }
		public ConfigTargetRoute(List<string> _sourceList, string _targetFile, string _encoding, string _outputEncoding, List<ConfigTargetRouteMergePage> _mergePageList) {
			routeSourceList = _sourceList;
			routeTargetFile = _targetFile;
			encoding = _encoding;
			outputEncoding = _outputEncoding;
			mergePageList = _mergePageList;
		}
		/// <summary>
		/// 子路径src集合,类似 [a.js,b.js,c.js]...
		/// </summary>
		public List<string> RouteSourceList {
			get { return routeSourceList; }
			set { routeSourceList = value; }
		}
		/// <summary>
		/// 合并后路径名称,a_b_c.js
		/// </summary>
		public string TargetFile {
			get { return routeTargetFile; }
			set { routeTargetFile = value; }
		}
		/// <summary>
		/// 替换路径的页面读取使用的编码格式
		/// </summary>
		public string Encoding {
			get { return encoding; }
			set { encoding = value; }
		}
		/// <summary>
		/// 替换路径的页面写入使用的编码格式
		/// </summary>
		public string OutputEncoding {
			get { return outputEncoding; }
			set { outputEncoding = value; }
		}
		/// <summary>
		/// 需要进行route修改的页面信息集合
		/// </summary>
		public List<ConfigTargetRouteMergePage> MergePageList {
			get { return mergePageList; }
			set { mergePageList = value; }
		}
	}

	/// <summary>
	/// concat 操作
	/// </summary>
	public class ConfigTargetConcat
	{
		private List<string> sourceList = new List<string>();
		private string targetFile = string.Empty;
		private string encoding = string.Empty;
		private string outputEncoding = string.Empty;

		public ConfigTargetConcat() { }
		public ConfigTargetConcat(List<string> _sourceList, string _targetFile, string _encoding, string _outputEncoding) {
			sourceList = _sourceList;
			targetFile = _targetFile;
			encoding = _encoding;
			outputEncoding = _outputEncoding;
		}

		public List<string> SourceList {
			get { return sourceList; }
			set { sourceList = value; }
		}
		public string TargetFile {
			get { return targetFile; }
			set { targetFile = value; }
		}
		public string Encoding {
			get { return encoding; }
			set { encoding = value; }
		}
		public string OutputEncoding {
			get { return outputEncoding; }
			set { outputEncoding = value; }
		}
	}

	/// <summary>
	/// 过滤文件类；routemergePage,delete的父类
	/// </summary>
	public class BaseConfigTargetFilterFile
	{
		private string dir = string.Empty;
		private string includes = string.Empty;
		private string excludes = string.Empty;


		public BaseConfigTargetFilterFile() { }

		public BaseConfigTargetFilterFile(string _dir, string _includes, string _excludes) {
			dir = _dir;
			includes = _includes;
			excludes = _excludes;
		}

		public string Dir {
			get { return dir; }
			set { dir = value; }
		}

		public string Includes {
			get { return includes; }
			set { includes = value; }
		}
		public string Excludes {
			get { return excludes; }
			set { excludes = value; }
		}

	}

	/// <summary>
	/// delete操作
	/// </summary>
	public class ConfigTargetDelete : BaseConfigTargetFilterFile
	{
		/*
			private string dir = string.Empty;
			private string includes = string.Empty;
			private string excludes = string.Empty;


			public ConfigTargetDelete() { }

			public ConfigTargetDelete(string _dir, string _includes, string _excludes) {
				dir = _dir;
				includes = _includes;
				excludes = _excludes;
			}

			public string Dir {
				get { return dir; }
				set { dir = value; }
			}

			public string Includes {
				get { return includes; }
				set { includes = value; }
			}
			public string Excludes {
				get { return excludes; }
				set { excludes = value; }
			}
		 * */
		public string DeleteId {
			get;
			set;
		}
	}

	/// <summary>
	/// route需要修改的页面信息
	/// </summary>
	public class ConfigTargetRouteMergePage : BaseConfigTargetFilterFile
	{
		/*
				private string dir = string.Empty;
				private string includes = string.Empty;
				private string excludes = string.Empty;


				public ConfigTargetRouteMergePage() { }

				public ConfigTargetRouteMergePage(string _dir, string _includes, string _excludes) {
					dir = _dir;
					includes = _includes;
					excludes = _excludes;
				}

				public string Dir {
					get { return dir; }
					set { dir = value; }
				}

				public string Includes {
					get { return includes; }
					set { includes = value; }
				}
				public string Excludes {
					get { return excludes; }
					set { excludes = value; }
				}
		 * */
		public string RouteMergeId {
			get;
			set;
		}
	}

	/// <summary>
	/// compress 压缩js,css文件选项
	/// </summary>
	public class ConfigTargetCompress
	{
		private List<BaseConfigTargetFilterFile> baseConfTarFilterFileList = new List<BaseConfigTargetFilterFile>();
		private string executable = string.Empty;
		private string destDir = string.Empty;
		private string compressorFilePath = string.Empty;
		private string beforeSrcFile = string.Empty;
		private string srcFile = string.Empty;
		private string outputArg = string.Empty;
		private string mapperType = string.Empty;
		private string mapperFrom = string.Empty;
		private string mapperTo = string.Empty;
		private string targetFile = string.Empty;

		public ConfigTargetCompress() { }

		/// <summary>
		/// 包含的符合条件文件集合
		/// </summary>
		public List<BaseConfigTargetFilterFile> BaseConfTarFilterFileList {
			get { return baseConfTarFilterFileList; }
			set { baseConfTarFilterFileList = value; }
		}
		/// <summary>
		/// cmd命令头 如 java
		/// </summary>
		public string ExecuTable {
			get { return executable; }
			set { executable = value; }
		}
		/// <summary>
		/// 压缩后的目标文件夹
		/// </summary>
		public string DestDir {
			get { return destDir; }
			set { destDir = value; }
		}
		/// <summary>
		/// 压缩程序jar路径
		/// </summary>
		public string CompressorFilePath {
			get { return compressorFilePath; }
			set { compressorFilePath = value; }
		}
		/// <summary>
		/// 待压缩文件前的命令参数字符串集合
		/// </summary>
		public string BeforeSrcFile {
			get { return beforeSrcFile; }
			set { beforeSrcFile = value; }
		}
		/// <summary>
		/// 待压缩的文件，有BaseConfTarFilterFileList结果，则此项可不填
		/// </summary>
		public string SrcFile {
			get { return srcFile; }
			set { srcFile = value; }
		}
		/// <summary>
		/// 输出文件的参数前缀
		/// </summary>
		public string OutputArg {
			get { return outputArg; }
			set { outputArg = value; }
		}
		/// <summary>
		/// 匹配类型
		/// </summary>
		public string MapperType {
			get { return mapperType; }
			set { mapperType = value; }
		}
		/// <summary>
		/// 从此格式的源文件名
		/// </summary>
		public string MapperFrom {
			get { return mapperFrom; }
			set { mapperFrom = value; }
		}
		/// <summary>
		/// 压缩成此格式的输出文件名
		/// </summary>
		public string MapperTo {
			get { return mapperTo; }
			set { mapperTo = value; }
		}
		/// <summary>
		/// 压缩目标文件,有mapper->regexpto结果，则此项可不填
		/// </summary>
		public string TargetFile {
			get { return targetFile; }
			set { targetFile = value; }
		}
	}

	/// <summary>
	/// command 执行命令语句集合
	/// </summary>
	public class ConfigTargetCommand
	{
		private string workDir = string.Empty;
		private List<string> commandList = new List<string>();

		public ConfigTargetCommand() { }
		public ConfigTargetCommand(string _workDir, List<string> _commandList) {
			workDir = _workDir;
			commandList = _commandList;
		}
		/// <summary>
		/// 工作目录
		/// </summary>
		public string WorkDir {
			get { return workDir; }
			set { workDir = value; }
		}
		/// <summary>
		/// 命令列表
		/// </summary>
		public List<string> CommandList {
			get { return commandList; }
			set { commandList = value; }
		}

	}

	/// <summary>
	/// batch 执行批处理的文件
	/// </summary>
	public class ConfigTargetBatch
	{
		/*
		private List<string> batchPathList = new List<string>();
		public ConfigTargetBatch(List<string> _batchPathList) {
			batchPathList = _batchPathList;
		}
		/// <summary>
		/// 命令文件地址列表
		/// </summary>
		public List<string> BatchList {
			get { return batchPathList; }
			set { batchPathList = value; }
		}
		 * */
		private string path = string.Empty;
		private string args = string.Empty;

		public ConfigTargetBatch() { }
		public ConfigTargetBatch(string _path, string _args) {
			path = _path;
			_args = args;
		}
		/// <summary>
		/// bat文件路径
		/// </summary>
		public string Path {
			get { return path; }
			set { path = value; }
		}
		/// <summary>
		/// 传入bat文件的参数
		/// </summary>
		public string Args {
			get { return args; }
			set { args = value; }
		}

	}

	/// <summary>
	/// replace 替换内容，替换的内容是marknote,mark的并集,只要符合的都替换
	/// </summary>
	public class ConfigTargetReplace
	{
		private string encoding = string.Empty;
		private string outputEncoding = string.Empty;
		private List<BaseConfigTargetFilterFile> fileSetList = new List<BaseConfigTargetFilterFile>();
		private List<MarkNoteValueReplace> markNoteRepList = new List<MarkNoteValueReplace>();
		private List<MarkValueReplace> markRepList = new List<MarkValueReplace>();

		#region constructor
		public ConfigTargetReplace() { }
		/// <summary>
		/// 记录编码的构造函数
		/// </summary>
		/// <param name="_encoding"></param>
		/// <param name="_outputEncoding"></param>
		/// <param name="_fileSetList"></param>
		/// <param name="_markNoteRepList"></param>
		/// <param name="_markRepList"></param>
		public ConfigTargetReplace(string _encoding, string _outputEncoding, List<BaseConfigTargetFilterFile> _fileSetList, List<MarkNoteValueReplace> _markNoteRepList, List<MarkValueReplace> _markRepList) {
			encoding = _encoding;
			outputEncoding = _outputEncoding;
			fileSetList = _fileSetList;
			markNoteRepList = _markNoteRepList;
			markRepList = _markRepList;
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="_fileSetList"></param>
		/// <param name="_markNoteRepList"></param>
		/// <param name="_markRepList"></param>
		public ConfigTargetReplace(List<BaseConfigTargetFilterFile> _fileSetList, List<MarkNoteValueReplace> _markNoteRepList, List<MarkValueReplace> _markRepList) {
			fileSetList = _fileSetList;
			markNoteRepList = _markNoteRepList;
			markRepList = _markRepList;
		}
		#endregion

		#region property
		/// <summary>
		/// 读取的源文件编码格式
		/// </summary>
		public string Encoding {
			get { return encoding; }
			set { encoding = value; }
		}
		/// <summary>
		/// 写入文件的编码格式
		/// </summary>
		public string OutputEncoding {
			get { return outputEncoding; }
			set { outputEncoding = value; }
		}
		/// <summary>
		/// 文件集合
		/// </summary>
		public List<BaseConfigTargetFilterFile> FileSetList {
			get { return fileSetList; }
			set { fileSetList = value; }
		}
		/// <summary>
		/// 起始结束标志内容替换集合
		/// </summary>
		public List<MarkNoteValueReplace> MarkNoteRepList {
			get { return markNoteRepList; }
			set { markNoteRepList = value; }
		}
		/// <summary>
		/// 标志内容替换集合
		/// </summary>
		public List<MarkValueReplace> MarkRepList {
			get { return markRepList; }
			set { markRepList = value; }
		}
		#endregion
	}

	/// <summary>
	/// 起始结束标志来替换内 (页面上最好只出现一次，所有相同的 start-内容-end都将被一次替换)
	/// </summary>
	public class MarkNoteValueReplace
	{
		private string start = string.Empty;
		private string end = string.Empty;
		private string newValue = string.Empty;
		private string path = string.Empty;

		public MarkNoteValueReplace() { }
		public MarkNoteValueReplace(string _start, string _end, string _newValue, string _path) {
			start = _start;
			end = _end;
			newValue = _newValue;
			path = _path;
		}
		/// <summary>
		/// 替换起始标志
		/// </summary>
		public string Start {
			get { return start; }
			set { start = value; }
		}
		/// <summary>
		/// 替换结束标志
		/// </summary>
		public string End {
			get { return end; }
			set { end = value; }
		}
		/// <summary>
		/// 将要替换进来的新值
		/// </summary>
		public string NewValue {
			get { return newValue; }
			set { newValue = value; }
		}
		/// <summary>
		/// 将要从此路径中取值用来替换
		/// </summary>
		public string Path {
			get { return path; }
			set { path = value; }
		}

	}
	/// <summary>
	/// 替换标志内容
	/// </summary>
	public class MarkValueReplace
	{
		private string markKey = string.Empty;
		private string newValue = string.Empty;
		private string path = string.Empty;

		public MarkValueReplace() { }
		public MarkValueReplace(string _markKey, string _newValue, string _path) {
			markKey = _markKey;
			newValue = _newValue;
			path = _path;
		}
		/// <summary>
		/// 待替换的标志
		/// </summary>
		public string MarkKey {
			get { return markKey; }
			set { markKey = value; }
		}
		/// <summary>
		/// 用来替换标志的值
		/// </summary>
		public string NewValue {
			get { return newValue; }
			set { newValue = value; }
		}
		/// <summary>
		/// 获取路径文件内容来替换标志
		/// </summary>
		public string Path {
			get { return path; }
			set { path = value; }
		}

	}

}
