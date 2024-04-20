using Core.CrosscuttingConcerns.Serilog.ConfigurationModels;
using Core.CrosscuttingConcerns.Serilog.Messages;
using Microsoft.Extensions.Configuration;
using NpgsqlTypes;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CrosscuttingConcerns.Serilog.Loggers
{
    public class PostgreSqlLogger : LoggerServiceBase
    {
        private readonly IConfiguration _configuration;

        public PostgreSqlLogger(IConfiguration configuration)
        {
            _configuration = configuration;

            PostgreSqlConfiguration logConfiguration = _configuration.GetSection("SeriLogConfigurations:PostgreSqlConfiguration").Get<PostgreSqlConfiguration>()
                ?? throw new Exception(SerilogMessages.NullOptionsMessage);

            //IDictionary<string, ColumnWriterBase> columnOptions = new Dictionary<string, ColumnWriterBase>
            //{
            //    {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
            //    {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
            //    {"level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
            //    {"raise_date", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
            //    {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
            //    {"properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
            //    {"props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
            //    {"machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") }
            //};

            global::Serilog.Core.Logger seriLogConfig = new LoggerConfiguration()
                                .WriteTo.PostgreSQL(logConfiguration.ConnectionString, logConfiguration.TableName, columnOptions:ColumnOptions.Default, needAutoCreateTable:logConfiguration.AutoCreateSqlTable)
                                .CreateLogger();

            Logger = seriLogConfig;
        }
    }
}
