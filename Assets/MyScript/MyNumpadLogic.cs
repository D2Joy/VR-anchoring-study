using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Dynamic;


public class MyNumpadLogic : MonoBehaviour
     , IPointerClickHandler // 2
     , IPointerEnterHandler
     , IPointerExitHandler
    //  , IPointerDownHandler
    //  , IPointerUpHandler
{
    //public string winCode = "4236";
    public string input = "";
    //public UnityEvent onWin;
    public TMPro.TextMeshProUGUI display;
    //public TMPro.TextMeshProUGUI display2;
    public string num;
    dynamic exp;

    void Start() {
        //display=GameObject.Find("Input(TMP)").GetComponent<TMPro.TextMeshProUGUI>();
        display.text=input;
        //display2.text=input;
        //input = display.text;
        //exp= GameObject.Find("Experiment").GetComponent<Experiment>();
        if(GameObject.Find("Platform").GetComponent<Experiment>().isActiveAndEnabled){//FindObjectOfType<Experiment>().isActiveAndEnabled
            //print("ExperimentExperimentExperiment");
            exp=FindObjectOfType<Experiment>();
        }else if (FindObjectOfType<ExpAnchor>().isActiveAndEnabled)
        {
            exp=FindObjectOfType<ExpAnchor>();
        }
        //exp=FindObjectOfType<Experiment>()??FindObjectOfType<ExpAnchor>();
    }

    public void ButtonPressed(string key) {
        input = display.text;

        // if (this.GetComponentInChildren<Text>())
        // {
        //     key=this.GetComponentInChildren<Text>().text;
        // } else{
        //     key=this.GetComponentInChildren<TMPro.TextMeshProUGUI>().text;
        // }
        if(this.CompareTag("responseTag")){
            exp.Answer(new List<string>{this.name,key});//;new List<dynamic>{new ExpandoObject().Init(new{response=this.name,response_button=key})}
            input = "";
        } else if(new List<string>{"QS","Finish"}.Contains(this.tag)) exp.Answer(this.gameObject);

        if(key == "Submit"){
            // var dd=this.transform.root.Find ("PanelDropdown").GetComponentInChildren<TMPro.TMP_Dropdown>(true);
            // string ddint=dd.GetComponentInChildren<TMPro.TextMeshProUGUI>(true).text;//dd.options[dd.value].text
            // var dd1=this.transform.root.Find ("PanelDropdown").Find("Dropdown (1)").GetComponentInChildren<TMPro.TMP_Dropdown>(true);//
            // string ddfrac=dd1.GetComponentInChildren<TMPro.TextMeshProUGUI>(true).text;//dd1.options[dd1.value].tex
            // //print(dd+"."+dd1+": "+ddint+"."+ddfrac);
            
            // if (ddint!="0" |ddfrac!="0")
            // {
            //     exp.Answer(new List<string>{this.name,ddint+"."+ddfrac});//!=""?ddfrac:"0"
            // } else 
            if(display.text.Length!=0){
                exp.Answer(new List<string>{this.name,display.text});
            }
            input = "";
        }else if(key == "back"){
            input = input.Length>1?input.Substring(0, input.Length - 1):"";
        }else{
            // if (input.Length>=2)
            // {
            //     var display2= display.transform.parent.parent.Find("Dropdown (1)/Label").GetComponent<TMPro.TextMeshProUGUI>();
            //     if (display2.text.Length==1)
            //     {
            //         display2.text=key;
            //     }
                
            // } else 
            if ((input.Contains(".")&&(input.Split('.')[1].Length==1|key=="." ))|(key=="."&&input.Length==0)|(input.StartsWith("0")&& key=="0" &&input.Split('0')[1].Length==0 ))
            {
                return;
            }
            
            input +=  key;//.Substring(1)
        }

        //print(input);
        //print(display);
        display.text = input;
    }
    
    public void OnPointerClick(PointerEventData eventData) // 3
    {
        //input = display.text;
        //display.text=input+"clicked";
        print("clicked--"+gameObject+"--"+gameObject.tag);	
        if (this.GetComponentInChildren<Text>())
        {
            ButtonPressed(this.GetComponentInChildren<Text>().text);
        } else{
            ButtonPressed(this.GetComponentInChildren<TMPro.TextMeshProUGUI>().text);
        }
    }
 
 
     public void OnPointerEnter(PointerEventData eventData)
     {
         //input = display.text;
         //isplay.text=input+"ENTERING";
         //print(this.name);
     }
 
     public void OnPointerExit(PointerEventData eventData)
     {
     }
}

public static class ExtensionMethods
{
    public static ExpandoObject Init(this ExpandoObject expando, dynamic obj)
    {
        var expandoDic = (IDictionary<string, object>)expando;
        foreach (System.Reflection.PropertyInfo fi in obj.GetType().GetProperties())
        {
           expandoDic[fi.Name] = fi.GetValue(obj, null);
        }
        return expando;
    }
}
