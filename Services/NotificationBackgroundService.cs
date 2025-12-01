namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// Background service that processes pending notifications and sends appointment reminders
    /// Runs every minute to check for pending notifications and upcoming appointments
    /// </summary>
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationBackgroundService> _logger;

        public NotificationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<NotificationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                        var appointmentService = scope.ServiceProvider.GetRequiredService<IAppointmentService>();

                        // Process pending notifications
                        await notificationService.ProcessPendingNotificationsAsync();

                        // Check for appointments that need reminders (24h and 1h before)
                        await appointmentService.SendUpcomingAppointmentRemindersAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Notification Background Service");
                }

                // Wait 1 minute before next check
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("Notification Background Service is stopping.");
        }
    }
}

