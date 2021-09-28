using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.UI.Selection;

namespace WIT.Patches
{
    class CheckEventSystem
    {
        [HarmonyLib.HarmonyPatch(typeof(KingmakerInputModule), "CheckEventSystem")]
        internal static class KinmagerInputModule_CheckEventSystem_Patch
        {
            private static bool Prefix()
            {
                return (false);
            }
        }
    }
}
