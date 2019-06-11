using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFollowMouseBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = InputManager.Cursor.Position.GroundPosition;
    }
}
