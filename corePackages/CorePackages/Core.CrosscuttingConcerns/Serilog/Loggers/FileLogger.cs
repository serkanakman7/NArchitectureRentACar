using Core.CrosscuttingConcerns.Serilog.ConfigurationModels;
using Core.CrosscuttingConcerns.Serilog.Messages;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CrosscuttingConcerns.Serilog.Loggers
{
    public class FileLogger : LoggerServiceBase
    {
        private readonly IConfiguration _configuration;

        public FileLogger(IConfiguration configuration)
        {
            _configuration = configuration;

            FileLogConfigruration logConfig = _configuration.GetSection("SeriLogConfigurations:FileLogConfiguration").Get<FileLogConfigruration>() ?? throw new Exception(SerilogMessages.NullOptionsMessage);

            string logFilePath = $"{Directory.GetCurrentDirectory() + logConfig.FolderPath}.txt";

            Logger = new LoggerConfiguration().WriteTo.File(
                logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: null,
                fileSizeLimitBytes: 5000000,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{level}] {Message}{NewLine}{Exception}"
                ).CreateLogger();
        }
    }
}
