using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class WordController : MonoBehaviour
{
    public static WordController Instance;

    public List<CanvasGroup> canvasGroupList;
    public List<GameObject> m_generatedGOs;
    public List<ItemSlot> m_slotList1;
    public List<ItemSlot> m_slotList2;
    public List<ItemSlot> m_slotListSingle;

    [SerializeField] private Transform slotTemplate;
    [SerializeField] private Transform itemTemplate;
    [SerializeField] private Transform wordContainer;

    private Sprite m_sprWord1;
    private Sprite m_sprWord2;
    private Sprite m_sprWordSingle;

    private Sprite m_sprWordBg1;
    private Sprite m_sprWordBg2;
    private Sprite m_sprWordBgSingle;

    [SerializeField] private Image m_Bg1;
    [SerializeField] private Image m_Bg2;
    [SerializeField] private Image m_BgSingle;

    private string m_word1;
    private string m_word2;
    private string m_wordSingle;

    private float m_screenWidth;
    private float m_screenHeight;

    private float m_space = 16;

    [SerializeField]
    private TMP_InputField input;

    void Awake()
    {
        if (Instance != null)
            GameObject.Destroy(Instance);
        else
            Instance = this;

        DontDestroyOnLoad(this);

        //Canvas canvas = FindObjectOfType<Canvas>();

        m_screenHeight = 414;
        m_screenWidth = 900;
        //m_screenHeight = canvas.GetComponent<RectTransform>().rect.height;
        //m_screenWidth = canvas.GetComponent<RectTransform>().rect.width;

        Logger.Log("WordController-Awake", string.Format("Screen width: {0}, and height: {1}", m_screenWidth, m_screenHeight));
    }

    public void Initialize(
        string word1,
        string word2,
        string bgColor1,
        string bgColor2,
        Sprite sprWord1,
        Sprite sprWord2,
        Sprite sprWordBg1,
        Sprite sprWordBg2)
    {
        m_word1 = word1;
        m_word2 = word2;

        m_sprWord1 = sprWord1;
        m_sprWord2 = sprWord2;

        m_sprWordBg1 = sprWordBg1;
        m_sprWordBg2 = sprWordBg2;

        Color colorBg1, colorBg2;
        ColorUtility.TryParseHtmlString(bgColor1, out colorBg1);
        ColorUtility.TryParseHtmlString(bgColor2, out colorBg2);

        m_Bg1.gameObject.SetActive(true);
        m_Bg2.gameObject.SetActive(true);
        m_BgSingle.gameObject.SetActive(false);
        m_Bg1.color = colorBg1;
        m_Bg2.color = colorBg2;

        string wordShuffled1, wordShuffled2;
        ShuffleWords(word1, word2, out wordShuffled1, out wordShuffled2);
        CreateWords(wordShuffled1, wordShuffled2);
    }

    public void Initialize(
        string word,
        string bgColor,
        Sprite sprWord,
        Sprite sprWordBg)
    {
        m_wordSingle = word;

        m_sprWordSingle = sprWord;

        m_sprWordBgSingle = sprWordBg;

        Color colorBg;
        ColorUtility.TryParseHtmlString(bgColor, out colorBg);

        m_Bg1.gameObject.SetActive(false);
        m_Bg2.gameObject.SetActive(false);
        m_BgSingle.gameObject.SetActive(true);
        m_BgSingle.color = colorBg;

        string wordShuffled;
        ShuffleWord(word, out wordShuffled);
        CreateWords(wordShuffled);
    }

    void ShuffleWords(string word1, string word2, out string shuffledWord1, out string shuffledWord2)
    {
        System.Random num = new System.Random();

        string concatWord = word1 + word2;

        shuffledWord1 = word1;
        shuffledWord2 = word2;

        while (shuffledWord1 == word1 || shuffledWord2 == word2)
        {
            concatWord = new string(concatWord.ToCharArray().
                        OrderBy(s => (num.Next(2) % 2) == 0).ToArray());

            shuffledWord1 = concatWord.Substring(0, word1.Length);
            shuffledWord2 = concatWord.Substring(word1.Length, word2.Length);
        }
    }

    void ShuffleWord(string word, out string shuffledWord)
    {
        System.Random num = new System.Random();

        shuffledWord = word;

        while (shuffledWord == word)
        {
            shuffledWord = new string(word.ToCharArray().
                        OrderBy(s => (num.Next(2) % 2) == 0).ToArray());
        }
    }

    void CreateWords(string word1, string word2)
    {
        slotTemplate.gameObject.SetActive(false);
        itemTemplate.gameObject.SetActive(false);

        float slotWidth = slotTemplate.GetComponent<RectTransform>().rect.width;
        float slotHeight = slotTemplate.GetComponent<RectTransform>().rect.height;
        int word1Len = word1.Length;
        int word2Len = word2.Length;

        int wordLen = word1Len;
        if (word2.Length > word1.Length)
        {
            wordLen = word2Len;
        }

        float containerWidth = wordLen * slotWidth + m_space * (wordLen - 1);
        float containerOffsetX = (m_screenWidth - containerWidth) / 2 + slotWidth / 2;
        float containerOffsetY1 = -m_screenHeight / 2 + slotHeight/2 + 5;
        float containerOffsetY2 = -m_screenHeight / 2 - slotHeight/2 - 17;

        CreateWord(word1, containerOffsetX, containerOffsetY1, m_sprWordBg1, m_sprWord1, m_slotList1);
        CreateWord(word2, containerOffsetX, containerOffsetY2, m_sprWordBg2, m_sprWord2, m_slotList2);
    }

    void CreateWords(string word)
    {
        slotTemplate.gameObject.SetActive(false);
        itemTemplate.gameObject.SetActive(false);

        float slotWidth = slotTemplate.GetComponent<RectTransform>().rect.width;
        float slotHeight = slotTemplate.GetComponent<RectTransform>().rect.height;
        int wordLen = word.Length;

        float containerWidth = wordLen * slotWidth + m_space * (wordLen - 1);
        float containerOffsetX = (m_screenWidth - containerWidth) / 2 + slotWidth / 2;
        float containerOffsetY = -m_screenHeight / 2;

        CreateWord(word, containerOffsetX, containerOffsetY, m_sprWordBgSingle, m_sprWordSingle, m_slotListSingle);
    }

    private void CreateWord(string word, float containerOffsetX, float containerOffsetY, Sprite sprWordBg, Sprite sprWord, List<ItemSlot> slotList)
    {
        float slotWidth = slotTemplate.GetComponent<RectTransform>().rect.width;

        for (int i = 0; i < word.Length; i++)
        {
            string letter = word.Substring(i, 1);

            // Instantiate slot
            Transform slotTransform = Instantiate(slotTemplate, wordContainer);
            RectTransform slotRectTransform = slotTransform.GetComponent<RectTransform>();
            slotRectTransform.anchoredPosition = new Vector2(containerOffsetX + (slotWidth + m_space) * i, containerOffsetY);
            slotTransform.gameObject.SetActive(true);
            slotList.Add(slotTransform.GetComponent<ItemSlot>());

            // Instantiate item
            Transform itemTransform = Instantiate(itemTemplate, wordContainer);
            slotTransform.GetComponent<ItemSlot>().SetItem(itemTransform.gameObject, false, false);
            itemTransform.GetComponent<Item>().SetLetter(letter);
            itemTransform.gameObject.SetActive(true);
            slotTransform.GetComponent<ItemSlot>().SetConfig(sprWordBg, sprWord, i);

            m_generatedGOs.Add(slotTransform.gameObject);
            m_generatedGOs.Add(itemTransform.gameObject);

            canvasGroupList.Add(itemTransform.GetComponent<CanvasGroup>());
        }
    }

    public void OnStartDrag()
    {
        Debug.Log("OnStartDrag");
        if (Level.Instance.m_gameState.m_gameType == GameType.SingleWord)
        {
            for (int i = 0; i < m_slotListSingle.Count; i++)
            {
                m_slotListSingle[i].SetInactiveFrame();
            }
        }
        else
        {
            for (int i = 0; i < m_slotList1.Count; i++)
            {
                Debug.Log("m_slotList1[i].SetInactiveFrame");
                m_slotList1[i].SetInactiveFrame();
            }

            for (int i = 0; i < m_slotList2.Count; i++)
            {
                m_slotList2[i].SetInactiveFrame();
            }
        }
    }

    public void OnEndDrag()
    {
        if (Level.Instance.m_gameState.m_gameType == GameType.SingleWord)
        {
            for (int i = 0; i < m_slotListSingle.Count; i++)
            {
                m_slotListSingle[i].ResetFrame();
            }
        }
        else
        {
            for (int i = 0; i < m_slotList1.Count; i++)
            {
                m_slotList1[i].ResetFrame();
            }

            for (int i = 0; i < m_slotList2.Count; i++)
            {
                m_slotList2[i].ResetFrame();
            }
        }
    }

    public void EvaluateMove()
    {
        if (Level.Instance.m_gameState.m_gameType == GameType.SingleWord)
        {
            EvaluateSingle();
        }
        else
        {
            EvaluateDouble();
        }
    }

    void EvaluateDouble()
    {
        string word1 = "", word2 = "";
        bool succeeded = true;
        for (int i = 0; i < m_slotList1.Count; i++)
        {
            word1 += m_slotList1[i].GetLetter();
            if (!m_slotList1[i].CheckLetter(m_word1))
            {
                succeeded = false;
            }
        }

        for (int i = 0; i < m_slotList2.Count; i++)
        {
            word2 += m_slotList2[i].GetLetter();
            if (!m_slotList2[i].CheckLetter(m_word2))
            {
                succeeded = false;
            }
        }

        if (succeeded)
        {
            Level.Instance.OnSucceed();
        }
        Logger.Log("EvaluateSingle", $"Compared {m_word1} with {word1} and {m_word2} with {word2}.");
    }

    void EvaluateSingle()
    {
        string word = "";
        bool succeeded = true;
        for (int i = 0; i < m_slotListSingle.Count; i++)
        {
            word += m_slotListSingle[i].GetLetter();
            if (!m_slotListSingle[i].CheckLetter(m_wordSingle))
            {
                succeeded = false;
            }
        }

        if (succeeded)
        {
            Level.Instance.OnSucceed();
        }
        Logger.Log("EvaluateSingle", $"Compared {m_wordSingle} with {word}.");
    }

    public void DisableDragging()
    {
        for (int i = 0; i < m_generatedGOs.Count; i++)
        {
            if (m_generatedGOs[i] != null)
            {
                Item item = m_generatedGOs[i].GetComponent<Item>();
                if (item != null)
                {
                    item.Lock();
                }
            }
        }
    }

    public void Cleanup()
    {
        for (int i = 0; i < m_generatedGOs.Count; i++)
        {
            if (m_generatedGOs[i] != null)
            {
                Destroy(m_generatedGOs[i]);
            }
        }
        m_slotList1.Clear();
        m_slotList2.Clear();
        m_slotListSingle.Clear();
        m_generatedGOs.Clear();
        canvasGroupList.Clear();
    }
}
