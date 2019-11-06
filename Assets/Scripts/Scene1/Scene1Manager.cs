#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using UnityEngine.UI;

[Serializable]
public class SceneObjectsContainer
{
    public string name;
    public List<Transform> sceneObjects;
}

public class Scene1Manager : MonoBehaviour, ISceneManager
{
    #region PRIVATE FIELDS
    [SerializeField] private List<SceneObjectsContainer> _sceneContainers;
    private Dictionary<string, List<Transform>> _sceneObjectsMap = new Dictionary<string, List<Transform>>();
    [SerializeField] private List<Interactable> _interactables;
    public int lastPickedAnswer { get; set; }
    public GlitchEffect glitch;
    private GameObject _player;
    [SerializeField] private Vector3 _playerInitialPos;
    [SerializeField] private Vector3 _playerInitialRot;
    [SerializeField] private Vector3 _playerAwakePos;
    [SerializeField] private Vector3 _playerAwakeRot;
    [SerializeField] private GameObject _navMeshPassage;
    [SerializeField] private AudioSource _bgMusic;
    [SerializeField] private AudioSource _alarmSound;
    [SerializeField] private AudioClip _calmMusic;
    [SerializeField] private AudioClip _intenseMusic;
    [SerializeField] private DoorJamesRoom _doorJamesRoom;
    [SerializeField] private DoorKidnapRoom _doorKidnapRoom;
    private int _itemInteractionCount;
    private const string JAMES_ROOM = "JamesRoom";
    private const string HALL = "Hall";
    private const string KIDNAP_ROOM = "KidnapRoom";
    private const string HALL_INTERS = "HallwayInteractables";
    private const int ITEM_INTERACTION_NEEDED = 3;
    public const bool DEBUG = false;
    public static Text debugText;
    private const string TUTORIAL1 = "tutorial1";
    private const string TUTORIAL2 = "tutorial2";
    private const string TUTORIAL3 = "tutorial3";
    private const string CROWD_INSANE = "CrowdInsane";
    private const string CROWD_HELP = "CrowdHelp";
    public const string CROWD_OMG = "CrowdOMG";
    public const string DOCTOR1 = "DoctorTalk1";
    public const string DOCTOR2 = "DoctorTalk2";
    public const string DOCTOR3 = "DoctorTalk3";
    public const string DOCTOR4 = "DoctorTalk4";
    public const string DOCTOR5 = "DoctorTalk5";
    public const string DOCTOR6 = "DoctorTalk6";
    public const string DOCTOR7 = "DoctorTalk7";
    public const string DOCTOR8 = "DoctorTalk8";
    public const string DOCTOR9 = "DoctorTalk9";
    public const string COAT = "Coat1";
    public const string GUN = "Gun";
    public const string CHART = "MedicalChart1";
    public const string KIDNAP1 = "Kidnap1";
    public const string KIDNAP2 = "Kidnap2";
    public const string KIDNAP3 = "Kidnap3";
    public const string KIDNAP4 = "Kidnap4";
    public const string KIDNAP5 = "Kidnap5";
    public const string KIDNAP6 = "Kidnap6";
    public const string KIDNAP7 = "Kidnap7";
    public const string KIDNAP8 = "Kidnap8";
    public const string CORRIDOR1 = "Corridor1";
    public const string CORRIDOR2 = "Corridor2";
    public const string CORRIDOR3 = "Corridor3";
    public const string MEMORY1 = "Memory1";
    public const string MEMORY2 = "Memory2";
    public const string DOGTAGS = "DogTags";
    public static Quest currentQuest { get; protected set; }
    public bool JamesThreatened { get; private set; }
    public bool JamesShot { get; private set; }
    public static bool BlockMovement { get; private set; }
    #endregion

    #region PROPERTIES
    public List<Interactable> interactables
    {
        get
        {
            return _interactables;
        }

        set
        {
            _interactables = value;
        }
    }
    #endregion

    #region EVENTS AND DELEGATES
    public event Action<SceneEventParams> OnSceneEventTriggerred;
    public static event Action OnFinishedTutorial;
    public static event Action OnCrowdNoise;
    public static event Action OnPlayerEnteredHostageRoom;
    public static event Action OnPlayerCollectedGun;
    public static event Action OnPlayerExitedMainRoom;
    public static event Action OnEndScene;
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        foreach (SceneObjectsContainer co in _sceneContainers)
        {
            _sceneObjectsMap.Add(co.name, co.sceneObjects);
        }
        HideSceneElements(_sceneObjectsMap[HALL].ToArray());
        HideSceneElements(_sceneObjectsMap[KIDNAP_ROOM].ToArray());
        HideSceneElements(_sceneObjectsMap[HALL_INTERS].ToArray());
    }

    private IEnumerator Start()
    {
        Init();
        yield return new WaitForSeconds(1);
        //PlayAwakeningAnimation();
        _navMeshPassage.SetActive(false);
        OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(10f, TUTORIAL1));
        ToggleMusic(_calmMusic, true);
        
    }

    private void OnDisable()
    {
        DeInit();
    }


    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Time.timeScale = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            Time.timeScale = 2;
#endif
    }


    #endregion

    #region PUBLIC METHODS
    public void Init()
    {
        glitch = Camera.main.GetComponent<GlitchEffect>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _player.transform.position = _playerInitialPos;
        _player.transform.rotation = Quaternion.Euler(_playerInitialRot);
        Kidnapper.OnKidnapperDialogue += HandleKidnapperDialogue;
        Kidnapper.OnShootQuickEvent += HandleShootQuickEvent;
        Kidnapper.OnKidnapperDied += HandleKidnapperDied;
        UIHandler.S.RegisterToSceneManager(this);
        UIHandler.OnGunCollected += HandleGunCollected;
        Movement.S.RegisterToSceneManager(this);
        MessageBox.OnButtonClicked += HandleMessageBoxPlayerClicked;
        Doctor.OnDoctorDialogue += HandleDoctorDialogue;
        Doctor.OnDoctorSteppedOutOfRoom += HandleDoctorSteppedOutOfRoom;
        Movement.OnPlayerEnteredRoom += HandlePlayerEnteredNewRoom;
        CorridorTrigger.OnCorridorTriggerHit += HandleCorridorTrigger;
        ArticleBox.OnCloseArticle += HandleArticleClosed;
        Quest.OnQuestFinished += HandleQuestFinished;
        MessageBox.OnDialogueStatusUpdate += HandleDialogueStatusChanged;
        JamesThreatened = false;
        JamesShot = false;
        StartQuest(new TutorialQuest());
        BlockMovement = true;
    }
    #endregion

    #region PRIVATE METHODS
    private IEnumerator PlayAwakeningAnimation()
    {
        yield return null;
        Movement.S.ToggleRootMotion(false);
        _player.GetComponentInChildren<Animator>().SetTrigger("GetUp");

        _player.transform.DOMove(_playerAwakePos, 1.25f).SetEase(Ease.Linear).OnComplete(() =>
        {
            StartCoroutine(PlayGlitchEffect());
            //Movement.S.SetNavMeshAgent(true);
            OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(3f, TUTORIAL2));
        });
    }

    private IEnumerator PlayGlitchEffect()
    {
        yield return new WaitForSeconds(1f);
        glitch.enabled = true;
        yield return new WaitForSeconds(2f);
        glitch.enabled = false;
    }

    public void DeInit()
    {
        UIHandler.S.DeregisterFromSceneManager();
        Movement.S.DeregisterFromSceneManager();
        MessageBox.OnButtonClicked -= HandleMessageBoxPlayerClicked;
        Doctor.OnDoctorDialogue -= HandleDoctorDialogue;
        Doctor.OnDoctorSteppedOutOfRoom -= HandleDoctorSteppedOutOfRoom;
        Movement.OnPlayerEnteredRoom -= HandlePlayerEnteredNewRoom;
        Kidnapper.OnKidnapperDialogue -= HandleKidnapperDialogue;
        Kidnapper.OnShootQuickEvent -= HandleShootQuickEvent;
        Kidnapper.OnKidnapperDied -= HandleKidnapperDied;
        UIHandler.OnGunCollected -= HandleGunCollected;
        CorridorTrigger.OnCorridorTriggerHit -= HandleCorridorTrigger;
        ArticleBox.OnCloseArticle -= HandleArticleClosed;
        Quest.OnQuestFinished -= HandleQuestFinished;
        MessageBox.OnDialogueStatusUpdate -= HandleDialogueStatusChanged;
    }

    private void HandleArticleClosed(string articleName)
    {
        //if(Scene1Manager.CanPlayerInteract())
        //{
        //    Movement.S.SetAllowedToMove(true);
        //}
        //if (articleName.Equals("DoctorQuintonRhyne"))
        //    _searchedDoctorInDB = true;
        //if(_itemInteractionCount == ITEM_INTERACTION_NEEDED && _searchedDoctorInDB)
        //{
        //    OnInteractedWithAllItemsAndSearchedDB?.Invoke();
        //    _navMeshPassage.SetActive(true);
        //}
    }

    public static bool CanPlayerInteract()
    {
        return !UIHandler.IsDisplayingElements() && !BlockMovement && !Movement.S.IsInCutScene && !Movement.S.IsMoving();
    }

    public void HandleGunCollected()
    {
        //Movement.S.SetAllowedToMove(true);
    }

    public void StartQuest(Quest quest)
    {
        currentQuest = quest;
        currentQuest.OnStartQuest();
    }

    private void HandleQuestFinished(Quest q)
    {
        if (currentQuest != q)
            Debug.LogError("Ended quest does not match current quest. This shouldn't happen!");
        
        if(currentQuest is TutorialQuest)
        {
            OnFinishedTutorial?.Invoke();
            _navMeshPassage.SetActive(true);
        }
        else if(currentQuest is CodePadQuest)
        {
            _doorKidnapRoom.Locked = false;
        }
        currentQuest.OnEndQuest();
        currentQuest = null;
    }

    private void HandleShootQuickEvent()
    {
        OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(0.5f, KIDNAP3));
    }

    private void HandleKidnapperDied(bool shotByJames)
    {
        UIHandler.S.DismissAllDialogues();
        //OnKidnapperDied?.Invoke();
        ToggleMusic(_intenseMusic, false);
        if (shotByJames)
            HandlePlayerChoseToShoot();
        if(JamesThreatened)
        {
            if(JamesShot)
            {
                OnSceneEventTriggerred(new SceneDialogueEventParams(1f, KIDNAP4));
            }
            else
            {
                OnSceneEventTriggerred(new SceneDialogueEventParams(1f, KIDNAP5));
            }
        }
        else
        {
            if (JamesShot)
            {
                OnSceneEventTriggerred(new SceneDialogueEventParams(1f, KIDNAP6));
            }
            else
            {
                OnSceneEventTriggerred(new SceneDialogueEventParams(1f, KIDNAP7));
            }
        }
        Movement.S.HolsterGun();
    }

    private void HandleKidnapperDialogue(string dialogueKey)
    {
        OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(0.5f, dialogueKey));
        if (dialogueKey == KIDNAP2)
            Movement.S.DrawGun();
    }

    private void HandleDialogueStatusChanged(string dialogueName, MessageBox.DialogueStatus dialogueStatus, bool block)
    {
        
        BlockMovement = block;
        switch (dialogueStatus)
        {
            case MessageBox.DialogueStatus.None:
                break;
            case MessageBox.DialogueStatus.Ended:
                break;
            case MessageBox.DialogueStatus.Continued:
                break;
            case MessageBox.DialogueStatus.QuickEventHit:
                break;
            case MessageBox.DialogueStatus.QuickEventMissed:
                break;
            case MessageBox.DialogueStatus.Answer1:
            case MessageBox.DialogueStatus.Answer2:
            case MessageBox.DialogueStatus.Answer3:
            case MessageBox.DialogueStatus.Answer4:
                lastPickedAnswer = (int)dialogueStatus;
                break;
            default:
                break;
        }
    }
   
    private void HandleMessageBoxPlayerClicked(string dialogueName, int buttonPressed, bool block)
    {
        switch (dialogueName)
        {
            case GUN:
                OnPlayerCollectedGun?.Invoke();
                break;
            case TUTORIAL1:
                if (buttonPressed == MessageBox.END_DIALOGUE)
                    StartCoroutine(PlayAwakeningAnimation());
                break;
            case TUTORIAL2:
                OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(1.5f, TUTORIAL3));
                break;
            case KIDNAP3:
                if (buttonPressed == MessageBox.QUICK_EVENT_HIT)
                    HandlePlayerChoseToShoot();
                break;
            case DOCTOR5:
                if (buttonPressed == 1)
                    HandlePlayerChoseToThreat();
                break;
            case KIDNAP4:
            case KIDNAP5:
            case KIDNAP6:
                if (buttonPressed == MessageBox.END_DIALOGUE)
                    OnEndScene?.Invoke();
                break;
            case KIDNAP7:
                if (buttonPressed == MessageBox.END_DIALOGUE)
                    OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(0.5f, KIDNAP8));
                break;
            case KIDNAP8:
                OnEndScene?.Invoke();
                break;
            default:
                break;
        }
    }
    
    private void HandlePlayerChoseToThreat()
    {
        JamesThreatened = true;
    }

    private void HandlePlayerChoseToShoot()
    {
        Movement.S.Shoot();
        JamesShot = true;
    }

    private void HandleDoctorDialogue(string str, float delay)
    {
        switch (str)
        {
            case DOCTOR1:
                OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(3f, DOCTOR1));
                break;
            case DOCTOR2:
                OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(0.2f, DOCTOR2));
                break;
            case DOCTOR3:
                OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(0.2f, DOCTOR3));
                break;
            case DOCTOR4:
                OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(0.2f, DOCTOR4));
                break;
            case DOCTOR5:
                OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(0.2f, DOCTOR5));
                break;
            case DOCTOR6:
                OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(0.2f, DOCTOR6));
                break;
            case DOCTOR7:
                OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(0.2f, DOCTOR7));
                break;
            case DOCTOR8:
                OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(0.2f, DOCTOR8));
                break;
            case DOCTOR9:
                OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(0.2f, DOCTOR9));
                break;
            case Doctor.FINISHED_DIALOGUE_1:
                StartCoroutine(CrowdNoiseCoroutine());
                break;
            default:
                break;
        }
    }

    private void HandleDoctorSteppedOutOfRoom()
    {
        BlockMovement = false;
        _doorJamesRoom.Locked = false;
        StartQuest(new CodePadQuest());
    }

    private void ToggleAlarm(bool on)
    {
        if (on)
        {
            _alarmSound.volume = 0.25f;
            _alarmSound.Play();
        }
            
        else
            _alarmSound.DOFade(0, 2f).OnComplete(()=>_alarmSound.Stop());
    }

    private void ToggleMusic(AudioClip ac, bool on)
    {
        StartCoroutine(ToggleMusicCoroutine(ac, on));
    }

    private IEnumerator ToggleMusicCoroutine(AudioClip ac, bool on)
    {
       
        if (on)
        {
            yield return new WaitForSeconds(2.1f);
            _bgMusic.clip = ac;
            _bgMusic.volume = 0;
            _bgMusic.Play();
            _bgMusic.DOFade(0.25f, 2f);
        }
        else
            _bgMusic.DOFade(0f, 2f);
    }

    private IEnumerator CrowdNoiseCoroutine()
    {
        ToggleMusic(_calmMusic, false);
        ToggleAlarm(true);
        yield return new WaitForEndOfFrame();
        Movement.S.SetAllowedToMove(false);
        OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(1.5f, CROWD_OMG));
        yield return new WaitForSeconds(0.5f);
        OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(1.5f, CROWD_INSANE));
        yield return new WaitForSeconds(0.5f);
        OnSceneEventTriggerred?.Invoke(new SceneDialogueEventParams(1.5f, CROWD_HELP));
        yield return new WaitForSeconds(0.5f);
        OnCrowdNoise?.Invoke();
        Movement.S.SetAllowedToMove(true);
        yield return new WaitForSeconds(5f);
        ToggleAlarm(false);
        ToggleMusic(_intenseMusic, true);
    }

    private void HandlePlayerEnteredNewRoom(Room currentRoom, Room prevRoom)
    {
        switch (currentRoom)
        {
            case Room.Corridor:
                if(prevRoom == Room.MainRoom)
                {
                    OnSceneEventTriggerred?.Invoke(new PlayerEnteredNewRoomEventParams(Room.Corridor, Room.MainRoom, true));
                    OnPlayerExitedMainRoom?.Invoke();
                    DisplaySceneElements(_sceneObjectsMap[HALL].ToArray());
                    DisplaySceneElements(_sceneObjectsMap[HALL_INTERS].ToArray());
                }
                else if(prevRoom == Room.HostageRoom)
                {
                    OnSceneEventTriggerred?.Invoke(new PlayerEnteredNewRoomEventParams(Room.Corridor, Room.HostageRoom, false));
                }
                break;
            case Room.HostageRoom:
                if(!_doorKidnapRoom.Locked)
                {
                    OnSceneEventTriggerred?.Invoke(new PlayerEnteredNewRoomEventParams(Room.HostageRoom, Room.Corridor, true));
                    BlockMovement = true;
                    OnPlayerEnteredHostageRoom?.Invoke();
                    HideSceneElements(_sceneObjectsMap[HALL].ToArray());
                    HideSceneElements(_sceneObjectsMap[HALL_INTERS].ToArray());
                    HideSceneElements(_sceneObjectsMap[JAMES_ROOM].ToArray());
                    DisplaySceneElements(_sceneObjectsMap[KIDNAP_ROOM].ToArray());
                }
                break;
            case Room.MainRoom:
                if(prevRoom == Room.Corridor)
                    OnSceneEventTriggerred?.Invoke(new PlayerEnteredNewRoomEventParams(Room.MainRoom, Room.Corridor, true));
                break;
            default:
                break;
        }
    }

    private void HandleCorridorTrigger(int index)
    {
        StartCoroutine(CorridorTriggerCoroutine(index));
    }

    private IEnumerator CorridorTriggerCoroutine(int index)
    {
        
        if(index == 0)
        {
            OnSceneEventTriggerred(new SceneDialogueEventParams(0, CORRIDOR1));
        }
        else
        {
            OnSceneEventTriggerred(new SceneDialogueEventParams(0, CORRIDOR2));
            yield return new WaitForSeconds(1f);
            OnSceneEventTriggerred(new SceneDialogueEventParams(0, CORRIDOR3));
        }
    }

    private void DisplaySceneElements(params Transform[] gos)
    {
        foreach (Transform parent in gos)
        {
            Renderer ren = parent.GetComponent<Renderer>();
            if(ren)
            {
                ren.enabled = true;
            }
            Collider col = parent.GetComponent<Collider>();
            if(col)
            {
                col.enabled = true;
            }
        }
    }

    private void HideSceneElements(params Transform[] gos)
    {
        foreach (Transform parent in gos)
        {
            Renderer ren = parent.GetComponent<Renderer>();
            if (ren)
            {
                ren.enabled = false;
            }
            Collider col = parent.GetComponent<Collider>();
            if (col)
            {
                col.enabled = false;
            }
        }
    }
    #endregion

}



