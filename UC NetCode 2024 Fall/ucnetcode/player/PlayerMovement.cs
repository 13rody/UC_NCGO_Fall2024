using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ucnetcode.player {
    public class PlayerMovement : MonoBehaviour
    {
        void Update()
        {
            Vector3 moveDirection = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.W)) moveDirection.z = +1f;
            if (Input.GetKey(KeyCode.A)) moveDirection.z = -1f;
            if (Input.GetKey(KeyCode.S)) moveDirection.x = -1f;
            if (Input.GetKey(KeyCode.D)) moveDirection.x = +1f;

            float moveSpeed = 3f;
            transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        }
    }
}
