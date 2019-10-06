using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightCardsUI : MonoBehaviour
{
    public GameObject selectedCardRaycastBlocker;

    public float showedCardScaleMultiplier = 1.5f;
    const float margin = 10;

    CardUI cardModel;
    float defaultScale;
    float defaultY;

    float availableWidth;
    float downscaledWidth;

    List<CardUI> hand = new List<CardUI>();

    CardUI showedCardUI;
    //int showedCardUIInitialSiblingIndex;
    bool showedCardIsSelected;

    private void Awake()
    {
        FightManager.OnCardPilesChanged += OnCardPilesChanged;
    }

    private void OnDestroy()
    {
        FightManager.OnCardPilesChanged -= OnCardPilesChanged;
    }

    // Start is called before the first frame update
    void Start()
    {
        cardModel = GetComponentInChildren<CardUI>();
        cardModel.SetFightCardsUI(this);
        defaultScale = cardModel.transform.localScale.x;
        defaultY = cardModel.transform.localPosition.y;

        availableWidth = GetComponent<RectTransform>().rect.width - margin;
        downscaledWidth = cardModel.GetComponent<RectTransform>().sizeDelta.x * defaultScale;

        cardModel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(CardUI cardUI)
    {
        if (showedCardUI != null && showedCardIsSelected)
            return;

        if (showedCardUI != null && showedCardUI != cardUI)
        {
            UnshowCard();
        }
        ShowCard(cardUI);
    }
    public void OnPointerExit(CardUI cardUI)
    {
        if (showedCardUI == cardUI && !showedCardIsSelected)
        {
            UnshowCard();
        }
    }
    public void OnClick(CardUI cardUI)
    {
        if (cardUI != showedCardUI)
        {
            Debug.LogWarning("Warning: clicked card isn't being shown.");
            UnshowCard();
            ShowCard(cardUI);
        }
        showedCardIsSelected = !showedCardIsSelected;
        if (!showedCardIsSelected)
        {
            UnshowCard();
            GlobalGameManager.Instance.fightManager.OnUnselectedHandCard();
        }
        else
        {
            selectedCardRaycastBlocker.SetActive(true);
            Debug.Log("Play card!");
            GlobalGameManager.Instance.fightManager.OnSelectedHandCard(cardUI.card);
        }
    }
    public void OnSelectedCardRaycastBlocker()
    {
        OnClick(showedCardUI);
    }

    void ShowCard(CardUI card)
    {
        Vector3 targetLocalPosition = new Vector3(card.transform.localPosition.x, card.rectTransform.sizeDelta.y * defaultScale * showedCardScaleMultiplier / 2f);
        float showedCardHalfWidth = card.rectTransform.sizeDelta.x * defaultScale * showedCardScaleMultiplier / 2f;
        targetLocalPosition.x = Mathf.Clamp(targetLocalPosition.x, -availableWidth / 2f + showedCardHalfWidth, availableWidth / 2f - showedCardHalfWidth);
        card.transform.SetAsLastSibling();
        card.transform.localScale = Vector3.one * defaultScale * showedCardScaleMultiplier;
        card.transform.localPosition = targetLocalPosition;
        showedCardUI = card;
    }
    void UnshowCard()
    {
        showedCardUI = null;
        showedCardIsSelected = false;
        FoldCards();
    }

    void OnCardPilesChanged()
    {
        List<Card> currentHand = GlobalGameManager.Instance.fightManager.hand;
        for (int i = 0; i < hand.Count; i++)
        {
            if (!currentHand.Contains(hand[i].card))
            {
                CardUI cardUI = hand[i];
                hand.RemoveAt(i);
                i--;
                Destroy(cardUI.gameObject);
            }
        }
        for (int i = 0; i < currentHand.Count; i++)
        {
            bool alreadyThere = false;
            for (int j = 0; j < hand.Count; j++)
            {
                if (hand[j].card == currentHand[i])
                {
                    alreadyThere = true;
                    break;
                }
            }
            if (!alreadyThere)
            {
                CardUI newCardUI = Instantiate(cardModel.gameObject, cardModel.transform.parent).GetComponent<CardUI>();
                newCardUI.Init(currentHand[i]);
                newCardUI.SetFightCardsUI(this);
                newCardUI.gameObject.name = currentHand[i].cardDefinition.name;
                newCardUI.gameObject.SetActive(true);
                hand.Add(newCardUI);
            }
        }

        FoldCards();
    }

    void FoldCards()
    {
        if (hand.Count > 0)
        {
            //TODO: Handle currently scaled card
            float firstCardX = (-(hand.Count - 1) / 2f) * downscaledWidth;
            float cardSpace = downscaledWidth;
            if (downscaledWidth * hand.Count > availableWidth - 2f)
            {
                firstCardX = -availableWidth / 2f + downscaledWidth / 2f;
                float centerSpace = availableWidth - downscaledWidth;
                cardSpace = centerSpace / (hand.Count - 1);
            }
            for (int i = 0; i < hand.Count; i++)
            {
                hand[i].transform.localScale = Vector3.one * defaultScale;
                hand[i].transform.localPosition = new Vector3(firstCardX + i * cardSpace, defaultY, 0);
                hand[i].transform.SetSiblingIndex(i);
            }
        }
        selectedCardRaycastBlocker.SetActive(false);
    }
}
