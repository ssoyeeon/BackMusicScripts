using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Dialogue System/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("캐릭터 기본 정보")]
    public string characterName;
    public Color nameColor = Color.white;
    public Sprite characterPortrait;        //캐릭터 초상화 (선택사항)

    [Header("음성 설정")]
    public AudioClip aSound;            //ㅏ,ㅐ
    public AudioClip eSound;            //ㅓ,ㅔ
    public AudioClip iSound;            //ㅣ
    public AudioClip oSound;            //ㅗ,ㅛ
    public AudioClip uSound;            //ㅜ,ㅠ,ㅡ
    public AudioClip defaultSound;      //기본 소리

    [Header("음성 특성")]
    [Range(0.5f, 2f)]
    public float pitch = 1.0f;          //음성 높낮이
    [Range(0.5f, 2f)]
    public float volume = 1.0f;         //음성 크기

    [Header("타이핑 속도")]
    [Range(0.05f, 0.5f)]
    public float typingSpeed = 0.15f;          //캐릭터 별 말하는 속도//캐릭터 설명

    [Header("캐릭터 설명")]
    [TextArea(2, 4)]
    public string description;          //캐릭터 별 말하는 속도//캐릭터 설명
}
