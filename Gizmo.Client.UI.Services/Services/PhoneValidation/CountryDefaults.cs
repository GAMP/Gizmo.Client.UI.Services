using System;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Services;

public static class CountryDefaults
{
    public static PhoneCountry? ResolveDefault(string? regionCode, IReadOnlyList<PhoneCountry> countries)
    {
        if (string.IsNullOrEmpty(regionCode) || countries.Count == 0)
            return null;

        return countries.FirstOrDefault(c =>
            string.Equals(c.RegionCode, regionCode, StringComparison.OrdinalIgnoreCase));
    }
}
