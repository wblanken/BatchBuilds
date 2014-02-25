using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace batch_builds
{
    class Program
    {
        static void Main(string[] args)
        {
            string project_Ini_Location = "E:\\BIOS\\projects.ini";
            string project_Get = "C:\\perl64\\bin\\GetProject.pl";
            string project_Refresh = "C:\\perl64\\bin\\RefreshTree.pl";
            string project_Dir = "E:\\BIOS\\bld\\L20_inc";
            string project_Build = "BuildImg.bat";

            string[] revisions = { "118769", "118785", "118787", "118789", "118794", "118795", "118796", "118800", "118852", "118854", /* "118857" */};

            string cmd = null;
            int WB = 3;

            Directory.SetCurrentDirectory(project_Dir);

            foreach (string rev in revisions)
            {
                Console.WriteLine("Getting L20 Rev: " + rev + "...");
                cmd = project_Get + " --config \"" + project_Ini_Location + "\" --revision " + rev + " L20";
                Execute(cmd, "perl");

                Console.WriteLine("Refreshing L20 Rev: " + rev + "...");
                cmd = project_Refresh + " --config " + project_Ini_Location + " L20";
                Execute(cmd, "perl");

                Console.WriteLine("Building L20 Rev: " + rev + "...");                
                cmd = project_Build + " L20 all timed";
                Execute(cmd);

                Console.WriteLine("Copying L20.bin for Rev: " + rev + " to L20_WB" + WB + ".bin...");
                cmd = "copy " + project_Dir + "\\L20.BIN " + project_Dir + "\\L20_WB" + WB + ".bin";
                Execute(cmd);
                WB++;
            }

        }

        private static void Execute(string cmd, string type = null)
        {
            int exitCode = 0;
            Process p = null;
            ProcessStartInfo psi = null;
            switch (type)
            {
                case "perl":
                    p = new Process();
                    psi = new ProcessStartInfo("perl.exe");
                    psi.Arguments = cmd;
                    break;

                default: //Batch
                    p = new Process();
                    psi = new ProcessStartInfo("cmd.exe", "/c " + cmd);
                    //psi.Arguments = cmd;
                    
                    break;
            }

            if (p != null && psi != null)
            {
                psi.CreateNoWindow = false;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = false;
                psi.RedirectStandardError = true;
                p.StartInfo = psi;

                p.Start();
                string output = null;
                string error = null;

                while (!p.HasExited)
                {
                    //Console.WriteLine(p.StandardOutput.ReadToEnd());
                    error = p.StandardError.ReadToEnd();                   
                }

                p.WaitForExit();
                exitCode = p.ExitCode;

                //Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
                Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
                Console.WriteLine("ExitCode: " + exitCode.ToString() + " ExecuteCommand: " + cmd);
                Console.WriteLine();
                p.Close();
            }
            
        }
    }
}
