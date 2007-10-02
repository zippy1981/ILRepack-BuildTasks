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

    using global::ILMerging;

    using global::Microsoft.Build.Framework;
    using global::Microsoft.Build.Utilities;

    public class ILMerge : Task
    {

        private ITaskItem m_attributeFile;
        private bool m_closed;
        private bool m_copyAttributes;
        private bool m_debugInfo;
        private ITaskItem m_excludeFile;
        private bool m_internalize;
        private bool m_log;
        private ITaskItem m_logFile;
        private ITaskItem m_outputFile;
        private ITaskItem m_keyFile;
        private ITaskItem[] m_assemblies;
        private ITaskItem m_targetKind;
        private ILMerging.ILMerge ILMerger;

        public virtual ITaskItem AttributeFile
        {
            get
            {
                return m_attributeFile;
            }
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

        public virtual ITaskItem ExcludeFile
        {
            get
            {
                return m_excludeFile;
            }
            set { m_excludeFile = BuildPath(ConvertEmptyToNull(value)); }
        }

        public virtual bool Internalize
        {
            get { return m_internalize; }
            set { m_internalize = value; }
        }

        public virtual bool ShouldLog
        {
            get { return m_log; }
            set { m_log = value; }
        }

        public virtual ITaskItem LogFile
        {
            get
            {
                return m_logFile;
            }
            set { m_logFile = BuildPath(ConvertEmptyToNull(value)); }
        }

        [Required]
        public virtual ITaskItem OutputFile
        {
            get
            {
                return m_outputFile;
            }
            set { m_outputFile = BuildPath(ConvertEmptyToNull(value)); }
        }

        public virtual ITaskItem SnkFile
        {
            get
            {
                return m_keyFile;
            }
            set { m_keyFile = BuildPath(ConvertEmptyToNull(value)); }
        }

        [Required]
        public virtual ITaskItem[] InputAssemblies
        {
            get { return m_assemblies; }
            set { m_assemblies = value; }
        }

        public virtual ITaskItem TargetKind
        {
            get { return m_targetKind; }
            set
            {
                m_targetKind = ConvertEmptyToNull(value);
                if (m_targetKind == null || m_targetKind.ItemSpec == null)
                {
                    m_targetKind = new TaskItem("sameasprimary");
                }
            }
        }

        public override bool Execute()
        {
            ILMerger = new ILMerging.ILMerge();
            ILMerger.AttributeFile = m_attributeFile.ItemSpec;
            ILMerger.Closed = m_closed;
            ILMerger.CopyAttributes = m_copyAttributes;
            ILMerger.DebugInfo = m_debugInfo;
            ILMerger.ExcludeFile = m_excludeFile.ItemSpec;
            ILMerger.Internalize = m_internalize;
            ILMerger.LogFile = m_logFile.ItemSpec;
            ILMerger.Log = m_log;
            ILMerger.OutputFile = m_outputFile.ItemSpec;
            ILMerger.KeyFile = m_keyFile.ItemSpec;

            switch (m_targetKind.ItemSpec.ToLower())
            {
                case "winexe":
                    ILMerger.TargetKind = ILMerging.ILMerge.Kind.WinExe; break;
                case "exe":
                    ILMerger.TargetKind = ILMerging.ILMerge.Kind.Exe; break;
                case "dll":
                    ILMerger.TargetKind = ILMerging.ILMerge.Kind.Dll; break;
                case "sameasprimary":
                    ILMerger.TargetKind = ILMerging.ILMerge.Kind.SameAsPrimaryAssembly;
                    break;
                default:
                    Log.LogError("TargetKind should be [exe|dll|winexe|sameasprimary]");
                    return false;
            }

            string[] assemblies = new string[m_assemblies.Length];
            for (int i = 0; i < assemblies.Length; i++)
            {
                assemblies[i] = m_assemblies[i].ItemSpec;
            }

            ILMerger.SetInputAssemblies(assemblies);
            ILMerger.SetSearchDirectories(new string[] { "." });

            try
            {
                Log.LogMessage(MessageImportance.Normal, "Merging {0} assembl{1} to '{2}'.", this.m_assemblies.Length, (this.m_assemblies.Length != 1) ? "ies" : "y", this.m_outputFile.ItemSpec);
                ILMerger.Merge();
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }

            return true;
        }

        private ITaskItem ConvertEmptyToNull(ITaskItem iti)
        {
            iti.ItemSpec = (iti.ItemSpec == null || iti.ItemSpec.Length == 0) ? null : iti.ItemSpec;
            return iti;
        }

        private ITaskItem BuildPath(ITaskItem iti)
        {
            iti.ItemSpec = (iti.ItemSpec == null || iti.ItemSpec.Length == 0) ? null :
                System.IO.Path.Combine(BuildEngine.ProjectFileOfTaskNode, iti.ItemSpec);
            return iti;
        }
    }
}