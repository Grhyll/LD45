using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    public GameObject knownCard;
    public GameObject unknownCard;

    public TextMeshProUGUI nameLabel;
    public TextMeshProUGUI manaCostLabel;
    public Image cardImage;
    public TextMeshProUGUI categoryLabel;
    public Image categoryIcon;
    public TextMeshProUGUI description;

    public GameObject creatureStats;
    public TextMeshProUGUI creatureDamage;
    public TextMeshProUGUI creatureHealth;

    FightCardsUI fightCardsUI;

    public Card card { get; private set; }

    public RectTransform rectTransform { get; private set; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetFightCardsUI(FightCardsUI _fightCardsUI)
    {
        fightCardsUI = _fightCardsUI;
    }

    public void Init(Card _card)
    {
        card = _card;

        knownCard.SetActive(true);
        unknownCard.SetActive(false);

        nameLabel.text = card.cardDefinition.displayName;
        manaCostLabel.text = card.ManaCost.ToString();
        cardImage.sprite = card.cardDefinition.sprite;

        CardCategoryInfo categoryInfo = GlobalGameManager.Instance.cardBaseDefinitionsLibrary.GetCategoryInfo(card.cardDefinition.cardCategory);
        categoryLabel.text = categoryInfo.displayName;
        categoryIcon.sprite = categoryIcon.sprite;

        description.text = card.GetDescription();

        if (card.cardDefinition.cardCategory == CardCategory.Creature)
        {
            creatureStats.SetActive(true);
            creatureDamage.text = card.Damage.ToString();
            creatureHealth.text = card.Health.ToString();
        }
        else
        {
            creatureStats.SetActive(false);
        }
    }

    public void InitUnknown()
    {
        card = null;

        knownCard.SetActive(false);
        unknownCard.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter()
    {
        if (fightCardsUI != null)
        {
            fightCardsUI.OnPointerEnter(this);
        }
    }

    public void OnPointerExit()
    {
        if (fightCardsUI != null)
        {
            fightCardsUI.OnPointerExit(this);
        }
    }

    public void OnClick()
    {
        if (fightCardsUI != null)
        {
            fightCardsUI.OnClick(this);
        }
    }
}
