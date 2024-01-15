using Unity.Collections;
using UnityEngine;

public class FlagWaveControl : MonoBehaviour
{

    private MaterialPropertyBlock _propertyBlock;
    [DisplayOnly]
    public float _strength;
    [DisplayOnly]
    public float _sinOrCos;
    private void Awake()
    {
        
        
    }

    // Start is called before the first frame update
    void Start()
    {
        var rd = GetComponent<SpriteRenderer>();
        
        _sinOrCos = Random.Range(0, 1f);
        _strength = Random.Range(1, 5f);

        _propertyBlock = new MaterialPropertyBlock();
        _propertyBlock.SetFloat("_strength",_strength);
        _propertyBlock.SetFloat("_SinOrCos",_sinOrCos);
        
        _propertyBlock.SetTexture("_MainTex",rd.sprite.texture);
        
        
        rd.SetPropertyBlock(_propertyBlock);
    }
}
