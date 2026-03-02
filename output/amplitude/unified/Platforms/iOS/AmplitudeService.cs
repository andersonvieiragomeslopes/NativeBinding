using System.Collections.Generic;
using Foundation;
using AmplitudeBinding.iOS;

namespace Amplitude.Maui
{
    public class AmplitudeService : IAmplitudeService
    {
        private global::AmplitudeBinding.iOS.Amplitude _instance = null!;

        public void Initialize(string apiKey, string? userId = null)
        {
            _instance = global::AmplitudeBinding.iOS.Amplitude.Instance;
            if (userId != null)
                _instance.InitializeApiKey(apiKey, userId);
            else
                _instance.InitializeApiKey(apiKey);
        }

        public void LogEvent(string eventType, IDictionary<string, object>? eventProperties = null)
        {
            if (eventProperties != null)
            {
                var dict = NSDictionary.FromObjectsAndKeys(
                    new List<object>(eventProperties.Values).ToArray(),
                    new List<string>(eventProperties.Keys).ToArray());
                _instance.LogEvent(eventType, dict);
            }
            else
            {
                _instance.LogEvent(eventType);
            }
        }

        public void LogRevenue(string? productId, double price, int quantity = 1, string? revenueType = null)
        {
            var revenue = AMPRevenue.Create();
            revenue.SetPrice(new NSNumber(price));
            revenue.SetQuantity(quantity);
            if (productId != null)
                revenue.SetProductIdentifier(productId);
            if (revenueType != null)
                revenue.SetRevenueType(revenueType);
            _instance.LogRevenueV2(revenue);
        }

        public void SetUserProperty(string property, object value)
        {
            var identify = AMPIdentify.Create();
            identify.Set(property, NSObject.FromObject(value));
            _instance.Identify(identify);
        }

        public void IncrementUserProperty(string property, double value)
        {
            var identify = AMPIdentify.Create();
            identify.Add(property, new NSNumber(value));
            _instance.Identify(identify);
        }

        public void ClearUserProperties()
        {
            _instance.ClearUserProperties();
        }

        public void SetUserId(string? userId)
        {
            _instance.SetUserId(userId);
        }

        public void SetDeviceId(string deviceId)
        {
            _instance.SetDeviceId(deviceId);
        }

        public string? GetDeviceId()
        {
            return _instance.GetDeviceId();
        }

        public long GetSessionId()
        {
            return _instance.GetSessionId();
        }

        public void SetGroup(string groupType, string groupName)
        {
            _instance.SetGroup(groupType, new NSString(groupName));
        }

        public void SetOptOut(bool optOut)
        {
            _instance.SetOptOut(optOut);
        }

        public void UploadEvents()
        {
            _instance.UploadEvents();
        }

        public void Reset()
        {
            _instance.SetUserId(null);
            _instance.RegenerateDeviceId();
        }

        public void SetServerZone(AmplitudeServerZone zone)
        {
            _instance.SetServerZone((AMPServerZone)(long)zone);
        }

        public void SetServerUrl(string serverUrl)
        {
            _instance.SetServerUrl(serverUrl);
        }
    }
}
