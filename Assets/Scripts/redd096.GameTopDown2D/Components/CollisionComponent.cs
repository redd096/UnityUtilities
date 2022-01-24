using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using NaughtyAttributes;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
	[AddComponentMenu("redd096/.GameTopDown2D/Components/Collision Component")]
	public class CollisionComponent : MonoBehaviour
	{
		enum EUpdateModes { Update, FixedUpdate, Coroutine }
		public enum EDirectionEnum { up, right, left, down }

		[Header("Check Raycasts")]
		[Tooltip("Check collisions on Update or FixedUpdate?")] [SerializeField] EUpdateModes updateMode = EUpdateModes.Coroutine;
		[Tooltip("Delay between updates using Coroutine method")] [EnableIf("updateMode", EUpdateModes.Coroutine)] [SerializeField] float timeCoroutine = 0.1f;
		[Tooltip("Number of rays cast for every side horizontally")] [SerializeField] int numberOfHorizontalRays = 4;
		[Tooltip("Number of rays cast for every side vertically")] [SerializeField] int numberOfVerticalRays = 4;
		[Tooltip("A small value to accomodate for edge cases")] [SerializeField] float offsetRays = 0.01f;
		[Tooltip("Layers that raycasts ignore")] [SerializeField] LayerMask layersToIgnore = default;

		[Header("Necessary Components (by default get in children)")]
		[SerializeField] BoxCollider2D boxCollider = default;

		[Header("DEBUG")]
		[SerializeField] bool drawDebugInPlay = false;
		//[ShowNativeProperty] bool IsHittingRight => rightHits.Count > 0;
		//[ShowNativeProperty] bool IsHittingLeft => leftHits.Count > 0;
		//[ShowNativeProperty] bool IsHittingUp => upHits.Count > 0;
		//[ShowNativeProperty] bool IsHittingDown => downHits.Count > 0;

		//events
		public System.Action<RaycastHit2D> onCollisionEnter { get; set; }
		public System.Action<RaycastHit2D> onCollisionStay { get; set; }
		public System.Action<Collider2D> onCollisionExit { get; set; }
		public System.Action<RaycastHit2D> onTriggerEnter { get; set; }
		public System.Action<RaycastHit2D> onTriggerStay { get; set; }
		public System.Action<Collider2D> onTriggerExit { get; set; }

		//hits
		List<RaycastHit2D> rightHits = new List<RaycastHit2D>();
		List<RaycastHit2D> leftHits = new List<RaycastHit2D>();
		List<RaycastHit2D> upHits = new List<RaycastHit2D>();
		List<RaycastHit2D> downHits = new List<RaycastHit2D>();

		//bounds
		Vector2 centerBounds;
		float horizontalExtents;
		float verticalExtents;

		//bounds limits
		float upBounds;
		float downBounds;
		float rightBounds;
		float leftBounds;

		//used for events
		Dictionary<Collider2D, RaycastHit2D> currentCollisions = new Dictionary<Collider2D, RaycastHit2D>();
		List<Collider2D> previousCollisions = new List<Collider2D>();

		//for debug
		float drawDebugDuration = -1;

		//update mode
		Coroutine updateCoroutine;

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
				UpdateCollisions();
		}

		void FixedUpdate()
		{
			//do only if update mode is FixedUpdate
			if (updateMode == EUpdateModes.FixedUpdate)
				UpdateCollisions();
		}

		IEnumerator UpdateCoroutine()
		{
			//do only if update mode is Coroutine
			while (updateMode == EUpdateModes.Coroutine)
			{
				UpdateCollisions();
				yield return new WaitForSeconds(timeCoroutine);
			}
		}

		[Button]
		void DrawCollisions()
		{
			//be sure drawDebug is true
			bool previousDraw = drawDebugInPlay;
			drawDebugInPlay = true;

			//set time
			drawDebugDuration = 2;

			//check collisions
			UpdateCollisions();

			//restore debug values
			drawDebugInPlay = previousDraw;
			drawDebugDuration = -1;
		}

		#region private API

		bool CheckComponents()
		{
			//be sure to have a box collider
			if (boxCollider == null)
			{
				boxCollider = GetComponentInChildren<BoxCollider2D>();

				if (boxCollider == null)
				{
					Debug.LogWarning("Miss BoxCollider on " + name);
					return false;
				}
			}

			return true;
		}

		void CheckCollisionsHorizontal()
		{
			//horizontal raycast vars
			Vector2 horizontalRaycastOriginBottom = new Vector2(centerBounds.x, downBounds + offsetRays);
			Vector2 horizontalRaycastOriginTop = new Vector2(centerBounds.x, upBounds - offsetRays);
			float raycastHorizontalLength = (rightBounds - leftBounds) * 0.5f;
			rightHits.Clear();
			leftHits.Clear();

			Vector2 raycastOriginPoint;
			RaycastHit2D rightHit;
			RaycastHit2D leftHit;
			for (int i = 0; i < numberOfHorizontalRays; i++)
			{
				//from bottom to top
				raycastOriginPoint = Vector2.Lerp(horizontalRaycastOriginBottom, horizontalRaycastOriginTop, (float)i / (numberOfHorizontalRays - 1));

				//raycast right and left
				rightHit = RayCastHitSomething(raycastOriginPoint, Vector2.right, raycastHorizontalLength);
				leftHit = RayCastHitSomething(raycastOriginPoint, Vector2.left, raycastHorizontalLength);

				//save hits
				if (rightHit) rightHits.Add(rightHit);
				if (leftHit) leftHits.Add(leftHit);

				//debug raycasts
				if (drawDebugInPlay)
				{
					DebugRaycast(raycastOriginPoint, Vector2.right, raycastHorizontalLength, rightHit ? Color.red : Color.cyan);
					DebugRaycast(raycastOriginPoint, Vector2.left, raycastHorizontalLength, leftHit ? Color.red : Color.cyan);
				}
			}
		}

		void CheckCollisionsVertical()
		{
			//vertical raycast vars
			Vector2 verticalRaycastOriginLeft = new Vector2(leftBounds + offsetRays, centerBounds.y);
			Vector2 verticalRaycastOriginRight = new Vector2(rightBounds - offsetRays, centerBounds.y);
			float raycastVerticalLength = (upBounds - downBounds) * 0.5f;
			upHits.Clear();
			downHits.Clear();

			Vector2 raycastOriginPoint;
			RaycastHit2D upHit;
			RaycastHit2D downHit;
			for (int i = 0; i < numberOfVerticalRays; i++)
			{
				//from left to right
				raycastOriginPoint = Vector2.Lerp(verticalRaycastOriginLeft, verticalRaycastOriginRight, (float)i / (numberOfVerticalRays - 1));

				//raycasts up and down
				upHit = RayCastHitSomething(raycastOriginPoint, Vector2.up, raycastVerticalLength);
				downHit = RayCastHitSomething(raycastOriginPoint, Vector2.down, raycastVerticalLength);

				//save hits
				if (upHit) upHits.Add(upHit);
				if (downHit) downHits.Add(downHit);

				//debug raycasts
				if (drawDebugInPlay)
				{
					DebugRaycast(raycastOriginPoint, Vector2.up, raycastVerticalLength, upHit ? Color.red : Color.blue);
					DebugRaycast(raycastOriginPoint, Vector2.down, raycastVerticalLength, downHit ? Color.red : Color.blue);
				}
			}
		}

		RaycastHit2D RayCastHitSomething(Vector2 originPoint, Vector2 direction, float distance)
		{
			float distanceToNearest = Mathf.Infinity;
			RaycastHit2D nearest = default;

			foreach (RaycastHit2D hit in Physics2D.RaycastAll(originPoint, direction, distance, ~layersToIgnore))
			{
				//for every hit, be sure to not hit self
				if (hit && hit.collider != boxCollider)
				{
					//save for events
					if (currentCollisions.ContainsKey(hit.collider) == false)
						currentCollisions.Add(hit.collider, hit);

					//calculate nearest hit, only if self collider is not trigger and doesn't hit trigger collider
					if (boxCollider.isTrigger == false && hit.collider.isTrigger == false)
					{
						if (hit.distance < distanceToNearest)
						{
							distanceToNearest = hit.distance;
							nearest = hit;
						}
					}
				}
			}

			return nearest;
		}

		void DebugRaycast(Vector2 originPoint, Vector2 direction, float distance, Color color)
		{
			//debug
			if (drawDebugDuration > 0)
				Debug.DrawRay(originPoint, direction * distance, color, drawDebugDuration);     //when called by press the button, visualizare for few seconds
			else
				Debug.DrawRay(originPoint, direction * distance, color);                        //else show at every update
		}

		void CheckCollisionEvents()
		{
			//call Enter or Stay
			foreach (Collider2D col in currentCollisions.Keys)
			{
				if (previousCollisions.Contains(col) == false)
				{
					if (col.isTrigger || boxCollider.isTrigger)
						onTriggerEnter?.Invoke(currentCollisions[col]);     //trigger enter
					else
						onCollisionEnter?.Invoke(currentCollisions[col]);   //collision enter
				}
				else
				{
					if (col.isTrigger || boxCollider.isTrigger)
						onTriggerStay?.Invoke(currentCollisions[col]);      //trigger stay
					else
						onCollisionStay?.Invoke(currentCollisions[col]);    //collision stay
				}
			}

			//call Exit
			foreach (Collider2D col in previousCollisions)
			{
				if (currentCollisions.ContainsKey(col) == false)
				{
					if (col.isTrigger || boxCollider.isTrigger)
						onTriggerExit?.Invoke(col);                         //trigger exit
					else
						onCollisionExit?.Invoke(col);                       //collision exit
				}
			}
		}

		#endregion

		#region public API

		/// <summary>
		/// Update collisions
		/// </summary>
		public void UpdateCollisions()
		{
			//start only if there are all necessary components
			if (CheckComponents() == false)
				return;

			//reset current collisions (to recreate this frame)
			currentCollisions.Clear();

			//check collisions
			UpdateBounds();
			CheckCollisionsHorizontal();
			CheckCollisionsVertical();

			//check events (collision enter, stay, exit)
			CheckCollisionEvents();

			//copy collisions in another list for next frame checks
			previousCollisions = new List<Collider2D>(currentCollisions.Keys);
		}

		/// <summary>
		/// Update bounds informations
		/// </summary>
		public void UpdateBounds()
		{
			//check only if there are all necessary components
			if (CheckComponents() == false)
				return;

			//update bounds
			centerBounds = boxCollider.bounds.center;
			verticalExtents = boxCollider.bounds.extents.y;
			horizontalExtents = boxCollider.bounds.extents.x;

			//bounds limits
			upBounds = centerBounds.y + verticalExtents;
			downBounds = centerBounds.y - verticalExtents;
			rightBounds = centerBounds.x + horizontalExtents;
			leftBounds = centerBounds.x - horizontalExtents;
		}

		/// <summary>
		/// Return reachable position in direction (cause collisions), nearest to desired one
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="desiredPosition"></param>
		/// <returns></returns>
		public Vector2 CalculateReachablePosition(EDirectionEnum direction, Vector2 desiredPosition)
		{
			//raycast vars
			UpdateBounds();
			float bounds = GetBounds(direction);
			int numberOfRays = direction == EDirectionEnum.right || direction == EDirectionEnum.left ? numberOfHorizontalRays : numberOfVerticalRays;
			Vector2 raycastOriginFirst = direction == EDirectionEnum.right || direction == EDirectionEnum.left ? new Vector2(centerBounds.x, downBounds + offsetRays) : new Vector2(leftBounds + offsetRays, centerBounds.y);
			Vector2 raycastOriginSecond = direction == EDirectionEnum.right || direction == EDirectionEnum.left ? new Vector2(centerBounds.x, upBounds - offsetRays) : new Vector2(rightBounds - offsetRays, centerBounds.y);
			float raycastLength = direction == EDirectionEnum.right || direction == EDirectionEnum.left ? horizontalExtents + Mathf.Abs(desiredPosition.x - transform.position.x) : verticalExtents + Mathf.Abs(desiredPosition.y - transform.position.y);
			Vector2 raycastDirection;
			Vector2 raycastOriginPoint;
			RaycastHit2D hit;

			//direction
			if (direction == EDirectionEnum.right || direction == EDirectionEnum.left)
				raycastDirection = direction == EDirectionEnum.right ? Vector2.right : Vector2.left;
			else
				raycastDirection = direction == EDirectionEnum.up ? Vector2.up : Vector2.down;

			//use raycasts
			for (int i = 0; i < numberOfRays; i++)
			{
				//from bottom to top, raycast right or left
				raycastOriginPoint = Vector2.Lerp(raycastOriginFirst, raycastOriginSecond, (float)i / (numberOfRays - 1));
				hit = RayCastHitSomething(raycastOriginPoint, raycastDirection, raycastLength);

				//adjust position
				if (hit)
				{
					if (direction == EDirectionEnum.right || direction == EDirectionEnum.left)
					{
						desiredPosition.x = hit.point.x - (bounds - transform.position.x);
					}
					else if (direction == EDirectionEnum.up || direction == EDirectionEnum.down)
					{
						desiredPosition.y = hit.point.y - (bounds - transform.position.y);
					}

					//add to hits, to save hitting this way
					if (direction == EDirectionEnum.right) rightHits.Add(hit);
					else if (direction == EDirectionEnum.left) leftHits.Add(hit);
					else if (direction == EDirectionEnum.up) upHits.Add(hit);
					else if (direction == EDirectionEnum.down) downHits.Add(hit);
				}
			}

			return desiredPosition;
		}

		/// <summary>
		/// Return last hitting information in direction (call UpdateCollisions to have updated informations)
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		public bool IsHitting(EDirectionEnum direction)
		{
			switch (direction)
			{
				case EDirectionEnum.up:
					return upHits.Count > 0;
				case EDirectionEnum.right:
					return rightHits.Count > 0;
				case EDirectionEnum.left:
					return leftHits.Count > 0;
				case EDirectionEnum.down:
					return downHits.Count > 0;
				default:
					return false;
			}
		}

		/// <summary>
		/// Return last hits in direction (call UpdateCollisions to have updated informations)
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		public RaycastHit2D[] GetHits(EDirectionEnum direction)
		{
			switch (direction)
			{
				case EDirectionEnum.up:
					return upHits.ToArray();
				case EDirectionEnum.right:
					return rightHits.ToArray();
				case EDirectionEnum.left:
					return leftHits.ToArray();
				case EDirectionEnum.down:
					return downHits.ToArray();
				default:
					return null;
			}
		}

		/// <summary>
		/// Return last center of the bounds (call UpdateBounds to have updated informations)
		/// </summary>
		/// <returns></returns>
		public Vector2 GetCenterBound()
		{
			return centerBounds;
		}

		/// <summary>
		/// Return last length from center bounds to limit in direction (call UpdateBounds to have updated informations)
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		public float GetBoundsLength(EDirectionEnum direction)
		{
			switch (direction)
			{
				case EDirectionEnum.up:
					return verticalExtents;
				case EDirectionEnum.right:
					return horizontalExtents;
				case EDirectionEnum.left:
					return horizontalExtents;
				case EDirectionEnum.down:
					return verticalExtents;
				default:
					return 0;
			}
		}

		/// <summary>
		/// Return last bounds informations (call UpdateBounds to have updated informations)
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		public float GetBounds(EDirectionEnum direction)
		{
			switch (direction)
			{
				case EDirectionEnum.up:
					return upBounds;
				case EDirectionEnum.right:
					return rightBounds;
				case EDirectionEnum.left:
					return leftBounds;
				case EDirectionEnum.down:
					return downBounds;
				default:
					return 0;
			}
		}

		#endregion
	}
}