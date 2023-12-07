using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DissolveParticlesState
{
    DISSOLVING,
    REINTEGRATING
}

public class DissolveParticlesManager: MonoBehaviour
{
    public GameObject _Targets;

    private GameObject _InitialCreature;
    private GameObject _FinalCreature;
    private SkinnedMeshRenderer[] _InitialMeshRenderers;
    private SkinnedMeshRenderer[] _FinalMeshRenderers;

    private ParticleSystem m_System;

    // Materials and properties
    public Material _ParticleMaterial;
    private List<Material> _InitialCreatureMaterials;
    private List<Material> _FinalCreatureMaterials;
    private int _ParticleMaterialHeightProperty;
    private int _MeshMaterialHeightProperty;


    private float _MinHeight = 0f;
    private float _MaxHeight = 8f;

    // Shaders
    [SerializeField] private Shader _MorphShader;
    [SerializeField] private Shader _ShaderToReplace;
    [SerializeField] private Shader _MorphOutlineShader;
    [SerializeField] private Shader _ShaderOutlineToReplace;

    // States
    public float _TotalTime = 5f;
    private float _CurrentTime;

    private DissolveParticlesState _State;


	void Awake ()
    {
        m_System = GetComponent<ParticleSystem>();

        //Get properties IDs
        _ParticleMaterialHeightProperty = Shader.PropertyToID("_Height");
        _MeshMaterialHeightProperty = Shader.PropertyToID("_DissolveHeight");
    }

    private void OnEnable()
    {
        BeginMorph(_Targets, 10f);
    }

    void BeginMorph( GameObject targets, float duration)
    {
        if ( targets.transform.childCount < 2 )
        {
            Debug.LogError("Expected the target to have 2 children to morph from the first to the second");
            return;
        }

        //Get GAMEOBJECTS
        _InitialCreature = targets.transform.GetChild(0).gameObject;
        _FinalCreature   = targets.transform.GetChild(1).gameObject;

        //Get RENDERERS
        _InitialMeshRenderers = _InitialCreature.GetComponentsInChildren<SkinnedMeshRenderer>();
        _FinalMeshRenderers   = _FinalCreature.GetComponentsInChildren<SkinnedMeshRenderer>();

        //Get MATERIALS
        _InitialCreatureMaterials = GetMaterials(_InitialMeshRenderers);
        _FinalCreatureMaterials   = GetMaterials(_FinalMeshRenderers);

        //Replace SHADERS
        SetShaders(_InitialCreatureMaterials);
        SetShaders(_FinalCreatureMaterials);

        _MinHeight = transform.position.y - 3;

        _TotalTime = duration / 2f;

        BeginDissolve();
    }


    void BeginDissolve()
    {
        List<Vector3> meshVertex = GetVertex(_InitialMeshRenderers);
        ConfigureParticleSystem(meshVertex);

        _FinalCreature.SetActive(false);
        _InitialCreature.SetActive(true);

        _State = DissolveParticlesState.DISSOLVING;
        _CurrentTime = _TotalTime;
    }


    void BeginReintegration()
    {
        List<Vector3> meshVertex = GetVertex(_FinalMeshRenderers);
        ConfigureParticleSystem(meshVertex);

        _InitialCreature.SetActive(false);
        _FinalCreature.SetActive(true);

        _State = DissolveParticlesState.REINTEGRATING;
        _CurrentTime = 0;

        Update();
    }


	void Update ()
    {
        if (_State == DissolveParticlesState.DISSOLVING)
        {
            if (_CurrentTime > 0)
            {
                float currentHeight = Mathf.Lerp(_MinHeight, _MaxHeight + _MinHeight, _CurrentTime / _TotalTime);

                Debug.DrawLine(new Vector3(-1, currentHeight, 0), new Vector3(1, currentHeight, 0));

                _ParticleMaterial.SetFloat(_ParticleMaterialHeightProperty, currentHeight);
                foreach (Material material in _InitialCreatureMaterials)
                    material.SetFloat(_MeshMaterialHeightProperty, currentHeight);

                _CurrentTime -= Time.deltaTime;
            }
            else
                BeginReintegration();
        }
        else if (_State == DissolveParticlesState.REINTEGRATING)
        {
            if (_CurrentTime < _TotalTime)
            {
                float currentHeight = Mathf.Lerp(_MinHeight, _MaxHeight + _MinHeight, _CurrentTime / _TotalTime);

                Debug.DrawLine(new Vector3(-1, currentHeight, 0), new Vector3(1, currentHeight, 0));

                _ParticleMaterial.SetFloat(_ParticleMaterialHeightProperty, currentHeight);
                foreach (Material material in _FinalCreatureMaterials)
                    material.SetFloat(_MeshMaterialHeightProperty, currentHeight);

                _CurrentTime += Time.deltaTime;
            }
            else
                ResetAndDisable();
        }
    }

    void ResetAndDisable()
    {
        RestoreShaders(_InitialCreatureMaterials);
        RestoreShaders(_FinalCreatureMaterials);

        this.enabled = false;
    }

    #region SELF SERVICE TOOLS

    List<Vector3> GetVertex( SkinnedMeshRenderer[] skinnedMeshRenderers)
    {
        Mesh mesh = new Mesh();

        List<Vector3> meshVertex = new List<Vector3>();
        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            int oldVertexCount = meshVertex.Count;
            renderer.BakeMesh(mesh);
            meshVertex.AddRange(mesh.vertices);

            for (int i = oldVertexCount; i < meshVertex.Count; i++)
            {
                Vector3 modifiedVertex;
                modifiedVertex.x = meshVertex[i].x / renderer.transform.lossyScale.x;
                modifiedVertex.y = meshVertex[i].y / renderer.transform.lossyScale.y;
                modifiedVertex.z = meshVertex[i].z / renderer.transform.lossyScale.z;
                meshVertex[i] = renderer.transform.TransformPoint(modifiedVertex);
            }

            /*
            if (mesh.bounds.max.y > _MaxHeight)
                _MaxHeight = mesh.bounds.max.y;*/
        }
        return meshVertex;
    }

    void ConfigureParticleSystem(List<Vector3> meshVertex)
    {
        List<Vector4> customData = new List<Vector4>();

        m_System.GetCustomParticleData(customData, 0);

        // Adjust vertex and particle count
        if (customData.Count < meshVertex.Count)
        { //If there are not enough particles, emit more
            print(meshVertex.Count + " " + customData.Count);
            m_System.Emit(meshVertex.Count - customData.Count);
            m_System.GetCustomParticleData(customData, 0);
        }
        else if (customData.Count > meshVertex.Count)
        { //If there are not enough vertex, start again sending particles from the first vertex onward
            int initialSize = meshVertex.Count;
            for ( int i = meshVertex.Count; i < customData.Count; i++ )
                meshVertex.Add(meshVertex[i - initialSize]);
        }

        for (int i = 0; i < customData.Count; i++)
        {
                customData[i] = new Vector4(meshVertex[i].x ,
                                            meshVertex[i].y ,
                                            meshVertex[i].z , 0.0f);
        }

        m_System.SetCustomParticleData(customData, 0);
    }

    List<Material> GetMaterials( SkinnedMeshRenderer[] renderers )
    {
        List<Material> materialsToReturn = new List<Material>();

        foreach (SkinnedMeshRenderer smr in renderers)
            foreach (Material material in smr.materials)
                    materialsToReturn.Add(material);

        return materialsToReturn;
    }

    void SetShaders ( List<Material> materials )
    {
        ReplaceShaders(materials, _ShaderOutlineToReplace, _MorphOutlineShader);
        ReplaceShaders(materials, _ShaderToReplace, _MorphShader);
    }

    void RestoreShaders ( List<Material> materials )
    {
        ReplaceShaders(materials, _MorphOutlineShader, _ShaderOutlineToReplace);
        ReplaceShaders(materials, _MorphShader, _ShaderToReplace);
    }

    void ReplaceShaders(List<Material> materials, Shader shaderToReplace, Shader replacementShader)
    {
        foreach (Material material in materials)
        {
            if (material.shader == shaderToReplace)
                material.shader = replacementShader;
        }
    }
    #endregion
}
