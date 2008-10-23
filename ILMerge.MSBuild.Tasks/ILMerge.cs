/**
 * Copyright (c) 2004, Evain Jb (jb@evain.net)
 * Modified 2007 Marcus Griep (neoeinstein+boo@gmail.com)
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *     - Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     - Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     - Neither the name of Evain Jb nor the names of its contributors may
 *       be used to endorse or promote products derived from this software
 *       without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 *
 *****************************************************************************/

namespace ILMerge.MSBuild.Tasks 
{
    using System.Collections.Generic;

    using System;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    using ILM = ILMerging;
    
    public class ILMerge : Task 
    {
        private string m_attributeFile;
        private bool m_closed;
        private bool m_copyAttributes;
        private bool m_debugInfo;
        private string m_excludeFile;
        private bool m_internalize;
        private ITaskItem[] m_libraryPath = new ITaskItem[0];
        private bool m_log;
        private string m_logFile;
        private string m_outputFile;
        private string m_keyFile;
        private ITaskItem[] m_assemblies = new ITaskItem[0];
        private ILMerging.ILMerge.Kind m_targetKind;
        private ILMerging.ILMerge ILMerger;

        public virtual string AttributeFile
        {
            get { return m_attributeFile; }
            set { m_attributeFile = BuildPath(ConvertEmptyToNull(value)); }
        }

        public virtual bool Closed
        {
            get { return m_closed; }
            set { m_closed = value; }
        }

        public virtual bool CopyAttributes
        {
            get { return m_copyAttributes; }
            set { m_copyAttributes = value; }
        }

        public virtual bool DebugInfo
        {
            get { return m_debugInfo; }
            set { m_debugInfo = value; }
        }

        public virtual string ExcludeFile
        {
            get { return m_excludeFile; }
            set { m_excludeFile = BuildPath(ConvertEmptyToNull(value)); }
        }

        public virtual bool Internalize
        {
            get { return m_internalize; }
            set { m_internalize = value; }
        }

        public virtual ITaskItem[] LibraryPath
        {
            get { return m_libraryPath; }
            set { m_libraryPath = value; }
        }

        public virtual bool ShouldLog
        {
            get { return m_log; }
            set { m_log = value; }
        }

        public virtual string LogFile
        {
            get { return m_logFile; }
            set { m_logFile = BuildPath(ConvertEmptyToNull(value)); }
        }

        [Required]
        public virtual string OutputFile
        {
            get { return m_outputFile; }
            set { m_outputFile = BuildPath(ConvertEmptyToNull(value)); }
        }

        public virtual string SnkFile
        {
            get { return m_keyFile; }
            set { m_keyFile = BuildPath(ConvertEmptyToNull(value)); }
        }

        [Required]
        public virtual ITaskItem[] InputAssemblies
        {
            get { return m_assemblies; }
            set { m_assemblies = value; }
        }

        public virtual string TargetKind
        {
            get
            {
                return m_targetKind.ToString();
            }
            set
            {
                if (Enum.IsDefined(typeof(ILM.ILMerge.Kind), value))
                {
                    m_targetKind = (ILM.ILMerge.Kind) Enum.Parse(typeof(ILM.ILMerge.Kind), value);
                }
                else
                {
                    Log.LogWarning("TargetKind should be [Exe|Dll|WinExe|SameAsPrimaryAssembly]; set to SameAsPrimaryAssembly");
                    m_targetKind = ILM.ILMerge.Kind.SameAsPrimaryAssembly;
                }
            }
        }

        public override bool Execute()
        {
            ILMerger = new ILM.ILMerge();
            ILMerger.AttributeFile = m_attributeFile;
            ILMerger.Closed = m_closed;
            ILMerger.CopyAttributes = m_copyAttributes;
            ILMerger.DebugInfo = m_debugInfo;
            ILMerger.ExcludeFile = m_excludeFile;
            ILMerger.Internalize = m_internalize;
            ILMerger.LogFile = m_logFile;
            ILMerger.Log = m_log;
            ILMerger.OutputFile = m_outputFile;
            ILMerger.KeyFile = m_keyFile;
            ILMerger.TargetKind = m_targetKind;

            string[] assemblies = new string[m_assemblies.Length];
            for (int i = 0; i < assemblies.Length; i++)
            {
                assemblies[i] = m_assemblies[i].ItemSpec;
            }

            ILMerger.SetInputAssemblies(assemblies);

            List<string> searchPath = new List<string>();
            searchPath.Add(".");
            foreach (ITaskItem iti in LibraryPath)
            {
                searchPath.Add(BuildPath(iti.ItemSpec));
            }
            ILMerger.SetSearchDirectories(searchPath.ToArray());

            try
            {
                Log.LogMessage(MessageImportance.Normal, "Merging {0} assembl{1} to '{2}'.", this.m_assemblies.Length, (this.m_assemblies.Length != 1) ? "ies" : "y", this.m_outputFile);
                ILMerger.Merge();
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }

            return true;
        }

        private static string ConvertEmptyToNull(string iti)
        {
            return string.IsNullOrEmpty(iti) ? null : iti;
        }

        private string BuildPath(string iti)
        {
            return string.IsNullOrEmpty(iti) ? null :
                System.IO.Path.Combine(BuildEngine.ProjectFileOfTaskNode, iti);
        }
    }
}
