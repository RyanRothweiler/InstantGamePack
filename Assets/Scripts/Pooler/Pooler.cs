using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooler : MonoBehaviour
{
	public static Pooler Instance;

	public class Pool
	{
		public GameObject obj;
		public Queue<GameObject> availableObjects;
		public int size;
	}
	private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();

	// Custom pools use the given key instead of the prefab obj name and are created here
	[System.Serializable]
	public class CustomPool
	{
		public string key;
		public GameObject fab;
		public int initialCount;
	}
	public CustomPool[] customPools;

	void Awake()
	{
		Instance = this;

		// Create custom pools
		for (int index = 0; index < customPools.Length; index++) {
			CreatePool(customPools[index].fab, customPools[index].initialCount, customPools[index].key);
		}
	}

	public void CreatePool(GameObject obj, int initialCount, string key = null)
	{
		string poolId = key;
		if (string.IsNullOrEmpty(poolId)) {
			poolId = obj.name;
		}

		if (pools.ContainsKey(poolId)) {
			Debug.Log(string.Format("That pool id already exists {0}. Not creating new pool but expanding the existing pool size.", poolId));
		} else {
			Pool newPool = new Pool();
			newPool.availableObjects = new Queue<GameObject>();
			newPool.obj = obj;
			pools[poolId] = newPool;
		}

		for (int index = 0; index < initialCount; index++) {
			AddObj(pools[poolId], poolId);
		}
	}

	public GameObject GetObject(GameObject obj)
	{
		return (GetObject(obj.name, obj));
	}

	public GameObject GetObject(string poolId)
	{
		return (GetObject(poolId, null));
	}

	private GameObject GetObject(string poolId, GameObject obj)
	{
		if (pools.ContainsKey(poolId)) {
			if (pools[poolId].availableObjects.Count == 0) {
				Debug.Log(string.Format("Expanding Pool {0}. New size is {1}", poolId, pools[poolId].size));
				AddObj(pools[poolId], poolId);
			}
			GameObject newObj = pools[poolId].availableObjects.Dequeue();
			newObj.SetActive(true);
			newObj.transform.SetParent(null, true);
			return (newObj);
		} else {
			if (obj != null) {
				int defaultSize = 5;
				Debug.Log(string.Format("That pool of {0} does not exist. Creating one with a default size of {1} Plese create it first.", poolId, defaultSize));
				CreatePool(obj, defaultSize);
				return (GetObject(obj));
			} else {
				Debug.LogError(string.Format("That pool of {0} does not exist. No obj given so cannot create pool. Fatal error.", poolId));
				return (null);
			}
		}
	}

	public void Recycle(GameObject obj)
	{
		PoolObj po = obj.GetComponent<PoolObj>();
		if (po == null) {
			Debug.LogError(string.Format("Game object {0} does not have a PoolObj component.", obj.name));
			return;
		}

		obj.gameObject.transform.SetParent(this.transform, true);
		obj.gameObject.SetActive(false);
		pools[po.poolId].availableObjects.Enqueue(obj.gameObject);
	}

	private void AddObj(Pool pool, string poolId)
	{
		GameObject newObj = Instantiate(pool.obj);
		newObj.gameObject.SetActive(false);
		newObj.transform.SetParent(this.transform, true);
		pool.availableObjects.Enqueue(newObj);
		pool.size++;

		PoolObj po = newObj.AddComponent<PoolObj>();
		po.poolId = poolId;
	}
}