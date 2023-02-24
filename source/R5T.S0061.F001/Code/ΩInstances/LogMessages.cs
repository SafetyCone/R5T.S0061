using System;


namespace R5T.S0061.F001
{
    public class LogMessages : ILogMessages
    {
        #region Infrastructure

        public static ILogMessages Instance { get; } = new LogMessages();


        private LogMessages()
        {
        }

        #endregion
    }
}
