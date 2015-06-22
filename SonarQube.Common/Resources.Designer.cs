﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SonarQube.Common {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SonarQube.Common.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Commencing retry-able operation. Max wait (milliseconds): {0}, pause between tries (milliseconds): {1}.
        /// </summary>
        internal static string DIAG_BeginningRetry {
            get {
                return ResourceManager.GetString("DIAG_BeginningRetry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating directory: {0}.
        /// </summary>
        internal static string DIAG_CreatingDirectory {
            get {
                return ResourceManager.GetString("DIAG_CreatingDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Removing the existing directory: {0}.
        /// </summary>
        internal static string DIAG_DeletingDirectory {
            get {
                return ResourceManager.GetString("DIAG_DeletingDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The directory already exists: {0}.
        /// </summary>
        internal static string DIAG_DirectoryAlreadyExists {
            get {
                return ResourceManager.GetString("DIAG_DirectoryAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Executing file {0}
        ///  Args: {1}
        ///  Working directory: {2}
        ///  Timeout (ms):{3}
        ///  Process id: {4}.
        /// </summary>
        internal static string DIAG_ExecutingFile {
            get {
                return ResourceManager.GetString("DIAG_ExecutingFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Process returned exit code {0}.
        /// </summary>
        internal static string DIAG_ExecutionExitCode {
            get {
                return ResourceManager.GetString("DIAG_ExecutionExitCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Timed out after waiting {0} ms for process {1} to complete. The process may still be running..
        /// </summary>
        internal static string DIAG_ExecutionTimedOut {
            get {
                return ResourceManager.GetString("DIAG_ExecutionTimedOut", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Retrying....
        /// </summary>
        internal static string DIAG_RetryingOperation {
            get {
                return ResourceManager.GetString("DIAG_RetryingOperation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operation timed out, Elapsed time (ms): {0}.
        /// </summary>
        internal static string DIAG_RetryOperationFailed {
            get {
                return ResourceManager.GetString("DIAG_RetryOperationFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operation succeeded. Elapsed time (ms): {0}.
        /// </summary>
        internal static string DIAG_RetryOperationSucceeded {
            get {
                return ResourceManager.GetString("DIAG_RetryOperationSucceeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to WARNING: .
        /// </summary>
        internal static string Logger_WarningPrefix {
            get {
                return ResourceManager.GetString("Logger_WarningPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specfied properties file could not be found.
        /// </summary>
        internal static string MissingSonarPropertiesFileError {
            get {
                return ResourceManager.GetString("MissingSonarPropertiesFileError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The settings file &apos;{0}&apos; does not contain a value for the property &apos;{1}&apos;.
        /// </summary>
        internal static string PropertyNotFoundInFileError {
            get {
                return ResourceManager.GetString("PropertyNotFoundInFileError", resourceCulture);
            }
        }
    }
}
