using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fps.GunPickups
{
    [ExecuteInEditMode]
    public class RotateChild : MonoBehaviour
    {

        [SerializeField] private float _speed = 180f;

        void Update()
        {
            if (transform.childCount > 0)
            {
                var toRotate = transform.GetChild(0);
                toRotate.Rotate(new Vector3(0, _speed * Time.deltaTime, 0), Space.World);
            }
        }
    }
}
