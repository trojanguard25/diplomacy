using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections;

namespace Polarsoft.Diplomacy.AI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

			string host = SystemInformation.ComputerName;	//Default to connecting to the local computer
			int port = 16713;								//Default to the default DAIDE port

		    Hashtable commandArguments = new Hashtable();
            foreach(string s in Environment.GetCommandLineArgs())
			{
				ParseCommandLineArgument(s, commandArguments);
			}

            if (commandArguments.Contains("n"))
			{
				host = (string)commandArguments["n"];
			}
			if (commandArguments.Contains("p"))
			{
				try
				{
					port = int.Parse((string)commandArguments["p"]);
				}
				catch (FormatException)
				{
					//Do nothing
				}
			}

            try
            {
                Application.Run(new Main(host, port));
            }
            catch( System.Net.Sockets.SocketException )
            {
                Environment.ExitCode = -1;
                Application.Exit();
            }

		}

        private static void ParseCommandLineArgument(string s, Hashtable commandArguments)
		{
			if (s[0] != '-')
			{
				return;
			}
			int iSeparator = s.IndexOf('=');
			if (iSeparator == -1)
			{
				commandArguments.Add(s.Substring(1), true);
			}
			else
			{
				commandArguments.Add(s.Substring(1, iSeparator - 1),
					s.Substring(iSeparator + 1));

			}
        }
    }
}