using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnSpriteManager : MonoBehaviour
{
    private RectTransform zoneSpawnSprite; // Transform de la zone où les sprites seront générés

    [Header("Difficulty rises")]
    [SerializeField] private int roundBatches;
    [SerializeField] private int firstDifficulty;
    [SerializeField] private int secondDifficulty;
    [SerializeField] private int thirdDifficulty;

    [Header("ParametersLists")]
    [SerializeField] private List<Color> chooseColors;
    [SerializeField] private List<float> chooseRotations;
    [SerializeField] private List<Vector3> chooseMovements;
    [SerializeField] private List<float> chooseScale;




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
        zoneSpawnSprite = GameManager.Instance.zoneSpawnSprite;
        chooseColors = new List<Color> { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan };
        chooseRotations = new List<float> { 0, 180, 90, 270, 45, 315, 225, 135 };
        chooseMovements = new List<Vector3> { Vector3.zero, Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        chooseScale = new List<float> { 0.5f, 1f, 1.5f, 2f };

    }


    public void ChooseTransformation(int score)
    {
        print(score);
        if (score==0 || score % roundBatches == 0)
        {

            selectedColors = new List<Color>();
            selectedRotations = new List<float>();
            selectedMovements = new List<Vector3> { Vector3.zero };
            selectedScale = new List<float>();

            //Penser à rajouter du random sur le nombre de fonctions à exécuter
            int numberOfFunctionsToExecute = 0;
            if (score > thirdDifficulty)
            {
                numberOfFunctionsToExecute = Random.Range(0, 4);
            }
            else if (score > secondDifficulty)
            {
                numberOfFunctionsToExecute = Random.Range(0, 3);
            }
            else if (score > firstDifficulty)
            {
                numberOfFunctionsToExecute = Random.Range(0, 2);
            }

            List<string> functions = new() { "ChooseColors", "ChooseRotations", "ChooseMovements", "ChooseScale" };

            // Coefficients de pondération pour chaque fonction
            List<float> weights = new() { 1f, 1f, 1f, 1f };

            numberOfFunctionsToExecute = Mathf.Clamp(numberOfFunctionsToExecute, 0, functions.Count);



            List<int> selectedIndices = new();
            List<float> cumulativeWeights = new();
            float totalWeight = 0f;

            // Calculer les poids cumulatifs
            foreach (float weight in weights)
            {
                totalWeight += weight;
                cumulativeWeights.Add(totalWeight);
            }

            while (selectedIndices.Count < numberOfFunctionsToExecute)
            {
                float randomValue = Random.Range(0f, totalWeight);
                for (int i = 0; i < cumulativeWeights.Count; i++)
                {
                    if (randomValue < cumulativeWeights[i])
                    {
                        if (!selectedIndices.Contains(i))
                        {
                            selectedIndices.Add(i);
                        }
                        break;
                    }
                }
            }

            // Exécuter les fonctions sélectionnées
            foreach (int index in selectedIndices)
            {
                switch (index)
                {
                    case 0:
                        ChooseColors(score);
                        break;
                    case 1:
                        ChooseRotations(score);
                        break;
                    case 2:
                        ChooseMovements(score);
                        break;
                    case 3:
                        ChooseScale(score);
                        break;
                }
            }
        }
    }

    public void ChooseColors(int score)
    {
        int numberOfColors = Mathf.Clamp(1+ score / 20, 0, chooseColors.Count);
        numberOfColors = Random.Range(1, numberOfColors + 1);
        selectedColors = new List<Color>();
        for (int i = 0; i < numberOfColors; i++)
        {
            Color randomColor;
            do
            {
                randomColor = chooseColors[Random.Range(0, chooseColors.Count)];
            } while (selectedColors.Contains(randomColor));
            selectedColors.Add(randomColor);
        }

        if(Random.value<GenerateExponentialRandom(30/score))
        {
            selectedTargetColor = Color.white;
        }
        else
        {
            selectedTargetColor = selectedColors[Random.Range(0, selectedColors.Count)];
        }



    }

    public void ChooseRotations(int score)
    {
        int numberOfRotations = Mathf.Clamp(1+ score / 15, 0, 5);
        selectedRotations = new List<float>();
        if(numberOfRotations == 5)
        {
            //Générer 8 valeurs d'angles aléatoires
            for (int i = 0; i < 8; i++)
            {
                float randomRotation;
                do
                {
                    randomRotation = Random.Range(0, 360);
                } while (selectedRotations.Contains(randomRotation));
                selectedRotations.Add(randomRotation);
            }
        }
        else
        {
            for (int i = 0; i < numberOfRotations*2; i++)
            {
                selectedRotations.Add(chooseRotations[i]);
            }
        }
        

        if (Random.value < GenerateExponentialRandom(30 / score))
        {
            selectedTargetRotation = 0f;
        }
        else
        {
            selectedTargetRotation = selectedRotations[Random.Range(0, selectedRotations.Count)];
        }
        
    }

    public void ChooseMovements(int score)
    {
        selectedMovements = new List<Vector3> {Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        selectedTargetMovement = selectedMovements[Random.Range(0, selectedMovements.Count)];
    }

    public void ChooseScale(int score)
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

        //1/2 chance de rebondir sur les bords
        bool isBouncing = false;
        if (Random.Range(0, 2) == 0)
        {
            isBouncing = true;
        }
        
        bool isSinusoidal = false;
        if (Random.Range(0, 2) == 0)
        {
            isSinusoidal = true;
        }

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
            spriteObject.GetComponent<SpriteClickable>().Initialized(spriteIndex, selectedTargetMovement, 100, isBouncing, isSinusoidal, 4, 8);
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
            spriteObject.GetComponent<SpriteClickable>().Initialized(spriteIndex, GetRandomMovement(), 100, isBouncing, isSinusoidal,4, 8);
            if (selectedScale.Count != 0)
            {
                spriteObject.GetComponent<RectTransform>().localScale = new Vector3(selectedTargetScale, selectedTargetScale, 1);
            }
        }


    }

    float GenerateExponentialRandom(float lambda)
    {
        float u = Random.value; // Génère un nombre aléatoire uniforme entre 0 et 1
        float result = 1 - Mathf.Exp(-u / lambda); // Appliquer ta fonction avec la nouvelle abscisse
        return result;
    }


}
