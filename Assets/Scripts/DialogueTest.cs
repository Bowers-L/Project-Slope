using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;
using Yarn.Unity;

public class DialogueTest : MonoBehaviour
{
    Timer dialogueProgressTimer;
    [SerializeField]
    GameObject DialogueRunnerObject;
    [SerializeField]
    GameObject DialogueView;
    DialogueRunner _dialogueRunner;
    InMemoryVariableStorage _inMemoryVariableStorage;
    DialogueAdvanceInput _dialogueInput;
    LineView _lineView;

    Timer dialogueProgressCooldownTimer;
    bool dialogueProgressCooldown = true;

    void Start()
    {
        _dialogueRunner = DialogueRunnerObject.GetComponent<DialogueRunner>();
        _inMemoryVariableStorage = DialogueRunnerObject.GetComponent<InMemoryVariableStorage>();
        _dialogueInput = DialogueRunnerObject.GetComponent<DialogueAdvanceInput>();
        _lineView = DialogueView.GetComponent<LineView>();

        _inMemoryVariableStorage.SetValue("$jamming", true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProgressDialogue();
            dialogueProgressCooldown = false;
            dialogueProgressCooldownTimer = Timer.Register(1f, () => dialogueProgressCooldown = true);
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
        if (dialogueProgressCooldown) _lineView.UserRequestedViewAdvancement();
    }
}
