using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AIDriverBase : MonoBehaviour
{
    [ShowInInspector, SerializeField]
    private Possessable possessable = null;

    public virtual Possessable Possessable
    {
        get => possessable;
        set => possessable = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (possessable == null)
            possessable = GetComponent<Possessable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
