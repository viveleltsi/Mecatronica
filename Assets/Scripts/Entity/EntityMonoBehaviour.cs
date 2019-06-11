using Assets.Scripts.Entity;
using Assets.Scripts.Manager;
using Dan.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMonoBehaviour : MonoBehaviour, ISelectableEntity
{

    [LinkToScriptableObject(typeof(Descriptor), "Descriptor")]
    public string DescriptorId;

    private Descriptor Descriptor
    {
        get
        {
            if (_descriptor == null)
                return _descriptor = ScriptableObjectManager.Instance.GetDescriptor(DescriptorId);
            else
                return _descriptor;
        }
    }

    private Descriptor _descriptor = null;

    [LinkToScriptableObject(typeof(AbstractScriptableObjectElement), "Object Element")]
    public string ElementId;

    private AbstractScriptableObjectElement Element
    {
        get
        {
            if (_element == null)
                return _element = ScriptableObjectManager.Instance.GetDescriptor(DescriptorId);
            else
                return _element;
        }
    }

    private AbstractScriptableObjectElement _element = null;

    protected void OnMouseUp()
    {
        Debug.Log($"click on me {Descriptor.EditorName}");
        SelectionManager.Instance.Select(this);
    }

    private void OnMouseEnter()
    {
        
    }

    public void Select()
    {
        Debug.Log("Select this entity");
    }

    public void Unselect()
    {
        Debug.Log("Unselect this entity");
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public AbstractScriptableObjectElement GetObjectElement()
    {
        throw new System.NotImplementedException();
    }
}
