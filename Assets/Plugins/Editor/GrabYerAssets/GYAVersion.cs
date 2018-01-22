#if UNITY_5 || UNITY_2017
#define UNITY_5_0_OR_NEWER
#endif

#if (UNITY_5_0_OR_NEWER && !UNITY_5_0)
#define UNITY_5_1_OR_NEWER
#endif

#if (UNITY_5_1_OR_NEWER && !UNITY_5_1)
#define UNITY_5_2_OR_NEWER
#endif

#if (UNITY_5_2_OR_NEWER && !UNITY_5_2)
#define UNITY_5_3_OR_NEWER
#endif

// Unity 5.3.4 and newer, auto assigns: UNITY_x_y_OR_NEWER

//using UnityEditor;
using UnityEngine;
using System;
//using System.Collections;
//using System.Text;
using System.Linq;

namespace XeirGYA
{
    public static class GYAVersion
    {
        // BEGIN - Version Methods

        // Use a 4 part version
        internal static Version appVer = new Version();

        //// Get Current Unity Version
        //internal static string currentUnityVersion;
        static GYAVersion()
        {
            //Debug.Log(
            //    "currentUnityVersion   =\t" + GYA.currentUnityVersion + "\n" +
            //    "UnityVersion          =\t" + Application.unityVersion + "\n" +
            //    "GetUnityVersion       =\t" + UnityEditorInternal.InternalEditorUtility.GetUnityVersion() + "\n" +
            //    "GYAUnityVersion       =\t" + GYAVersion.GetUnityVersion() + "\n" +
            //    "GetUnityVersionDigits =\t" + UnityEditorInternal.InternalEditorUtility.GetUnityVersionDigits() + "\n" +
            //    "GYAUnityVersionDigits =\t" + GYAVersion.GetUnityVersionDigits() + "\n" +
            //    "GetFullUnityVersion   =\t" + UnityEditorInternal.InternalEditorUtility.GetFullUnityVersion() + "\n" +
            //    "GYAUnityVersionDate   =\t" + GYAVersion.GetUnityVersionDateFormatted()
            //); 

            //LoadVars();
            //Debug.Log(currentUnityVersion); // null
        }

        //public static void init_currentUnityVersion()
        //{
        //    _currentUnityVersion = UnityEngine.Application.unityVersion; // i.e.- 5.4.0b13
        //    Debug.Log(currentUnityVersion); // null
        //}

        //private static string _currentUnityVersion;
        //public static string currentUnityVersion
        //{
        //    get
        //    {
        //        if (_currentUnityVersion == null)
        //            //currentUnityVersion = UnityEngine.Application.unityVersion;
        //            init_currentUnityVersion();
        //        return _currentUnityVersion;
        //    }
        //}

        // returns: "3.16.5.2905" as string
        public static string appVersion
        {
            get { return appVer.ToString(); }
        }

        // returns: "3.16.5.2905" as Version
        public static Version GetAppVersion
        {
            get { return appVer; }
        }

        public static string SetAppVersion(Int32 vMajor, Int32 vMinor, Int32 vBuild, Int32 vRevision)
        {
            return SetAppVersion(vMajor, vMinor, vBuild, vRevision, -1);
        }

        // Use Major/MinorRevision input such as SetAppVersion(3, 16, 5, 29, 5)
        // Output as "3.16.5.2905" instead of "3.16.5.29/5"
        public static string SetAppVersion(Int32 vMajor, Int32 vMinor, Int32 vBuild, Int32 vMajorRevision,
            Int32 vMinorRevision)
        {
            //Version ver = new Version(vMajor, vMinor, vBuild, (vMajorRevision << 16) + vMinorRevision);
            var vMajorMinorRev = Int32.Parse(vMajorRevision +
                                             (vMinorRevision >= 0 ? vMinorRevision.ToString("00") : ""));
            Version ver = new Version(vMajor, vMinor, vBuild, vMajorMinorRev);

            appVer = ver;
            return string.Format("{0}.{1}.{2}.{3:000}", ver.Major, ver.Minor, ver.Build, ver.Revision);
        }

        // BEGIN - Unity Version Methods

        // Return: true if == pVersion, false if != pVersion
        // pVersion format: "5.4.0b13", vDepth handling: "1.2.3444"
        public static bool UnityVersionIsEqualTo(String pVersion, int vDepth = 4)
        {
            int _bool = UnityVersionCompareTo(pVersion, vDepth);
            if (_bool == 0)
                return true;
            return false;
        }

        // Return: true if >= pVersion, false if < pVersion
        // pVersion format: "5.4.0b13", vDepth handling: "1.2.3444"
        public static bool UnityVersionIsEqualOrNewerThan(String pVersion, int vDepth = 4)
        {
            int _bool = UnityVersionCompareTo(pVersion, vDepth);
            if (_bool == -1)
                return false;
            return true;
        }

        // Return: true if <= pVersion, false if > pVersion
        // pVersion format: "5.4.0b13", vDepth handling: "1.2.3444"
        public static bool UnityVersionIsEqualOrOlderThan(String pVersion, int vDepth = 4)
        {
            int _bool = UnityVersionCompareTo(pVersion, vDepth);
            if (_bool == 1)
                return false;
            return true;
        }

        // Return: -1 if < pVersion, 0 if == pVersion, -1 if > pVersion
        // pVersion format: "5.4.0b13", vDepth handling: "1.2.3444"
        public static int UnityVersionCompareTo(String pVersion, int vDepth = 4)
        {
            //Debug.Log(pVersion);
            if (pVersion == null) // If v2Ver is null, then Unity version is greater
            {
                return 1;
            }

            // NOTE: get_unityVersion can only be called from the main thread.
            string v1Ver = UnityEngine.Application.unityVersion; // 5.4.0b13
            //Debug.Log(GYA.currentUnityVersion);
            //string v1Ver = GYA.currentUnityVersion;
            //string v1Ver = currentUnityVersion;

            //Version v1 = UnityEditorInternal.InternalEditorUtility.GetUnityVersion(); // 5.4.0
            Version v1 = GetUnityVersion(); // 5.4.0
            int v1BuildType = GetUnityVersionTypeWeight(v1Ver); // b (weighted = 2)
            int v1BuildRev = GetUnityVersionRevision(v1Ver); // 13

            Version v2 = new Version(GetUnityVersionBasic(pVersion));
            int v2BuildType = GetUnityVersionTypeWeight(pVersion);
            int v2BuildRev = GetUnityVersionRevision(pVersion);

            if (v1 == null) // If v1 is null, then Unity version exception
                throw new ArgumentNullException("pVersion");

            if (v2 == null) // If v2 is null, then Unity version is greater
                return 1;

            if (v1.Major != v2.Major && vDepth >= 1) // 5.4.0b13 = 5
                if (v1.Major > v2.Major)
                    return 1;
                else
                    return -1;

            if (v1.Minor != v2.Minor && vDepth >= 2) // 5.4.0b13 = 4
                if (v1.Minor > v2.Minor)
                    return 1;
                else
                    return -1;

            if (v1.Build != v2.Build && vDepth >= 3) // 5.4.0b13 = 0
                if (v1.Build > v2.Build)
                    return 1;
                else
                    return -1;

            if (v1BuildType != v2BuildType && vDepth >= 4) // 5.4.0b13 = b as digit 2
                if (v1BuildType > v2BuildType)
                    return 1;
                else
                    return -1;

            if (v1BuildRev != v2BuildRev && vDepth >= 4) // 5.4.0b13 = 13
                if (v1BuildRev > v2BuildRev)
                    return 1;
                else
                    return -1;

            return 0; // Unity version is equal
        }

        // Return version segments

        // 5.4.0b13 -> 5.4.0
        public static string GetUnityVersionBasic(string pString)
        {
            // OLD
            //if no versionType detected, return pString
            //if ((pString.Before(GetUnityVersionType(pString)) == String.Empty))
            //    return pString;

            //return pString.Before(GetUnityVersionType(pString));

            // NEW
            var pattern = @"^\D*(\d+\.\d+\.\d+)[a-zA-Z]*\d*";
            var matches = System.Text.RegularExpressions.Regex.Matches(pString, pattern);
            //Debug.Log(matches[0].Groups[1].Value);
            return matches[0].Groups[1].Value;
        }

        // 5.4.0b13 -> b .. Build Type : a == alpha, b == beta, rc == release candidate, f == final, p == patch
        //  or "Unity5.4.2f2-GVR13"
        public static string GetUnityVersionType(string pString)
        {
            // OLD
            //System.Text.RegularExpressions.Regex text = new System.Text.RegularExpressions.Regex(@"[^a-zA-Z]");
            //return text.Replace(pString, "");

            // NEW - Fix for "unity_version": "5.5.0xf3Linux" causing FormatException
            //return text.Replace(pString, "").Replace("Linux", "");
            var pattern = @"^\D*\d+\.\d+\.\d+([a-zA-Z]*)\d*";
            var matches = System.Text.RegularExpressions.Regex.Matches(pString, pattern);
            //Debug.Log(matches[0].Groups[1].Value);
            return matches[0].Groups[1].Value;
        }

        // 5.4.0b13 -> 2 .. Build Type : 1 == alpha, 2 == beta, 3 == release candidate, 4 == final, 5 == patch
        public static int GetUnityVersionTypeWeight(string pString)
        {
            string _text = GetUnityVersionType(pString);

            if (_text == "a") // alpha
                return 1;
            if (_text == "b") // beta
                return 2;
            if (_text == "rc") // release candidate
                return 3;
            if (_text == "xf") // ?? .. Linux - As in: 5.5.0xf3Linux .. not positive that 'xf' should reside here
                return 4;
            if (_text == "f") // final
                return 5;
            if (_text == "p") // patch
                return 6;

            return 0; // not detected
        }

        // 5.4.0b13 -> 13
        // Fixed [3.16.12.10] handling rev like "2-GVR13" from "Unity5.4.2f2-GVR13", returns 2
        public static int GetUnityVersionRevision(string pString)
        {
            //return int.Parse(pString.After(GetUnityVersionType(pString))); // OLD

            int tRev = 0;
            string tRevString = pString.After(GetUnityVersionType(pString));

            string digits = new string(tRevString.TakeWhile(Char.IsDigit).ToArray());
            Int32.TryParse(digits, out tRev);

            return tRev;
        }

        // Unity Version Major/Minor/Builds

        // Unity 4 version of UnityEditorInternal.InternalEditorUtility.GetUnityVersion()
        public static Version GetUnityVersion()
        {
#if UNITY_5_0_OR_NEWER
            Version version = UnityEditorInternal.InternalEditorUtility.GetUnityVersion();
#else
            Version version = new Version( GetUnityVersionBasic(UnityEngine.Application.unityVersion) );
#endif
            return version;
        }

        // Unity 4 version of UnityEditorInternal.InternalEditorUtility.GetUnityVersionDigits()
        // Return the Unity version digits, "5.4.0"
        public static string GetUnityVersionDigits()
        {
#if UNITY_5_0_OR_NEWER
            var version = UnityEditorInternal.InternalEditorUtility.GetUnityVersionDigits();
#else
            var version = GetUnityVersionBasic(UnityEngine.Application.unityVersion);
#endif
            return version;
        }

        //// Return the Unity version digits, "5.4.0"
        public static DateTime GetUnityVersionDateFormatted()
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dt.AddSeconds(GetUnityVersionDate);
        }

        // Return the Unity version full, "5.4.0b13"
        public static string GetUnityVersionWithTypeRevision
        {
            get { return UnityEngine.Application.unityVersion; }
        }

        // Return the Unity Major version, "5.4.0" Returns 5
        public static int GetUnityVersionMajor
        {
            get
            {
                //Version version = new Version(UnityEditorInternal.InternalEditorUtility.GetUnityVersionDigits());
                Version version = new Version(GetUnityVersionDigits());
                //Debug.Log( version.MajorRevision );	// -1
                return version.Major;
            }
        }

        // Return the Unity Minor version, "5.4.0" Returns 4
        public static int GetUnityVersionMinor
        {
            get
            {
                //Version version = new Version(UnityEditorInternal.InternalEditorUtility.GetUnityVersionDigits());
                Version version = new Version(GetUnityVersionDigits());
                //Debug.Log( version.MinorRevision );	// -1
                return version.Minor;
            }
        }

        // Return the Unity Build version, "5.4.0" Returns 0
        public static int GetUnityVersionBuild
        {
            get
            {
                //Version version = new Version(UnityEditorInternal.InternalEditorUtility.GetUnityVersionDigits());
                Version version = new Version(GetUnityVersionDigits());
                //Debug.Log( version.Revision );		// -1
                return version.Build;
            }
        }

        // Get Unity version date int, such as 1459313001
        public static int GetUnityVersionDate
        {
            get
            {
                return UnityEditorInternal.InternalEditorUtility.GetUnityVersionDate();
            }
        }

        // Return true/false, Is Unity version a beta build?
        public static bool IsUnityBeta
        {
            get
            {
                return UnityEditorInternal.InternalEditorUtility.IsUnityBeta();
            }
        }

        // END - Unity Version Methods
    }
}