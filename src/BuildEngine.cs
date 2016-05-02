using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Simplic.DynamicUI
{
    public class BuildEngine : IBuildEngine
    {
        public int ColumnNumberOfTaskNode
        {
            get
            {
                return 0;
            }
        }

        public bool ContinueOnError
        {
            get
            {
                return false;
            }
        }

        public int LineNumberOfTaskNode
        {
            get
            {
                return 0;
            }
        }

        public string ProjectFileOfTaskNode
        {
            get
            {
                return "";
            }
        }

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            return true;
        }

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
            
        }

        public void LogErrorEvent(BuildErrorEventArgs e)
        {

        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {

        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {

        }
    }
}
