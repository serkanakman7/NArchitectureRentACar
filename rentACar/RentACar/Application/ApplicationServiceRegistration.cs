using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Core.CrosscuttingConcerns.Rules;
using FluentValidation;
using Core.Application.Pipelines.Validation;
using Core.Application.Pipelines.Transaction;
using Core.Application.Pipelines.Caching;
using Core.Application.Pipelines.Logging;
using Core.CrosscuttingConcerns.Serilog;
using Core.CrosscuttingConcerns.Serilog.Loggers;

namespace Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMediatR(configure =>
            {
                configure.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

                configure.AddOpenBehavior(typeof(RequestValidationBehavior<,>));

                configure.AddOpenBehavior(typeof(TransactionScopeBehavior<,>));

                configure.AddOpenBehavior(typeof(CachingBehavior<,>));

                configure.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });

            services.AddSingleton<LoggerServiceBase, PostgreSqlLogger>();

            services.AddSubClassOfType(Assembly.GetExecutingAssembly(), typeof(BaseBusinessRules));

            return services;
        }

        public static IServiceCollection AddSubClassOfType(this IServiceCollection services, Assembly assembly, Type type, Func<IServiceCollection, Type, IServiceCollection> addWithLifeCycle = null)
        {
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();
            foreach (var item in types)
                if (addWithLifeCycle == null)
                    services.AddScoped(item);
                else
                    addWithLifeCycle(services, item);
            return services;

        }
    }
}
