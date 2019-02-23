using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Athanor.Collections
{
    public static class Vector3Ext
    {
        public static Vector3 Mean(this IEnumerable<Vector3> set)
        {
            return set.Aggregate((x, y) => x + y) / set.Count();
        }
    }
}
