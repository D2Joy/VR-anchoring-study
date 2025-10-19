using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.IO;
using System.Linq;
using SpeechLib;
using UnityEngine.UI;
using System.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Buffers;
using System.Threading.Tasks;
using System.Threading;
using CsvHelper.Configuration.Attributes;


public class ExpAnchor : Experiment
{
    public List<dynamic> qs_list;//{get; set;}//=new List<dynamic>();
    protected List<List<dynamic>> qs_ll;
    public string inputcsv;
    public Vector3 buttonScale;
    // Start is called before the first frame update

    public override void Start()
    {
        buttonScale = qs_Button.transform.localScale;
        var config = new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };
        using (var csvR = new CsvReader(File.OpenText(path +@"\Assets\"+inputcsv), System.Globalization.CultureInfo.CurrentCulture)){
            //read stimuli into list
            qs_list = csvR.GetRecords<dynamic>().ToList();//
        }
        
        // int levels = qs_list.Where(q=>q.question.Contains("Are you")).Count()/qs_list.Where(q=>q.question.Contains("Are you")).Select(p => new { p.anchor_type, p.environment_ref }).Distinct().Count();
        // //print(levels);

        // //print(string.Join(", ",qs_list.Where(q=>q.question.Contains("Are you")).GroupBy(p => new { p.anchor_type, p.environment_ref }).ToList()[0]));
        // for(int i=0;i<levels;i++){
        //     float level_height=UnityEngine.Random.Range(5f, 15f);
 
        //     qs_list.Where(q=>q.question.Contains("Are you") &Convert.ToSingle(q.index) %levels==i).ToList().ForEach(a=>{a.height=level_height;});
        // }
        // foreach(var group in qs_list.Select(p=>p.environment_ref).Distinct()){
        //     qs_list.Where(a=> a.environment_ref==group)=qs_list.Where(a=> a.environment_ref==group).OrderBy(x => Guid.NewGuid()).ToList();
        // }

        // qs_list.Where(q=>q.question.Contains("Are you") ).Select((h) => new{fheight=Convert.ToSingle(h.height),fanchor_offset=Convert.ToSingle(h.anchor_offset)}).ToList().ForEach(a=>{a.low_anchor=a.height-a.anchor_offset;a.high_anchor=a.height+a.anchor_offset;});
        //randomize:
 
        //split raw stimuli list into nested list by environment_ref
        qs_ll = new List<List<dynamic>>(){
            qs_list.Where(x=> x.environment_ref=="low").ToList(),
            qs_list.Where(x=> x.environment_ref=="high").ToList(),
        };
        Directory.CreateDirectory(path + @"\vrdata_anchor\");
        if(Directory.GetFiles(path + @"\vrdata_anchor\").Length%2==0){
            qs_ll.Reverse();
        }
        // foreach(var nodee in qs_ll[0]){
        //     //print(nodee.Count);
        //     Debug.Log("qs_ll: "+string.Join(",\n ", nodee.height+"-"+nodee.question+"? "+nodee.anchor_type+"-"+nodee.environment_ref));//
        // } 
        //check same heights not adjacent, otherwise re-randomize
        randomize:
        //randomize sub list
        for(int i = 0; i < qs_ll.Count(); i++) {
            qs_ll[i]=qs_ll[i].OrderBy(x => Guid.NewGuid()).ToList();
        }   

        if (inputcsv=="spreadsheet_anchor.csv"){
        //interleave join 2 sub lists
            qs_list=qs_list.Where(a=>a.question.Contains("How high")).Concat(qs_ll[0].Zip(qs_ll[1], (a, b) => new[] { a, b }).SelectMany(p => p)).ToList();
            //qs_list=qs_list.OrderBy(x => x.anchor_type).ThenBy(x => x.environment_ref).ToList();
        } else {
            qs_list=qs_list.Where(a=>a.question.Contains("How high")).Concat(qs_ll.SelectMany(a=>a)).ToList();//
        }
        // foreach(var node in qs_list){
        //     Debug.Log("qs_list:"+node.index+"--"+node.height+"--"+node.anchor_offset);
        //     //Debug.Log("qs_list node split----");
        //     } 
        // // Debug.Log("qs_list split----");
        //qs_list=qs_list.OrderBy(x => Guid.NewGuid()).ToList();
        //get CRT new index
        List<float> qs_height_l=qs_list.Where(q=>q.question.Contains("Are you")).Select(h => Convert.ToSingle(h.height)).Cast<float>().ToList();
            //Debug.Log("qs_height_l: "+string.Join(", ", qs_height_l));
            //count consecutive duplicate
            //print(qs_height_l.Where((item,idx) => idx ==0||item ==qs_height_l[idx-1]).Count());//.Where(a=>a!=0)
        if (qs_height_l.Where((item,idx) => idx==0 ||item ==qs_height_l[idx-1]).Count() >1){
            //print(qs_height_l.Where((item,idx) => idx==0 ||item ==qs_height_l[idx-1]).Count()-1+" consecutive duplicate detected, re-randomize");
            goto randomize;
        }
        
        qs_ll = new List<List<dynamic>>();
        foreach(var node in qs_list){
            //Debug.Log("qs_list:"+node.index+"--"+node.question);
            if (node.question.Contains("Are you"))
            {
                // if (node.anchor_type=="low"){
                //     node.question=node.question+node.low_anchor+" m?";
                // } else if (node.anchor_type=="high"){
                //     node.question=node.question+node.high_anchor+" m?";
                // }
                //node.low_anchor=float.Parse(node.height)-float.Parse(node.anchor_offset);
                //node.high_anchor=float.Parse(node.height)+float.Parse(node.anchor_offset);
                node.anchor=float.Parse(node.height)+float.Parse(node.anchor_offset);//node.anchor_type=="high"?:float.Parse(node.height)-float.Parse(node.anchor_offset)
                
                var dict = (IDictionary<string,dynamic>)node;
                dict["question"] = dict["question"]+dict["anchor"]+" m?";//node.anchor_type+"_anchor"
                //print(dict["question"] as string);
                //node.question=dict["question"]as string;      

                dynamic q2= new ExpandoObject();
                //q2 = qs_list.Where(a=>a.question.Contains("How high") &a.anchor_type==node.anchor_type &a.environment_ref==node.environment_ref).Select(x => { var re=x; re.height = node.height; return re;}).ToList()[0];


                q2.index=qs_list.Where(a=>a.question.Contains("How high")).First().index;
                q2.height=float.Parse(node.height);
                q2.question=qs_list.Where(a=>a.question.Contains("How high")).First().question;
                q2.correct=Math.Round(float.Parse(node.height),2).ToString();
                q2.incorrect="";
                q2.anchor_type=node.anchor_type;
                q2.anchor_offset=node.anchor_offset;
                q2.environment_ref=node.environment_ref;

                //var q12= new List<dynamic>(){node, q2};    
                if (node.anchor_offset!="0"){
                    qs_ll.Add(new List<dynamic>(){node, q2});
                }else qs_ll.Add(new List<dynamic>(){q2});
                
                //qs_ll.Add(q12);
                //qs_ll=qs_ll.Append(q12).ToList();//new List<dynamic>{node, q2}.Select(x => { var re=x; re.height = node.height; return re; })
                //Debug.Log("qs_ll: "+string.Join(",\n ", new List<dynamic>{node, q2}.Select(x=>x.height+"-"+x.question+"-"+x.anchor_type+"-"+x.environment_ref).ToList()));
            }

            //Debug.Log("qs_list:"+node.index+"-"+node.anchor_type+"--"+node.height+"\n" +node.question);
            //Debug.Log("qs_list node split----");
        }
        Debug.Log("variable split----");
        foreach(var nodee in qs_ll){
            //print(nodee.Count);
                Debug.Log("qs_ll: "+string.Join(",\n ", nodee.Select(x=>x.height+"-"+x.question+x.anchor_offset+"-"+x.environment_ref).ToList()));//
        } 

        //qs_list= qs_list.OrderBy(x => Guid.NewGuid()).ToList();
        GameObject.Find("Platform").GetComponent<ElevatorAnchor>().heights = qs_height_l;//qs_list.Select(p =>Convert.ToSingle(p.height) ).Cast<float>().ToList();


        qs_text=canvaQS.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        foreach (var child in canvaQS.GetComponentsInChildren<Button>(true))
        {
            child.gameObject.SetActive(!child.gameObject.CompareTag("responseTag")&&child.gameObject.name!="Num.");
        }
        //canvaQS.transform.Find ("PanelDropdown").gameObject.SetActive(false);
        canvaQS.transform.Find("PanelScroller").gameObject.SetActive(false);
        canvaQS.transform.Find("PanelInput").gameObject.SetActive(false);

        // foreach (var child in canvaQS.transform.Find ("PanelDropdown").Cast<Transform>().SelectMany(t => t.GetComponents<Transform>()))//TMPro.TextMeshProUGUI
        // {
        //     //print(child.name);
        //     child.gameObject.SetActive(false);
        // }
        
        guid=Guid.NewGuid();
        voice = new SpVoice();
        stopwatch = new System.Diagnostics.Stopwatch();
        answers_li= new List<dynamic>();
            dynamic expando = new ExpandoObject();
            expando.date = System.DateTime.Now;//.ToString("yyyy/MM/dd HH:mm:ss")
            guid=Guid.NewGuid();
            shortuid=RandomIdGenerator.GetBase36(10);
            expando.id = shortuid;//guid.ToString("n");
            expando.height ="BEGIN";
        answers_li.Add(expando);

        stopwatch.Start();
    }

    // Update is called once per frame
    public override void Update()
    {//disabled, no need
        if (false& voice.Status.InputSentenceLength!=0 & voice.Status.RunningState == SpeechRunState.SRSEDone &stopwatch.IsRunning==false &qs<qs_ll[level].Count){// &!qs_Button.activeSelf &(qs_Button.tag=="QS2"||qs_Button.tag=="QS"&!qs_Button.GetComponent<Image>().enabled) 
                //timer.Start();
                voice.Speak(string.Empty, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
                qs_text.text= qs_ll[level][qs].question;
                canvaQS.transform.Find ("Panel/Text m").GetComponentInChildren<TMPro.TextMeshProUGUI>().text=Regex.Match(qs_ll[level][qs].question,"[1-9][0-9]? +m").ToString();
                //print(Regex.Match(qs_ll[level][qs].question,"\\d +m"));
                //shuffledCRTfloat.Parse()-
                stopwatch.Start();
                //1 close ended qusetion
                if (qs_ll[level][qs].question.StartsWith("Are you"))//incorrect.Length!=0 
                {
                    foreach (var child in canvaQS.GetComponentsInChildren<Button>(true))
                    {
                        child.gameObject.SetActive(child.gameObject.CompareTag("responseTag")&&child.gameObject.name!="Num.");
                        //print(child.gameObject.name);
                    }

                    var response = canvaQS.GetComponentsInChildren<Button>();
                    response=response.OrderBy(x => Guid.NewGuid()).ToArray();
                    response[0].GetComponentInChildren<TMPro.TextMeshProUGUI>().text=qs_ll[level][qs].incorrect;
                    response[1].GetComponentInChildren<TMPro.TextMeshProUGUI>().text=qs_ll[level][qs].correct;
                    //print(stopwatch.ElapsedMilliseconds);

                }  else if(qs_ll[level][qs].question.StartsWith("How high")) {//2 open ended qusetion
                    //print("How high question");
                    qs_Button.SetActive(false);
                    qs_Button.transform.localScale = new Vector3(.0000001f,.0000001f,.0000001f);
                    qs_Button.transform.position=qs_Button.transform.position-new Vector3(0f,2f,0f);
                    qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text="Submit";
                    //foreach (var child in canvaQS.transform.Find ("PanelL").Cast<Transform>().SelectMany(t => t.GetComponents<Transform>()))//.GetComponentsInChildren<Transform>(true))//
                    //{//print(child.name);
                    //    child.gameObject.SetActive(true);
                    //}
                    //canvaQS.transform.Find("PanelDropdown").gameObject.SetActive(true);
                    //canvaQS.transform.Find("PanelScroller").gameObject.SetActive(true);
                    canvaQS.transform.Find("PanelInput").gameObject.SetActive(true);

                    // foreach (var child in canvaQS.transform.Find ("PanelScroller"))//.GetComponentsInChildren<Transform>(true))////.SelectMany(t => t.GetComponents<Transform>()).Cast<Transform>()
                    // {//print(child.name);
                    //     child.gameObject.SetActive(true);
                    // }
                    // qs_Button.tag="responseTag";
                }

                //keyboard input monitor
                // if(Input.inputString!=null){
                //     StartCoroutine(ReceiveInput());
                // }
            }
            // if(Input.GetKeyDown(KeyCode.I)){
            //     ScreenCapture.CaptureScreenshot(qs_ll[level][qs].height.ToString()+"_"+(qs_ll[level][qs].environment_ref=="high"?"1":"0")+"_"+qs_ll[level][qs].question.StartsWith("Are you")+".png");
            // }
    }

    public async Task ShowQuestion(){
        qs_text.text = "...";
        Task task = new Task(() =>
        {     
            voice.Speak("ready"); 
        });
        try
        {
            task.Start();//
            await task;
        }
        catch (Exception ex)
        {
            print($"Task aborted. ErrorMessage={ex.Message} ErrorType={ex.GetType()} ");
            throw;
        }

        qs_text.text = qs_ll[level][qs].question;
        canvaQS.transform.Find("Panel/Text m").GetComponentInChildren<TMPro.TextMeshProUGUI>().text = Regex.Match(qs_ll[level][qs].question, "[1-9][0-9]? +m").ToString();
        //print(Regex.Match(qs_ll[level][qs].question,"\\d +m"));
        //shuffledCRT
        stopwatch.Start();
        //1 close ended qusetion
        if (qs_ll[level][qs].question.StartsWith("Are you"))//incorrect.Length!=0 
        {
            foreach (var child in canvaQS.GetComponentsInChildren<Button>(true))
            {
                child.gameObject.SetActive(child.gameObject.CompareTag("responseTag") && child.gameObject.name != "Num.");
                //print(child.gameObject.name);
            }

            var response = canvaQS.GetComponentsInChildren<Button>();
            response = response.OrderBy(x => Guid.NewGuid()).ToArray();
            response[0].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = qs_ll[level][qs].incorrect;
            response[1].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = qs_ll[level][qs].correct;

        }
        else if (qs_ll[level][qs].question.StartsWith("How high"))
        {//2 open ended qusetion (not run as no QS button click)
         print("How high question");
            //qs_Button.transform.localScale = new Vector3(.0000001f, .0000001f, .0000001f);
            qs_Button.transform.position = qs_Button.transform.position - new Vector3(0f, 2f, 0f);
            qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Submit";
            qs_Button.SetActive(false);
            
            //foreach (var child in canvaQS.transform.Find ("PanelL").Cast<Transform>().SelectMany(t => t.GetComponents<Transform>()))//.GetComponentsInChildren<Transform>(true))//
            //{//print(child.name);
            //    child.gameObject.SetActive(true);
            //}
            //canvaQS.transform.Find("PanelDropdown").gameObject.SetActive(true);
            //canvaQS.transform.Find("PanelScroller").gameObject.SetActive(true);
            canvaQS.transform.Find("PanelInput").gameObject.SetActive(true);
        }
        
    }

    public override void Answer(List<string> ans){
        stopwatch.Stop();
        print("real answer");

        dynamic expando = new ExpandoObject();
            expando.date=System.DateTime.Now;//.ToString("yyyy/MM/dd HH:mm:ss")
            expando.id=shortuid;//guid.ToString("n");
            expando.height=qs_ll[level][qs].height;//qs/(qs_list.Count/3).ToString()(float)
            expando.reaction_time=stopwatch.ElapsedMilliseconds;//.ToString()
            expando.response_button=ans[0];
            expando.response=ans[1];
           // if(qs_ll[level][qs].question.Contains("Are you")){
            expando.is_correct=ans[1]==qs_ll[level][qs].correct;
            // } else if(qs_ll[level][qs].question.Contains("How high")){
            //     expando.is_correct=float.Parse(ans[1])==Math.Round(qs_ll[level][qs].height,2);
            // }
            expando.correct=qs_ll[level][qs].correct;            
            expando.incorrect=qs_ll[level][qs].incorrect;
            expando.question= qs_ll[level][qs].question;
            expando.anchor_offset= qs_ll[level][qs].anchor_offset;
            expando.anchor_type= qs_ll[level][qs].anchor_type;
            expando.environment_ref= qs_ll[level][qs].environment_ref;
            expando.qs_index= qs_ll[level][qs].index;

        answers_li.Add(expando);

        Debug.Log("level-qs-"+level+"-"+qs+"   expando.response: "+ expando.response);//string.Join(", ", answers_li)

        foreach (var child in canvaQS.GetComponentsInChildren<Button>(true))
        {
            child.gameObject.SetActive(!child.gameObject.CompareTag("responseTag")&&child.gameObject.name!="Num.");
            //print(child.gameObject.name);
        }    
        canvaQS.transform.Find ("Panel/Text m").GetComponentInChildren<TMPro.TextMeshProUGUI>().text="";
        qs+=1;
        qs_Button.SetActive(true);
        qs_Button.transform.localScale = buttonScale;
        // qs_Button.GetComponent<Image>().enabled=true;
        // qs_Button.GetComponent<Button>().interactable = true;
        if( qs>=qs_ll[level].Count) {

            //ele_Button.GetComponent<Renderer>().enabled = true;   
            //print("prepare up");
            //foreach (var child in canvaQS.transform.Find ("PanelL").Cast<Transform>().SelectMany(t => t.GetComponents<Transform>()))
            //{
            //    //print(child.name);
            //    var dd=child.GetComponent< TMPro.TMP_Dropdown>() ;
            //    if (dd!= null)
            //    {
            //        dd.value = 0;
            //    }
            //    child.gameObject.SetActive(false);
            //}

            canvaQS.transform.Find("PanelScroller/PanelR/Template (1)/Viewport").Find("Content").GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(0f, .1f);
            canvaQS.transform.Find("PanelScroller/PanelL/Template (1)/Viewport").Find("Content").GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(0f, .1f);
            qs_Button.transform.position=qs_Button.transform.position-new Vector3(0f,-2f,0f);
            //canvaQS.transform.Find("PanelDropdown").gameObject.SetActive(false);
            canvaQS.transform.Find("PanelScroller").gameObject.SetActive(false);
            canvaQS.transform.Find("PanelInput").gameObject.SetActive(false);
            // foreach (var child in canvaQS.transform.Find ("PanelScroller").Cast<Transform>().SelectMany(t => t.GetComponents<Transform>()))
            // {
            //     child.gameObject.SetActive(false);
            // }

            //voice.Speak(string.Empty, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);

            if(level+1>=qs_ll.Count){//%(qs_list.Count/3)==0qs_list.Count/3check all current level qs
                qs_text.text="This is the end of the experiment.\n Thank you! ";
                qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text="Finish";   
                qs_Button.tag="Finish"; 

                //return;
            } else{
            qs_text.text = "If you are ready, press the button below to advance to next level";
            qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "next level";   
            qs_Button.tag = "ElevatorUp";
            }
         }else if(qs==1) {
            qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text="Submit"; //Start       
            //qs_text.text="If you are ready, click 'Start' button with your laser pointer to reveal the next question.";// and options.
            //qs_Button.tag="QS2";
            print("How high question");
            qs_text.text = qs_ll[level][qs].question;
            qs_Button.SetActive(false);
            qs_Button.transform.localScale = new Vector3(.0000001f, .0000001f, .0000001f);
            qs_Button.transform.position = qs_Button.transform.position - new Vector3(0f, 2f, 0f);
            //qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Submit";
            //foreach (var child in canvaQS.transform.Find ("PanelL").Cast<Transform>().SelectMany(t => t.GetComponents<Transform>()))//.GetComponentsInChildren<Transform>(true))//
            //{//print(child.name);
            //    child.gameObject.SetActive(true);
            //}
            //canvaQS.transform.Find("PanelDropdown").gameObject.SetActive(true);
            //canvaQS.transform.Find("PanelScroller").gameObject.SetActive(true);
            canvaQS.transform.Find("PanelInput").gameObject.SetActive(true);
        }
        voice.Speak("|", SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        //print("voice.Status.InputSentenceLength"+voice.Status.InputSentenceLength);
        stopwatch.Reset();
        stopwatch.Start();
    }

    public void Answer(GameObject gobj){
        stopwatch.Stop();
        print("simple answer");
        dynamic expando = new ExpandoObject();//AnswerDefault();
            expando.date=System.DateTime.Now;//.ToString("yyyy/MM/dd HH:mm:ss")
            expando.id=shortuid;//guid.ToString("n");
            //print(level+"--"+qs);
            expando.height= gobj.tag != "Finish"? qs_ll[level][qs].height:"END";//qs/(qs_list.Count/3).ToString()(float)
            expando.reaction_time=stopwatch.ElapsedMilliseconds;//.ToString()
            expando.response_button=gobj.name;
            expando.response=gobj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text;

        answers_li.Add(expando);

        stopwatch.Reset();
    }



    public override void WriteToFile(){
        if(answers_li!= null){
            //Debug.Log("answers_li: "+string.Join(", \n", answers_li));x.response+"-"+
            //Debug.Log("answers_li: "+string.Join(",/n ", answers_li.Skip(1).Select(x=>x.response).ToList()));

        // dynamic expando=new ExpandoObject();            
        //     expando.date = System.DateTime.Now;
        //     expando.id = shortuid;//guid.ToString("n");
        //     expando.type ="END";

        // answers_li.Add(expando);
        //answers_li.Add(new ExpandoObject());
        
        Directory.CreateDirectory(path + @"\vrdata_anchor\testing_data");
        //check directory valid file number by size
        string[] files=Directory.GetFiles(path + @"\vrdata_anchor\"); 
        int nComplete=0;
        foreach(string f in files){
             if (new FileInfo(f).Length>4000){
                nComplete++; 
             }
        }
        //print(nComplete);
        //Debug.Log("files: "+string.Join(", \n", files));
        string name= path + @"\vrdata_anchor\answers_vr_"+shortuid;

        int recordQ1_count = answers_li.Select(a=>((IDictionary<string,dynamic>)a)).Where(a=>a.Keys.Contains("qs_index")).Where(a=>a["qs_index"]!="25").Count();
        int recordQ2_count = answers_li.Select(a=>((IDictionary<string,dynamic>)a)).Where(a=>a.Keys.Contains("qs_index")).Where(a=>a["qs_index"]=="25").Count();
        int inputQ1_count = qs_ll.SelectMany(a=>a).Select(a=>((IDictionary<string,dynamic>)a)).Where(a=>a["index"]!="25").Count();
        int inputQ2_count = qs_ll.SelectMany(a=>a).Select(a=>((IDictionary<string,dynamic>)a)).Where(a=>a["index"]=="25").Count();
        print(recordQ1_count+"?"+inputQ1_count+"--"+recordQ2_count+"?"+inputQ2_count);        
        if(recordQ1_count>= inputQ1_count && recordQ2_count>= inputQ2_count) { //answers_li.Where(a=>a!=null).Count()-2==qs_ll.Sum(x => x.Count())
            name+=" - P"+(nComplete+1);
            print("valid file: "+name);
        } else {
            name= name.Replace("vrdata_anchor", @"vrdata_anchor\testing_data");
            print("invalid file: "+name);
            }//path + @"\vrdata_anchor\testing data";

        
        using (StreamWriter theWriter = new StreamWriter(name+".csv",true))//DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")
            using (var csvW = new CsvWriter(theWriter,  new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture) { HasHeaderRecord = true }))//
            {
                //csvWri.Configuration.HasHeaderRecord = false;
                csvW.WriteHeader<CsvSheet_anchor>();
                csvW.NextRecord();
                csvW.WriteRecords(answers_li);
            }

        print("data WRITE TO "+ name+".csv");
        }
    }

    public override void OnApplicationQuit() {//cease speech, write to CSV
        voice.Speak(string.Empty, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        print("override OnApplicationQuit");
        WriteToFile();
    }
    
            
    public ExpandoObject AnswerDefault(){
        dynamic expando = new ExpandoObject();
        expando.date = System.DateTime.Now;//.ToString("yyyy/MM/dd HH:mm:ss")
        expando.id = shortuid;//guid.ToString("n");
        expando.height = "";//qs/(qs_list.Count/3).ToString()(float)
        expando.reaction_time = 0;//.ToString()
        expando.response_button = "";
        expando.response = "";
        expando.is_correct = "";
        expando.correct = "";
        expando.incorrect = "";
        expando.question = "";
        expando.anchor_offset = "";
        expando.anchor_type = "";
        expando.environment_ref = "";
        expando.qs_index = "";
        return expando;
    }
    public class CsvSheet_anchor
    {
        [Index(0)]
        public System.DateTime date { get; set; }
        [Index(1)]
        public string id { get; set; }
        [Index(2)]
        public float height { get; set; }
        [Index(3)]
        public long reaction_time { get; set; }
        [Index(4)]
        public string response_button { get; set; }
        [Index(5)]
        public string response { get; set; }
        [Index(6)]
        public bool is_correct { get; set; }
        [Index(7)]
        public string correct { get; set; }
        [Index(8)]
        public string incorrect { get; set; }
        [Index(9)]
        public string question { get; set; }
        [Index(10)]
        public string anchor_offset { get; set; }
        [Index(11)]
        public string anchor_type { get; set; }
        [Index(12)]
        public string environment_ref { get; set; }
        [Index(13)]
        public string qs_index { get; set; }
    }
    
}
