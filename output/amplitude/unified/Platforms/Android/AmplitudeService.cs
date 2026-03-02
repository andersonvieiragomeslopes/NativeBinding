using System.Collections.Generic;

namespace Amplitude.Maui
{
    public class AmplitudeService : IAmplitudeService
    {
        private global::Amplitude.Android.Amplitude _instance = null!;

        public void Initialize(string apiKey, string? userId = null)
        {
            var context = global::Android.App.Application.Context;
            var config = new global::Amplitude.Android.Configuration(apiKey, context);
            _instance = new global::Amplitude.Android.Amplitude(config);
        }

        public void LogEvent(string eventType, IDictionary<string, object>? eventProperties = null)
        {
            if (eventProperties != null)
            {
                var map = new Dictionary<string, Java.Lang.Object>();
                foreach (var kvp in eventProperties)
                    map[kvp.Key] = new Java.Lang.String(kvp.Value?.ToString() ?? "");
                _instance.Track(eventType, map);
            }
            else
            {
                _instance.Track(eventType);
            }
        }

        public void LogRevenue(string? productId, double price, int quantity = 1, string? revenueType = null)
        {
            var revenue = new global::Com.Amplitude.Core.Events.Revenue();
            revenue.Price = new Java.Lang.Double(price);
            revenue.Quantity = quantity;
            if (productId != null)
                revenue.ProductId = productId;
            if (revenueType != null)
                revenue.RevenueType = revenueType;
            _instance.Revenue(revenue);
        }

        public void SetUserProperty(string property, object value)
        {
            var props = new Dictionary<string, Java.Lang.Object>
            {
                [property] = new Java.Lang.String(value?.ToString() ?? "")
            };
            _instance.Identify(props);
        }

        public void IncrementUserProperty(string property, double value)
        {
            var identify = new global::Com.Amplitude.Core.Events.Identify();
            identify.Add(property, value);
            _instance.Identify(identify);
        }

        public void ClearUserProperties()
        {
            var identify = new global::Com.Amplitude.Core.Events.Identify();
            identify.ClearAll();
            _instance.Identify(identify);
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
            return _instance.Configuration?.DeviceId;
        }

        public long GetSessionId()
        {
            return _instance.SessionId;
        }

        public void SetGroup(string groupType, string groupName)
        {
            _instance.SetGroup(groupType, groupName);
        }

        public void SetOptOut(bool optOut)
        {
            if (_instance.Configuration != null)
                _instance.Configuration.OptOut = optOut;
        }

        public void UploadEvents()
        {
            _instance.Flush();
        }

        public void Reset()
        {
            _instance.Reset();
        }

        public void SetServerZone(AmplitudeServerZone zone)
        {
            if (_instance.Configuration != null)
            {
                _instance.Configuration.ServerZone = zone == AmplitudeServerZone.EU
                    ? global::Com.Amplitude.Core.ServerZone.Eu!
                    : global::Com.Amplitude.Core.ServerZone.Us!;
            }
        }

        public void SetServerUrl(string serverUrl)
        {
            if (_instance.Configuration != null)
                _instance.Configuration.ServerUrl = serverUrl;
        }
    }
}
