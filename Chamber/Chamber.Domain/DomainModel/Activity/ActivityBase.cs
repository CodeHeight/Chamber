using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Chamber.Domain.DomainModel.Activity
{
    public abstract class ActivityBase
    {
        public const string Equality = @"=";
        protected const string Separator = @",";
        protected const string RegexNameValue = @"^([^=]+)=([^=]+)$";

        public Activity ActivityMapped { get; set; }

        /// <summary>
        /// Turn the unprocessed data into keyed name-value pairs
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> UnpackData(Activity activity)
        {
            if (activity == null)
            {
                throw new ApplicationException("Attempting to unpack activity data when no database record.");
            }

            var keyValuePairs = new Dictionary<string, string>();

            // Form of data is "name=value,name=value" etc
            var keyValuePairsRaw = activity.Data.Split(new[] { ',' });

            var pattern = new Regex(RegexNameValue, RegexOptions.None);

            foreach (var keyValuePairRaw in keyValuePairsRaw)
            {
                var match = pattern.Match(keyValuePairRaw);

                if (match.Success)
                {
                    keyValuePairs.Add(match.Groups[1].Value, match.Groups[2].Value);
                }
            }

            return keyValuePairs;
        }
    }
}