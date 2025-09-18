using UnityEngine;

namespace NetworkDemo
{
    public sealed class SimpleWASD : MonoBehaviour
    {
        public float speed = 3f;
        void Update()
        {
            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");
            var dir = new Vector3(h, 0f, v).normalized;
            transform.Translate(dir * speed * Time.deltaTime, Space.World);
        }
    }
}