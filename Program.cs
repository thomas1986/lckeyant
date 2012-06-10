using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LckeyAnt
{
	class Program
	{//每次cmd进来都是新实例
		//static string te = "ttt";
		static void Main(string[] args) {
			GlobalCurrent.PROJECT_WORKSPACE=AppDomain.CurrentDomain.BaseDirectory;
			CmdProcess.processArgs(args);

		}
	}
}
