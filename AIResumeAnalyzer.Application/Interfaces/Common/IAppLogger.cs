using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Application.Interfaces.Common;

public interface IAppLogger<T>
{
    void LogInformation(string message);

    void LogWarning(string message);

    void LogError(string message);

    void LogError(Exception exception, string message);
}