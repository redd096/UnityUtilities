namespace redd096
{
	using System.Collections;
	using UnityEngine;

	[AddComponentMenu("redd096/MonoBehaviours/Camera Shake")]
	public class CameraShake : MonoBehaviour
	{
		[Header("Camera - by default is this transform")]
		[Tooltip("If null, use this transform")] [SerializeField] Transform camTransform;

		[Header("Shake")]
		[Tooltip("How long should shake")] [SerializeField] float shakeDuration = 1f;
		[Tooltip("Amplitude of the shake. A larger value shakes the camera harder")] [SerializeField] float shakeAmount = 0.7f;

		Coroutine shakeCoroutine;
		Vector3 originalPos;

		void Awake()
		{
			//get reference if null
			if (camTransform == null)
			{
				camTransform = transform;
			}

			//then save original position
			originalPos = camTransform.localPosition;
		}

		IEnumerator ShakeCoroutine()
		{
			//set shake duration
			float shake = Time.time + shakeDuration;

			//shake for shakeDuration
			while (shake > Time.time)
			{
				camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

				yield return null;
			}

			//then reset to original position
			camTransform.localPosition = originalPos;
		}

		public void StartShake()
		{
			//start or restart shake coroutine
			if (shakeCoroutine != null)
				StopCoroutine(shakeCoroutine);

			shakeCoroutine = StartCoroutine(ShakeCoroutine());
		}
	}
}