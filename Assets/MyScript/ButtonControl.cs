using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using UnityEngine;
using HTC.UnityPlugin.ColliderEvent;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour    
	, IColliderEventPressEnterHandler
    , IColliderEventPressExitHandler
	, IPointerClickHandler // 2
{
    //public GameObject m_ElevatorController;
	public GameObject platform;
	public GameObject numpad;
	public TMPro.TextMeshProUGUI debugText;

	[HideInInspector]
	//public bool is_elevatorMoving;
	dynamic elevator;//Elevator
	

	// Start is called before the first frame update
	void Start()
    {
		// gameObject.SetActive(!gameObject.CompareTag ("ElevatorUp"));
		//gameObject.GetComponent<Renderer>().enabled = false;
		if (platform.GetComponent<Elevator>()!=null &&platform.GetComponent<Elevator>().isActiveAndEnabled)
		{
			print("ElevatorElevatorElevator");
			elevator = platform.GetComponent<Elevator>();
		}else 
		if (platform.GetComponent<ElevatorAnchor>().isActiveAndEnabled)
		{
			elevator =platform.GetComponent<ElevatorAnchor>();
		}
		//elevator = GameObject.Find("Platform").GetComponent<Elevator>()??GameObject.Find("Platform").GetComponent<ElevatorAnchor>(); 
		//elevator = m_ElevatorController.GetComponent<Elevator>();
		//is_elevatorMoving = elevator.isElevatorMoving;
    }


	public void ButtonPressed() {
		if(!elevator.isElevatorMoving){
		print("pressed--"+gameObject.tag);
		if(gameObject.CompareTag ("ElevatorUp")){
			//SendCall("ElevatorUp");
			elevator.ElevatorGO("ElevatorUp");
			//gameObject.SetActive(false);
		}
		if(gameObject.CompareTag ("QS")){
			//SendCall("QS");
			elevator.GetQuestion();
			this.transform.localScale = new Vector3(.0000001f,.0000001f,.0000001f);
			//gameObject.SetActive(false);
		}
		// if(gameObject.CompareTag ("QS2")){
		// 	//SendCall("QS");
		// 	elevator.GetQuestion2();
		// 	//gameObject.SetActive(false);
		// }
		if (gameObject.CompareTag ("Finish")){
			#if UNITY_EDITOR
			// Application.Quit() does not work in the editor so
			// UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit();
			#endif
		}
		}	
    }

    //HIDE WHEN PLATFORM IS MOVING
    void FixedUpdate()
    {
		//print(Time.deltaTime+"--"+GameObject.Find("Platform").GetComponent<Elevator>().isElevatorMoving);
		//hide button when moving
        // if (elevator.isElevatorMoving){//GameObject.Find("Platform").GetComponent<Elevator>()
		// 	//print (elevator.isElevatorMoving+"HIDE");
		// 	//gameObject.GetComponent<Renderer>().enabled = false;
        // 	gameObject.SetActive(false);
		// 	//numpad.SetActive(false);
		// }else{

		// }

		// var ray = new Ray(this.transform.position,this.transform.forward);
		// RaycastHit hit;
		// if(Physics.Raycast(ray,out hit)){
		// 	debugText.text="hit";
		// 	if(hit.transform.gameObject.CompareTag ("QS")){
		// 	SendCall("QS");}
		// }else{
		// 	debugText.text="NA";
		// }
    }
	public void OnPointerClick(PointerEventData eventData) // 3
    {
        //input = display.text;
        //display.text=input+"clicked";
		if(GetComponent<Button>().interactable==false) return;
        ButtonPressed();
        //print("OnPointerClick");
    }

	public void OnColliderEventPressEnter(ColliderButtonEventData eventData)
    {
		if(GetComponent<Button>().interactable==false) return;
		ButtonPressed();
	}
	    public void OnColliderEventPressExit(ColliderButtonEventData eventData)
    {
    }

	// public void SendCall (string Call) {
	// 	//elevator = GameObject.Find("Platform").GetComponent<Elevator>();
	// 	if (!elevator.isElevatorMoving)
	// 	{
	// 		if(Call == "ElevatorUp"  )
	// 		{
	// 		elevator.ElevatorGO("ElevatorUp");
	// 		}
	// 		else if(Call == "ElevatorDown" )
	// 		{
	// 		elevator.ElevatorGO("ElevatorDown");
	// 		}
	// 		else if(Call == "QS" )
	// 		{
	// 		elevator.GetQuestion();
	// 		}
	// 	}
	// }
	void OnMouseDown(){
        // this object was clicked - do something
		print(elevator.isElevatorMoving + "clicked--"+gameObject.tag);	
		debugText.text="mousedown";
		// if(gameObject.CompareTag ("ElevatorUp")){
		// 	SendCall("ElevatorUp");
		// }
		// if(gameObject.CompareTag ("QS")){
		// 	SendCall("QS");
		// }
		//this.enable=false;
	}   

	void OnMouseOver(){
		print(elevator.isElevatorMoving + "OnMouseOver--"+gameObject.tag);	
		debugText.text="OnMouseOver";
		// if(gameObject.CompareTag ("ElevatorUp")){
		// 	SendCall("ElevatorUp");
		// }
		// if(gameObject.CompareTag ("QS")){
		// 	SendCall("QS");
		// }
	}
}
