using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System;
using UnityEngine.Events;
using SpeechLib;

	[System.Serializable]
	public class ElevatorSounds {
		public AudioClip ElevatorStartMoving;
		public AudioClip ElevatorStop;
	}

	[RequireComponent (typeof (AudioSource))]
public class Elevator : MonoBehaviour
{
    public float maxSpeed=0.05f;//
    public float accelerate=0.01f;
    //public Text heightText;
    public TMPro.TextMeshProUGUI heightText;
    //cameraFollow camerafollow;
    public GameObject ele_Button;
    public GameObject qs_Button;
    public GameObject numpad;
    //public GameObject canvasInput;
    public UnityEvent ask;
    public List<float> heights;
    public ElevatorSounds ElevatorSounds =  new ElevatorSounds();	

    [HideInInspector]
    public bool isElevatorMoving=true;
    
    string elevatorDirection;    
    protected Experiment exp;
    //Rigidbody rb;
    protected float height;
    int level=0;
    int des=0;
    //public List<Vector2> test;
    float v0=0f;
    Vector3 velocity= Vector3.zero;

    private bool soundplayed;
	private bool m_soundplayed;

    // Start is called before the first frame update
    void Start(){
    	//rb=this.GetComponent<Rigidbody>();
		//exp = otherCamera.GetComponent<Experiment>();
		height=transform.position.y;
		UpdateHeight();
        exp= this.GetComponent<Experiment>();
        numpad.SetActive(false);
        //canvasInput.SetActive(true);
        //isElevatorMoving=true;
        //this.GetComponent<AudioSource>().Stop();
    }
    public void ElevatorGO (string eleDirection) {
        if(isElevatorMoving) return;
		elevatorDirection = eleDirection;   
		print("current level: "+level);

		if(elevatorDirection == "ElevatorUp"  && level<heights.Count-1)
		{
            des=heights.FindIndex(x => x>height)>-1?heights.FindIndex(x => x>height):heights.Count-1;
			//level += 1;     
            isElevatorMoving = true;
		}
		if(elevatorDirection == "ElevatorDown"  && level>=1)
		{
            des=heights.FindIndex(x => x<height)>-1?heights.FindLastIndex(x => x<height):0;
			//level -= 1;
            //print("NEXT--"+heights.FindIndex(x => x<height));
            isElevatorMoving = true;
		}
        //print("destination:"+level);
        print("destination-level: "+des);
		soundplayed = false;
		m_soundplayed = false;
        //isElevatorMoving = true;
	}

	protected void UpdateHeight(){
        heightText.text="Height:"+ Math.Round(height,1).ToString()+" m";
        if (height>50f){
            heightText.text+="   You are out!";//"\r\n"+
        }
	}

    // Update is called once per frame
    void Update(){
        //show CRT
        // if(Input.GetKeyDown(KeyCode.Q)& !isElevatorMoving){
        //     //ask.Invoke();
        //     //qs_Button.SetActive(false);
        //     GetQuestion();
        // }

        if(Input.GetKeyDown(KeyCode.O)&level>0&qs_Button.GetComponent<Image>().enabled){//&exp.canvaQS.transform.Find("Panel/ButtonL").gameObject.activeSelf!=true
            ElevatorGO("ElevatorDown");
        }
        if(Input.GetKeyDown(KeyCode.P)&level<=2&qs_Button.GetComponent<Image>().enabled){
            ElevatorGO("ElevatorUp");
        }
        UpdateHeight();
    }
    void FixedUpdate()
    {
        height = transform.position.y;
        // if(Input.GetKeyDown(KeyCode.M)){
        //     rb.MovePosition(transform.position + Vector3.up*heights[level]);
        // //transform.position += maxSpeed*transform.up*Time.deltaTime;
        // if(height < 6f){
        //     Vector3 destination = transform.position + Vector3.up*maxSpeed*Time.deltaTime;
    	//     //rb.AddForce(movement * maxSpeed);
        //     rb.MovePosition(destination.magnitude < new Vector3(0.0f,6f,0.0f).magnitude?destination :new Vector3(0.0f,6f,0.0f));
        // }
		// }
        // if(Input.GetKey(KeyCode.H)){
        // if(height < 20f &height >=6f){
        // rb.MovePosition(transform.position + Vector3.up*maxSpeed*Time.deltaTime);
        // }}

    //move up SIGNAL
    // if( new string[]{"ElevatorUp","ElevatorDown"}.Contains(elevatorDirection)){
    //     //print("contains");
	// 	isElevatorMoving = true;
    //     //transform.position = Vector3.MoveTowards(transform.position, Vector3.up*heights[level], maxSpeed);
    // }
    //move down SIGNAL
    // if(elevatorDirection == "ElevatorDown"){
	// 	//isElevatorMoving = true;
	// 	//transform.position = Vector3.MoveTowards(transform.position, Vector3.up*heights[level], maxSpeed);
	// }
    //stop at next height, and set moving statu to false
    if (isElevatorMoving){
        //print("velocity: "+v0);
        //print(transform.position.y);
         if(!m_soundplayed){
			this.GetComponent<AudioSource>().clip = ElevatorSounds.ElevatorStartMoving;
			this.GetComponent<AudioSource>().Play();
		 	m_soundplayed = true;
		 }
        v0 += Time.fixedDeltaTime*accelerate;//accelerate per fixedDeltaTime slower than gravity(9.81f)
        //transform.position = Vector3.MoveTowards(transform.position, Vector3.up*heights[des], v0<maxSpeed? v0:maxSpeed);
        //transform linear v0 by distance into % increasing every fixedUpdate(), then reach target at t-temp= 100% 
        float p_temp= 50*v0/Math.Abs(heights[des]-heights[level]);//
        p_temp=p_temp>1?1:p_temp;
        //print(p_temp);
        transform.position = Vector3.Lerp(Vector3.up*heights[level], Vector3.up*heights[des],  p_temp*p_temp * (3f - 2f*p_temp));//sigmoid function on target distance so on speed /p_temp<1?p_temp:1/  
        //transform.position = Vector3.SmoothDamp(transform.position, Vector3.up*heights[des], ref velocity, Math.Abs(heights[level]-heights[des])/(maxSpeed*50)-v0*25);//
        //rb.MovePosition(v0<1f?v0*Vector3.up*heights[des]:Vector3.up*heights[des]);
        numpad.SetActive(false);
        exp.qs_text.text="Please wait when elevator is running.";
        if(this.transform.position.y == heights[des]){
            //Debug.Log("tempheight"+heights[level]+"-"+this.transform.position);
            //if(!soundplayed){
                this.GetComponent<AudioSource>().Stop();
                this.GetComponent<AudioSource>().PlayOneShot(ElevatorSounds.ElevatorStop);
            //    soundplayed = true;
            //}
            level=des;//sync level with destination
		    isElevatorMoving = false;
            ElevatorGO("");
            //elevatorDirection="";
            v0=0f;
            qs_Button.SetActive(true);
            exp.PrepareQuestion(level,heights);
            // exp.qs_text.text="If you are ready, click 'Start' button with your laser pointer to reveal the question and options.";            
            // 
            // qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text="Start";   
            // qs_Button.tag="QS";

            //ele_Button.SetActive(true);
            //ele_Button.GetComponent<Renderer>().enabled = false;
            //qs_Button.GetComponent<Image>().enabled = true;

            
	    }
        }    
    }

    public void GetQuestion(){
        // if (exp.GetVoice().Status.RunningState ==  SpeechRunState.SRSEIsSpeaking)
        // {
        //     return;
        // }

        if(level+2>heights.Count()) {exp.qs_text.text="This is the end of the experiment.\n Thank you! ";return;}
        //qs_Button.SetActive(false);
        qs_Button.GetComponent<Image>().enabled = false;
        qs_Button.GetComponent<Button>().interactable = false;
        qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text="";
        //ele_Button.SetActive(true);
        //ele_Button.GetComponent<Renderer>().enabled = false;
        //numpad.SetActive(true);
        //canvasInput.SetActive(true);
        exp.Question(level);
    }

    void OnCollisionEnter(Collision collision){
        print("colliderEnter"+collision.gameObject.name);
        collision.gameObject.transform.parent=this.gameObject.transform;
        if(collision.gameObject.CompareTag("Player")){
            print("Player colliderEnter");
            //collision.gameObject.transform.root.parent=this.gameObject.transform;
    }}
    void OnCollisionStay(Collision other)
	{
        if(other.gameObject.CompareTag("Player")){
            print ("Player colliderStay");
        other.gameObject.transform.parent=this.gameObject.transform;
        }
        //else print(other.gameObject.name);
	}
	void OnCollisionExit(Collision other)
	{
        print("colliderExit"+other.gameObject.name);
        other.gameObject.transform.parent=null;
        if(other.gameObject.CompareTag("Player")){
            print ("Player colliderExit."); 
            //other.gameObject.transform.root.parent=null;
        }
	}

    void OnControllerColliderHit(ControllerColliderHit hit) {
        print("OnControllerColliderHit");
    }
    // void OnDestroy() {
    //     exp.WriteToFile();
    // }
}
