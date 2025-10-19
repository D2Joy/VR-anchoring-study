/// <summary>
/// Brought you by Mrs. YakaYocha
/// https://www.youtube.com/channel/UCHp8LZ_0-iCvl-5pjHATsgw
/// 
/// Please donate: https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=RJ8D9FRFQF9VS
/// </summary>
using UnityEngine.Events;
using UnityEngine.XR;
using Valve.VR;
using HTC.UnityPlugin.Vive;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;

namespace UnityEngine.UI.Extensions.Examples
{
    public class ScrollingNum : MonoBehaviour, IPointerEnterHandler
    {

        public RectTransform daysScrollingPanel;


        public ScrollRect daysScrollRect;


        public GameObject daysButtonPrefab;


        private GameObject[] daysButtons;


        public RectTransform daysCenter;


        UIVerticalScroller2 daysVerticalScroller;


        public TMPro.TextMeshProUGUI dateText;

        private int daysSet;

        public int ndays;
        public float itemScale;

        //SteamVR_TrackedObject trackedObj;
        //SteamVR_Controller.Device device;

        public void OnPointerEnter(PointerEventData eventData)
        {
           print(eventData.pointerEnter);
            //print(EventSystem.current.currentSelectedGameObject);
            //print(eventData.selectedObject==EventSystem.current.currentSelectedGameObject);
            //EventSystem.current.SetSelectedGameObject(daysScrollRect.gameObject);
            print(EventSystem.current.currentSelectedGameObject);

        }
        // public void OnPointerClick(PointerEventData eventData)
        // {
        //     print(eventData.selectedObject);
        //     EventSystem.current.SetSelectedGameObject(gameObject);
        // }
        public void Select(){
            print(EventSystem.current.currentSelectedGameObject);
            EventSystem.current.SetSelectedGameObject(gameObject);
            print(EventSystem.current.currentSelectedGameObject);
        }
        
        private void InitializeDays()
        {
            // Debug.Log(transform.parent.gameObject.name);
            // if(transform.parent.gameObject.name=="PanelR"){
            //     ndays=10;
            //     //days=new List<int>(days).FindAll(x=>x<10).ToArray();
            //     //Debug.Log("days: "+string.Join(", \n", days));
            // }            
            int[] days = new int[ndays];
            daysButtons = new GameObject[days.Length];

            for (var i = 0; i < days.Length; i++)
            {
                days[i] = i ;//+ 1
                string tempday = days[i].ToString();
                // if (tempday.Length==1)
                // {
                //     tempday="0"+tempday;
                // }
                GameObject clone = Instantiate(daysButtonPrefab, daysScrollingPanel);
                if(clone.GetComponentInChildren<TMPro.TextMeshProUGUI>()!=null)
                    clone.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "" + tempday;
                else clone.GetComponentInChildren<Text>().text = "" + tempday;

                clone.name = "Day_" + tempday;
                clone.AddComponent<CanvasGroup>();
                daysButtons[i] = clone;
            }
        }

        // Use this for initialization
        public void Awake()
        {
            //daysScrollRect.enabled=true;
            //daysScrollingPanel=true;
            //daysCenter.
            InitializeDays();

            //Yes Unity complains about this but it doesn't matter in this case.

            daysVerticalScroller = new UIVerticalScroller2(daysCenter, daysCenter, daysScrollRect, daysButtons, itemScale);


            daysVerticalScroller.Start();

            Debug.Log("XR Device Present: " + XRDevice.isPresent);
            // Debug.Log("XR User Presence: " + XRDevice.userPresence);
            // Debug.Log("XR Model: " + XRDevice.model);
            Debug.Log("XR Device Active: " + XRSettings.isDeviceActive);
            Debug.Log("XR Enabled: " + XRSettings.enabled);
            print(SteamVR.instance);
            print(SteamVR.initializedState);
            if (XRDevice.isPresent||XRSettings.isDeviceActive||XRSettings.enabled||SteamVR.instance!=null||SteamVR.initializedState == SteamVR.InitializedStates.InitializeSuccess)
            {
                daysScrollRect.scrollSensitivity=0.5f;
            }

            //daysScrollRect.gameObject.OnPointerEnter.AddListener(InitializeDays);
        }

 
        // public void SetDate()
        // {
        //     daysSet = int.Parse(inputFieldDays.text) - 1;


        //     daysVerticalScroller.SnapToElement(daysSet);

        // }

        public void Update()
        {

            daysVerticalScroller.Update();

            string dayString = daysVerticalScroller.result;

            //var device = SteamVR_Controller.Input((int)trackedObj.index);
            //print( EventSystem.current.currentSelectedGameObject);
            if (ViveInput.GetPressEx(HandRole.RightHand, ControllerButton.PadTouch) && EventSystem.current.currentSelectedGameObject==this.gameObject)
            {
                print("Pressing Touchpad");
                Vector2 touch = ViveInput.GetPadPressAxisEx(HandRole.RightHand);
                StartCoroutine(Padinput(touch));
            }
            // if (dayString.EndsWith("1") && dayString != "11")
            //     dayString = dayString + "st";
            // else if (dayString.EndsWith("2") && dayString != "12")
            //     dayString = dayString + "nd";
            // else if (dayString.EndsWith("3") && dayString != "13")
            //     dayString = dayString + "rd";
            // else
            //     dayString = dayString + "th";

            dateText.text = dayString ;//+" m"monthString + " " ++ " " + yearsString
        }

        public void DaysScrollUp()
        {
            daysVerticalScroller.ScrollUp();
        }

        public void DaysScrollDown()
        {
            daysVerticalScroller.ScrollDown();
        }

        IEnumerator Padinput(Vector2 touch){
                //Vector2 touchpad = (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));

                if (touch.y > 0.5f)
                {
                    print("Moving Up");
                    daysVerticalScroller.ScrollUp();
                } else if (touch.y < -0.5f)
                {
                    print("Moving Down");
                    daysVerticalScroller.ScrollDown();
                }
                yield return new WaitForSeconds(1f);
        }
    }


    public class UIVerticalScroller2 : MonoBehaviour
    {
        [Tooltip("desired ScrollRect")]
        public ScrollRect scrollRect;
        [Tooltip("Center display area (position of zoomed content)")]
        public RectTransform center;
        [Tooltip("Size / spacing of elements")]
        public RectTransform elementSize;
        [Tooltip("Scale = 1/ (1+distance from center * shrinkage)")]
        public Vector2 elementShrinkage = new Vector2(1f / 200, 1f / 200);
        [Tooltip("Minimum element scale (furthest from center)")]
        public Vector2 minScale = new Vector2(0.7f, 0.7f);
        [Tooltip("Select the item to be in center on start.")]
        public int startingIndex = -1;
        [Tooltip("Stop scrolling past last element from inertia.")]
        public bool stopMomentumOnEnd = true;
        [Tooltip("Set Items out of center to not interactible.")]
        public bool disableUnfocused = true;
        [Tooltip("Button to go to the next page. (optional)")]
        public GameObject scrollUpButton;
        [Tooltip("Button to go to the previous page. (optional)")]
        public GameObject scrollDownButton;
        [Tooltip("Event fired when a specific item is clicked, exposes index number of item. (optional)")]
        public IntEvent OnButtonClicked;
        [Tooltip("Event fired when the focused item is Changed. (optional)")]
        public IntEvent OnFocusChanged;
        [HideInInspector]
        public GameObject[] _arrayOfElements;

        public int focusedElementIndex { get; private set; }

        public string result { get; private set; }
        public float itemScale { get; set; }

        private float[] distReposition;
        private float[] distance;
        //private int elementsDistance;


        //Scrollable area (content of desired ScrollRect)
        [HideInInspector]
        public RectTransform scrollingPanel{ get { return scrollRect.content; } }


        /// <summary>
        /// Constructor when not used as component but called from other script, don't forget to set the non-optional properties.
        /// </summary>
        public UIVerticalScroller2()
        {
        }

        /// <summary>
        /// Constructor when not used as component but called from other script
        /// </summary>
        public UIVerticalScroller2(RectTransform center, RectTransform elementSize, ScrollRect scrollRect, GameObject[] arrayOfElements, float itemScale)
        {
            this.center = center;
            this.elementSize = elementSize;
            this.scrollRect = scrollRect;
            _arrayOfElements = arrayOfElements;
            this.itemScale=itemScale;
        }

        /// <summary>
        /// Awake this instance.
        /// </summary>
        public void Awake()
        {
            if (!scrollRect)
            {
                scrollRect = GetComponent<ScrollRect>();
            }
            if (!center)
            {
                Debug.LogError("Please define the RectTransform for the Center viewport of the scrollable area");
            }
            if (!elementSize)
            {
                elementSize = center;
            }
            if (_arrayOfElements == null || _arrayOfElements.Length == 0)
            {
                _arrayOfElements = new GameObject[scrollingPanel.childCount];
                for (int i = 0; i < scrollingPanel.childCount; i++)
                {
                    _arrayOfElements[i] = scrollingPanel.GetChild(i).gameObject;
                }     
            }
        }

        /// <summary>
        /// Recognises and resizes the children.
        /// </summary>
        /// <param name="startingIndex">Starting index.</param>
        /// <param name="arrayOfElements">Array of elements.</param>
        public void updateChildren(int startingIndex = -1, GameObject[] arrayOfElements = null)
        {
            // Set _arrayOfElements to arrayOfElements if given, otherwise to child objects of the scrolling panel.
            if (arrayOfElements != null)
            {
                _arrayOfElements = arrayOfElements;
            }
            else
            {
                _arrayOfElements = new GameObject[scrollingPanel.childCount];
                for (int i = 0; i < scrollingPanel.childCount; i++)
                {
                    _arrayOfElements[i] = scrollingPanel.GetChild(i).gameObject;
                }
            }

            // resize the elements to match elementSize rect
            for (var i = 0; i < _arrayOfElements.Length; i++)
            {
                int j = i;
                if (_arrayOfElements[i].GetComponent<Button>()!=null)
                {
                    _arrayOfElements[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    if (OnButtonClicked != null)
                    {
                        _arrayOfElements[i].GetComponent<Button>().onClick.AddListener(() => OnButtonClicked.Invoke(j));
                    }
                }

                RectTransform r = _arrayOfElements[i].GetComponent<RectTransform>();
                r.anchorMax = r.anchorMin = r.pivot = new Vector2(0.5f, 0.5f);
                r.localPosition = new Vector2(0, i * elementSize.rect.size.y);
                r.sizeDelta = elementSize.rect.size*itemScale;//
            }

            // prepare for scrolling
            distance = new float[_arrayOfElements.Length];
            distReposition = new float[_arrayOfElements.Length];
            focusedElementIndex = -1;

            //scrollRect.scrollSensitivity = elementSize.rect.height / 5;

            // if starting index is given, snap to respective element
            if (startingIndex > -1)
            {
                startingIndex = startingIndex > _arrayOfElements.Length ? _arrayOfElements.Length - 1 : startingIndex;
                SnapToElement(startingIndex);
            }
        }

        public void Start()
        {

            if (scrollUpButton)
                scrollUpButton.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ScrollUp();
                    });

            if (scrollDownButton)
                scrollDownButton.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ScrollDown();
                    });
            updateChildren(startingIndex, _arrayOfElements);
        }


        public void Update()
        {
            if (_arrayOfElements.Length < 1)
            {
                return;
            }

            for (var i = 0; i < _arrayOfElements.Length; i++)
            {
                distReposition[i] = center.GetComponent<RectTransform>().position.y - _arrayOfElements[i].GetComponent<RectTransform>().position.y;
                distance[i] = Mathf.Abs(distReposition[i]);
                //print(center.GetComponent<RectTransform>().position.y);
                //print(distance[i]);

                //Magnifying effect
                Vector2 scale = Vector2.Max(minScale, new Vector2(1 / (1 + distance[i] * elementShrinkage.x), (1 / (1 + distance[i] * elementShrinkage.y))))*1/itemScale;
                _arrayOfElements[i].GetComponent<RectTransform>().transform.localScale = new Vector3(scale.x, scale.y, 1f);
            }

            // detect focused element
            float minDistance = Mathf.Min(distance);
            //print(minDistance);
            int oldFocusedElement = focusedElementIndex;
            for (var i = 0; i < _arrayOfElements.Length; i++)
            {
                _arrayOfElements[i].GetComponent<CanvasGroup>().interactable = !disableUnfocused || minDistance == distance[i];
                if (minDistance == distance[i])
                {
                    focusedElementIndex = i;
                    if (_arrayOfElements[i].transform.Find("Item Checkmark"))
                    {
                        _arrayOfElements[i].transform.Find("Item Checkmark").gameObject.SetActive(true);
                    }
                    
                    if(_arrayOfElements[i].GetComponentInChildren<Text>()!=null)
                        result = _arrayOfElements[i].GetComponentInChildren<Text>().text;
                    else result = _arrayOfElements[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text;
                }   else{
                    if (_arrayOfElements[i].transform.Find("Item Checkmark"))
                    {
                        _arrayOfElements[i].transform.Find("Item Checkmark").gameObject.SetActive(false);
                    }
                }
            }
            if (focusedElementIndex != oldFocusedElement && OnFocusChanged != null)
            {
                OnFocusChanged.Invoke(focusedElementIndex);
            }


            if (!Input.GetMouseButton(0))//UIExtensionsInputManager
            {
                // scroll slowly to nearest element when not dragged
                ScrollingElements();
            }


            // stop scrolling past last element from inertia
            if (stopMomentumOnEnd
                && (_arrayOfElements[0].GetComponent<RectTransform>().position.y > center.position.y
                || _arrayOfElements[_arrayOfElements.Length - 1].GetComponent<RectTransform>().position.y < center.position.y))
            {
                scrollRect.velocity = Vector2.zero;
            }
        }

        private void ScrollingElements()
        {
            float newY = Mathf.Lerp(scrollingPanel.anchoredPosition.y, scrollingPanel.anchoredPosition.y + distReposition[focusedElementIndex],  2f);//Time.deltaTime *
            Vector2 newPosition = new Vector2(scrollingPanel.anchoredPosition.x, newY);
            scrollingPanel.anchoredPosition = newPosition;
        }

        public void SnapToElement(int element)
        {
            float deltaElementPositionY = elementSize.rect.height * element;
            Vector2 newPosition = new Vector2(scrollingPanel.anchoredPosition.x, -deltaElementPositionY);
            scrollingPanel.anchoredPosition = newPosition;

        }

        public void ScrollUp()
        {
            float deltaUp = elementSize.rect.height / 1.2f;
            Vector2 newPositionUp = new Vector2(scrollingPanel.anchoredPosition.x, scrollingPanel.anchoredPosition.y - deltaUp);
            scrollingPanel.anchoredPosition = Vector2.Lerp(scrollingPanel.anchoredPosition, newPositionUp, 1);
        }

        public void ScrollDown()
        {
            float deltaDown = elementSize.rect.height / 1.2f;
            Vector2 newPositionDown = new Vector2(scrollingPanel.anchoredPosition.x, scrollingPanel.anchoredPosition.y + deltaDown);
            scrollingPanel.anchoredPosition = newPositionDown;
        }

        [System.Serializable]
        public class IntEvent:UnityEvent<int>
        {

        }
    }
}