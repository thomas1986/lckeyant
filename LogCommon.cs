using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace LckeyAnt
{
	public class LogCommon
	{
		private static LogCommon instance = null;
		private static readonly object lockObj = new object();
		private string m_strLogFileName = null;                 //文件名称
		private string rootPath = GlobalCurrent.PROJECT_WORKSPACE + GlobalCurrent.LOG_ROOT_DIR;//日志根目录
		private const int MAX_FILE_LEN = 20 * 1024 * 1024;	    // 定义一个缺省的日志文件尺寸20M   
		private static Mutex m_WriteMutex = new Mutex();//互斥

		public LogCommon(string fileName) {
			CreateLogFile(fileName);
		}
		/// <summary>
		/// Instance 获取单件类的实例
		/// </summary>
		public static LogCommon Instance {
			get {
				lock (lockObj) {
					if (instance == null) {
						instance = new LogCommon(DateTime.Now.ToString("yyyy-MM-dd"));
					}
					if (instance.m_strLogFileName.IndexOf(DateTime.Now.ToString("yyyy-MM-dd")) == -1) {
						instance.CreateLogFile(DateTime.Now.ToString("yyyy-MM-dd"));
					}
				}
				return instance;
			}
		}
		/// <summary>
		///CreateLogFile( string strLogPathFile )
		/// 检查和构造路径
		/// </summary>
		/// <param name="strLogPathFile"></param>
		private void CreateLogFile(string strLogPathFile) {
			if (!Directory.Exists(rootPath)) {

				Directory.CreateDirectory(rootPath);

			}
			if (File.Exists(rootPath + "\\" + strLogPathFile + ".txt")) {
				FileInfo fileInfoTmp = new FileInfo(rootPath + "\\" + strLogPathFile + ".txt");
				if (fileInfoTmp.Length > MAX_FILE_LEN) {
					m_strLogFileName = rootPath + "\\" + strLogPathFile + "_1.txt";
				} else {
					m_strLogFileName = rootPath + "\\" + strLogPathFile + ".txt";
				}
			} else {
				m_strLogFileName = rootPath + "\\" + strLogPathFile + ".txt";
			}
		}

		#region write
		
		/// <summary>
		/// 输出单行信息
		/// </summary>
		/// <param name="msg"></param>
		public void WriteLine(string msg) {
			WriteByAction((streamWriterLog) => {
				streamWriterLog.WriteLine(msg);
			});
		}

		/// <summary>
		/// 输出多行msg
		/// </summary>
		/// <param name="msgs"></param>
		public void WriteLine(string[] msgs) {
			WriteByAction((streamWriterLog) => {
				int len = msgs.Length;
				for (int i = 0; i < len; i++) {
					streamWriterLog.WriteLine(msgs[i]);
				}
			});
		}

		/// <summary>
		/// 输出多行msg
		/// </summary>
		/// <param name="msgs"></param>
		public void WriteLine(List<string> msgList) {
			WriteByAction((streamWriterLog) => {
				foreach (string msg in msgList) {
					streamWriterLog.WriteLine(msg);
				}
			});
		}

		/// <summary>
		/// 不分多行输出msg
		/// </summary>
		/// <param name="msgs"></param>
		public void Write(string[] msgs) {
			WriteByAction((streamWriterLog) => {
				StringBuilder sb = new StringBuilder();
				int len = msgs.Length;
				for (int i = 0; i < len; i++) {
					sb.Append(msgs[i]);
				}
				streamWriterLog.WriteLine(sb.ToString());
			});
		}

		/// <summary>
		/// 不分多行输出msg
		/// </summary>
		/// <param name="msgs"></param>
		public void Write(List<string> msgList) {
			WriteByAction((streamWriterLog) => {
				StringBuilder sb = new StringBuilder();
				foreach (string msg in msgList) {
					sb.Append(msg);
				}
				streamWriterLog.WriteLine(sb.ToString());
			});
		}
		/// <summary>
		/// 写入基方法， 供外部action调用，写入日志
		/// </summary>
		/// <param name="outStreamWriteAction"></param>
		public void WriteByAction(Action<StreamWriter> outStreamWriteAction) {
			FileStream fileStreamLog = null;      //文件流
			StreamWriter streamWriterLog = null;  //输出流

			m_WriteMutex.WaitOne();
			//logEx.TerminalID,logEx.ExceptionType,logEx.TypeName,logEx.LogMessage,logEx.ExceptionTime

			//string strLineText = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") + " " + iLevel.ToString() + " " + strText;
			try {
				fileStreamLog = new FileStream(m_strLogFileName, FileMode.Append, FileAccess.Write, FileShare.None);
				fileStreamLog.Seek(0, System.IO.SeekOrigin.End);
				streamWriterLog = new StreamWriter(fileStreamLog, System.Text.Encoding.UTF8);
				//供外部调用write数据
				outStreamWriteAction(streamWriterLog);
				if (streamWriterLog != null) {
					streamWriterLog.Close();
					streamWriterLog = null;
				}
				if (fileStreamLog != null) {
					fileStreamLog.Close();
					fileStreamLog = null;
				}

			} catch (Exception) {

			} finally {

				if (streamWriterLog != null) {
					streamWriterLog.Close();
					streamWriterLog = null;
				}
				if (fileStreamLog != null) {
					fileStreamLog.Close();
					fileStreamLog = null;
				}
			}
			m_WriteMutex.ReleaseMutex();
		}
		
		#endregion
	}
}
