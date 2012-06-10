using System;
using System.Collections.Generic;
using System.Text;

namespace LckeyAnt
{
	/// <summary>
	/// �쳣��Ϣ
	/// exception.innerexception(�Զ������в�׼ȷ_set)��logInnerException����ˣ���
	/// </summary>
	public class LogException : ApplicationException
	{
		private int exceptionType = 9999;
		private string typeName = "null";
		private string logMessage = string.Empty;
		private Exception logInnerException = null;
		private string exceptionTime = string.Empty;

		#region ��ʧinnerException�Ĺ��췽ʽ

		/// <summary>
		/// �쳣���췽����TypeName����type��key�Զ���ȡ
		/// </summary>
		/// <param name="_exceptionType"></param>
		/// <param name="_logInnerException"></param>
		private LogException(Exception _logInnerException, int _exceptionType) {
			this.ExceptionType = _exceptionType;
			this.TypeName = ExceptionAccess.getExValueByKey(_exceptionType);
			this.LogMessage = _logInnerException.StackTrace;
			this.LogInnerException = _logInnerException;
			this.ExceptionTime = DateTime.Now.ToString();
			//ά��ԭʼexception���ڲ��쳣innerException��
			//this.InnerException=_logInnerException; //ֻ������
		}

		/// <summary>
		/// �쳣���췽����TypeName����type��key�Զ���ȡ
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
		/// �쳣���췽��
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
		/// ��������ֻ����logMessage,����ʹ��Ĭ��ֵ9999
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

		#region base�̳еĹ��췽ʽ

		/// <summary>
		/// ά�����ڲ��쳣��(����ʹ�ô˷���ʵ����)
		/// �Ѿ�������logEx��message,innerexception���ԣ�ֻ��set������
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
		/// ����message����
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

		#region setLogEx��������

		/// <summary>
		/// ����ֵ��ά�� �ڲ��쳣���������ڼ̳���base�Ĺ��췽��
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
		/// �쳣���ͣ���Ӧxml��key
		/// </summary>
		public int ExceptionType {
			get { return exceptionType; }
			set { exceptionType = value; }
		}
		/// <summary>
		/// �쳣����������Ӧxml��value
		/// </summary>
		public string TypeName {
			get { return typeName; }
			set { typeName = value; }
		}
		/// <summary>
		/// �쳣����
		/// </summary>
		public string LogMessage {
			get { return logMessage; }
			set { logMessage = value; }
		}
		/// <summary>
		/// �ڲ��쳣
		/// </summary>
		public Exception LogInnerException {
			get { return logInnerException; }
			set { logInnerException = value; }
		}
		/// <summary>
		/// ����ʱ��
		/// </summary>
		public string ExceptionTime {
			get { return exceptionTime; }
			set { exceptionTime = value; }
		}
		#endregion
	}
}
