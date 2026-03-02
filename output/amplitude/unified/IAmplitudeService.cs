using System.Collections.Generic;

namespace Amplitude.Maui
{
    /// <summary>
    /// Cross-platform interface for the Amplitude analytics SDK.
    /// </summary>
    public interface IAmplitudeService
    {
        /// <summary>
        /// Initialize the Amplitude SDK with your API key.
        /// </summary>
        void Initialize(string apiKey, string? userId = null);

        /// <summary>
        /// Log an event with an optional set of properties.
        /// </summary>
        void LogEvent(string eventType, IDictionary<string, object>? eventProperties = null);

        /// <summary>
        /// Log a revenue event.
        /// </summary>
        void LogRevenue(string? productId, double price, int quantity = 1, string? revenueType = null);

        /// <summary>
        /// Set a user property to a value.
        /// </summary>
        void SetUserProperty(string property, object value);

        /// <summary>
        /// Increment a numeric user property.
        /// </summary>
        void IncrementUserProperty(string property, double value);

        /// <summary>
        /// Clear all user properties.
        /// </summary>
        void ClearUserProperties();

        /// <summary>
        /// Set the user ID for the current user.
        /// </summary>
        void SetUserId(string? userId);

        /// <summary>
        /// Set the device ID.
        /// </summary>
        void SetDeviceId(string deviceId);

        /// <summary>
        /// Get the current device ID.
        /// </summary>
        string? GetDeviceId();

        /// <summary>
        /// Get the current session ID.
        /// </summary>
        long GetSessionId();

        /// <summary>
        /// Assign the user to a group.
        /// </summary>
        void SetGroup(string groupType, string groupName);

        /// <summary>
        /// Enable or disable opt-out. When opted out, no events are tracked.
        /// </summary>
        void SetOptOut(bool optOut);

        /// <summary>
        /// Force upload all pending events.
        /// </summary>
        void UploadEvents();

        /// <summary>
        /// Reset the user identity (userId and deviceId) for a new anonymous user.
        /// </summary>
        void Reset();

        /// <summary>
        /// Set the server zone (US or EU).
        /// </summary>
        void SetServerZone(AmplitudeServerZone zone);

        /// <summary>
        /// Set a custom server URL for event uploads.
        /// </summary>
        void SetServerUrl(string serverUrl);
    }

    /// <summary>
    /// Amplitude server zones.
    /// </summary>
    public enum AmplitudeServerZone
    {
        US = 0,
        EU = 1,
    }
}
