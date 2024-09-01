using System.Collections;
using UnityEngine;

namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// Instantiate line renderer to show shoot in game
    /// </summary>
    [System.Serializable]
    public class ExampleWeaponShootFeedbacks : IComponentRD
    {
        [SerializeField] Transform barrel;
        [SerializeField] LineRenderer weaponShootPrefab;
        [SerializeField] float durationFade = 0.3f;

        public IGameObjectRD Owner { get; set; }

        ExampleWeaponRange weapon;

        //static to use same pooling for every weapon
        static Pooling<LineRenderer> pooling = new Pooling<LineRenderer>();
        static Transform lineRenderersParent;

        public void Awake()
        {
            //be sure to have parent
            if (lineRenderersParent == null)
                lineRenderersParent = new GameObject($"Weapons' shoots container").transform;

            //add events
            weapon = Owner.transform.GetComponent<ExampleWeaponRange>();
            if (weapon)
            {
                weapon.onEveryBullet += OnEveryBullet;
            }
            else
            {
                Debug.LogError($"Missing weapon on {GetType().Name}", Owner.transform.gameObject);
            }
        }

        public void OnDestroy()
        {
            //remove events
            if (weapon)
            {
                weapon.onEveryBullet -= OnEveryBullet;
            }
        }

        private void OnEveryBullet()
        {
            OnEveryBullet(weapon.transform.position, Vector3.zero, weapon.transform.forward, default);
        }

        private void OnEveryBullet(Vector3 origin, Vector3 originRandomOffset, Vector3 direction, RaycastHit hit)
        {
            //instantiate line renderer
            LineRenderer lineRenderer = pooling.Instantiate(weaponShootPrefab, lineRenderersParent);
            if (hit.transform)
                lineRenderer.SetPositions(new Vector3[] { barrel.position + originRandomOffset, hit.point });
            else
                lineRenderer.SetPositions(new Vector3[] { barrel.position + originRandomOffset, origin + direction * weapon.bulletData.MaxDistance });

            //start fade coroutine
            weapon.Owner.transform.GetComponent<SimplePlayerPawn>().StartCoroutine(FadeCoroutine(lineRenderer));
        }

        IEnumerator FadeCoroutine(LineRenderer lineRenderer)
        {
            //fade color
            float delta = 0;
            while (delta < 1)
            {
                delta += Time.deltaTime / durationFade;

                Color start = lineRenderer.startColor;
                Color end = lineRenderer.endColor;
                start.a = Mathf.Lerp(1, 0, delta);
                end.a = Mathf.Lerp(1, 0, delta);
                lineRenderer.startColor = start;
                lineRenderer.endColor = end;

                yield return null;
            }

            //and deactive
            Pooling.Destroy(lineRenderer.gameObject);
        }
    }
}