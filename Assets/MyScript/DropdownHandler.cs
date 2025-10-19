using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.UI.Extensions.Examples;

public class DropdownHandler : ScrollingNum
{
    // Start is called before the first frame update
    TMPro.TMP_Dropdown dd;
    ScrollRect sr;
    public int[] range;
    void Start()
    {
        dd= this.GetComponent<TMPro.TMP_Dropdown>();
        sr= this.GetComponentInChildren<ScrollRect>();
        //print(dd);
        dd.options.Clear();

        List<string> integer = Enumerable.Range(range[0], range[1]).ToList().ConvertAll<string>(x => x.ToString());;//, fraction= Enumerable.Range(0, 99).ToList();
        if (dd.name=="Dropdown (1)")
        {
            //integer=integer.FindAll(a=> int.Parse(a)<1||int.Parse(a)>9);
            integer[integer.FindIndex(x=>x=="0")]="00";
            integer.Where(a=>float.Parse(a)/10<1).ToList().ForEach(b=>b="0"+b);
        }
        foreach(string n in integer){
            dd.options.Add(new TMPro.TMP_Dropdown.OptionData{text = n});//.ToString()
        }
        //dd.transform.Find("Template").gameObject.SetActive(true);
        dd.interactable=false;
        //dd.transform.Find("Dropdown List").gameObject.SetActive(false);
        //dd.value=3;
        // dd.RefreshShownValue();
        //gameObject.SetActive(false);
       // sr.
        //dd.Hide();
        
    }

    // Update is called once per frame
    void Update()
    {
        //base.Update();
       
    }
}
