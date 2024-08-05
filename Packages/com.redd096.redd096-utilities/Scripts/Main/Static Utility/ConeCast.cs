using System.Collections.Generic;
using UnityEngine;

namespace redd096
{
    public static class ConeCast
    {
        /// <summary>
        /// Use a sphere cast to find every hit object, then calculate the angle to check if they're in cone
        /// </summary>
        /// <returns></returns>
        public static RaycastHit[] ConeCastAll(Vector3 origin, float radius, Vector3 direction, float distance, float coneAngle, LayerMask layer)
        {
            //sphere cast
            RaycastHit[] hits = Physics.SphereCastAll(origin, radius, direction, distance, layer);
            List<RaycastHit> coneList = new List<RaycastHit>();

            for (int i = 0; i < hits.Length; i++)
            {
                //calculate direction angle, to check if in cone
                Vector3 directionToHit = (hits[i].transform.position - origin).normalized;
                if (Vector3.Angle(direction, directionToHit) < coneAngle)
                {
                    //and add to list
                    coneList.Add(hits[i]);
                }
            }

            return coneList.ToArray();
        }

        public static void DrawConeGizmos(Vector3 origin, float radius, Vector3 direction, float distance, float coneAngle, Color colorSphere = default)
        {
            //draw cone
            Vector3 left = Quaternion.AngleAxis(-coneAngle, Vector3.up) * direction;
            Vector3 right = Quaternion.AngleAxis(coneAngle, Vector3.up) * direction;
            Vector3 down = Quaternion.AngleAxis(-coneAngle, Vector3.right) * direction;
            Vector3 up = Quaternion.AngleAxis(coneAngle, Vector3.right) * direction;
            Gizmos.DrawRay(origin, left * distance);
            Gizmos.DrawRay(origin, right * distance);
            Gizmos.DrawRay(origin, down * distance);
            Gizmos.DrawRay(origin, up * distance);
            //Gizmos.DrawLine(origin + left * coneDistance, origin + right * coneDistance);
            //Gizmos.DrawLine(origin + down * coneDistance, origin + up * coneDistance);
            Gizmos.DrawLine(origin + left * distance, origin + up * distance);
            Gizmos.DrawLine(origin + left * distance, origin + down * distance);
            Gizmos.DrawLine(origin + right * distance, origin + up * distance);
            Gizmos.DrawLine(origin + right * distance, origin + down * distance);

            Gizmos.color = colorSphere;

            //draw sphere cast
            for (float i = 0; i < distance; i += radius * 0.25f)
                Gizmos.DrawWireSphere(origin + direction * i, radius);
            Gizmos.DrawWireSphere(origin + direction * distance, radius);

            Gizmos.color = Color.white;
        }
    }
}