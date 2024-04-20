using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.CrosscuttingConcerns.Exceptions.Extensions
{
    public static class ProblemDetailExtensions
    {
        public static string AsJson<TProblemDetails>(this TProblemDetails details) where TProblemDetails : ProblemDetails => JsonSerializer.Serialize(details);
    }
}
