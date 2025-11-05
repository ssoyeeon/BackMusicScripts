using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [Header("대화 설정")]
    public CharacterData speaker;       //말하는 캐릭터

    [TextArea(2, 4)]
    public string message;              //대화 내용

    [Header("특수 효과")]
    public bool useSlowTyping;      //천천히 타이핑
    public bool useEmphasis;        //강조 효과
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("대화 정보")]
    public string dialogueTitle;        //대화 제목 

    [Header("대화 내용")]
    public DialogueLine[] dialogueLines;

    [Header("설정")]
    public bool loopDialogue = true;        //대화 반복 여부

    [Header("설명")]
    [TextArea(2, 3)]
    public string description;      //대화의 설명
}
