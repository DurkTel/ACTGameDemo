using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public IEnumerator Start()
    {
        AssetManager.Initialize(AssetLoadMode.Resources);

        yield return GMAudioManager.Instance.Init();

        GMAudioManager.Initialize();
    }

}
