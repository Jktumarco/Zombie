using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System;

public class CharactersController : MonoBehaviour
{
    public static CharactersController instance;
    public Action OnSelect;
    [SerializeField] private Inventary curInventary;
    [SerializeField] private Character curCharacter;
    [SerializeField] private Transform selectionAreaTransform;
    private Vector3 startPosition;
    [SerializeField] private List<Character> selectedCharacterList;
    [SerializeField] bool canSelection;

    public Inventary CurInventary { get => curInventary; set => curInventary = value; }
    public Character CurCharacter { get => curCharacter; set => curCharacter = value; }

    private void Awake()
    {
        instance = this;
        selectionAreaTransform.gameObject.SetActive(false);
        selectedCharacterList = new List<Character>();
        OnSelect += OnActivateInput;
       
    }
    void Update()
    {
        OnSelect.Invoke();
        if (canSelection) { MousAreaSelection(); }
    }
    void MousAreaSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {

            startPosition = UtilsClass.GetMouseWorldPosition();
            selectionAreaTransform.gameObject.SetActive(true);
        }

        if (InputsController.instance.currentMouseEvent == mouseTypeEvent.OnHold)
        {
            foreach (Character item in selectedCharacterList)
            {

                item.IsSelection = false;
                item.SetSelectedVisible(false);
                item._characterState = characterState.automaticStrike;
            }
            selectedCharacterList.Clear();
            Vector3 currentMousePosition = UtilsClass.GetMouseWorldPosition();
            Vector3 lowerLeft = new Vector3(
                Mathf.Min(startPosition.x, currentMousePosition.x),
                Mathf.Min(startPosition.y, currentMousePosition.y)
                );
            Vector3 upperRight = new Vector3(
                Mathf.Max(startPosition.x, currentMousePosition.x),
                Mathf.Max(startPosition.y, currentMousePosition.y)
                );
            selectionAreaTransform.position = lowerLeft;
            selectionAreaTransform.localScale = upperRight - lowerLeft;
        }

        if (Input.GetMouseButtonUp(0))
        {

            selectionAreaTransform.localScale = Vector3.zero;
            selectionAreaTransform.gameObject.SetActive(false);

            //selectedCharacterList.Clear();
            Collider2D[] collider2DArray = Physics2D.OverlapAreaAll(startPosition, UtilsClass.GetMouseWorldPosition());
            foreach (Collider2D collider2D in collider2DArray)
            {
                Character character = collider2D.GetComponent<Character>();
                if (character != null)
                {
                    Debug.Log(character.name);
                    selectedCharacterList.Add(character);
                    CurInventary = character.Inventary;
                }
            }
        }
    }
    void OnActivateInput()
    {
        if(selectedCharacterList == null) { return; }
        else foreach (Character character in selectedCharacterList)
            {
                character.IsSelection = true;
                character.SetSelectedVisible(true);
                character._characterState = characterState.goToPoinOrStrike;
            }
    }
}
