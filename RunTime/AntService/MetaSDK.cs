

namespace Ant.Metaverse
{
    public class MetaSDK
    {
        // 鲸探小程序appid
        public const string JINGTANAPPID = "2021002139687739";
        private static string TenantId;
        private static string AppId;

        public static string GetTenantId()
        {
            return TenantId;
        }

        public static string GetAppId()
        {
            return AppId;
        }

        public static void Init(string tenantId, string appId)
        {
            TenantId = tenantId;
            AppId = appId;
        }

        public static void Dispose()
        {
            TenantId = null;
            AppId = null;
        }
    }
}