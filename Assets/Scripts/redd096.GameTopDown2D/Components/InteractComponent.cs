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
		[Tooltip("Delay between updates using Coroutine method")] [ShowIf("updateMode", EUpdateModes.Coroutine)] [SerializeField] float timeCoroutine = 0.1f;
		[Tooltip("Area to check for interactables")] [SerializeField] float radiusInteract = 1.5f;
		[Tooltip("Ignore interactables with this layer")] [SerializeField] LayerMask layersToIgnore = default;

		[Header("DEBUG")]
		[SerializeField] bool drawDebug = false;

		//events
		public System.Action<InteractableBASE> onFoundInteractable { get; set; }
		public System.Action<InteractableBASE> onLostInteractable { get; set; }

		//update mode
		Coroutine updateCoroutine;

		//interactables
		InteractableBASE[] possibleInteractables;
		InteractableBASE nearestInteractable;

		Redd096Main owner;

        void OnDrawGizmos()
        {
			//draw area interactable
            if(drawDebug)
            {
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere(transform.position, radiusInteract);
				Gizmos.color = Color.white;
			}
        }

        void Awake()
		{
			//get references
			owner = GetComponent<Redd096Main>();
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

		InteractableBASE FindNearest()
        {
			if (possibleInteractables == null)
				return null;

			//find nearest
			float distance = Mathf.Infinity;
			InteractableBASE nearest = null;
			foreach(InteractableBASE interactable in possibleInteractables)
            {
				if(Vector2.Distance(interactable.transform.position, transform.position) < distance)
                {
					distance = Vector2.Distance(interactable.transform.position, transform.position);
					nearest = interactable;
				}
            }

			return nearest;
        }

        #endregion

        #region public API

        /// <summary>
        /// Find every interactable in area and nearest one
        /// </summary>
        public void FindInteractables()
		{
			//find every interactable in area
			List<InteractableBASE> list = new List<InteractableBASE>();
			foreach(Collider2D col in Physics2D.OverlapCircleAll(transform.position, radiusInteract, ~layersToIgnore))
            {
				InteractableBASE interactable = col.GetComponentInParent<InteractableBASE>();
				if(interactable)
                {
					list.Add(interactable);
                }
            }

			//set interactables and find nearest
			possibleInteractables = list.ToArray();
			InteractableBASE nearest = FindNearest();

			if(nearest != nearestInteractable)
			{
				//call events
				if (nearestInteractable)
					onLostInteractable?.Invoke(nearestInteractable);

				if(nearest)
					onFoundInteractable?.Invoke(nearest);

				//and set nearest interactable
				nearestInteractable = nearest;
            }
		}

		/// <summary>
		/// Interact with nearest interactable
		/// </summary>
		public void Interact()
        {
			if (nearestInteractable)
				nearestInteractable.Interact(owner);
        }

		/// <summary>
		/// Return nearest interactable
		/// </summary>
		/// <returns></returns>
		public InteractableBASE GetNearestInteractable()
        {
			return nearestInteractable;
        }

		/// <summary>
		/// Return a list of every possible interactable
		/// </summary>
		/// <returns></returns>
		public InteractableBASE[] GetEveryPossibleInteractable()
        {
			return possibleInteractables;
        }

		#endregion
	}
}