using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    public Action OnInventaryUpdate;
    public Action<float> OnDamage;
    public Action<float> OnHealth;
    public Action<string> OnDeleteItemFromAllInventary;
    public Action<string> OnDestroyItemFromAllInventary;
    public Action<string> OnAddItemToMainInventary;
    public Action<string> OnAddItemToQuickInventary;
    public Action OnUseHealth;
    public Action OnDie;
    public Action OnStart;
    public Action OnUpdateStatusPlace;
    public Action OnCheckZeroAmountItem;
    static public UI_Controller instance;

    [SerializeField] private GameObject uI_QuickInventaryDownPanelRoot;
    [SerializeField] private GameObject uI_InventaryPanelRoot;
    [SerializeField] private GameObject uI_QuickInventaryPanelRoot;
    [SerializeField] private Canvas canvasDie;
    [SerializeField] Image backPanelBlockRay;
    [SerializeField] List<UI_Base> windowUI;

    [SerializeField] InventaryPlace[] inventaryPlaces;

    public GameObject UI_QuickInventaryDownPanelRoot { get => uI_QuickInventaryDownPanelRoot;}
    public GameObject UI_InventaryPanelRoot { get => uI_InventaryPanelRoot;}
    public GameObject UI_QuickInventaryPanelRoot { get => uI_QuickInventaryPanelRoot;}

    private void OnEnable()
    {
        if (instance == null) { instance = this; }
        
    }
    private void Awake()
    {
        OnDie += DieWindow;
    }
    private void Start()
    {
        inventaryPlaces = UI_QuickInventaryDownPanelRoot.gameObject.GetComponentsInChildren<InventaryPlace>();
    }
    public void PanelBlockRaycastTarget(bool targetRay) { backPanelBlockRay.raycastTarget = targetRay; }
    public void DieWindow()
    {
        canvasDie.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        OnDie -= DieWindow;
    }

}
