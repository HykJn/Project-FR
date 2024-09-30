using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [Header("-- Prefabs")]
    public GameObject _pfPlayerSlash;

    //GameObject Arraies
    GameObject[] _goPlayerSlash = new GameObject[20];

    GameObject[] pool;

    public static ObjectManager instance;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        for(int i = 0; i < _goPlayerSlash.Length; i++)
        {
            _goPlayerSlash[i] = Instantiate(_pfPlayerSlash);
            _goPlayerSlash[i].transform.parent = this.transform;
            _goPlayerSlash[i].SetActive(false);
        }
    }

    public GameObject Activate(string obj)
    {
        switch (obj)
        {
            case "PlayerSlash":
                pool = _goPlayerSlash;
                break;
        }

        for(int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].activeSelf)
            {
                pool[i].SetActive(true);
                //pool[i].transform.parent = null;
                return pool[i];
            }
        }
        return null;
    }
}
