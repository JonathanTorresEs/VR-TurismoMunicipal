using System;
using System.Collections.Generic;
using UnityEngine;

// This is a basic prefab spawner. 
// If you use prefabs heavily in your project, better caching is recommended.

namespace DigitalSalmon.C360 {
	[AddComponentMenu("Complete360Tour/Media/PrefabReactor")]
	public class PrefabReactor : MonoBehaviour {
		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private readonly HashSet<GameObject> spawnedPrefabs = new HashSet<GameObject>();
		private readonly HashSet<IMappedPrefab> activeMappedPrefabs = new HashSet<IMappedPrefab>();

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void OnEnable() { Complete360Tour.MediaSwitch += C360_MediaSwitch; }

		protected void OnDisable() { Complete360Tour.MediaSwitch -= C360_MediaSwitch; }

		//-----------------------------------------------------------------------------------------
		// Event Handlers:
		//-----------------------------------------------------------------------------------------

		protected void C360_MediaSwitch(MediaSwitchStates state, NodeData data) {
			switch (state) {
				case MediaSwitchStates.BeforeSwitch:
					InformPrefabs(state);
					break;
				case MediaSwitchStates.Switch:
					DestroyPrefabs();
					if (data != null) CreatePrefabs(data, data.GetElements<PrefabElement>());
					break;
				case MediaSwitchStates.AfterSwitch:
					InformPrefabs(state);
					break;
				default:
					throw new ArgumentOutOfRangeException("state", state, null);
			}
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private void InformPrefabs(MediaSwitchStates state) {
			foreach (IMappedPrefab mappedPrefab in activeMappedPrefabs) {
				mappedPrefab.UpdateState(state);
			}
		}

		private void DestroyPrefabs() {
			foreach (GameObject obj in spawnedPrefabs) {
				Destroy(obj);
			}

			activeMappedPrefabs.Clear();
			spawnedPrefabs.Clear();
		}

		private void CreatePrefabs(NodeData data, IEnumerable<PrefabElement> elements) {
			foreach (PrefabElement element in elements) {
				GameObject template = Resources.Load<GameObject>(element.PrefabPath);

				if (template == null) {
					Debug.LogWarning("Failed to locate prefab at path " + element.PrefabPath + ", skipping prefab.");
					continue;
				}

				CreatePrefab(template, element, data);
			}
		}

		private void CreatePrefab(GameObject template, PrefabElement element, NodeData data) {
			GameObject spawnedPrefab = Instantiate(template);
			spawnedPrefab.transform.SetParent(transform, false);
			spawnedPrefabs.Add(spawnedPrefab);

			NodeDataElement.UpdateMappedElementPosition(spawnedPrefab.transform, element);

			IMappedPrefab[] mappedPrefabComponents = spawnedPrefab.GetComponentsInChildren<IMappedPrefab>();
			foreach (IMappedPrefab component in mappedPrefabComponents) {
				component.UpdateData(element, data);
				activeMappedPrefabs.Add(component);
			}
		}
	}
}