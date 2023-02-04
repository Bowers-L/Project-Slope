using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityTimer;
using Yarn.Unity;

public class DialogueTest : MonoBehaviour
{
    public enum DialogueBoxType {normal, yell}

    public string name1;
    public string name2;

    [SerializeField]
    GameObject DialogueRunnerObject;
    [SerializeField]
    GameObject DialogueView;
    [SerializeField]
    GameObject DialogueBox;
    [SerializeField]
    GameObject DialogueTextObject;
    [SerializeField]
    GameObject NameTextObject;

    DialogueRunner _dialogueRunner;
    InMemoryVariableStorage _inMemoryVariableStorage;
    CustomDialogueView _lineView;
    YarnProject _yarnProject;
    Animator _dialogueBoxAnimator;

    DialogueBoxType dialogueBoxType = DialogueBoxType.normal;
    string boxType = "normal";
    Timer dialogueProgressCooldownTimer;
    bool dialogueProgressCooldown = true;

    LocalizedLine currentLine;

    void Start()
    {
        _dialogueRunner = DialogueRunnerObject.GetComponent<DialogueRunner>();
        _inMemoryVariableStorage = DialogueRunnerObject.GetComponent<InMemoryVariableStorage>();
        _lineView = DialogueView.GetComponent<CustomDialogueView>();
        _yarnProject = _dialogueRunner.yarnProject;

        _dialogueBoxAnimator = DialogueBox.GetComponent<Animator>();

        _inMemoryVariableStorage.SetValue("$jamming", true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProgressDialogue();
            dialogueProgressCooldown = false;
            dialogueProgressCooldownTimer = Timer.Register(0.5f, () => dialogueProgressCooldown = true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _inMemoryVariableStorage.SetValue("$jamming", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _inMemoryVariableStorage.SetValue("$jamming", false);
        }
    }

    void ProgressDialogue()
    {
        if (_dialogueRunner.CurrentNodeName == null || _dialogueRunner.CurrentNodeName == "")
        {
            StartNode();
        } else if (dialogueProgressCooldown) {
            _lineView.UserRequestedViewAdvancement();

            currentLine = _lineView.GetCurrentLine();

            if (currentLine.CharacterName != null && currentLine.CharacterName == name1)
            {
                Debug.Log(currentLine.RawText + ", point left.");
                DialogueBox.transform.rotation = Quaternion.Euler(0, 0, 0);
                DialogueTextObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                NameTextObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            } else if (currentLine.CharacterName == name2)
            {
                Debug.Log(currentLine.RawText + ", point right.");
                DialogueBox.transform.rotation = Quaternion.Euler(0, 180, 0);
                DialogueTextObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                NameTextObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            
            if (currentLine.Metadata != null) boxType = currentLine.Metadata[0];
            string[] boxTypeValue = boxType.Split(':');
            if (boxTypeValue[0] == "boxType")
            {
                ChangeDialogueBox(boxTypeValue[1]);
            }
        }
    }

    void ChangeDialogueBox(string boxType)
    {
        switch(boxType){
            case "normal":
                _dialogueBoxAnimator.Play("DialogueBox-Normal");
                break;
            case "yell":
                _dialogueBoxAnimator.Play("DialogueBox-Yell");
                break;
            default:
                break;
        }
    }

    public void StartNode(string node = "TestScript")
    {
        _dialogueRunner.StartDialogue(node);
        _lineView.UserRequestedViewAdvancement();
    }
}
