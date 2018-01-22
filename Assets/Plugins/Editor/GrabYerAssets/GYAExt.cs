#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
#define UNITY_3
#endif

#if (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7)
#define UNITY_4
#endif

#if UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define UNITY_4_2_OR_NEWER
#endif

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

#define EnableZiosEditorThemeTweaks

// Unity 5.3.4 and newer, auto assigns: UNITY_x_y_OR_NEWER

using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
//using System.Text;
using System.Reflection;
using GYAInternal.Json;
using GYAInternal.Json.Linq;

namespace XeirGYA
{
    // GYA Extensions
    public static class GYAExt
    {
        //public static void GYAExt() {}

        internal static ActiveOS activeOS = ActiveOS.Unknown;
        internal enum ActiveOS
        {
            Unknown,
            Windows,
            Mac,
            Linux
        }

        // To load the assembly and type:
        //Assembly fileLoad = Assembly.LoadFile(@"<path>\System.Design.dll");
        //Type myType = fileLoad.GetType("System.Collections.Generic");

        // Invoke
        internal static BindingFlags _flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                                              BindingFlags.Static;

        public static object Invoke(object p_target, string p_method, params object[] p_args)
        {
            //Example
            //Invoke(gameObject,"SetActiveRecursively",true)

            Type t = p_target.GetType();
            MethodInfo mi = t.GetMethod(p_method, _flags);
            return mi == null ? null : mi.Invoke(p_target, p_args);
        }

        //public static void Awake()
        //{
        //    AssignIsOS();
        //}

        // Stopwatch Use:
        static System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

        public static void StopwatchStart()
        {
            stopWatch.Start();
        }

        public static void StopwatchStop()
        {
            stopWatch.Stop();
        }

        public static string StopwatchElapsed(bool consoleOutput = true, bool cumulative = false)
        {
            if (!cumulative)
                stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00} ({4:00}ms)", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10, ts.TotalMilliseconds);
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            if (!cumulative)
                stopWatch.Reset();
            if (consoleOutput)
                Log("Elapsed Time: " + elapsedTime);
            //elapsedTime = GYA.Instance.gyaAbbr + " - Elapsed Time: " + elapsedTime + "\n";
            elapsedTime = "Elapsed Time: " + elapsedTime;

            return elapsedTime;
        }

        // item[i].setSize(9);
        public static string setSize(String _s, int _num)
        {
            var temp_s = _s;
            if (_s.Length >= _num)
                return _s;
            for (int i = 0; i < _num - _s.Length; i++)
                temp_s += " ";
            return temp_s;
        }

        // NEEDS TESTING
        // Return a contrasting text color for a given background color
        //public static string ContrastingTextForBackground(string bgColor)
        //{
        //	var rgbval = "8A23C0"; //hex
        //	var r = rgbval >> 16;
        //	var g = (rgbval & 65280) >> 8;
        //	var b = rgbval & 255;
        //	var brightness = r*0.299 + g*0.587 + b*0.114;
        //	return (brightness > 160) ? "#000" : "#fff";
        //}

        // Return False if 0 , True if NOT 0
        public static bool ToBool(this int x)
        {
            //return (x != 0 ? true : false);
            return Convert.ToBoolean(x);
        }

        // Return True if 1
        public static bool IsTrue(this int x)
        {
            //return (x == 1 ? true : false);

            //bool y = Convert.ToBoolean(x);
            return (Convert.ToBoolean(x));
        }

        // Return True if 0
        public static bool IsFalse(this int x)
        {
            //return (x == 0 ? true : false);

            //bool y = Convert.ToBoolean(x);
            return (!Convert.ToBoolean(x));
        }

        // Decimal to Hex
        public static string DecToHex(int decValue)
        {
            return string.Format("{0:x}", decValue);
        }

        // Hex to Decimal
        public static long HexToDec(string hexValue)
        {
            return Convert.ToInt64(hexValue, 16);
        }

        //byte[] data = FromHex("47-61-74-65-77-61-79-53-65-72-76-65-72");
        //string s = Encoding.ASCII.GetString(data);
        public static byte[] FromHex(string hexValue)
        {
            hexValue = hexValue.Replace("-", "");
            byte[] raw = new byte[hexValue.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hexValue.Substring(i * 2, 2), 16);
            }
            return raw;
        }

        public static string GetLine(string text, int lineNo)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : "";
        }

        // Get string Between first/last of other strings
        public static string Between(this string value, string a, string b)
        {
            int posA = value.IndexOf(a, StringComparison.InvariantCultureIgnoreCase);
            int posB = value.LastIndexOf(b, StringComparison.InvariantCultureIgnoreCase);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        public static string GetUntilOrEmpty(this string text, string stopAt = "-")
        {
            //if (!String.IsNullOrWhiteSpace(text))
            if (!String.IsNullOrEmpty(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }
            return String.Empty;
        }

        // Get first string Before another string
        public static string Before(this string value, string a)
        {
            int posA = value.IndexOf(a, StringComparison.InvariantCultureIgnoreCase);
            if (posA == -1)
            {
                return "";
            }
            return value.Substring(0, posA);
        }

        // Get last string After another string
        public static string After(this string value, string a)
        {
            int posA = value.LastIndexOf(a, StringComparison.InvariantCultureIgnoreCase);
            if (posA == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }


        // Return Digits or Alpha only


        public static string DigitsOnly(string pString)
        {
            System.Text.RegularExpressions.Regex text = new System.Text.RegularExpressions.Regex(@"[^\d]");
            return text.Replace(pString, "");
        }

        public static string AlphaOnly(string pString)
        {
            System.Text.RegularExpressions.Regex text = new System.Text.RegularExpressions.Regex(@"[^a-zA-Z]");
            return text.Replace(pString, "");
        }

        // Check if an Int is null
        public static bool IsIntNull(int? pInt)
        {
            return (pInt == null);
        }

        public static bool IsInt(object pVal)
        {
            int n;
            bool isNumeric = int.TryParse(pVal.ToString(), out n);
            return isNumeric;
        }

        public static int IntOrZero(object pVal)
        {
            return IsInt(pVal) ? Convert.ToInt32(pVal.ToString()) : 0;
        }

        // Forced format for asset dates
        // ISO 8601 = YYYY-MM-DDThh:mm:ssTZD (eg 1997-07-16T19:20:30+01:00)
        // o = "2015-09-10T03:10:57-05:00" or "2015-10-12T09:34:51Z"
        // s = "2015-09-10T08:10:57"
        //public static string DateAsISO8601 (string pDateTime)
        //{
        //	DateTimeOffset tDate = DateTimeOffset.Parse(pDateTime);
        //	return DateAsISO8601(tDate);
        //}

        public static string DateAsISO8601(string pDateTime)
        {
            DateTime utcDate = DateTime.SpecifyKind(Convert.ToDateTime(pDateTime), DateTimeKind.Utc);
            return DateAsISO8601(utcDate);
        }

        public static string DateAsISO8601(DateTime pDateTime)
            //public static DateTimeOffset DateAsISO8601 (DateTime pDateTime)
        {
            CultureInfo dateCulture = CultureInfo.InvariantCulture;
            DateTimeStyles dateStyle = DateTimeStyles.AssumeUniversal;

            DateTimeOffset utcDate = DateTimeOffset.Parse(pDateTime.ToString(CultureInfo.InvariantCulture), dateCulture, dateStyle);
            return DateAsISO8601(utcDate);
        }

        public static string DateAsISO8601(DateTimeOffset pDateTime)
            //public static DateTimeOffset DateAsISO8601 (DateTimeOffset pDateTime)
        {
            CultureInfo dateCulture = CultureInfo.InvariantCulture;

            // 2013-12-11T06:23:32Z
            return pDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", dateCulture);
        }

        public static DateTimeOffset DateStringAsDTO(string pDateTime)
        {
            CultureInfo dateCulture = CultureInfo.InvariantCulture;
            DateTimeStyles dateStyle = DateTimeStyles.AssumeUniversal;

            DateTime utcDate = DateTime.SpecifyKind(Convert.ToDateTime(pDateTime), DateTimeKind.Utc);
            DateTimeOffset utcDate2 = DateTimeOffset.Parse(utcDate.ToString(CultureInfo.InvariantCulture), dateCulture, dateStyle);

            return utcDate2.ToUniversalTime();
        }

        // Check for valid date, return MinValue if not valid
        public static string ValidOrMinDate(DateTimeOffset pDateTime)
        {
            return ValidOrMinDate(pDateTime.ToString());
        }

        public static string ValidOrMinDate(string pDateTime)
        {
            DateTime tmpDate;
            if (!DateTime.TryParse(pDateTime, out tmpDate))
            {
                tmpDate = DateTime.MinValue;
                //tmpDate = new DateTimeOffset(fileData.CreationTimeUtc);
            }
            return tmpDate.ToString(CultureInfo.InvariantCulture);
        }


        public static Type[] GetAllDerivedTypes(this AppDomain aAppDomain, Type aType)
        {
            var assemblies = aAppDomain.GetAssemblies();
            return (from assembly in assemblies from type in assembly.GetTypes() where type.IsSubclassOf(aType) select type).ToArray();
        }

        public static Rect GetEditorMainWindowPos()
        {
            var containerWinType = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject))
                .FirstOrDefault(t => t.Name == "ContainerWindow");
            if (containerWinType == null)
                throw new MissingMemberException(
                    "Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
            var showModeField = containerWinType.GetField("m_ShowMode",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var positionProperty = containerWinType.GetProperty("position",
                BindingFlags.Public | BindingFlags.Instance);
            if (showModeField == null || positionProperty == null)
                throw new MissingFieldException(
                    "Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
            var windows = Resources.FindObjectsOfTypeAll(containerWinType);
            foreach (var win in windows)
            {
                var showmode = (int) showModeField.GetValue(win);
                if (showmode == 4) // main window
                {
                    var pos = (Rect) positionProperty.GetValue(win, null);
                    return pos;
                }
            }
            throw new NotSupportedException(
                "Can't find internal main window. Maybe something has changed inside Unity");
        }

        public static void CenterOnMainWin(this UnityEditor.EditorWindow aWin)
        {
            var main = GetEditorMainWindowPos();
            var pos = aWin.position;
            float w = (main.width - pos.width) * 0.5f;
            float h = (main.height - pos.height) * 0.5f;
            pos.x = main.x + w;
            pos.y = main.y + h;
            aWin.position = pos;
        }

        // --

        //public static string NullToEmpty (string pVar)
        //{
        //	return pVar || String.Empty;
        //}

        // Print properties of an object to the console as json
        public static void LogAsJson(object obj, bool header = true)
        {
            try
            {
                String dText;
                if (obj == null)
                {
                    dText = header ? "Properties of -- NULL\n" : "";
                }
                else
                {
                    dText = header ? string.Format("Properties of -- {0}\n", obj.GetType()) : "";
                    dText += JsonConvert.SerializeObject(obj, Formatting.Indented);
                }
                Debug.Log(dText);
            }
            catch (Exception ex)
            {
                LogWarning("LogAsJson: " + ex.Message);
                //return null;
            }
        }

        public static string ToJson(object obj, bool formatted = false)
        {
            try
            {
                if (obj == null)
                    //return "NULL";
                    return null;

                return JsonConvert.SerializeObject(obj, (formatted ? Formatting.Indented : Formatting.None));
            }
            catch (Exception ex)
            {
                LogWarning("ToJson: " + ex.Message);
                return null;
            }
        }

        //// Can't use dynamic, find alternative
        //internal static string FormatJson(string json)
        //{
        //	dynamic parsedJson = JsonConvert.DeserializeObject(json);
        //	return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        //}

        // string actual = JToken.Parse(original).RemoveFields(new string[]{"__metadata", "remove"}).ToString(Newtonsoft.Json.Formatting.None);
        public static JToken RemoveFields(this JToken token, string[] fields)
        {
            JContainer container = token as JContainer;
            if (container == null)
                return token;

            List<JToken> removeList = new List<JToken>();
            foreach (JToken el in container.Children())
            {
                JProperty p = el as JProperty;
                if (p != null && fields.Contains(p.Name))
                    removeList.Add(el);

                el.RemoveFields(fields);
            }

            foreach (JToken el in removeList)
                el.Remove();

            return token;
        }

        //internal static void LogDebugPanel (string title, object value, string category = null)
        //{
        //	DebugPanel.Log(string title, string category, object value);
        //}

        // Simple Formatted Log for GYA
        internal static void Log(string pString, string pString2 = null, bool indent = true)
        {
            if (pString2 == null)
                Debug.Log(GYA.gyaVars.abbr + " - " + pString + "\n");
            else
                Debug.Log(GYA.gyaVars.abbr + " - " + pString + (indent ? NewLineIndent() : "\n\n") +
                                      pString2);
        }

        internal static void LogWarning(string pString, string pString2 = null, bool indent = true)
        {
            if (pString2 == null)
                Debug.LogWarning(GYA.gyaVars.abbr + " - " + pString + "\n");
            else
                Debug.LogWarning(GYA.gyaVars.abbr + " - " + pString + (indent ? NewLineIndent() : "\n\n") +
                                             pString2);
        }

        internal static void LogError(string pString, string pString2 = null, bool indent = true)
        {
            if (pString2 == null)
                Debug.LogError(GYA.gyaVars.abbr + " - " + pString + "\n");
            else
                Debug.LogError(GYA.gyaVars.abbr + " - " + pString + (indent ? NewLineIndent() : "\n\n") +
                                           pString2);
        }

        internal static string NewLineIndent()
        {
            return "\n" + Indent(11);
        }

        public static string Indent(int count)
        {
            return "".PadLeft(count);
        }

        public static object TryParseJSON(string jsonString)
        {
            try
            {
                var o = JObject.Parse(jsonString);
                // Handle non-exception-throwing cases:
                // Neither JSON.parse(false) or JSON.parse(1234) throw errors, hence the type-checking,
                // but... JSON.parse(null) returns 'null', and typeof null === "object",
                // so we must check for that, too.
                //if (o && typeof(o) === "object" && o !== null) {
                if (o != null)
                {
                    return o;
                }
            }
            catch (Exception) {}
            return false;
        }

        // Return path modified for platform
        public static string PathFixedForOS(string source)
        {
            char directorySeparatorDefault = '/'; // OS X, iOS, *nix, Android, etc
            char directorySeparatorWindows = '\\'; // Windows
            //source = source.Replace("\"", String.Empty);
            source = IsOSWin ? source.Replace(directorySeparatorDefault, directorySeparatorWindows) : source.Replace(directorySeparatorWindows, directorySeparatorDefault);
            return source;
        }

        public static string NullToEmpty(string pString)
        {
            return string.IsNullOrEmpty(pString) ? String.Empty : pString;
        }

        // Set Rect values, avoid CS1612: Cannot modify a value type return value
        public static Rect SetRect(this Rect sourceRect, float? pX = null, float? pY = null, float? pWidth = null,
            float? pHeight = null)
        {
            sourceRect.Set(
                pX ?? sourceRect.x,
                pY ?? sourceRect.y,
                pWidth ?? sourceRect.width,
                pHeight ?? sourceRect.height
            );
            return sourceRect;
        }
        //public static Rect SetRect(this Rect sourceRect, float? pX = null, float? pY = null, float? pWidth = null, float? pHeight = null)
        //{
        //	var targetRect = sourceRect;
        //	targetRect.Set(
        //		(pX == null ? sourceRect.x : (float)pX),
        //		(pY == null ? sourceRect.y : (float)pY),
        //		(pWidth == null ? sourceRect.width : (float)pWidth),
        //		(pHeight == null ? sourceRect.height : (float)pHeight)
        //	);

        //	sourceRect = targetRect;
        //	return sourceRect;
        //}

        // Use: bool contains = string2.Contains("string1", StringComparison.OrdinalIgnoreCase);
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            if (source == null || toCheck == null)
                return false;
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool In<T>(this T source, params T[] list)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return list.Contains(source);
        }

        // Prev for enum
        public static T Prev<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[]) Enum.GetValues(src.GetType());
            int cnt = Array.IndexOf<T>(Arr, src) - 1;
            return (cnt == -1) ? Arr[Arr.Length - 1] : Arr[cnt];
        }

        // Next for enum
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[]) Enum.GetValues(src.GetType());
            int cnt = Array.IndexOf<T>(Arr, src) + 1;
            return (cnt == Arr.Length) ? Arr[0] : Arr[cnt];
        }

        // Return the count of an enumerable
        public static int CountEnum(IEnumerable enumerable)
        {
            return (from object item in enumerable select item).Count();
        }

        // For Casting to Enum
        public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }

        // Open Asset Store page for specific asset
        public static void OpenAssetURL(int assetID, string urlOverride = null)
        {
            OpenAssetURL(assetID, false, urlOverride);
        }

        // Open Asset Store page for specific asset
        public static void OpenAssetURL(int pID, bool openURLInUnity = false, string urlOverride = null)
        {
            //EditorApplication.ExecuteMenuItem( "Window/Asset Store" );
            string openURLSite = "https://www.assetstore.unity3d.com/#/";
            string openURL = string.Empty;

            if (urlOverride != null)
            {
                openURLSite = urlOverride;
                openURL = pID.ToString();
            }
            else
            {
                //string openURL = pkgDetails.link.type + "/" + pkgDetails.link.id.ToString();
                openURL = "content/" + pID;
            }

            if (openURLInUnity)
            {
                openURL = "com.unity3d.kharma:" + openURL;

                // Open in Unity's Asset Store Window
                //UnityEditorInternal.AssetStore.Open (string.Format ("content/{0}?assetID={1}", activeAsset.packageID, activeAsset.id));
                //UnityEditorInternal.AssetStore.Open (string.Format ("content/{0}", pkgDetails.link.id.ToString()));
                UnityEditorInternal.AssetStore.Open(string.Format(openURL));
            }
            else
            {
                // Open in browser
                openURL = openURLSite + openURL;

                if (IsOSWin)
                {
                    System.Diagnostics.Process.Start(openURL);
                }
                if (IsOSMac)
                {
                    System.Diagnostics.Process.Start("open", openURL);
                }
                if (IsOSLinux)
                {
                    //System.Diagnostics.Process.Start ("open", openURL);
                    System.Diagnostics.Process.Start(openURL);
                }
            }
        }

        // Open in window passed folder name, optionally strip the filename
        public static void ShellOpenFolder(string @folder, bool stripName = false)
        {
            folder = PathFixedForOS(folder);
            if (stripName)
                folder = Path.GetDirectoryName(folder);

            if (folder != null && Directory.Exists(folder))
            {
                if (IsOSWin)
                {
                    folder = "\"" + folder + "\"";
                    System.Diagnostics.Process.Start(@folder);
                    //System.Diagnostics.Process.Start("explorer.exe", "/select," + folder);
                }
                if (IsOSMac)
                {
                    folder = "\"" + folder + "\"";
                    System.Diagnostics.Process.Start("open", @folder);
                }
                if (IsOSLinux)
                {
                    folder = folder + "/.";
                    EditorUtility.RevealInFinder(@folder);
                }
            }
        }

        // Running Pro version?
        public static bool IsPro
        {
            get { return UnityEditorInternal.InternalEditorUtility.HasPro(); }
        }

        // Using Pro skin?
        public static bool IsProSkin
        {
            get
            {
                //return true; // DARKSKIN - enable for testing

#if EnableZiosEditorThemeTweaks
                return (EditorGUIUtility.isProSkin || IsZiosEditorThemeDark);
#else
		        return (EditorGUIUtility.isProSkin);
#endif
            }
        }

        // Using Zios Dark Theme? .. ZiosEditorThemeIsDark
        public static bool IsZiosEditorThemeDark
        {
            get { return (EditorPrefs.GetBool("EditorTheme-Dark")); }
        }

        public static void AssignIsOS()
        {
            activeOS = ActiveOS.Unknown;
            if (SystemInfo.operatingSystem.IndexOf("Windows", StringComparison.InvariantCultureIgnoreCase) != -1)
                activeOS = ActiveOS.Windows;
            if (SystemInfo.operatingSystem.IndexOf("Mac OS", StringComparison.InvariantCultureIgnoreCase) != -1)
                activeOS = ActiveOS.Mac;
            if (SystemInfo.operatingSystem.IndexOf("Linux", StringComparison.InvariantCultureIgnoreCase) != -1)
                activeOS = ActiveOS.Linux;
        }

        // Is current OS Mac
        public static bool IsOSMac
        {
            get
            {
                //return SystemInfo.operatingSystem.IndexOf("Mac OS", StringComparison.InvariantCultureIgnoreCase) != -1;
                if (activeOS == ActiveOS.Unknown)
                    AssignIsOS();
                return activeOS == ActiveOS.Mac;
            }
        }

        // Is current OS Windows
        public static bool IsOSWin
        {
            get
            {
                //return SystemInfo.operatingSystem.IndexOf("Windows", StringComparison.InvariantCultureIgnoreCase) != -1;
                if (activeOS == ActiveOS.Unknown)
                    AssignIsOS(); 
                return activeOS == ActiveOS.Windows;
            }
        }

        // Is current OS Linux
        public static bool IsOSLinux
        {
            get
            {
                //return SystemInfo.operatingSystem.IndexOf("Linux", StringComparison.InvariantCultureIgnoreCase) != -1;
                if (activeOS == ActiveOS.Unknown)
                    AssignIsOS(); 
                return activeOS == ActiveOS.Linux;
            }
        }

        // Is mouse over gui component & asset window
        public static bool IsMouseOver(Rect item)
        {
            return Event.current.type == EventType.Repaint && item.Contains(Event.current.mousePosition);
        }

        // Return the Folder: Unity App
        public static string PathUnityApp
        {
            get { return Path.GetDirectoryName(EditorApplication.applicationPath); }
        }

        // Return the Folder: Unity Project Assets
        public static string PathUnityProjectAssets
        {
            get { return Path.GetFullPath(Path.Combine(PathUnityProject, "Assets")); }
        }

        // Return the Folder: Unity Project
        public static string PathUnityProject
        {
            get { return Path.GetDirectoryName(Application.dataPath); }
        }

        // Return the Unity Folder: Standard Assets with Path
        public static string PathUnityStandardAssets
        {
            //get { return Path.GetFullPath( Path.Combine (Path.GetDirectoryName (EditorApplication.applicationPath), FolderUnityStandardAssets) ); }
            get { return Path.GetFullPath(Path.Combine(PathUnityApp, FolderUnityStandardAssets)); }
        }

        // Return the Unity Folder: Standard Assets without Path
        public static string FolderUnityStandardAssets
        {
            get
            {
                // Unity 5 Asset Folder
                string standardAssetsPath = "Standard Assets";
                //#if UNITY_4_2_OR_NEWER // Pre Unity 5 Standard Assets folder
#if UNITY_3 || UNITY_4 // Pre Unity 5 Standard Assets folder
				standardAssetsPath = "Standard Packages";
#endif
                return standardAssetsPath;
            }
        }

        public static string FileInGYADataFiles(string pFile)
        {
            return Path.Combine(PathGYADataFiles, pFile);
        }

        public static string PathGYADataFiles
        {
            get
            {
                const string dataPath = "Grab Yer Assets";
                return Path.Combine(PathUnityDataFiles, dataPath);
            }
        }

        // Return the Unity Folder: Asset Store
        public static string FolderUnityAssetStore
        {
            get { return "Asset Store"; }
        }

        //// Return the Unity 3/4 Folder: Asset Store
        //public static string FolderUnityAssetStore4
        //{
        //    get { return "Asset Store"; }
        //}

        //// Return the Unity 5 Folder: Asset Store
        //public static string FolderUnityAssetStore5
        //{
        //    get { return "Asset Store-5.x"; }
        //}

        //// Return the Unity 2017 Folder: Asset Store
        //public static string FolderUnityAssetStore2017
        //{
        //    get { return "Asset Store-2017.x"; }
        //}

        //// Find the Inactive AS Folder
        //public static string FolderUnityAssetStoreInActive
        //{
        //    get
        //    {
        //        if (FolderUnityAssetStoreActive == FolderUnityAssetStore5)
        //        {
        //            return FolderUnityAssetStore;
        //        }
        //        return FolderUnityAssetStore5;
        //    }
        //}

        // Return the correct Unity Folder: Asset Store for the actively running version
        public static string FolderUnityAssetStoreActive
        {
            get
            {
                // System specific asset folder
                string folderPath = FolderUnityAssetStore; // Default for Unity 3/4 AS Folder

                if (!(GYAVersion.GetUnityVersionMajor == 3 || GYAVersion.GetUnityVersionMajor == 4))
                {
                    folderPath += "-5.x";

                    // This is here IF Unity switches to "Asset Store-2017.x"
                    // Example: Unity 5 = "Asset Store-5.x", Unity 2017 = "Asset Store-2017.x"
                    //if (GYAVersion.UnityVersionIsEqualOrNewerThan ("2017.1.0f0", 4)) // Still uses "Asset Store-5.x" folder
                    //folderPath += "-" + GYAVersion.GetUnityVersionMajor + ".x";
                }
//#if UNITY_5
//                folderPath = FolderUnityAssetStore5;
//#elif UNITY_2017
//                folderPath = FolderUnityAssetStore2017;
//#endif
                return folderPath;
            }
        }

        // Return the correct Path of the Unity Folder: Asset Store for the actively running version
        public static string PathUnityAssetStoreActive
        {
            get {
                return Path.Combine(PathUnityDataFiles, FolderUnityAssetStoreActive);
            }


            //            get
            //            {
            //                // System specific asset path, default to Unity 3/4 AS Folder
            //                string folderPath = PathUnityAssetStore4;
            //#if UNITY_5
            //                folderPath = PathUnityAssetStore5;
            //#elif UNITY_2017
            //                folderPath = PathUnityAssetStore2017;
            //#endif
            //    return folderPath;
            //}
        }

        // Return the Unity Folder: Asset Store Parent Folder
        public static string PathUnityCookiesFile
        {
            get
            {
                string cookiePath = string.Empty;
                if (IsOSWin)
                {
                    // PathUnityDataFiles will not work as Cookies is in the LocalLow folder
                    //C:\Users\{USER}\AppData\LocalLow\Unity\Browser\Cookies\Cookies
                    cookiePath =
                        PathFixedForOS(Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                                .Replace("AppData\\Roaming", "AppData"), "LocalLow/Unity/Browser/Cookies/Cookies"));

                    //Alt options:
                    //cookiePath = PathFixedForOS(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"..\LocalLow\Unity\Browser\Cookies\Cookies"));

                    //cookiePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                    //Path.Combine(cookiePath, @"..\LocalLow")
                }

                if (IsOSMac)
                {
                    cookiePath = Path.Combine(GYAExt.PathUnityDataFiles, "Browser/Cookies/Cookies");
                }

                if (IsOSLinux)
                {
                    // /home/{USER}/.config/unity3d/Unity/Browser/Cookies/Cookies
                    cookiePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "unity3d/Unity/Browser/Cookies/Cookies");
                }
                //Debug.Log(cookiePath);
                return cookiePath;
            }
        }

        // Return the Unity Folder: Asset Store Parent Folder
        public static string PathUnityDataFiles
        {
            get
            {
                // System specific asset path

                // Windows:	%AppData%\Unity
                // Mac:	~/Library/Unity
                // Linux:	~/.local/share/unity3d

                if (IsOSWin)
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Unity");
                }
                if (IsOSMac)
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Unity");
                }
                if (IsOSLinux)
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "unity3d");
                }
                return "";
            }
        }

        // Return the Unity Folder: Asset Store
        public static string PathUnityAssetStore
        {
            get { return Path.Combine(PathUnityDataFiles, FolderUnityAssetStore); }
        }

        //// Return the Unity 3/4 Folder: Asset Store
        //public static string PathUnityAssetStore4
        //{
        //    get { return Path.Combine(PathUnityDataFiles, FolderUnityAssetStore4); }
        //}

        //// Return the Unity 5 Folder: Asset Store "Asset Store-5.x"
        //public static string PathUnityAssetStore5
        //{
        //    get { return Path.Combine(PathUnityDataFiles, FolderUnityAssetStore5); }
        //}

        //// Return the Unity 2017 Folder: Asset Store "Asset Store-5.x"
        //public static string PathUnityAssetStore2017
        //{
        //    get { return Path.Combine(PathUnityDataFiles, FolderUnityAssetStore2017); }
        //}

        // Convert bytes to KB/MB/GB
        public static string BytesToKB(this int fileSizeBytes)
        {
            //return fileSizeBytes.BytesToKB();
            double fs = fileSizeBytes;
            return fs.BytesToKB();
        }

        public static string BytesToKB(this float fileSizeBytes)
        {
            double fs = fileSizeBytes;
            return fs.BytesToKB();
        }

        //public static string BytesToKB (double fileSizeBytes)
        public static string BytesToKB(this double fileSizeBytes)
        {
            // Get filesize of asset
            string[] sizes = {"KB", "MB", "GB"};
            int order = 0;
            fileSizeBytes = fileSizeBytes / 1024;

            while (fileSizeBytes >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                fileSizeBytes = fileSizeBytes / 1024;
            }
            // Format fileSize string
            return String.Format("{0:0.00} {1}", fileSizeBytes, sizes[order]);
        }

        internal static string intToSizeString(int inValue)
        {
            if (inValue < 0)
            {
                return "unknown";
            }
            float num = inValue;
            var array = new string[]
            {
                "TB",
                "GB",
                "MB",
                "KB",
                "Bytes"
            };
            int num2 = array.Length - 1;
            while (num > 1000 && num2 >= 0)
            {
                num /= 1000;
                num2--;
            }
            return num2 < 0 ? "<error>" : string.Format("{0:#.##} {1}", num, array[num2]);
        }

        // Return the byte range for the size header
        public static string GetByteRangeHeader(double pkgSize)
        {
            string headerText = string.Empty;
            int kb = 1024;
            pkgSize = pkgSize / 1024;

            if (pkgSize > kb * 1000)
                headerText = "1 GB+";
            if (pkgSize > kb * 500 && pkgSize < kb * 1000)
                headerText = "500 MB - < 1 GB";
            if (pkgSize > kb * 250 && pkgSize < kb * 500)
                headerText = "250 MB - < 500 MB";
            if (pkgSize > kb * 100 && pkgSize < kb * 250)
                headerText = "100 MB - < 250 MB";
            if (pkgSize > kb * 50 && pkgSize < kb * 100)
                headerText = "50 MB - < 100 MB";
            if (pkgSize > kb * 10 && pkgSize < kb * 50)
                headerText = "10 MB - < 50 MB";
            if (pkgSize > kb * 1 && pkgSize < kb * 10)
                headerText = "1 MB - < 10 MB";
            if (pkgSize < kb * 1)
                headerText = "< 1 MB";

            return headerText;
        }

        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        // Create Prefab and export as unityPackage
        public static void exportGOAsPrefabPackage(GameObject go, string prefabName)
        {
            var prefabPath = "Assets/" + prefabName + ".prefab";
            var exportPath = prefabName + ".unitypackage";
            GameObject prefab = PrefabUtility.CreatePrefab(prefabPath, go);
            // Prevents: warning CS0219: The variable `prefab' is assigned but its value is never used
            //if (prefab.isStatic) {}
            prefab.hideFlags = HideFlags.None;
            AssetDatabase.SaveAssets();
            AssetDatabase.ExportPackage(prefabPath, exportPath, ExportPackageOptions.IncludeDependencies);
        }
    }
}