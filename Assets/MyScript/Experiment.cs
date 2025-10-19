using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Text;
using SpeechLib;
using System.Threading;
using UnityEngine.UI;
using CsvHelper;
using CsvHelper.Configuration;
using System.Dynamic;
//using System.Media;
//using Microsoft.CSharp;

public class Experiment : MonoBehaviour
{
    public GameObject ele_Button;
    public GameObject qs_Button;
    public TMPro.TextMeshProUGUI display;
   
    public GameObject canvaQS;
    //ButtonControl button;
    protected string path = System.IO.Directory.GetCurrentDirectory();
    protected System.Diagnostics.Stopwatch stopwatch;
    protected Guid guid;
    protected string shortuid;
    //[HideInInspector]
    public SpVoice voice {get; set;}
    public TMPro.TextMeshProUGUI qs_text{get; set;}

    //System.Timers.Timer timer;    
    //List<string> shuffledCRT;
    private  List<dynamic> qs_list;// {get; set;}//;
    protected List<dynamic> answers_li;
    List<List<dynamic>> qs_ll;
    List<List<string>> answers_li2;
    protected int qs=0;
    protected int level;
    // Start is called before the first frame update
    public virtual void Start()
    {       
        qs_text=canvaQS.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        foreach (var child in canvaQS.GetComponentsInChildren<Button>(true))
        {
            child.gameObject.SetActive(!child.gameObject.CompareTag("responseTag"));
        }
   
        var config = new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };
        using (var csvR = new CsvReader(File.OpenText(path +@"\spreadsheet.csv"), System.Globalization.CultureInfo.CurrentCulture)){
            //read stimuli into list
            qs_list = csvR.GetRecords<dynamic>().ToList();//
        }
        //Debug.Log(new List<dynamic>().Add(qs_list.ElementAt(3)));//reflective

        qs_ll = new List<List<dynamic>>(){
            qs_list.Where(x=>x.type == "CRT"&&x.question.Contains("in total")).ToList(),
            qs_list.Where(x=>x.type == "CRT"&&x.question.StartsWith("If it")).ToList(),
            qs_list.Where(x=>x.type == "CRT"&&x.question.Contains(". If it")).ToList()
        };

        //randomize CRT of the same subtype
        for(int i = 0; i < qs_ll.Count(); i++) {
            qs_ll[i]=qs_ll[i].OrderBy(x => Guid.NewGuid()).ToList();
        }      
        // var aaa = new List<dynamic> {qs_ll[0][0],qs_ll[1][0]};//Enumerable.Concat(qs_ll[0][0],qs_ll[1][0]).Value
        // foreach(var node in aaa){
        //     Debug.Log("aaa:"+node.index+"--"+node.type);
        //     Debug.Log("aaa node split----");}  
        int initcount = qs_ll.Count();
        //select from CRT subtypes to form groups 
        for(int i = qs_ll.Count()-1; i>=0 ; i--){//< initcount
            //qs_ll.Add(qs_ll[0].GetRange(i,1).Concat(qs_ll[1].GetRange(i,1)).Concat(qs_ll[2].GetRange(i,1)).ToList());
            qs_ll.Add(qs_ll.GetRange(0,initcount).Where(subList => subList.Any()).Select(subList => subList[i]).ToList()   );    
        }
        // foreach(var node in qs_ll){
        //     Debug.Log("qs_ll: "+string.Join(", ", node.Select(x=>x.type+"-"+x.index).ToList()));//
        //     Debug.Log("node split----");}  
        qs_ll.RemoveRange(0,qs_ll.Count()/2);

        //subset decoys
        qs_list= qs_list.Where(x=>x.type != "CRT").ToList();
        qs_list= qs_list.OrderBy(x => Guid.NewGuid()).ToList();

        //add decoys on each level  
        for(int i = 0; i < qs_ll.Count(); i++){
            //print(i);
            qs_ll[i].AddRange(qs_list.GetRange(i*qs_list.Count()/qs_ll.Count(),qs_list.Count()/qs_ll.Count()).ToList());
            //qs_list.RemoveRange(0,10);
        }
        //randomize each level
        for(int i = 0; i < qs_ll.Count(); ) {
            qs_ll[i]=qs_ll[i].OrderBy(x => Guid.NewGuid()).ToList();

            //get CRT new index
            List<int> crt_index_l=qs_ll[i].Select((ele, ind) =>  new {Element = ele, Index = ind}).Where(ei => ei.Element.type == "CRT").Select(ei => ei.Index).ToList();
            //Debug.Log("crt_index_l["+i+"]: "+string.Join(", ", crt_index_l));

            //check if CRT items are consecutive (to separate CRT, decoys must be at least 2 for each level)
            if (crt_index_l.Select((ii,j) => ii-j).Distinct().Count()!=crt_index_l.Count()){
                //print("consecutiveness detected, re-randomize");
                continue;
            }else i++;
        }  
        foreach(var node in qs_ll){
            Debug.Log("qs_ll: "+string.Join(", ", node.Select(x=>x.type+"-"+x.index).ToList()));//
            // foreach(var nodee in node){
            // Debug.Log("qs_ll:"+nodee.index+"-"+nodee.type);
            //}
            Debug.Log("node split----");}        
        //print(qs_ll[0][0].question);
        //qs_list=qs_list.GetRange(0, 6);//
        //qs_list=qs_ll.SelectMany(x=>x).Select(ei => ei.index+"-"+ei.type).ToList();
        //Debug.Log("qs_list list: "+string.Join(", \n", qs_list));        
        // foreach (var node in qs_list)
        // {
        //     Console.WriteLine($"record contains: {node.incorrect}");
        //     Debug.Log("qs_list:"+node.index+"--"+node.type);
        // }

        //string[] spreadsheet= File.ReadAllLines( path +@"\spreadsheet.csv");
        // List<string> CRT =File.ReadAllLines( path +@"\CRT.txt").ToList(); //new List<string>{"CRT1","CRT2","CRT3"};//
        // Debug.Log("CRT list: "+string.Join(", \n", CRT));//print(CRT.Count +"-"+ CRT);
        // ////RANDOMIZE
        // shuffledCRT = CRT.OrderBy(x => Guid.NewGuid()).ToList();
        // Debug.Log("shuffledCRT list: "+string.Join(", \n", shuffledCRT));

        voice = new SpVoice();
        answers_li2= new List<List<string>>();
        stopwatch = new System.Diagnostics.Stopwatch();
        //display=GameObject.Find("Input(TMP)").GetComponent<TMPro.TextMeshProUGUI>();
        answers_li= new List<dynamic>();

        dynamic expando = new ExpandoObject();
            expando.date = System.DateTime.Now;//.ToString("yyyy/MM/dd HH:mm:ss")
            guid=Guid.NewGuid();
            shortuid=RandomIdGenerator.GetBase36(10);
            expando.id = shortuid;//guid.ToString("n");
            expando.type ="BEGIN";
            //answers_li.Add( System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            //answers_li.Add(guid.ToString());
            //answers_li.Add("BEGIN\n");
        CsvSheet csvs=new CsvSheet(){
            date = System.DateTime.Now,
            id = shortuid,
            type ="BEGIN"};      
        answers_li.Add(expando);
    }


    public class CsvSheet
    {
        public System.DateTime date { get; set; }
        public string id { get; set; }
        public string height { get; set; }
        public long reaction_time { get; set; }
        public string response_button { get; set; }
        public string response { get; set; }
        public bool is_correct { get; set; }
        public string correct { get; set; }
        public string incorrect { get; set; }
        public string question { get; set; }
        public string type { get; set; }
        public string qs_index { get; set; }
    }

    public virtual void WriteToFile(){
        if(answers_li!= null){
            //Debug.Log("answers_li: "+string.Join(", \n", answers_li));x.response+"-"+
            //Debug.Log("answers_li: "+string.Join(",/n ", answers_li.Skip(1).Select(x=>x.response).ToList()));

        dynamic expando=new ExpandoObject();            
            expando.date = System.DateTime.Now;
            expando.id = shortuid;//guid.ToString("n");
            expando.type ="END";
        CsvSheet csvs=new CsvSheet(){
            date = System.DateTime.Now,
            id = shortuid,
            type ="END"}; 
        answers_li.Add(expando);
        //answers_li.Add(new ExpandoObject());
        using (StreamWriter theWriter = new StreamWriter(path + @"\vrdata\answers_vr0_"+shortuid+".csv",true))//DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")
            using (var csvW = new CsvWriter(theWriter,  new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture) { HasHeaderRecord = true }))//
            {
                //csvWri.Configuration.HasHeaderRecord = false;
                csvW.WriteHeader<CsvSheet>();
                csvW.NextRecord();
                csvW.WriteRecords(answers_li);
            }

        // File.AppendAllText(path+@"\answers.csv", csv.ToString());        
        // {
        //     foreach(var ans in answers_li)
        //     {
        //         if(ans.EndsWith("\n")){
        //             theWriter.Write(ans);
        //         }else 
        //             theWriter.Write(ans + ",");//
        //         //theWriter.Write(ans[0] + ","+ans[1]+",");
        //     }
        //     theWriter.Write("\n");
        // }        

        // var csv = new StringBuilder();
        // foreach (var list in answers_li2)
        // {
        //     String newLine = string.Format("{0},{1}", list[0], list[1]); 
        //     // csv.Append(list[0]+",");
        //     // csv.Append(list[1]+",");
        //     csv.AppendLine(newLine);  
        // } 

        print("data WRITE TO "+ path + @"\vrdata\answers_vr0_"+shortuid+".csv");
        }
    }

    public virtual void Answer(List<string> ans){
        stopwatch.Stop();

        dynamic expando = new ExpandoObject();
            expando.date=System.DateTime.Now;//.ToString("yyyy/MM/dd HH:mm:ss")
            expando.id=shortuid;//guid.ToString("n");
            expando.height=level;//qs/(qs_list.Count/3).ToString()
            expando.reaction_time=stopwatch.ElapsedMilliseconds;//.ToString()
            expando.response_button=ans[0];
            expando.response=ans[1];
            expando.is_correct=ans[1]==qs_ll[level][qs].correct;
            expando.correct=qs_ll[level][qs].correct;            
            expando.incorrect=qs_ll[level][qs].incorrect;
            expando.question=qs_ll[level][qs].question;
            expando.type=qs_ll[level][qs].type;
            expando.qs_index=qs_ll[level][qs].index;

        CsvSheet csvs=new CsvSheet(){
            date=System.DateTime.Now,//.ToString("yyyy/MM/dd HH:mm:ss")
            id=shortuid,//guid.ToString("n"),
            height=level.ToString(),//qs/(qs_list.Count/3).ToString()
            reaction_time=stopwatch.ElapsedMilliseconds,//.ToString()
            response_button=ans[0],
            response=ans[1],
            is_correct=ans[1]==qs_ll[level][qs].correct,
            correct=qs_ll[level][qs].correct,            
            incorrect=qs_ll[level][qs].incorrect,
            question=qs_ll[level][qs].question,
            type=qs_ll[level][qs].type,
            qs_index=qs_ll[level][qs].index,
        };
        answers_li.Add(expando);
        //answers_li.Add(ans);
        // answers_li.Add( System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        // answers_li.Add(guid.ToString());
        // answers_li.Add((qs/2).ToString());
        // answers_li.Add(stopwatch.ElapsedMilliseconds.ToString());//+"ms"
        // answers_li.AddRange(ans);
        // answers_li.Add(qs_list[qs].answer);
        // answers_li.Add(qs_list[qs].incorrect);
        // answers_li.Add(qs_list[qs].correct);
        // answers_li.Add(qs_list[qs].type);
        // answers_li.Add(qs_list[qs].index+"\n");
        Debug.Log("qs--"+qs+"   expando.response: "+ expando.response);//string.Join(", ", answers_li)

        foreach (var child in canvaQS.GetComponentsInChildren<Button>(true))
        {
            child.gameObject.SetActive(!child.gameObject.CompareTag("responseTag"));
            //print(child.gameObject.name);
        }    

        qs+=1;        
        qs_Button.GetComponent<Image>().enabled=true;
        qs_Button.GetComponent<Button>().interactable = true;
        if( qs>=qs_ll[level].Count) {
            if(level+1>=qs_ll.Count()){//%(qs_list.Count/3)==0qs_list.Count/3check all current level qs
                qs_text.text="This is the end of the experiment.\n Thank you! ";
                qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text="Finish";   
                qs_Button.tag="Finish"; 
                return;
            }

            //ele_Button.GetComponent<Renderer>().enabled = true;   
            qs_text.text="If you are ready, press the button below to advance to next level";
            qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text="next level";   
            qs_Button.tag="ElevatorUp";
        }else{
            qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text="Start";        
            qs_text.text="If you are ready, click 'Start' button with your laser pointer to reveal the question and options.";
        }
        stopwatch.Reset();
    }
    public void Answer(){
        stopwatch.Stop();
        List<string> temp = new List<string>
        {
            display.text,
            stopwatch.ElapsedMilliseconds.ToString() + "ms"
        };
        answers_li2.Add(temp);
        //answers_li.Add(display.text);
        //answers_li.Add(stopwatch.ElapsedMilliseconds.ToString()+"ms");
        stopwatch.Reset();
        
    }
    //initiate qs level
    public void PrepareQuestion(int lv, List<float> heights){
        level=lv;
        if(level+1>heights.Count()) {qs_text.text="This is the end of the experiment.\n Thank you! ";return;}
        else qs_text.text="If you are ready, click 'Start' button with your laser pointer to reveal the question and options.";
        qs=0;
        //if((level+1)<=qs_list.Count/2){
            //qs=level*(qs_list.Count/(heights.Count-1));
            Debug.Log("initiate question level:--"+level+"  qs:"+qs);  
        //}  

        qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text="Start";   
        qs_Button.tag="QS";
    }

    public void TTS(string tts){  
        voice.Voice = voice.GetVoices("","").Item(0) ;//string.Empty
        voice.Speak(tts.Split('_')[0], SpeechVoiceSpeakFlags.SVSFlagsAsync);
        //voice.Rate=-3;
    }
    public virtual void OnApplicationQuit() {//cease speech, write to CSV
        //voice.Speak(string.Empty, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        WriteToFile();
    }
    //speech lead question
    public virtual void Question(int level){
        //System.Random rnd = new System.Random();
        //int index = rnd.Next(CRT.Count);
        //qs=level*3;
        //shuffledCRT
        //print((level+1)*3+"qushang:---"+qs_list.Count/3);
        // qs_Button.GetComponent<Image>().enabled = false;
        // qs_Button.GetComponent<Button>().interactable = false;
        // qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text="";
        if (qs_Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text == "Start")
        {
            qs_text.text = "...";
            //if ((level+1)<=qs_list.Count/2){
            //text to speech GUIDANCE
            //TTS("The Question"+"will be presented in three");
            //TTS("two");
            TTS("ready");
            //SystemSounds.Beep.Play();
            //voice.Speak("1",SpeechVoiceSpeakFlags.SVSFDefault);
            //voice.WaitUntilDone (-1);
            //}else return;
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {   
        //print(voice.Status.RunningState);   //print(voice.SpeakCompleteEvent());
        //print(voice.Status.InputSentenceLength+"-inputlength");print(voice.Status.LastStreamNumberQueued+"-lastStreamNumberQueued");print(voice.Status.CurrentStreamNumber+"-currentStreamNumber");    
            //if ( !GameObject.Find("ele_button").GetComponent<Renderer>().enabled  )//!GameObject.Find("Platform").GetComponent<Elevator>().elevatorMoving &
             if (voice.Status.InputSentenceLength!=0 & voice.Status.RunningState == SpeechRunState.SRSEDone &stopwatch.IsRunning==false &!qs_Button.GetComponent<Image>().enabled &qs_Button.tag=="QS") {//&ele_Button.activeSelf &!qs_Button.activeSelf 
                //timer.Start();
                qs_text.text= qs_ll[level][qs].question;//shuffledCRT
                stopwatch.Start();
                foreach (var child in canvaQS.GetComponentsInChildren<Button>(true))
                {
                    child.gameObject.SetActive(child.gameObject.CompareTag("responseTag"));
                }

                var response = canvaQS.GetComponentsInChildren<Button>();
                response=response.OrderBy(x => Guid.NewGuid()).ToArray();
                response[0].GetComponentInChildren<TMPro.TextMeshProUGUI>().text=qs_ll[level][qs].incorrect;
                response[1].GetComponentInChildren<TMPro.TextMeshProUGUI>().text=qs_ll[level][qs].correct;
                //print(stopwatch.ElapsedMilliseconds);
                //keyboard input monitor
                // if(Input.inputString!=null){
                //     StartCoroutine(ReceiveInput());
                // }
            }
            // if(Input.GetKeyDown(KeyCode.RightBracket)){
            //     qs+=1;
            //     Debug.Log("qs--"+qs);
            // }else if(Input.GetKeyDown(KeyCode.LeftBracket)){
            //     qs-=1;
            //     Debug.Log("qs--"+qs);
            // }
    }
    //keyboard input
    IEnumerator ReceiveInput(){
        string inputs=Input.inputString;
        foreach (char c in Input.inputString){
            if (c == '\b'){ // has backspace/delete been pressed?
                display.text = display.text.Length>1?display.text.Substring(0, display.text.Length - 1):"";
                // if (display.text.Length != 0){
                //     display.text = display.text.Substring(0, display.text.Length - 1);
                // }
                }
            else if ((c == '\n') || (c == '\r') &  display.text !="0") { // enter/return
                //timer.Stop();

                Answer();
                qs_text.text="Qs";
                display.text = "";
                voice.Speak(string.Empty, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
                ele_Button.GetComponent<Renderer>().enabled = true;
                //qs_Button.GetComponent<Renderer>().enabled = false;
                //qs_Button.SetActive(false);
            }
            else if (Char.IsDigit(c) || c=='.'){//write only number and dot
                if (c=='.' & display.text.Contains("."))
                {
                    ;//no more than one .
                }else{
                    // if(display.text[0]=='0'){
                    //     display.text=display.text.Substring(1, display.text.Length-1);
                    // }
                    display.text += c;
                    }
                //READ TIME
                // stopwatch.Stop();
                // print(stopwatch.ElapsedMilliseconds);
                // stopwatch.Start();
                
            }yield return null;
            
        }
        
    }

}

public static class RandomIdGenerator 
{
    private static char[] _base62chars = 
        "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
        .ToCharArray();

    private static System.Random _random = new System.Random();

    public static string GetBase62(int length) 
    {
        var sb = new StringBuilder(length);

        for (int i=0; i<length; i++) 
            sb.Append(_base62chars[_random.Next(62)]);

        return sb.ToString();
    }       

    public static string GetBase36(int length) 
    {
        var sb = new StringBuilder(length);

        for (int i=0; i<length; i++) 
            sb.Append(_base62chars[_random.Next(36)]);

        return sb.ToString();
    }
}

static class LinqExtensions
{
    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
    {
            return list.Select((item, index) => new {index, item})
                       .GroupBy(x => x.index % parts)
                       .Select(x => x.Select(y => y.item));
    }
}