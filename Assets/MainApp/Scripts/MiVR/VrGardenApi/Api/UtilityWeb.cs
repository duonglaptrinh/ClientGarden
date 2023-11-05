using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VrGardenApi
{
    public class UtilityWeb
    {
        public static Dictionary<string, string> ParseQueryStringEmpty()
        {
            var query = new Dictionary<string, string>();// System.Web.ParseQueryString(string.Empty);
            //query.Add("skip", "");
            //query.Add("take", "");
            //query.Add("orderBy", "");
            return query;
        }
    }
}
