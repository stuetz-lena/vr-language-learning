//========= Copyright 2016-2022, HTC Corporation. All rights reserved. ===========

#pragma warning disable 0649
using HTC.UnityPlugin.LiteCoroutineSystem;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using GrabberPool = HTC.UnityPlugin.Utility.ObjectPool<BlubleDraggable.Grabber>;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.XR;

// demonstrate of dragging things useing built in EventSystem handlers
public class BlubleDraggable : GrabbableBase<PointerEventData, BlubleDraggable.Grabber> 
    , IInitializePotentialDragHandler
    , IBeginDragHandler
    , IDragHandler
    , IEndDragHandler
{
    //---- added Code ----
    [Tooltip("Time factor for bluble constant movement forwards")][SerializeField]
    float timeFactor = 1;
    [Tooltip("Time factor for floating")][SerializeField]
    float fSpeed = 0.25f;

    [Tooltip("Audio to be played in case of bluble emerging")][SerializeField]
    AudioSource emerging;
    [Tooltip("Audio to be played in case of wrong sorting")][SerializeField]
    AudioSource fail; 
    [Tooltip("Audio to be played in case of correct sorting")][SerializeField]
    AudioSource success;
    [Tooltip("Audio to be played in case of bluble disappearing")][SerializeField]
    AudioSource pop; 

    [Tooltip("Material for bluble in case of correct sorting (within results)")][SerializeField]
    Material green;
    [Tooltip("Material for bluble in case of wrong sorting (within results)")][SerializeField]
    Material red;
    [Tooltip("Selection material for player 1")][SerializeField]
    Material player1Material;
    [Tooltip("Selection material for player 2")][SerializeField]
    Material player2Material;
    [Tooltip("Selection material for player 3")][SerializeField]
    Material player3Material;
    [Tooltip("Selection material for player 4")][SerializeField]
    Material player4Material;
    [Tooltip("Start color for particle system effect in case of correct sorting")][SerializeField]
    Color correctColorStart = new Color(0.0f, 1.0f, 0.0f, 0.8f); 
    [Tooltip("End color for particle system effect in case of correct sorting")][SerializeField]
    Color correctColorEnd = new Color(0.43f, 0.01f, 0.47f, 0.5f);
    [Tooltip("Start color for particle system effect in case of wrong sorting")][SerializeField]
    Color wrongColorStart = new Color(1.0f, 0.0f, 0.0f, 0.8f);
    [Tooltip("End color for particle system effect in case of wrong sorting")][SerializeField]
    Color wrongColorEnd = new Color(0.43f, 0.01f, 0.47f, 0.5f);

    [Tooltip("XR Origin node for button control")][SerializeField]
    XRNode node; //for UI
    
    float deviationX = 0; //set by initalisation to stay constant
    float initialY = 0; //set by initalisation to stay constant
    AudioSource wordSource = null; //audio for the vocabulary, set by initialisation
    Material orgMaterial;
    String orgText = "";

    float timeCounter = 0; //for constant movement
    bool isHit = false; //if the bubble was already sorted but still exists for playing sounds and effects
    bool standStill = false; //if the bubble was placed somewhere or handed over
    bool hintUsed = false; //if the hint was already used

    // Start is called before the first frame update
    void Start(){
        if(emerging)
            emerging.Play();
        orgMaterial = this.GetComponent<Renderer>().material;
        orgText = this.GetComponentInChildren<TextMeshPro>().text;
        this.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update(){
        if(!isHit && !isDragged && !standStill && !NetworkManager.Instance.GetIsPaused()){ 
            //bubble should not forcefully move if it is already sorted, currently dragged, handed over or the game is paused
            timeCounter += 0.02f; //to enable clients to take over the master role without a drastic position change, counter needs to be increased on all
            if(PhotonNetwork.IsMasterClient){ //only the master should actually move the bubbles
                float y = Mathf.Sin(timeCounter)*fSpeed + initialY; //move up and down depending on time
                transform.position = new Vector3(Camera.main.transform.position.x + deviationX, y, transform.position.z - timeFactor*Time.deltaTime); //move towards the camera
            }
            //Rotate text to camera
            this.GetComponentInChildren<TextMeshPro>().transform.rotation = Camera.main.transform.rotation;
        }
        UnityEngine.XR.Interaction.Toolkit.InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(node), UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.SecondaryButton, out bool secondaryButton, 0.5f);
        if (secondaryButton && isGrabbed && !hintUsed)
        {
            Debug.Log("secondary with bubble");
            ShowHint();
        }
    }

    public void SetDeviationX(float deviation){
        deviationX = deviation;
    }

    public void SetInitialY(float initial){
        initialY = initial;
    }

    public void SetWordSource(AudioSource audio){
        wordSource = audio;
    }

    public Material GetGreen(){
        return green;
    }

    public Material GetRed(){
        return red;
    }

    public void Stopper(){ 
        //triggered via drop event, stop handed over bubble from constant movement
        this.GetComponent<PhotonView>().RPC("TriggerStandStill", RpcTarget.All, this.GetComponent<PhotonView>().ViewID, true);
    }

    [PunRPC]
    void TriggerStandStill(int viewID, bool value){ 
        PhotonView.Find(viewID).gameObject.GetComponent<BlubleDraggable>().standStill = value;
    }

    public void ShowHint(){ //triggered via button click
        this.GetComponent<PhotonView>().RPC("Hinter", RpcTarget.All, this.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void Hinter(int viewID){
        BlubleDraggable currentBlubble = PhotonView.Find(viewID).gameObject.GetComponent<BlubleDraggable>();
        currentBlubble.hintUsed = true; //save usage for score calculation
        if(currentBlubble.GetComponentInChildren<Button>())
            currentBlubble.GetComponentInChildren<Button>().interactable = false; //deactivate the button
        currentBlubble.GetComponentInChildren<TextMeshPro>().text = GameController.Instance.GetTranslation(currentBlubble.GetComponentInChildren<TextMeshPro>().text);
        StartCoroutine(HideHint(2, viewID));
    }

    IEnumerator HideHint(float time, int viewID){
        yield return new WaitForSeconds(time); 
        BlubleDraggable currentBlubble = PhotonView.Find(viewID).gameObject.GetComponent<BlubleDraggable>();
        currentBlubble.GetComponentInChildren<TextMeshPro>().text = orgText; //reset text
        if(currentBlubble.GetComponentInChildren<Canvas>()) //hide button canvas to restrict hint usage to once
            currentBlubble.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
    }

    public void ChangeMaterialOnSelection(){ 
        //triggered on after grabbed and drop to visalize selection by player
        this.GetComponent<PhotonView>().RPC("ChangeMaterial", RpcTarget.All, this.GetComponent<PhotonView>().ViewID, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    void ChangeMaterial(int viewID, int actorNr){
        Renderer myRenderer = PhotonView.Find(viewID).gameObject.GetComponent<Renderer>();
        if(myRenderer.material != orgMaterial){
            myRenderer.material = orgMaterial; //reset material
        } else {
            switch(actorNr){ //change material depending on playerNr
                case 1: myRenderer.material = player1Material;
                    break;
                case 2: myRenderer.material = player2Material;
                    break;
                case 3: myRenderer.material = player3Material;
                    break;
                case 4: myRenderer.material = player4Material;
                    break;
                default: myRenderer.material = player1Material;
                    break;
            }
        }
    }

    public void PlayAudioText(){ 
        //triggered via after grabbed event to play vocabulary audio
        if(wordSource != null){
            wordSource.Play();
        } else {
            //searching for the child audio component containing the vocabulary on the bubble
            Component[] audios = this.GetComponents(typeof(AudioSource)); 
            for(int i = 0; i < audios.Length; i++) {
                AudioSource wordAudio = audios[i] as AudioSource;
                if(wordAudio.clip.ToString().Contains(this.GetComponentInChildren<TextMeshPro>().text) || wordAudio.clip.ToString().Contains(orgText)){
                    wordSource = wordAudio;
                    PlayAudioText();
                }
            }
        }
    }

    void OnCollisionEnter(Collision other){
        if(!isHit && orgText != "Blase" && orgText != ""){ //if the bubble was already handled, do not act again; if the bubble was not set up correctly do not act to avoid errors
            if(orgText != "")
                GetComponentInChildren<TextMeshPro>().text = orgText; //reset text in case resetter was not yet called before sorting
            Collider myCollider = gameObject.GetComponent<Collider>();
            PhotonView photonView = this.GetComponent<PhotonView>();

            if((other.collider.CompareTag("Bucket_der") && myCollider.CompareTag("der")) || (other.collider.CompareTag("Bucket_die") && myCollider.CompareTag("die")) || (other.collider.CompareTag("Bucket_das") && myCollider.CompareTag("das"))){ //correct sorting
                /*if(!PhotonNetwork.IsMasterClient){
                    photonView.RPC("SortingOrExit", RpcTarget.All, 1, this.GetComponent<PhotonView>().ViewID, this.GetComponentInChildren<TextMeshPro>().text, other.collider.tag);
                } else {*/
                    SortingOrExit(1, this.GetComponent<PhotonView>().ViewID, this.GetComponentInChildren<TextMeshPro>().text);
                //} 
            } else if (other.collider.CompareTag("Bucket_der") || other.collider.CompareTag("Bucket_die")  || other.collider.CompareTag("Bucket_das")){ //if we hit any other bucket
                /*if(!PhotonNetwork.IsMasterClient){
                    photonView.RPC("SortingOrExit", RpcTarget.All, 2, this.GetComponent<PhotonView>().ViewID, this.GetComponentInChildren<TextMeshPro>().text, other.collider.tag);
                } else {*/
                    SortingOrExit(2, this.GetComponent<PhotonView>().ViewID, this.GetComponentInChildren<TextMeshPro>().text, other.collider.tag);
                //} 
            }
        
            if(other.collider.tag == "Floor_end") { //destroy bubbles if they hit the walls
                /*if(!PhotonNetwork.IsMasterClient){
                    photonView.RPC("SortingOrExit", RpcTarget.All, 0, this.GetComponent<PhotonView>().ViewID, this.GetComponentInChildren<TextMeshPro>().text, other.collider.tag);
                } else {*/
                    SortingOrExit(0, this.GetComponent<PhotonView>().ViewID, this.GetComponentInChildren<TextMeshPro>().text);
                //}  
            }

            if(other.collider.tag == "Player") { //stop bubble if it hits a player or is pushed on them
                /*if(!PhotonNetwork.IsMasterClient){
                    photonView.RPC("TriggerStandStill", RpcTarget.All, this.GetComponent<PhotonView>().ViewID, true);
                } else {*/
                    TriggerStandStill(this.GetComponent<PhotonView>().ViewID, true);
                //}  
            }
        }
    }

    [PunRPC]
    void SortingOrExit(int what, int viewID, string word, string bucket = ""){
        BlubleDraggable currentBlubble = PhotonView.Find(viewID).gameObject.GetComponent<BlubleDraggable>();
        currentBlubble.isHit = true; //save sorting or exit
        if(what == 1)
            Debug.Log("Correct solution given for " + word);
        if(what == 2)
            Debug.Log("Wrong solution given for " + word);

        //play sounds
        if(currentBlubble.wordSource) //if the word audio is currently played, stop it
            currentBlubble.wordSource.Stop();
        if(currentBlubble.pop) 
            currentBlubble.pop.Play();
        if(what == 1)
            if(currentBlubble.success)
                currentBlubble.success.Play();
        if(what == 2)
            if(currentBlubble.fail)
                currentBlubble.fail.Play();

        if(what == 1) //let robo change body color in case of correct sorting
            NetworkManager.Instance.RoboChangeMaterial(); 

        //play particle system
        ParticleSystem ps = currentBlubble.GetComponent<ParticleSystem>();
        if(ps){ 
            ParticleSystem.ColorOverLifetimeModule colorModule = ps.colorOverLifetime;
            if(what == 1) //adjust effect color based on sorting result  
                colorModule.color = new ParticleSystem.MinMaxGradient(currentBlubble.correctColorStart, currentBlubble.correctColorEnd);
            if(what == 2) //adjust effect color based on sorting result  
                colorModule.color = new ParticleSystem.MinMaxGradient(currentBlubble.wrongColorStart, currentBlubble.wrongColorEnd);
            ps.Play(); 
            //hide bubble & text after explosion
            currentBlubble.GetComponent<Renderer>().enabled = false; 
            currentBlubble.GetComponentInChildren<TextMeshPro>().GetComponent<Renderer>().enabled = false;
            if(currentBlubble.GetComponentInChildren<Canvas>())
                currentBlubble.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
        }

        //report sorting or exit
        switch(what){
            case 0: GameController.Instance.Fail(word);
                break; 
            case 1: //trigger score increase & save correct answer
                    if(currentBlubble.hintUsed){
                        GameController.Instance.Congrats(1, word); //if hint was used add one point to the score
                    } else {
                        GameController.Instance.Congrats(2, word); //if hint wasn't used add two points to the score
                    }
                break; 
            case 2: GameController.Instance.Fail(word, bucket);
                break;
            default: GameController.Instance.Fail(word);
                break;
        }

        //disable & destroy bubble
        currentBlubble.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); //disable selection
        currentBlubble.gameObject.GetComponent<Collider>().enabled = false; //disable events
        if(PhotonNetwork.IsMasterClient){
            switch(what){
                case 0: StartCoroutine(DestroyBluble(currentBlubble.pop.clip.length, viewID));
                    break; 
                case 1: StartCoroutine(DestroyBluble(currentBlubble.success.clip.length, viewID));
                    break; 
                case 2: StartCoroutine(DestroyBluble(currentBlubble.fail.clip.length-1, viewID)); //sound was a bit too long in the end, adjust for other sound or edit source
                    break; 
                default: StartCoroutine(DestroyBluble(currentBlubble.pop.clip.length, viewID));
                    break;
            } 
        }
    }

    IEnumerator DestroyBluble(float time, int viewID){ //MasterONLYfunction
        yield return new WaitForSeconds(time); //wait for audio clip end
        PhotonView.Find(viewID).TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
        if(this.GetComponent<PhotonView>().IsMine)
            PhotonNetwork.Destroy(PhotonView.Find(viewID).gameObject);
    }
    //---- addedCodeEnd ------

     [Serializable]
    public class UnityEventDraggable : UnityEvent<BlubleDraggable> { }

    public class Grabber : GrabberBase<PointerEventData>
    {
        private static GrabberPool m_pool;
        private PointerEventData m_eventData;

        public static Grabber Get(PointerEventData eventData)
        {
            if (m_pool == null)
            {
                m_pool = new GrabberPool(() => new Grabber());
            }

            var grabber = m_pool.Get();
            grabber.m_eventData = eventData;
            return grabber;
        }

        public static void Release(Grabber grabber)
        {
            grabber.m_eventData = null;
            m_pool.Release(grabber);
        }

        public override PointerEventData eventData { get { return m_eventData; } }

        public override RigidPose grabberOrigin
        {
            get
            {
                var cam = eventData.pointerPressRaycast.module.eventCamera;
                var ray = cam.ScreenPointToRay(eventData.position);
                return new RigidPose(ray.origin, Quaternion.LookRotation(ray.direction, cam.transform.up)) * grabber2hit;
            }
        }

        public override RigidPose grabOffset { get { return hit2pivot; } set { hit2pivot = value; } }

        public RigidPose grabber2hit { get; set; }

        public RigidPose hit2pivot { get; set; }

        public float hitDistance
        {
            get { return grabber2hit.pos.z; }
            set
            {
                var p = grabber2hit;
                p.pos.z = value;
                grabber2hit = p;
            }
        }
    }

    private LiteCoroutine m_updateCoroutine;
    private LiteCoroutine m_physicsCoroutine;

    [FormerlySerializedAs("initGrabDistance")]
    [SerializeField]
    private float m_initGrabDistance = 0.5f;
    [Range(MIN_FOLLOWING_DURATION, MAX_FOLLOWING_DURATION)]
    [FormerlySerializedAs("followingDuration")]
    [SerializeField]
    private float m_followingDuration = DEFAULT_FOLLOWING_DURATION;
    [FormerlySerializedAs("overrideMaxAngularVelocity")]
    [SerializeField]
    private bool m_overrideMaxAngularVelocity = true;
    [FormerlySerializedAs("unblockableGrab")]
    [SerializeField]
    private bool m_unblockableGrab = true;
    [SerializeField]
    [FormerlySerializedAs("m_scrollDelta")]
    private float m_scrollingSpeed = 0.01f;
    [SerializeField]
    private float m_minStretchScale = 1f;
    [SerializeField]
    private float m_maxStretchScale = 1f;
    [FormerlySerializedAs("afterGrabbed")]
    [SerializeField]
    private UnityEventDraggable m_afterGrabbed = new UnityEventDraggable();
    [FormerlySerializedAs("beforeRelease")]
    [SerializeField]
    private UnityEventDraggable m_beforeRelease = new UnityEventDraggable();
    [FormerlySerializedAs("onDrop")]
    [SerializeField]
    private UnityEventDraggable m_onDrop = new UnityEventDraggable(); // change rigidbody drop velocity here

    public bool isDragged { get { return isGrabbed; } }

    public PointerEventData draggedEvent { get { return isGrabbed ? currentGrabber.eventData : null; } }

    public float initGrabDistance { get { return m_initGrabDistance; } set { m_initGrabDistance = value; } }

    public override float followingDuration { get { return m_followingDuration; } set { m_followingDuration = Mathf.Clamp(value, MIN_FOLLOWING_DURATION, MAX_FOLLOWING_DURATION); } }

    public override bool overrideMaxAngularVelocity { get { return m_overrideMaxAngularVelocity; } set { m_overrideMaxAngularVelocity = value; } }

    public bool unblockableGrab { get { return m_unblockableGrab; } set { m_unblockableGrab = value; } }

    public override float minScaleOnStretch { get { return m_minStretchScale; } set { m_minStretchScale = value; } }

    public override float maxScaleOnStretch { get { return m_maxStretchScale; } set { m_maxStretchScale = value; } }

    public UnityEventDraggable afterGrabbed { get { return m_afterGrabbed; } }

    public UnityEventDraggable beforeRelease { get { return m_beforeRelease; } }

    public UnityEventDraggable onDrop { get { return m_onDrop; } }

    private bool moveByVelocity { get { return !unblockableGrab && grabRigidbody != null && !grabRigidbody.isKinematic; } }

    [Obsolete("Use grabRigidbody instead")]
    public Rigidbody rigid { get { return grabRigidbody; } set { grabRigidbody = value; } }

    public float scrollingSpeed { get { return m_scrollingSpeed; } set { m_scrollingSpeed = value; } }

    protected override void Awake()
    {
        base.Awake();

        afterGrabberGrabbed += () => m_afterGrabbed.Invoke(this);
        beforeGrabberReleased += () => m_beforeRelease.Invoke(this);
        onGrabberDrop += () => m_onDrop.Invoke(this);
    }

    protected virtual void OnDisable() { ForceRelease(); }

    protected override Grabber CreateGrabber(PointerEventData eventData)
    {
        var hitResult = eventData.pointerPressRaycast;
        float distance;
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Middle:
            case PointerEventData.InputButton.Right:
                distance = Mathf.Min(hitResult.distance, initGrabDistance);
                break;
            case PointerEventData.InputButton.Left:
                distance = hitResult.distance;
                break;
            default:
                return null;
        }

        var grabber = Grabber.Get(eventData);
        grabber.grabber2hit = new RigidPose(new Vector3(0f, 0f, distance), Quaternion.identity);
        grabber.hit2pivot = RigidPose.FromToPose(new RigidPose(hitResult.worldPosition, hitResult.module.eventCamera.transform.rotation), new RigidPose(transform));
        return grabber;
    }

    protected override void DestoryGrabber(Grabber grabber)
    {
        Grabber.Release(grabber);
    }

    public virtual void OnInitializePotentialDrag(PointerEventData eventData)
    {
        eventData.useDragThreshold = false;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (AddGrabber(eventData))
        {
            if (m_updateCoroutine.IsNullOrDone())
            {
                LiteCoroutine.StartCoroutine(ref m_updateCoroutine, DragUpdate(), false);

                if (moveByVelocity)
                {
                    LiteCoroutine.StartCoroutine(ref m_physicsCoroutine, PhysicsGrabUpdate(), false);
                }
            }
        }
    }

    private static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    private IEnumerator PhysicsGrabUpdate()
    {
        yield return waitForFixedUpdate;

        while (isGrabbed)
        {
            OnGrabRigidbody();

            yield return waitForFixedUpdate;
        }

        yield break;
    }

    private IEnumerator DragUpdate()
    {
        yield return null;

        while (isGrabbed)
        {
            for (int i = allGrabbers.Count - 1; i >= 0; --i)
            {
                var grabber = allGrabbers.GetValueByIndex(i);
                var scrollDelta = grabber.eventData.scrollDelta * m_scrollingSpeed;
                if (scrollDelta != Vector2.zero)
                {
                    grabber.hitDistance = Mathf.Max(0f, grabber.hitDistance + scrollDelta.y);
                }
            }

            if (!moveByVelocity)
            {
                RecordLatestPosesForDrop(Time.time, 0.05f);
                OnGrabTransform();
            }

            yield return null;
        }
    }

    public virtual void OnDrag(PointerEventData eventData) { }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        RemoveGrabber(eventData);
    }
}
