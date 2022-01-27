using System.Collections.Generic;
using System.Collections;
using UnityEngine;
//using NaughtyAttributes;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
	[AddComponentMenu("redd096/.GameTopDown2D/Components/Interact Component")]
	public class InteractComponent : MonoBehaviour
	{
		enum EUpdateModes { Update, FixedUpdate, Coroutine }

		[Header("Find Interactables")]
		[Tooltip("Find interactables on Update or FixedUpdate?")] [SerializeField] EUpdateModes updateMode = EUpdateModes.Coroutine;
		[Tooltip("Delay between updates using Coroutine method")] [EnableIf("updateMode", EUpdateModes.Coroutine)] [SerializeField] float timeCoroutine = 0.1f;
		[Tooltip("Area to check for interactables")] [SerializeField] float radiusInteract = 1.5f;
		[Tooltip("Ignore interactables with this layer")] [SerializeField] LayerMask layersToIgnore = default;

		[Header("DEBUG")]
		[SerializeField] bool drawDebug = false;

		//events
		public System.Action<IInteractable> onFoundInteractable { get; set; }
		public System.Action<IInteractable> onLostInteractable { get; set; }

		//update mode
		Coroutine updateCoroutine;

		//interactables
		Dictionary<Collider2D, IInteractable> possibleInteractables = new Dictionary<Collider2D, IInteractable>();
		IInteractable nearestInteractable;
		IInteractable previousNearestInteractable;

		void OnDrawGizmos()
		{
			//draw area interactable
			if (drawDebug)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere(transform.position, radiusInteract);
				Gizmos.color = Color.white;
			}
		}

		void OnEnable()
		{
			//start coroutine
			if (updateMode == EUpdateModes.Coroutine)
				updateCoroutine = StartCoroutine(UpdateCoroutine());
		}

		void OnDisable()
		{
			//be sure to stop coroutine
			if (updateCoroutine != null)
			{
				StopCoroutine(updateCoroutine);
				updateCoroutine = null;
			}
		}

		void Update()
		{
			//do only if update mode is Update
			if (updateMode == EUpdateModes.Update)
				FindInteractables();
		}

		void FixedUpdate()
		{
			//do only if update mode is FixedUpdate
			if (updateMode == EUpdateModes.FixedUpdate)
				FindInteractables();
		}

		IEnumerator UpdateCoroutine()
		{
			//do only if update mode is Coroutine
			while (updateMode == EUpdateModes.Coroutine)
			{
				FindInteractables();
				yield return new WaitForSeconds(timeCoroutine);
			}
		}

		#region private API

		void FindNearest(out IInteractable nearest)
		{
			float distance = Mathf.Infinity;
			nearest = null;

			if (possibleInteractables == null)
				return;

			//find nearest
			foreach (Collider2D col in possibleInteractables.Keys)
			{
				if (col && Vector2.Distance(col.transform.position, transform.position) < distance)
				{
					distance = Vector2.Distance(col.transform.position, transform.position);
					nearest = possibleInteractables[col];
				}
			}
		}

		#endregion

		#region public API

		/// <summary>
		/// Find every interactable in area and nearest one
		/// </summary>
		public void FindInteractables()
		{
			//find every interactable in area
			possibleInteractables.Clear();
			IInteractable interactable;
			foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, radiusInteract, ~layersToIgnore))
			{
				interactable = col.GetComponentInParent<IInteractable>();
				if (interactable != null)
				{
					possibleInteractables.Add(col, interactable);
				}
			}

			//find nearest
			FindNearest(out nearestInteractable);

			if (previousNearestInteractable != nearestInteractable)
			{
				//call events
				if (previousNearestInteractable != null)
					onLostInteractable?.Invoke(previousNearestInteractable);

				if (nearestInteractable != null)
					onFoundInteractable?.Invoke(nearestInteractable);

				//and save previous nearest interactable
				previousNearestInteractable = nearestInteractable;
			}
		}

		/// <summary>
		/// Interact with nearest interactable
		/// </summary>
		public void Interact()
		{
			if (nearestInteractable != null)
				nearestInteractable.Interact(this);
		}

		/// <summary>
		/// Return nearest interactable
		/// </summary>
		/// <returns></returns>
		public IInteractable GetNearestInteractable()
		{
			return nearestInteractable;
		}

		/// <summary>
		/// Return previous nearest interactable
		/// </summary>
		/// <returns></returns>
		public IInteractable GetPreviousNearestInteractable()
		{
			return previousNearestInteractable;
		}

		/// <summary>
		/// Return array of every possible interactable
		/// </summary>
		/// <returns></returns>
		public IInteractable[] GetEveryPossibleInteractable()
		{
			IInteractable[] tempPossibleInteractables = new IInteractable[possibleInteractables.Count];
			int i = 0;

			//add every value of dictionary to array
			foreach (IInteractable interactable in possibleInteractables.Values)
			{
				tempPossibleInteractables[i] = interactable;
				i++;
			}

			//return array
			return tempPossibleInteractables;
		}

		#endregion
	}
}