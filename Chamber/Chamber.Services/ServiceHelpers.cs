using System;
using System.Collections.Generic;
using System.Linq;
using Chamber.Domain.DomainModel;
using Chamber.Utilities;

namespace Chamber.Services
{
    public static class ServiceHelpers
    {
        #region Cache

        /// <summary>
        /// permission cache key for getting and setting keys
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string ReturnCacheKey(Guid categoryId, Guid roleId)
        {
            return $"permissioncache-{categoryId}-{roleId}";
        }
        #endregion

        #region Url

        /// <summary>
        /// Creates a friendly url
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string CreateUrl(string name)
        {
            return StringUtils.CreateUrl(name, "-");
        }

        public static string GenerateSlug(string stringToSlug, IEnumerable<Entity> similarList, string previousSlug)
        {
            // url generator
            var slug = CreateUrl(stringToSlug);

            // To list the entities
            var matchingEntities = similarList.ToList();

            // if the similarList is empty, just return this slug
            if (!matchingEntities.Any())
            {
                return slug;
            }

            // If the previous slug is null, then it's a newly created Entity
            if (string.IsNullOrEmpty(previousSlug))
            {
                // Now check another entity doesn't have the same one
                if (matchingEntities.Any())
                {
                    // See if there is only one. And if it matches exactly
                    // someone else has this, grab all like it and do a count, stick a suffix on
                    slug = string.Concat(slug, "-", matchingEntities.Count);
                }
            }
            else
            {
                // It's an update, see if they have changed the title by checking slugs
                if (slug != previousSlug)
                {
                    // Name/Title has changed
                    if (matchingEntities.Any())
                    {
                        slug = string.Concat(slug, "-", matchingEntities.Count);
                    }
                }
            }

            return slug;
        }

        #endregion

        #region Extension Methods

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        #endregion

        public static IList<State> CreateStateList()
        {
            List<State> states = new List<State>();

            states.Add(new State() { Abbreviations = "AL", Name = "Alabama" });
            states.Add(new State() { Abbreviations = "AK", Name = "Alaska" });
            states.Add(new State() { Abbreviations = "AZ", Name = "Arizona" });
            states.Add(new State() { Abbreviations = "AR", Name = "Arkansas" });
            states.Add(new State() { Abbreviations = "CA", Name = "California" });
            states.Add(new State() { Abbreviations = "CO", Name = "Colorado" });
            states.Add(new State() { Abbreviations = "CT", Name = "Connecticut" });
            states.Add(new State() { Abbreviations = "DE", Name = "Delaware" });
            states.Add(new State() { Abbreviations = "FL", Name = "Florida" });
            states.Add(new State() { Abbreviations = "GA", Name = "Georgia" });
            states.Add(new State() { Abbreviations = "HI", Name = "Hawaii" });
            states.Add(new State() { Abbreviations = "ID", Name = "Idaho" });
            states.Add(new State() { Abbreviations = "IL", Name = "Illinois" });
            states.Add(new State() { Abbreviations = "IN", Name = "Indiana" });
            states.Add(new State() { Abbreviations = "IA", Name = "Iowa" });
            states.Add(new State() { Abbreviations = "KS", Name = "Kansas" });
            states.Add(new State() { Abbreviations = "KY", Name = "Kentucky" });
            states.Add(new State() { Abbreviations = "LA", Name = "Louisiana" });
            states.Add(new State() { Abbreviations = "ME", Name = "Maine" });
            states.Add(new State() { Abbreviations = "MD", Name = "Maryland" });
            states.Add(new State() { Abbreviations = "MA", Name = "Massachusetts" });
            states.Add(new State() { Abbreviations = "MI", Name = "Michigan" });
            states.Add(new State() { Abbreviations = "MN", Name = "Minnesota" });
            states.Add(new State() { Abbreviations = "MS", Name = "Mississippi" });
            states.Add(new State() { Abbreviations = "MO", Name = "Missouri" });
            states.Add(new State() { Abbreviations = "MT", Name = "Montana" });
            states.Add(new State() { Abbreviations = "NE", Name = "Nebraska" });
            states.Add(new State() { Abbreviations = "NV", Name = "Nevada" });
            states.Add(new State() { Abbreviations = "NH", Name = "New Hampshire" });
            states.Add(new State() { Abbreviations = "NJ", Name = "New Jersey" });
            states.Add(new State() { Abbreviations = "NM", Name = "New Mexico" });
            states.Add(new State() { Abbreviations = "NY", Name = "New York" });
            states.Add(new State() { Abbreviations = "NC", Name = "North Carolina" });
            states.Add(new State() { Abbreviations = "ND", Name = "North Dakota" });
            states.Add(new State() { Abbreviations = "OH", Name = "Ohio" });
            states.Add(new State() { Abbreviations = "OK", Name = "Oklahoma" });
            states.Add(new State() { Abbreviations = "OR", Name = "Oregon" });
            states.Add(new State() { Abbreviations = "PA", Name = "Pennsylvania" });
            states.Add(new State() { Abbreviations = "RI", Name = "Rhode Island" });
            states.Add(new State() { Abbreviations = "SC", Name = "South Carolina" });
            states.Add(new State() { Abbreviations = "SD", Name = "South Dakota" });
            states.Add(new State() { Abbreviations = "TN", Name = "Tennessee" });
            states.Add(new State() { Abbreviations = "TX", Name = "Texas" });
            states.Add(new State() { Abbreviations = "UT", Name = "Utah" });
            states.Add(new State() { Abbreviations = "VT", Name = "Vermont" });
            states.Add(new State() { Abbreviations = "VA", Name = "Virginia" });
            states.Add(new State() { Abbreviations = "WA", Name = "Washington" });
            states.Add(new State() { Abbreviations = "WV", Name = "West Virginia" });
            states.Add(new State() { Abbreviations = "WI", Name = "Wisconsin" });
            states.Add(new State() { Abbreviations = "WY", Name = "Wyoming" });
            return states.ToList();
        }

        public static IList<Bit> CreateBitList()
        {
            List<Bit> status = new List<Bit>();
            status.Add(new Bit() { Name = "No", Condition = false });
            status.Add(new Bit() { Name = "Yes", Condition = true });
            return status.ToList();
        }
    }
}
