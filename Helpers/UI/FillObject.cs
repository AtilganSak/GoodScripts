using Highborn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Highborn
{
    public class FillObject : MonoBehaviour
    {
        public SpriteRenderer fillRender;

        public void SetValue(float value)
        {
            fillRender.material.SetFloat("_Arc1", Util.Remap(value, 0, 1, 360, 0));
        }
    }
}
