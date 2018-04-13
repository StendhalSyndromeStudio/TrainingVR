using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using PhotonNetwork.Synchronization;
using UnityEngine;

namespace PhotonNetwork
{
    public struct strMemberInfo
    {
        public readonly CodeInfo codeInfo;
        public readonly MemberInfo memberInfo;
        public readonly string name;
        public readonly List<PhotonAttribute> photonAttribute;

        public strMemberInfo(CodeInfo codeInfo, string name, MemberInfo memberInfo, List<PhotonAttribute> photonAttribute)
        {
            this.codeInfo = codeInfo;
            this.name = name;
            this.memberInfo = memberInfo;
            this.photonAttribute = photonAttribute;
        }
        /// <summary>
        /// Добавление условия
        /// </summary>
        /// <param name="obj"></param>
        internal void Add(PhotonAttribute obj)
        {
            this.photonAttribute.Add(obj);
        }
    }

    public static class ExPhotonBehaviour
    {
        internal static GameObject mainParent(this GameObject gameObject)
        {
            try
            {
                Transform transform = gameObject.transform;
                while (transform.parent != null)
                {
                    transform = transform.parent;
                }
                return transform.gameObject;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Поиск PhotonNetworkView (поиск идет снизу вверх)
        /// </summary>
        /// <param name="photonBehaviour"></param>
        /// <param name="photonNetworkView"></param>
        /// <returns></returns>
        internal static bool mainPhotonNetworkView(this PhotonBehaviour photonBehaviour, out PhotonNetworkView photonNetworkView)
        {
            photonNetworkView = null;
            if (photonBehaviour == null) return false;

            UnityEngine.Transform objParent = photonBehaviour.gameObject.transform;
            while (objParent != null)
            {
                photonNetworkView = objParent.GetComponent<PhotonNetworkView>();
                if (photonNetworkView != null)
                {
                    return true;
                }
                objParent = objParent.parent;
            }
            return false;
        }

        /// <summary>
        /// Путь к объекту
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        internal static string Path(this Transform transform)
        {
            List<string> parents = new List<string>();
            while(transform != null)
            {
                parents.Add(transform.gameObject.name);
                transform = transform.parent;
            }
            parents.Reverse();
            return String.Join("/", parents.ToArray());
        }

        public static List<MethodInfo> ParseMetod(this object target, string typeString)
        {
            List<MethodInfo> result = new List<MethodInfo>();
            Type typeObj = Type.GetType(typeString);

            foreach (MethodInfo method in typeObj.GetMethods(BindingFlags.Public| BindingFlags.NonPublic| BindingFlags.Instance))
            {
                try
                {
                    object[] attribs = method.GetCustomAttributes(typeof(PhotonAttribute), false);
                    if (attribs.Length != 0)
                    {
                        result.Add(method);
                    }
                }
                catch { }
            }
            return result;
        }

        public static Dictionary<string, strMemberInfo> ParseField(this object target, string typeString)
        {
            Dictionary<string, strMemberInfo> result = new Dictionary<string, strMemberInfo>();
            Type typeObj = Type.GetType(typeString);
            strMemberInfo _strMemberInfo;
            foreach (FieldInfo field in typeObj.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object[] attribs = field.GetCustomAttributes(typeof(PhotonAttribute), false);
                if (attribs.Length == 0) continue;
                _strMemberInfo = new strMemberInfo(CodeInfo.Field, field.Name, field, new List<PhotonAttribute>());
                foreach (object obj in attribs)
                {
                    _strMemberInfo.Add((PhotonAttribute)obj);
                }
                result.Add(field.Name, _strMemberInfo);
                /*
                if (attribs.Length != 0)
                {
                    result.Add(field.Name, new strMemberInfo(Synchronization.CodeInfo.Field, field.Name, field, (PhotonAttribute)attribs[0]));
                }*/
            }
            return result;
        }

        public static Dictionary<string, strMemberInfo> ParseProperty(this object target, string typeString)
        {
            Dictionary<string, strMemberInfo> result = new Dictionary<string, strMemberInfo>();
            Type typeObj = Type.GetType(typeString);
            strMemberInfo _strMemberInfo;
            foreach (PropertyInfo prop in typeObj.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object[] attribs = prop.GetCustomAttributes(typeof(PhotonAttribute), false);
                if (attribs.Length == 0) continue;
                _strMemberInfo = new strMemberInfo(CodeInfo.Property, prop.Name, prop, new List<PhotonAttribute>());
                foreach (object obj in attribs)
                {
                    _strMemberInfo.Add((PhotonAttribute)obj);
                }
                result.Add(prop.Name, _strMemberInfo);
                /*
                if (attribs.Length != 0)
                {
                    result.Add(prop.Name, new strMemberInfo(CodeInfo.Property, prop.Name, prop, (PhotonAttribute)attribs[0]));
                }*/
            }
            return result;
        }


        public static List<MethodInfo> ParseMetod(this object target)
        {
            return ParseMetod(target, target.GetType().ToString());
        }

        public static Dictionary<string, strMemberInfo> ParseField(this object target)
        {
            return ParseField(target, target.GetType().ToString());
        }

        public static Dictionary<string, strMemberInfo> ParseProperty(this object target)
        {
            return ParseProperty(target, target.GetType().ToString());
        }
    }
}
