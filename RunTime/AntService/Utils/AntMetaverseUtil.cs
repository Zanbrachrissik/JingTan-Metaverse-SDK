using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Ant.Metaverse
{
    public class AntMetaverseUtil{
        public static Dictionary<string, Assembly> AssemblyDic;
        public static Dictionary<int, Type> TypeDic;
        public static Dictionary<int, MethodInfo> MethodInfoDic;

        public static Assembly GetLoadedAssembly(string assemblyName)
        {
            if(AssemblyDic == null){
                InitAssemblyDic();
            }
            AssemblyDic.TryGetValue(assemblyName, out Assembly assembly);
            return assembly;
        }

        public static MethodInfo GetMethodInfo(string assemblyName, string typeName, string methodName)
        {
            Assembly assembly = GetLoadedAssembly(assemblyName);
            if (assembly == null)
            {
                return null;
            }
            Type type = GetLoadedType(assembly, typeName);
            if (type == null)
            {
                return null;
            }
            int hashCode = (assembly.GetName().Name + typeName + methodName).GetHashCode();
            if (MethodInfoDic == null)
            {
                MethodInfoDic = new Dictionary<int, MethodInfo>();
            }
            if (MethodInfoDic.TryGetValue(hashCode, out MethodInfo methodInfo))
            {
                return methodInfo;
            }
            methodInfo = type.GetMethod(methodName);
            if (methodInfo == null)
            {
                return null;
            }
            MethodInfoDic[hashCode] = methodInfo;
            return methodInfo;
        }

        public static Type GetLoadedType(Assembly assembly, string typeName)
        {
            if (assembly == null)
            {
                return null;
            }
            int hashCode = (assembly.GetName().Name + typeName).GetHashCode();
            if (TypeDic == null)
            {
                TypeDic = new Dictionary<int, Type>();
            }
            if (TypeDic.TryGetValue(hashCode, out Type type))
            {
                return type;
            }
            type = assembly.GetType(typeName);
            if (type == null)
            {
                return null;
            }
            TypeDic[hashCode] = type;
            return type;
        }

        public static void InitAssemblyDic()
        {
            AssemblyDic = new Dictionary<string, Assembly>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                AssemblyDic[assembly.GetName().Name] = assembly;
            }
            Debug.Log("InitAssemblyDic" + AssemblyDic.Count);
        }

        public static JToken GetJtokenByKey(JObject jObject, string key)
        {
            if (jObject.TryGetValue(key, out JToken jToken))
            {
                return jToken;
            }
            return new JValue("-1");
        }

#if JINGTAN_APP
        public static void OpenUrl(string url, Action<Exception, string> callback = null)
        {
            try{
                var player = new
                {
                    url = url
                };

                // 将匿名对象转换为JSON字符串
                string json = JsonConvert.SerializeObject(player);
                Debug.Log("OpenUrl: " + AntMetaverseUtil.GetMethodInfo("Assembly-CSharp", "Platform", "InvokeNativeMethod") + json);
                AntMetaverseUtil.GetMethodInfo("Assembly-CSharp", "Platform", "InvokeNativeMethod")?.Invoke(null, new string[]{"openUrl", json, ""});
                callback?.Invoke(null, "success");
            }
            catch(Exception e){
                callback?.Invoke(e, null);
            }
        }

        public static void ShowLoading(){
            AntMetaverseUtil.GetMethodInfo("Assembly-CSharp", "Platform", "InvokeNativeMethod")?.Invoke(null, new string[]{"showLoading", "", ""});
        }

        public static void HideLoading(){
            AntMetaverseUtil.GetMethodInfo("Assembly-CSharp", "Platform", "InvokeNativeMethod")?.Invoke(null, new string[]{"hideLoading", "", ""});
        }
#endif


        public static void Dispose()
        {
            AssemblyDic = null;
            TypeDic = null;
            MethodInfoDic = null;
            MetaSDK.Dispose();
        }

    }
}