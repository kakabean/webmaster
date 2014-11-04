using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebMaster.uninstall
{
    class Program
    {
        static void Main(string[] args) {
            string sysroot = System.Environment.SystemDirectory;
            System.Diagnostics.Process.Start(sysroot + "\\msiexec.exe", "/x {6BD69B1F-17E2-429E-86AC-86FB13586A1D} /qf");
        }
    }
}
