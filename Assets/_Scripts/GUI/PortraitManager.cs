using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitManager : MonoBehaviour
{
    public static PortraitManager _Ref;

    public RenderTexture[] _CloseupTextures = new RenderTexture[GameManager.MAX_PACK_SIZE];
    public RenderTexture[] _FullBodyTextures = new RenderTexture[GameManager.MAX_PACK_SIZE];

    public Camera[] _CloseUpCamera = new Camera[GameManager.MAX_PACK_SIZE];
    public Camera[] _FullBodyCamera = new Camera[GameManager.MAX_PACK_SIZE];

    private GameObject[] _PortraitDummies = new GameObject[GameManager.MAX_PACK_SIZE];

    /// <summary>
    /// Layer indexes, precalculated
    /// </summary>
    private int[] _Layers = new int[GameManager.MAX_PACK_SIZE];

    private void Awake()
    {
        if (_Ref == null)
            _Ref = this;
        else
            DestroyImmediate(this);

        //Get layer indexes
        for ( int i = 0; i < GameManager.MAX_PACK_SIZE; i++)
            _Layers[i] = LayerMask.NameToLayer("Portrait" + i.ToString());

        GameManager.playerCreatureAddedEvent += CreateDummy;
    }

    private void Start()
    {
        CreateDummies();
    }

    private void CreateDummies()
    {
        for ( int i = 0; i < GameManager.MAX_PACK_SIZE; i++ )
        {
            CreateDummy(i);
        }
    }

    public void CreateDummy( int creatureIndex )
    {
        if (CreatureStorage.activePack.Count > creatureIndex && CreatureStorage.activePack[creatureIndex] != null && CreatureStorage.activePack[creatureIndex].species.model != null)
        {
            if (_PortraitDummies[creatureIndex] != null)
                Destroy(_PortraitDummies[creatureIndex]);

            _PortraitDummies[creatureIndex] = Instantiate(CreatureStorage.activePack[creatureIndex].species.model, transform);

            //Set the layers so it is only visible to its portrait camera
            foreach (Transform trans in _PortraitDummies[creatureIndex].GetComponentsInChildren<Transform>(true))
                trans.gameObject.layer = _Layers[creatureIndex];
        }
    }

    private void OnDestroy()
    {
        GameManager.playerCreatureAddedEvent -= CreateDummy;
    }
}
