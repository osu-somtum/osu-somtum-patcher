using System.Reflection;
using OsuPatcher.Runtime.Constants;
using OsuPatcher.Runtime.Helpers;

namespace OsuPatcher.Runtime.Wrappers
{
    /// <summary>
    /// Manages in-game notifications.
    /// </summary>
    internal class NotificationManager
    {
        private static readonly MethodBase BaseShowMessage = ILPatch.FindMethodBySignature(Patterns.NotificationManager_ShowMessage);

        private static readonly MethodBase BaseShowMessageMassive = ILPatch.FindMethodBySignature(Patterns.NotificationManager_ShowMessageMassive);

        /// <summary>
        /// Shows a standard notification message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="color">The color of the notification.</param>
        /// <param name="time">Duration in milliseconds.</param>
        public static void ShowMessage(
            string message,
            object color,
            int time = 5000)
         => BaseShowMessage.Invoke(null, new object[] { message, color, time, null });

        /// <summary>
        /// Shows a massive notification message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="time">Duration in milliseconds.</param>
        /// <param name="notificationType">Type of the notification.</param>
        public static void ShowMessageMassive(
            string message,
            int time = 5000,
            NotificationType notificationType = NotificationType.Info)
        => BaseShowMessageMassive.Invoke(null, new object[] { message, time, notificationType });

        /// <summary>
        /// Defines the type of notification.
        /// </summary>
        public enum NotificationType
        {
            Info,
            Warning,
            Error
        }
    }
}
