using TMPro;
using UnityEngine;

namespace WIT.Utilities
{
    public class HamHelpers
    {
        public static Vector3 MapValueVector(float a0, float a1, float b0, float b1, float a)
        {
            float v = b0 + (b1 - b0) * ((a - a0) / (a1 - a0));
            return new Vector3(v, v, v);
        }
	}
}