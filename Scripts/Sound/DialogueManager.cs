using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI 요소")]
    public GameObject dialoguePanel;
    public TMP_Text characterNameText;
    public TMP_Text dialogueText;
    public Image characterPortrait;     //캐릭터 초상화

    [Header("음성 설정")]
    public AudioSource audioSource;

    [Header("구두점 음성 (고정")]
    public AudioClip dotSound;
    public AudioClip questionSound;
    public AudioClip exclamationSound;
    public AudioClip defaultPuncSound;

    [Header("대화 데이터")]
    public DialogueData currentDialogue;

    [Header("공통 설정")]
    public float defaultTypingSpeed = 0.15f;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private CharacterData currentCharacter;

    public int loading = 1;     //게임 상태가 0 
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        if (loading == 1)
        {
            SetDialogue(currentDialogue);
        }
    }

    IEnumerator LoadingScene()
    {
        loading = 2;
        TestNextDialogue();
        yield return new WaitForSeconds(3f);
        loading = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && loading != 1)
        {
            if (isTyping)
            {
                // 타이핑 중이면 즉시 완료
                StopAllCoroutines();
                CompleteCurrentLine();
            }
            else
            {
                // 다음 대화로
                NextDialogue();
            }
        }
        if (loading == 1)
        {
            StartCoroutine(LoadingScene());
        }
    }

    public void SetDialogue(DialogueData newDialogue)
    {
        currentDialogue = newDialogue;
        currentLineIndex = 0;
        StartDialogue();
    }

    void StartDialogue()
    {
        if (currentDialogue == null || currentDialogue.dialogueLines.Length == 0) return;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        if (currentLineIndex >= currentDialogue.dialogueLines.Length)
        {
            if (currentDialogue.loopDialogue)
            {
                currentLineIndex = 0;
            }
            else
            {
                EndDialogue();
                return;
            }
        }

        DialogueLine currentLine = currentDialogue.dialogueLines[currentLineIndex];
        currentCharacter = currentLine.speaker;

        if (currentCharacter != null)
        {
            SetupCharacterUI();
            float speed = currentLine.useSlowTyping ? 0.3f :
                         (currentCharacter.typingSpeed > 0 ? currentCharacter.typingSpeed : defaultTypingSpeed);

            StartCoroutine(TypeText(currentLine.message, speed));
        }
        else
        {
            Debug.LogWarning($"대화 라인 {currentLineIndex}에 캐릭터가 설정되지 않았습니다!");
        }
    }

    public void EndDialogue()
    {
        Debug.Log("대화 종료");
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    void SetupCharacterUI()
    {
        // 캐릭터 이름과 색상 설정
        characterNameText.text = currentCharacter.characterName;
        characterNameText.color = currentCharacter.nameColor;

        // 캐릭터 초상화 설정 (있는 경우)
        if (characterPortrait != null && currentCharacter.characterPortrait != null)
        {
            characterPortrait.sprite = currentCharacter.characterPortrait;
            characterPortrait.gameObject.SetActive(true);
        }
        else if (characterPortrait != null)
        {
            characterPortrait.gameObject.SetActive(false);
        }

        // 음성 설정
        audioSource.pitch = currentCharacter.pitch;
        audioSource.volume = currentCharacter.volume;

        Debug.Log($"{currentCharacter.characterName}이(가) 말합니다!");
    }

    void NextDialogue()
    {
        currentLineIndex++;
        DisplayCurrentLine();
    }

    void CompleteCurrentLine()
    {
        isTyping = false;
        DialogueLine currentLine = currentDialogue.dialogueLines[currentLineIndex];
        dialogueText.text = currentLine.message;
    }

    IEnumerator TypeText(string message, float speed)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in message)
        {
            dialogueText.text += c;
            PlayCharacterSound(c);
            yield return new WaitForSeconds(speed);
        }

        isTyping = false;
    }

    void PlayCharacterSound(char character)
    {
        if (currentCharacter == null) return;

        audioSource.Stop();

        if (char.IsLetter(character) || char.IsDigit(character))
        {
            AudioClip soundToPlay = GetVowelSound(character);

            if (soundToPlay != null)
            {
                audioSource.clip = soundToPlay;
                audioSource.Play();
                Debug.Log($"'{character}' 소리 재생 (글자/숫자)");
            }
        }
        else if (char.IsPunctuation(character))
        {
            AudioClip punctuationClip = GetPunctuationSound(character);

            if (punctuationClip != null)
            {
                audioSource.clip = punctuationClip;
                audioSource.Play();
                Debug.Log($"'{character}' 구두점 소리 재생: {punctuationClip.name}");
            }
        }
    }

    AudioClip GetVowelSound(char c)
    {
        // 숫자 처리
        if (char.IsDigit(c))
        {
            return GetNumberSound(c);
        }

        // 한글 처리
        if (c >= '가' && c <= 'R')
        {
            return GetKoreanVowelSound(c);
        }

        // 영어 처리
        if (char.IsLetter(c))
        {
            return GetEnglishVowelSound(c);
        }

        return currentCharacter.defaultSound;
    }

    AudioClip GetPunctuationSound(char punctuation)
    {
        switch (punctuation)
        {
            case '.':   // 마침표
            case ',':   // 쉼표
            case ';':   // 세미콜론
            case ':':   // 콜론
                return dotSound;

            case '?':   // 물음표
                return questionSound;

            case '!':   // 느낌표
                return exclamationSound;

            case '~':   // 물결표
            case '-':   // 하이픈
            case '"':   // 따옴표
            case '\'':  // 작은따옴표
            case '(':   // 괄호
            case ')':
            case '[':
            case ']':
            case '{':
            case '}':
            default:
                return defaultPuncSound;
        }
    }

    AudioClip GetNumberSound(char number)
    {
        // 숫자를 한국어 발음에 맞게 모음으로 매핑
        switch (number)
        {
            case '0': return currentCharacter.oSound;   // 영(yeong) → ㅕ → e
            case '1': return currentCharacter.iSound;   // 일(il) → ㅣ → i  
            case '2': return currentCharacter.iSound;   // 이(i) → ㅣ → i
            case '3': return currentCharacter.aSound;   // 삼(sam) → ㅏ → a
            case '4': return currentCharacter.aSound;   // 사(sa) → ㅏ → a
            case '5': return currentCharacter.oSound;   // 오(o) → ㅗ → o
            case '6': return currentCharacter.uSound;   // 육(yuk) → ㅜ → u
            case '7': return currentCharacter.iSound;   // 칠(chil) → ㅣ → i
            case '8': return currentCharacter.aSound;   // 팔(pal) → ㅏ → a
            case '9': return currentCharacter.uSound;   // 구(gu) → ㅜ → u
            default: return currentCharacter.defaultSound;
        }
    }

    AudioClip GetKoreanVowelSound(char korean)
    {
        int code = korean - '가';
        int vowelIndex = (code % 588) / 28;

        switch (vowelIndex)
        {
            case 0:
            case 1:
            case 2:
            case 3:  // ㅏ, ㅐ, ㅑ, ㅒ
                return currentCharacter.aSound;

            case 4:
            case 5:
            case 6:
            case 7:  // ㅓ, ㅔ, ㅕ, ㅖ
                return currentCharacter.eSound;

            case 8:
            case 12:  // ㅗ, ㅛ
                return currentCharacter.oSound;

            case 13:
            case 17:
            case 18:  // ㅜ, ㅠ, ㅡ
                return currentCharacter.uSound;

            case 20:  // ㅣ
                return currentCharacter.iSound;

            default:
                return currentCharacter.defaultSound;
        }
    }

    AudioClip GetEnglishVowelSound(char english)
    {
        char lower = char.ToLower(english);

        switch (lower)
        {
            case 'a': return currentCharacter.aSound;
            case 'e': return currentCharacter.eSound;
            case 'i': return currentCharacter.iSound;
            case 'o': return currentCharacter.oSound;
            case 'u': return currentCharacter.uSound;
            default: return currentCharacter.uSound; // 자음
        }
    }

    // 에디터에서 쉽게 테스트할 수 있는 함수
    [ContextMenu("다음 대화")]
    void TestNextDialogue()
    {
        if (!isTyping)
        {
            NextDialogue();
        }
    }
}
