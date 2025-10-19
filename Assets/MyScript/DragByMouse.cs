using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.Utility;

public class DragByMouse : MonoBehaviour
    , IColliderEventPressEnterHandler
    , IColliderEventPressExitHandler
    , IColliderEventHoverEnterHandler
    , IColliderEventHoverExitHandler
{
    private Vector3 mOffset;
    private float mZCoord;

    private Material normal;
    public Material heightlight;
    public Material hover;
    private readonly static List<Renderer> s_rederers = new List<Renderer>();

    public TMPro.TextMeshProUGUI debugText;

    void Start () {
        normal = GetComponent<MeshRenderer>().material;
    }
    void OnMouseDown()
    {
        debugText.text="clicked"+gameObject.name;
        mZCoord = Camera.main.WorldToScreenPoint(
            gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos

        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();

    }


    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;
        // z coordinate of game object on screen
        mousePoint.z = mZCoord;
        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseAsWorldPoint() + mOffset;
    }


    void OnCollisionEnter(Collision collision)
    {
        //print("colliderEnter");
        //debugText.text+="entering"+gameObject.name;
        //SetChildRendererMaterial(heightlight);
    }
    void OnCollisionStay(Collision other){

            //print ("colliderStay");
            SetChildRendererMaterial(heightlight);
	}
	void OnCollisionExit(Collision other){

           //print ("colliderExit."); 
           SetChildRendererMaterial(normal);
	}


    public void OnColliderEventPressEnter(ColliderButtonEventData eventData)
    {
        SetChildRendererMaterial(heightlight);
    }

    public void OnColliderEventPressExit(ColliderButtonEventData eventData)
    {
        SetChildRendererMaterial(normal);
    }

    public void OnColliderEventHoverEnter(ColliderHoverEventData eventData)
    {
       SetChildRendererMaterial(hover);
    }

    public void OnColliderEventHoverExit(ColliderHoverEventData eventData)
    {
        SetChildRendererMaterial(normal);
    }

    private void SetChildRendererMaterial(Material targetMat)
    {
        GetComponentsInChildren(true, s_rederers);

        if (s_rederers.Count > 0)
        {
            for (int i = s_rederers.Count - 1; i >= 0; --i)
            {
                s_rederers[i].sharedMaterial = targetMat;
            }

            s_rederers.Clear();
        }
    }
}
