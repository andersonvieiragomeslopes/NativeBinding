using System;
using Foundation;
using ObjCRuntime;

namespace AmplitudeBinding.iOS
{
    // @interface AMPIdentify : NSObject
    [BaseType(typeof(NSObject))]
    interface AMPIdentify
    {
        // @property (nonatomic, strong, readonly) NSMutableDictionary *userPropertyOperations;
        [Export("userPropertyOperations", ArgumentSemantic.Strong)]
        NSMutableDictionary UserPropertyOperations { get; }

        // + (instancetype)identify;
        [Static]
        [Export("identify")]
        AMPIdentify Create();

        // - (AMPIdentify *)add:(NSString *)property value:(NSObject *)value;
        [Export("add:value:")]
        AMPIdentify Add(string property, NSObject value);

        // - (AMPIdentify *)append:(NSString *)property value:(NSObject *)value;
        [Export("append:value:")]
        AMPIdentify Append(string property, NSObject value);

        // - (AMPIdentify *)clearAll;
        [Export("clearAll")]
        AMPIdentify ClearAll();

        // - (AMPIdentify *)prepend:(NSString *)property value:(NSObject *)value;
        [Export("prepend:value:")]
        AMPIdentify Prepend(string property, NSObject value);

        // - (AMPIdentify *)set:(NSString *)property value:(NSObject *)value;
        [Export("set:value:")]
        AMPIdentify Set(string property, NSObject value);

        // - (AMPIdentify *)setOnce:(NSString *)property value:(NSObject *)value;
        [Export("setOnce:value:")]
        AMPIdentify SetOnce(string property, NSObject value);

        // - (AMPIdentify *)unset:(NSString *)property;
        [Export("unset:")]
        AMPIdentify Unset(string property);

        // - (AMPIdentify *)preInsert:(NSString *)property value:(NSObject *)value;
        [Export("preInsert:value:")]
        AMPIdentify PreInsert(string property, NSObject value);

        // - (AMPIdentify *)postInsert:(NSString *)property value:(NSObject *)value;
        [Export("postInsert:value:")]
        AMPIdentify PostInsert(string property, NSObject value);

        // - (AMPIdentify *)remove:(NSString *)property value:(NSObject *)value;
        [Export("remove:value:")]
        AMPIdentify Remove(string property, NSObject value);
    }

    // @interface AMPRevenue : NSObject
    [BaseType(typeof(NSObject))]
    interface AMPRevenue
    {
        // @property (nonatomic, strong, readonly) NSString *productId;
        [NullAllowed, Export("productId", ArgumentSemantic.Strong)]
        string ProductId { get; }

        // @property (nonatomic, strong, readonly) NSNumber *price;
        [NullAllowed, Export("price", ArgumentSemantic.Strong)]
        NSNumber Price { get; }

        // @property (nonatomic, readonly) NSInteger quantity;
        [Export("quantity")]
        nint Quantity { get; }

        // @property (nonatomic, strong, readonly) NSString *revenueType;
        [NullAllowed, Export("revenueType", ArgumentSemantic.Strong)]
        string RevenueType { get; }

        // @property (nonatomic, strong, readonly) NSData *receipt;
        [NullAllowed, Export("receipt", ArgumentSemantic.Strong)]
        NSData Receipt { get; }

        // @property (nonatomic, strong, readonly) NSDictionary *properties;
        [NullAllowed, Export("properties", ArgumentSemantic.Strong)]
        NSDictionary Properties { get; }

        // + (instancetype)revenue;
        [Static]
        [Export("revenue")]
        AMPRevenue Create();

        // - (AMPRevenue *)setProductIdentifier:(NSString *)productIdentifier;
        [Export("setProductIdentifier:")]
        AMPRevenue SetProductIdentifier(string productIdentifier);

        // - (AMPRevenue *)setQuantity:(NSInteger)quantity;
        [Export("setQuantity:")]
        AMPRevenue SetQuantity(nint quantity);

        // - (AMPRevenue *)setPrice:(NSNumber *)price;
        [Export("setPrice:")]
        AMPRevenue SetPrice(NSNumber price);

        // - (AMPRevenue *)setRevenueType:(NSString *)revenueType;
        [Export("setRevenueType:")]
        AMPRevenue SetRevenueType(string revenueType);

        // - (AMPRevenue *)setReceipt:(NSData *)receipt;
        [Export("setReceipt:")]
        AMPRevenue SetReceipt(NSData receipt);

        // - (AMPRevenue *)setEventProperties:(NSDictionary *)eventProperties;
        [Export("setEventProperties:")]
        AMPRevenue SetEventProperties(NSDictionary eventProperties);

        // - (NSDictionary *)toNSDictionary;
        [Export("toNSDictionary")]
        NSDictionary ToNSDictionary();
    }

    // @interface AMPTrackingOptions : NSObject
    [BaseType(typeof(NSObject))]
    interface AMPTrackingOptions
    {
        // @property (nonatomic, strong, readonly) NSMutableSet *disabledFields;
        [Export("disabledFields", ArgumentSemantic.Strong)]
        NSMutableSet DisabledFields { get; }

        // + (instancetype)options;
        [Static]
        [Export("options")]
        AMPTrackingOptions Create();

        // - (AMPTrackingOptions *)disableCarrier;
        [Export("disableCarrier")]
        AMPTrackingOptions DisableCarrier();

        // - (AMPTrackingOptions *)disableCity;
        [Export("disableCity")]
        AMPTrackingOptions DisableCity();

        // - (AMPTrackingOptions *)disableCountry;
        [Export("disableCountry")]
        AMPTrackingOptions DisableCountry();

        // - (AMPTrackingOptions *)disableDeviceManufacturer;
        [Export("disableDeviceManufacturer")]
        AMPTrackingOptions DisableDeviceManufacturer();

        // - (AMPTrackingOptions *)disableDeviceModel;
        [Export("disableDeviceModel")]
        AMPTrackingOptions DisableDeviceModel();

        // - (AMPTrackingOptions *)disableDMA;
        [Export("disableDMA")]
        AMPTrackingOptions DisableDMA();

        // - (AMPTrackingOptions *)disableIDFA;
        [Export("disableIDFA")]
        AMPTrackingOptions DisableIDFA();

        // - (AMPTrackingOptions *)disableIDFV;
        [Export("disableIDFV")]
        AMPTrackingOptions DisableIDFV();

        // - (AMPTrackingOptions *)disableIPAddress;
        [Export("disableIPAddress")]
        AMPTrackingOptions DisableIPAddress();

        // - (AMPTrackingOptions *)disableLanguage;
        [Export("disableLanguage")]
        AMPTrackingOptions DisableLanguage();

        // - (AMPTrackingOptions *)disableLatLng;
        [Export("disableLatLng")]
        AMPTrackingOptions DisableLatLng();

        // - (AMPTrackingOptions *)disableOSName;
        [Export("disableOSName")]
        AMPTrackingOptions DisableOSName();

        // - (AMPTrackingOptions *)disableOSVersion;
        [Export("disableOSVersion")]
        AMPTrackingOptions DisableOSVersion();

        // - (AMPTrackingOptions *)disablePlatform;
        [Export("disablePlatform")]
        AMPTrackingOptions DisablePlatform();

        // - (AMPTrackingOptions *)disableRegion;
        [Export("disableRegion")]
        AMPTrackingOptions DisableRegion();

        // - (AMPTrackingOptions *)disableVersionName;
        [Export("disableVersionName")]
        AMPTrackingOptions DisableVersionName();

        // + (AMPTrackingOptions *)forCoppaControl;
        [Static]
        [Export("forCoppaControl")]
        AMPTrackingOptions ForCoppaControl();
    }

    // @interface AMPDefaultTrackingOptions : NSObject
    [BaseType(typeof(NSObject))]
    interface AMPDefaultTrackingOptions
    {
        // @property (nonatomic, assign) BOOL sessions;
        [Export("sessions")]
        bool Sessions { get; set; }

        // @property (nonatomic, assign) BOOL appLifecycles;
        [Export("appLifecycles")]
        bool AppLifecycles { get; set; }

        // @property (nonatomic, assign) BOOL deepLinks;
        [Export("deepLinks")]
        bool DeepLinks { get; set; }

        // @property (nonatomic, assign) BOOL screenViews;
        [Export("screenViews")]
        bool ScreenViews { get; set; }

        // + (instancetype)initWithSessions:(BOOL)sessions appLifecycles:(BOOL)appLifecycles deepLinks:(BOOL)deepLinks screenViews:(BOOL)screenViews;
        [Static]
        [Export("initWithSessions:appLifecycles:deepLinks:screenViews:")]
        AMPDefaultTrackingOptions Create(bool sessions, bool appLifecycles, bool deepLinks, bool screenViews);

        // + (instancetype)initWithAllEnabled;
        [Static]
        [Export("initWithAllEnabled")]
        AMPDefaultTrackingOptions CreateWithAllEnabled();

        // + (instancetype)initWithNoneEnabled;
        [Static]
        [Export("initWithNoneEnabled")]
        AMPDefaultTrackingOptions CreateWithNoneEnabled();
    }

    // @interface AMPPlan : NSObject
    [BaseType(typeof(NSObject))]
    interface AMPPlan
    {
        // @property (nonatomic, strong, readonly) NSString *branch;
        [NullAllowed, Export("branch", ArgumentSemantic.Strong)]
        string Branch { get; }

        // @property (nonatomic, strong, readonly) NSString *source;
        [NullAllowed, Export("source", ArgumentSemantic.Strong)]
        string Source { get; }

        // @property (nonatomic, strong, readonly) NSString *version;
        [NullAllowed, Export("version", ArgumentSemantic.Strong)]
        string Version { get; }

        // @property (nonatomic, strong, readonly) NSString *versionId;
        [NullAllowed, Export("versionId", ArgumentSemantic.Strong)]
        string VersionId { get; }

        // + (instancetype)plan;
        [Static]
        [Export("plan")]
        AMPPlan Create();

        // - (AMPPlan *)setBranch:(NSString *)branch;
        [Export("setBranch:")]
        AMPPlan SetBranch(string branch);

        // - (AMPPlan *)setSource:(NSString *)source;
        [Export("setSource:")]
        AMPPlan SetSource(string source);

        // - (AMPPlan *)setVersion:(NSString *)version;
        [Export("setVersion:")]
        AMPPlan SetVersion(string version);

        // - (AMPPlan *)setVersionId:(NSString *)versionId;
        [Export("setVersionId:")]
        AMPPlan SetVersionId(string versionId);
    }

    // @interface AMPIngestionMetadata : NSObject
    [BaseType(typeof(NSObject))]
    interface AMPIngestionMetadata
    {
        // @property (nonatomic, strong, readonly) NSString *sourceName;
        [NullAllowed, Export("sourceName", ArgumentSemantic.Strong)]
        string SourceName { get; }

        // @property (nonatomic, strong, readonly) NSString *sourceVersion;
        [NullAllowed, Export("sourceVersion", ArgumentSemantic.Strong)]
        string SourceVersion { get; }

        // + (instancetype)ingestionMetadata;
        [Static]
        [Export("ingestionMetadata")]
        AMPIngestionMetadata Create();

        // - (AMPIngestionMetadata *)setSourceName:(NSString *)sourceName;
        [Export("setSourceName:")]
        AMPIngestionMetadata SetSourceName(string sourceName);

        // - (AMPIngestionMetadata *)setSourceVersion:(NSString *)sourceVersion;
        [Export("setSourceVersion:")]
        AMPIngestionMetadata SetSourceVersion(string sourceVersion);
    }

    // @interface AMPMiddlewarePayload : NSObject
    [BaseType(typeof(NSObject))]
    interface AMPMiddlewarePayload
    {
        // @property NSMutableDictionary * _Nonnull event;
        [Export("event", ArgumentSemantic.Assign)]
        NSMutableDictionary Event { get; set; }

        // @property NSMutableDictionary * _Nullable extra;
        [NullAllowed, Export("extra", ArgumentSemantic.Assign)]
        NSMutableDictionary Extra { get; set; }

        // - (instancetype _Nonnull)initWithEvent:(NSMutableDictionary *)event withExtra:(NSMutableDictionary *)extra;
        [Export("initWithEvent:withExtra:")]
        NativeHandle Constructor(NSMutableDictionary @event, [NullAllowed] NSMutableDictionary extra);
    }

    // @protocol AMPMiddleware
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface AMPMiddleware
    {
        // @required - (void)run:(AMPMiddlewarePayload *)payload next:(AMPMiddlewareNext)next;
        [Abstract]
        [Export("run:next:")]
        void Run(AMPMiddlewarePayload payload, Action<AMPMiddlewarePayload> next);
    }

    // @interface Amplitude : NSObject
    [BaseType(typeof(NSObject))]
    interface Amplitude
    {
        // @property (nonatomic, copy, readonly) NSString *apiKey;
        [Export("apiKey")]
        string ApiKey { get; }

        // @property (nonatomic, copy, readonly, nullable) NSString *userId;
        [NullAllowed, Export("userId")]
        string UserId { get; }

        // @property (nonatomic, copy, readonly) NSString *deviceId;
        [Export("deviceId")]
        string DeviceId { get; }

        // @property (nonatomic, copy, readonly, nullable) NSString *instanceName;
        [NullAllowed, Export("instanceName")]
        string InstanceName { get; }

        // @property (nonatomic, assign, readwrite) BOOL optOut;
        [Export("optOut")]
        bool OptOut { get; set; }

        // @property (nonatomic, assign, readwrite) BOOL useDynamicConfig;
        [Export("useDynamicConfig")]
        bool UseDynamicConfig { get; set; }

        // @property (nonatomic, assign) int eventUploadThreshold;
        [Export("eventUploadThreshold")]
        int EventUploadThreshold { get; set; }

        // @property (nonatomic, assign) int eventUploadMaxBatchSize;
        [Export("eventUploadMaxBatchSize")]
        int EventUploadMaxBatchSize { get; set; }

        // @property (nonatomic, assign) int eventMaxCount;
        [Export("eventMaxCount")]
        int EventMaxCount { get; set; }

        // @property (nonatomic, assign) int eventUploadPeriodSeconds;
        [Export("eventUploadPeriodSeconds")]
        int EventUploadPeriodSeconds { get; set; }

        // @property (nonatomic, assign) long minTimeBetweenSessionsMillis;
        [Export("minTimeBetweenSessionsMillis")]
        nint MinTimeBetweenSessionsMillis { get; set; }

        // @property (nonatomic, strong) AMPDefaultTrackingOptions *defaultTracking;
        [Export("defaultTracking", ArgumentSemantic.Strong)]
        AMPDefaultTrackingOptions DefaultTracking { get; set; }

        // @property (nonatomic, copy, nullable) NSString *libraryName;
        [NullAllowed, Export("libraryName")]
        string LibraryName { get; set; }

        // @property (nonatomic, copy, nullable) NSString *libraryVersion;
        [NullAllowed, Export("libraryVersion")]
        string LibraryVersion { get; set; }

        // @property (nonatomic, copy, readonly) NSString *contentTypeHeader;
        [Export("contentTypeHeader")]
        string ContentTypeHeader { get; }

        // @property (nonatomic, assign) BOOL deferCheckInForeground;
        [Export("deferCheckInForeground")]
        bool DeferCheckInForeground { get; set; }

        // + (Amplitude *)instance;
        [Static]
        [Export("instance")]
        Amplitude Instance { get; }

        // + (Amplitude *)instanceWithName:(nullable NSString *)instanceName;
        [Static]
        [Export("instanceWithName:")]
        Amplitude GetInstanceWithName([NullAllowed] string instanceName);

        // - (void)initializeApiKey:(NSString *)apiKey;
        [Export("initializeApiKey:")]
        void InitializeApiKey(string apiKey);

        // - (void)initializeApiKey:(NSString *)apiKey userId:(nullable NSString *)userId;
        [Export("initializeApiKey:userId:")]
        void InitializeApiKey(string apiKey, [NullAllowed] string userId);

        // - (void)checkInForeground;
        [Export("checkInForeground")]
        void CheckInForeground();

        // - (void)logEvent:(NSString *)eventType;
        [Export("logEvent:")]
        void LogEvent(string eventType);

        // - (void)logEvent:(NSString *)eventType withEventProperties:(nullable NSDictionary *)eventProperties;
        [Export("logEvent:withEventProperties:")]
        void LogEvent(string eventType, [NullAllowed] NSDictionary eventProperties);

        // - (void)logEvent:(NSString *)eventType withEventProperties:(nullable NSDictionary *)eventProperties outOfSession:(BOOL)outOfSession;
        [Export("logEvent:withEventProperties:outOfSession:")]
        void LogEvent(string eventType, [NullAllowed] NSDictionary eventProperties, bool outOfSession);

        // - (void)logEvent:(NSString *)eventType withEventProperties:(nullable NSDictionary *)eventProperties withGroups:(nullable NSDictionary *)groups;
        [Export("logEvent:withEventProperties:withGroups:")]
        void LogEventWithGroups(string eventType, [NullAllowed] NSDictionary eventProperties, [NullAllowed] NSDictionary groups);

        // - (void)logEvent:(NSString *)eventType withEventProperties:(nullable NSDictionary *)eventProperties withGroups:(nullable NSDictionary *)groups outOfSession:(BOOL)outOfSession;
        [Export("logEvent:withEventProperties:withGroups:outOfSession:")]
        void LogEventWithGroups(string eventType, [NullAllowed] NSDictionary eventProperties, [NullAllowed] NSDictionary groups, bool outOfSession);

        // - (void)logEvent:(NSString *)eventType withEventProperties:(nullable NSDictionary *)eventProperties withGroups:(nullable NSDictionary *)groups withTimestamp:(NSNumber *)timestamp outOfSession:(BOOL)outOfSession;
        [Export("logEvent:withEventProperties:withGroups:withTimestamp:outOfSession:")]
        void LogEventWithGroups(string eventType, [NullAllowed] NSDictionary eventProperties, [NullAllowed] NSDictionary groups, NSNumber timestamp, bool outOfSession);

        // - (void)logRevenueV2:(AMPRevenue *)revenue;
        [Export("logRevenueV2:")]
        void LogRevenueV2(AMPRevenue revenue);

        // - (void)identify:(AMPIdentify *)identify;
        [Export("identify:")]
        void Identify(AMPIdentify identify);

        // - (void)identify:(AMPIdentify *)identify outOfSession:(BOOL)outOfSession;
        [Export("identify:outOfSession:")]
        void Identify(AMPIdentify identify, bool outOfSession);

        // - (void)setUserProperties:(NSDictionary *)userProperties;
        [Export("setUserProperties:")]
        void SetUserProperties(NSDictionary userProperties);

        // - (void)clearUserProperties;
        [Export("clearUserProperties")]
        void ClearUserProperties();

        // - (void)setGroup:(NSString *)groupType groupName:(NSObject *)groupName;
        [Export("setGroup:groupName:")]
        void SetGroup(string groupType, NSObject groupName);

        // - (void)groupIdentifyWithGroupType:(NSString *)groupType groupName:(NSObject *)groupName groupIdentify:(AMPIdentify *)groupIdentify;
        [Export("groupIdentifyWithGroupType:groupName:groupIdentify:")]
        void GroupIdentify(string groupType, NSObject groupName, AMPIdentify groupIdentify);

        // - (void)groupIdentifyWithGroupType:(NSString *)groupType groupName:(NSObject *)groupName groupIdentify:(AMPIdentify *)groupIdentify outOfSession:(BOOL)outOfSession;
        [Export("groupIdentifyWithGroupType:groupName:groupIdentify:outOfSession:")]
        void GroupIdentify(string groupType, NSObject groupName, AMPIdentify groupIdentify, bool outOfSession);

        // - (void)setUserId:(nullable NSString *)userId;
        [Export("setUserId:")]
        void SetUserId([NullAllowed] string userId);

        // - (void)setUserId:(nullable NSString *)userId startNewSession:(BOOL)startNewSession;
        [Export("setUserId:startNewSession:")]
        void SetUserId([NullAllowed] string userId, bool startNewSession);

        // - (void)setDeviceId:(NSString *)deviceId;
        [Export("setDeviceId:")]
        void SetDeviceId(string deviceId);

        // - (void)setOptOut:(BOOL)enabled;
        [Export("setOptOut:")]
        void SetOptOut(bool enabled);

        // - (void)setOffline:(BOOL)offline;
        [Export("setOffline:")]
        void SetOffline(bool offline);

        // - (void)useAdvertisingIdForDeviceId;
        [Export("useAdvertisingIdForDeviceId")]
        void UseAdvertisingIdForDeviceId();

        // - (void)setTrackingOptions:(AMPTrackingOptions *)options;
        [Export("setTrackingOptions:")]
        void SetTrackingOptions(AMPTrackingOptions options);

        // - (void)enableCoppaControl;
        [Export("enableCoppaControl")]
        void EnableCoppaControl();

        // - (void)disableCoppaControl;
        [Export("disableCoppaControl")]
        void DisableCoppaControl();

        // - (void)setServerUrl:(NSString *)serverUrl;
        [Export("setServerUrl:")]
        void SetServerUrl(string serverUrl);

        // - (void)setContentTypeHeader:(NSString *)contentType;
        [Export("setContentTypeHeader:")]
        void SetContentTypeHeader(string contentType);

        // - (void)setBearerToken:(NSString *)token;
        [Export("setBearerToken:")]
        void SetBearerToken(string token);

        // - (void)setPlan:(AMPPlan *)plan;
        [Export("setPlan:")]
        void SetPlan(AMPPlan plan);

        // - (void)setIngestionMetadata:(AMPIngestionMetadata *)ingestionMetadata;
        [Export("setIngestionMetadata:")]
        void SetIngestionMetadata(AMPIngestionMetadata ingestionMetadata);

        // - (AMPServerZone)serverZone;
        [Export("serverZone")]
        AMPServerZone ServerZone { get; }

        // - (void)setServerZone:(AMPServerZone)serverZone;
        [Export("setServerZone:")]
        void SetServerZone(AMPServerZone serverZone);

        // - (void)setServerZone:(AMPServerZone)serverZone updateServerUrl:(BOOL)updateServerUrl;
        [Export("setServerZone:updateServerUrl:")]
        void SetServerZone(AMPServerZone serverZone, bool updateServerUrl);

        // - (void)addEventMiddleware:(id<AMPMiddleware>)middleware;
        [Export("addEventMiddleware:")]
        void AddEventMiddleware(NSObject middleware);

        // - (void)removeEventMiddleware:(id<AMPMiddleware>)middleware;
        [Export("removeEventMiddleware:")]
        void RemoveEventMiddleware(NSObject middleware);

        // - (NSString *)getDeviceId;
        [Export("getDeviceId")]
        string GetDeviceId();

        // - (void)regenerateDeviceId;
        [Export("regenerateDeviceId")]
        void RegenerateDeviceId();

        // - (long long)getSessionId;
        [Export("getSessionId")]
        long GetSessionId();

        // - (void)setSessionId:(long long)timestamp;
        [Export("setSessionId:")]
        void SetSessionId(long timestamp);

        // - (void)uploadEvents;
        [Export("uploadEvents")]
        void UploadEvents();

        // - (void)printEventsCount;
        [Export("printEventsCount")]
        void PrintEventsCount();
    }
}
