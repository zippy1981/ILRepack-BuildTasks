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

namespace ILMerge.NAnt.Tasks
{

    using System;

    using ILMerging;

    using global::NAnt.Core;
    using global::NAnt.Core.Attributes;
    using global::NAnt.Core.Types;
    using global::NAnt.Core.Util;
    using System.Collections.Generic;

    [TaskName("ilmerge")]
    public class ILMergeTask : Task
    {

        private string m_attributeFile;
        private bool m_closed;
        private bool m_copyAttributes;
        private bool m_debugInfo;
        private string m_excludeFile;
        private bool m_internalize;
        private FileSet m_libraryPath;
        private bool m_log;
        private string m_logFile;
        private string m_outputFile;
        private string m_keyFile;
        private FileSet m_primaryAssembly;
        private FileSet m_assemblies;
        private string m_targetKind;
        private ILMerge ILMerge;

        [TaskAttribute("attributefile")]
        public virtual string AttributeFile
        {
            get
            {
                return m_attributeFile != null ?
                    Project.GetFullPath(m_attributeFile) : null;
            }
            set
            {
                m_attributeFile = StringUtils.ConvertEmptyToNull(value);
            }
        }

        [TaskAttribute("closed")]
        [BooleanValidator]
        public virtual bool Closed
        {
            get { return m_closed; }
            set { m_closed = value; }
        }

        [TaskAttribute("copyattributes")]
        [BooleanValidator]
        public virtual bool CopyAttributes
        {
            get { return m_copyAttributes; }
            set { m_copyAttributes = value; }
        }

        [TaskAttribute("debuginfo")]
        [BooleanValidator]
        public virtual bool DebugInfo
        {
            get { return m_debugInfo; }
            set { m_debugInfo = value; }
        }

        [TaskAttribute("excludefile")]
        public virtual string ExcludeFile
        {
            get
            {
                return m_excludeFile != null ?
                    Project.GetFullPath(m_excludeFile) : null;
            }
            set
            {
                m_excludeFile = StringUtils.ConvertEmptyToNull(value);
            }
        }

        [TaskAttribute("internalize")]
        [BooleanValidator]
        public virtual bool Internalize
        {
            get { return m_internalize; }
            set { m_internalize = value; }
        }

        [TaskAttribute("librarypath")]
        public virtual FileSet LibraryPath
        {
            get { return m_libraryPath; }
            set { m_libraryPath = value; }
        }

        [TaskAttribute("shouldlog")]
        [BooleanValidator]
        public virtual bool ShouldLog
        {
            get { return m_log; }
            set { m_log = value; }
        }

        [TaskAttribute("logfile")]
        public virtual string LogFile
        {
            get
            {
                return m_logFile != null ?
                    Project.GetFullPath(m_logFile) : null;
            }
            set
            {
                m_logFile = StringUtils.ConvertEmptyToNull(value);
            }
        }

        [TaskAttribute("outputfile", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public virtual string OutputFile
        {
            get
            {
                return m_outputFile != null ?
                    Project.GetFullPath(m_outputFile) : null;
            }
            set
            {
                m_outputFile = StringUtils.ConvertEmptyToNull(value);
            }
        }

        [TaskAttribute("snkfile")]
        public virtual string SnkFile
        {
            get
            {
                return m_keyFile != null ?
                    Project.GetFullPath(m_keyFile) : null;
            }
            set
            {
                m_keyFile = StringUtils.ConvertEmptyToNull(value);
            }
        }
        
        [BuildElement("primaryassembly", Required = true)]
        public virtual FileSet PrimaryAssembly
        {
            get { return m_primaryAssembly; }
            set { m_primaryAssembly = value; }
        }

        [BuildElement("assemblies")]
        public virtual FileSet InputAssemblies
        {
            get { return m_assemblies; }
            set { m_assemblies = value; }
        }

        [TaskAttribute("targetkind")]
        [StringValidator(AllowEmpty = false)]
        public virtual string TargetKind
        {
            get
            {
                return m_targetKind;
            }
            set
            {
                m_targetKind = StringUtils.ConvertEmptyToNull(value);
                if (m_targetKind == null)
                {
                    m_targetKind = "sameasprimary";
                }
            }
        }

        protected override void ExecuteTask()
        {
            ILMerge = new ILMerge();
            ILMerge.AttributeFile = m_attributeFile;
            ILMerge.Closed = m_closed;
            ILMerge.CopyAttributes = m_copyAttributes;
            ILMerge.DebugInfo = m_debugInfo;
            ILMerge.ExcludeFile = m_excludeFile;
            ILMerge.Internalize = m_internalize;
            ILMerge.LogFile = m_logFile;
            ILMerge.Log = m_log;
            ILMerge.OutputFile = m_outputFile;
            ILMerge.KeyFile = m_keyFile;

            switch (m_targetKind.ToLower())
            {
                case "winexe":
                    ILMerge.TargetKind = ILMerge.Kind.WinExe; break;
                case "exe":
                    ILMerge.TargetKind = ILMerge.Kind.Exe; break;
                case "dll":
                    ILMerge.TargetKind = ILMerge.Kind.Dll; break;
                case "sameasprimary":
                    ILMerge.TargetKind = ILMerge.Kind.SameAsPrimaryAssembly;
                    break;
                default:
                    throw new BuildException(
                        "TargetKind should be [exe|dll|winexe|sameasprimary]");
            }

            string[] assemblies = new string[m_assemblies.FileNames.Count + 1];
            if (this.m_primaryAssembly.FileNames.Count != 1)
            {
                this.Log(Level.Error, "Only one primary assembly is allowed in the <primaryassembly> fileset, but found {0}.", this.m_primaryAssembly.FileNames.Count);
                return;
            }
            assemblies[0] = this.m_primaryAssembly.FileNames[0];
            for (int i = 1; i < assemblies.Length; i++)
            {
                assemblies[i] = m_assemblies.FileNames[i-1];
            }

            ILMerge.SetInputAssemblies(assemblies);

            List<string> searchPath = new List<string>();
            searchPath.Add(".");
            if (LibraryPath != null) {
               foreach (string libpath in LibraryPath.FileNames) {
                  searchPath.Add(libpath);
               }
            }
            ILMerge.SetSearchDirectories(searchPath.ToArray());

            try
            {
                this.Log(Level.Info, "Merging {0} assembl{1} to '{2}'.", this.m_assemblies.FileNames.Count, (this.m_assemblies.FileNames.Count != 1) ? "ies" : "y", this.m_outputFile);
                ILMerge.Merge();
            }
            catch (Exception e)
            {
                throw new BuildException("Failed to merge assemblies", e);
            }
        }
    }
}
