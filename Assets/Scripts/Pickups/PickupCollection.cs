using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCollection : MonoBehaviour
{
    public enum CollectionState
    {
        Consulting, 
        Creating, 
        Improving,
    }

    public GameObject sellButton;

    CollectionState collectionState = CollectionState.Consulting;

    PickupUI pickupModel;

    List<PickupUI> pickups = new List<PickupUI>();

    float availableSpace;
    float pickupSize;

    PickupUI selectedPickup;

    const int maxPickupsAmount = 12;

    CreateCardUI createCardUI { get { return GlobalGameManager.Instance.deckBuildingManager.deckbuildingUI.createCardUI; } }
    ImprovingCardUI improvingCardUI { get { return GlobalGameManager.Instance.deckBuildingManager.deckbuildingUI.improvingCardUI; } }

    // Start is called before the first frame update
    void Start()
    {
        pickupModel = GetComponentInChildren<PickupUI>();
        pickupSize = pickupModel.GetComponent<RectTransform>().rect.width;
        pickupModel.gameObject.SetActive(false);

        availableSpace = GetComponent<RectTransform>().rect.width;

        sellButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCurrentPickupsAmount()
    {
        return pickups.Count;
    }

    public void OnNewState(CollectionState newState)
    {
        UnselectSelectedPickup();
        collectionState = newState;
        UpdatePickupStates();
    }

    public void EarnPickup(PickupDefinition pickup)
    {
        if (pickups.Count >= maxPickupsAmount)
        {
            //Debug.LogError("Error: can't add pickup");
            return;
        }
        PickupUI newPickupUI = Instantiate(pickupModel.gameObject, pickupModel.transform.parent).GetComponent<PickupUI>();
        newPickupUI.gameObject.SetActive(true);
        newPickupUI.Init(pickup, this);
        pickups.Add(newPickupUI);
        Reorganize();
    }
    public void OnPickupConsumed(PickupUI pickup)
    {
        pickups.Remove(pickup);
        Destroy(pickup.gameObject);
        Reorganize();
    }

    public void UpdatePickupStates()
    {
        switch (collectionState)
        {
            case CollectionState.Consulting:
                for (int i = 0; i < pickups.Count; i++)
                {
                    pickups[i].EnableSelection();
                }
                break;

            case CollectionState.Creating:
                for (int i = 0; i < pickups.Count; i++)
                {
                    if (createCardUI.CanSelectPickup(pickups[i].pickupDefinition))
                    {
                        pickups[i].EnableSelection();
                    }
                    else
                    {
                        pickups[i].DisableSelection();
                    }
                }
                break;

            case CollectionState.Improving:
                for (int i = 0; i < pickups.Count; i++)
                {
                    if (improvingCardUI.CanSelectPickup(pickups[i]))
                    {
                        pickups[i].EnableSelection();
                    }
                    else
                    {
                        pickups[i].DisableSelection();
                    }
                }
                break;
        }
    }

    void Reorganize()
    {
        float spaceBetweenPickups = availableSpace / (maxPickupsAmount / 2);
        for (int i = 0; i < pickups.Count; i++)
        {
            pickups[i].transform.localPosition = new Vector3(-availableSpace / 2f + pickupSize / 2f + (i % (maxPickupsAmount / 2)) * spaceBetweenPickups,
                (pickups.Count > maxPickupsAmount / 2) ? ((i >= maxPickupsAmount / 2) ? -1f : 1f) * spaceBetweenPickups / 2f : 0f);
        }
    }

    public void OnPickupClicked(PickupUI pickup)
    {
        switch (collectionState)
        {
            case CollectionState.Consulting:
                if (pickup == selectedPickup)
                {
                    UnselectSelectedPickup();
                }
                else
                {
                    if (selectedPickup != null)
                        selectedPickup.Unselect();

                    selectedPickup = pickup;
                    selectedPickup.Select();
                    sellButton.SetActive(true);
                }
                break;

            case CollectionState.Creating:
                createCardUI.OnSelectedPickup(pickup);
                break;

            case CollectionState.Improving:
                improvingCardUI.OnSelectedPickup(pickup);
                break;
        }
    }
    void UnselectSelectedPickup()
    {
        if (selectedPickup != null)
        {
            selectedPickup.Unselect();
            sellButton.SetActive(false);
            selectedPickup = null;
        }
    }

    public void OnSellButton()
    {
        if (selectedPickup != null)
        {
            GlobalGameManager.Instance.CoinsAmount += 1;
            pickups.Remove(selectedPickup);
            Destroy(selectedPickup.gameObject);
            selectedPickup = null;
            sellButton.SetActive(false);
            Reorganize();
        }
    }
}
