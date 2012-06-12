using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LckeyAnt
{
	public class RunCommand
	{

		#region run outin command
		/// <summary>
		/// 调用bat,传入参数args,只能使用传入的参数执行一次
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="args"></param>
		public static void callBat(string fileName, string args) {
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.EnableRaisingEvents = false;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.CreateNoWindow = false;//true
			//p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardOutput = true;
			// @"C:\Documents and Settings\thomas\桌面\test\t.bat";
			LogOutput.logConsoleNow(fileName);
			p.StartInfo.FileName = fileName;
			//p.StartInfo.WorkingDirectory = @"E:\";
			p.StartInfo.Arguments = args;//传入bat的参数
			p.StartInfo.LoadUserProfile = false;
			p.Start();
			string ret = p.StandardOutput.ReadToEnd(); //获取返回值
			LogOutput.logConsoleNow(ret);
			p.WaitForExit();
			p.Close();
		}
		/**
		 *http://msdn.microsoft.com/en-us/library/h6ak8zt5.aspx
		 *http://forum.codecall.net/topic/49702-running-command-line-with-processstart/
		 *
		 *如：Process.Start("IExplore.exe", "www.northwindtraders.com");
		 *    Process.Start("ftp.exe","xx.xx.xx.xxx");
		 *    Process.Start("cmd.exe","/copy xxx");
		 */
		/// <summary>
		/// 显示cmd窗口自己调用
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="args"></param>
		public static void callSysExe(string fileName, string args) {
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.StartInfo.UseShellExecute = true;
			p.StartInfo.CreateNoWindow = false;//true
			//p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardOutput = true;
			//System.Diagnostics.Process.Start("IExplore.exe", "www.baidu.com");
			//直接使用命令则是默认调用cmd.exe
			//System.Diagnostics.Process.Start("ipconfig");
			System.Diagnostics.Process.Start(fileName, args);

		}

		/// <summary>
		/// 使用cmd环境执行命令，同步执行一句传入命令
		/// </summary>
		/// http://www.cnblogs.com/wucg/archive/2012/03/16/2399980.html
		/// <param name="workDir"></param>
		/// <param name="cmdStr"></param>
		public static void callCmdSync(string workDir, string cmdStr) {
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.EnableRaisingEvents = false;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.CreateNoWindow = false;//true
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.FileName = "cmd.exe"; //fileName;
			p.StartInfo.WorkingDirectory = workDir;//@"E:\";
			//p.StartInfo.Arguments = args;//传入bat的参数
			p.StartInfo.LoadUserProfile = false;
			p.Start();
			//输出到命令行
			p.StandardInput.WriteLine(cmdStr);
			p.StandardInput.WriteLine("exit");
			//捕获不到类似 dirxxx的错误信息
			//string ret = p.StandardOutput.ReadToEnd(); //获取返回值
			//LogOutput.logConsoleNow(ret);			
			/* 按行获取返回值 */
			string line = p.StandardOutput.ReadLine();//每次读取一行
			while (!p.StandardOutput.EndOfStream) {
				if (line != string.Empty) {
					LogOutput.logConsoleNow(line + " ");
				}
				line = p.StandardOutput.ReadLine();
			}
			p.WaitForExit();
			p.Close();
			LogOutput.logConsoleNow("--cmd over--");
		}

		/// <summary>
		/// 使用cmd环境执行命令，同步执行一句传入命令
		/// </summary>
		/// http://www.cnblogs.com/wucg/archive/2012/03/16/2399980.html
		/// <param name="workDir"></param>
		/// <param name="cmdStr"></param>
		public static void callCmdSync(string workDir, List<string> cmdList) {
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.EnableRaisingEvents = false;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.CreateNoWindow = false;//true
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.FileName = "cmd.exe"; //fileName;
			p.StartInfo.WorkingDirectory = workDir;//@"E:\";
			//p.StartInfo.Arguments = args;//传入bat的参数
			p.StartInfo.LoadUserProfile = false;
			p.Start();
			LogOutput.logConsole("pause  会被主动过滤掉，会引起下面传入参数错误");
			//输出到命令行
			foreach (string cmdStr in cmdList) {
				LogOutput.logConsole(cmdStr);
				//过滤掉pause情况，会引起下面传入错误
				if (cmdStr.ToLower() != "pause") {
					p.StandardInput.WriteLine(cmdStr);
					p.StandardInput.Flush();
				}
			}
			p.StandardInput.WriteLine("exit");
			//捕获不到类似 dirxxx的错误信息
			//string ret = p.StandardOutput.ReadToEnd(); //获取返回值
			//LogOutput.logConsoleNow(ret);			
			/* 按行获取返回值 */
			string line = p.StandardOutput.ReadLine();//每次读取一行
			while (!p.StandardOutput.EndOfStream) {
				if (line != string.Empty) {
					LogOutput.logConsoleNow(line + " ");
				}
				line = p.StandardOutput.ReadLine();
			}
			p.WaitForExit();
			p.Close();
			LogOutput.logConsoleNow("--cmd over--");
		}

		#endregion
	}
}
