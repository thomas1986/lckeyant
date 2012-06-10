using System;
using System.Collections.Generic;
using System.Text;

namespace LckeyAnt
{
	/// <summary>
	/// 异常信息
	/// exception.innerexception(自定义类中不准确_set)被logInnerException替代了，，
	/// </summary>
	public class LogException : ApplicationException
	{
		private int exceptionType = 9999;
		private string typeName = "null";
		private string logMessage = string.Empty;
		private Exception logInnerException = null;
		private string exceptionTime = string.Empty;

		#region 丢失innerException的构造方式

		/// <summary>
		/// 异常构造方法，TypeName根据type的key自动读取
		/// </summary>
		/// <param name="_exceptionType"></param>
		/// <param name="_logInnerException"></param>
		private LogException(Exception _logInnerException, int _exceptionType) {
			this.ExceptionType = _exceptionType;
			this.TypeName = ExceptionAccess.getExValueByKey(_exceptionType);
			this.LogMessage = _logInnerException.StackTrace;
			this.LogInnerException = _logInnerException;
			this.ExceptionTime = DateTime.Now.ToString();
			//维护原始exception的内部异常innerException链
			//this.InnerException=_logInnerException; //只读……
		}

		/// <summary>
		/// 异常构造方法，TypeName根据type的key自动读取
		/// </summary>
		/// <param name="_exceptionType"></param>
		/// <param name="_logInnerException"></param>
		private LogException(Exception _logInnerException, int _exceptionType, string _logMessage) {
			this.ExceptionType = _exceptionType;
			this.TypeName = ExceptionAccess.getExValueByKey(_exceptionType);
			this.LogMessage = _logMessage;
			this.LogInnerException = _logInnerException;
			this.ExceptionTime = DateTime.Now.ToString();
		}

		/// <summary>
		/// 异常构造方法
		/// </summary>
		/// <param name="_exceptionType"></param>
		/// <param name="_typeName"></param>
		/// <param name="_logMessage"></param>
		/// <param name="_logInnerException"></param>
		private LogException(Exception _logInnerException, int _exceptionType, string _typeName, string _logMessage) {
			this.ExceptionType = _exceptionType;
			this.TypeName = _typeName;
			this.LogMessage = _logMessage;
			this.LogInnerException = _logInnerException;
			this.ExceptionTime = DateTime.Now.ToString();
		}

		/// <summary>
		/// 懒方法，只传入logMessage,其他使用默认值9999
		/// </summary>
		/// <param name="_logInnerException"></param>
		/// <param name="_logMessage"></param>
		private LogException(Exception _logInnerException, string _logMessage) {
			this.ExceptionType = exceptionType;
			this.TypeName = ExceptionAccess.getExValueByKey(exceptionType);
			this.LogMessage = _logMessage;
			this.LogInnerException = _logInnerException;
			this.ExceptionTime = DateTime.Now.ToString();
		}

		#endregion

		#region base继承的构造方式

		/// <summary>
		/// 维护了内部异常链(尽量使用此方法实例化)
		/// 已经保存了logEx的message,innerexception属性，只许set其他的
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerexception"></param>
		public LogException(string message, Exception innerexception)
			: base(message, innerexception) {
			this.LogMessage = message;
			this.LogInnerException = innerexception;
			this.ExceptionTime = DateTime.Now.ToString();
		}
		/// <summary>
		/// 保存message属性
		/// </summary>
		/// <param name="message"></param>
		public LogException(string message)
			: base(message) {
			//this.ExceptionType = exceptionType;
			//this.TypeName = ExceptionAccess.getExValueByKey(exceptionType);
			this.LogMessage = message;
			this.ExceptionTime = DateTime.Now.ToString();
		}
		public LogException() : base() { }

		#endregion

		#region setLogEx补充属性

		/// <summary>
		/// 设置值，维护 内部异常链，适用于继承于base的构造方法
		/// </summary>
		/// <param name="_logInnerException"></param>
		/// <param name="_logMessage"></param>
		public LogException setLogEx(int _exceptionType, string _typeName) {
			this.ExceptionType = _exceptionType;
			this.TypeName = _typeName;
			return this;
		}
		public LogException setLogEx(int _exceptionType) {
			this.ExceptionType = _exceptionType;
			this.TypeName = ExceptionAccess.getExValueByKey(exceptionType);
			return this;
		}

		#endregion

		#region property

		/// <summary>
		/// 异常类型，对应xml中key
		/// </summary>
		public int ExceptionType {
			get { return exceptionType; }
			set { exceptionType = value; }
		}
		/// <summary>
		/// 异常类型名，对应xml中value
		/// </summary>
		public string TypeName {
			get { return typeName; }
			set { typeName = value; }
		}
		/// <summary>
		/// 异常详情
		/// </summary>
		public string LogMessage {
			get { return logMessage; }
			set { logMessage = value; }
		}
		/// <summary>
		/// 内部异常
		/// </summary>
		public Exception LogInnerException {
			get { return logInnerException; }
			set { logInnerException = value; }
		}
		/// <summary>
		/// 发生时间
		/// </summary>
		public string ExceptionTime {
			get { return exceptionTime; }
			set { exceptionTime = value; }
		}
		#endregion
	}
}
