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

// demonstrate of dragging things useing built in EventSystem handlers
public class BlubleDraggable : GrabbableBase<PointerEventData, BlubleDraggable.Grabber> 
    , IInitializePotentialDragHandler
    , IBeginDragHandler
    , IDragHandler
    , IEndDragHandler
{
    //---- added Code ----
    [Tooltip("Time factor for bubble constant movement forwards")]
    public float timeFactor = 1;
    [Tooltip("Time factor for floating")]
    public float fSpeed = 0.25f;

    [Tooltip("Audio to be played in case of bluble emerging")]
    public AudioSource emergingBluble;
    [Tooltip("Audio to be played in case of wrong sorting")]
    public AudioSource fail; 
    [Tooltip("Audio to be played in case of correct sorting")]
    public AudioSource success;
    [Tooltip("Audio to be played in case of bluble disappearing")]
    public AudioSource pop; 
    [Tooltip("Material for bluble to be set in case of correct sorting")]
    public Material green;
    [Tooltip("Material for bluble to be set in case of wrong sorting")]
    public Material red;
    
    private float time = 0; //for constant movement
    private float deviationX = 0; //set by initalisation to stay constant
    private float initialY = 0; //set by initalisation to stay constant
    private bool isHit = false; //if the bluble was already sorted but still exists
    private AudioSource wordSource = null; //audio for the spoken vocabulary, set by initialisation

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient){ //only the master client should control the bubbles
            if(!isHit && !isDragged && !GameController.Instance.GetIsPaused()){ //bubble should not forcefully move if it is already sorted, currently dragged or the game is paused
                time += 0.02f;
                float y = Mathf.Sin(time) * fSpeed + initialY; //move up and down depending on time
                transform.position = new Vector3(deviationX + Camera.main.transform.position.x, y,  -1 * timeFactor * Time.deltaTime + transform.position.z); //move forwards towards the camera
            }
            //Rotate text to camera
            this.GetComponentInChildren<TextMeshPro>().transform.rotation = Camera.main.transform.rotation;
        }
    }

    public void SetDeviationX(float deviation){
        deviationX = deviation;
    }
    public void SetInitialY(float initial){
        initialY = initial;
    }

    public void PlayAudioText(){
        Component[] audios = this.GetComponents(typeof(AudioSource)); //Searching for the child audio component containing the vocabulary on the bluble and play it
        for(int i = 0; i < audios.Length; i++) {
            AudioSource wordAudio = audios[i] as AudioSource;
            //Debug.Log(wordAudio.clip.ToString() + " " + this.GetComponentInChildren<TextMeshPro>().text);
            if(wordAudio.clip.ToString().Contains(this.GetComponentInChildren<TextMeshPro>().text)){
                wordAudio.Play();
                wordSource = wordAudio;
            }
        }
    }

    private void OnCollisionEnter(Collision other){
        if(!isHit){ //if the bubble was already sorted in, do not act again 
            Collider myCollider = gameObject.GetComponent<Collider>();
            Renderer myRenderer = gameObject.GetComponent<Renderer>();
            if((other.collider.CompareTag("Bucket_der") && myCollider.CompareTag("der")) || (other.collider.CompareTag("Bucket_die") && myCollider.CompareTag("die")) || (other.collider.CompareTag("Bucket_das") && myCollider.CompareTag("das"))){ //correct sorting
                Debug.Log("Richtig!");
                //PhotonView photonView = this.GetComponent<PhotonView>();
                //photonView.RPC("Successful", RpcTarget.All, photonView.ViewID); 
                isHit = true; //save sorting
                if(wordSource){ //if word audio is currently played, stop it
                    wordSource.Stop();
                }
                if(pop) //play sounds
                    pop.Play();
                if(success) {
                    success.Play();
                }
                //myRenderer.material = green;
                RoboMaterialController rob = GameController.Instance.GetRobo().GetComponentInChildren<RoboMaterialController>();
                rob.RoboChangeMaterial(); //let robo change body color

                ParticleSystem ps = this.GetComponent<ParticleSystem>();
                if(ps){ //play particle system
                    ParticleSystem.ColorOverLifetimeModule colorModule = ps.colorOverLifetime;
                    colorModule.color = new ParticleSystem.MinMaxGradient(new Color(0.0f, 1.0f, 0.0f, 0.8f), new Color(0.43f, 0.01f, 0.47f, 0.5f));
                    ps.Play(); 
                    this.GetComponent<Renderer>().enabled = false; //hide bubble & text
                    this.GetComponentInChildren<TextMeshPro>().GetComponent<Renderer>().enabled = false;
                }
                PhotonView photonView = GameController.Instance.GetComponent<PhotonView>();
                photonView.RPC("Congrats", RpcTarget.All, 2, this.GetComponentInChildren<TextMeshPro>().text);
                //GameController.Instance.Congrats(2, this.GetComponentInChildren<TextMeshPro>().text); //scoreIncrease
                int LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                this.gameObject.layer = LayerIgnoreRaycast;
                //Destroy(this.gameObject,success.clip.length); //wait until sound is played*/
                BlubleDestroyer(success.clip.length);
            }
            
            if ((other.collider.CompareTag("Bucket_der") && myCollider.CompareTag("die")) || (other.collider.CompareTag("Bucket_der") && myCollider.CompareTag("das")) || (other.collider.CompareTag("Bucket_die") && myCollider.CompareTag("der")) || (other.collider.CompareTag("Bucket_die") && myCollider.CompareTag("das")) || (other.collider.CompareTag("Bucket_das") && myCollider.CompareTag("der")) || (other.collider.CompareTag("Bucket_das") && myCollider.CompareTag("die"))){
                isHit = true;
                if(wordSource){
                    wordSource.Stop();
                }
                if(pop)
                    pop.Play();
                if(fail) {
                    fail.Play();
                }
                //myRenderer.material = red;
                ParticleSystem ps = this.GetComponent<ParticleSystem>();
                if(ps){
                    ParticleSystem.ColorOverLifetimeModule colorModule = ps.colorOverLifetime;
                    colorModule.color = new ParticleSystem.MinMaxGradient(new Color(1.0f, 0.0f, 0.0f, 0.8f), new Color(0.43f, 0.01f, 0.47f, 0.5f));
                    ps.Play(); 
                    myRenderer.enabled = false;
                    //yield return new WaitForSeconds(ps.main.duration);
                    this.GetComponentInChildren<TextMeshPro>().GetComponent<Renderer>().enabled = false;
                }
                Debug.Log("Leider falsch");
                GameController.Instance.Fail(this.GetComponentInChildren<TextMeshPro>().text, other.collider.tag); //save wrong answer
                int LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                this.gameObject.layer = LayerIgnoreRaycast;
                //Destroy(this.gameObject, fail.clip.length-1); //sound was a bit to long in the end, adjust for other sound or edit sound 
                BlubleDestroyer(fail.clip.length-1);
            } //PhotonNetwork.Destroy needed
        
            if(other.collider.tag == "Floor_end" || other.collider.tag == "Player") { //destroy blubles if the hit the player
                isHit = true;
                if(wordSource){
                    wordSource.Stop();
                }
                if(pop)
                    pop.Play();
                ParticleSystem ps = this.GetComponent<ParticleSystem>();
                if(ps){
                    ps.Play(); 
                }
                myRenderer.enabled = false;
                this.GetComponentInChildren<TextMeshPro>().GetComponent<Renderer>().enabled = false;
                GameController.Instance.Fail(this.GetComponentInChildren<TextMeshPro>().text); //save destroyed bluble
                int LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                this.gameObject.layer = LayerIgnoreRaycast;
                //Destroy(this.gameObject, pop.clip.length);
                BlubleDestroyer(pop.clip.length);
            }
        }
    }

    public void BlubleDestroyer(float time){
        StartCoroutine(DestroyBluble(time));
    }

    private IEnumerator DestroyBluble(float time){
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(this.gameObject);
    }

    /*[PunRPC] //IEnumerator
    public void Successful(int viewID){
        isHit = true; //save sorting
        if(wordSource){ //if word audio is currently played, stop it
            wordSource.Stop();
        }
        if(pop) //play sounds
            pop.Play();
        if(success) {
            success.Play();
        }
        //myRenderer.material = green;
        RoboMaterialController rob = GameController.Instance.GetRobo().GetComponentInChildren<RoboMaterialController>();
        rob.RoboChangeMaterial(); //let robo change body color

        ParticleSystem ps = this.GetComponent<ParticleSystem>();
        if(ps){ //play particle system
            ParticleSystem.ColorOverLifetimeModule colorModule = ps.colorOverLifetime;
            colorModule.color = new ParticleSystem.MinMaxGradient(new Color(0.0f, 1.0f, 0.0f, 0.8f), new Color(0.43f, 0.01f, 0.47f, 0.5f));
            ps.Play(); 
            this.GetComponent<Renderer>().enabled = false; //hide bubble & text
            this.GetComponentInChildren<TextMeshPro>().GetComponent<Renderer>().enabled = false;
        }
        //PhotonView photonView = GameController.Instance.GetComponent<PhotonView>();
        //photonView.RPC("Congrats", RpcTarget.All, 2, this.GetComponentInChildren<TextMeshPro>().text);
        GameController.Instance.Congrats(2, this.GetComponentInChildren<TextMeshPro>().text); //scoreIncrease
        Destroy(this.gameObject,success.clip.length); //wait until sound is played
    }*/

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

        //added Code to play word audio on grabbing of the bubble
        afterGrabberGrabbed += () => PlayAudioText();

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
