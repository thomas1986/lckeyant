using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LckeyAnt
{
	public class CmdProcess
	{
		public static void processArgs(string[] args) {
			LogOutput.log("#####################################################");
			LogOutput.logConsoleNow("Application start");
			if (args.Length > 0) {
				string _argsStr = string.Empty;
				for (int i = 0; i < args.Length; i++) {
					_argsStr += args[i];
				}
				try {
					LogOutput.logConsoleNow("start process " + _argsStr);
					processByArgs(args);
					LogOutput.logConsoleNow("end   process " + _argsStr);
				} catch (Exception ex) {

					LogOutput.logConsole(new LogException("执行命令参数 " + _argsStr + " 出错：" + ex.Message, ex));
				}
			} else {
				//启动程序时，args无值传入
				Console.WriteLine("************************************");
				Console.WriteLine("**提示信息：");
				Console.WriteLine("* 1,使用lckeyant -? 获取提示信息");
				Console.WriteLine(@"* 2,目录路径中间用空格，则在整个目录加双引号 'C:\documents and settings'\xxx");
				Console.WriteLine("************************************");

				//RunCommand.callCmdSync(@"G:\ant_test", @"java -jar compiler.jar --js=a.js --js_output_file=a.min.js");

				string consReadLine = Console.ReadLine();
				while (consReadLine != "exit") {
					string[] consArgs = consReadLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					string[] consArgsShift = new string[consArgs.Length - 1];
					for (int i = 1; i < consArgs.Length; i++) {
						consArgsShift[i - 1] = consArgs[i];
					}
					try {
						LogOutput.logConsoleNow("start process " + consReadLine);
						processByArgs(consArgsShift);
						LogOutput.logConsoleNow("end   process " + consReadLine);
					}/* catch (LogException logEx) {
						LogOutput.logConsole(new LogException("执行命令 " + consReadLine + " 出错：" + logEx.LogMessage, logEx));
					}*/
						catch (Exception ex) {
						LogOutput.logConsole(new LogException("执行命令 " + consReadLine + " 出错：" + ex.Message, ex));
					}
					consReadLine = Console.ReadLine();
				}
				//调用系统命令行
				//string cmdStr= Console.ReadLine();
				//Combine.callSysCmd("cmd.exe", cmdStr);
				////RunCommand.callBat("cmd.exe", "");
				//Combine.callSysCmd("cmd.exe","build_project_bat -?");
				////Console.WriteLine("ok");
			}
		}

		/// <summary>
		/// 接受命令参数，判断进入执行分支
		/// </summary>
		/// <param name="args"></param>
		public static void processByArgs(string[] args) {
			if (args.Length > 0) {
				if (args[0].Substring(0, 1) == "-") {
					//清空
					GlobalCurrent.RUNTIME_LOGEXCEPTION_LIST = new List<LogException>();

					#region 接受参数，执行命令

					string cmdType = args[0].Substring(1);
					//参数都转成小写,不区分大小写
					cmdType = cmdType.ToLower();
					switch (cmdType) {
						//根据xml执行所有操作
						case "build":
							ConfigAccess confAccess = new ConfigAccess();
							//可以只传入第2个参数,不传入根路径
							string args2 = args.Length < 3 ? string.Empty : args[2];
							confAccess.execByConfigInfo(confAccess.getConfigInfo(args[1], args2));
							break;
						#region route
						case "route_page_srcs2target":
							//currentPage,srcs,target,rootPath,encoding,outputencoding
							MergeRoute.repSinglePageSingleItem(args[1], args[2], args[3], args[4], args[5], args[6]);
							break;
						case "route_page_srcs2targets":
							//currentPage,combineFiles,encoding,outputencoding
							MergeRoute.repSinglePageMultiItems(args[1], args[2], args[3], args[4]);
							break;
						case "route_pages_srcs2targets":
							//htmlPages,combineFiles,encoding,outputencoding
							MergeRoute.repMultiPagesMultiItems(args[1], args[2], args[3], args[4]);
							break;
						#endregion

						#region command
						case "cmd":
							//使用系统cmd.exe执行命令
							//注意args[2]使用 "" 包含起来
							RunCommand.callCmdSync(args[1], args[2]);
							break;
						case "bat":
							//执行其他bat文件内容的命令
							RunCommand.callBat(args[1], args[2]);
							break;
						case "run":
							//调用系统可执行程序:(程序名,参数)
							RunCommand.callSysExe(args[1], args[2]);
							break;
						#endregion

						case "concat_all":
							//(string concatUriDetail, string encoding, string outputencoding
							ConcatFile.concatFilesByUriDetail(args[1], args[2], args[3]);
							break;
						case "concat":
							//string sourceFilePath, string targetFilePath, string encoding, string outputEncoding
							ConcatFile.concatFileSource2Target(args[1], args[2], args[3], args[4]);
							break;
						case "replace":
							Console.WriteLine("none...");
							break;
						case "delete":
							//arg1...argN...
							int len = args.Length;
							for (int i = 0; i < len; i++) {
								//以工作根目录为参照路径
								string delPath = Path.Combine(GlobalCurrent.PROJECT_WORKSPACE, args[i]);
								if (File.Exists(delPath)) {
									File.Delete(delPath);
								}
							}
							break;
						case "?":
							//帮助
							string arg1 = string.Empty;
							if (args.Length > 1) {
								arg1 = args[1];
							}
							showHelpInfo(arg1);
							break;
						case "exit":
							//退出
							Console.WriteLine("exit");
							break;
						default:
							showHelpInfo("all");
							break;
					}

					#endregion

				}
			} else {
				LogOutput.logConsoleNow("缺少参数");
			}
		}


		/// <summary>
		/// 显示提示信息 build_project_bat -? helpBranch
		/// </summary>
		/// <param name="helpBranch"></param>
		public static void showHelpInfo(string helpBranch) {
			if (helpBranch == string.Empty || helpBranch == null)
				helpBranch = "all";

			//Console.WriteLine("\r\n");
			Console.WriteLine("********************");
			Console.WriteLine("* 命令不区分大小写");
			Console.WriteLine("* exit 退出");
			Console.WriteLine("* build_project_bat -命令名 参数1 参数2 ... 参数N");
			Console.WriteLine("***");
			switch (helpBranch) {
				case "all":
					Console.WriteLine("* [根据xml执行所有]");
					Console.WriteLine("* -exec xmlpath rootpath  ");
					Console.WriteLine("* [合并当前页面中的srcs路径引用(js,css)为target路径]：");
					Console.WriteLine("* -route_page_srcs2target currentPage srcs target rootPath encoding outputencoding ");
					Console.WriteLine("* [合并当前页面中的多种srcs路径引用(js,css)为对应的多个target路径]：");
					Console.WriteLine("* -route_page_srcs2targets currentPage combineFiles encoding outputencoding ");
					Console.WriteLine("* [多页面对多(js,css)=>多target个的(js,css)]：");
					Console.WriteLine("* -route_pages_srcs2targets htmlPageFiles combineFiles encoding outputencoding ");
					Console.WriteLine("* [传入阐述batArgs到fileName的bat文件执行批处理任务]：");
					Console.WriteLine("* -bat fileName batArgs");
					Console.WriteLine("* [在工作目录workDir内执行cmdStr命令]：");
					Console.WriteLine("* -cmd workDir cmdStr ");
					Console.WriteLine("* [传入参数args执行可执行命令如ie.exe]：");
					Console.WriteLine("* -run execName args ");
					Console.WriteLine("* [拼接多个文件内容,读取编码，输出编码格式]：");
					Console.WriteLine("* -concat_all combineFiles encoding outputencoding ");
					Console.WriteLine("* [将源目录文件内容追加到目标文件中]：");
					Console.WriteLine("* -concat sourceFilePath targetFilePath  encoding  outputEncoding  ");
					Console.WriteLine("* [删除arg1-N文件,可以相对于工作根目录的路径]：");
					Console.WriteLine("* -delete arg1 arg2 ...  argN  ");

					//提示信息
					Console.WriteLine("* ------参数example--------");
					Console.WriteLine(@"* ------webPages与htmlPageFiles同 如 E:\xx\xx\root\index.html,E:\xx\default.html");
					Console.WriteLine("* ------srcs 如: a.js,b.js,c.js");
					Console.WriteLine("* ------target 如: a_b_c.js");
					Console.WriteLine(@"* ------combineFiles 如: C:\xxx\root::a.js,b.js,c.js=>a_b_c.js ");
					Console.WriteLine(@"* ------htmlPageFiles 如: E:\xx\xx\root\index.html,E:\xx\default.html");
					Console.WriteLine("********************");
					break;
				default: break;

			}

		}

	}
}
