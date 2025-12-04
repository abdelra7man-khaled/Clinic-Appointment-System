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

        public void Log(string message, LogType logtype)
        {
            if (logtype == LogType.Info)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"{logtype.ToString()}: [{DateTime.Now}] {message}");
            }
            else if (logtype == LogType.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{logtype.ToString()}: [{DateTime.Now}] {message}");
            }
            else if (logtype == LogType.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{logtype.ToString()}: [{DateTime.Now}] {message}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{logtype.ToString()}: [{DateTime.Now}] {message}");
            }

            Console.ResetColor();

        }

        public void LogInfo(string message)
        {
            Log(message, LogType.Info);
        }

        public void LogSuccess(string message)
        {
            Log(message, LogType.Success);
        }
        public void LogError(string message)
        {
            Log(message, LogType.Error);
        }

        public void LogWarning(string message)
        {
            Log(message, LogType.Warning);
        }
    }
}
