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

using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.ComponentModel;
using System.Reflection;
//using System.Reflection.Emit;
//using System.Runtime.InteropServices;
using GYAInternal.Json;

namespace XeirGYA
{
    // -- UnityEditor AssetStore methods
    public class GYAReflect : MonoBehaviour
    {
        static BindingFlags _flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                     BindingFlags.Static;

        static readonly BindingFlags _flagsDeclared = _flags | BindingFlags.DeclaredOnly;

        // Package List RAW - Downloaded AS Packages
        // Use AssetStoreContext.GetPackageList instead
        public static object UASGet_PackageInfo_GetPackageList()
        {
            var result = GYAReflect.MI_Invoke("UnityEditor.PackageInfo", "GetPackageList");
            GYAExt.LogAsJson(result);

            //Debug.Log(result[0]);

            return result;
        }

        // Package List - Downloaded AS Packages - 7.1s for initial update, subsequent request are cached
        public static IEnumerator UASGet_AssetStoreContext_GetPackageList()
        {
            var result = GYAReflect.MI_Invoke("UnityEditor.AssetStoreContext", "GetPackageList");
            yield return null;

            GYAReflect.SaveJSONToFile(result, GYA.gyaVars.Files.ASPackage.file);
            GYAFile.LoadASPackages();

            //GYAExt.LogAsJson(result);
        }

        // Create or over-write the json file for Unity Asset Store Download List
        internal static void SaveJSONToFile(object objToSave, string toFile)
        {
            string file = toFile;
            TextWriter writer = null;

            try
            {
                if (File.Exists(file))
                    File.SetAttributes(file, FileAttributes.Normal);

                var contentsToWriteToFile = JsonConvert.SerializeObject(objToSave, Formatting.Indented);
                writer = new StreamWriter(file, false);
                writer.Write(contentsToWriteToFile);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(GYA.gyaVars.abbr + " - SaveJSONToFile Error: " + ex.Message);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        // Return the value of a Property/Field/Method
        public static object GetVal(string tName, string fName)
        {
            Type type = FindType(tName);
            var members = type.GetMember(fName, _flags);

            // Property
            if (members[0].MemberType.ToString() == "Property")
            {
                return type.GetProperty(fName, _flags).GetValue(null, null);
            }
            // Field
            if (members[0].MemberType.ToString() == "Field")
            {
                return type.GetField(fName, _flags).GetValue(null);
            }
            // Method - Here in case try to use GetVal to access a method that returns a value
            if (members[0].MemberType.ToString() == "Method")
            {
                return GYAReflect.MI_Invoke(tName, fName);
            }

            return null;
        }

        // Set the value of a Property/Field
        public static void SetVal(string tName, string fName, object value)
        {
            Type type = FindType(tName);
            var members = type.GetMember(fName, _flags);

            var getVal = GetVal(tName, fName);

            // Property
            if (members[0].MemberType.ToString() == "Property")
            {
                // Get Value
                var pi = type.GetProperty(fName, _flags);
                pi.SetValue(getVal, Convert.ChangeType(value, pi.PropertyType), null);
            }
            // Field
            if (members[0].MemberType.ToString() == "Field")
            {
                // Get Value
                var fi = type.GetField(fName, _flags);
                fi.SetValue(getVal, Convert.ChangeType(value, fi.FieldType));
            }
        }

        // Call method by string name
        public void MethodByName(string methodName)
        {
            string mName = methodName; // Optional string extension if desired + "OptText"
            MethodInfo info = GetType().GetMethod(mName, _flags);
            StartCoroutine((IEnumerator) info.Invoke(this, null));
        }

        // Example: GYAExt.LogAsJson(GYAImport.MI_Create("UnityEditor.AssetStoreAsset", "AssetStoreAsset"));
        public static object MI_Create(string tName, string mName)
        {
            Assembly Core = typeof(EditorWindow).Assembly;
            Type miType = Core.GetType(tName);
            object miObj = Activator.CreateInstance(miType);
            //MethodInfo mi = miType.GetMethod(mName, _flags);

            //var miResult = mi.Invoke( miObj, null );
            //GYAExt.LogAsJson(miResult);
            return miObj;
        }

        public static object MI_CreateSO(string tName, string mName)
        {
            Assembly Core = typeof(EditorWindow).Assembly;
            Type miType = Core.GetType(tName);
            //object miObj = Activator.CreateInstance(miType);
            object miObj = ScriptableObject.CreateInstance(miType);
            //MethodInfo mi = miType.GetMethod(mName, _flags);

            //var miResult = mi.Invoke( miObj, null );
            //GYAExt.LogAsJson(miResult);
            return miObj;
        }

        // Example: GYAExt.LogAsJson(GYAImport.MI_Invoke("UnityEditor.AssetStoreContext", "GetPackageList"));
        public static object MI_Invoke(string tName, string mName)
        {
            Assembly Core = typeof(EditorWindow).Assembly;
            Type miType = Core.GetType(tName);

            //Type miType = FindType(tName);
            //GYAExt.LogAsJson(miType);
            // If static bypass instance
            object miObj = Activator.CreateInstance(miType);
            MethodInfo mi = miType.GetMethod(mName, _flags);

            //GYAExt.LogAsJson(mi);

            if (mi == null)
            {
                //Debug.LogWarning("MI_Invoke: NULL (" + tName + "." + mName + ")\n");
                return null;
            }

            // Instance
            var miResult = mi.Invoke(miObj, null);
            // Static
            //var miResult = mi.Invoke( null, null );

            //GYAExt.LogAsJson(miResult);
            return miResult;
        }

        // Example: GYAExt.LogAsJson(GYAImport.MI_Invoke("UnityEditor.AssetStoreContext", "SessionHasString", "id"));
        public static object MI_Invoke(string tName, string mName, object pParams)
        {
            object[] miParams = {pParams};

            Assembly Core = typeof(EditorWindow).Assembly;
            Type miType = Core.GetType(tName);

            //Type miType = FindType(tName);
            //GYAExt.LogAsJson(miType);
            MethodInfo mi = miType.GetMethod(mName,
                _flags, Type.DefaultBinder,
                new[] {typeof(string)},
                null
            );

            //MethodInfo mi =
            //	typeof(EditorWindow).Assembly.GetType(tName).GetMethod(mName,
            //	_flags, Type.DefaultBinder,
            //	new[] { typeof(string) },
            //	null
            //	);

            if (mi == null)
            {
                //Debug.LogWarning("MI_Invoke: NULL (" + tName + "." + mName + ")\n");
                return null;
            }

            var miResult = mi.Invoke(null, miParams);
            return miResult;
        }

        // Example: GYAExt.LogAsJson(GYAImport.MI_InvokeSO("UnityEditor.AssetStoreAssetInspector", "GetInfoString"));
        public static object MI_InvokeSO(string tName, string mName)
        {
            Assembly Core = typeof(EditorWindow).Assembly;
            Type miType = Core.GetType(tName);
            var miObj = ScriptableObject.CreateInstance(miType);
            MethodInfo mi = miType.GetMethod(mName, _flags);

            if (mi == null)
            {
                //Debug.LogWarning("MI_InvokeSO: NULL (" + tName + "." + mName + ")\n");
                return null;
            }
            var miResult = mi.Invoke(miObj, null);
            return miResult;
        }

        public static object[] CreateObjectArray(params object[] args)
        {
            return args;
        }

        public static String GetAssemblyNameContainingType(String typeName)
        {
            foreach (Assembly currentassembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type t = currentassembly.GetType(typeName, false, true);
                if (t != null)
                {
                    return currentassembly.FullName;
                }
            }

            return "not found";
        }

        public static string GetAssemblyLocationOfObject(object o)
        {
            return Assembly.GetAssembly(o.GetType()).Location;
        }

        // Find assembly by given string name - ie. "<UnityEditor>.AssetStoreClient"
        //public static Assembly FindAssembly(string typeName)
        //{
        //	var type = Type.GetType(typeName);
        //	if (type != null) return type;
        //	foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        //	{
        //		type = asm.GetType(typeName);
        //		if (type != null)
        //			return asm;
        //	}
        //	return null ;
        //}

        // Find type by given string name - ie. "UnityEditor.<AssetStoreClient>"
        public static Type FindType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        // Invoke optional package - CodeStage.PackageToFolder
        //public static object PackageToFolder (string tName, string mName, params object[] pParams)
        public static object PackageToFolder(params object[] pParams)
        {
            var tName = "CodeStage.PackageToFolder.Package2Folder";
            var mName = "ImportPackageToFolder";

            Type miType = FindType(tName);
            MethodInfo mi = miType.GetMethod(mName, _flags);

            if (mi == null)
            {
                GYAExt.Log("PackageToFolder: NULL (" + tName + "." + mName + ")\n");
                return null;
            }

            var miResult = mi.Invoke(null, pParams);
            return miResult;
        }

        // Get the field names of the class
        public static void FI_GetFieldNamesOfClass(string s)
        {
            StringBuilder OutputText = new StringBuilder();

            Type t = Type.GetType(s);
            FieldInfo[] _fields = t.GetFields(_flagsDeclared);
            // Show field info
            OutputText.AppendLine("# of fields:" + _fields.Length);
            foreach (FieldInfo fi in _fields)
            {
                OutputText.AppendLine(fi.Name);
            }
            Debug.Log(OutputText);
        }

        public static IEnumerable<FieldInfo> MI_GetAllFields(Type t)
        {
            if (t == null)
                return Enumerable.Empty<FieldInfo>();

            return t.GetFields(_flagsDeclared).Concat(MI_GetAllFields(t.BaseType));
        }

        // Print properties of a Type ie- "UnityEditor.ImportPackageItem,UnityEditor"
        public static void MI_GetTypeProperties(Type t)
        {
            StringBuilder OutputText = new StringBuilder();

            OutputText.AppendLine("Analysis of type " + t.Name);
            OutputText.AppendLine("Type Name: " + t.Name);
            OutputText.AppendLine("Full Name: " + t.FullName);
            OutputText.AppendLine("Namespace: " + t.Namespace);

            Type tBase = t.BaseType;
            if (tBase != null)
            {
                OutputText.AppendLine("Base Type: " + tBase.Name);
            }

            Type tUnderlyingSystem = t.UnderlyingSystemType;
            if (tUnderlyingSystem != null)
            {
                OutputText.AppendLine("UnderlyingSystem Type: " + tUnderlyingSystem.Name);
            }

            OutputText.AppendLine("Is Abstract Class: " + t.IsAbstract);
            OutputText.AppendLine("Is Array: " + t.IsArray);
            OutputText.AppendLine("Is Class: " + t.IsClass);
            OutputText.AppendLine("Is a COM Object : " + t.IsCOMObject);

            OutputText.AppendLine("\nPUBLIC MEMBERS:");
            MemberInfo[] Members = t.GetMembers();

            foreach (MemberInfo NextMember in Members)
            {
                OutputText.AppendLine(NextMember.DeclaringType + " " +
                                      NextMember.MemberType + "  " + NextMember.Name);
            }
            Debug.Log(OutputText);
        }

        public static bool HasMethod(object objectToCheck, string methodName)
        {
            var type = objectToCheck.GetType();
            return type.GetMethod(methodName, _flags) != null;
        }

        public static bool NamespaceExists(string namespaceToCheck)
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.Namespace == namespaceToCheck
                select type).Any();
        }


        //
        // -- NOT USED
        //

        public static object MI_InvokeArgs(bool isStatic, string tName, string mName,
            params object[] args) //object parameters)
        {
            object result = null;
            object obj = null;
            //object[] parameters = new object[] { "Test", "Message" };

            Assembly asm = typeof(EditorWindow).Assembly;
            Type miType = asm.GetType(tName);
            if (!isStatic) obj = Activator.CreateInstance(miType);
            //object obj = Activator.CreateInstance(type, _flags, null, args, null);
            //MethodInfo mi = type.GetMethod(mName, _flags);

            //MethodInfo mi =
            //	typeof(EditorWindow).Assembly.GetType(tName).GetMethod(mName, _flags, Type.DefaultBinder,
            //	new[] { typeof(parameters) },
            //	null
            //	);

            if (args.Length == 0)
            {
                MethodInfo method = miType.GetMethod(mName, _flags);
                result = method.Invoke(isStatic ? null : obj, null);
            }
            else
            {
                Type[] types = new Type[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == null)
                        args[i] = string.Empty;
                    types[i] = args[i].GetType();
                }
                MethodInfo method = miType.GetMethod(mName, _flags, null, types, null);
                result = method.Invoke(isStatic ? null : obj, args);
            }

            //GYAExt.LogAsJson(mi);

            //if (mi == null) {
            //	//Debug.LogWarning("MI_Invoke: NULL (" + tName + "." + mName + ")\n");
            //	return null;
            //}
            //var miResult = mi.Invoke( obj, null );
            //GYAExt.LogAsJson(miResult);
            return result;
        }

        //string z = Wrapper.callMethod<string>("Concatenate", "Old", "Mac", "Donald");
        //bool? result = Wrapper.callMethod<bool>("DoSomething", 115, "Foobar", false);
        public static T callMethod<T>(string tName, string methodName, params System.Object[] args)
        {
            System.Object res = invokeMethod(tName, methodName, args);
            if (res != null)
                return (T) res;
            return default(T);
        }

        public static object invokeMethod(string tName, string methodName, params object[] args)
        {
            object result = null;

            //Object[] evidence = {new Zone(SecurityZone.Internet)};
            //Evidence assemblyEvidence = new Evidence(evidence, null);
            //AppDomain appDomain = AppDomain.CreateDomain("myDomain");
            //Assembly assm = Assembly.LoadFile("C:\\myDir\\myAssembly.dll");
            //Type objType = assm.GetType("myNamespace.myClass");

            Assembly asm = typeof(EditorWindow).Assembly;
            Type type = asm.GetType(tName);

            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            object myClass = constructor.Invoke(new object[] { });
            if (args.Length == 0)
            {
                MethodInfo method = type.GetMethod(methodName);
                result = method.Invoke(myClass, null);
            }
            else
            {
                Type[] types = new Type[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == null)
                        args[i] = string.Empty;
                    types[i] = args[i].GetType();
                }
                MethodInfo method = type.GetMethod(methodName, types);
                result = method.Invoke(myClass, args);
            }
            //AppDomain.Unload(appDomain);
            return result;
        }

        public static void CallMethodInstance(string mName)
        {
            Type type = typeof(EditorWindow);
            object obj = Activator.CreateInstance(type);
            MethodInfo methodInfo = type.GetMethod(mName, _flags);
            methodInfo.Invoke(obj, null);
        }

        //If the functions you are going to call are static you don't need an instance of the type:
        public static void CallMethodStatic(string mName)
        {
            Type type = typeof(EditorWindow);
            MethodInfo methodInfo = type.GetMethod(mName, _flags);
            methodInfo.Invoke(null, null);
        }

        public static List<KeyValuePair<string, string>> GetProperties(object item) //where T : class
        {
            var result = new List<KeyValuePair<string, string>>();
            if (item != null)
            {
                var type = item.GetType();
                var properties = type.GetProperties(_flagsDeclared);
                result.AddRange(from pi in properties
                    let selfValue = type.GetProperty(pi.Name).GetValue(item, null)
                    select selfValue != null
                        ? new KeyValuePair<string, string>(pi.Name, selfValue.ToString())
                        : new KeyValuePair<string, string>(pi.Name, null));
            }
            return result;
        }
    }

    public class GYACoroutine
    {
        public static GYACoroutine start(IEnumerator _routine)
        {
            GYACoroutine coroutine = new GYACoroutine(_routine);
            coroutine.start();
            return coroutine;
        }

        readonly IEnumerator routine;

        GYACoroutine(IEnumerator _routine)
        {
            routine = _routine;
        }

        public void start()
        {
            EditorApplication.update += update;
        }

        public void stop()
        {
            EditorApplication.update -= update;
        }

        public void update()
        {
            if (!routine.MoveNext())
            {
                stop();
            }
        }
    }
}