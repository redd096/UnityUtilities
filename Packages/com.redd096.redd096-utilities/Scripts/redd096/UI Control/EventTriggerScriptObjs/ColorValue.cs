﻿using UnityEngine;

namespace redd096
{
    [CreateAssetMenu(fileName = "Color", menuName = "redd096/UI Control/Color")]
    public class ColorValue : ScriptableObject
    {
        [SerializeField] Color color = Color.red;

        public Color Color { get { return color; } }
    }
}