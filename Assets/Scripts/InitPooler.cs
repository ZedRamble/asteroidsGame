using System.Collections.Generic;
using UnityEngine;

public class InitPooler : MonoBehaviour
{
    public static List<GameObject> PoolInitFunc(GameObject gm, Transform objParent, int n, GameControll gameControll, bool checkAdd)
    {
        GameObject init = gm;
        List<GameObject> objPool = new List<GameObject>();
        for (int i = 0; i < n; i++)
        {
            init = Instantiate(gm, objParent);
            if (checkAdd)
            {
                if (init.layer == 10 || init.layer == 9)
                    init.GetComponent<BulletControll>().gameControll = gameControll;
                else
                {
                    int len = init.transform.childCount;
                    for (int j = 0; j < len; j++)
                        init.transform.GetChild(j).GetComponent<AsteroidController>().gameControll = gameControll;
                }
            }
            init.SetActive(false);
            objPool.Add(init);
        }

        return objPool;
    }
}
