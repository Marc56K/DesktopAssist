using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssistApp
{
    internal class Logger
    {
        private readonly Action<string> _logAction;

        public Logger(Action<string> logAction)
        {
            _logAction = logAction;
        }

        public void Info(string message)
        {
            _logAction?.Invoke(message);
        }

        public void Warning(string message)
        {
            _logAction?.Invoke("WARNING: " + message);
        }

        public void Error(string message)
        {
            _logAction?.Invoke("ERROR: " + message);
        }
    }
}
