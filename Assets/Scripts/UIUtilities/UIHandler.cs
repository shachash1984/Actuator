#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using TMPro;

public enum AnimationType { None, BlackScreen}

public class UIHandler : MonoBehaviour {

    static public UIHandler S;
    private ISceneManager currentSceneManager;
    [SerializeField] private InputField _dbSearchField;
    [SerializeField] private Button _dbSearchButton;
    [SerializeField] private GameObject _articlePanelPrefab;
    [SerializeField] private GameObject _articlePrefab;
    [SerializeField] private GameObject _messagePanelPrefab;
    [SerializeField] private Sprite _arrowSprite;
    [SerializeField] private GameObject _timerPrefab;
    [SerializeField] private GameObject _interactiveWordPrefab;
    private static GameObject _currentTimer;
    private Transform _canvasTransform;
    [SerializeField] private CanvasGroup _blackPanel;
    public static event Action OnGunCollected;
    public static event Action<bool> OnNotificationStatusChanged;
    public Sequence seqDisplay;
    public Sequence seqNext;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private CanvasGroup _gamePlayPanel;
    [SerializeField] private TextMeshProUGUI _endText;
    [SerializeField] private RectTransform _thoughtsLog;
    [SerializeField] private RectTransform _previousSearches;
    [SerializeField] private GameObject _prevSearchPrefab;
    [SerializeField] private GameObject _gun;
    [SerializeField] private float _textTypeSpeed = 5f;
    [SerializeField] private GameObject _codePadPrefab;
    private GameObject _codePadReference;
    private List<string> _interactiveWords = new List<string>();
    public bool IsCodePadActive { get => _codePadReference != null; }

    private void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
        DontDestroyOnLoad(gameObject);
        Init();
        _endText.DOFade(0, 0);
    }

    private void OnDisable()
    {
        DeInit();
    }

    private void Update()
    {
        if(IsCodePadActive && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCodePad(0, false);
        }
    }

    public void Init()
    {
        _canvasTransform = transform;
        Interactable.InteractionEvent += HandleInteractionEvent;
        Scene1Manager.OnPlayerCollectedGun += HideGun;
        Scene1Manager.OnEndScene += EndScene;
        ArticleBox.OnCloseArticle += HandleArticleCLosed;
        _blackPanel.alpha = 0;
        _dbSearchButton.onClick.RemoveAllListeners();
        _dbSearchButton.onClick.AddListener(() =>
        {
            DisplayArticle(_dbSearchField.text.ToLower());
            AddSearchItemToPreviousSearches(_dbSearchField.text);
        });
        _settingsButton.onClick.RemoveAllListeners();
        _settingsButton.onClick.AddListener(() =>
        {
            ToggleThoughtsLog(_gamePlayPanel.transform.localScale.x == 0);
            
        });
        _gamePlayPanel.transform.DOScale(0, 0);
    }

    public void DeInit()
    {
        Interactable.InteractionEvent -= HandleInteractionEvent;
        Scene1Manager.OnPlayerCollectedGun -= HideGun;
        Scene1Manager.OnEndScene -= EndScene;
        ArticleBox.OnCloseArticle -= HandleArticleCLosed;
    }

    private void AddSearchItemToPreviousSearches(string search)
    {
        Text t = Instantiate(_prevSearchPrefab, _previousSearches).GetComponent<Text>();
        t.text = search;
    }

    public void RegisterToSceneManager(ISceneManager ism)
    {
        currentSceneManager = ism;
        currentSceneManager.OnSceneEventTriggerred += HandleSceneEvent;
    }

    public void DeregisterFromSceneManager()
    {
        currentSceneManager.OnSceneEventTriggerred -= HandleSceneEvent;
        currentSceneManager = null;
    }

    private void HandleArticleCLosed(string articleName)
    {
        _dbSearchField.text = "";
    }

    public void HandleSceneEvent(SceneEventParams sep)
    {
        if (sep is SceneDialogueEventParams startSEP)
        {
            DisplayMessage(startSEP.dialogueKey, startSEP.delay);
        }
    }

    public void HandleInteractionEvent(InteractionParams ip)
    {
        if (ip is InteractionPopDialogParams ipdp)
        {
            DisplayMessage(ipdp.dialogKey, ipdp.delay);
        }
        else if(ip is InteractionCodeParams)
        {
            if((ip as InteractionCodeParams).id == 0)
            {
                ToggleCodePad(0, true, (Scene1Manager.currentQuest as CodePadQuest));
            }
        }
    }

    private void ToggleCodePad(int id, bool on, CodePadQuest q = null)
    {
        if(id == 0)
        {
            if(on)
            {
                _codePadReference = Instantiate(_codePadPrefab, transform);
                _codePadReference.transform.localScale = Vector3.zero;
                _codePadReference.transform.DOScale(1, 0.5f);
                if (q != null)
                    _codePadReference.GetComponent<CodePad>().Init(q);
            }
            else
            {
                if (DOTween.IsTweening(_codePadReference.transform))
                    return;

                _codePadReference.transform.DOScale(0, 0.5f).OnComplete(() => 
                {
                    Destroy(_codePadReference);
                    _codePadReference = null;
                });
            }
            
        }
    }

    private void HideGun()
    {
        _gun.SetActive(false);
        _gun.GetComponent<JamesClothes>().ToggleIndicator(false);
        OnGunCollected?.Invoke();
    }

    public void ToggleBlackScreen(bool on, float delay = 1f)
    {
        if (on)
            _blackPanel.DOFade(1, delay);
        else
            _blackPanel.DOFade(0, delay);
    }

    private void EndScene()
    {
        _gamePlayPanel.DOFade(0, 3f);
        _blackPanel.DOFade(1, 3f).OnComplete(()=> _endText.DOFade(1, 1f));
    }

    public void DisplayMessage(string key, float delay = 0)
    {
        Dialogue d = DialogueReader.S.GetDialogueFromText(key, DialogueReader.S.GetTextFromFolder(DialogueMapper.GetDialoguePath(key)));
        StartCoroutine(DisplayMessageCoroutine(d, delay));
    }

    private IEnumerator DisplayMessageCoroutine(Dialogue d, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject messagePanel = Instantiate(_messagePanelPrefab, _canvasTransform);
        messagePanel.tag = MessageBox.ACTIVE_TAG;
        MessageBox mb = messagePanel.GetComponent<MessageBox>();
        mb.dialogue = d;
        RectTransform messageBoxRectTransform = messagePanel.GetComponent<RectTransform>();
        messageBoxRectTransform.localScale = Vector3.zero;
        RectTransform contentPanel = messageBoxRectTransform.GetChild(1) as RectTransform;
        if(!mb.dialogue.background)
        {
            mb.GetComponent<Image>().DOFade(0, 0);
        }
        //assigning correct size and position for the message box

        float width = Camera.main.pixelWidth;
        float height = Camera.main.pixelHeight;

        Vector2 pos = new Vector2(d.topLeft.x * width, d.topLeft.y * height);
        Vector2 size = new Vector2((d.bottomRight.x - d.topLeft.x) * width, (d.topLeft.y - d.bottomRight.y) * height);
        messageBoxRectTransform.position = pos;
        if (size.x < 220)
            size.x = 220;
        if (size.y < 227)
            size.y = 227;
        messageBoxRectTransform.sizeDelta = size;


        //assigning the header
        mb.headerText = messageBoxRectTransform.GetChild(0).GetComponent<Text>();
        mb.headerText.text = d.header;
        mb.headerText.color = Color.white;
        

        //assigning the content
        mb.contentText = new Text[d.contents[0].Count];
        for (int i = 0; i < mb.contentText.Length; i++)
        {
            GameObject go = new GameObject("Line_" + i);
            go.transform.parent = contentPanel;
            mb.contentText[i] = go.AddComponent<Text>();
            mb.contentText[i].font = mb.font;
            mb.contentText[i].fontSize = d.fontSize;
            mb.contentText[i].transform.localScale = Vector3.one;
            mb.contentText[i].rectTransform.rect.Set(0, 0, contentPanel.rect.width, MessageBox.LINE_HEIGHT);
            mb.contentText[i].color = Color.white;
        }

        //if this is a multiple answer message
        if (d.answers.Length > 0)
        {
            //assigning the answers
            RectTransform answerPanel = messageBoxRectTransform.GetChild(2) as RectTransform;
            mb.answers = new Button[d.answers.Length];
            for (int i = 0; i < mb.answers.Length; i++)
            {
                GameObject go = new GameObject("Answer_" + i);
                go.transform.parent = answerPanel;
                go.AddComponent<Image>();
                mb.answers[i] = go.AddComponent<Button>();
                mb.answers[i].image.color = Color.clear;
                mb.answers[i].transform.localScale = Vector3.one;
                go.AddComponent<VerticalLayoutGroup>();
                GameObject textGO = new GameObject("Answer_" + i + "_text");
                textGO.transform.parent = mb.answers[i].transform;
                textGO.transform.localScale = Vector3.one;
                Text textComp = textGO.AddComponent<Text>();
                textComp.font = mb.font;
                textComp.fontStyle = FontStyle.Bold;
                textComp.fontSize = mb.dialogue.fontSize;
                textComp.color = Color.white;
                textComp.rectTransform.anchorMin = mb.GetComponent<RectTransform>().anchorMin;
                textComp.rectTransform.anchorMax = mb.GetComponent<RectTransform>().anchorMax;
            }
            mb.nextButton = null;
        }
        //else if this is a quick event
        else if(d.isQuickEvent)
        {
            mb.answers = null;
            if(d.quickEventKeys[0] != KeyCode.None)
            {
                _currentTimer = Instantiate(_timerPrefab, mb.transform);
                RectTransform timerRectTransform = _currentTimer.GetComponent<RectTransform>();
                timerRectTransform.localPosition = new Vector2(messageBoxRectTransform.rect.width - MessageBox.NEXT_BUTTON_X_PADDING, -messageBoxRectTransform.rect.height + MessageBox.NEXT_BUTTON_Y_PADDING);
                timerRectTransform.sizeDelta = new Vector2(MessageBox.NEXT_BUTTON_WIDTH, MessageBox.NEXT_BUTTON_HEIGHT);
            }
        }
        //else this is a message with a next button
        else
        {
            mb.answers = null;
            //assigning next button
            GameObject go = new GameObject("NextButton");
            go.transform.parent = messageBoxRectTransform;
            LayoutElement le = go.AddComponent<LayoutElement>();
            le.ignoreLayout = true;
            mb.nextButton = go.AddComponent<Button>();
            mb.nextButton.image = go.AddComponent<Image>();
            mb.nextButton.image.sprite = _arrowSprite;
            RectTransform nextButtonRectTransform = mb.nextButton.GetComponent<RectTransform>();
            nextButtonRectTransform.localPosition = new Vector2(messageBoxRectTransform.rect.width - MessageBox.NEXT_BUTTON_X_PADDING, -messageBoxRectTransform.rect.height + MessageBox.NEXT_BUTTON_Y_PADDING);
            nextButtonRectTransform.sizeDelta = new Vector2(MessageBox.NEXT_BUTTON_WIDTH, MessageBox.NEXT_BUTTON_HEIGHT);
            mb.nextButton.transform.localScale = Vector3.zero;
        }
            
        
        messageBoxRectTransform.DOScale(1, 0.5f);
        yield return new WaitUntil(() => !DOTween.IsTweening(messageBoxRectTransform));

        //creating content animation sequence
        
        
        for (int i = 0; i < mb.contentText.Length; i++)
        {
            if(d.background == true)
            {
                mb.contentText[i].DOText(d.contents[0][i], 1 / _textTypeSpeed);
                if (d.contents[0].Count > 1 && !d.isQuickEvent)
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
            }
            else
            {
                mb.contentText[i].text = d.contents[0][i];
            }
        }

        //initializing buttons
        mb.InitAnswerButtons();
        mb.UpdateNextButton();
        mb.InitQuickEvent();

        seqDisplay = DOTween.Sequence();
        //creating answer animation sequence
        if (mb.answers != null)
        {
            for (int i = 0; i < mb.answers.Length; i++)
            {
                seqDisplay.Append(mb.answers[i].GetComponentInChildren<Text>().DOText(d.answers[i], 1/_textTypeSpeed));
            }
        }

        //creating next button
        if(mb.nextButton != null)
        {
            seqDisplay.Append(mb.nextButton.transform.DOScale(1, 0.1f));
        }

        if(!string.IsNullOrEmpty(d.interactiveWord))
        {
            GameObject iWord = Instantiate(_interactiveWordPrefab, _thoughtsLog);
            CanvasGroup cg = iWord.GetComponent<CanvasGroup>();
            cg.alpha = 0;
            iWord.GetComponentInChildren<Text>().text = d.interactiveWord;
            AddInteractiveWord(d.interactiveWord);
            seqDisplay.OnComplete(() =>
            {
                cg.DOFade(1, 1f);

            });
        }

        //show the text
        seqDisplay.Play().OnComplete(() =>
        {
            if (mb.nextButton != null)
                mb.nextButton.transform.DOScale(1.2f, 0.3f).SetLoops(-1, LoopType.Yoyo);
        });


    }

    public void NextText()
    {
        StartCoroutine(NextTextCoroutine());
    }

    private IEnumerator NextTextCoroutine()
    {
        yield return new WaitForEndOfFrame();
        MessageBox mb = GetComponentInChildren<MessageBox>();
        mb.fontSize = mb.dialogue.fontSize;
        mb.currentContent++;
        int childCount = mb.transform.GetChild(1).childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(mb.transform.GetChild(1).GetChild(i).gameObject);
        }

        RectTransform contentPanel = mb.transform.GetChild(1) as RectTransform;

        if (mb.currentContent >= mb.dialogue.contents.Count)
        {
            Debug.Log("dialogue name: " + mb.dialogue.GetName());
            Debug.Log("dialogue contents count: " + mb.dialogue.contents.Count);
            Debug.LogError("current Content is out of range, This shouldn't happen!");
            Debug.Break();
        }

        mb.contentText = new Text[mb.dialogue.contents[mb.currentContent].Count];

        

        for (int i = 0; i < mb.contentText.Length; i++)
        {
            GameObject go = new GameObject("Line_" + i);
            go.transform.parent = contentPanel;
            mb.contentText[i] = go.AddComponent<Text>();
            mb.contentText[i].font = mb.font;
            mb.contentText[i].fontSize = mb.fontSize;
            mb.contentText[i].transform.localScale = Vector3.one;
            mb.contentText[i].rectTransform.rect.Set(0, 0, contentPanel.rect.width, MessageBox.LINE_HEIGHT);

        }

        //seqNext = DOTween.Sequence();
        Dialogue d = mb.dialogue;
        if(mb.nextButton != null)
        {
            if (DOTween.IsTweening(mb.nextButton.transform))
                mb.nextButton.transform.DOKill();
            mb.nextButton.transform.DOScale(0, 0.25f);
        }

        Tweener textTween = null;
        for (int i = 0; i < mb.contentText.Length; i++)
        {
            textTween = mb.contentText[i].DOText(d.contents[mb.currentContent][i], 1 / _textTypeSpeed);
            if (d.contents[mb.currentContent].Count > 1 && !d.isQuickEvent)
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
        }
        if (textTween != null)
        {
            textTween.OnComplete(() =>
            {
                if(mb.nextButton != null)
                {
                    mb.nextButton.transform.DOScale(1, 0.25f).OnComplete(() =>
                    {
                        mb.nextButton.transform.DOScale(1.2f, 0.3f).SetLoops(-1, LoopType.Yoyo);
                    });

                }
                if (!string.IsNullOrEmpty(d.interactiveWord))
                {
                    GameObject iWord = Instantiate(_interactiveWordPrefab, _thoughtsLog);
                    CanvasGroup cg = iWord.GetComponent<CanvasGroup>();
                    cg.alpha = 0;
                    iWord.GetComponentInChildren<Text>().text = d.interactiveWord;
                    cg.DOFade(1, 1f);
                }
            });
        }
            

        mb.UpdateNextButton();
        
    }

    public bool IsTweening()
    {
        if (seqNext == null || seqDisplay == null)
            return false;
        return DOTween.IsTweening(seqDisplay) || DOTween.IsTweening(seqNext);
    }

    public void DismissMessage(float delay)
    {
        StartCoroutine(DismissMessageCoroutine(delay));
    }

    public void DismissAllDialogues()
    {
        MessageBox[] openDialogues = FindObjectsOfType<MessageBox>();
        foreach (MessageBox mb in openDialogues)
        {
            mb.transform.DOScale(0, 0.2f).OnComplete(() =>
            {
                Destroy(mb.gameObject);
            });
        }
    }

    private IEnumerator DismissMessageCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject[] gos = GameObject.FindGameObjectsWithTag(MessageBox.INACTIVE_TAG);
        foreach (GameObject go in gos)
        {
            if(go.GetComponent<MessageBox>().dialogue.background)
            {
                go.transform.DOScale(0, 0.5f).OnComplete(() =>
                {
                    Button b = go.GetComponent<MessageBox>().nextButton;
                    if (b != null)
                        b.transform.DOKill();
                    Destroy(go);
                });
            }
            else
            {
                go.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() =>
                {
                    Button b = go.GetComponent<MessageBox>().nextButton;
                    if (b != null)
                        b.transform.DOKill();
                    Destroy(go);
                });
            }
        }
        
    }

    public void DismissArticle()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag(ArticleBox.INACTIVE_TAG);
        foreach (GameObject go in gos)
        {
            go.transform.DOScale(0, 0.5f).OnComplete(() =>
            {
                Destroy(go);
            });
        }
    }

    private static int GetOpenDialogueCount()
    {
        return MessageBox.GetOpenMessagesCount();
    }

    private static int GetOpenArticleCount()
    {
        return ArticleBox.GetOpenArticleCount();
    }

    public static bool IsDisplayingElements()
    {
        return S.IsCodePadActive || 
            ThoughtsLogPanel.IsMouseOverPanel() ||
            GetOpenDialogueCount() > 0 ||
            GetOpenArticleCount() > 0;
    }

    public static void UpdateTimer(float value)
    {
        _currentTimer.GetComponent<Image>().fillAmount = value;
    }

    public void DisplayArticle(string key, float delay = 0)
    {
        if(!ArticleMapper.ContainsPath(key))
        {
            key = "null";
        }
        GameObject articlePanel = Instantiate(_articlePanelPrefab, _canvasTransform);
        ArticleBox ab = articlePanel.GetComponent<ArticleBox>();
        ArticleReader.S.GetArticleBoxFromText(key, ArticleReader.S.GetTextFromFolder(ArticleMapper.GetArticlePath(key)), ref ab);
        StartCoroutine(DisplayArticleCoroutine(articlePanel, ab, delay));
    }

    private IEnumerator DisplayArticleCoroutine(GameObject articlePanel ,ArticleBox articleBox, float delay = 0)
    {
        Sequence seqArticleContent = DOTween.Sequence();
        RectTransform articleBoxRectTransform = articlePanel.GetComponent<RectTransform>();
        articleBoxRectTransform.localScale = Vector3.zero;
        //articlePanel.GetComponent<Image>().DOFade(0, 0);
        yield return new WaitForSeconds(delay);

        //Creating the ArticleBox
        float width = Camera.main.pixelWidth;
        float height = Camera.main.pixelHeight;

        Vector2 pos = new Vector2(articleBox.topLeft.x * width, articleBox.topLeft.y * height);
        Vector2 size = new Vector2((articleBox.bottomRight.x - articleBox.topLeft.x) * width, (articleBox.topLeft.y - articleBox.bottomRight.y) * height);
        articleBoxRectTransform.position = pos;
        articleBoxRectTransform.sizeDelta = size;
       
        seqArticleContent.Append(articleBoxRectTransform.DOScale(1, 0.5f));

        //Creating contents within Article Box
        width = articleBoxRectTransform.rect.width;
        height = articleBoxRectTransform.rect.height;
        for (int i = 0; i < articleBox.articles.Length; i++)
        {
            GameObject go = Instantiate(_articlePrefab, articleBoxRectTransform);
            pos.x = articleBox.articles[i].topLeft.x * width;
            pos.y = -height +  articleBox.articles[i].topLeft.y * height;
            size.x = (articleBox.articles[i].bottomRight.x - articleBox.articles[i].topLeft.x) * width;
            size.y = (articleBox.articles[i].topLeft.y - articleBox.articles[i].bottomRight.y) * height;
            RectTransform goRT = go.GetComponent<RectTransform>();
            goRT.localPosition = pos;
            goRT.sizeDelta = size;
            Rect r = goRT.rect;
            HorizontalLayoutGroup hlg = go.GetComponent<HorizontalLayoutGroup>();
            hlg.padding.left = (int)(r.width * 0.05f);
            hlg.padding.right = (int)(r.width * 0.05f);
            hlg.padding.top = (int)(r.width * 0.07f);
            hlg.padding.bottom = (int)(r.width * 0.07f);

            if (articleBox.articles[i].articleType == ArticleType.Text)
            {
                go.transform.GetChild(1).gameObject.SetActive(false);
                Text t = go.transform.GetChild(0).GetComponent<Text>();
                t.gameObject.SetActive(true);
                t.fontSize = articleBox.articles[i].fontSize;
                t.transform.localScale = Vector3.one;
                t.color = Color.white;
                foreach (string s in articleBox.articles[i].contents)
                {
                    seqArticleContent.Append(t.DOText(s, 1/_textTypeSpeed));
                }
            }
            else if(articleBox.articles[i].articleType == ArticleType.Image)
            {
                go.transform.GetChild(0).gameObject.SetActive(false);
                Image img = go.transform.GetChild(1).GetComponent<Image>();
                img.transform.localScale = Vector3.zero;
                img.gameObject.SetActive(true);
                string path = string.Concat("Articles/", articleBox.articles[i].imagePath);
                ResourceRequest request = Resources.LoadAsync<Sprite>(path);
                while (!request.isDone)
                {
                    yield return null;
                }
                img.sprite = request.asset as Sprite;
                seqArticleContent.Append(img.transform.DOScale(1, 0.1f));
            }
            if (!string.IsNullOrEmpty(articleBox.articles[i].interactiveWord))
            {
                GameObject iWord = Instantiate(_interactiveWordPrefab, _thoughtsLog);
                CanvasGroup cg = iWord.GetComponent<CanvasGroup>();
                cg.alpha = 0;
                iWord.GetComponentInChildren<Text>().text = articleBox.articles[i].interactiveWord;
                AddInteractiveWord(articleBox.articles[i].interactiveWord);
                seqArticleContent.Append(cg.DOFade(1, 1f));
            }
        }
        Transform xButton = articleBoxRectTransform.GetChild(0);
        xButton.SetAsLastSibling();
        seqArticleContent.Play();
    }

    private void ToggleThoughtsLog(bool on)
    {
        int val = on ? 1 : 0;
        _gamePlayPanel.transform.DOScale(val, 0.5f).OnStart(() => 
        {
            _settingsButton.GetComponent<OptionsButton>().ToggleButtonSpriteChange(on);
        });
    }

    public void AddInteractiveWord(string iWord)
    {
        _interactiveWords.Add(iWord);
        OnNotificationStatusChanged?.Invoke(_interactiveWords.Count > 0);
    }

    public void RemoveInteractiveWord(string iWord)
    {
        _interactiveWords.Remove(iWord);
        OnNotificationStatusChanged?.Invoke(_interactiveWords.Count > 0);
    }
}


    

