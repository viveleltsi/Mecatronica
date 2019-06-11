using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehaviour : MonoBehaviour
{

    public GameObject Prefab;
    public int Quantity;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Quantity; i++)
        {
            Instantiate<GameObject>(Prefab);
        }
    }

}
