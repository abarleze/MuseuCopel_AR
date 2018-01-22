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

//#define UseSQLite
#if UseSQLite
using GYAInternal.SQLite.Net;
#endif

// Unity 5.3.4 and newer, auto assigns: UNITY_x_y_OR_NEWER

//#define EnableVerCheck // Uncomment to enable version checking on data file
//#define TESTING

using UnityEditor;
using UnityEngine;
//using UnityEngine.Serialization;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;
using GYAInternal.Json;
//using GYAInternal.Json.Serialization;
using GYAInternal.Json.Linq;
//using System.Security.AccessControl;
//using System.Security.Principal;

namespace XeirGYA
{
    public partial class GYAFile
    {
        public static bool IsSQLiteDatabase(string pathToFile)
        {
            bool result = false;
            if (File.Exists(pathToFile))
            {
                using (FileStream stream = new FileStream(pathToFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    byte[] header = new byte[16];
                    for (int i = 0; i < 16; i++)
                        header[i] = (byte)stream.ReadByte();
                    result = System.Text.Encoding.UTF8.GetString(header).Contains("SQLite format 3");
                    stream.Close();
                }
            }
            return result;
        }

        //Basic cookies class for Unity Cookies file
        public class Cookies
        {
            public string name { get; set; }
            public string value { get; set; }
        }

#if UseSQLite

        //SQLite access to Unity Cookies file (sqlite)
        public static string GetFromUnityCookies_KharmaSession()
        {
            var stringToFind = "kharma_session";
            //var stringLength = 86;

            string cookiePath = GYAExt.PathUnityCookiesFile;

            FileInfo fi = new FileInfo(cookiePath);
            string fileFullName = fi.FullName;
            fileFullName = GYAExt.PathFixedForOS(fileFullName);

            //List<string> foundStrings = new List<string>();

            if (File.Exists(fileFullName) && IsSQLiteDatabase(fileFullName))
            {
                var conn = new SQLiteConnection(fileFullName);

                //var query = conn.Query<Cookies>("select * from cookies");
                //var query = conn.Table<Cookies>().Where(v => v.name.Equals(stringToFind));
                var query = conn.Query<Cookies>("select * from cookies where name = ?", stringToFind);

                if (query.Count() > 0)
                {
                    foreach (var row in query)
                    {
                        //GYAExt.Log("Row: " + query.Count());
                        return row.value;
                    }
                }

                //GYAExt.Log("Results: " + query.Count());
                //GYAExt.LogAsJson(query);
                //foreach (var row in query) GYAExt.Log("Query\t: " + row.name + "\n\t\t: " + row.value);

                conn.Close();
            }

            return ""; // Not found
        }

#else

        //Brute force READ ONLY from Unity Cookies file (sqlite)
        //This is not pretty or clean, but it does the trick
        //and avoids loading sqlite to access just one field
        public static string GetFromUnityCookies_KharmaSession()
        {
            var stringToFind = "kharma_session";
            var stringLength = 86;

            string cookiePath = GYAExt.PathUnityCookiesFile;

            FileInfo fi = new FileInfo(cookiePath);
            string fileFullName = fi.FullName;
            fileFullName = GYAExt.PathFixedForOS(fileFullName);

            List<string> foundStrings = new List<string>();

            try
            {
                if (File.Exists(fileFullName) && IsSQLiteDatabase(fileFullName))
                {
                    byte[] bStringToFind = Encoding.UTF8.GetBytes(stringToFind);

                    using (FileStream fs = File.Open(fileFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
	                    int byteRead;

                        do
                        {
                            byteRead = fs.ReadByte();
                            if (byteRead == -1) continue;
                            bool found = true;
                            foreach (byte t in bStringToFind)
                            {
                                if ((byte) byteRead == t)
                                {
                                    byteRead = fs.ReadByte();
                                    if (byteRead != -1) continue;
                                    found = false;
                                    break;
                                }

                                found = false;
                                break;
                            }
                            if (found)
                            {
                                //Debug.Log("String was found at offset: " + (fs.Position-1).ToString() + " (Hex: " + (fs.Position-1).ToString("X4") + ")");

                                fs.Seek(-1, SeekOrigin.Current);

                                byte[] bytes = new byte[86];
                                int position = 0;

                                int nbBytesRead = fs.Read(bytes, position, stringLength);
                                if (nbBytesRead == 0)
                                {
                                    found = false;
                                    break;
                                }

                                foundStrings.Add(Encoding.UTF8.GetString(bytes, 0, stringLength));
                                return foundStrings[0]; // Return 1st entry
                            }
                        } while (byteRead != -1);
                    }
                }
            }
            catch (Exception e)
            {
                GYAExt.LogWarning("GetDataFromUnityCookies - Reading file: " + cookiePath, "" + e.Message);
            }
            return ""; // Not found
        }

#endif

        //public static string GetDataFromUnityCookiesOLD(string stringToFind)
        //{
        //    string cookiePath = GYAExt.PathUnityCookiesFile;

        //    FileInfo fi = new FileInfo(cookiePath);
        //    string fileFullName = fi.FullName;
        //    //fileFullName = fileFullName.Replace('\\', '/');
        //    fileFullName = GYAExt.PathFixedForOS(fileFullName);

        //    List<string> foundStrings = new List<string>();

        //    try
        //    {
        //        if (File.Exists(fileFullName) && IsSQLiteDatabase(fileFullName))
        //        {

        //            byte[] bStringToFind = Encoding.UTF8.GetBytes(stringToFind);
        //            byte[] bEndOfRecord = { 47, 0 }; // End of Record Marker

        //            using (FileStream fs = File.Open(fileFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //            {
        //                int byteRead;

        //                do
        //                {
        //                    byteRead = fs.ReadByte();
        //                    if (byteRead == -1) continue;
        //                    bool found = true;
        //                    foreach (byte t in bStringToFind)
        //                    {
        //                        if ((byte)byteRead == t)
        //                        {
        //                            byteRead = fs.ReadByte();
        //                            if (byteRead != -1) continue;
        //                            found = false;
        //                            break;
        //                        }

        //                        found = false;
        //                        break;
        //                    }
        //                    if (found)
        //                    {
        //                        //Debug.Log("String was found at offset: " + (fs.Position-1).ToString() + " (Hex: " + (fs.Position-1).ToString("X4") + ")");

        //                        fs.Seek(-1, SeekOrigin.Current);

        //                        byte[] bytes = new byte[1024];
        //                        int position = 0;
        //                        do
        //                        {
        //                            int nbBytesRead = fs.Read(bytes, position, 1024);
        //                            if (nbBytesRead == 0)
        //                            {
        //                                found = false;
        //                                break;
        //                            }
        //                            for (int i = position; i <= position + nbBytesRead - bEndOfRecord.Length; i++)
        //                            {
        //                                found = !bEndOfRecord.Where((t, j) => bytes[i + j] != t).Any();
        //                                if (found)
        //                                {
        //                                    //strings.Add(System.Text.Encoding.Unicode.GetString(bytes, 0, i));
        //                                    foundStrings.Add(Encoding.UTF8.GetString(bytes, 0, i));
        //                                    // Restart the loop after the bEndOfRecord
        //                                    //fs.Seek(-nbBytesRead - position + i + bEndOfRecord.Length, SeekOrigin.Current);
        //                                    //break;

        //                                    //GYAExt.LogAsJson(foundStrings[0]);
        //                                    return foundStrings[0]; // Return 1st entry
        //                                }
        //                            }
        //                            if (found) continue;
        //                            Array.Resize(ref bytes, bytes.Length + 1024);
        //                            position += 1024;
        //                        } while (!found);
        //                    }
        //                } while (byteRead != -1);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        GYAExt.LogWarning("GetDataFromUnityCookies - Reading file: " + cookiePath, "" + e.Message);
        //    }
        //    return ""; // Not found
        //}

        // Copy to clipboard, default as JSON
        internal static void CopyToClipboard(object obj, bool asString = false)
        {
            TextEditor te = new TextEditor();

            if (asString) // as String, no quotes
            {
#if UNITY_5_3_OR_NEWER
                te.text = obj.ToString();
#else
				te.content = new GUIContent(obj.ToString());
#endif
            }
            else // as JSON
            {
#if UNITY_5_3_OR_NEWER
                te.text = GYAExt.ToJson(obj, true);
#else
				te.content = new GUIContent(GYAExt.ToJson(obj, true));
#endif
            }

            te.SelectAll();
            te.Copy();
        }

        internal static Dictionary<string, string> GetGUIDsAndPathsForAllAssets()
        {
            // Gather GUID & Path for All Assets
            var assets = AssetDatabase.GetAllAssetPaths().ToList();
            Dictionary<string, string> guidList = assets.ToDictionary(asset => AssetDatabase.AssetPathToGUID(asset));
            CopyToClipboard(guidList);
            return guidList;
        }

        // Extract a fragment "property" from JSON and return it as a structure <T>
        internal static T DeserializeJSONPropertyAs<T>(string pJsonText, string pPropertyName)
        {
            var root = JObject.Parse(pJsonText);
            var serializer = new JsonSerializer();
            return serializer.Deserialize<T>(root[pPropertyName].CreateReader());
        }

        internal static long GetDirectorySize(FileInfo[] files)
        {
            return files.Sum(f => f.Length);
        }

        internal static long GetDirectorySize(string path, string searchPattern = "*.*")
        {
            // Get array of all file names.
            string[] files = Directory.GetFiles(path, searchPattern);

            // Calculate total bytes of all files in a loop.
            return files.Select(f => new FileInfo(f)).Select(fi => fi.Length).Sum();
        }

        // ConvertGYAv2Tov3
        internal static void ConvertGYAv2Tov3()
        {
            string jsonFileUserOLD = Path.Combine(GYAExt.PathGYADataFiles, "GYA Settings.json");

            if ((!File.Exists(GYA.gyaVars.Files.Prefs.file)) && File.Exists(jsonFileUserOLD))
            {
                GYAExt.LogWarning("Converting Settings & Groups to v3.",
                    "Please upgrade All projects that use GYA to v3.");

                File.SetAttributes(jsonFileUserOLD, FileAttributes.Normal);
                string jsonText = File.ReadAllText(jsonFileUserOLD);

                // -- Perform Settings changes here
                JObject json = JObject.Parse(jsonText);
                var root = json["Settings"];
                string pathUserAssets = (string) root["pathUserAssets"];
                JArray pathUserAssetsList = (JArray) root["pathUserAssetsList"];

                // Update fields
                pathUserAssetsList.Insert(0, pathUserAssets); // Merge pathUserAssets
                root["pathUserAssets"].Replace(""); // Clear field
                root["pathUserAssetsList"] = pathUserAssetsList; // Replace with merged data

                // remove: "pathUserAssets": "", & rename pathUserAssetsList
                var prefsText = json.ToString().Replace("\"pathUserAssets\": \"\",", "");
                prefsText = prefsText.Replace("\"pathUserAssetsList\"", "\"pathUserAssets\"");

                // Extract 'settings' fragment
                GYA.gyaVars.Prefs = DeserializeJSONPropertyAs<GYAPrefs>(prefsText, "Settings");
                //GYA.gyaVars.Prefs.autoConsolidate = false; // Removed in v3
                GYAFile.SaveGYAPrefs();

                // Extract 'groups' fragment
                GYA.gyaData.Groups = DeserializeJSONPropertyAs<List<GYAData.Group>>(prefsText, "Group");
                GYAFile.SaveGYAGroups();

                // -- Backup v2 file, leave original in case other projects have not been upgraded yet
                string targetFile = jsonFileUserOLD.Replace(".json", ".v2");
                if (!File.Exists(targetFile))
                    File.Copy(jsonFileUserOLD, targetFile);

                GYAExt.Log("Old Settings backed up with extension '.v2'.",
                    "Next 'Refresh' will update your asset collection to v3 if required.");
            }
        }

        internal static bool IsPALValid(string stringPAL)
        {
            //GYAExt.Log(stringPAL);

            // Search for "\"name\": \"Packages\"," .. OR .. "\"items\": [", if found then PAL is pre-May 15th 2017

            if (stringPAL.Contains("\"name\": \"Packages\",") && stringPAL.Contains("\"items\": ["))
            {
                GYAExt.Log("'Purchased Assets List' has invalid structure.  Deleting.",
                    "Refresh to download current PAL.");
                File.Delete(GYA.gyaVars.Files.ASPurchase.file);
                GYA.gyaVars.Files.ASPurchase.fileExists = false;

                return false;
            }

            //ErrorStateClear();
            return true;
        }

        // Load AS Packages
        internal static bool LoadASPackages()
        {
            if (!File.Exists(GYA.gyaVars.Files.ASPackage.file))
            {
                GYA.gyaVars.Files.ASPackage.fileExists = false;
                return false;
            }

            GYA.gyaVars.Files.ASPackage.fileExists = true;
            string jsonText = File.ReadAllText(GYA.gyaVars.Files.ASPackage.file);
            GYA.asPackages = JsonConvert.DeserializeObject<ASPackageList>(jsonText);

            return true;
        }

        // Load AS Purchased
        internal static bool LoadASPurchased()
        {
            if (!File.Exists(GYA.gyaVars.Files.ASPurchase.file))
            {
                GYA.gyaVars.Files.ASPurchase.fileExists = false;
                return false;
            }

            try
            {
                string jsonText = File.ReadAllText(GYA.gyaVars.Files.ASPurchase.file);

                // Add code to detect if PAL has changed, if it has then delete outdated file
                if (IsPALValid(jsonText))
                {
                    GYA.asPurchased = JsonConvert.DeserializeObject<ASPurchased>(jsonText);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                GYAExt.LogWarning("LoadASPurchased - Deleting file: " + GYA.gyaVars.Files.ASPurchase.file,
                    "It will be re-created during the next 'Refresh' if required.\n\n" + e.Message);

                if (File.Exists(GYA.gyaVars.Files.ASPurchase.file))
                    File.Delete(GYA.gyaVars.Files.ASPurchase.file);

                return false;
            }

            GYA.gyaVars.Files.ASPurchase.fileExists = true;
            return true;
        }

        // Load GYA Settings
        internal static bool LoadGYAPrefs()
        {
            if (!File.Exists(GYA.gyaVars.Files.Prefs.file))
            {
                SaveGYAPrefs();
            }

            try
            {
                GYA.gyaVars.Files.Prefs.fileExists = true;
                string jsonText = File.ReadAllText(GYA.gyaVars.Files.Prefs.file);
                GYA.gyaVars.Prefs = JsonConvert.DeserializeObject<GYAPrefs>(jsonText);

                // Verify GYA.gyaVars.Prefs.pathUserAssets != null
                //if (GYA.gyaVars.Prefs.pathUserAssets == null)
                //	GYA.gyaVars.Prefs.pathUserAssets = new List<string>();
            }
            catch (Exception e)
            {
                GYAExt.LogWarning("LoadGYAPrefs - Error loading User Prefs: ", e.Message);
                GYA.Instance.ErrorStateSet(GYA.ErrorCode.Error);
                return false;
            }

            return true;
        }

        // Load GYA Settings
        internal static bool LoadGYAGroups()
        {
            // Check if exists
            if (!File.Exists(GYA.gyaVars.Files.Groups.file))
            {
                GYA.Instance.GroupCreate("Favorites");
                SaveGYAGroups();
            }

            GYA.gyaVars.Files.Groups.fileExists = true;
            string jsonText = File.ReadAllText(GYA.gyaVars.Files.Groups.file);
            GYA.gyaData.Groups = JsonConvert.DeserializeObject<List<GYAData.Group>>(jsonText);

            return true;
        }

        // Create or over-write the json file for user info
        internal static void SaveGYAPrefs(bool append = false)
        {
            // Make any last minute changes to User data prior to saving
            GYA.gyaData.version = GYA.gyaVars.version;

            // Let the saving commence
            TextWriter writer = null;
            GYAPrefs objectToWrite = GYA.gyaVars.Prefs;

            try
            {
                if (File.Exists(GYA.gyaVars.Files.Prefs.file))
                    File.SetAttributes(GYA.gyaVars.Files.Prefs.file, FileAttributes.Normal);

                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite, Formatting.Indented);
                //var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite, Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
                writer = new StreamWriter(GYA.gyaVars.Files.Prefs.file, append);
                writer.Write(contentsToWriteToFile);
            }
            catch (Exception ex)
            {
                GYAExt.LogWarning("SaveGYAPrefs Error: ", ex.Message);
                GYA.Instance.ErrorStateSet(GYA.ErrorCode.Error);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        // Create or over-write the json file for user info
        internal static void SaveGYAGroups(bool append = false)
        {
            // Make any last minute changes to User data prior to saving
            GYA.gyaData.version = GYA.gyaVars.version;

            // Let the saving commence
            TextWriter writer = null;
            List<GYAData.Group> objectToWrite = GYA.gyaData.Groups;

            try
            {
                if (File.Exists(GYA.gyaVars.Files.Groups.file))
                    File.SetAttributes(GYA.gyaVars.Files.Groups.file, FileAttributes.Normal);

                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite, Formatting.Indented);
                //var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite, Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
                writer = new StreamWriter(GYA.gyaVars.Files.Groups.file, append);
                writer.Write(contentsToWriteToFile);
            }
            catch (Exception ex)
            {
                GYAExt.LogWarning("SaveGYAGroups Error: ", ex.Message);
                GYA.Instance.ErrorStateSet(GYA.ErrorCode.Error);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        // Load the package data from file: Grab Yer Assets.json
        //internal void LoadGYAAssets (bool bypassVerCheck = false) {
        public static bool LoadGYAAssets()
        {
            GYA.gyaVars.Files.Assets.fileExists = true;
            string jsonText = File.ReadAllText(GYA.gyaVars.Files.Assets.file);

            try
            {
                //gyaData = JsonConvert.DeserializeObject<GYAAssets>(jsonText);
                GYA.gyaData = JsonConvert.DeserializeObject<GYAData>(jsonText);

                // Refresh & Save if diff major version
                //if ( !GYAFile.isDataVersionSame() )
                //{
                //	GYAPackage.RefreshAllCollections(true);
                //	return false;
                //}

                LoadGYAGroups();

                // verify JSON
                if (GYA.Instance.JSONObjectsAreNotNULL)
                {
#if EnableVerCheck // File version check to make sure that old data isn't used in case of layout changes
//string fileVersion = pkgData.version;
					int verCompare = GYA.gyaData.version.CompareTo(GYA.gyaVars.version);
					//if (fileVersion != gyaVars.version && !bypassVerCheck) {
					if (GYA.gyaVars.Prefs.enableOfflineMode)
					{
						// Offline mode enabled, do not refresh package list
						GYAExt.Log("Offline Mode is currently ENABLED.  Using exisitng data file.");
					}
					else if (verCompare < 0)
					{
						// If pkgData.version doesn't match the current assetVersion, refresh package data
						GYAExt.Log(Path.GetFileName(GYA.gyaVars.Files.Assets.file) + " is out of date - Refreshing data file.", "Versions: (Current GYA = " + GYA.gyaVars.version + ") ( Old JSON = " + GYA.gyaData.version + ")");
						GYAPackage.RefreshAllCollections();
						return;
					}

#endif

                    // Post-proc json
                    //LoadJSONPostProcess();

                    GYA.gyaData.Assets.Sort((x, y) => -x.version_id.CompareTo(y.version_id));

                    foreach (GYAData.Asset t in GYA.gyaData.Assets)
                    {
// Check for damaged asset
                        if (t.isDamaged)
                        {
                            if (t.title
                                .StartsWith("unknown", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var newTitle = GYAPackage.GetAssetNameFromOldIDIfExist(t.id,
                                    t.version_id);
                                t.title = newTitle;
                            }
                        }
                    }

                    //GYAPackage.GYADataPostProcessing();

                    // Calculate the old assets
                    GYAPackage.BuildOldAssetsList();
                    //}
                    //
                    GYA.Instance.RefreshSV();

                    // Check if loaded file is same major version, if not, save current version
                    if (!GYAFile.isAssetsFileSameVersion())
                    {
                        //GYA.gyaData.Assets.ForEach(x =>  {
                        //	x.fileDateCreated = GYAExt.DateAsISO8601(x.fileDateCreated) as DateTimeOffset;
                        //	x.fileDataCreated = GYAExt.DateAsISO8601(x.fileDataCreated) as DateTimeOffset;
                        //});

                        GYAFile.SaveGYAAssets();
                    }
                }

                ProcessASPurchased();
            }
            catch (Exception ex)
            {
                GYAExt.LogError("LoadGYAAssets: ", ex.Message);

                // DISABLED in v3.16.8.x
                //if (File.Exists(GYA.gyaVars.Files.Assets.file))
                //{
                //	GYAExt.Log("LoadGYAAssets: Deleting GYA Assets.json - Press Refresh to manually re-create it.");
                //	File.SetAttributes(GYA.gyaVars.Files.Assets.file, FileAttributes.Normal);
                //	File.Delete(GYA.gyaVars.Files.Assets.file);
                //}
                GYA.Instance.ErrorStateSet(GYA.ErrorCode.ErrorStep2);
            }

            return true;
        }

        // Process asPurchased - check for deprecated assets, etc.
        public static void ProcessASPurchased()
        {
            if (GYA.asPurchased.results != null && GYA.asPurchased.results.Count > 0)
            {
                // Remove any assets marked as NOT DOWNLOADED
                //GYA.gyaData.Assets.RemoveAll(x => x.notDownloaded);

                //foreach(var v in GYA.gyaData.Assets)
                foreach (GYAData.Asset t in GYA.gyaData.Assets)
                {
//var vTmp = GYA.asPurchased.results[0].items.Find( x => x.id == GYA.gyaData.Assets[i].id );
                    var vTmp = GYA.asPurchased.results.Find(x => x.id == t.id);
                    if (vTmp != null) // version_id's match
                    {
                        // Purchased Item
                        t.Purchased = vTmp;

                        // Is in Purchased list
                        t.isInPurchasedList = true;

                        // Is deprecated
                        t.isDeprecated = (vTmp.status.ToLower() == "deprecated");


                        // Date Purchased
                        if (vTmp.purchased_at != null)
                        {
                            t.datePurchased = GYAExt.DateStringAsDTO(vTmp.purchased_at.ToString());
                        }
                        else
                        {
                            //GYA.gyaData.Assets[i].datePurchased = Convert.ToDateTime(DateTime.MinValue);

                            var tempDateTime = Convert.ToDateTime(DateTime.MinValue);
                            tempDateTime = DateTime.SpecifyKind(tempDateTime, DateTimeKind.Utc);
                            t.datePurchased = tempDateTime;
                        }

                        if (vTmp.created_at != null)
                        {
                            t.dateCreated = GYAExt.DateStringAsDTO(vTmp.created_at.ToString());
                        }
                        else
                        {
                            //GYA.gyaData.Assets[i].datePurchased = Convert.ToDateTime(DateTime.MinValue);

                            var tempDateTime = Convert.ToDateTime(DateTime.MinValue);
                            tempDateTime = DateTime.SpecifyKind(tempDateTime, DateTimeKind.Utc);
                            t.dateCreated = tempDateTime;
                        }

                        if (vTmp.updated_at != null)
                        {
                            t.dateUpdated = GYAExt.DateStringAsDTO(vTmp.updated_at.ToString());
                        }
                        else
                        {
                            //GYA.gyaData.Assets[i].datePurchased = Convert.ToDateTime(DateTime.MinValue);

                            var tempDateTime = Convert.ToDateTime(DateTime.MinValue);
                            tempDateTime = DateTime.SpecifyKind(tempDateTime, DateTimeKind.Utc);
                            t.dateUpdated = tempDateTime;
                        }

                        // Icon link
                        t.icon = vTmp.icon;
                    }
                }
            }
        }

        public static bool isAssetsFileSameVersion()
        {
            bool isSameMajorVersion = GYA.gyaData.version[0] == GYA.gyaVars.version[0];
            return isSameMajorVersion;
        }

        // Return JSON As Formatted JSON
        public static string JsonAsFormatted(string json, bool asIndented = true)
        {
            try
            {
                using (var stringReader = new StringReader(json))
                using (var stringWriter = new StringWriter())
                {
                    var jsonReader = new JsonTextReader(stringReader);
                    var jsonWriter = new JsonTextWriter(stringWriter)
                    {
                        Formatting = (asIndented ? Formatting.Indented : Formatting.None),
                        Culture = System.Globalization.CultureInfo.InvariantCulture,
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        DateTimeZoneHandling = DateTimeZoneHandling.Utc
                    };

                    jsonWriter.WriteToken(jsonReader);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                GYAExt.LogWarning("JsonAsFormatted: ", ex.Message);
                return null;
            }
        }

        internal static void SaveFile(string pFile, string pText, bool pAddGYADataPath = false)
        {
            if (pAddGYADataPath)
            {
                // Default to GYA Data folder
                pFile = Path.GetFullPath(Path.Combine(GYAExt.PathGYADataFiles, pFile));
            }

            try
            {
                if (File.Exists(pFile))
                {
                    File.SetAttributes(pFile, FileAttributes.Normal);
                    File.Delete(pFile);
                }

                using (StreamWriter sw = new StreamWriter(pFile))
                {
                    sw.Write(pText);
                }
            }
            catch (Exception ex)
            {
                GYAExt.LogWarning("SaveFile: " + pFile, ex.Message, false);
            }
        }

        // Multi-stage save: <file>.tmp, to JsonAsFormatted(x), to <file>
        internal static void SaveFileJson(string pFile, string pText, bool pAddGYADataPath = false)
        {
            string pFileTmp = pFile + ".tmp";

            // Save as <file>.tmp
            SaveFile(pFileTmp, pText, pAddGYADataPath);

            // Save file as <file>
            var pFormatted = JsonAsFormatted(pText);
            if (pFormatted != null)
            {
                SaveFile(pFile, pFormatted, pAddGYADataPath);
                // Delete tmp file
                if (File.Exists(pFileTmp))
                    File.Delete(pFileTmp);
            }
            else
            {
                //GYAExt.LogWarning("SaveFileJson: " + pFile, "Check file for issues: " + pFileTmp);
                GYAExt.LogWarning("SaveFileJson - File may be invalid: " + pFileTmp);
            }
        }

        // Create or over-write the json file for package info
        // Blank = save directly from gyaData.Assets
        public static void SaveGYAAssets(string jsonToWrite = "")
        {
            GYA.gyaData.version = GYA.gyaVars.version; // Update the version for the file

            TextWriter writer = null;
            string contentsToWriteToFile = String.Empty;

            try
            {
                if (File.Exists(GYA.gyaVars.Files.Assets.file))
                    File.SetAttributes(GYA.gyaVars.Files.Assets.file, FileAttributes.Normal);

                if (jsonToWrite.Length == 0)
                {
                    // Save just the Assets
                    var gyaTmp = new GYAData
                    {
                        version = GYA.gyaData.version,
                        Assets = GYA.gyaData.Assets
                    };

                    contentsToWriteToFile = JsonConvert.SerializeObject(gyaTmp, Formatting.Indented);
                    gyaTmp = null;
                }
                else
                {
                    var parsedJSON = JsonConvert.DeserializeObject(jsonToWrite);
                    contentsToWriteToFile = JsonConvert.SerializeObject(parsedJSON, Formatting.Indented);
                }

                writer = new StreamWriter(GYA.gyaVars.Files.Assets.file, false);
                writer.Write(contentsToWriteToFile);
            }
            catch (Exception ex)
            {
                GYAExt.LogWarning("SaveGYAAssets Error: ", ex.Message);
                GYA.Instance.ErrorStateSet(GYA.ErrorCode.Error);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        // Backup the "Grab Yer Assets User.json" file
        internal static void BackupUserData()
        {
            //string backupExt = DateTime.Now.ToString ("MMddHHmmss");
            string backupExt = "bak";
            string jsonFilePrefsBackup = Path.ChangeExtension(GYA.gyaVars.Files.Prefs.file, backupExt);
            string jsonFileGroupsBackup = Path.ChangeExtension(GYA.gyaVars.Files.Groups.file, backupExt);

            GYAExt.Log(
                "Backing up as:\t" + Path.GetFileName(jsonFilePrefsBackup) + " & " +
                Path.GetFileName(jsonFileGroupsBackup), "\tTo:\t" + Path.GetDirectoryName(jsonFilePrefsBackup)
            );

            // If data files exists load it, else create it
            if (Directory.Exists(GYAExt.PathGYADataFiles))
            {
                // Prefs
                if (File.Exists(GYA.gyaVars.Files.Prefs.file))
                {
                    File.SetAttributes(GYA.gyaVars.Files.Prefs.file, FileAttributes.Normal);
                    if (File.Exists(jsonFilePrefsBackup))
                        File.SetAttributes(jsonFilePrefsBackup, FileAttributes.Normal);

                    File.Copy(GYA.gyaVars.Files.Prefs.file, jsonFilePrefsBackup, true);
                }
                else
                {
                    GYAExt.LogWarning("User file not found: " + GYA.gyaVars.Files.Prefs.file);
                }

                // Groups
                if (File.Exists(GYA.gyaVars.Files.Groups.file))
                {
                    File.SetAttributes(GYA.gyaVars.Files.Groups.file, FileAttributes.Normal);
                    if (File.Exists(jsonFileGroupsBackup))
                        File.SetAttributes(jsonFileGroupsBackup, FileAttributes.Normal);

                    File.Copy(GYA.gyaVars.Files.Groups.file, jsonFileGroupsBackup, true);
                }
                else
                {
                    GYAExt.LogWarning("User file not found: " + GYA.gyaVars.Files.Groups.file);
                }
            }
        }

        // Restore the "Grab Yer Assets User.json" file
        internal static void RestoreUserData()
        {
            string backupExt = "bak";
            string jsonFilePrefsBackup = Path.ChangeExtension(GYA.gyaVars.Files.Prefs.file, backupExt);
            string jsonFileGroupsBackup = Path.ChangeExtension(GYA.gyaVars.Files.Groups.file, backupExt);

            GYAExt.Log(
                "Restoring:\t" + Path.GetFileName(jsonFilePrefsBackup) + " & " + Path.GetFileName(jsonFileGroupsBackup),
                "From:\t" + Path.GetDirectoryName(jsonFilePrefsBackup));

            // Prefs - If data files exists load it, else create it
            if (Directory.Exists(GYAExt.PathGYADataFiles))
            {
                if (File.Exists(jsonFilePrefsBackup))
                {
                    File.SetAttributes(jsonFilePrefsBackup, FileAttributes.Normal);
                    if (File.Exists(GYA.gyaVars.Files.Prefs.file))
                        File.SetAttributes(GYA.gyaVars.Files.Prefs.file, FileAttributes.Normal);

                    File.Copy(jsonFilePrefsBackup, GYA.gyaVars.Files.Prefs.file, true);
                }
                else
                {
                    GYAExt.LogWarning("User backup file not found: " + jsonFilePrefsBackup);
                }
            }

            // Groups - If data files exists load it, else create it
            if (Directory.Exists(GYAExt.PathGYADataFiles))
            {
                if (File.Exists(jsonFileGroupsBackup))
                {
                    File.SetAttributes(jsonFileGroupsBackup, FileAttributes.Normal);
                    if (File.Exists(GYA.gyaVars.Files.Groups.file))
                        File.SetAttributes(GYA.gyaVars.Files.Groups.file, FileAttributes.Normal);

                    File.Copy(jsonFileGroupsBackup, GYA.gyaVars.Files.Groups.file, true);
                }
                else
                {
                    GYAExt.LogWarning("User backup file not found: " + jsonFileGroupsBackup);
                }
            }
        }


        // Convert 0 to EMPTY field for CSV export
        internal static string CSVZeroToEmpty(int pInt)
        {
            return pInt == 0 ? "" : pInt.ToString();
        }

        internal static string WrapCSVCell(object obj)
        {
            bool mustQuote = true;
            //bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));

            string str = obj.ToString();

            if (mustQuote)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");

                foreach (char nextChar in str)
                {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                        sb.Append("\"");
                }

                sb.Append("\"");
                return sb.ToString();
            }
            return str;
        }

        // Save Embedded Asset Info as CSV
        internal static void SaveAsCSV(List<GYAData.Asset> pkgList, string path)
        {
            var result = new StringBuilder();
            var csvFile = new List<string>();
            TextWriter writer = null;

            csvFile.Add(
                "\"icon\",\"title\",\"link\",\"id\",\"pubdate\",\"version\",\"version_id\",\"unity_version\",\"category_label\",\"category_id\",\"publisher_label\",\"publisher_id\",\"filePath\",\"fileSize\",\"isExported\",\"collection\"");

            foreach (GYAData.Asset item in pkgList)
            {
                string assetURL = string.Empty;
                string assetIcon = string.Empty;

                // Ignore exported (non-Asset Store packages)
                if (!item.isExported)
                {
                    // Works for Google Sheets
                    // Numbers/Excel may encode # to %23 when clicking/sending the link to the browser
                    assetURL = "https://www.assetstore.unity3d.com/#/" + item.link.type + "/" + item.link.id;
                    assetURL = "=HYPERLINK(\"" + assetURL + "\", \"link\")";

                    if (!string.IsNullOrEmpty(item.icon))
                    {
                        assetIcon = "=IMAGE(\"http:";
                        assetIcon += item.icon;
                        assetIcon += "\")";
                    }
                }

                csvFile.Add(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}",
                    WrapCSVCell(assetIcon),
                    WrapCSVCell(item.title),
                    WrapCSVCell(assetURL),
                    //WrapCSVCell(Path.GetFileNameWithoutExtension(item.filePath)),
                    WrapCSVCell(CSVZeroToEmpty(item.id)),
                    WrapCSVCell(item.pubdate),
                    WrapCSVCell(item.version),
                    WrapCSVCell(CSVZeroToEmpty(item.version_id)),
                    WrapCSVCell(item.unity_version),
                    WrapCSVCell(item.category.label),
                    WrapCSVCell(CSVZeroToEmpty(item.category.id)),
                    WrapCSVCell(item.publisher.label),
                    WrapCSVCell(CSVZeroToEmpty(item.publisher.id)),
                    WrapCSVCell(item.filePath),
                    WrapCSVCell(item.fileSize),
                    WrapCSVCell(item.isExported),
                    WrapCSVCell(item.collection)
                    //WrapCSVCell(item.isDamaged)
                    //WrapCSVCell(item.description),
                    //WrapCSVCell(item.publishnotes)
                ));
            }

            foreach (string line in csvFile)
            {
                result.AppendLine(line);
            }

            try
            {
                writer = new StreamWriter(path);
                writer.Write(result);
            }
            catch (Exception ex)
            {
                GYAExt.LogWarning("Exporting Error:", ex.Message);
            }
            finally
            {
                if (writer != null)
                    writer.Close();

                GYAExt.Log("Exported Asset List as: " + path);
            }
        }

        // Version for GYAStore - embed asset icon url for google sheets
        internal static void SaveAsCSVGroup(string path)
        {
            var result = new StringBuilder();
            var csvFile = new List<string>();
            TextWriter writer = null;

            csvFile.Add("\"icon\",\"title\",\"link\",\"version\",\"category_label\"");

            foreach (GYAData.Asset item in GYA.grpData[GYA.showGroup])
            {
                string assetURL = string.Empty;
                string assetIcon = string.Empty;

                // Asset Icon
                // Ignore exported (non-Asset Store packages)
                if (!item.isExported)
                {
                    // Retrieve icon url
                    if (!string.IsNullOrEmpty(item.icon))
                    {
                        assetIcon = "=IMAGE(\"http:";
                        assetIcon += item.icon;
                        assetIcon += "\")";
                    }

                    // Works for Google Sheets, Libre Office
                    // Numbers/Excel may encode # to %23 when clicking/sending the link to the browser

                    // Asset Link
                    assetURL = "https://www.assetstore.unity3d.com/#/" + item.link.type + "/" + item.link.id;
                    assetURL = "=HYPERLINK(\"" + assetURL + "\", \"link\")";
                }

                csvFile.Add(string.Format("{0},{1},{2},{3},{4}",
                    WrapCSVCell(assetIcon),
                    WrapCSVCell(item.title),
                    WrapCSVCell(assetURL),
                    WrapCSVCell(item.version),
                    WrapCSVCell(item.category.label)
                ));
            }

            foreach (string line in csvFile)
            {
                result.AppendLine(line);
            }

            try
            {
                writer = new StreamWriter(path);
                writer.Write(result);
            }
            catch (Exception ex)
            {
                GYAExt.LogWarning("Exporting Error: ", ex.Message);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
                GYAExt.Log("Exported Group List as: " + path);
            }
        }

        //// Yes I could do this in a loop, but I didn't
        //internal static void CleanOutdatedGYASupportFiles()
        //{
        //    // Delete ALL outdated files
        //    string resultText = "Scanning for and Deleting Outdated GYA Files/Folders.\n";

        //    string oldpathOldAssetsFolderName = "-" + GYA.gyaVars.name + " (Old)"; // Old Assets folder
        //    string oldjsonFilePackagesName = GYA.gyaVars.name + ".json"; // GYA Packages Data file
        //    string oldjsonFileUserName = GYA.gyaVars.name + " User.json"; // GYA User Data file
        //    string oldjsonFileUserBak = GYA.gyaVars.name + " User.bak"; // GYA User Data file backup

        //    // User File
        //    string path1 = Path.GetFullPath(Path.Combine(GYAExt.PathUnityAssetStore4, oldjsonFileUserName));
        //    string path2 = Path.GetFullPath(Path.Combine(GYAExt.PathUnityDataFiles, oldjsonFileUserName));
        //    // User File BAK
        //    string path3 = Path.GetFullPath(Path.Combine(GYAExt.PathUnityAssetStore4, oldjsonFileUserBak));
        //    string path4 = Path.GetFullPath(Path.Combine(GYAExt.PathUnityDataFiles, oldjsonFileUserBak));
        //    // Assets File
        //    string path5 = Path.GetFullPath(Path.Combine(GYAExt.PathUnityAssetStore4, oldjsonFilePackagesName));
        //    string path6 = Path.GetFullPath(Path.Combine(GYAExt.PathUnityDataFiles, oldjsonFilePackagesName));
        //    // Old Assets Folder
        //    string path7 = Path.GetFullPath(Path.Combine(GYAExt.PathUnityAssetStore4, oldpathOldAssetsFolderName));
        //    string path8 = Path.GetFullPath(Path.Combine(GYAExt.PathUnityDataFiles, oldpathOldAssetsFolderName));

        //    try
        //    {
        //        // User file
        //        if (File.Exists(path1))
        //        {
        //            resultText = resultText + "\tDeleting: " + path1 + "\n";
        //            File.Delete(path1);
        //        }
        //        if (File.Exists(path2))
        //        {
        //            resultText = resultText + "\tDeleting: " + path2 + "\n";
        //            File.Delete(path2);
        //        }
        //        // User file BAK
        //        if (File.Exists(path3))
        //        {
        //            resultText = resultText + "\tDeleting: " + path3 + "\n";
        //            File.Delete(path3);
        //        }
        //        if (File.Exists(path4))
        //        {
        //            resultText = resultText + "\tDeleting: " + path4 + "\n";
        //            File.Delete(path4);
        //        }
        //        // Assets file
        //        if (File.Exists(path5))
        //        {
        //            resultText = resultText + "\tDeleting: " + path5 + "\n";
        //            File.Delete(path5);
        //        }
        //        if (File.Exists(path6))
        //        {
        //            resultText = resultText + "\tDeleting: " + path6 + "\n";
        //            File.Delete(path6);
        //        }
        //        // Old Assets folder
        //        if (Directory.Exists(path7))
        //        {
        //            resultText = resultText + "\tDeleting: " + path7 + "\n";
        //            Directory.Delete(path7, true);
        //        }
        //        if (Directory.Exists(path8))
        //        {
        //            resultText = resultText + "\tDeleting: " + path8 + "\n";
        //            Directory.Delete(path8, true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GYAExt.LogError("Error Removing file/folder: ", ex.Message);
        //    }
        //    resultText = resultText + "\tScanning complete.\n";
        //    GYAExt.Log(resultText, null, false);
        //}

        //// Move both prior Old Assets contents to the new one
        //internal static int MoveFolderContentsOldAssets(bool overwrite = false)
        //{
        //    int count = 0;
        //    count += MoveFolderContents(Path.Combine(GYAExt.PathUnityAssetStore4, "-" + GYA.gyaVars.name + " (Old)"),
        //        GYA.gyaVars.pathOldAssetsFolder, overwrite);
        //    count += MoveFolderContents(Path.Combine(GYAExt.PathUnityDataFiles, "-" + GYA.gyaVars.name + " (Old)"),
        //        GYA.gyaVars.pathOldAssetsFolder, overwrite);
        //    return count;
        //}

        // Move Contents from one folder to another accounting for symlinks, Create if required
        internal static int MoveFolderContents(string moveFrom, string moveTo, bool overwrite = false)
        {
            int count = 0;
            int countTotal = 0;
            string result = string.Empty;
            //string fileFrom = "";

            if (moveFrom != null && moveTo != null)
            {
                if (!Directory.Exists(moveFrom))
                {
                    // Folder missing, exit
                    //GYAExt.Log("Source folder does NOT exist: " + moveFrom );
                }
                else
                {
                    if (!Directory.Exists(moveTo))
                        CreateFolder(moveTo);

                    DirectoryInfo directory = new DirectoryInfo(moveFrom);

                    if (directory.Exists)
                    {
                        FileInfo[] files = directory.GetFiles("*.unity?ackage", SearchOption.AllDirectories)
                            .Where(fi => (fi.Attributes & FileAttributes.Hidden) == 0).ToArray();

                        int filenameStartIndex = (directory.FullName.Length + 1);
                        using (
                            var progressBar = new GYA.ProgressBar(
                                string.Format("{0} Copying {1}", GYA.gyaVars.abbr, directory.FullName),
                                files.Length,
                                80,
                                stepNumber => files[stepNumber].FullName.Substring(filenameStartIndex)
                            )
                        )

                            for (int i = 0; i < files.Length; ++i)
                            {
                                var fileTo = Path.Combine(moveTo, files[i].Name);
                                progressBar.Update(i);
                                try
                                {
                                    File.SetAttributes(files[i].FullName, FileAttributes.Normal);
                                    countTotal += 1;
                                    if (File.Exists(fileTo) && !overwrite)
                                    {
                                        result += "EXISTS:\t" + files[i].FullName + "\n\tTo: " + fileTo + "\n";
                                    }
                                    else
                                    {
                                        if (GYAFile.IsSymLink(moveFrom) || GYAFile.IsSymLink(moveTo))
                                        {
                                            File.Copy(files[i].FullName, fileTo, overwrite);
                                            result += "Copied:\t" + files[i].FullName + "\n\tTo: " + fileTo + "\n";
                                        }
                                        else
                                        {
                                            if (overwrite)
                                            {
                                                File.Delete(fileTo);
                                                File.Move(files[i].FullName, fileTo);
                                            }
                                            else
                                            {
                                                File.Move(files[i].FullName, fileTo);
                                            }
                                            result += "Moved:\t" + files[i].FullName + "\n\tTo: " + fileTo + "\n";
                                        }
                                        // If successful, delete the old file
                                        if (File.Exists(fileTo))
                                        {
                                            count += 1;
                                            File.Delete(files[i].FullName);
                                        }
                                    }
                                }
                                catch (IOException ex)
                                {
                                    GYAExt.LogWarning("Error Moving: " + files[i].FullName,
                                        "To: " + fileTo + "\n\n" + ex.Message);
                                    result += "ERROR:\t" + files[i].FullName + "\n\tTo: " + fileTo + "\n";
                                }
                            }
                        if (countTotal > 0)
                        {
                            result = "Copied " + count + " of " + countTotal + "\n\n" + result;
                            GYAExt.Log(result);
                            GYAExt.Log("Once you have verified that your Old Assets have been moved without error,",
                                "you can use 'Menu->Maintenance->Clean up Outdated GYA Support Files' to cleanup the outdated files/folders.");
                            GYAPackage.RefreshAllCollections();
                        }
                    }
                }
            }
            return count;
        }

        //internal static void MoveFolderContentsOldAssetsMain()
        //{
        //    string pathTmp = Path.Combine(GYAExt.PathUnityAssetStore4, "-" + GYA.gyaVars.name + " (Old)");
        //    string pathTmp2 = Path.Combine(GYAExt.PathUnityDataFiles, "-" + GYA.gyaVars.name + " (Old)");
        //    if (Directory.Exists(pathTmp) || Directory.Exists(pathTmp2))
        //    {
        //        var count = MoveFolderContentsOldAssets(false);
        //        GYAExt.Log("(" + count + ") Old Asset(s) reclaimed and moved to the '" +
        //                   GYA.gyaVars.pathOldAssetsFolderName + "' folder.");

        //        int countTmp = 0;

        //        // running even thou folder not exist!
        //        if (Directory.Exists(pathTmp))
        //        {
        //            countTmp += GYAPackage.GetAssetCountFromFolder(pathTmp);
        //            if (countTmp == 0)
        //            {
        //                DeleteEmptySubFolders(pathTmp);
        //                try
        //                {
        //                    if (Directory.GetFileSystemEntries(pathTmp).Length == 0)
        //                    {
        //                        Directory.Delete(pathTmp, false);
        //                        GYAExt.Log("Deleted: " + pathTmp);
        //                    }
        //                }
        //                catch (IOException ex)
        //                {
        //                    GYAExt.LogWarning("Error Cleaning: " + pathTmp, ex.Message);
        //                }
        //            }
        //        }

        //        countTmp = 0;
        //        if (Directory.Exists(pathTmp2))
        //        {
        //            countTmp += GYAPackage.GetAssetCountFromFolder(pathTmp2);
        //            if (countTmp == 0)
        //            {
        //                DeleteEmptySubFolders(pathTmp2);
        //                try
        //                {
        //                    if (Directory.GetFileSystemEntries(pathTmp2).Length == 0)
        //                    {
        //                        Directory.Delete(pathTmp2, false);
        //                        GYAExt.Log("Deleted: " + pathTmp2);
        //                    }
        //                }
        //                catch (IOException ex)
        //                {
        //                    GYAExt.LogWarning("Error Cleaning: \n" + pathTmp2, ex.Message);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        GYAExt.Log("No prior Old Asset Store folders found.");
        //    }
        //}

        // Create folder if not exist
        internal static void CreateFolder(string folder, bool silent = false)
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    // Default way
                    Directory.CreateDirectory(folder);

                    if (!silent)
                    {
                        GYAExt.Log("Created Folder:\t'" + folder + "'");
                    }
                }
            }
            catch (Exception ex)
            {
                GYAExt.LogError("Error Creating Folder:\t'" + folder + "'", ex.Message);
            }
        }

        internal static bool OldAssetNeedsToMove(List<GYAData.Asset> packages, int oldLine)
        {
            bool needsToMove = !(Path.GetFullPath(Path.GetDirectoryName(packages[oldLine].filePath)) ==
                                 GYA.gyaVars.pathOldAssetsFolder);
            return needsToMove;
        }

        //// Delete all matching files from folder, straighforward option
        //internal static void DeleteAllFilesInFolderWithExt (string pPath, string[] pExt)
        //{
        //	//var pPath = GYA.gyaVars.pathOldAssetsFolder;
        //	//var ext = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { ".unitypackage" };

        //	var ext = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { pExt };
        //	var files = new DirectoryInfo(pPath).GetFiles().Where(p => ext.Contains(p.Extension));
        //	foreach (var file in files)
        //	{
        //	    file.Attributes = FileAttributes.Normal;
        //	    File.Delete(file.FullName);
        //	}
        //}

        internal static void DeleteAllFilesInOldAssetsFolder(bool needsRefresh = true)
        {
            string filesInfo = String.Empty;
            int filesDeleted = 0;
            List<GYAData.Asset> packageDelete = GYA.gyaData.Assets.FindAll(x => x.collection == GYA.svCollection.Old &&
                                                                                x.filePath.Contains(
                                                                                    GYA.gyaVars.pathOldAssetsFolder,
                                                                                    StringComparison
                                                                                        .OrdinalIgnoreCase));

            foreach (GYAData.Asset t in packageDelete)
            {
// Delete asset
                var fileData = DeleteAsset(t);
                filesDeleted = filesDeleted + fileData.Key;
                filesInfo = filesInfo + fileData.Value.Split('\n')[1] + "\n";
            }
            if (filesDeleted > 0)
            {
                GYAExt.Log("( " + filesDeleted + " ) package(s) deleted from the Old Assets folder.",
                    filesInfo);
            }

            if (needsRefresh)
            {
                //GYAPackage.RefreshAllCollections();
                GYAPackage.RefreshOld(false);
                GYAPackage.RefreshStore(false);
                GYA.Instance.RefreshSV();
                GYAFile.SaveGYAAssets();
            }
        }

        // Delete the selected assets
        internal static void DeleteAssetMultiple(bool needsRefresh = true)
        {
            string filesInfo = String.Empty;
            int filesDeleted = 0;
            List<GYAData.Asset> packageDelete = GYA.gyaData.Assets.FindAll(x => x.isMarked);

            // Check all assets
            foreach (GYAData.Asset t in packageDelete)
            {
// Is asset marked
                if (t.isMarked)
                {
                    // Delete asset
                    var fileData = DeleteAsset(t);
                    filesDeleted = filesDeleted + fileData.Key;
                    filesInfo = filesInfo + fileData.Value.Split('\n')[1] + "\n";
                }
            }
            if (filesDeleted > 0)
                GYAExt.Log("( " + filesDeleted + " ) package(s) deleted.", filesInfo);

            // Make sure list is up-to-date
            if (needsRefresh)
                GYAPackage.RefreshAllCollections();
        }

        // Move assets to the passed folder - defaults to old assets
        internal static KeyValuePair<int, string> DeleteAsset(GYAData.Asset packageMove)
        {
            string moveInfo = String.Empty;
            int filesDeleted = 0;
            string fileToDelete = Path.GetFullPath(packageMove.filePath);

            // Does file already exist at destination?
            if (File.Exists(fileToDelete))
            {
                // Yes it is
                moveInfo = moveInfo + "Deleting: " + packageMove.title + "\n" + "Path: " + fileToDelete + "\n";

                try
                {
                    // Delete the file
                    File.Delete(fileToDelete);

                    //UnityEditor.MoveAssetToTrash(@fileToDelete);

                    //GYA.gyaData.Assets.Remove(packageMove);

                    // Verification
                    if (File.Exists(fileToDelete))
                    {
                        //moveInfo = moveInfo + "Error Deleting: " + packageMove.title + "\n" + "Path: " + fileToDelete + "\n";
                        GYAExt.LogWarning("Error: File not Deleted:", moveInfo);
                        //GYAExt.LogWarning(moveInfo, "Error: File not Deleted:");
                    }
                    else
                    {
                        GYA.gyaData.Assets.Remove(packageMove);
                        filesDeleted += 1;
                    }
                }
                catch (IOException ex)
                {
                    GYAExt.LogWarning("Error: File Delete Failed:", moveInfo + "\n\n" + ex.Message);
                }
            }
            else
            {
                GYAExt.Log("Unable to delete - File doesn't exist: " + fileToDelete);
            }
            return new KeyValuePair<int, string>(filesDeleted, moveInfo);
        }

        // Create full version suffix to append: v<Asset Version (<Unity Version>)
        internal static string GetAssetVersionToAppend(GYAData.Asset packageName)
        {
            // Asset version - Default
            string appendString = string.Empty;
            //string verString = packageName.version;
            //string uniString = packageName.unity_version;

            //string verString = ReturnValidFile(packageName.version);
            //string uniString = ReturnValidFile(packageName.unity_version);
            string verString = packageName.version;
            string uniString = packageName.unity_version;

            if (!packageName.isExported)
            {
                // Unity version tag missing or blank
                if (string.IsNullOrEmpty(uniString))
                    uniString = "";

                // Version string
                if (verString.Length > 0)
                    appendString = " v" + verString;

                if (uniString.Length > 0)
                    appendString = appendString + " (" + uniString + ")";
            }

            return ReturnValidFile(appendString);
        }

        internal static string GetTitleAssetVersionRemoved(GYAData.Asset packageName)
        {
            // Make asset title the filename
            //string filename = GYAFile.ReturnValidFile(packageName.title);
            string filename = packageName.title;
            filename = filename.Replace("\"", String.Empty);
            // Create full version suffix to append
            //string verString = GYAFile.ReturnValidFile(GetAssetVersionToAppend(packageName));
            string verString = GetAssetVersionToAppend(packageName);

            // Remove version
            if (verString.Length > 0)
                filename = filename.Replace(verString, "");

            return ReturnValidFile(filename);
        }

        internal static string GetTitleAssetVersionAppended(GYAData.Asset packageName, bool includeExtension = false)
        {
            // Make asset title the filename
            //string filename = GYAFile.ReturnValidFile(packageName.title);
            string filename = packageName.title;
            filename = filename.Replace("\"", String.Empty);

            // Create full version suffix to append
            //string verString = GYAFile.ReturnValidFile(GetAssetVersionToAppend(packageName));
            string verString = GetAssetVersionToAppend(packageName);

            // Add version and extension
            if (!filename.Contains(verString))
                filename = filename + verString;
            if (includeExtension)
                filename = filename + Path.GetExtension(packageName.filePath);

            return ReturnValidFile(filename);
        }

        // Move assets to folder - defaults to old assets - copyOverride forces a copy instead of move
        public static KeyValuePair<int, string> MoveAssetToPath(GYAData.Asset packageMove, string pathMoveAsset = null,
            bool copyOverride = false, bool quietMode = false, bool appendVer = true)
        {
            string moveInfo = String.Empty;
            int filesMoved = 0;

            //string filename = Path.GetFileNameWithoutExtension(packageMove.filePath);
            string filename = Path.GetFileName(packageMove.filePath);
            string pathMoveFrom = GYAExt.PathFixedForOS(Path.GetFullPath(packageMove.filePath));

            // Path to move to
            if (pathMoveAsset == null)
                pathMoveAsset = GYA.gyaVars.pathOldAssetsFolder;

            // Begin rename
            if (appendVer)
            {
                filename = GetTitleAssetVersionAppended(packageMove, true);

                // Check for invalid chars - Path
                pathMoveAsset = GYAFile.ReturnValidPath(pathMoveAsset);
                pathMoveAsset = Path.GetFullPath(Path.Combine(pathMoveAsset, filename));
            }

            // Does file already exist at destination?
            if (File.Exists(pathMoveAsset))
            {
                // Yes it is, do nothing
                if (!quietMode)
                    GYAExt.Log("File already exists: " + pathMoveAsset + "\nFile NOT copied !!");
            }
            else
            {
                // Create folder if required
                if (pathMoveAsset != null && !Directory.Exists(Path.GetDirectoryName(pathMoveAsset)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(pathMoveAsset));
                }

                // No, it's not
                string copyMoveTxt = "Move";
                if (copyOverride)
                    copyMoveTxt = "Copy";

                moveInfo = moveInfo + copyMoveTxt + ": " + pathMoveFrom + "\n" +
                           "To: " + pathMoveAsset + "\n";

                try
                {
                    // Move the file
                    try
                    {
                        if (File.Exists(pathMoveFrom))
                            File.SetAttributes(pathMoveFrom, FileAttributes.Normal);

                        if (copyOverride)
                        {
                            // Copy file
                            if (pathMoveAsset != null) File.Copy(pathMoveFrom, pathMoveAsset);
                        }
                        else
                        {
                            // Move file
                            if (GYAFile.IsSymLink(pathMoveFrom) || GYAFile.IsSymLink(pathMoveAsset))
                            {
                                // Must Copy/Delete because of symlink
                                File.Copy(pathMoveFrom, pathMoveAsset);
                                if (File.Exists(pathMoveAsset))
                                    File.Delete(pathMoveFrom);
                            }
                            else
                            {
                                // Can Move file
                                File.Move(pathMoveFrom, pathMoveAsset);
                            }
                        }
                    }
                    catch (IOException ex)
                    {
                        GYAExt.LogWarning(
                            "Error " + (copyOverride ? "Copying: " : "Moving: ") + pathMoveFrom + " to " +
                            pathMoveAsset, ex.Message, false);
                    }

                    // Verification
                    if (File.Exists(pathMoveAsset))
                    {
                        filesMoved += 1;
                    }
                    else
                    {
                        GYAExt.LogWarning("Error: File " + copyMoveTxt + " - Unable to locate file at the target path:",
                            moveInfo);
                    }
                }
                catch (IOException ex)
                {
                    GYAExt.LogWarning("Error: File " + copyMoveTxt + " - Failed:", moveInfo + "\n\n" + ex.Message);
                }
            }
            return new KeyValuePair<int, string>(filesMoved, moveInfo);
        }

        // Delete empty folders recursively & handle .DS_Store files
        internal static void DeleteEmptySubFolders(string startLocation)
        {
            if (Directory.Exists(startLocation))
            {
                foreach (var directory in Directory.GetDirectories(startLocation))
                {
                    try
                    {
                        // If exists ds_store, delete it if it's the only file in folder
                        if (Directory.GetFileSystemEntries(directory, ".DS_Store").Length == 1)
                        {
                            //File.SetAttributes(Path.Combine(directory, ".DS_Store"), FileAttributes.Normal);
                            File.Delete(Path.Combine(directory, ".DS_Store"));
                        }

                        DeleteEmptySubFolders(directory);
						if (Directory.GetFileSystemEntries (directory).Length == 0)
						{
							Directory.Delete (directory, false);
							GYAExt.Log ("Deleted Empty Sub Folders from the Asset Store folder: " + startLocation);
						}
					}
                    catch (IOException ex)
                    {
                        GYAExt.LogWarning("Error: DeleteEmptySubFolders Failed: " + directory, ex.Message, false);
                    }
                }
            }
        }

        //public static void ExtractToSelected(String unityPackageFileName, String targetFolder = "", bool packageNameAsFolder = true, bool overwrite = false, bool includeMeta = true)
        //internal static void ExtractToSelected(string path)
        //{
        //    //int filesMoved = 0;
        //    //string filesInfo = String.Empty;

        //    // Check all old assets
        //    int stepNumber = 0;
        //    GYAData.Asset packageToCopy = null;
        //    using (
        //        var progressBar = new GYA.ProgressBar(
        //            string.Format("{0} Extracting Selected Package(s)", GYA.gyaVars.abbr),
        //            GYAImport.CountToImport(),
        //            0,
        //            _ => Path.GetFileName(packageToCopy.filePath)
        //        )
        //    )
        //        foreach (GYAData.Asset t in GYA.gyaData.Assets)
        //        {
        //            // Is asset not in the old asset folder
        //            if (t.isMarked)
        //            {
        //                packageToCopy = t;
        //                progressBar.Update(stepNumber++);

        //                // Extract asset
        //                //var fileData = MoveAssetToPath(GYA.gyaData.Assets[i], path, true);
        //                //filesMoved = filesMoved + fileData.Key;
        //                //filesInfo = filesInfo + fileData.Value;

        //                //GYAExtract.ExtractUnityPackage(GYA.gyaData.Assets[i].filePath, path, false, true, true);
        //            }
        //        }
        //    //if (filesMoved > 0)
        //    //	GYAExt.Log("( " + filesMoved.ToString() + " ) package(s) copied to: " + path. filesInfo, false);
        //}

        // Move marked assets to the prescribed folder
        internal static void CopyToSelected(string path)
        {
            int filesMoved = 0;
            string filesInfo = String.Empty;

            // Check all old assets
            int stepNumber = 0;
            GYAData.Asset packageToCopy = null;
            using (
                var progressBar = new GYA.ProgressBar(
                    string.Format("{0} Copying Selected Package(s)", GYA.gyaVars.abbr),
                    GYAImport.CountToImport(),
                    0,
                    _ => Path.GetFileName(packageToCopy.filePath)
                )
            )
                foreach (GYAData.Asset t in GYA.gyaData.Assets)
                {
// Is asset not in the old asset folder
                    if (t.isMarked)
                    {
                        packageToCopy = t;
                        progressBar.Update(stepNumber++);

                        // Move asset
                        var fileData = MoveAssetToPath(t, path, true);
                        filesMoved = filesMoved + fileData.Key;
                        filesInfo = filesInfo + fileData.Value;
                    }
                }
            if (filesMoved > 0)
                GYAExt.Log("( " + filesMoved + " ) package(s) copied to: " + path, filesInfo, false);

            //RefreshPackages();
        }

        // Rename asset to include version
        internal static void RenameWithVersion(object package)
        {
            GYAData.Asset pObject = (GYAData.Asset) package;
            RenameWithVersion(pObject, true);
        }

        // Rename asset to include version
        internal static void RenameWithVersion(GYAData.Asset package, bool showResults = true)
        {
            int filesMoved = 0;
            string filesInfo = String.Empty;

            if (!package.isExported)
            {
                // Move asset
                var fileData = MoveAssetToPath(package, Path.GetDirectoryName(package.filePath), false, false, true);
                filesMoved = filesMoved + fileData.Key;
                filesInfo = filesInfo + fileData.Value;

                //RefreshPackages();
            }

            if (showResults && filesMoved > 0)
            {
                GYAExt.Log(GYA.gyaVars.abbr + " - Package renamed:", filesInfo, false);
            }
        }

        // Rename selected to include version
        internal static void RenameWithVersionSelected()
        {
            int filesMoved = 0;
            string filesInfo = String.Empty;

            // Check all old assets
            foreach (GYAData.Asset t in GYA.gyaData.Assets)
            {
                if (t.isMarked)
                {
                    // Move asset
                    var fileData = MoveAssetToPath(t,
                        Path.GetDirectoryName(t.filePath), false, true, true);
                    filesMoved = filesMoved + fileData.Key;
                    filesInfo = filesInfo + fileData.Value;
                }
            }
            if (filesMoved > 0)
            {
                GYAExt.Log("( " + filesMoved + " ) package(s) renamed.", filesInfo, false);
                // Make sure list is up-to-date
                GYAPackage.RefreshAllCollections();
            }
        }

        // Rename AS assets to include version
        internal static void RenameWithVersionCollection()
        {
            int filesMoved = 0;
            string filesInfo = String.Empty;

            // Check all old assets
            foreach (GYAData.Asset t in GYA.gyaData.Assets)
            {
                if (t.collection == GYA.svCollection.Store)
                {
                    // Move asset
                    var fileData = MoveAssetToPath(t,
                        Path.GetDirectoryName(t.filePath), false, true);
                    filesMoved = filesMoved + fileData.Key;
                    filesInfo = filesInfo + fileData.Value;
                }
            }
            if (filesMoved > 0)
            {
                GYAExt.Log("( " + filesMoved + " ) package(s) protected.");
            }
            //RefreshPackages();
        }

        // Move old assets to the prescribed folder
        internal static void OldAssetsMove(bool needsRefresh = true)
        {
            int filesMoved = 0;
            string filesInfo = String.Empty;

            // Check all old assets
            foreach (GYAData.Asset t in GYA.gyaData.Assets)
            {
                if (t.isOldToMove)
                {
                    // Move asset
                    var fileData = MoveAssetToPath(t);
                    filesMoved = filesMoved + fileData.Key;
                    filesInfo = filesInfo + fileData.Value;
                }
            }
            if (filesMoved > 0)
            {
                //gyaVars.FilesCount.old += filesMoved;
                GYAExt.Log("( " + filesMoved + " ) package(s) moved to the Old Assets Folder.", filesInfo,
                    false);
            }
            // Make sure list is up-to-date
            if (needsRefresh)
            {
                //GYAPackage.RefreshPackages();
                GYAPackage.RefreshStore(false);
                GYAPackage.RefreshOld(false);
                GYA.Instance.RefreshSV();
            }
        }

        //internal static void CheckSymlink()
        //{
        //    GYA.gyaVars.isSymLinkPathAS5 = IsSymLink(GYAExt.PathUnityAssetStore5);
        //    GYA.gyaVars.symlinkTargetAS5 = GetSymLinkTarget(GYAExt.PathUnityAssetStore5);
        //}

        [DllImport("UnityEditor")]
        internal static extern void MoveAssetToTrash(string path);

        //public static void MoveAssetToTrash(string fileToDelete)
        //{
        //	_MoveAssetToTrash(@fileToDelete);
        //}

        internal const int FILE_SHARE_READ = 1;
        internal const int FILE_SHARE_WRITE = 2;

        internal const int CREATION_DISPOSITION_OPEN_EXISTING = 3;
        internal const int FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

        [DllImport("kernel32.dll", EntryPoint = "GetFinalPathNameByHandleW", CharSet = CharSet.Unicode,
            SetLastError = true)]
        public static extern int GetFinalPathNameByHandle(IntPtr handle, [In, Out] StringBuilder path, int bufLen,
            int flags);

        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode,
            IntPtr SecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

        // Return target path is a symlink
        // OR .. return the unmodified file path if not a symlink or not Win OS
        //public static string GetSymLinkTarget(string symlink)
        //{
        //    if (Directory.Exists(symlink))
        //    {
        //        DirectoryInfo folderPath = new DirectoryInfo(symlink);
        //        return GetSymLinkTarget(folderPath);
        //    }

        //    return ""; // Doesn't exist
        //}

        //public static string GetSymLinkTarget(DirectoryInfo symlink)
        //{
        //    // If Windows
        //    if (GYAExt.IsOSWin)
        //    {
        //        SafeFileHandle directoryHandle = CreateFile(symlink.FullName, 0, 2, IntPtr.Zero,
        //            CREATION_DISPOSITION_OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero);
        //        if (directoryHandle.IsInvalid)
        //        {
        //            GYAExt.LogError("Path Is Invalid: " + symlink,
        //                "Make sure the path is currently accessible and try again.");
        //            throw new Win32Exception(Marshal.GetLastWin32Error());
        //        }

        //        StringBuilder path = new StringBuilder(512);
        //        int size = GetFinalPathNameByHandle(directoryHandle.DangerousGetHandle(), path, path.Capacity, 0);
        //        if (size < 0)
        //            throw new Win32Exception(Marshal.GetLastWin32Error());

        //        // The remarks section of GetFinalPathNameByHandle mentions the return being prefixed with "\\?\"
        //        // More information about "\\?\" .. http://msdn.microsoft.com/en-us/library/aa365247(v=VS.85).aspx

        //        if (path[0] == '\\' && path[1] == '\\' && path[2] == '?' && path[3] == '\\')
        //        {
        //            return path.ToString().Substring(4);
        //        }

        //        return path.ToString();
        //    }

        //    // If NOT Windows return path string
        //    return symlink.FullName;
        //}

        // TRUE if a symlink/reparse point
        public static bool IsSymLink(string path)
        {
            bool pathBool = false; // Default is NOT a symlink
            try
            {
                if (File.Exists(path) || Directory.Exists(path))
                {
                    if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                        pathBool = true;
            	}
            }
            catch (UnauthorizedAccessException ex)
            {
                GYAExt.LogWarning("IsSymLink UnauthorizedAccessException:", ex.Message);
            }
            catch (Exception ex)
            {
                GYAExt.LogWarning("IsSymLink Exception:", ex.Message);
            }
            return pathBool;
        }

        public static FileAttributes RemoveFileAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

        public static string ReturnValidPath(string pText)
        {
            string invalidChars = new string(GYAFile.GetInvalidPathChars());
            var validChars = pText.Where(x => !invalidChars.Contains(x)).ToArray();
            return new string(validChars);
        }

        public static string ReturnValidFile(string pText)
        {
            //string invalidChars = new string(GetInvalidFileNameChars());
            //var validChars = pText.Where(x => !invalidChars.Contains(x)).ToArray();
            //return new string(validChars);

            //System.Collections.Generic.HashSet<char> validChars = new System.Collections.Generic.HashSet<char>() { GetInvalidFileNameChars().ToArray() };
            System.Collections.Generic.HashSet<char> validChars = GetInvalidFileNameChars().ToHashSet();
            string s = pText;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
                if (!validChars.Contains(s[i]))
                    sb.Append(s[i]);

            //Debug.Log("Nice Name: " + sb.ToString());
            return sb.ToString();
        }

        public static bool IsValidFileName(string filename)
        {
            string text = GYAFile.RemoveInvalidCharsFromFileName(filename, false);
            return text == filename && !string.IsNullOrEmpty(text);
        }

        public static string RemoveInvalidCharsFromFileName(string filename, bool logIfInvalidChars = false)
        {
            if (string.IsNullOrEmpty(filename))
                return filename;

            filename = filename.Trim();
            if (string.IsNullOrEmpty(filename))
                return filename;

            string text = new string(GYAFile.GetInvalidFileNameChars());
            string text2 = string.Empty;
            bool flag = false;
            string text3 = filename;
            foreach (char c in text3)
            {
                if (text.IndexOf(c) == -1)
                {
                    text2 += c;
                }
                else
                {
                    flag = true;
                }
            }
            if (flag && logIfInvalidChars)
            {
                string displayStringOfInvalidCharsOfFileName =
                    GYAFile.GetDisplayStringOfInvalidCharsOfFileName(filename);
                if (displayStringOfInvalidCharsOfFileName.Length > 0)
                {
                    GYAExt.LogWarning("A filename cannot contain the following character(s): " +
                                      displayStringOfInvalidCharsOfFileName);
                }
            }
            return text2;
        }

        public static string GetDisplayStringOfInvalidCharsOfFileName(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return string.Empty;

            string text = new string(GYAFile.GetInvalidFileNameChars());
            string text2 = string.Empty;
            foreach (char c in filename)
            {
                if (text.IndexOf(c) >= 0 && text2.IndexOf(c) == -1)
                {
                    if (text2.Length > 0)
                        text2 += " ";

                    text2 += c;
                }
            }
            return text2;
        }

        //public static string[] PackageStorePath (string publisher_name, string category_name, string package_name, string package_id, string url)
        //{
        //	string InvalidPathCharsRegExp = new System.Text.RegularExpressions.Regex ("[^a-zA-Z0-9() _-]");

        //	string[] array = new string[] {
        //		publisher_name,
        //		category_name,
        //		package_name
        //	};
        //	for (int i = 0; i < 3; i++) {
        //		array [i] = InvalidPathCharsRegExp.Replace (array [i], "");
        //	}
        //	if (array [2] == "") {
        //		array [2] = InvalidPathCharsRegExp.Replace (package_id, "");
        //	}
        //	if (array [2] == "") {
        //		array [2] = InvalidPathCharsRegExp.Replace (url, "");
        //	}
        //	return array;
        //}

        public static char[] GetInvalidFileNameChars()
        {
            if (GYAExt.IsOSWin)
            {
                return new[]
                {
                    '\0',
                    '\u0001',
                    '\u0002',
                    '\u0003',
                    '\u0004',
                    '\u0005',
                    '\u0006',
                    '\a',
                    '\b',
                    '\t',
                    '\n',
                    '\v',
                    '\f',
                    '\r',
                    '\u000e',
                    '\u000f',
                    '\u0010',
                    '\u0011',
                    '\u0012',
                    '\u0013',
                    '\u0014',
                    '\u0015',
                    '\u0016',
                    '\u0017',
                    '\u0018',
                    '\u0019',
                    '\u001a',
                    '\u001b',
                    '\u001c',
                    '\u001d',
                    '\u001e',
                    '\u001f',
                    '"',
                    '<',
                    '>',
                    '|',
                    ':',
                    '*',
                    '?',
                    '\\',
                    '/'
                };
            }
            return new[]
            {
                '\0',
                '/',
                '|',
                ':',
                '*'
            };
        }

        public static char[] GetInvalidPathChars()
        {
            if (GYAExt.IsOSWin)
            {
                return new[]
                {
                    '"',
                    '<',
                    '>',
                    '|',
                    '\0',
                    '\u0001',
                    '\u0002',
                    '\u0003',
                    '\u0004',
                    '\u0005',
                    '\u0006',
                    '\a',
                    '\b',
                    '\t',
                    '\n',
                    '\v',
                    '\f',
                    '\r',
                    '\u000e',
                    '\u000f',
                    '\u0010',
                    '\u0011',
                    '\u0012',
                    '\u0013',
                    '\u0014',
                    '\u0015',
                    '\u0016',
                    '\u0017',
                    '\u0018',
                    '\u0019',
                    '\u001a',
                    '\u001b',
                    '\u001c',
                    '\u001d',
                    '\u001e',
                    '\u001f'
                };
            }
            return new char[1];
        }

        internal static void CopyFileIfExists(string src, string dst, bool overwrite)
        {
            if (File.Exists(src))
                GYAFile.UnityFileCopy(src, dst, overwrite);
        }

        internal static void CreateOrCleanDirectory(string dir)
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);

            Directory.CreateDirectory(dir);
        }

        //internal static List<string> GetAllFilesRecursive(string path)
        //{
        //    List<string> files = new List<string>();
        //    WalkFilesystemRecursively(path, delegate(string p) { files.Add(p); }, p => true);
        //    return files;
        //}

        internal static void MoveFileIfExists(string src, string dst)
        {
            if (File.Exists(src))
            {
                FileUtil.DeleteFileOrDirectory(dst);
                FileUtil.MoveFileOrDirectory(src, dst);
                //File.SetLastWriteTime (dst, DateTime.get_Now ());
            }
        }

        internal static string NiceWinPath(string unityPath)
        {
            //return (Application.platform != RuntimePlatform.WindowsEditor) ? unityPath : unityPath.Replace("/", "\\");
            return (Application.platform != RuntimePlatform.WindowsEditor) ? unityPath.Replace("\\", "/") : unityPath.Replace("/", "\\");
        }

        internal static string RemovePathPrefix(string fullPath, string prefix)
        {
            string[] array = fullPath.Split(Path.DirectorySeparatorChar);
            string[] array2 = prefix.Split(Path.DirectorySeparatorChar);

            int num = 0;
            if (array[0] == string.Empty)
                num = 1;

            while (num < array.Length && num < array2.Length && array[num] == array2[num])
            {
                num++;
            }
            if (num == array.Length)
                return string.Empty;

            char directorySeparatorChar = Path.DirectorySeparatorChar;
            return string.Join(directorySeparatorChar.ToString(), array, num, array.Length - num);
        }

        internal static void ReplaceTextInFile(string path, params string[] input)
        {
            path = GYAFile.NiceWinPath(path);
            string[] array = File.ReadAllLines(path);
            for (int i = 0; i < input.Length; i += 2)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = array[j].Replace(input[i], input[i + 1]);
                }
            }
            File.WriteAllLines(path, array);
        }

        internal static void UnityFileCopy(string from, string to)
        {
            GYAFile.UnityFileCopy(from, to, false);
        }

        internal static void UnityFileCopy(string from, string to, bool overwrite)
        {
            File.Copy(GYAFile.NiceWinPath(from), GYAFile.NiceWinPath(to), overwrite);
        }

        //internal static void WalkFilesystemRecursively(string path, Action<string> fileCallback,
        //    Func<string, bool> directoryCallback)
        //{
        //    string[] files = Directory.GetFiles(path);
        //    foreach (string text in files)
        //    {
        //        fileCallback.Invoke(text);
        //    }
        //    string[] directories = Directory.GetDirectories(path);
        //    foreach (string text2 in directories)
        //    {
        //        if (directoryCallback(text2))
        //            GYAFile.WalkFilesystemRecursively(text2, fileCallback, directoryCallback);
        //    }
        //}

        //internal static bool AppendTextAfter (string path, string find, string append)
        //{
        //	bool result = false;
        //	path = GYAFile.NiceWinPath (path);
        //	List<string> list = new List<string> (File.ReadAllLines (path));
        //	for (int i = 0; i < list.get_Count (); i++)
        //	{
        //		if (list.get_Item (i).Contains (find))
        //		{
        //			list.Insert (i + 1, append);
        //			result = true;
        //			break;
        //		}
        //	}
        //	File.WriteAllLines (path, list.ToArray ());
        //	return result;
        //}

        internal static bool Regex_ReplaceTextInFile(string path, params string[] input)
        {
            bool result = false;
            path = GYAFile.NiceWinPath(path);
            string[] array = File.ReadAllLines(path);
            for (int i = 0; i < input.Length; i += 2)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    string text = array[j];
                    array[j] = System.Text.RegularExpressions.Regex.Replace(text, input[i], input[i + 1]);
                    if (text != array[j])
                    {
                        result = true;
                    }
                }
            }
            File.WriteAllLines(path, array);
            return result;
        }
    }
}