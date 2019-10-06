using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGrid : MonoBehaviour
{
    public static PlayGrid Instance { get { return GlobalGameManager.Instance.grid; } }

    public FeedbacksManager gridFeedbackManager;

    GridSpot[][] spots;

    // Opti: pool that (and so many other things...)
    FightCreature fightCreatureModel;

    public FightCreature mc { get; private set; }

    public const int size = 11;
    const float margin = 10f;


    // Start is called before the first frame update
    void Start()
    {
        spots = new GridSpot[size][];
        for (int i = 0; i < size; i++)
        {
            spots[i] = new GridSpot[size];
        }

        float canvasSize = GetComponent<RectTransform>().rect.width/*.sizeDelta.x*/ - margin;

        GridSpot spotModel = GetComponentInChildren<GridSpot>();

        float spotDefaultSize = spotModel.GetComponent<RectTransform>().sizeDelta.x;
        float spotScale = canvasSize / (size * spotDefaultSize);

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GridSpot spot = spotModel;
                if (i > 0 || j > 0)
                {
                    spot = Instantiate(spotModel.gameObject, spotModel.transform.parent).GetComponent<GridSpot>();
                }
                spot.transform.localPosition = new Vector3(i + 0.5f - size / 2f, j + 0.5f - size / 2f, 0f) * canvasSize / size;
                spot.transform.localScale = Vector3.one * spotScale;
                spots[i][j] = spot;
                spot.Init(i, j);
            }
        }

        fightCreatureModel = GetComponentInChildren<FightCreature>();
        fightCreatureModel.transform.localScale = Vector3.one * spotScale;
        fightCreatureModel.gameObject.SetActive(false);

        gridFeedbackManager.transform.localScale = Vector3.one * spotScale;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public FightCreature SpawnFightCreature(Card creatureCard, GridSpot spot, bool ally)
    {
        if (spot.IsFree())
        {
            GameObject newFightCreatureObject = Instantiate(fightCreatureModel.gameObject, fightCreatureModel.transform.parent);
            newFightCreatureObject.SetActive(true);
            FightCreature result = newFightCreatureObject.GetComponent<FightCreature>();
            result.Init(creatureCard, spot, ally);
            newFightCreatureObject.transform.localPosition = spot.transform.localPosition;
            spot.OnEntityEnters(result);
            if (creatureCard.cardDefinition.cardType == CardDefinitionType.MC)
                mc = result;
            return result;
        }
        else
        {
            Debug.LogWarning("Trying to spawn fight creature on taken spot.");
            return null;
        }
    }

    public GridSpot GetSpot(int coordX, int coordY, bool onlyIfFree = false)
    {
        if (coordX >= 0 && coordX < size && coordY >= 0 && coordY < size &&
            (!onlyIfFree || spots[coordX][coordY].IsFree()))
        {
            return spots[coordX][coordY];
        }
        return null;
    }
    public bool IsFree(int coordX, int coordY)
    {
        GridSpot spot = GetSpot(coordX, coordY, true);
        return spot != null;
    }

    public void UpdateAllSpotVisuals()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                spots[i][j].UpdateVisuals();
            }
        }
    }



    public class MoveSet
    {
        GridSpot startSpot;
        List<MoveSetElement> elements = new List<MoveSetElement>();

        public void Clear()
        {
            startSpot = null;
            elements.Clear();
        }

        public void Fill(GridSpot start, int maxDistance, PlayGrid grid)
        {
            Clear();
            startSpot = start;
            elements.Add(new MoveSetElement(startSpot, null, 0));
            Explore(elements[0], 0, maxDistance, grid);
        }
        void Explore(MoveSetElement startElement, int currentDistance, int maxDistance, PlayGrid grid)
        {
            currentDistance++;
            if (currentDistance > maxDistance)
                return;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    GridSpot spot = grid.GetSpot(startElement.spot.CoordX + i, startElement.spot.CoordY + j, true);
                    if (spot != null)
                    {
                        MoveSetElement newOrUpdatedElement = TryAndAddOrUpdate(spot, startElement, currentDistance);
                        if(newOrUpdatedElement != null)
                        {
                            Explore(newOrUpdatedElement, currentDistance, maxDistance, grid);
                        }
                    }
                }
            }
        }
        MoveSetElement TryAndAddOrUpdate(GridSpot spot, MoveSetElement previousElement, int distance)
        {
            for (int s = 0; s < elements.Count; s++)
            {
                if (elements[s].spot == spot)
                {
                    if (elements[s].distance > distance ||
                        (elements[s].distance == distance && (previousElement.spot.CoordX == spot.CoordX || previousElement.spot.CoordY == spot.CoordY)))
                    {
                        elements[s].Update(previousElement, distance);
                        return elements[s];
                    }
                    return null;
                }
            }
            MoveSetElement result = new MoveSetElement(spot, previousElement, distance);
            elements.Add(result);
            return result;
        }

        public MoveSetElement GetElement(GridSpot spot, bool ignoreInitialSpot = true)
        {
            if (ignoreInitialSpot && spot == startSpot)
            {
                return null;
            }
            for (int s = 0; s < elements.Count; s++)
            {
                if (elements[s].spot == spot)
                {
                    return elements[s];
                }
            }
            return null;
        }
        public bool GetPath(GridSpot destination, ref List<GridSpot> path)
        {
            path.Clear();
            MoveSetElement destinationElement = GetElement(destination);
            if (destinationElement != null)
            {
                path.Add(destination);
                while (destinationElement.previousElement.previousElement != null)
                {
                    destinationElement = destinationElement.previousElement;
                    path.Insert(0, destinationElement.spot);
                }
                return true;
            }
            return false;
        }
        public GridSpot GetClosestSpot(GridSpot target, int maxDistance)
        {
            float closestSqrDistance = float.PositiveInfinity;
            int closestDistance = int.MaxValue;
            GridSpot bestResult = null;
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].distance <= maxDistance)
                {
                    float sqrDist = (target.transform.position - elements[i].spot.transform.position).sqrMagnitude;
                    if (sqrDist < closestSqrDistance - 0.01f ||
                        (sqrDist < closestSqrDistance + 0.01f && elements[i].distance < closestDistance))
                    {
                        closestSqrDistance = sqrDist;
                        closestDistance = elements[i].distance;
                        bestResult = elements[i].spot;
                    }
                }
            }
            return bestResult;
        }
    }
    // This will allocate a lot, ideally it should be a struct or whatever, but it comes with its own set of challenges, and there's no time for that
    public class MoveSetElement
    {
        public GridSpot spot;
        public MoveSetElement previousElement;
        public int distance;

        public MoveSetElement(GridSpot _spot, MoveSetElement _previousElement, int _distance)
        {
            spot = _spot;
            previousElement = _previousElement;
            distance = _distance;
        }
        public void Update(MoveSetElement newPreviousElement, int newDistance)
        {
            previousElement = newPreviousElement;
            distance = newDistance;
        }
    }
    public void GetMoveSet(GridSpot startSpot, int maxDistance, ref MoveSet moveSet)
    {
        moveSet.Fill(startSpot, maxDistance, this);
    }
}
