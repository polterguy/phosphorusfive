/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Configuration;

/// <summary>
///     Main namespace for all configuration code that is specific to your application-pool
/// </summary>
namespace p5.webapp.code.configuration
{
    /// <summary>
    ///     Class wrapping your Phosphorus Five configuration section from web.config
    /// </summary>
    public class PhosphorusConfiguration : ConfigurationSection
    {
        /// <summary>
        ///     Gets the plugin directory
        /// </summary>
        /// <value>The plugin directory</value>
        [ConfigurationProperty ("assemblyDirectory", DefaultValue = "~/plugins/", IsRequired = false)]
        public string PluginDirectory
        {
            get { return this ["assemblyDirectory"] as string; }
        }

        /// <summary>
        ///     Gets the default username used to raise Active Events on behalf of
        /// </summary>
        /// <value>The plugin directory</value>
        [ConfigurationProperty ("defaultContextUsername", IsRequired = false, DefaultValue = "guest")]
        public string DefaultContextUsername
        {
            get { return this ["defaultContextUsername"] as string; }
        }

        /// <summary>
        ///     Gets the default role used to raise Active Events as
        /// </summary>
        /// <value>Default context role</value>
        [ConfigurationProperty ("defaultContextRole", IsRequired = false, DefaultValue = "guest")]
        public string DefaultContextRole
        {
            get { return this ["defaultContextRole"] as string; }
        }

        /// <summary>
        ///     Gets the path to the file on disc that is used for authenticating and authorizing users
        /// </summary>
        /// <value>Path to auth file</value>
        [ConfigurationProperty ("authFile", IsRequired = false, DefaultValue = "~/auth")]
        public string AuthFile
        {
            get { return this ["authFile"] as string; }
        }

        /// <summary>
        ///     Gets the number of seconds a specific IP address must wait between attempting to login to the system
        /// </summary>
        /// <value>Number of seconds to cool off between login attempts</value>
        [ConfigurationProperty ("loginCoolOffSeconds", IsRequired = false, DefaultValue = "5")]
        public int LoginCoolOffSeconds
        {
            get { return (int)this ["loginCoolOffSeconds"]; }
        }

        /// <summary>
        ///     Gets the number of days a persisted login cookie will be stored and valid on client side
        /// </summary>
        /// <value>Number of days to persist cookies on client side for non-root context tickets</value>
        [ConfigurationProperty ("persistCredentialCookieDays", IsRequired = true)]
        public int PersistCredentialCookieDays
        {
            get { return (int)this ["persistCredentialCookieDays"]; }
        }

        /// <summary>
        ///     Gets the default SMTP server URL used for sending emails
        /// </summary>
        /// <value>SMTP server URL</value>
        [ConfigurationProperty ("smtpServer", IsRequired = false)]
        public string SmtpServer
        {
            get { return (string)this ["smtpServer"]; }
        }

        /// <summary>
        ///     Gets the default port to use to connect to SMTP server
        /// </summary>
        /// <value>Port to use when connecting to SMTP server</value>
        [ConfigurationProperty ("smtpPort", IsRequired = false)]
        public int SmtpPort
        {
            get { return (int)this ["smtpPort"]; }
        }

        /// <summary>
        ///     Gets the default of whether or not to use SSL when connecting to SMTP server
        /// </summary>
        /// <value>Whether or not to use SSL when connecting to SMTP server</value>
        [ConfigurationProperty ("smtpUseSsl", IsRequired = false)]
        public bool SmtpUseSsl
        {
            get { return (bool)this ["smtpUseSsl"]; }
        }

        /// <summary>
        ///     Gets the default username to use when connecting to SMTP server
        /// </summary>
        /// <value>Default username to use when connecting to SMTP server</value>
        [ConfigurationProperty ("smtpUsername", IsRequired = false)]
        public string SmtpUsername
        {
            get { return (string)this ["smtpUsername"]; }
        }

        /// <summary>
        ///     Gets the default password to use when connecting to SMTP server
        /// </summary>
        /// <value>Default password to use when connecting to SMTP server</value>
        [ConfigurationProperty ("smtpPassword", IsRequired = false)]
        public string SmtpPassword
        {
            get { return (string)this ["smtpPassword"]; }
        }

        /// <summary>
        ///     Gets the default POP3 server URL used for sending emails
        /// </summary>
        /// <value>The plugin directory</value>
        [ConfigurationProperty ("pop3Server", IsRequired = false)]
        public string Pop3Server
        {
            get { return (string)this ["pop3Server"]; }
        }

        /// <summary>
        ///     Gets the default port to use to connect to POP3 server
        /// </summary>
        /// <value>The plugin directory</value>
        [ConfigurationProperty ("pop3Port", IsRequired = false)]
        public int Pop3Port
        {
            get { return (int)this ["pop3Port"]; }
        }

        /// <summary>
        ///     Gets the default of whether or not to use SSL when connecting to POP3 server
        /// </summary>
        /// <value>The plugin directory</value>
        [ConfigurationProperty ("pop3UseSsl", IsRequired = false)]
        public bool Pop3UseSsl
        {
            get { return (bool)this ["pop3UseSsl"]; }
        }

        /// <summary>
        ///     Gets the default username to use when connecting to POP3 server
        /// </summary>
        /// <value>Default username to use when connecting to POP3 server</value>
        [ConfigurationProperty ("pop3Username", IsRequired = false)]
        public string Pop3Username
        {
            get { return (string)this ["smtpUsername"]; }
        }

        /// <summary>
        ///     Gets the default password to use when connecting to POP3 server
        /// </summary>
        /// <value>Default password to use when connecting to POP3 server</value>
        [ConfigurationProperty ("pop3Password", IsRequired = false)]
        public string Pop3Password
        {
            get { return (string)this ["pop3Password"]; }
        }

        /// <summary>
        ///     Gets the name of the server PGP private/public key to use for encryption/signature operations
        /// </summary>
        /// <value>Default password to use when connecting to POP3 server</value>
        [ConfigurationProperty ("marvinPgpKey", IsRequired = false)]
        public string MarvinPgpKey
        {
            get { return (string)this ["marvinPgpKey"]; }
        }

        /// <summary>
        ///     Gets the password of the server PGP private/public key to use for encryption/signature operations
        /// </summary>
        /// <value>Default password to use when connecting to POP3 server</value>
        [ConfigurationProperty ("marvinPgpKeyPassword", IsRequired = false)]
        public string MarvinPgpKeyPassword
        {
            get { return (string)this ["marvinPgpKeyPassword"]; }
        }

        /// <summary>
        ///     Gets the password salt to use for storing passwords
        /// </summary>
        /// <value>Default password to use when connecting to POP3 server</value>
        [ConfigurationProperty ("webServiceInvocationsFreshTime", IsRequired = false, DefaultValue = "00:05:00.0000000")]
        public string WebServiceInvocationsFreshTime
        {
            get { return (string)this ["webServiceInvocationsFreshTime"]; }
        }

        /// <summary>
        ///     Gets the assemblies
        /// </summary>
        /// <value>The assemblies</value>
        [ConfigurationProperty ("assemblies")]
        public ActiveEventAssemblyCollection Assemblies
        {
            get { return this ["assemblies"] as ActiveEventAssemblyCollection; }
        }
    }
}