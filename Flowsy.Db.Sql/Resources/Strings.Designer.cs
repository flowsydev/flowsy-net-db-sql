﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Flowsy.Db.Sql.Resources {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("Flowsy.Db.Sql.Resources.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static string NoConnectionConfigurationProvided {
            get {
                return ResourceManager.GetString("NoConnectionConfigurationProvided", resourceCulture);
            }
        }
        
        internal static string InvalidConnectionKey {
            get {
                return ResourceManager.GetString("InvalidConnectionKey", resourceCulture);
            }
        }
        
        internal static string CouldNotGetConnection {
            get {
                return ResourceManager.GetString("CouldNotGetConnection", resourceCulture);
            }
        }
        
        internal static string CouldNotSetValueXForParameterX {
            get {
                return ResourceManager.GetString("CouldNotSetValueXForParameterX", resourceCulture);
            }
        }
        
        internal static string CouldNotParseValueX {
            get {
                return ResourceManager.GetString("CouldNotParseValueX", resourceCulture);
            }
        }
        
        internal static string InitializationStatementExecuted {
            get {
                return ResourceManager.GetString("InitializationStatementExecuted", resourceCulture);
            }
        }
        
        internal static string ConnectionFactoryOrConfigurationListWasExpected {
            get {
                return ResourceManager.GetString("ConnectionFactoryOrConfigurationListWasExpected", resourceCulture);
            }
        }
        
        internal static string NoMigrationConfigurationProvidedForConnectionWithKey {
            get {
                return ResourceManager.GetString("NoMigrationConfigurationProvidedForConnectionWithKey", resourceCulture);
            }
        }
        
        internal static string CouldNotCreateDatabaseConnectionForConfigurationWithKey {
            get {
                return ResourceManager.GetString("CouldNotCreateDatabaseConnectionForConfigurationWithKey", resourceCulture);
            }
        }
        
        internal static string DatabaseMigrationFailedForConnectionWithKey {
            get {
                return ResourceManager.GetString("DatabaseMigrationFailedForConnectionWithKey", resourceCulture);
            }
        }
        
        internal static string MustBeginWorkByInvokingMethodX {
            get {
                return ResourceManager.GetString("MustBeginWorkByInvokingMethodX", resourceCulture);
            }
        }
        
        internal static string NoOptionsRegisteredForX {
            get {
                return ResourceManager.GetString("NoOptionsRegisteredForX", resourceCulture);
            }
        }
    }
}
