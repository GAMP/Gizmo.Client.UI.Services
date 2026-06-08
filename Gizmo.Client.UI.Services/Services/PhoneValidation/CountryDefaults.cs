using System;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Services;

public static class CountryDefaults
{
    public static PhoneCountry? ResolveDefault(string? regionCode, IReadOnlyList<PhoneCountry> countries)
    {
        if (countries.Count == 0)
            return null;

        if (!string.IsNullOrEmpty(regionCode))
        {
            var match = countries.FirstOrDefault(c =>
                string.Equals(c.RegionCode, regionCode, StringComparison.OrdinalIgnoreCase));
            if (match is not null)
                return match;
        }

        return countries.OrderBy(c => c.CountryName, StringComparer.OrdinalIgnoreCase).First();
    }
}
