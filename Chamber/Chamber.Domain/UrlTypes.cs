using System.Web;
using Chamber.Domain.Constants;

namespace Chamber.Domain
{
    public enum UrlType
    {
        Member,
        Tag
    }

    public static class UrlTypes
    {
        public static string UrlTypeName(UrlType e)
        {
            switch (e)
            {
                case UrlType.Member:
                    return SiteConstants.Instance.MemberUrlIdentifier;
                default:
                    return SiteConstants.Instance.TagsUrlIdentifier;

            }
        }

        public static string GenerateUrl(UrlType e, string slug)
        {
            return VirtualPathUtility.ToAbsolute($"~/{UrlTypeName(e)}/{HttpUtility.UrlEncode(HttpUtility.HtmlDecode(slug))}/");
        }

        public static string GenerateFileUrl(string filePath)
        {
            return VirtualPathUtility.ToAbsolute(filePath);
        }
    }
}
