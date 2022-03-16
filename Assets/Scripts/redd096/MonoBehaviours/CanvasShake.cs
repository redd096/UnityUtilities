using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace redd096
{
	public class CanvasShake : MonoBehaviour
	{
		public static CanvasShake instance;

		[Header("Objects to Shake (inside canvas)")]
		[SerializeField] RectTransform[] objectsToShake = default;

		[Header("Shake")]
		[Tooltip("How long should shake")] [SerializeField] float shakeDuration = 0.3f;
		[Tooltip("Amplitude of the shake. A larger value shakes the camera harder")] [SerializeField] float shakeAmount = 15f;

		[Header("Overwrite - start shake also if another is running")]
		[Tooltip("If another shake is already running, stop it and start new one")] [SerializeField] bool overwriteShake = true;

		Coroutine shakeCoroutine;
		Dictionary<RectTransform, Vector2> startPositions = new Dictionary<RectTransform, Vector2>();

		void Start()
		{
			//set instance to call it without use FindObjectOfType
			instance = this;

			//save start positions
			foreach (RectTransform obj in objectsToShake)
				if (obj)
					startPositions.Add(obj, obj.localPosition);
		}

		IEnumerator ShakeCoroutine(float duration, float amount)
		{
			//reset position
			foreach (RectTransform obj in startPositions.Keys)
				if (obj)
					obj.localPosition = startPositions[obj];

			//set shake duration
			float shake = Time.time + duration;

			//shake
			while (shake > Time.time && Time.timeScale > 0)
			{
				foreach (RectTransform obj in startPositions.Keys)
					if (obj)
						obj.localPosition = startPositions[obj] + (Vector2)Random.insideUnitSphere * amount;

				yield return null;
			}

			//then reset to original position
			foreach (RectTransform obj in startPositions.Keys)
				if (obj)
					obj.localPosition = startPositions[obj];

			shakeCoroutine = null;
		}

		#region public API

		/// <summary>
		/// Start shake, using variables in inspector
		/// </summary>
		public void StartShake()
		{
			StartShake(shakeDuration, shakeAmount);
		}

		/// <summary>
		/// Start shake, using specific duration and amount
		/// </summary>
		public void StartShake(float duration, float amount)
		{
			//do only if there is not another shake, or can overwrite it
			if (shakeCoroutine == null || overwriteShake)
			{
				//start or restart shake coroutine
				if (shakeCoroutine != null)
					StopCoroutine(shakeCoroutine);

				shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, amount));
			}
		}

		#endregion
	}
}