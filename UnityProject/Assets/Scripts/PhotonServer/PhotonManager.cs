using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhotonManager : MonoBehaviour {

	public enum TypeSourcePrefab { Resources , StreamingAssets }

	public class InstanceObject
	{
		public TypeSourcePrefab source;
		public string prefab_name;
		public string bundle_name;
		public GameObject obj;
		public int group;

		public InstanceObject(string _prefab, GameObject _object, int id_group)
		{
			source = TypeSourcePrefab.Resources;
			bundle_name = "";
			prefab_name = _prefab;
            obj = _object;
			group = id_group;
		}

		public InstanceObject(string _bundle, string _prefab, GameObject _object, int id_group)
		{
			source = TypeSourcePrefab.StreamingAssets;
			bundle_name = _bundle;
			prefab_name = _prefab;
			obj = _object;
			group = id_group;
		}

	}

	private static PhotonManager _instance;
	public static PhotonManager Instance
	{
		get { return _instance; }
	}

	private Dictionary<int, InstanceObject> dictObject = new Dictionary<int, InstanceObject>();
	private int counterID = 0;

	void Awake ()
	{
		_instance = this;
	}
	
	public GameObject InstantiateResources(string prefab_name, string instance_name, Vector3 pos, Quaternion rot)
	{
		GameObject inst = Resources.Load(prefab_name) as GameObject;
		if (inst == null)
		{
			return null;
		}
		GameObject obj = GameObject.Instantiate(inst, pos, rot) as GameObject;
		obj.name = instance_name;
		counterID++;
		InstanceObject instObj = new InstanceObject(prefab_name, obj, 1);
		dictObject.Add(counterID, instObj);
		return obj;
	}

	public void InstantiateStreamingAssets(string bundle_name, string prefab_name, string instance_name, Vector3 pos, Quaternion rot)
	{
		
		GameObject inst = Resources.Load(prefab_name) as GameObject;
		if (inst == null)
		{
			return;
		}
		GameObject obj = GameObject.Instantiate(inst, pos, rot) as GameObject;
		obj.name = instance_name;
		counterID++;
		
		InstanceObject instObj = new InstanceObject(bundle_name, prefab_name, obj, 1);
		dictObject.Add(counterID, instObj);
	}

	public void RemoveObject(int networkID)
	{
		if (dictObject.ContainsKey(networkID))
		{
			Destroy(dictObject[networkID].obj);
			dictObject.Remove(networkID);
		}
	}


	public void GetInstances(string playerID)
	{
		foreach (KeyValuePair<int, InstanceObject> kvp in dictObject)
		{
			switch (kvp.Value.source)
			{
				case TypeSourcePrefab.Resources:
					PhotonServerTCP.Instance.NetWorkUnity("InstantiateResources", PhotonServerTCP.Instance.CharacterName, (byte)Rmode.player, playerID, new object[] {
						kvp.Value.prefab_name, kvp.Value.obj.name, 
                        kvp.Value.obj.transform.position.x, kvp.Value.obj.transform.position.y, kvp.Value.obj.transform.position.z,
						kvp.Value.obj.transform.rotation.x, kvp.Value.obj.transform.rotation.y, kvp.Value.obj.transform.rotation.z, kvp.Value.obj.transform.rotation.w,
						});
					break;
			}
		}
	}





}
