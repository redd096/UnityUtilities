namespace redd096
{
    using UnityEngine;

    public static class Raycasts
    {
        /// <summary>
        /// Simple cast - but set few things when doesn't hit. 
        /// If radius 0 raycast, else spherecast
        /// </summary>
        public static bool Cast(Transform cam, out RaycastHit hit, float distance, float radius = 0f)
        {
            bool hitSomething;
            LayerMask layer = GetLayer(new string[] { "Player", "WeaponEquipped" });
            QueryTriggerInteraction query = GetQuery(QueryTriggerInteraction.Ignore);

            if (Math.EqualFloat(radius, 0))
            {
                //if radius 0 raycast
                hitSomething = Raycast(cam, out hit, distance, layer, query);
            }
            else
            {
                //else spherecast
                hitSomething = Spherecast(cam, out hit, distance, radius, layer, query);
            }

            //set things when doesn't hit
            CheckHit(hitSomething, hit, cam, out hit, distance);

            return hitSomething;
        }

        #region private API

        /// <summary>
        /// Get LayerMask
        /// </summary>
        private static LayerMask GetLayer(string[] exceptLayers)
        {
            return CreateLayer.LayerAllExcept(exceptLayers);
        }

        /// <summary>
        /// Get QueryTriggerInteraction
        /// </summary>
        private static QueryTriggerInteraction GetQuery(QueryTriggerInteraction query)
        {
            return query;
        }

        /// <summary>
        /// Simple raycast
        /// </summary>
        private static bool Raycast(Transform cam, out RaycastHit hit, float distance, LayerMask layer, QueryTriggerInteraction query)
        {
            //doesn't hit player or weaponEquipped layers, neither trigger
            return Physics.Raycast(cam.position, cam.forward, out hit, distance, layer, query);
        }

        /// <summary>
        /// Simple spherecast
        /// </summary>
        private static bool Spherecast(Transform cam, out RaycastHit hit, float distance, float radius, LayerMask layer, QueryTriggerInteraction query)
        {
            //doesn't hit player or weaponEquipped layers, neither trigger
            return Physics.SphereCast(cam.position, radius, cam.forward, out hit, distance, layer, query);
        }

        /// <summary>
        /// Set things when doesn't hit 
        /// </summary>
        private static void CheckHit(bool hitSomething, RaycastHit hit, Transform cam, out RaycastHit _hit, float distance)
        {
            _hit = hit;

            if (hitSomething == false)
            {
                _hit.point = cam.position + cam.forward * distance;
                _hit.distance = distance;
            }
        }

        #endregion
    }
}