using System;
using System.Collections.Generic;
using System.Reflection;
using UnityClientKSGT;

namespace PhotonNetwork.Synchronization
{
    internal class ManagerSynchronization
    {
        internal static bool GET_ObjSynchronization(EventValueType<PhotonBehaviour.CodeOwner> isOwner, strMemberInfo info, object parent, out ISynchronization parameter)
        {
            switch(info.codeInfo)
            {
                case CodeInfo.Field: return GET_ObjSynchronization_MemberInfo(CodeInfo.Field, isOwner, ((FieldInfo)info.memberInfo).FieldType, info.memberInfo, parent, info.photonAttribute, out parameter);
                case CodeInfo.Property: return GET_ObjSynchronization_MemberInfo(CodeInfo.Property, isOwner, ((PropertyInfo)info.memberInfo).PropertyType, info.memberInfo, parent, info.photonAttribute, out parameter);
            }
            parameter = null;
            return false;
        }

        private static bool GET_ObjSynchronization_MemberInfo(CodeInfo codeInfo, EventValueType<PhotonBehaviour.CodeOwner> isOwner, Type typeValue, MemberInfo memberInfo, object parent, List<PhotonAttribute> att, out ISynchronization parameter)
        {
            parameter = null;
            bool result = false;
            switch(typeValue.ToString())
            {
                case "UnityEngine.Vector3": { parameter = new Synchronization_Vector3(codeInfo, isOwner, memberInfo, parent, att); result = true; }break;
                case "UnityEngine.Quaternion": { parameter = new Synchronization_Quaternion(codeInfo, isOwner, memberInfo, parent, att); result = true; } break;
            }

            return result;
        }
    }
}