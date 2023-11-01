using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Highborn
{
    public class ViewportReset : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<RectTransform>().SetFullStrech();            
        }
    }
}
