﻿namespace EasyLogger
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using log4net.Config;

    /// <summary>
    /// An implementation of a log4net service.
    /// </summary>
    public sealed class Log4NetService : ILogService
    {
        private static readonly Lazy<Log4NetService> Lazy = new Lazy<Log4NetService>(() => new Log4NetService(), true);

        /// <summary>
        /// Returns a single instance of the <see cref="Log4NetService"/>
        /// </summary>
        public static Log4NetService Instance => Lazy.Value;

        /// <summary>
        /// Creates and configures an instance of the <see cref="Log4NetService"/> by looking for a default
        /// <c>log4net.config</c> file in the executing directory. It gets the directory
        /// path of the calling application <c>RelativeSearchPath</c> IS NULL if the
        /// executing assembly i.e. calling assembly is a stand-alone assembly file
        /// (Console, WinForm, etc). <c>RelativeSearchPath</c> IS NOT NULL if the 
        /// calling assembly is a web hosted application i.e. a WebSite
        /// </summary>
        /// <exception cref="FileNotFoundException">Valid Log4net config file not found</exception>
        private Log4NetService()
        {
            var log4NetConfigDir = AppDomain.CurrentDomain.RelativeSearchPath;

            if (string.IsNullOrWhiteSpace(log4NetConfigDir))
            {
                log4NetConfigDir = AppDomain.CurrentDomain.BaseDirectory;
            }

            var defaultConfigFile = new FileInfo(Path.Combine(log4NetConfigDir, "log4net.config"));
            ConfigureImpl(defaultConfigFile);
        }

        /// <summary>
        /// Configures the <see cref="Log4NetService"/> using the <paramref name="configFile"/>.
        /// </summary>
        /// <param name="configFile">Path to a valid log4net config file</param>
        /// <exception cref="ArgumentException">Thrown when log4net Config file is null</exception>
        /// <exception cref="FileNotFoundException">Thrown when a valid Log4net config file is not found</exception>
        public void Configure(FileInfo configFile)
        {
            ConfigureImpl(configFile);
        }

        /// <summary>
        /// Returns an <see cref="ILogger"/> for the given <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type for which an <see cref="ILogger"/> should be returned</param>
        /// <returns>The <see cref="ILogger"/></returns>
        [DebuggerStepThrough]
        public ILogger GetLogger(Type type)
        {
            return new Log4NetLogger(log4net.LogManager.GetLogger(type));
        }

        /// <summary>
        /// Returns an <see cref="ILogger"/> for the given <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name for which an <see cref="ILogger"/> should be returned</param>
        /// <returns>The <see cref="ILogger"/></returns>
        [DebuggerStepThrough]
        public ILogger GetLogger(string name)
        {
            return new Log4NetLogger(log4net.LogManager.GetLogger(name));
        }

        /// <summary>
        /// Returns an <see cref="ILogger"/> for the given <typeparamref name="T"/>
        /// </summary>
        /// <returns>The <see cref="ILogger"/></returns>
        [DebuggerStepThrough]
        public ILogger GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        /// <summary>
        /// Disposes the <see cref="Log4NetService"/>
        /// </summary>
        [DebuggerStepThrough]
        public void Dispose()
        {
            log4net.LogManager.Shutdown();
        }

        private static void ConfigureImpl(FileInfo configFile)
        {
            if (configFile == null) { throw new ArgumentException("Config file cannot be null", nameof(configFile)); }
            if (!configFile.Exists) { throw new FileNotFoundException("Could not find a valid log4net configuration file", configFile.FullName); }

            XmlConfigurator.ConfigureAndWatch(configFile);
        }
    }
}