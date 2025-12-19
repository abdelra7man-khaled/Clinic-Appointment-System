namespace Clinic.Services.Logging
{
    public class Logger
    {
        private static Logger _instance = null;
        private static readonly object _lock = new object();
        private Logger() { }

        public static Logger Instance
        {
            get
            {
                if (_instance is null)
                {
                    lock (_lock)
                    {
                        if (_instance is null)
                        {
                            _instance = new Logger();
                        }
                    }
                }

                return _instance;
            }
        }

        private void Log(string message, LogType logType)
        {
            if (logType == LogType.INFO)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{logType.ToString()}: [{DateTime.Now}] {message}");
            }
            else if (logType == LogType.SUCCESS)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{logType.ToString()}: [{DateTime.Now}] {message}");
            }
            else if (logType == LogType.ERROR)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{logType.ToString()}: [{DateTime.Now}] {message}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{logType.ToString()}: [{DateTime.Now}] {message}");
            }

            Console.ResetColor();

        }

        public void LogInfo(string message)
        {
            Log(message, LogType.INFO);
        }

        public void LogSuccess(string message)
        {
            Log(message, LogType.SUCCESS);
        }
        public void LogError(string message)
        {
            Log(message, LogType.ERROR);
        }

        public void LogWarning(string message)
        {
            Log(message, LogType.WARNING);
        }
    }
}
