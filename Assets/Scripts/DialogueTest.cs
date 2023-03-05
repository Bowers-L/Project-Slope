using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityTimer;
using Yarn.Unity;

public class DialogueTest : MonoBehaviour
{
    public UnityEvent endNodeSignal;

    public enum DialogueBoxType {normal, yell}

    public string name1;
    public string name2;

    [SerializeField] GameObject DialogueRunnerObject;
    [SerializeField] GameObject DialogueView;
    [SerializeField] GameObject DialogueBox;
    [SerializeField] GameObject DialogueTextObject;
    [SerializeField] GameObject char1;
    [SerializeField] GameObject char2;
    [SerializeField] Sprite[] emoteList;
    [SerializeField] float delayPerMessage = 4f;

    DialogueRunner _dialogueRunner;
    InMemoryVariableStorage _inMemoryVariableStorage;
    CustomDialogueView _lineView;
    TextMeshProUGUI _text;
    YarnProject _yarnProject;
    Animator _dialogueBoxAnimator;

    DialogueBoxType dialogueBoxType = DialogueBoxType.normal;
    string boxType = "normal";

    SpriteRenderer char1sprite;
    SpriteRenderer char2sprite;
    Animator char1anim;
    Animator char2anim;
    RectTransform dialogueBoxTransform;
    string emoteType = "default";

    //Timer dialogueProgressCooldownTimer;
    Timer dialogueProgressTimer;
    bool dialogueProgressCooldown = true;

    Timer bounceTimer;

    LocalizedLine currentLine;

    bool endOfNode;

    void Start()
    {
        _dialogueRunner = DialogueRunnerObject.GetComponent<DialogueRunner>();
        _inMemoryVariableStorage = DialogueRunnerObject.GetComponent<InMemoryVariableStorage>();
        _lineView = DialogueView.GetComponent<CustomDialogueView>();
        _yarnProject = _dialogueRunner.yarnProject;
        _text = DialogueTextObject.GetComponent<TextMeshProUGUI>();

        _dialogueBoxAnimator = DialogueBox.GetComponent<Animator>();
        dialogueBoxTransform = DialogueBox.GetComponent<RectTransform>();

        char1anim = char1.GetComponent<Animator>();
        char2anim = char2.GetComponent<Animator>();

        char1sprite = char1.GetComponent<SpriteRenderer>();
        char2sprite = char2.GetComponent<SpriteRenderer>();

        _inMemoryVariableStorage.SetValue("$jamming", true);
    }

    void Update()
    {
        //Debug Input
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartNode();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _inMemoryVariableStorage.SetValue("$jamming", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _inMemoryVariableStorage.SetValue("$jamming", false);
        }
        */
    }

    void ProgressDialogue()
    {
        if (_dialogueRunner.CurrentNodeName == null || _dialogueRunner.CurrentNodeName == "")
        {
            if (dialogueProgressTimer != null) dialogueProgressTimer.Cancel();
        } else {
            _dialogueBoxAnimator.SetTrigger("bounce");
            bounceTimer = Timer.Register(0.055555f, () => NextLine());
        }
    }

    public void NextLine()
    {
        _lineView.UserRequestedViewAdvancement();
        ProcessDialogue();
    }

    void ProcessDialogue()
    {
        currentLine = _lineView.GetCurrentLine();
        int fontSizeValue = 63 - (currentLine.RawText.Length/4);
        //Debug.Log("Font Size: " + fontSizeValue);
        if (fontSizeValue > 50) fontSizeValue = 50;
        if (fontSizeValue < 36) fontSizeValue = 36;
        _text.fontSize = fontSizeValue;

        ProcessSpeaker();
        
        if (currentLine.Metadata != null) {
            foreach (string s in currentLine.Metadata)
            {
                if (s.Split(':')[0] == "boxtype") 
                {
                    ChangeDialogueBox(s.Split(':')[1]);
                }
                if (s.Split(':')[0] == "emotetype")
                {
                    ChangeEmote(s.Split(':')[1]);
                }
            }
        } else {
            //Debug.Log("No metadata found for current line");
        }
    }

    void ProcessSpeaker()
    {
        if (currentLine.CharacterName != null && currentLine.CharacterName == name1)
        {
            if (DialogueBox != null) DialogueBox.transform.rotation = Quaternion.Euler(0, 0, 0);
            if (DialogueBox != null) DialogueTextObject.transform.rotation = Quaternion.Euler(0, 0, 0);

            if (DialogueBox != null) dialogueBoxTransform.localPosition = new Vector3(-80, dialogueBoxTransform.localPosition.y, dialogueBoxTransform.localPosition.z);

            if (char1sprite != null) char1sprite.color = new Color(1, 1, 1, 1);
            if (char2sprite != null) char2sprite.color = new Color(0.6f, 0.6f, 0.6f, 1);
        } 
        else if (currentLine.CharacterName == name2)
        {
            if (DialogueBox != null) DialogueBox.transform.rotation = Quaternion.Euler(0, 180, 0);
            if (DialogueBox != null) DialogueTextObject.transform.rotation = Quaternion.Euler(0, 0, 0);

            if (DialogueBox != null) dialogueBoxTransform.localPosition = new Vector3(80, dialogueBoxTransform.localPosition.y, dialogueBoxTransform.localPosition.z);

            if (char1sprite != null) char1sprite.color = new Color(0.6f, 0.6f, 0.6f, 1);
            if (char2sprite != null) char2sprite.color = new Color(1, 1, 1, 1);
        }
    }

    void ChangeDialogueBox(string boxType)
    {
        //Debug.Log("ChangeDialogueBox(): " + boxType);
        switch(boxType){
            case "normal":
                _dialogueBoxAnimator.Play("DialogueBox-Normal");
                break;
            case "yell":
                _dialogueBoxAnimator.Play("DialogueBox-Yell");
                break;
            case "think":
                _dialogueBoxAnimator.Play("DialogueBox-Think");
                break;
            case "sad":
                _dialogueBoxAnimator.Play("DialogueBox-Sad");
                break;
            default:
                break;
        }
    }

    void ChangeEmote(string emoteType)
    {
        switch(emoteType) {
            case "evanhappy":
                if (char1sprite != null) char1sprite.sprite = emoteList[0];
                break;
            case "vivianhappy":
                if (char2sprite != null) char2sprite.sprite = emoteList[3];
                break;
            case "pouty":
                if (char2sprite != null) char2sprite.sprite = emoteList[5];
                break;
            case "angry":
                if (char2sprite != null) char2sprite.sprite = emoteList[4];
                break;
            case "idiot":
                if (char1sprite != null) char1sprite.sprite = emoteList[1];
                break;
            case "nervous":
                if (char1sprite != null) char1sprite.sprite = emoteList[2];
                break;
            default:
                break;
        }
    }

    public void StartNode(string node = "Intro")
    {
        endOfNode = false;
        if (_lineView != null) _lineView.canvasGroupEnabled = true;
        if (_dialogueRunner != null) _dialogueRunner.StartDialogue(node);
        if (_dialogueBoxAnimator != null) _dialogueBoxAnimator.SetTrigger("bounce");
        ProcessDialogue();

        dialogueProgressTimer = Timer.Register(
            duration: delayPerMessage,
            isLooped: true,
            onComplete: () => 
            {
                ProgressDialogue();
            }
        );
    }

    public void EndNode()
    {
        Debug.Log("ENDED NODE");
        endOfNode = true;
        if (dialogueProgressTimer != null) dialogueProgressTimer.Cancel();
        endNodeSignal.Invoke();
        if (_lineView != null) _lineView.canvasGroupEnabled = false;
        if (_lineView != null) _lineView.SetCanvasAlpha(0);

        //EndNode event calls after processing the next line, which appears to be whatever the last line is.
        //Can't find another way to figure out when it's the last line of a node, so here's a jank solution for this race condition:
        Timer.Register(0.06f, () => {
            if (char1sprite != null) char1sprite.color = new Color(1, 1, 1, 1);
            if (char2sprite != null) char2sprite.color = new Color(1, 1, 1, 1);
            if (dialogueBoxTransform != null) dialogueBoxTransform.localPosition = new Vector3(0, dialogueBoxTransform.localPosition.y, dialogueBoxTransform.localPosition.z);
        });
    }
}
