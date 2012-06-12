using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LckeyAnt
{
	public class LogOutput
	{
		#region log

		/// <summary>
		/// 输出异常信息到日志
		/// </summary>
		/// <param name="logEx"></param>
		public static void log(LogException logEx) {
			//记录异常到全局
			GlobalCurrent.RUNTIME_LOGEXCEPTION_LIST.Add(logEx);
			//logEx.LogInnerException
			List<string> msgList = new List<string>();
			recurInnerEx(logEx, (_logEx) => {
				msgList.Add(_logEx.LogMessage + _logEx.StackTrace);
			}, (_ex) => {
				msgList.Add(_ex.Message + _ex.StackTrace);
			});

			//输出信息1
			string msg = "#Exception : [ " + logEx.ExceptionTime + " ] [ " + logEx.ExceptionType + " ] [ " + logEx.TypeName + " ] ";
			//记录异常日志
			//LogCommon.Instance.WriteLine(new string[] { msg, logEx.LogMessage });
			LogCommon.Instance.WriteByAction((logWriter) => {
				logWriter.WriteLine(msg);
				foreach (string _msg in msgList) {
					logWriter.WriteLine(_msg);
				}
			});
		}

		/// <summary>
		/// 输出普通信息到日志
		/// </summary>
		/// <param name="msg"></param>
		public static void log(string msg) {
			//记录到日志
			LogCommon.Instance.WriteLine(msg);
		}

		/// <summary>
		/// 输出多行普通信息到日志
		/// </summary>
		/// <param name="msgs"></param>
		public static void log(string[] msgs) {
			//记录到日志
			LogCommon.Instance.WriteLine(msgs);
		}

		#endregion

		#region log & console

		/// <summary>
		/// 输出普通信息到日志
		/// </summary>
		/// <param name="msg"></param>
		public static void logConsole(string msg) {
			Console.WriteLine(msg);
			//记录到日志
			LogCommon.Instance.WriteLine(msg);
		}
		/// <summary>
		/// 输出带当前时间的信息到日志
		/// </summary>
		/// <param name="msg"></param>
		public static void logConsoleNow(string msg) {
			Console.WriteLine(msg);
			//记录到日志
			LogCommon.Instance.WriteLine("[ " + DateTime.Now.ToString() + " ] " + msg);
		}
		/// <summary>
		/// 输出多行普通信息到日志
		/// </summary>
		/// <param name="msgs"></param>
		public static void logConsole(string[] msgs) {
			int len = msgs.Length;
			for (int i = 0; i < len; i++) {
				Console.WriteLine(msgs);
			}
			//记录到日志
			LogCommon.Instance.WriteLine(msgs);
		}

		/// <summary>
		/// 输出异常信息到日志
		/// </summary>
		/// <param name="logEx"></param>
		public static void logConsole(LogException logEx) {
			//输出到控制台
			Console.WriteLine(logEx.LogMessage);
			log(logEx);
		}
		#endregion

		#region 异常详细信息追溯

		/// <summary>
		/// 追溯Exception.InnerException
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="logExAction"></param>
		/// <param name="exAction"></param>
		public static void recurInnerEx(Exception ex, Action<LogException> logExAction, Action<Exception> exAction) {
			//处理Exception
			exAction(ex);
			if (ex.InnerException is LogException) {
				recurInnerEx((LogException)ex.InnerException, logExAction, exAction);
			} else if (ex.InnerException != null && ex.InnerException is Exception) {
				recurInnerEx(ex.InnerException, logExAction, exAction);
			}

		}

		/// <summary>
		/// 优先追溯LogException.LogInnerException
		/// </summary>
		/// <param name="logEx"></param>
		/// <param name="logExAction"></param>
		/// <param name="exAction"></param>
		public static void recurInnerEx(LogException logEx, Action<LogException> logExAction, Action<Exception> exAction) {
			// Action<string, string> action) {//List<string> msgList
			//处理LogException
			logExAction(logEx);
			if (logEx.LogInnerException is LogException) {
				recurInnerEx((LogException)logEx.LogInnerException, logExAction, exAction);
			} else if (logEx.LogInnerException != null && logEx.LogInnerException is Exception) {
				recurInnerEx(logEx.LogInnerException, logExAction, exAction);
			}
		}

		#endregion
	}
}
