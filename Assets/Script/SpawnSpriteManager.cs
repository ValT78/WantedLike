using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnSpriteManager : MonoBehaviour
{
    private RectTransform zoneSpawnSprite; // Transform de la zone où les sprites seront générés

    [Header("Parameters")]
    private List<string> functions;

    private List<Color> selectedColors;
    private List<float> selectedRotations;
    private List<Vector3> selectedMovements;
    private List<float> selectedScale;


    private Color selectedTargetColor;
    private float selectedTargetRotation;
    private Vector3 selectedTargetMovement;
    private float selectedTargetScale;

    private void Awake()
    {
        functions = new List<string> { "ChooseColors", "ChooseRotations", "ChooseMovements", "ChooseScale" };
        zoneSpawnSprite = GameManager.Instance.zoneSpawnSprite;

    }


    public void ChooseTransformation(int score)
    {
        selectedColors = new List<Color>();
        selectedRotations = new List<float>();
        selectedMovements = new List<Vector3> { Vector3.zero };
        selectedScale = new List<float>();

        //Penser à rajouter du random sur le nombre de fonctions à exécuter
        int numberOfFunctionsToExecute = 0;
        if (score > 15)
        {
            numberOfFunctionsToExecute = 3;
        }
        else if (score > 10)
        {
            numberOfFunctionsToExecute = 2;
        }
        else if (score > 5)
        {
            numberOfFunctionsToExecute = 1;
        }

        numberOfFunctionsToExecute = Mathf.Clamp(numberOfFunctionsToExecute, 0, functions.Count);

        // Sélectionner des fonctions différentes au hasard
        List<int> selectedIndices = new();
        while (selectedIndices.Count < numberOfFunctionsToExecute)
        {
            int index = Random.Range(0, functions.Count);
            if (!selectedIndices.Contains(index))
            {
                selectedIndices.Add(index);
            }
        }

        // Exécuter les fonctions sélectionnées
        foreach (int index in selectedIndices)
        {
            switch (index)
            {
                case 0:
                    ChooseColors();
                    break;
                case 1:
                    ChooseRotations();
                    break;
                case 2:
                    ChooseMovements();
                    break;
                case 3:
                    ChooseScale();
                    break;
            }
        }

    }

    public void ChooseColors()
    {
        selectedColors = new List<Color> { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan };
        selectedTargetColor = selectedColors[Random.Range(0, selectedColors.Count)];
    }

    public void ChooseRotations()
    {
        selectedRotations = new List<float> { 0, 90, 180, 270 };
        selectedTargetRotation = selectedRotations[Random.Range(0, selectedRotations.Count)];
    }

    public void ChooseMovements()
    {
        selectedMovements = new List<Vector3> { Vector3.zero, Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        selectedTargetMovement = selectedMovements[Random.Range(0, selectedMovements.Count)];
    }

    public void ChooseScale()
    {
        selectedScale = new List<float> { 0.5f, 1f, 1.5f, 2f };
        selectedTargetScale = selectedScale[Random.Range(0, selectedScale.Count)];
    }


    public Color GetRandomColor()
    {
        return selectedColors[Random.Range(0, selectedColors.Count)];
    }

    public float GetRandomRotation()
    {
        return selectedRotations[Random.Range(0, selectedRotations.Count)];
    }

    public Vector3 GetRandomMovement()
    {
        return selectedMovements[Random.Range(0, selectedMovements.Count)];
    }

    public float GetRandomScale()
    {
        return selectedScale[Random.Range(0, selectedScale.Count)];
    }

    public void ApplyTransformations(GameObject spriteObject, bool isTarget, int spriteIndex) {
        spriteObject.transform.SetParent(zoneSpawnSprite);
        spriteObject.GetComponent<RectTransform>().localScale = Vector3.one;

        // Positionner les sprites dans la zone
        float x = Random.Range(zoneSpawnSprite.rect.xMin, zoneSpawnSprite.rect.xMax);
        float y = Random.Range(zoneSpawnSprite.rect.yMin, zoneSpawnSprite.rect.yMax);
        spriteObject.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);

        if (isTarget)
        {
            if (selectedColors.Count != 0)
            {
                spriteObject.GetComponent<Image>().color = selectedTargetColor;
            }
            if (selectedRotations.Count != 0)
            {
                spriteObject.transform.Rotate(Vector3.forward, selectedTargetRotation);
            }
            spriteObject.GetComponent<SpriteClickable>().Initialized(spriteIndex, selectedTargetMovement, 100, true);
            if (selectedScale.Count != 0)
            {
                spriteObject.GetComponent<RectTransform>().localScale = new Vector3(selectedTargetScale, selectedTargetScale, 1);
            }
        }
        else
        {
            if (selectedColors.Count != 0)
            {
                spriteObject.GetComponent<Image>().color = GetRandomColor();
            }
            if (selectedRotations.Count != 0)
            {
                spriteObject.GetComponent<RectTransform>().localRotation *= Quaternion.Euler(0, 0, GetRandomRotation());
            }
            spriteObject.GetComponent<SpriteClickable>().Initialized(spriteIndex, GetRandomMovement(), 100, false);
            if (selectedScale.Count != 0)
            {
                spriteObject.GetComponent<RectTransform>().localScale = new Vector3(selectedTargetScale, selectedTargetScale, 1);
            }
        }


    }


}
