using UnityEngine;

namespace BOYAREngine.Utils
{
    public class Utils : MonoBehaviour
    {
        /// <summary> Converts given bitmask to layer number </summary>
        /// <returns> layer number </returns>
        public static int ToLayer(int bitmask)
        {
            var result = bitmask > 0 ? 0 : 31;
            while (bitmask > 1)
            {
                bitmask >>= 1;
                result++;
            }
            return result;
        }
    }
}

