using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using Valve.VR;

	// [System.Serializable]
	// public class ElevatorSounds {
	// 	public AudioClip ElevatorStartMoving;
	// 	public AudioClip ElevatorStop;
	// }

	// [RequireComponent (typeof (AudioSource))]

public class ElevatorAnchor : MonoBehaviour
{
    public float maxSpeed=0.05f;//
    public float accelerate=0.025f;
    //public Text heightText;
    public TMPro.TextMeshProUGUI heightText;
    //cameraFollow camerafollow;
    public GameObject ele_Button;
    public GameObject qs_Button;
    public GameObject player;
    public GameObject blackPanel;
    public GameObject CanvasEnvironment;
    //public GameObject numpad;
    //public GameObject canvasInput;

    public List<float> heights;
    public ElevatorSounds ElevatorSounds =  new ElevatorSounds();	

    [HideInInspector]
    public bool isElevatorMoving=true;
    
    string elevatorDirection;    
    protected ExpAnchor exp;
    //Rigidbody rb;
    protected float height;
    int level=0;
    int des=0;
    //public List<Vector2> test;
    float v0=0f;
    Vector3 velocity= Vector3.zero;
    public float blindDuration=3f;

    private bool soundplayed;
	private bool m_soundplayed;
    public SteamVR_Action_Vibration vibe;
    // Start is called before the first frame update
    void Start()
    {
        //Elevator ele = new Elevator();
        height=transform.position.y;
		UpdateHeight();
        exp= this.GetComponent<ExpAnchor>();
        player.transform.parent=this.gameObject.transform;

        //print(exp.qs_list.Count());
        //Debug.Log("exp.height list: "+string.Join(", ", exp.qs_list.Select(p => p.height).ToList())); 
        //heights=exp.qs_list.Select(p => (float)p.height).ToList();
        //Debug.Log("heights list: "+string.Join(", ", heights));    
        //foreach(var node in heights){//exp.qs_list.Select(p => (float)p.height).ToList()
        //    Debug.Log("aaa node split----"+node);} 
       
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O)&level>0&qs_Button.GetComponent<Image>().enabled){//&exp.canvaQS.transform.Find("Panel/ButtonL").gameObject.activeSelf!=true
            ElevatorGO("ElevatorDown");
        }
        if(Input.GetKeyDown(KeyCode.P)&level<heights.Count-1&qs_Button.GetComponent<Image>().enabled){
            qs_Button.GetComponent<Button>().onClick.Invoke(); 
            ElevatorGO("ElevatorUp");
        }
        UpdateHeight();
    }

    void FixedUpdate()
    {
        height = transform.position.y;
    
        // if(blindDuration<=0) {
        //     blackPanel.SetActive(false);
        //     print("black end with "+blindDuration);
        //     blindDuration=3f;
        // }
        // blindDuration-=Time.fixedDeltaTime;
    //stop at next height, and set moving statu to false
    //print(isElevatorMoving);
        if (isElevatorMoving){
            //blackPanel.SetActive(true);
            StartCoroutine(RemoveAfterSeconds(blindDuration, blackPanel));
            //print("blindDuration: "+blindDuration);
        //print("velocity: "+v0);
        //print(transform.position.y);
        //  if(!m_soundplayed){
		// 	this.GetComponent<AudioSource>().clip = ElevatorSounds.ElevatorStartMoving;
		// 	this.GetComponent<AudioSource>().Play();
		//  	m_soundplayed = true;
		//  }
        v0 += Time.fixedDeltaTime*accelerate;//accelerate per fixedDeltaTime slower than gravity(9.81f)
        //transform.position = Vector3.MoveTowards(transform.position, Vector3.up*heights[des], v0<maxSpeed? v0:maxSpeed);
        //transform linear v0 by distance into % increasing every fixedUpdate(), then reach target at t-temp= 100% 
        float p_temp= 50*v0/Math.Abs(heights[des]-heights[level]);//
        p_temp=p_temp>1?1:p_temp;
        //print(p_temp);
        //transform.position = Vector3.Lerp(Vector3.up*heights[level], Vector3.up*heights[des],  p_temp*p_temp * (3f - 2f*p_temp));//sigmoid function on target distance so on speed /p_temp<1?p_temp:1/  
        transform.position = Vector3.Lerp(Vector3.up*heights[level], Vector3.up*heights[des], 1f);
        //transform.position = Vector3.SmoothDamp(transform.position, Vector3.up*heights[des], ref velocity, Math.Abs(heights[level]-heights[des])/(maxSpeed*50)-v0*25);//
        //rb.MovePosition(v0<1f?v0*Vector3.up*heights[des]:Vector3.up*heights[des]);
        //numpad.SetActive(false);
        exp.qs_text.text="Please wait when elevator is running.";
        if(SteamVR.instance!=null){
            vibe.Execute(0,Time.fixedDeltaTime,60,1,SteamVR_Input_Sources.RightHand);
        }
        
        if(this.transform.position.y == heights[des]){
            //Debug.Log("tempheight"+heights[level]+"-"+this.transform.position);
            //if(!soundplayed){
                //this.GetComponent<AudioSource>().Stop();
                //this.GetComponent<AudioSource>().PlayOneShot(ElevatorSounds.ElevatorStop);
            //    soundplayed = true;
            //}
            level=des;//sync level with destination
		    isElevatorMoving = false;
            ElevatorGO("");
            //elevatorDirection="";
            v0=0f;
            qs_Button.SetActive(true);


            // foreach(var go in GameObject.Find ("Trees").GetComponentsInChildren<Transform>(true)){//GameObject.FindGameObjectsWithTag("height_ref")
            //     if (go.tag=="height_ref")
            //     {
            //         go.gameObject.SetActive(true);
            //     } 
            // }
            // foreach(var go in GameObject.Find ("Terrace").GetComponentsInChildren<Transform>(true)){//GameObject.FindGameObjectsWithTag("height_ref")
            //     if (go.tag=="height_ref")
            //     {
            //         go.gameObject.SetActive(true);
            //     } 
            // }
            print("environment_ref:"+exp.qs_list[level+1].environment_ref);//.Where(a=>a.question.Contains("Are you")).ToList()
            if (exp.qs_list[level+1].environment_ref=="low"){//.Where(a=>a.question.Contains("Are you")).ToList()
                foreach(var go in GameObject.FindGameObjectsWithTag("height_ref")){
                    //print(go.transform.parent.gameObject.name);
                    if (go.transform.parent.gameObject.name=="Grass")
                    {
                        if(go.transform.position.Equals(new Vector3(0f,0f,0f))){
                            if (go.name.Contains("N") ){
                                go.transform.position=go.transform.position+new Vector3(0f,0f,56f); 
                            } else 
                                go.transform.position=go.transform.position+new Vector3(0f,0f,-56f);
                            if(go.name.Contains('W') ){
                                go.transform.position=go.transform.position+new Vector3(-40f,0f,0f);
                            } else
                                go.transform.position=go.transform.position+new Vector3(40f,0f,0f);
                        }    
                        //go.transform.position=go.transform.position*2;
                    } else if (go.transform.parent.name=="Trees"){
                        foreach(var renderer in go.GetComponentsInChildren<Renderer>()){
                            renderer.enabled=false;
                        }
                    }
                    else {
                        go.GetComponent<Renderer>().enabled=false;//SetActive(false);
                    }
                }
            } else if (exp.qs_list[level+1].environment_ref=="high"){//.Where(a=>a.question.Contains("Are you")).ToList()
            //print(" CanvasEnvironment.GetComponentInChildren<RawImage>()"+ CanvasEnvironment.GetComponentInChildren<Image>().sprite);
            //CanvasEnvironment.GetComponentInChildren<Image>().sprite = Resources.Load <Sprite>(exp.qs_list[level+1].height.ToString());
            //CanvasEnvironment.GetComponentInChildren<Image>().color=Color.white;
                foreach(var go in GameObject.FindGameObjectsWithTag("height_ref")){
                    if (go.transform.parent.gameObject.name=="Grass")
                    {
                        go.transform.position=new Vector3(0f,0f,0f);
                        // if (level!=0){
                        //     //go.transform.position=go.transform.position*1/2;
                        //     if (go.name.Contains("N")){
                        //         go.transform.position=go.transform.position+new Vector3(0f,0f,-56f);
                                
                        //     } else 
                        //         go.transform.position=go.transform.position+new Vector3(0f,0f,56f);
                        //     if(go.name.Contains('W')){
                        //         go.transform.position=go.transform.position+new Vector3(40f,0f,0f);
                        //     } else
                        //         go.transform.position=go.transform.position+new Vector3(-40f,0f,0f);
                        //     }
                        
                    }else if (go.transform.parent.name=="Trees"){
                        foreach(var renderer in go.GetComponentsInChildren<Renderer>()){
                            renderer.enabled=true;
                        }
                    } else {
                        go.GetComponent<Renderer>().enabled=true;//.SetActive(true);
                    }   
                } 
            }
            exp.PrepareQuestion(level,heights);
            //ScreenCapture.CaptureScreenshot(exp.qs_list[level+1].height.ToString()+"_"+(exp.qs_list[level+1].environment_ref=="high"?"1":"0")+".png");//"_"+(exp.qs_list[level+1].anchor_type=="high"?"1":"0")+
	    }
        }    
    }

    public void GetQuestion(){
        // qs_Button.GetComponent<Image>().enabled = false;
        // qs_Button.GetComponent<Button>().interactable = false;
        // qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text="";
        //qs_Button.SetActive(false);
        try
        {
            exp.ShowQuestion().Start();
        }
        catch (Exception ex)
        {
            if (ex.GetType().ToString() == "System.InvalidOperation" | ex.GetType().ToString() == "System.InvalidOperationException") { }
            else print($"Task aborted. ErrorType={ex.GetType()} ErrorMessage={ex.Message}");
        }

        //exp.Question(level);
    }
    // public void GetQuestion2(){
    //     // qs_Button.GetComponent<Image>().enabled = false;
    //     // qs_Button.GetComponent<Button>().interactable = false;
    //     // qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text="";

    //     qs_Button.SetActive(false);
    //     exp.Question(level);
    // }
            
    public void ElevatorGO (string eleDirection) {//override
        if(isElevatorMoving) return;
		elevatorDirection = eleDirection;   
		print("current level: "+level);
        //heights[0]=8f;
        //heights[1]=12f;
		if(elevatorDirection == "ElevatorUp"  && level<heights.Count-1)
		{
            //des=heights.FindIndex(x => x>height)>-1?heights.FindIndex(x => x>height):heights.Count-1;
            des++;
			//level += 1;     
            isElevatorMoving = true;
            //blindDuration=3f;
		}
		// if(elevatorDirection == "ElevatorDown"  && level>=1)
		// {
        //     des=heights.FindIndex(x => x<height)>-1?heights.FindLastIndex(x => x<height):0;
		// 	//level -= 1;
        //     //print("NEXT--"+heights.FindIndex(x => x<height));
        //     isElevatorMoving = true;
		// }
        //print("destination:"+level);
        print("destination level: "+des+"--height: "+heights[des]);
		soundplayed = false;
		m_soundplayed = false;
        //isElevatorMoving = true;
	}

    IEnumerator RemoveAfterSeconds (float countdown, GameObject obj){
        obj.SetActive(true);
        //print(Time.time);
        yield return new WaitForSeconds(countdown);
        obj.SetActive(false);
    }
    protected void UpdateHeight(){
        heightText.text="Height:"+ Math.Round(height,1).ToString()+" m";
        if (height>50f){
            heightText.text+="   You are out!";//"\r\n"+
        }
	}

    void OnCollisionEnter(Collision collision){
        print("colliderEnter"+collision.gameObject.name);
        collision.gameObject.transform.parent=this.gameObject.transform;
        if(collision.gameObject.CompareTag("Player")){
            print("Player colliderEnter");
            //collision.gameObject.transform.root.parent=this.gameObject.transform;
    }}
}
